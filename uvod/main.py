cisla = [4, 7, 1, 8, 3, 7, 1]
print("puvodni:", cisla)

upraveny_seznam = list(dict.fromkeys(cisla))
print("bez duplicit:", upraveny_seznam)

serazeny_seznam = sorted(upraveny_seznam)
print("serazeny:", serazeny_seznam)
print("nejmensi:", serazeny_seznam[0])
print("nejvetsi:", serazeny_seznam[-1])

osoba = ("marek", "broz", 2)
jmeno, prijmeni, vek = osoba
print(f"jmeno prijmeni: {jmeno} {prijmeni} vek {vek} let.")

A = {"jablko", "banán", "hruška"}
B = {"banán", "pomeranč", "hruška"}
print("Mnozina A:", A)
print("Mnozina B:", B)

print("Prunik A a B:", A & B)
print("Prvky jen v prvni mnozine:", A - B)
print("Prvky v obou mnozinach bez duplicit:", A | B)
print("Prvky jen v jedne mnozine:", A ^ B)