from __future__ import annotations
import math
import cmath


class KomplexniCislo:
    __slots__ = ("re", "im")

    def __init__(self, re=0.0, im=0.0):
        self.re = float(re)
        self.im = float(im)

    #prevod na KomplexniCislo
    @staticmethod
    def _c(o):
        if isinstance(o, KomplexniCislo):
            return o
        if isinstance(o, (int, float)):
            return KomplexniCislo(o, 0)
        return NotImplemented

    #operatory
    def __add__(self, o):
        o = self._c(o)
        return KomplexniCislo(self.re + o.re, self.im + o.im)

    __radd__ = __add__

    def __sub__(self, o):
        o = self._c(o)
        return KomplexniCislo(self.re - o.re, self.im - o.im)

    def __rsub__(self, o):
        o = self._c(o)
        return KomplexniCislo(o.re - self.re, o.im - self.im)

    def __mul__(self, o):
        o = self._c(o)
        return KomplexniCislo(
            self.re * o.re - self.im * o.im,
            self.re * o.im + self.im * o.re
        )

    __rmul__ = __mul__

    def __truediv__(self, o):
        o = self._c(o)
        d = o.re**2 + o.im**2
        if d == 0:
            raise ZeroDivisionError("dělení nulou")
        return KomplexniCislo(
            (self.re * o.re + self.im * o.im) / d,
            (self.im * o.re - self.re * o.im) / d
        )

    def __neg__(self):
        return KomplexniCislo(-self.re, -self.im)

    #komplexni funkce
    def __abs__(self):
        return math.hypot(self.re, self.im)

    def arg(self):
        return math.atan2(self.im, self.re)

    def conjugate(self):
        return KomplexniCislo(self.re, -self.im)

    def sqrt(self):
        z = cmath.sqrt(complex(self))
        return KomplexniCislo(z.real, z.imag)

    def exp(self):
        z = cmath.exp(complex(self))
        return KomplexniCislo(z.real, z.imag)

    def __complex__(self):
        return complex(self.re, self.im)

    def __repr__(self):
        return f"KomplexniCislo({self.re}, {self.im})"

    def __str__(self):
        s = "+" if self.im >= 0 else "-"
        return f"{self.re} {s} {abs(self.im)}i"


if __name__ == "__main__":
    a = KomplexniCislo(2, 0)
    b = KomplexniCislo(1, -4)

    print("=== ZAKLADNI HODNOTY ===")
    print("a:", a)
    print("b:", b)

    print("\n=== ARITMETICKE OPERACE ===")
    print("a + b =", 1 / b)
    print("a - b =", a - b)
    print("a * b =", a * b)

    print("\n=== OPERACE S REALNYMI CISLY ===")
    print("a + 5 =", a + 5)
    print("5 + a =", 5 + a)
    print("a - 2 =", a - 2)
    print("2 - a =", 2 - a)
    print("a * 2 =", a * 2)
    print("2 * a =", 2 * a)
    print("a / 2 =", a / 2)

    print("\n=== KOMPLEXNI VLASTNOSTI CISLA b ===")
    print("abs(b) =", abs(b))
    print("arg(b) =", b.arg())
    print("conjugate(b) =", b.conjugate())
    print("sqrt(b) =", b.sqrt())

    print("\n=== FUNKCE ===")
    print("exp(a) =", a.exp())
    print("complex(a) =", complex(a))

