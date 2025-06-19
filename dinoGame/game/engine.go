package game

import (
	"math/rand"
	"time"
)

func RunGame() {
	rand.Seed(time.Now().UnixNano())
	p := Player{X: 1, Y: Height - 2}
	obs := []Obstacle{}
	frame, score := 0, 0
	for {
		if frame%20 == 0 {
			obs = append(obs, SpawnObstacle())
		}
		ReadInput(&p)
		if p.IsJumping {
			p.Y -= p.Velocity
			p.Velocity--
			if p.Y >= Height-2 {
				p.Y, p.IsJumping, p.Velocity = Height-2, false, 0
			}
			if p.Y < 0 {
				p.Y = 0
			}
		}
		if p.IsDucking && !p.IsJumping {
			p.Y = Height - 1
		} else if !p.IsJumping {
			p.Y = Height - 2
		}
		for i := range obs {
			obs[i].X--
		}
		alive := []Obstacle{}
		for _, o := range obs {
			if o.X+o.Width > 0 {
				alive = append(alive, o)
			}
		}
		obs = alive
		for _, o := range obs {
			for w := 0; w < o.Width; w++ {
				for h := 0; h < o.Height; h++ {
					if (p.X == o.X+w || p.X+1 == o.X+w) && p.Y == o.Y-h {
						ClearScreen()
						println("Game Over!")
						println("Score:", score)
						return
					}
				}
			}
		}
		score++
		Render(&p, obs, score)
		Sleep(50)
		frame++
	}
}
