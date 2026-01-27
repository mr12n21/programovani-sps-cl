// Package game implementuje hlavní logiku hry
package game

import (
	"fmt"

	"2048-go/animal"
	"2048-go/board"
)

// Game řídí průběh hry
type Game struct {
	Board         *board.Board
	CurrentPlayer animal.AnimalType
	DogWon        bool
	CatWon        bool
	GameOver      bool
	Moves         int
}

// NewGame vytvoří novou hru
func NewGame() *Game {
	return &Game{
		Board:         board.NewBoard(),
		CurrentPlayer: animal.Dog, // Začíná hráč s psy
		DogWon:        false,
		CatWon:        false,
		GameOver:      false,
		Moves:         0,
	}
}

// SwitchPlayer přepne na dalšího hráče
func (g *Game) SwitchPlayer() {
	if g.CurrentPlayer == animal.Dog {
		g.CurrentPlayer = animal.Cat
	} else {
		g.CurrentPlayer = animal.Dog
	}
}

// GetCurrentPlayerName vrací jméno aktuálního hráče
func (g *Game) GetCurrentPlayerName() string {
	if g.CurrentPlayer == animal.Dog {
		return "Psi"
	}
	return "Kočky"
}

// ProcessMove zpracuje pohyb hráče
func (g *Game) ProcessMove(direction string) bool {
	// Kontrola platnosti směru
	if direction != "up" && direction != "down" && direction != "left" && direction != "right" {
		return false
	}

	// Provede pohyb
	if !g.Board.Move(direction, g.CurrentPlayer) {
		return false // Tah nebyl možný
	}

	g.Moves++

	// Kontrola vítězství
	if g.Board.HasPlayerWon(g.CurrentPlayer) {
		g.GameOver = true
		if g.CurrentPlayer == animal.Dog {
			g.DogWon = true
		} else {
			g.CatWon = true
		}
		return true
	}

	// Kontrola konce hry
	if g.Board.IsGameOver() {
		g.GameOver = true
		return true
	}

	// Přepne na dalšího hráče
	g.SwitchPlayer()
	return true
}

// DisplayState zobrazí aktuální stav hry
func (g *Game) DisplayState() {
	g.Board.Display()

	dogLevel := g.Board.GetHighestLevel(animal.Dog)
	catLevel := g.Board.GetHighestLevel(animal.Cat)

	fmt.Printf("Psi (nejvyšší úroveň): %d/3\n", dogLevel)
	fmt.Printf("Kočky (nejvyšší úroveň): %d/3\n", catLevel)
	fmt.Printf("Celkový počet tahů: %d\n", g.Moves)
	fmt.Printf("Na tahu: %s\n\n", g.GetCurrentPlayerName())
}

// DisplayRules zobrazí pravidla hry
func (g *Game) DisplayRules() {
	fmt.Println("==================================================")
}

// GetWinnerName vrací jméno vítěze
func (g *Game) GetWinnerName() string {
	if g.DogWon {
		return "Psi"
	} else if g.CatWon {
		return "Kočky"
	} else {
		dogLevel := g.Board.GetHighestLevel(animal.Dog)
		catLevel := g.Board.GetHighestLevel(animal.Cat)
		if dogLevel > catLevel {
			return "Psi"
		} else if catLevel > dogLevel {
			return "Kočky"
		} else {
			return "Remíza"
		}
	}
}

// DisplayGameOver zobrazí konečný stav hry
func (g *Game) DisplayGameOver() {
	fmt.Println("\n" + "==================================================")
	fmt.Println("KONEC HRY")
	fmt.Println("==================================================")

	if g.DogWon {
		fmt.Println("PSI")
	} else if g.CatWon {
		fmt.Println("KOČKY")
	} else {
		dogLevel := g.Board.GetHighestLevel(animal.Dog)
		catLevel := g.Board.GetHighestLevel(animal.Cat)

		if dogLevel > catLevel {
			fmt.Println("PSI")
		} else if catLevel > dogLevel {
			fmt.Println("KOČKY")
		} else {
			fmt.Println("REMÍZA")
		}
	}

	fmt.Println("\nKonečné úrovně:")
	fmt.Printf("  Psi:    %d/3\n", g.Board.GetHighestLevel(animal.Dog))
	fmt.Printf("  Kočky:  %d/3\n", g.Board.GetHighestLevel(animal.Cat))
	fmt.Printf("  Tahů:   %d\n", g.Moves)
	fmt.Println("==================================================")
}
