package main

import (
	"bufio"
	"fmt"
	"log"
	"os"
	"unicode"
)

var directions = []struct{ dx, dy int }{
	{-1, -1}, {-1, 0}, {-1, 1},
	{0, -1}, {0, 1},
	{1, -1}, {1, 0}, {1, 1},
}

func parseGrid(file *os.File) [][]rune {
	var grid [][]rune
	scanner := bufio.NewScanner(file)
	for scanner.Scan() {
		line := scanner.Text()
		grid = append(grid, []rune(line))
	}

	return grid
}

func processGridAndGetSum(grid [][]rune) int {

	width := len(grid[0])
	height := len(grid)

	valid := false
	curValue := 0

	sum := 0

	// Process the grid
	for i, row := range grid {
		valid = false
		curValue = 0
		for j, cell := range row {

			// Cur is digit, multiply prev value by 10 and add cur value
			// update valid by checking up and down and left and right
			if unicode.IsDigit(cell) {

				curValue = curValue*10 + int(cell-'0')
				if !valid {
					for _, dir := range directions {
						nx, ny := i+dir.dx, j+dir.dy
						if nx >= 0 && nx < width && ny >= 0 && ny < height {
							if !unicode.IsDigit(grid[nx][ny]) && grid[nx][ny] != '.' {
								valid = true
							}
						}
					}
				}

				// stop process if j == width - 1
				if j == width-1 {
					if valid {
						sum += curValue
					}
				}
			} else {
				if valid {
					sum += curValue
					valid = false
				}
				curValue = 0
			}
		}
	}

	return sum
}

func processGridAndGetRatios(grid [][]rune) int {

	width := len(grid[0])
	height := len(grid)

	gears := make(map[int][]int)
	gearIndex := make(map[int]bool)

	valid := false
	curValue := 0

	sum := 0

	// Process the grid
	for i, row := range grid {
		valid = false
		curValue = 0
		gearIndex = make(map[int]bool)

		for j, cell := range row {

			// Cur is digit, multiply prev value by 10 and add cur value
			// update valid by all directions
			if unicode.IsDigit(cell) {

				curValue = curValue*10 + int(cell-'0')
				if !valid {
					for _, dir := range directions {
						nx, ny := i+dir.dx, j+dir.dy
						if nx >= 0 && nx < width && ny >= 0 && ny < height {

							// if valid, record the index of *
							if !unicode.IsDigit(grid[nx][ny]) && grid[nx][ny] == '*' {
								gearIndex[nx*height+ny] = true
								valid = true
							}
						}
					}
				}

				// stop process if j == width - 1
				if j == width-1 {
					if valid {
						for index, value := range gearIndex {
							if value {
								// add the value to the gear map
								gears[index] = append(gears[index], curValue)
							}
						}
					}
				}
			} else {
				if valid {
					for index, value := range gearIndex {
						if value {
							// add the value to the gear map
							gears[index] = append(gears[index], curValue)
						}
					}
				}
				valid = false
				gearIndex = make(map[int]bool)
				curValue = 0
			}
		}
	}

	for _, values := range gears {
		// find exact 2 match and add up
		if len(values) == 2 {
			sum += values[0] * values[1]
		}
	}

	return sum
}

func parseFile(filename string) {
	file, err := os.Open(filename)
	if err != nil {
		log.Fatal(err)
	}
	defer file.Close()

	grid := parseGrid(file)

	sum := processGridAndGetRatios(grid)

	fmt.Printf("\n\nSum: %d\n", sum)
}

func main() {
	parseFile("./input.txt")
}
