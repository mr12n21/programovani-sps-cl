from datetime import datetime

class Vypujcka:
    def __init__(self, kniha, uzivatel):
        self.kniha = kniha
        self.uzivatel = uzivatel
        self.datum_pujceni = datetime.now()
        self.datum_vraceni = None

    def vratit(self):
        self.datum_vraceni = datetime.now()
        self.kniha.dostupna = True
        if self.kniha.isbn in self.uzivatel.vypujcene_knihy:
            self.uzivatel.vypujcene_knihy.remove(self.kniha.isbn)

    def __str__(self):
        doba = (self.datum_vraceni or datetime.now()) - self.datum_pujceni
        dny = doba.days
        if self.datum_vraceni:
            return f"{self.uzivatel.jmeno} vrátil {self.kniha.nazev} po {dny} dnech."
        else:
            return f"{self.uzivatel.jmeno} má: {self.kniha.nazev} ({dny} dní)"