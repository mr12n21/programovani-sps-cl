# Hra 2048 - Tahová Strategie (Go)

Implementace hry 2048 v jazyce Go podle zadání: **Hra 2048 jako Tahová strategie pro dva hráče** s vlastním systémem interakce zvířat.

## Popis Projektu

Toto je **Go aplikace** implementující hru 2048 s modifikacemi:
- **Dva hráči**: jeden ovládá psy (štěně → pes → vlk), druhý kočky (kotě → kočka → lev)
- **Hrací pole**: čtvercová matice 4×4
- **Cíl**: Jako první vytvořit vlka (pro psy) nebo lva (pro kočky)

## Architektura Aplikace

### Struktura projektu:

```
2048-go/
├── main.go              # Hlavní program s terminálovým rozhraním
├── go.mod               # Go module definice
├── animal/
│   └── animal.go        # Abstraktní rozhraní Animal a konkrétní Dog/Cat
├── board/
│   └── board.go         # Logika hrací desky
├── game/
│   └── game.go          # Řízení průběhu hry
└── README.md            # Tento soubor
```

### Popis balíčků:

1. **`animal`** - Rozhraní `Animal` a konkrétní typy `DogAnimal` a `CatAnimal`
   - Implementace evoluce zvířat
   - Funkce pro slučování zvířat
   - Hierarchie: štěně→pes→vlk, kotě→kočka→lev

2. **`board`** - Struktura `Board` řídící hrací desku
   - Pohyb zvířat (nahoru, dolů, vlevo, vpravo)
   - Slučování zvířat podle pravidel
   - Detekce konce hry
   - Systém vrácení tahů (undo)

3. **`game`** - Struktura `Game` řídící logiku hry
   - Správa hráčů a jejich tahů
   - Kontrola vítězství
   - Zobrazení stavu hry

4. **`main.go`** - Hlavní program
   - Interaktivní ovládání
   - Nápověda a pravidla

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
- Go 1.21 nebo vyšší

### Kompilace a spuštění:
```bash
cd prg/2048-go
go run main.go
```

### Nebo zkompilovat a spustit:
```bash
go build -o 2048-game
./2048-game
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
- [x] Rozhraní a polymorfismus
- [x] Čitelný, komentovaný kód
- [x] Modulární struktura

## Poznámky k Implementaci

- **Rozhraní**: Slučování je implementováno pomocí funkce `CombineAnimals()` v balíčku `animal`
- **Polymorfismus**: `DogAnimal` a `CatAnimal` implementují rozhraní `Animal`
- **Undo systém**: Uchovává historii stavů desky
- **Flexibilita**: Pravidla slučování lze snadno měnit v `CombineAnimals()`

## Rozdíly oproti Python verzi

- Použití rozhraní (interface) místo abstraktních tříd
- Explicitní správa paměti pomocí kopírování
- Silné typování
- Package-based organizace kódu

## Budoucí Vylepšení

Možná rozšíření:
- [ ] Unit testy
- [ ] Grafické rozhraní
- [ ] Uložení a načtení hry
- [ ] Leaderboard
- [ ] AI protivník
- [ ] Více herních režimů

## Autor

Vytvořeno pro projekt "Programování na SPS-CL"

---

**Verze**: 1.0  
**Datum**: Leden 2026  
**Jazyk**: Go 1.21+
