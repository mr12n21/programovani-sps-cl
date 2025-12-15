#!/bin/bash
set -euo pipefail

# Spustí Python skript pro generování souborů, kompresi (GZIP/BZIP2/XZ)
# a vytvoření tabulky s výsledky do vysledek.md

python3 "$(dirname "$0")/generate_and_compress.py"

echo "Hotovo. Viz inf/komprese/vysledek.md a komprimované soubory (.gz/.bz2/.xz)."
