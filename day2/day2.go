package main

import (
	"bufio"
	"fmt"
	"log"
	"os"
	"regexp"
	"strconv"
	"strings"
)

func parseGame(s string) (int, map[string]int) {
	parts := strings.Split(s, ": ")
	gameID, _ := strconv.Atoi(parts[0][5:])
	sections := strings.Split(parts[1], "; ")

	colorCount := make(map[string]int)
	colorCount["red"] = 0
	colorCount["blue"] = 0
	colorCount["green"] = 0

	for _, section := range sections {

		re := regexp.MustCompile(`\d+ \w+`)
		matches := re.FindAllString(section, -1)
		for _, match := range matches {
			parts := strings.Split(match, " ")
			count, _ := strconv.Atoi(parts[0])
			color := parts[1]
			if count > colorCount[color] {
				colorCount[color] = count
			}
		}
	}

	return gameID, colorCount
}

func parseGameAndGetGameId(s string, redCount int, greenCount int, blueCount int) int {
	gameID, colorCount := parseGame(s)
	fmt.Printf("maxRedCount: %d, maxBlueCount: %d, maxGreenCount: %d\n", colorCount["red"], colorCount["blue"], colorCount["green"])

	if colorCount["red"] <= redCount && colorCount["blue"] <= blueCount && colorCount["green"] <= greenCount {
		return gameID
	}

	return 0
}

func parseGameAndGetPower(s string, redCount int, greenCount int, blueCount int) int {
	_, colorCount := parseGame(s)
	product := colorCount["red"] * colorCount["blue"] * colorCount["green"]
	fmt.Printf("minRedCount: %d, minBlueCount: %d, minGreenCount: %d, Product: %d\n", colorCount["red"], colorCount["blue"], colorCount["green"], product)

	return product
}

func parseFile(filename string) {
	file, err := os.Open(filename)
	if err != nil {
		log.Fatal(err)
	}
	defer file.Close()

	scanner := bufio.NewScanner(file)

	sum := 0
	for scanner.Scan() {
		sum += parseGameAndGetPower(scanner.Text(), 12, 13, 14)
	}

	fmt.Printf("\n\nSum: %d\n", sum)

	if err := scanner.Err(); err != nil {
		log.Fatal(err)
	}
}

func main() {
	parseFile("./input1.txt")
}
