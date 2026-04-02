"""
Generate HomeScene menu button background (9-slice friendly) and hover glow PNGs
matched to dark sci-fi / cyan accent lounge aesthetic.
"""
from __future__ import annotations

import math
import os

import numpy as np
from PIL import Image

OUT_DIR = os.path.join(os.path.dirname(__file__), "..", "Assets", "Images", "HomeScene")


def smoothstep(edge0: float, edge1: float, x: np.ndarray) -> np.ndarray:
    t = np.clip((x - edge0) / (edge1 - edge0 + 1e-9), 0.0, 1.0)
    return t * t * (3.0 - 2.0 * t)


def sdf_rounded_rect(
    px: np.ndarray, py: np.ndarray, cx: float, cy: float, hw: float, hh: float, rad: float
) -> np.ndarray:
    """Signed distance to rounded rectangle (negative inside). px,py = pixel coords."""
    x = px - cx
    y = py - cy
    bx = hw - rad
    by = hh - rad
    qx = np.abs(x) - bx
    qy = np.abs(y) - by
    ox = np.maximum(qx, 0.0)
    oy = np.maximum(qy, 0.0)
    outside = np.sqrt(ox * ox + oy * oy)
    inside = np.minimum(np.maximum(qx, qy), 0.0)
    return outside + inside - rad


def make_button_bg(w: int = 512, h: int = 128) -> Image.Image:
    yy, xx = np.mgrid[0:h, 0:w].astype(np.float32)
    cx, cy = (w - 1) * 0.5, (h - 1) * 0.5
    hw, hh = w * 0.5 - 0.5, h * 0.5 - 0.5
    rad = min(26.0, min(hw, hh) * 0.45)

    d = sdf_rounded_rect(xx, yy, cx, cy, hw, hh, rad)
    # Anti-aliased mask
    alpha = smoothstep(0.6, -0.6, d)

    # Vertical gradient fill (slightly brighter toward top — ceiling light)
    t = yy / max(h - 1, 1)
    r = (18 + t * 14 + (1 - t) * 8).astype(np.float32)
    g = (32 + t * 22 + (1 - t) * 12).astype(np.float32)
    b = (52 + t * 28 + (1 - t) * 18).astype(np.float32)

    # Inner vignette
    nx = (xx - cx) / max(hw, 1)
    ny = (yy - cy) / max(hh, 1)
    vig = 1.0 - 0.22 * (nx * nx + ny * ny)
    r *= vig
    g *= vig
    b *= vig

    # Cyan rim + bottom accent (neon strip feel)
    rim = np.exp(-(d * d) / (2.0 * 1.15 * 1.15)) * (d <= 1.5)
    rim = np.clip(rim, 0, 1)
    r = r + rim * 55
    g = g + rim * 120
    b = b + rim * 95

    # Specular line along top inside edge
    top_glow = np.exp(-((yy - (cy - hh * 0.65)) ** 2) / (2.0 * 4.5 * 4.5)) * (d < 0)
    top_glow *= np.clip(1.0 - np.abs(xx - cx) / (hw * 0.92), 0, 1)
    r = r + top_glow * 35
    g = g + top_glow * 55
    b = b + top_glow * 70

    rgba = np.zeros((h, w, 4), dtype=np.uint8)
    rgba[..., 0] = np.clip(r, 0, 255).astype(np.uint8)
    rgba[..., 1] = np.clip(g, 0, 255).astype(np.uint8)
    rgba[..., 2] = np.clip(b, 0, 255).astype(np.uint8)
    rgba[..., 3] = np.clip(alpha * 255, 0, 255).astype(np.uint8)

    return Image.fromarray(rgba, "RGBA")


def make_hover_glow(w: int = 1024, h: int = 220) -> Image.Image:
    yy, xx = np.mgrid[0:h, 0:w].astype(np.float32)
    cx, cy = (w - 1) * 0.5, (h - 1) * 0.5
    nx = (xx - cx) / (w * 0.5)
    ny = (yy - cy) / (h * 0.5)

    # Soft horizontal capsule
    gx = np.exp(-(nx * nx) * 5.8)
    gy = np.exp(-(ny * ny) * 14.0)
    base = gx * gy

    # Brighter core band
    core = np.exp(-(nx * nx) * 18.0) * np.exp(-(ny * ny) * 22.0)

    a = np.clip((base * 0.55 + core * 0.9), 0, 1)
    # Feather edges of texture
    edge = np.minimum(np.minimum(xx, w - 1 - xx), np.minimum(yy, h - 1 - yy))
    a *= smoothstep(0.0, 24.0, edge)

    # Cyan-white color
    r = 160 + core * 95
    g = 235 + core * 20
    b = 255
    rgba = np.zeros((h, w, 4), dtype=np.uint8)
    rgba[..., 0] = np.clip(r * a + 40 * (1 - a), 0, 255).astype(np.uint8)
    rgba[..., 1] = np.clip(g * a + 220 * (1 - a), 0, 255).astype(np.uint8)
    rgba[..., 2] = 255
    rgba[..., 3] = np.clip(a * 200, 0, 255).astype(np.uint8)

    # Subtle sparkle (deterministic)
    rng = np.random.default_rng(42)
    sparks = rng.random((h, w)) < 0.0012
    rgba[sparks, 0] = 255
    rgba[sparks, 1] = 255
    rgba[sparks, 2] = 255
    rgba[sparks, 3] = np.maximum(rgba[sparks, 3], 140)

    return Image.fromarray(rgba, "RGBA")


def main() -> None:
    os.makedirs(OUT_DIR, exist_ok=True)
    bg_path = os.path.join(OUT_DIR, "HomeSceneButtonBg.png")
    glow_path = os.path.join(OUT_DIR, "HomeSceneButtonHoverGlow.png")

    make_button_bg().save(bg_path, "PNG", optimize=True)
    make_hover_glow().save(glow_path, "PNG", optimize=True)
    print("Wrote", bg_path)
    print("Wrote", glow_path)


if __name__ == "__main__":
    main()
