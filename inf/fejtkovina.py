def text_to_ascii_to_binary(text):
    return "\n".join(f"1: '{c}' | 2: {ord(c)} | 3: {format(ord(c),'08b')}" for c in text)

def main():
    s = input("vstup: ").strip()
    print("\nVysledek:\n" + text_to_ascii_to_binary(s) if s else "oh")

if __name__ == "__main__":
    main()


s = input("Zadej text: ").strip()
print("\n".join(f"'{c}' {ord(c)} {format(ord(c),'08b')}" for c in s) if s else "neni vstup")