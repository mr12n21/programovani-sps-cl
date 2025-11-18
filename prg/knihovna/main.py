# main.py
from knihovna import Knihovna
from models.kniha import TistenaKniha, ElektronickaKniha
from models.uzivatel import Student, Ucitel
from demo_data import napln_demo_data

def main():
    knihovna = Knihovna()
    napln_demo_data(knihovna)  # Načte demo data

    while True:
        print("\n" + "="*50)
        print("  KNIHOVNÍ SYSTÉM")
        print("="*50)
        print("1. Zobrazit knihy")
        print("2. Zobrazit uživatele")
        print("3. Zobrazit výpůjčky")
        print("4. Půjčit knihu")
        print("5. Vrátit knihu")
        print("6. Přidat knihu")
        print("7. Přidat uživatele")
        print("0. Konec")
        print("-"*50)

        volba = input("Vyber: ").strip()

        if volba == "1":
            knihovna.zobraz_knihy()
        elif volba == "2":
            knihovna.zobraz_uzivatele()
        elif volba == "3":
            knihovna.zobraz_vypujcky()
        elif volba == "4":
            isbn = input("ISBN: ")
            uid = input("ID uživatele: ")
            knihovna.pujcit_knihu(isbn, uid)
        elif volba == "5":
            isbn = input("ISBN: ")
            uid = input("ID uživatele: ")
            knihovna.vratit_knihu(isbn, uid)

        elif volba == "6":
            typ = input("Typ (t/e): ").lower()
            isbn = input("ISBN: ")
            nazev = input("Název: ")
            autor = input("Autor: ")
            if typ == "t":
                strany = int(input("Počet stran: "))
                knihovna.pridej_knihu(TistenaKniha(isbn, nazev, autor, strany))
            else:
                fmt = input("Formát (PDF/EPUB): ")
                knihovna.pridej_knihu(ElektronickaKniha(isbn, nazev, autor, fmt))
        elif volba == "7":
            typ = input("Typ (s/u): ").lower()
            uid = input("ID: ")
            jmeno = input("Jméno: ")
            email = input("Email: ")
            if typ == "s":
                knihovna.pridej_uzivatele(Student(uid, jmeno, email))
            else:
                knihovna.pridej_uzivatele(Ucitel(uid, jmeno, email))
        elif volba == "0":
            print("Na shledanou!")
            break
        else:
            print("Neplatná volba!")

if __name__ == "__main__":
    main()