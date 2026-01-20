"""
Abstraktní třída pro zvířata a konkrétní implementace psů a koček.
"""
from abc import ABC, abstractmethod
from typing import Optional





class Animal(ABC):
    """Abstraktní třída pro všechna zvířata."""
    def __init__(self, animal_type: str, level: int):
        """
        Inicializace zvířete.
        
        Args:
            animal_type: Typ zvířete ('dog' nebo 'cat')
            level: Úroveň zvířete (1-3)
        """
        self.animal_type = animal_type
        self.level = level
    
    @abstractmethod
    def get_name(self) -> str:
        """Vrátí jméno zvířete."""
        pass
    
    @abstractmethod
    def evolve(self) -> Optional['Animal']:
        """Evoluuje na další úroveň, vrací nové zvíře nebo None."""
        pass
    
    def __add__(self, other: 'Animal') -> Optional['Animal']:
        """
        Operátor +: spojování dvou zvířat.
        - Stejný typ a úroveň: vytvoří zvíře vyšší úrovně
        - Různé typy, stejná úroveň: nic se neděje (vrací None)
        - Různé úrovně: zvíře s vyšší úrovní zůstává
        """
        if not isinstance(other, Animal):
            return None
        
        # Pokud mají různou úroveň, vyhraje vyšší úroveň
        if self.level != other.level:
            return self if self.level > other.level else other
        
        # Pokud mají různý typ a stejnou úroveň, nic se neděje
        if self.animal_type != other.animal_type:
            return None
        
        # Stejný typ a úroveň -> evoluce
        return self.evolve()
    
    def __sub__(self, other: 'Animal') -> Optional['Animal']:
        """Operátor -: alternativa pro spojování (stejné chování jako +)."""
        return self + other
    
    def __repr__(self) -> str:
        return f"{self.get_name()} (level {self.level})"
    
    def __eq__(self, other: 'Animal') -> bool:
        """Porovnání dvou zvířat."""
        if not isinstance(other, Animal):
            return False
        return self.animal_type == other.animal_type and self.level == other.level


class Dog(Animal):
    """Třída pro psy: štěně -> pes -> vlk."""
    
    EVOLUTION_CHAIN = {
        1: "Štěně",
        2: "Pes",
        3: "Vlk"
    }
    
    def __init__(self, level: int = 1):
        super().__init__('dog', level)
    
    def get_name(self) -> str:
        return self.EVOLUTION_CHAIN.get(self.level, "Neznámý pes")
    
    def evolve(self) -> Optional['Dog']:
        """Evoluuje psa na další úroveň."""
        if self.level < 3:
            return Dog(self.level + 1)
        return None 
    
    def copy(self) -> 'Dog':
        """Vytvoří kopii zvířete."""
        return Dog(self.level)


class Cat(Animal):
    """Třída pro kočky: kotě -> kočka -> lev."""
    
    EVOLUTION_CHAIN = {
        1: "Kotě",
        2: "Kočka",
        3: "Lev"
    }
    
    def __init__(self, level: int = 1):
        super().__init__('cat', level)
    
    def get_name(self) -> str:
        return self.EVOLUTION_CHAIN.get(self.level, "Neznámá kočka")
    
    def evolve(self) -> Optional['Cat']:
        """Evoluuje kočku na další úroveň."""
        if self.level < 3:
            return Cat(self.level + 1)
        return None
    
    def copy(self) -> 'Cat':
        """Vytvoří kopii zvířete."""
        return Cat(self.level)
