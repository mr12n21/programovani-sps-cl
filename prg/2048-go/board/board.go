// Package board implementuje logiku herní desky
package board

import (
	"fmt"
	"math/rand"
	"time"

	"2048-go/animal"
)

// Board reprezentuje herní desku 4x4
type Board struct {
	Grid    [4][4]animal.Animal
	History [][4][4]animal.Animal
	rng     *rand.Rand
}

// NewBoard vytvoří novou herní desku
func NewBoard() *Board {
	b := &Board{
		History: make([][4][4]animal.Animal, 0),
		rng:     rand.New(rand.NewSource(time.Now().UnixNano())),
	}
	b.AddRandomAnimal("")
	b.AddRandomAnimal("")
	return b
}

// AddRandomAnimal přidá náhodné zvíře na prázdné místo
func (b *Board) AddRandomAnimal(forceType string) bool {
	emptyCells := make([][2]int, 0)
	for i := 0; i < 4; i++ {
		for j := 0; j < 4; j++ {
			if b.Grid[i][j] == nil {
				emptyCells = append(emptyCells, [2]int{i, j})
			}
		}
	}

	if len(emptyCells) == 0 {
		return false
	}

	cell := emptyCells[b.rng.Intn(len(emptyCells))]
	row, col := cell[0], cell[1]

	// Pokud není určen typ, vybere se náhodně
	animalType := forceType
	if animalType == "" {
		types := []string{"dog", "cat"}
		animalType = types[b.rng.Intn(len(types))]
	}

	if animalType == "dog" {
		b.Grid[row][col] = animal.NewDog(1)
	} else {
		b.Grid[row][col] = animal.NewCat(1)
	}

	return true
}

// SaveState uloží aktuální stav desky do historie
func (b *Board) SaveState() {
	var state [4][4]animal.Animal
	for i := 0; i < 4; i++ {
		for j := 0; j < 4; j++ {
			if b.Grid[i][j] != nil {
				state[i][j] = b.Grid[i][j].Copy()
			}
		}
	}
	b.History = append(b.History, state)
}

// Undo vrátí poslední tah
func (b *Board) Undo() bool {
	if len(b.History) == 0 {
		return false
	}
	b.Grid = b.History[len(b.History)-1]
	b.History = b.History[:len(b.History)-1]
	return true
}

// Move provede pohyb na desce
func (b *Board) Move(direction string, player animal.AnimalType) bool {
	b.SaveState()
	originalBoard := b.copyGrid()

	switch direction {
	case "up":
		b.moveUp(player)
	case "down":
		b.moveDown(player)
	case "left":
		b.moveLeft(player)
	case "right":
		b.moveRight(player)
	default:
		b.History = b.History[:len(b.History)-1]
		return false
	}

	// Pokud se deska nezměnila, vrátí false
	if b.boardsEqual(originalBoard, b.Grid) {
		b.History = b.History[:len(b.History)-1]
		return false
	}

	// Přidá nové zvíře
	b.AddRandomAnimal("")
	return true
}

func (b *Board) moveLeft(player animal.AnimalType) {
	for i := 0; i < 4; i++ {
		row := make([]animal.Animal, 0)
		for j := 0; j < 4; j++ {
			if b.Grid[i][j] != nil {
				row = append(row, b.Grid[i][j])
			}
		}
		row = b.merge(row, player)
		for j := 0; j < 4; j++ {
			if j < len(row) {
				b.Grid[i][j] = row[j]
			} else {
				b.Grid[i][j] = nil
			}
		}
	}
}

func (b *Board) moveRight(player animal.AnimalType) {
	for i := 0; i < 4; i++ {
		row := make([]animal.Animal, 0)
		for j := 0; j < 4; j++ {
			if b.Grid[i][j] != nil {
				row = append(row, b.Grid[i][j])
			}
		}
		row = b.mergeReverse(row, player)
		for j := 0; j < 4; j++ {
			if j < len(row) {
				b.Grid[i][3-j] = row[len(row)-1-j]
			} else {
				b.Grid[i][3-j] = nil
			}
		}
	}
}

func (b *Board) moveUp(player animal.AnimalType) {
	for j := 0; j < 4; j++ {
		col := make([]animal.Animal, 0)
		for i := 0; i < 4; i++ {
			if b.Grid[i][j] != nil {
				col = append(col, b.Grid[i][j])
			}
		}
		col = b.merge(col, player)
		for i := 0; i < 4; i++ {
			if i < len(col) {
				b.Grid[i][j] = col[i]
			} else {
				b.Grid[i][j] = nil
			}
		}
	}
}

