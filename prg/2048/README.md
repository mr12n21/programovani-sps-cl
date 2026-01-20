# Hra 2048 - Tahová Strategie

Implementace hry 2048 podle zadání: **Hra 2048 jako Tahová strategie pro dva hráče** s vlastním systémem interakce zvířat.

## Popis Projektu

Toto je **Python aplikace** implementující hru 2048 s modifikacemi:
- **Dva hráči**: jeden ovládá psy (štěně → pes → vlk), druhý kočky (kotě → kočka → lev)
- **Hrací pole**: čtvercová matice 4×4
- **Cíl**: Jako první vytvořit vlka (pro psy) nebo lva (pro kočky)

## Architektura Aplikace

### Soubory:

1. **`animal.py`** - Abstraktní třída `Animal` a konkrétní třídy `Dog` a `Cat`
   - Implementace evoluce zvířat
   - Operátory pro slučování (`+`, `-`)
   - Hierarchie: štěně→pes→vlk, kotě→kočka→lev

2. **`board.py`** - Třída `GameBoard` řídící hrací desku
   - Pohyb zvířat (nahoru, dolů, vlevo, vpravo)
   - Slučování zvířat podle pravidel
   - Detekce konce hry
   - Systém vrácení tahů (undo)

3. **`game.py`** - Třída `Game` řídící logiku hry
   - Správa hráčů a jejich tahů
   - Kontrola vítězství
   - Zobrazení stavu hry

4. **`main.py`** - Hlavní program s terminálovým rozhraním
   - Interaktivní ovládání
   - Nápověda a pravidla

5. **`test_game.py`** - Unit testy
   - Testy evoluce zvířat
   - Testy slučování
   - Testy herní logiky

6. **`README.md`** - Tento soubor

## Pravidla Hry

### Slučování Zvířat:
1. **Stejný typ + stejná úroveň** → evoluce (vyšší úroveň)
   - Štěně + štěně = Pes
   - Pes + pes = Vlk
   - Kotě + kotě = Kočka
   - Kočka + kočka = Lev

2. **Různé typy + stejná úroveň** → nic se neděje (zůstanou jak jsou)

3. **Různá úroveň** → vyšší úroveň „sní" nižší

### Pohyb:
- Hráči se střídají v tazích
- Tah = posun všech zvířat v jednom směru + eventuální slučování
- Po každém tahu se přidá nové zvíře (úroveň 1)

### Cíl:
- Vytvořit **Vlka** (pro hráče s psy) nebo **Lva** (pro hráče s kočkami)
- Pokud jsou všechna místa obsazena a nelze provést tah: vyhraje hráč s vyšší úrovní

## Spuštění Programu

### Předpoklady:
- Python 3.7+
- Žádné externí závislosti

### Spuštění hry:
```bash
python3 main.py
```

### Příkazy v Terminálu:
```
up                    - Posun zvířat nahoru
down                  - Posun zvířat dolů
left                  - Posun zvířat vlevo
right                 - Posun zvířat vpravo
undo                  - Vrátit poslední tah
help                  - Zobrazit nápovědu
rules                 - Zobrazit pravidla
quit / exit           - Konec hry
```

## Spuštění Testů

```bash
python3 -m unittest test_game.py -v
```

## Příklad Hry

```
==================================================
╔════════════════════════════════════════════════╗
║         HRA 2048 - Tahová Strategie           ║
║        Psi vs. Kočky: Boj o dominanci        ║
╚════════════════════════════════════════════════╝
==================================================

========================================
[  Pes   ][  ----  ][  ----  ][  ----  ]
[  ----  ][  Kotě  ][  ----  ][  ----  ]
[  ----  ][  ----  ][  ----  ][  ----  ]
[  ----  ][  ----  ][  ----  ][  Kotě  ]
========================================

Psi (nejvyšší úroveň): 2/3
Kočky (nejvyšší úroveň): 1/3
Celkový počet tahů: 0
Na tahu: Psi

Psi, zadej tah (up/down/left/right): right
```

## Implementované Funkce

✅ **Základní mechaniky:**
- [x] Deska 4×4 s zvířaty
- [x] Pohyb v 4 směrech
- [x] Slučování zvířat
- [x] Generování nových zvířat

✅ **Logika hry:**
- [x] Detekce konce hry
- [x] Detekce vítězství
- [x] Správa tahů dvou hráčů
- [x] Undo funkce

✅ **Uživatelské rozhraní:**
- [x] Zobrazení desky v terminálu
- [x] Skóre a stav hry
- [x] Interaktivní ovládání
- [x] Nápověda a pravidla

✅ **Kvalita kódu:**
- [x] Abstraktní třídy a dědičnost
- [x] Čitelný, komentovaný kód
- [x] Unit testy
- [x] Dokumentace

## Struktura Projektu

```
2048/
├── animal.py          # Třídy zvířat
├── board.py           # Logika desky
├── game.py            # Logika hry
├── main.py            # Vstupní bod
├── test_game.py       # Testy
└── README.md          # Dokumentace
```

## Poznámky k Implementaci

- **Matematické operátory**: Slučování je implementováno pomocí operátoru `+` a `-` v třídě `Animal`
- **Polymorfismus**: `Dog` a `Cat` dědí z abstraktní třídy `Animal`
- **Undo systém**: Uchovává historii stavů desky
- **Flexibilita**: Pravidla slučování lze snadno měnit v metodě `_combine_animals()`

## Budoucí Vylepšení

Možná rozšíření:
- [ ] Grafické rozhraní (pygame)
- [ ] Uložení a načtení hry
- [ ] Leaderboard
- [ ] AI protivník
- [ ] Více herních režimů

## Autor

Vytvořeno pro projekt "Programování na SPS-CL"

---

**Verze**: 1.0  
**Datum**: Leden 2026
