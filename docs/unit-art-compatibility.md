# Everkin unit art compatibility — 2026-10-02

> **Superseded in part (2026-10-09):** rear art is scrapped. Each creature now has one image; the card front shows a portrait crop and the back the whole creature (production.md §2.3, Creature art). The front/rear findings below are historical.

## Result

Split units 18–27 into separate front (`_f`) and rear (`_r`) PNGs. There are now 56 active images representing 28 complete pairs. Unit 13 is absent; existing IDs were preserved. The ten original combined files are retained in `images/kin/sources/`.

Extraction followed a varying boundary through the central gap, rather than cutting across the midpoint. No opaque pixels (alpha > 128) lie on those boundaries. The total alpha sum was verified against each original, and the resulting contact sheet was visually checked. Extracted views retain source resolution and transparency and are centered on canvases with the original dimensions; no scaling or repainting was performed. Existing separate files were unchanged.

## Assessment

Usable for a prototype, but not yet visually consistent enough for finished unit cards.

| Area | Current compatibility |
| --- | --- |
| Naming/pairing | Good: every active ID has one front and one rear. ID 13 is missing. |
| File format | Good: all active files are PNG with real alpha transparency. |
| Canvas dimensions | Mixed: 46 files are 1086 × 1448; units 23–27 contribute 10 files at 1122 × 1402. |
| Visual style | Broadly coherent faceted fantasy creatures, with differences in polygon density, proportions, and detail. Units 18–27 tend to look smaller and simpler. |
| Framing and apparent size | Needs work: significant subject height spans roughly 40–99% of canvas height. Extracted units 18–27 occupy roughly 40–51%; units 1–9 mostly occupy 86–99%. Shape differences account for some of this, so blindly equalizing heights would distort the intended relative sizes. |
| Front/rear alignment | Needs work: there is no shared ground line or consistent subject anchor. Swapping files can cause visible movement. Some pairs have noticeable scale/projection differences, including 10, 12, 19, and 20. |
| Edge quality | Needs cleanup: full-resolution inspection of 19 and 24 reveals colored halos/speckles and some pale residue. Nonzero-alpha outliers elsewhere extend far outside the main silhouettes. Dark card backgrounds make these defects more noticeable. |
| View direction | Mixed: views are three-quarter/side angles rather than a standardized camera. Pairs such as 9, 12, and 16 read more like opposite side views than clear front/back turns; manually approve their intended use. |
| Distinctiveness | Units 12 and 16 have very similar blue shark silhouettes and palettes; distinct card labels or stronger art differences may be useful. |

## Recommended next pass

1. Clean alpha-edge residue before using automatic visible bounds for layout.
2. Choose one canvas size and a consistent inset art region; preserve aspect ratio.
3. Set scale and ground/center anchors per unit, using the same scale for both views of each pair. Do not stretch each image to fill a card.
4. Check the art at the actual card size on both light and dark backgrounds, and verify front/rear switching.

This is an asset inspection, not an in-engine card test. No card scene was rendered. The contact sheet is `docs/unit-art-contact-sheet.png`.

## Per-file measurements

Bounds below include pixels with alpha >= 128; disconnected opaque residue can still affect these bounds. Transparency percentage counts alpha = 0. The contact sheet uses a common preview frame, so it is for comparing framing and pairing rather than judging exact source aspect ratios.

