import math
from collections import Counter
import tkinter as tk
from tkinter import ttk, messagebox


ALPHABET_SIZES = {
    "ASCII": 256,
    "Unicode": 297334,
}


def entropy(text: str) -> float:
    if not text:
        return 0.0
    data = text.encode("utf-8")
    counts = Counter(data)
    n = len(data)
    H = 0.0
    for c in counts.values():
        p = c / n
        H -= p * math.log2(p)
    return H


def redundancy(H: float, alphabet_size: int) -> float:
    return max(0.0, math.log2(alphabet_size) - H)


def kraft_inequality(lengths):
    # lengths: list[int] of codeword lengths in bits
    if not lengths:
        return False, 0.0
    S = sum(2 ** (-l) for l in lengths if l > 0)
    return S <= 1.0 + 1e-12, S


class EntropyApp(tk.Tk):
    def __init__(self):
        super().__init__()
        self.title("Výpočet Entropie, Redundance a Kraftovy nerovnosti")
        self.geometry("800x600")
        self._build_ui()

    def _build_ui(self):
        nb = ttk.Notebook(self)
        nb.pack(fill=tk.BOTH, expand=True)

        # Entropy/Redundancy tab
        frame_er = ttk.Frame(nb)
        nb.add(frame_er, text="Entropie & Redundance")

        ttk.Label(frame_er, text="Zadejte řetězec:").pack(anchor="w", padx=10, pady=(10, 0))
        self.text_input = tk.Text(frame_er, height=10)
        self.text_input.pack(fill=tk.BOTH, expand=True, padx=10, pady=10)

        opts_frame = ttk.Frame(frame_er)
        opts_frame.pack(fill=tk.X, padx=10, pady=5)
        ttk.Label(opts_frame, text="Abeceda:").pack(side=tk.LEFT)
        self.alphabet_var = tk.StringVar(value="ASCII")
        ttk.Combobox(opts_frame, textvariable=self.alphabet_var, values=list(ALPHABET_SIZES.keys()), state="readonly").pack(side=tk.LEFT, padx=10)

        calc_btn = ttk.Button(frame_er, text="Spočítat", command=self.calculate_entropy)
        calc_btn.pack(padx=10, pady=5, anchor="w")

        self.result_lbl = ttk.Label(frame_er, text="")
        self.result_lbl.pack(padx=10, pady=5, anchor="w")

        # Kraft inequality tab
        frame_kraft = ttk.Frame(nb)
        nb.add(frame_kraft, text="Kraftova nerovnost")

        ttk.Label(frame_kraft, text="Zadejte délky kódových slov (v bitech), oddělené čárkou:").pack(anchor="w", padx=10, pady=(10, 0))
        self.lengths_entry = ttk.Entry(frame_kraft)
        self.lengths_entry.pack(fill=tk.X, padx=10, pady=10)

        kraft_btn = ttk.Button(frame_kraft, text="Vyhodnotit", command=self.evaluate_kraft)
        kraft_btn.pack(padx=10, pady=5, anchor="w")

        self.kraft_result = ttk.Label(frame_kraft, text="")
        self.kraft_result.pack(padx=10, pady=5, anchor="w")

    def calculate_entropy(self):
        text = self.text_input.get("1.0", tk.END).rstrip("\n")
        H = entropy(text)
        alphabet = self.alphabet_var.get()
        size = ALPHABET_SIZES.get(alphabet, 256)
        R = redundancy(H, size)
        self.result_lbl.config(text=f"Entropie: {H:.4f} bit/znak | Redundance ({alphabet}): {R:.4f} bit/znak")

    def evaluate_kraft(self):
        raw = self.lengths_entry.get().strip()
        if not raw:
            messagebox.showwarning("Chyba", "Zadejte aspoň jednu délku.")
            return
        try:
            lengths = [int(s) for s in raw.split(',') if s.strip()]
        except ValueError:
            messagebox.showerror("Chyba", "Délky musí být celá čísla oddělená čárkou.")
            return
        ok, S = kraft_inequality(lengths)
        msg = "Je možné vytvořit prefixový kód." if ok else "Prefixový kód není možný."
        self.kraft_result.config(text=f"Σ 2^(-l_i) = {S:.6f} → {msg}")


def main():
    app = EntropyApp()
    app.mainloop()


if __name__ == "__main__":
    main()
