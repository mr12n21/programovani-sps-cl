"""
Logika hrací desky pro hru 2048.
"""
from typing import List, Optional, Tuple
from animal import Animal, Dog, Cat
import random
import copy


class GameBoard:
    """Třída reprezentující hrací desku 4x4."""
    
    def __init__(self):
        """Inicializuje novou hrací desku."""
        self.board: List[List[Optional[Animal]]] = [[None for _ in range(4)] for _ in range(4)]
        self.history: List[List[List[Optional[Animal]]]] = []
        self.add_random_animal()
        self.add_random_animal()
    
    def add_random_animal(self, force_type: Optional[str] = None) -> bool:
        """
        Přidá náhodné zvíře na prázdné místo na desce.
        
        Args:
            force_type: Typ zvířete ('dog' nebo 'cat'). Pokud None, vybere se náhodně.
        
        Returns:
            True pokud bylo zvíře přidáno, False pokud není místo.
        """
        empty_cells = [(i, j) for i in range(4) for j in range(4) if self.board[i][j] is None]
        
        if not empty_cells:
            return False
        
        row, col = random.choice(empty_cells)
        
        # Pokud není určen typ, vybere se náhodně
        if force_type is None:
            force_type = random.choice(['dog', 'cat'])
        
        # Vytvoří zvíře úrovně 1
        if force_type == 'dog':
            self.board[row][col] = Dog(1)
        else:
            self.board[row][col] = Cat(1)
        
        return True
    
    def save_state(self):
        """Uloží aktuální stav desky do historie."""
        self.history.append(copy.deepcopy(self.board))
    
    def undo(self) -> bool:
        """Vrátí poslední tah. Vrací True pokud se undo podařilo."""
        if not self.history:
            return False
        self.board = self.history.pop()
        return True
    
    def move(self, direction: str, player: str) -> bool:
        """
        Provede pohyb na desce.
        
        Args:
            direction: Směr pohybu ('up', 'down', 'left', 'right')
            player: 'dog' nebo 'cat' - určuje prioritu při slučování
        
        Returns:
            True pokud byl tah platný (něco se změnilo), False jinak.
        """
        self.save_state()
        original_board = copy.deepcopy(self.board)
        
        if direction == 'up':
            self._move_up(player)
        elif direction == 'down':
            self._move_down(player)
        elif direction == 'left':
            self._move_left(player)
        elif direction == 'right':
            self._move_right(player)
        else:
            self.history.pop()
            return False
        
        # Pokud se deska nezměnila, vrátí False
        if self._boards_equal(original_board, self.board):
            self.history.pop()
            return False
        
        # Přidá nové zvíře
        self.add_random_animal()
        return True
    
    def _move_left(self, player: str):
        """Posunutí všech zvířat doleva."""
        for i in range(4):
            row = self.board[i]
            # Filtruje prázdná místa
            animals = [a for a in row if a is not None]
            # Slučuje zvířata
            animals = self._merge(animals, player)
            # Doplní zpět na řádek s prázdnými místy
            self.board[i] = animals + [None] * (4 - len(animals))
    
    def _move_right(self, player: str):
        """Posunutí všech zvířat doprava."""
        for i in range(4):
            row = self.board[i]
            animals = [a for a in row if a is not None]
            animals = self._merge_reverse(animals, player)
            self.board[i] = [None] * (4 - len(animals)) + animals
    
    def _move_up(self, player: str):
        """Posunutí všech zvířat nahoru."""
        for j in range(4):
            col = [self.board[i][j] for i in range(4)]
            animals = [a for a in col if a is not None]
            animals = self._merge(animals, player)
            for i, animal in enumerate(animals):
                self.board[i][j] = animal
            for i in range(len(animals), 4):
                self.board[i][j] = None
    
    def _move_down(self, player: str):
        """Posunutí všech zvířat dolů."""
        for j in range(4):
            col = [self.board[i][j] for i in range(4)]
            animals = [a for a in col if a is not None]
            animals = self._merge_reverse(animals, player)
            for i, animal in enumerate(animals):
                self.board[3 - i][j] = animal
            for i in range(len(animals)):
                self.board[i][j] = None
    
    def _merge(self, animals: List[Animal], player: str) -> List[Animal]:
        """
        Slučuje zvířata zleva doprava podle pravidel.
        """
        if not animals:
            return []
        
        result = [animals[0]]
        
        for i in range(1, len(animals)):
            current = animals[i]
            last = result[-1]
            
            # Pokusí se spojit zvířata
            merged = self._combine_animals(last, current, player)
            
            if merged is not None:
                result[-1] = merged
            else:
                result.append(current)
        
        return result
    
    def _merge_reverse(self, animals: List[Animal], player: str) -> List[Animal]:
        """
        Slučuje zvířata zprava doleva podle pravidel.
        """
        animals_reversed = animals[::-1]
        merged = self._merge(animals_reversed, player)
        return merged[::-1]
    
    def _combine_animals(self, animal1: Animal, animal2: Animal, player: str) -> Optional[Animal]:
        """
        Kombinuje dvě zvířata. Závisí na tom, který hráč je na tahu.
        - Pokud jsou stejného typu a úrovně: evoluují
        - Pokud jsou stejné úrovně, ale různého typu: vyhraje typ aktuálního hráče
        - Pokud mají různé úrovně: vyhraje vyšší
        """
        
        # Stejná úroveň
        if animal1.level == animal2.level:
            # Stejný typ: evoluují
            if animal1.animal_type == animal2.animal_type:
                return animal1.evolve()
            # Různý typ: vyhraje typ aktuálního hráče, nebo vyšší nebere nižší
            else:
                # Pokud je jeden typ stejný jako hráč, ten vyhraje
                if animal1.animal_type == player:
                    return animal1
                elif animal2.animal_type == player:
                    return animal2
                # Jinak zůstává první zvíře
                return None
        else:
            # Různá úroveň: vyhraje vyšší
            if animal1.level > animal2.level:
                return animal1
            else:
                return animal2
    
    def is_game_over(self) -> bool:
        """Zkontroluje, zda je hra skončena (bez možných tahů)."""
        # Pokud jsou volná místa, hra neskončila
        if any(self.board[i][j] is None for i in range(4) for j in range(4)):
            return False
        
        # Zkontroluje, zda jsou možné slučovací tahy
        for i in range(4):
            for j in range(4):
                current = self.board[i][j]
                
                # Kontrola vpravo
                if j < 3:
                    right = self.board[i][j + 1]
                    if current.animal_type == right.animal_type and current.level == right.level:
                        return False
                
                # Kontrola dolů
                if i < 3:
                    down = self.board[i + 1][j]
                    if current.animal_type == down.animal_type and current.level == down.level:
                        return False
        
        return True
    
    def has_player_won(self, player: str) -> bool:
        """Zkontroluje, zda hráč získal nejvyšší variantu svého zvířete (úroveň 3)."""
        target_level = 3
        for i in range(4):
            for j in range(4):
                animal = self.board[i][j]
                if animal and animal.animal_type == player and animal.level == target_level:
                    return True
        return False
    
    def get_highest_level(self, player: str) -> int:
        """Vrací nejvyšší úroveň zvířete daného hráče."""
        max_level = 0
        for i in range(4):
            for j in range(4):
                animal = self.board[i][j]
                if animal and animal.animal_type == player:
                    max_level = max(max_level, animal.level)
        return max_level
    
    def display(self):
        """Zobrazí hrací desku v terminálu."""
        print("\n" + "=" * 40)
        for i in range(4):
            row_str = ""
            for j in range(4):
                animal = self.board[i][j]
                if animal is None:
                    cell = "[ ----- ]"
                else:
                    name = animal.get_name()
                    cell = f"[{name:^7}]"
                row_str += cell
            print(row_str)
        print("=" * 40 + "\n")
    
    def _boards_equal(self, board1: List[List[Optional[Animal]]], 
                      board2: List[List[Optional[Animal]]]) -> bool:
        """Porovná dvě desky."""
        for i in range(4):
            for j in range(4):
                a1, a2 = board1[i][j], board2[i][j]
                if (a1 is None and a2 is not None) or (a1 is not None and a2 is None):
                    return False
                if a1 is not None and a2 is not None:
                    if a1.animal_type != a2.animal_type or a1.level != a2.level:
                        return False
        return True
