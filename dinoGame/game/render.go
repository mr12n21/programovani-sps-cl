package game

import (
	"fmt"
	"strings"
)

func ClearScreen() {
	fmt.Print("\033[H\033[2J")
}

func Render(p *Player, obs []Obstacle, score int) {
	buf := make([][]string, Height)
	for i := range buf {
		buf[i] = make([]string, Width)
		for j := range buf[i] {
			buf[i][j] = " "
		}
	}
	for x := 0; x < Width; x++ {
		buf[Height-1][x] = "─"
	}
	sym := "🦖"
	if p.IsDucking {
		sym = "v"
	} else if p.IsJumping {
		if p.Velocity > 0 {
			sym = "↑"
		} else {
			sym = "↓"
		}
	}
	for w := 0; w < 2; w++ {
		if p.X+w < Width {
			buf[p.Y][p.X+w] = sym
		}
	}
	for _, o := range obs {
		for w := 0; w < o.Width; w++ {
			for h := 0; h < o.Height; h++ {
				x := o.X + w
				y := o.Y - h
				if x >= 0 && x < Width && y >= 0 && y < Height {
					buf[y][x] = o.Symbol
				}
			}
		}
	}
	ClearScreen()
	for _, row := range buf {
		fmt.Println(strings.Join(row, ""))
	}
	fmt.Printf("\nSkóre: %d\n", score)
}
