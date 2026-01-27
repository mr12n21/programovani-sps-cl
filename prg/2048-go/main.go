package main

import (
	"bufio"
	"fmt"
	"os"
	"strings"

	"2048-go/game"
)

func printWelcome() {
	fmt.Println("==================================================")
}

func printHelp() {
	fmt.Println(`
DOSTUPNÉ PŘÍKAZY:
  up, down, left, right  - Pohyb zvířat v daném směru
  undo                   - Vrátit poslední tah
  help                   - Zobrazit tuto nápovědu
  rules                  - Zobrazit pravidla
  quit, exit             - Konec hry`)
}

func main() {
	printWelcome()
	g := game.NewGame()
	g.DisplayRules()
	fmt.Println("start")
	g.DisplayState()
	scanner := bufio.NewScanner(os.Stdin)

	for !g.GameOver {
		fmt.Printf("%s, zadej tah (up/down/left/right): ", g.GetCurrentPlayerName())
		if !scanner.Scan() {
			break
		}
		userInput := strings.TrimSpace(strings.ToLower(scanner.Text()))
		if userInput == "" {
			continue
		}

		switch userInput {
		case "quit", "exit":
			fmt.Println("\nHra ukončena.")
			return
		case "help":
			printHelp()
		case "rules":
			g.DisplayRules()
		case "undo":
			if g.Board.Undo() {
				g.Moves--
				fmt.Println("Poslední tah vrácen.")
				g.DisplayState()
			} else {
				fmt.Println("Nelze vrátit tah - historie je prázdná.")
			}
		case "up", "down", "left", "right":
			if g.ProcessMove(userInput) {
				g.DisplayState()
				if g.GameOver {
					g.DisplayGameOver()
				}
			} else {
				fmt.Println("Tah nebyl možný - zkus jiný směr.")
			}
		default:
			fmt.Printf("Neznámý příkaz: '%s'. Zadej 'help' pro nápovědu.\n\n", userInput)
		}
	}

	if err := scanner.Err(); err != nil {
		fmt.Fprintf(os.Stderr, "Chyba při čtení vstupu: %v\n", err)
		os.Exit(1)
	}
}
