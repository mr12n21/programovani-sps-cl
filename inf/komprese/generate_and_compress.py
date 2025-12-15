import os
import bz2
import gzip
import lzma
import math
import random
from collections import Counter

BASE_DIR = os.path.dirname(__file__)

FILES = [
    ("200.txt", "A" * 200),
    ("wikiped.txt", None),
    ("a200.txt", "".join(chr(random.randint(32, 126)) for _ in range(200))),
]

ALPHABET_SIZES = {
    "ASCII": 256,
    "Unicode": 297334,
}


def read_or_default(path: str, default_content: str) -> bytes:
    full = os.path.join(BASE_DIR, path)
    if os.path.exists(full):
        with open(full, "rb") as f:
            return f.read()
    return default_content.encode("utf-8")


def entropy(data: bytes) -> float:
    if not data:
        return 0.0
    counts = Counter(data)
    n = len(data)
    H = 0.0
    for c in counts.values():
        p = c / n
        H -= p * math.log2(p)
    return H


def redundancy_bits_per_symbol(H: float, alphabet_size: int) -> float:
    max_bits = math.log2(alphabet_size)
    return max(0.0, max_bits - H)


def write_file(path: str, content: bytes) -> None:
    full = os.path.join(BASE_DIR, path)
    with open(full, "wb") as f:
        f.write(content)


def compress_all(name: str, content: bytes):
    paths = {}
    # gzip
    gz_path = os.path.join(BASE_DIR, f"{name}.gz")
    with gzip.open(gz_path, "wb", compresslevel=9) as f:
        f.write(content)
    paths["GZIP"] = gz_path
    # bzip2
    bz_path = os.path.join(BASE_DIR, f"{name}.bz2")
    with open(bz_path, "wb") as f:
        f.write(bz2.compress(content, compresslevel=9))
    paths["BZIP2"] = bz_path
    # xz (lzma)
    xz_path = os.path.join(BASE_DIR, f"{name}.xz")
    with lzma.open(xz_path, "wb", preset=9) as f:
        f.write(content)
    paths["XZ"] = xz_path
    return paths


def sizeof(path: str) -> int:
    return os.path.getsize(path)


def gen_files():
    results = []
    for fname, gen_content in FILES:
        if gen_content is None:
            # try to read wikiped.txt; use placeholder if missing
            content = read_or_default(fname, "Wikipedia odstavec nenalezen.")
        else:
            content = gen_content.encode("utf-8") if isinstance(gen_content, str) else gen_content

        write_file(fname, content)
        comp_paths = compress_all(fname, content)

        original_size = len(content)
        H = entropy(content)
        red_ascii = redundancy_bits_per_symbol(H, ALPHABET_SIZES["ASCII"])
        red_unicode = redundancy_bits_per_symbol(H, ALPHABET_SIZES["Unicode"])

        row = {
            "file": fname,
            "original": original_size,
            "entropy": H,
            "redundancy_ascii": red_ascii,
            "redundancy_unicode": red_unicode,
            "compressed": {fmt: sizeof(path) for fmt, path in comp_paths.items()},
        }
        results.append(row)
    return results


def percent_compression(orig: int, comp: int) -> float:
    if orig == 0:
        return 0.0
    return 100.0 * (1.0 - comp / orig)


def write_table_md(results):
    md_path = os.path.join(BASE_DIR, "vysledek.md")
    lines = []
    lines.append("# Komprese a entropie")
    lines.append("")
    lines.append("| Soubor | Původní (B) | GZIP (B) | GZIP % | BZIP2 (B) | BZIP2 % | XZ (B) | XZ % | Entropie (bit/znak) | Redundance ASCII | Redundance Unicode |")
    lines.append("|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|")
    for r in results:
        orig = r["original"]
        gz = r["compressed"].get("GZIP", 0)
        bz = r["compressed"].get("BZIP2", 0)
        xz = r["compressed"].get("XZ", 0)
        lines.append(
            f"| {r['file']} | {orig} | {gz} | {percent_compression(orig, gz):.2f} | "
            f"{bz} | {percent_compression(orig, bz):.2f} | {xz} | {percent_compression(orig, xz):.2f} | "
            f"{r['entropy']:.4f} | {r['redundancy_ascii']:.4f} | {r['redundancy_unicode']:.4f} |"
        )

    with open(md_path, "w", encoding="utf-8") as f:
        f.write("\n".join(lines))


def main():
    results = gen_files()
    write_table_md(results)
    print("Hotovo: vygenerováno, zkomprimováno, tabulka aktualizována v vysledek.md")


if __name__ == "__main__":
    main()
