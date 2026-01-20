#!/usr/bin/env python3
"""
Hlavní program - vstupní bod hry 2048 se zvířaty.
"""
from game import Game


def print_welcome():
    """Zobrazí uvítací zprávu."""
    print("\n" + "=" * 50)
    print("╔════════════════════════════════════════════════╗")
    print("║         HRA 2048 - Tahová Strategie           ║")
    print("║        Psi vs. Kočky: Boj o dominanci        ║")
    print("╚════════════════════════════════════════════════╝")
    print("=" * 50)


def print_help():
    """Zobrazí nápovědu."""
    print("""
DOSTUPNÉ PŘÍKAZY:
  up, down, left, right  - Pohyb zvířat v daném směru
  undo                   - Vrátit poslední tah
  help                   - Zobrazit tuto nápovědu
  rules                  - Zobrazit pravidla
  quit, exit             - Konec hry
""")


def main():
    """Hlavní funkce hry."""
    print_welcome()
    
    game = Game()
    game.display_rules()
    
    print("Hra začíná! Prvních zvířat již bylo umístěno.\n")
    game.display_state()
    
    while not game.game_over:
        try:
            user_input = input(f"{game.get_current_player_name()}, zadej tah (up/down/left/right): ").strip().lower()
            
            if not user_input:
                continue
            
            if user_input in ['quit', 'exit']:
                print("\nHra ukončena.")
                break
            
            if user_input == 'help':
                print_help()
                continue
            
            if user_input == 'rules':
                game.display_rules()
                continue
            
            if user_input == 'undo':
                if game.board.undo():
                    game.moves -= 1
                    print("Poslední tah vrácen.\n")
                    game.display_state()
                else:
                    print("Nelze vrátit tah - historie je prázdná.\n")
                continue
            
            if user_input in ['up', 'down', 'left', 'right']:
                if game.process_move(user_input):
                    game.display_state()
                    
                    if game.game_over:
                        game.display_game_over()
                else:
                    print("Tah nebyl možný - zkus jiný směr.\n")
            else:
                print(f"Neznámý příkaz: '{user_input}'. Zadej 'help' pro nápovědu.\n")
        
        except KeyboardInterrupt:
            print("\n\nHra byla přerušena uživatelem.")
            break
        except Exception as e:
            print(f"Chyba: {e}")
            continue


if __name__ == '__main__':
    main()
