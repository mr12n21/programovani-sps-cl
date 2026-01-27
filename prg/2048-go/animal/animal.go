package animal

import "fmt"

type AnimalType string

const (
	Dog AnimalType = "dog"
	Cat AnimalType = "cat"
)

type Animal interface {
	GetType() AnimalType
	GetLevel() int
	GetName() string
	Evolve() Animal
	Copy() Animal
}

type BaseAnimal struct {
	Type  AnimalType
	Level int
}

func (a *BaseAnimal) GetType() AnimalType {
	return a.Type
}

func (a *BaseAnimal) GetLevel() int {
	return a.Level
}

type DogAnimal struct {
	BaseAnimal
}

func NewDog(level int) *DogAnimal {
	return &DogAnimal{
		BaseAnimal: BaseAnimal{
			Type:  Dog,
			Level: level,
		},
	}
}

func (d *DogAnimal) GetName() string {
	names := map[int]string{
		1: "Štěně",
		2: "Pes",
		3: "Vlk",
	}
	if name, ok := names[d.Level]; ok {
		return name
	}
	return "Neznámý pes"
}

// Evolve evoluuje psa na další úroveň
func (d *DogAnimal) Evolve() Animal {
	if d.Level < 3 {
		return NewDog(d.Level + 1)
	}
	return nil
}

// Copy vytvoří kopii psa
func (d *DogAnimal) Copy() Animal {
	return NewDog(d.Level)
}

// String pro výpis
func (d *DogAnimal) String() string {
	return fmt.Sprintf("%s (level %d)", d.GetName(), d.Level)
}

// CatAnimal struktura pro kočky
type CatAnimal struct {
	BaseAnimal
}

// NewCat vytvoří novou kočku
func NewCat(level int) *CatAnimal {
	return &CatAnimal{
		BaseAnimal: BaseAnimal{
			Type:  Cat,
			Level: level,
		},
	}
}

// GetName vrací jméno kočky podle úrovně
func (c *CatAnimal) GetName() string {
	names := map[int]string{
		1: "Kotě",
		2: "Kočka",
		3: "Lev",
	}
	if name, ok := names[c.Level]; ok {
		return name
	}
	return "Neznámá kočka"
}

// Evolve evoluuje kočku na další úroveň
func (c *CatAnimal) Evolve() Animal {
	if c.Level < 3 {
		return NewCat(c.Level + 1)
	}
	return nil
}

// Copy vytvoří kopii kočky
func (c *CatAnimal) Copy() Animal {
	return NewCat(c.Level)
}

// String pro výpis
func (c *CatAnimal) String() string {
	return fmt.Sprintf("%s (level %d)", c.GetName(), c.Level)
}

// CombineAnimals kombinuje dvě zvířata podle pravidel hry
func CombineAnimals(a1, a2 Animal, currentPlayer AnimalType) Animal {
	if a1 == nil || a2 == nil {
		return nil
	}

	// Stejná úroveň
	if a1.GetLevel() == a2.GetLevel() {
		// Stejný typ: evoluují
		if a1.GetType() == a2.GetType() {
			return a1.Evolve()
		}
		// Různý typ: vyhraje typ aktuálního hráče
		if a1.GetType() == currentPlayer {
			return a1
		} else if a2.GetType() == currentPlayer {
			return a2
		}
		// Jinak se nic neděje
		return nil
	}

	// Různá úroveň: vyhraje vyšší
	if a1.GetLevel() > a2.GetLevel() {
		return a1
	}
	return a2
}

// Equals porovná dvě zvířata
func Equals(a1, a2 Animal) bool {
	if a1 == nil && a2 == nil {
		return true
	}
	if a1 == nil || a2 == nil {
		return false
	}
	return a1.GetType() == a2.GetType() && a1.GetLevel() == a2.GetLevel()
}
