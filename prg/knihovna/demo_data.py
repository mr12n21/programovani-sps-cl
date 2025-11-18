from models.kniha import TistenaKniha, ElektronickaKniha
from models.uzivatel import Student, Ucitel

def napln_demo_data(knihovna):
    knihovna.pridej_knihu(TistenaKniha("1", "Kniha A", "Petr novak", 430))
    knihovna.pridej_knihu(ElektronickaKniha("2", "Python", "Jan Novák", "elektronicka"))
    knihovna.pridej_knihu(TistenaKniha("3", "Kniha C", "nedko", 328))

    knihovna.pridej_uzivatele(Student("S1", "Petr Novák", "petr@skola.cz"))
    knihovna.pridej_uzivatele(Ucitel("U1", "Anna Svobodová", "anna@skola.cz"))