from abc import ABC, abstractmethod

class Uzivatel(ABC):
    def __init__(self, id_uzivatele, jmeno, email):
        self.id_uzivatele = id_uzivatele
        self.jmeno = jmeno
        self.email = email
        self.vypujcene_knihy = []

    @abstractmethod
    def max_vypujcek(self):
        pass

    def muze_pujcit(self):
        return len(self.vypujcene_knihy) < self.max_vypujcek()

    def __str__(self):
        return f"{self.jmeno} ({self.email})"


class Student(Uzivatel):
    def max_vypujcek(self):
        return 1

    def __str__(self):
        return f"Student: {super().__str__()} (max 1 knihy)"


class Ucitel(Uzivatel):
    def max_vypujcek(self):
        return 2

    def __str__(self):
        return f"Učitel: {super().__str__()} (max 2 knih)"