| File | Canvas | Visible bounds size | Height occupancy | Bounds top-left | Fully transparent |
| --- | --- | --- | --- | --- | --- |
| 1_f.png | 1086 x 1448 | 1028 x 1359 | 93,9% | 34, 46 | 51,6% |
| 1_r.png | 1086 x 1448 | 1019 x 1383 | 95,5% | 33, 21 | 51,6% |
| 2_f.png | 1086 x 1448 | 962 x 1293 | 89,3% | 65, 36 | 61,7% |
| 2_r.png | 1086 x 1448 | 941 x 1313 | 90,7% | 106, 41 | 62,5% |
| 3_f.png | 1086 x 1448 | 991 x 1293 | 89,3% | 79, 64 | 61,8% |
| 3_r.png | 1086 x 1448 | 975 x 1293 | 89,3% | 56, 69 | 61,6% |
| 4_f.png | 1086 x 1448 | 787 x 1417 | 97,9% | 161, 15 | 70,9% |
| 4_r.png | 1086 x 1448 | 805 x 1427 | 98,5% | 199, 4 | 70,4% |
| 5_f.png | 1086 x 1448 | 1013 x 1249 | 86,3% | 55, 116 | 51,7% |
| 5_r.png | 1086 x 1448 | 1020 x 1243 | 85,8% | 56, 109 | 52,6% |
| 6_f.png | 1086 x 1448 | 1001 x 1405 | 97% | 58, 22 | 61,4% |
| 6_r.png | 1086 x 1448 | 978 x 1418 | 97,9% | 86, 7 | 59,7% |
| 7_f.png | 1086 x 1448 | 1000 x 1405 | 97% | 53, 22 | 68,7% |
| 7_r.png | 1086 x 1448 | 990 x 1411 | 97,4% | 49, 18 | 67,1% |
| 8_f.png | 1086 x 1448 | 997 x 1405 | 97% | 49, 16 | 55,6% |
| 8_r.png | 1086 x 1448 | 987 x 1408 | 97,2% | 81, 10 | 57,5% |
| 9_f.png | 1086 x 1448 | 1020 x 1423 | 98,3% | 34, 12 | 64,8% |
| 9_r.png | 1086 x 1448 | 1038 x 1432 | 98,9% | 28, 6 | 62,3% |
| 10_f.png | 1086 x 1448 | 1068 x 1024 | 70,7% | 14, 244 | 58,5% |
| 10_r.png | 1086 x 1448 | 1054 x 955 | 66% | 17, 258 | 64,7% |
| 11_f.png | 1086 x 1448 | 1042 x 1035 | 71,5% | 29, 196 | 59,6% |
| 11_r.png | 1086 x 1448 | 1041 x 1051 | 72,6% | 27, 194 | 58,8% |
| 12_f.png | 1086 x 1448 | 1041 x 1000 | 69,1% | 24, 240 | 68,5% |
| 12_r.png | 1086 x 1448 | 1043 x 908 | 62,7% | 32, 267 | 72,2% |
| 14_f.png | 1086 x 1448 | 1027 x 994 | 68,6% | 37, 220 | 62,1% |
| 14_r.png | 1086 x 1448 | 1022 x 975 | 67,3% | 34, 227 | 61,8% |
| 15_f.png | 1086 x 1448 | 1053 x 913 | 63,1% | 18, 289 | 66,3% |
| 15_r.png | 1086 x 1448 | 1024 x 907 | 62,6% | 46, 283 | 66,6% |
| 16_f.png | 1086 x 1448 | 1032 x 1113 | 76,9% | 29, 173 | 70,1% |
| 16_r.png | 1086 x 1448 | 1046 x 1124 | 77,6% | 25, 170 | 71,1% |
| 17_f.png | 1086 x 1448 | 1062 x 1042 | 72% | 14, 284 | 76,2% |
| 17_r.png | 1086 x 1448 | 1056 x 1063 | 73,4% | 16, 275 | 76,5% |
| 18_f.png | 1086 x 1448 | 951 x 692 | 47,8% | 112, 373 | 74,8% |
| 18_r.png | 1086 x 1448 | 977 x 685 | 47,3% | 54, 377 | 76,7% |
| 19_f.png | 1086 x 1448 | 1039 x 659 | 45,5% | 37, 425 | 84,2% |
| 19_r.png | 1086 x 1448 | 1058 x 578 | 39,9% | 10, 411 | 84,4% |
| 20_f.png | 1086 x 1448 | 991 x 727 | 50,2% | 51, 358 | 79,2% |
| 20_r.png | 1086 x 1448 | 984 x 658 | 45,4% | 46, 387 | 81,2% |
| 21_f.png | 1086 x 1448 | 1026 x 658 | 45,4% | 34, 398 | 76,4% |
| 21_r.png | 1086 x 1448 | 1017 x 654 | 45,2% | 51, 402 | 76,9% |
| 22_f.png | 1086 x 1448 | 848 x 714 | 49,3% | 161, 371 | 88,5% |
| 22_r.png | 1086 x 1448 | 884 x 700 | 48,3% | 155, 369 | 87,4% |
| 23_f.png | 1122 x 1402 | 1082 x 699 | 49,9% | 18, 355 | 78,6% |
| 23_r.png | 1122 x 1402 | 1084 x 695 | 49,6% | 18, 350 | 78,8% |
| 24_f.png | 1122 x 1402 | 716 x 697 | 49,7% | 240, 356 | 87,9% |
| 24_r.png | 1122 x 1402 | 698 x 691 | 49,3% | 230, 347 | 88,8% |
| 25_f.png | 1122 x 1402 | 1012 x 683 | 48,7% | 53, 358 | 77,7% |
| 25_r.png | 1122 x 1402 | 977 x 664 | 47,4% | 54, 349 | 80,4% |
| 26_f.png | 1122 x 1402 | 596 x 689 | 49,1% | 264, 356 | 91,6% |
| 26_r.png | 1122 x 1402 | 574 x 689 | 49,1% | 305, 356 | 91,1% |
| 27_f.png | 1122 x 1402 | 1013 x 716 | 51,1% | 77, 345 | 79,8% |
| 27_r.png | 1122 x 1402 | 1019 x 664 | 47,4% | 65, 357 | 79,9% |
| 28_f.png | 1086 x 1448 | 907 x 1364 | 94,2% | 117, 39 | 64,6% |
| 28_r.png | 1086 x 1448 | 888 x 1418 | 97,9% | 108, 14 | 64,1% |
| 29_f.png | 1086 x 1448 | 957 x 1439 | 99,4% | 64, 5 | 58,7% |
| 29_r.png | 1086 x 1448 | 983 x 1417 | 97,9% | 72, 12 | 56,8% |
