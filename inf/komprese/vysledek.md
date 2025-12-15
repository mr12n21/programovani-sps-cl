# Komprese a entropie

| Soubor | Původní (B) | GZIP (B) | GZIP % | BZIP2 (B) | BZIP2 % | XZ (B) | XZ % | Entropie (bit/znak) | Redundance ASCII | Redundance Unicode |
|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|
| 200.txt | 200 | 32 | 84.00 | 39 | 80.50 | 72 | 64.00 | 0.0000 | 8.0000 | 18.1817 |
| wikiped.txt | 601 | 369 | 38.60 | 383 | 36.27 | 428 | 28.79 | 4.6853 | 3.3147 | 13.4964 |
| a200.txt | 200 | 224 | -12.00 | 265 | -32.50 | 260 | -30.00 | 6.1755 | 1.8245 | 12.0062 |