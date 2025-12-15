# Komprese a entropie

Generování souborů, komprese přes GZIP/BZIP2/XZ a tabulka s výsledky.
GUI aplikace pro výpočet Entropie, Redundance (ASCII/Unicode) a vyhodnocení Kraftovy nerovnosti.

## Rychlé spuštění

```bash
# vygeneruje 3 soubory, zkomprimuje je (gz/bz2/xz) a aktualizuje tabulku
bash inf/komprese/test.sh

# spustí GUI aplikaci
python3 inf/komprese/entropy_app.py
```

## Vytvoření spustitelného programu (volitelné)

```bash
# nainstalujte PyInstaller, pokud není
python3 -m pip install --user pyinstaller

# vytvořte binárku (Linux)
pyinstaller --onefile inf/komprese/entropy_app.py

# výsledný spustitelný soubor bude ve složce dist/entropy_app
./dist/entropy_app
```

## Výstupy

- Původní soubory: `inf/komprese/200.txt`, `inf/komprese/wikiped.txt`, `inf/komprese/a200.txt`
- Komprimované soubory: `*.gz`, `*.bz2`, `*.xz` ve `inf/komprese/`
- Tabulka: `inf/komprese/vysledek.md`
- Aplikace: `inf/komprese/entropy_app.py` (nebo binárka přes PyInstaller)