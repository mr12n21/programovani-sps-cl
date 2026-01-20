"""
Hlavní třída hry - řídí průběh a logiku herního kola.
"""
from board import GameBoard


class Game:
    """Třída řídící hru 2048 pro dva hráče."""
    
    def __init__(self):
        """Inicializuje novou hru."""
        self.board = GameBoard()
        self.current_player = 'dog'  # Začíná hráč s psy
        self.dog_won = False
        self.cat_won = False
        self.game_over = False
        self.moves = 0
    
    def switch_player(self):
        """Přepne na dalšího hráče."""
        self.current_player = 'cat' if self.current_player == 'dog' else 'dog'
    
    def get_current_player_name(self) -> str:
        """Vrací jméno aktuálního hráče."""
        return "Psi" if self.current_player == 'dog' else "Kočky"
    
    def process_move(self, direction: str) -> bool:
        """
        Zpracuje pohyb hráče.
        
        Args:
            direction: Směr pohybu ('up', 'down', 'left', 'right')
        
        Returns:
            True pokud byl tah platný, False jinak.
        """
        # Kontrola platnosti směru
        if direction not in ['up', 'down', 'left', 'right']:
            return False
        
        # Provede pohyb
        if not self.board.move(direction, self.current_player):
            return False  # Tah nebyl možný
        
        self.moves += 1
        
        # Kontrola vítězství
        if self.board.has_player_won(self.current_player):
            self.game_over = True
            if self.current_player == 'dog':
                self.dog_won = True
            else:
                self.cat_won = True
            return True
        
        # Kontrola konce hry
        if self.board.is_game_over():
            self.game_over = True
            return True
        
        # Přepne na dalšího hráče
        self.switch_player()
        return True
    
    def display_state(self):
        """Zobrazí aktuální stav hry."""
        self.board.display()
        
        dog_level = self.board.get_highest_level('dog')
        cat_level = self.board.get_highest_level('cat')
        
        print(f"Psi (nejvyšší úroveň): {dog_level}/3")
        print(f"Kočky (nejvyšší úroveň): {cat_level}/3")
        print(f"Celkový počet tahů: {self.moves}")
        print(f"Na tahu: {self.get_current_player_name()}\n")
    
    def display_rules(self):
        """Zobrazí pravidla hry."""
        print("\n" + "=" * 50)
        print("PRAVIDLA HRY 2048")
        print("=" * 50)
        print("""
Dva hráči si střídají tahy:
1. Hráč s psy (štěně → pes → vlk)
2. Hráč s kočkami (kotě → kočka → lev)

SLUČOVÁNÍ ZVÍŘAT:
- Stejný typ + stejná úroveň = evoluce (vyšší úroveň)
- Různé typy + stejná úroveň = nic (pokud je to typ aktuálního hráče)
- Různá úroveň = vyšší úroveň vítězí

CÍL:
- Vytvořit vlka (pro psy) nebo lva (pro kočky)
- Nebo mít nejvyšší úroveň když už žádné tahy nejsou

OVLÁDÁNÍ:
- up, down, left, right = pohyb
- undo = vrátit poslední tah
- quit = konec hry

""")
        print("=" * 50 + "\n")
    
    def get_winner_name(self) -> str:
        """Vrací jméno vítěze."""
        if self.dog_won:
            return "Psi"
        elif self.cat_won:
            return "Kočky"
        else:
            dog_level = self.board.get_highest_level('dog')
            cat_level = self.board.get_highest_level('cat')
            if dog_level > cat_level:
                return "Psi"
            elif cat_level > dog_level:
                return "Kočky"
            else:
                return "Remíza"
    
    def display_game_over(self):
        """Zobrazí konečný stav hry."""
        print("\n" + "=" * 50)
        print("KONEC HRY")
        print("=" * 50)
        
        if self.dog_won:
            print("🐺 PSI ZVÍTĚZILI! 🐺")
        elif self.cat_won:
            print("🦁 KOČKY ZVÍTĚZILY! 🦁")
        else:
            dog_level = self.board.get_highest_level('dog')
            cat_level = self.board.get_highest_level('cat')
            
            if dog_level > cat_level:
                print("🐕 PSI ZVÍTĚZILI (vyšší úroveň)! 🐕")
            elif cat_level > dog_level:
                print("😺 KOČKY ZVÍTĚZILY (vyšší úroveň)! 😺")
            else:
                print("⚖️ REMÍZA! ⚖️")
        
        print(f"\nKonečné úrovně:")
        print(f"  Psi:    {self.board.get_highest_level('dog')}/3")
        print(f"  Kočky:  {self.board.get_highest_level('cat')}/3")
        print(f"  Tahů:   {self.moves}")
        print("=" * 50 + "\n")
