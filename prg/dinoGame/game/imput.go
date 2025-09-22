package game

import (
	"os"
	"syscall"
	"time"

	"golang.org/x/term"
)

func ReadInput(p *Player) {
	old, _ := term.MakeRaw(int(syscall.Stdin))
	defer term.Restore(int(syscall.Stdin), old)
	syscall.SetNonblock(syscall.Stdin, true)

	buf := make([]byte, 1)
	n, _ := os.Stdin.Read(buf)
	if n == 0 {
		p.IsDucking = false
		return
	}
	switch buf[0] {
	case 'w', 'W':
		if !p.IsJumping {
			p.IsJumping = true
			p.Velocity = 5
		}
		
	case 's', 'S':
		if !p.IsJumping {
			p.IsDucking = true
		}
	}
	time.Sleep(5 * time.Millisecond)
}
