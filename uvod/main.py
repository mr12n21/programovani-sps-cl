import math

def obsah_podstavy(r):
    return math.pi * r ** 2

def obsah_plaste(r, v):
    return 2 * math.pi * r * v

def objem_valce(r, v):
    return obsah_podstavy(r) * v

r = float(input("Zadej poloměr podstavy (cm): "))
v = float(input("Zadej výšku válce (cm): "))

print(f"Obsah podstavy: {obsah_podstavy(r):.2f}")
print(f"Obsah pláště: {obsah_plaste(r, v):.2f}")
print(f"Objem válce: {objem_valce(r, v):.2f}")