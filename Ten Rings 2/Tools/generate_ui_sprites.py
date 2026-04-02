"""
Regenerate UI PNGs into Assets/UI. Run:
  py -3.13 Tools/generate_ui_sprites.py
"""
from __future__ import annotations

import math
import struct
import zlib
from pathlib import Path


def write_png(path: Path, rgba: bytes, width: int, height: int) -> None:
    def chunk(tag: bytes, data: bytes) -> bytes:
        return struct.pack(">I", len(data)) + tag + data + struct.pack(">I", zlib.crc32(tag + data) & 0xFFFFFFFF)

    raw = b""
    stride = width * 4
    for y in range(height):
        raw += b"\x00" + rgba[y * stride : (y + 1) * stride]
    png = b"\x89PNG\r\n\x1a\n"
    png += chunk(b"IHDR", struct.pack(">IIBBBBB", width, height, 8, 6, 0, 0, 0))
    png += chunk(b"IDAT", zlib.compress(raw, 9))
    png += chunk(b"IEND", b"")
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_bytes(png)


def rounded_rect_mask(w: int, h: int, radius: float) -> list[float]:
    r = min(radius, w / 2 - 0.5, h / 2 - 0.5)
    out = []
    for y in range(h):
        for x in range(w):
            cx = max(r, min(w - 1 - r, x))
            cy = max(r, min(h - 1 - r, y))
            dx = x - cx
            dy = y - cy
            d = math.sqrt(dx * dx + dy * dy) - r + 0.5
            a = 1.0 - max(0.0, min(1.0, d))
            out.append(a)
    return out


def panel_gradient(w: int, h: int, border: int) -> bytes:
    mask = rounded_rect_mask(w, h, 24.0)
    buf = bytearray(w * h * 4)
    for y in range(h):
        for x in range(w):
            i = y * w + x
            a = mask[i]
            if a <= 0:
                continue
            t = y / max(1, h - 1)
            br = int(200 + 25 * (1 - t))
            bg = int(224 + 20 * t)
            bb = int(245 + 10 * (1 - t))
            edge = min(x, y, w - 1 - x, h - 1 - y)
            if edge < border:
                boost = (border - edge) / border
                br = min(255, int(br + 40 * boost))
                bg = min(255, int(bg + 35 * boost))
                bb = min(255, int(bb + 15 * boost))
            alpha = int(0.55 * 255 * a + 0.25 * 255 * a * (edge / border if edge < border else 0.3))
            alpha = max(0, min(255, alpha))
            j = i * 4
            buf[j] = br
            buf[j + 1] = bg
            buf[j + 2] = bb
            buf[j + 3] = alpha
    return bytes(buf)


def solid_rounded(w: int, h: int, rgb: tuple[int, int, int], base_alpha: float, radius: float) -> bytes:
    mask = rounded_rect_mask(w, h, radius)
    buf = bytearray(w * h * 4)
    r, g, b = rgb
    for i, m in enumerate(mask):
        j = i * 4
        buf[j] = r
        buf[j + 1] = g
        buf[j + 2] = b
        buf[j + 3] = int(max(0, min(255, base_alpha * 255 * m)))
    return bytes(buf)


def ring_icon(size: int) -> bytes:
    buf = bytearray(size * size * 4)
    cx = cy = (size - 1) / 2
    r_outer = size * 0.38
    r_inner = size * 0.26
    for y in range(size):
        for x in range(size):
            dx = x - cx
            dy = y - cy
            d = math.sqrt(dx * dx + dy * dy)
            in_ring = r_inner <= d <= r_outer
            if in_ring:
                a = 1.0
            elif d < r_inner:
                a = max(0.0, 1.0 - (r_inner - d) / 2.0)
            elif d > r_outer:
                a = max(0.0, 1.0 - (d - r_outer) / 2.0)
            else:
                a = 0.0
            ai = int(255 * max(0, min(1, a)))
            if ai < 12:
                continue
            j = (y * size + x) * 4
            buf[j] = 220
            buf[j + 1] = 245
            buf[j + 2] = 255
            buf[j + 3] = ai
    return bytes(buf)


def main() -> None:
    root = Path(__file__).resolve().parent.parent / "Assets" / "UI"
    w = h = 256
    write_png(root / "UIPanel_Frost.png", panel_gradient(w, h, 64), w, h)

    bw, bh = 256, 128
    write_png(root / "UIButton_Frost.png", solid_rounded(bw, bh, (180, 210, 240), 0.35, 20), bw, bh)

    gw, gh = 256, 256
    write_png(root / "UIGlow_Soft.png", solid_rounded(gw, gh, (255, 255, 255), 0.12, 28), gw, gh)

    isize = 256
    write_png(root / "UIIcon_RingOutline.png", ring_icon(isize), isize, isize)

    dw, dh = 512, 8
    div = bytearray(dw * dh * 4)
    for x in range(dw):
        a = int(120 + 80 * math.sin(x * 0.08))
        for y in range(dh):
            j = (y * dw + x) * 4
            div[j] = 200
            div[j + 1] = 230
            div[j + 2] = 255
            div[j + 3] = min(255, a + y * 8)
    write_png(root / "UIDivider_Soft.png", bytes(div), dw, dh)

    print("Wrote PNGs to", root)


if __name__ == "__main__":
    main()
