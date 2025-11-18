from models.kniha import TistenaKniha, ElektronickaKniha
from models.uzivatel import Student, Ucitel
from models.vypujcka import Vypujcka

class Knihovna:
    def __init__(self):
        self.knihy = {}
        self.uzivatele = {}
        self.vypujcky = []

    def pridej_knihu(self, kniha):
        if kniha.isbn in self.knihy:
            print("Kniha s tímto ISBN již existuje!")
        else:
            self.knihy[kniha.isbn] = kniha
            print(f"Přidána: {kniha}")

    def pridej_uzivatele(self, uzivatel):
        if uzivatel.id_uzivatele in self.uzivatele:
            print("Uživatel s tímto ID již existuje!")
        else:
            self.uzivatele[uzivatel.id_uzivatele] = uzivatel
            print(f"Přidán: {uzivatel}")

    def pujcit_knihu(self, isbn, id_uzivatele):
        if isbn not in self.knihy:
            print("Kniha nenalezena!")
            return
        if id_uzivatele not in self.uzivatele:
            print("Uživatel nenalezen!")
            return

        kniha = self.knihy[isbn]
        uzivatel = self.uzivatele[id_uzivatele]

        if not kniha.dostupna:
            print(f"Kniha '{kniha.nazev}' není dostupná.")
            return
        if not uzivatel.muze_pujcit():
            print(f"Uživatel {uzivatel.jmeno} dosáhl limitu!")
            return

        kniha.dostupna = False
        uzivatel.vypujcene_knihy.append(isbn)
        self.vypujcky.append(Vypujcka(kniha, uzivatel))
        print(f"Půjčeno: {kniha.nazev} → {uzivatel.jmeno}")

    def vratit_knihu(self, isbn, id_uzivatele):
        for v in self.vypujcky:
            if (v.kniha.isbn == isbn and
                v.uzivatel.id_uzivatele == id_uzivatele and
                v.datum_vraceni is None):
                v.vratit()
                print(f"Vraceno: {v.kniha.nazev}")
                return
        print("Výpůjčka nenalezena.")

    def zobraz_knihy(self):
        print("\n--- KNIHY ---")
        for k in self.knihy.values():
            print(k.info())

    def zobraz_uzivatele(self):
        print("\n--- UŽIVATELÉ ---")
        for u in self.uzivatele.values():
            print(u)
            print(f"  Vypůjčeno: {len(u.vypujcene_knihy)} / {u.max_vypujcek()}")

    def zobraz_vypujcky(self):
        print("\n--- VÝPŮJČKY ---")
        aktivni = [v for v in self.vypujcky if v.datum_vraceni is None]
        if not aktivni:
            print("Žádné aktivní výpůjčky.")
        for v in aktivni:
            print(f"  • {v}")