func (b *Board) moveDown(player animal.AnimalType) {
	for j := 0; j < 4; j++ {
		col := make([]animal.Animal, 0)
		for i := 0; i < 4; i++ {
			if b.Grid[i][j] != nil {
				col = append(col, b.Grid[i][j])
			}
		}
		col = b.mergeReverse(col, player)
		for i := 0; i < 4; i++ {
			if i < len(col) {
				b.Grid[3-i][j] = col[len(col)-1-i]
			} else {
				b.Grid[3-i][j] = nil
			}
		}
	}
}

func (b *Board) merge(animals []animal.Animal, player animal.AnimalType) []animal.Animal {
	if len(animals) == 0 {
		return []animal.Animal{}
	}

	result := []animal.Animal{animals[0]}

	for i := 1; i < len(animals); i++ {
		current := animals[i]
		last := result[len(result)-1]

		merged := animal.CombineAnimals(last, current, player)

		if merged != nil {
			result[len(result)-1] = merged
		} else {
			result = append(result, current)
		}
	}

	return result
}

func (b *Board) mergeReverse(animals []animal.Animal, player animal.AnimalType) []animal.Animal {
	// Otočí pole
	reversed := make([]animal.Animal, len(animals))
	for i := 0; i < len(animals); i++ {
		reversed[i] = animals[len(animals)-1-i]
	}

	merged := b.merge(reversed, player)

	// Otočí zpět
	result := make([]animal.Animal, len(merged))
	for i := 0; i < len(merged); i++ {
		result[i] = merged[len(merged)-1-i]
	}

	return result
}

// IsGameOver zkontroluje, zda je hra skončena
func (b *Board) IsGameOver() bool {
	// Pokud jsou volná místa, hra neskončila
	for i := 0; i < 4; i++ {
		for j := 0; j < 4; j++ {
			if b.Grid[i][j] == nil {
				return false
			}
		}
	}

	// Zkontroluje možné slučovací tahy
	for i := 0; i < 4; i++ {
		for j := 0; j < 4; j++ {
			current := b.Grid[i][j]

			// Kontrola vpravo
			if j < 3 {
				right := b.Grid[i][j+1]
				if b.canCombine(current, right) {
					return false
				}
			}

			// Kontrola dolů
			if i < 3 {
				down := b.Grid[i+1][j]
				if b.canCombine(current, down) {
					return false
				}
			}
		}
	}

	return true
}

func (b *Board) canCombine(a1, a2 animal.Animal) bool {
	if a1 == nil || a2 == nil {
		return false
	}
	// Mohou se spojit, pokud mají stejný typ a úroveň, nebo různé úrovně
	return (a1.GetType() == a2.GetType() && a1.GetLevel() == a2.GetLevel()) ||
		(a1.GetLevel() != a2.GetLevel())
}

// HasPlayerWon zkontroluje, zda hráč vyhrál
func (b *Board) HasPlayerWon(player animal.AnimalType) bool {
	for i := 0; i < 4; i++ {
		for j := 0; j < 4; j++ {
			a := b.Grid[i][j]
			if a != nil && a.GetType() == player && a.GetLevel() == 3 {
				return true
			}
		}
	}
	return false
}

// GetHighestLevel vrací nejvyšší úroveň zvířete daného hráče
func (b *Board) GetHighestLevel(player animal.AnimalType) int {
	maxLevel := 0
	for i := 0; i < 4; i++ {
		for j := 0; j < 4; j++ {
			a := b.Grid[i][j]
			if a != nil && a.GetType() == player {
				if a.GetLevel() > maxLevel {
					maxLevel = a.GetLevel()
				}
			}
		}
	}
	return maxLevel
}

// Display zobrazí hrací desku
func (b *Board) Display() {
	fmt.Println("\n" + "========================================")
	for i := 0; i < 4; i++ {
		rowStr := ""
		for j := 0; j < 4; j++ {
			a := b.Grid[i][j]
			var cell string
			if a == nil {
				cell = "[  ----  ]"
			} else {
				name := a.GetName()
				cell = fmt.Sprintf("[ %-6s ]", name)
			}
			rowStr += cell
		}
		fmt.Println(rowStr)
	}
	fmt.Println("========================================\n")
}

func (b *Board) copyGrid() [4][4]animal.Animal {
	var copy [4][4]animal.Animal
	for i := 0; i < 4; i++ {
		for j := 0; j < 4; j++ {
			if b.Grid[i][j] != nil {
				copy[i][j] = b.Grid[i][j].Copy()
			}
		}
	}
	return copy
}

func (b *Board) boardsEqual(b1, b2 [4][4]animal.Animal) bool {
	for i := 0; i < 4; i++ {
		for j := 0; j < 4; j++ {
			if !animal.Equals(b1[i][j], b2[i][j]) {
				return false
			}
		}
	}
	return true
}
