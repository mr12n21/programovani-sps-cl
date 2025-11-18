from abc import ABC, abstractmethod

class Kniha(ABC):
    def __init__(self, isbn, nazev, autor):
        self.isbn = isbn
        self.nazev = nazev
        self.autor = autor
        self.dostupna = True
    @abstractmethod
    def info(self):
        pass

    def __str__(self):
        return f"{self.nazev} ({self.autor})"

class TistenaKniha(Kniha):
    def __init__(self, isbn, nazev, autor, pocet_stran):
        super().__init__(isbn, nazev, autor)
        self.pocet_stran = pocet_stran

    def info(self):
        stav = "Dostupná" if self.dostupna else "Vypůjčená"
        return f"[Tištěná] {self.nazev} - {self.autor} ({self.pocet_stran} str.) | {stav}"

class ElektronickaKniha(Kniha):
    def __init__(self, isbn, nazev, autor, format_souboru="PDF"):
        super().__init__(isbn, nazev, autor)
        self.format_souboru = format_souboru.upper()

    def info(self):
        stav = "Dostupná" if self.dostupna else "Vypůjčená"
        return f"[E-kniha] {self.nazev} - {self.autor} ({self.format_souboru}) | {stav}"