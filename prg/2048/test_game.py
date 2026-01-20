"""
Testy základní funkcionalnosti hry.
"""
import unittest
from animal import Dog, Cat
from board import GameBoard


class TestAnimal(unittest.TestCase):
    """Testy třídy Animal."""
    
    def test_dog_creation(self):
        """Test vytvoření psa."""
        dog = Dog(1)
        self.assertEqual(dog.animal_type, 'dog')
        self.assertEqual(dog.level, 1)
        self.assertEqual(dog.get_name(), "Štěně")
    
    def test_cat_creation(self):
        """Test vytvoření kočky."""
        cat = Cat(1)
        self.assertEqual(cat.animal_type, 'cat')
        self.assertEqual(cat.level, 1)
        self.assertEqual(cat.get_name(), "Kotě")
    
    def test_dog_evolution(self):
        """Test evoluce psa."""
        dog = Dog(1)
        evolved = dog.evolve()
        self.assertIsNotNone(evolved)
        self.assertEqual(evolved.level, 2)
        self.assertEqual(evolved.get_name(), "Pes")
    
    def test_cat_evolution(self):
        """Test evoluce kočky."""
        cat = Cat(2)
        evolved = cat.evolve()
        self.assertIsNotNone(evolved)
        self.assertEqual(evolved.level, 3)
        self.assertEqual(evolved.get_name(), "Lev")
    
    def test_max_level_no_evolution(self):
        """Test, že zvíře na max úrovni se neevoluje."""
        wolf = Dog(3)
        self.assertIsNone(wolf.evolve())
    
    def test_same_type_same_level_merge(self):
        """Test slučování zvířat stejného typu a úrovně."""
        dog1 = Dog(1)
        dog2 = Dog(1)
        result = dog1 + dog2
        self.assertIsNotNone(result)
        self.assertEqual(result.level, 2)
    
    def test_different_type_same_level(self):
        """Test slučování zvířat různého typu a stejné úrovně."""
        dog = Dog(1)
        cat = Cat(1)
        result = dog + cat
        self.assertIsNone(result)
    
    def test_different_levels(self):
        """Test slučování zvířat různých úrovní."""
        dog = Dog(2)
        cat = Cat(1)
        result = dog + cat
        self.assertIsNotNone(result)
        self.assertEqual(result.level, 2)


class TestGameBoard(unittest.TestCase):
    """Testy třídy GameBoard."""
    
    def test_board_initialization(self):
        """Test inicializace desky."""
        board = GameBoard()
        # Má přesně 2 zvířata na začátku
        count = sum(1 for i in range(4) for j in range(4) if board.board[i][j] is not None)
        self.assertEqual(count, 2)
    
    def test_add_animal(self):
        """Test přidání zvířete."""
        board = GameBoard()
        initial_count = sum(1 for i in range(4) for j in range(4) if board.board[i][j] is not None)
        result = board.add_random_animal()
        self.assertTrue(result)
        new_count = sum(1 for i in range(4) for j in range(4) if board.board[i][j] is not None)
        self.assertEqual(new_count, initial_count + 1)
    
    def test_undo_move(self):
        """Test vrácení tahu."""
        board = GameBoard()
        board.save_state()
        board.board[0][0] = Dog(1)
        self.assertTrue(board.undo())
        self.assertIsNone(board.board[0][0])
    
    def test_game_over_detection(self):
        """Test detekce konce hry."""
        board = GameBoard()
        # Naplní desku zvířaty, která se nemohou slučovat
        # Střídavý pattern tak, aby se nic neslučovalo
        values = [
            (Dog(1), Cat(1), Dog(2), Cat(2)),
            (Cat(1), Dog(1), Cat(2), Dog(2)),
            (Dog(2), Cat(2), Dog(1), Cat(1)),
            (Cat(2), Dog(2), Cat(1), Dog(1))
        ]
        for i in range(4):
            for j in range(4):
                board.board[i][j] = values[i][j]
        
        # S tímto uspořádáním by měla být hra skončena
        self.assertTrue(board.is_game_over())


class TestGameLogic(unittest.TestCase):
    """Testy herní logiky."""
    
    def test_move_left(self):
        """Test pohybu doleva."""
        board = GameBoard()
        # Vyčistí desku
        for i in range(4):
            for j in range(4):
                board.board[i][j] = None
        
        # Umístí zvířata na řádek
        board.board[0][0] = Dog(1)
        board.board[0][2] = Dog(1)
        
        board._move_left('dog')
        
        # Oba psi by měli být na začátku řádku a spojeni
        self.assertIsNotNone(board.board[0][0])
        self.assertEqual(board.board[0][0].level, 2)


if __name__ == '__main__':
    unittest.main()
