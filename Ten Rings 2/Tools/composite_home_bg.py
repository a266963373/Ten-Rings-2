"""Composite protagonist onto Home scene background. Preserves space-only art in HomeScene_BG_SpaceOnly.png."""
import os
import shutil

import numpy as np
from PIL import Image

DIR = r"d:\Unity\Ten-Rings-2\Ten Rings 2\Assets\Images\HomeScene"
BG_OUT = os.path.join(DIR, "HomeScene_BG_SpaceHub.png")
SPACE_ONLY = os.path.join(DIR, "HomeScene_BG_SpaceOnly.png")
CHAR = os.path.join(DIR, "Protagonist1_Suling_NoBG.png")

KEY = np.array([199, 199, 199], dtype=np.int16)
TOL = 18


def main() -> None:
    if not os.path.isfile(SPACE_ONLY):
        shutil.copy2(BG_OUT, SPACE_ONLY)

    bg = np.array(Image.open(SPACE_ONLY).convert("RGB"))
    char = np.array(Image.open(CHAR).convert("RGB"))
    if bg.shape != char.shape:
        raise SystemExit(f"Size mismatch: bg {bg.shape} vs char {char.shape}")

    diff = np.abs(char.astype(np.int16) - KEY)
    is_bg = (diff[:, :, 0] <= TOL) & (diff[:, :, 1] <= TOL) & (diff[:, :, 2] <= TOL)
    alpha = (~is_bg).astype(np.float32)[..., None]

    comp = (char.astype(np.float32) * alpha + bg.astype(np.float32) * (1.0 - alpha)).astype(np.uint8)
    Image.fromarray(comp, "RGB").save(BG_OUT, "PNG", optimize=True)
    print("Wrote", BG_OUT, "(base:", SPACE_ONLY, ")")


if __name__ == "__main__":
    main()
