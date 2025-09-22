package game

const (
	Width  = 80
	Height = 12
)

type Player struct {
	X         int
	Y         int
	Velocity  int
	IsJumping bool
	IsDucking bool
}

type Obstacle struct {
	X      int
	Y      int
	Width  int
	Height int
	Symbol string
}
