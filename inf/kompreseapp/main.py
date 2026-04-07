"""
Kompresní aplikace - Shannon-Fano, RLE, LZW
Web aplikace pomocí Flask.
"""

from flask import Flask, render_template, request, jsonify

app = Flask(__name__)


def shannon_fano_frekvence(text):
    """Spočítá frekvence znaků a vrátí seřazený seznam (znak, frekvence) sestupně."""
    freq = {}
    for ch in text:
        freq[ch] = freq.get(ch, 0) + 1
    return sorted(freq.items(), key=lambda x: -x[1])


def shannon_fano_rozdeleni(symboly, kody):
    """Rekurzivně rozdělí symboly na dvě skupiny s co nejpodobnějším součtem frekvencí."""
    if len(symboly) == 1:
        kody[symboly[0][0]] = kody.get(symboly[0][0], "")
        return

    celkem = sum(f for _, f in symboly)
    soucet = 0
    nejlepsi_rozdil = celkem
    bod = 0
    for i in range(len(symboly) - 1):
        soucet += symboly[i][1]
        rozdil = abs(celkem - 2 * soucet)
        if rozdil < nejlepsi_rozdil:
            nejlepsi_rozdil = rozdil
            bod = i + 1

    leva = symboly[:bod]
    prava = symboly[bod:]

    for znak, _ in leva:
        kody[znak] = kody.get(znak, "") + "0"
    for znak, _ in prava:
        kody[znak] = kody.get(znak, "") + "1"

    if len(leva) > 1:
        shannon_fano_rozdeleni(leva, kody)
    if len(prava) > 1:
        shannon_fano_rozdeleni(prava, kody)


def shannon_fano_komprese(text):
    """Komprese textu algoritmem Shannon-Fano. Vrací (zakódovaný řetězec, kódovací tabulku)."""
    if not text:
        return "", {}

    frekvence = shannon_fano_frekvence(text)

    if len(frekvence) == 1:
        kody = {frekvence[0][0]: "0"}
    else:
        kody = {}
        shannon_fano_rozdeleni(frekvence, kody)

    zakonovany = "".join(kody[ch] for ch in text)
    return zakonovany, kody


def shannon_fano_dekomprese(zakonovany, kody):
    """Dekomprese Shannon-Fano zakódovaného řetězce pomocí kódovací tabulky."""
    if not zakonovany:
        return ""

    obracena = {kod: znak for znak, kod in kody.items()}

    vysledek = []
    aktualni = ""
    for bit in zakonovany:
        aktualni += bit
        if aktualni in obracena:
            vysledek.append(obracena[aktualni])
            aktualni = ""

    return "".join(vysledek)



def rle_komprese(text):
    """Komprese textu algoritmem RLE. Např. 'AAAABBB' -> '4A3B'."""
    if not text:
        return ""

    vysledek = []
    pocet = 1
    for i in range(1, len(text)):
        if text[i] == text[i - 1]:
            pocet += 1
        else:
            vysledek.append(f"{pocet}{text[i - 1]}")
            pocet = 1
    vysledek.append(f"{pocet}{text[-1]}")

    return "".join(vysledek)


def rle_dekomprese(komprimovany):
    """Dekomprese RLE řetězce. Např. '4A3B' -> 'AAAABBB'."""
    if not komprimovany:
        return ""

    vysledek = []
    cislo = ""
    for ch in komprimovany:
        if ch.isdigit():
            cislo += ch
        else:
            vysledek.append(ch * int(cislo))
            cislo = ""

    return "".join(vysledek)



def lzw_komprese(text):
    """Komprese textu algoritmem LZW. Vrací seznam indexů ze slovníku."""
    if not text:
        return [], {}

    slovnik = {}
    dalsi_kod = 0
    for ch in sorted(set(text)):
        slovnik[ch] = dalsi_kod
        dalsi_kod += 1

    vysledek = []
    aktualni = ""

    for ch in text:
        aktualni_plus = aktualni + ch
        if aktualni_plus in slovnik:
            aktualni = aktualni_plus
        else:
            vysledek.append(slovnik[aktualni])
            slovnik[aktualni_plus] = dalsi_kod
            dalsi_kod += 1
            aktualni = ch

    if aktualni:
        vysledek.append(slovnik[aktualni])

    return vysledek, slovnik


def lzw_dekomprese(komprimovany, pocatecni_znaky):
    """Dekomprese LZW seznamu indexů zpět na text."""
    if not komprimovany:
        return ""

    slovnik = {}
    dalsi_kod = 0
    for ch in sorted(pocatecni_znaky):
        slovnik[dalsi_kod] = ch
        dalsi_kod += 1

    aktualni = slovnik[komprimovany[0]]
    vysledek = [aktualni]

    for kod in komprimovany[1:]:
        if kod in slovnik:
            zaznam = slovnik[kod]
        elif kod == dalsi_kod:
            zaznam = aktualni + aktualni[0]
        else:
            raise ValueError(f"Neplatný komprimovaný kód: {kod}")

        vysledek.append(zaznam)
        slovnik[dalsi_kod] = aktualni + zaznam[0]
        dalsi_kod += 1
        aktualni = zaznam

    return "".join(vysledek)


@app.route("/")
def index():
    return render_template("index.html")


@app.route("/compress", methods=["POST"])
def compress():

    data = request.get_json()

    text = data.get("text", "")
    metoda = data.get("method", "")

    if not text:
        return jsonify({"error": "Nebyl zadán žádný text."}), 400

    vysledek = {"original": text, "method": metoda}

    if metoda == "shannon-fano":
        zakonovany, kody = shannon_fano_komprese(text)
        dekomprimovany = shannon_fano_dekomprese(zakonovany, kody)

        tabulka = [{"znak": z, "kod": k} for z, k in sorted(kody.items())]

        vysledek.update({
            "encoded": zakonovany,
            "table": tabulka,
            "encoded_bits": len(zakonovany),
            "original_bits": len(text) * 8,
            "ratio": round(len(zakonovany) / (len(text) * 8) * 100, 1),
            "decompressed": dekomprimovany,
            "match": dekomprimovany == text,
        })

    elif metoda == "rle":
        komprimovany = rle_komprese(text)
        dekomprimovany = rle_dekomprese(komprimovany)

        vysledek.update({
            "compressed": komprimovany,
            "compressed_len": len(komprimovany),
            "original_len": len(text),
            "ratio": round(len(komprimovany) / len(text) * 100, 1),
            "decompressed": dekomprimovany,
            "match": dekomprimovany == text,
        })

    elif metoda == "lzw":
        indexy, slovnik = lzw_komprese(text)
        pocatecni_znaky = sorted(set(text))
        dekomprimovany = lzw_dekomprese(indexy, pocatecni_znaky)

        slovnik_list = [
            {"key": k, "value": v}
            for k, v in sorted(slovnik.items(), key=lambda x: x[1])[:40]
        ]

        vysledek.update({
            "indices": indexy,
            "indices_count": len(indexy),
            "dictionary": slovnik_list,
            "dict_total": len(slovnik),

            "decompressed": dekomprimovany,
            "match": dekomprimovany == text,

        })


    else:
        return jsonify({"error": "Neplatná metoda."}), 400

    return jsonify(vysledek)


if __name__ == "__main__":
    app.run(debug=True, port=5000)
