package game

import "math/rand"

func SpawnObstacle() Obstacle {
	if rand.Intn(2) == 0 {
		return Obstacle{X: Width - 1, Y: Height - 2, Width: 1, Height: 3, Symbol: "##"}
	}
	return Obstacle{X: Width - 1, Y: Height - 1, Width: 3, Height: 1, Symbol: "--"}
}
