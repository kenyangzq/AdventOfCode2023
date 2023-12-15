package main

import (
	"bufio"
	"fmt"
	"log"
	"os"
	"unicode"
)

func parseFile(filename string) {
	file, err := os.Open(filename)
	if err != nil {
		log.Fatal(err)
	}
	defer file.Close()

	scanner := bufio.NewScanner(file)

	sum := rune(0)
	for scanner.Scan() {
		first, last := extractDigitsWord(scanner.Text())
		fmt.Printf("First: %d, Last: %d from %s\n", first, last, scanner.Text())
		sum += first*10 + last
	}

	fmt.Printf("\n\nSum: %d\n", sum)

	if err := scanner.Err(); err != nil {
		log.Fatal(err)
	}
}

func min(a, b int) int {
	if a < b {
		return a
	}
	return b
}

func extractDigitsWord(s string) (rune, rune) {
	var firstDigit, lastDigit rune

	numbers := map[string]int{
		"one":   1,
		"two":   2,
		"three": 3,
		"four":  4,
		"five":  5,
		"six":   6,
		"seven": 7,
		"eight": 8,
		"nine":  9,
	}

	for i := 0; i < len(s); i++ {
		if unicode.IsDigit(rune(s[i])) {
			firstDigit = rune(s[i]) - '0'
			break
		}

		found := false
		for key, value := range numbers {
			if len(s)-i >= len(key) && s[i:i+len(key)] == key {
				fmt.Printf("Found %s with value %d\n", key, value)
				firstDigit = rune(value)
				found = true
				break
			}
		}
		if found {
			break
		}
	}

	for i := len(s) - 1; i >= 0; i-- {
		if unicode.IsDigit(rune(s[i])) {
			lastDigit = rune(s[i]) - '0'
			break
		}

		found := false
		for key, value := range numbers {
			if i+1 >= len(key) && s[i-len(key)+1:i+1] == key {
				fmt.Printf("Found %s with value %d\n", key, value)
				lastDigit = rune(value)
				found = true
				break
			}
		}
		if found {
			break
		}
	}

	return firstDigit, lastDigit
}

func extractDigits(s string) (rune, rune) {
	var firstDigit, lastDigit rune

	for _, c := range s {
		if unicode.IsDigit(c) {
			firstDigit = rune(c) - '0'
			break
		}
	}

	for i := len(s) - 1; i >= 0; i-- {
		if unicode.IsDigit(rune(s[i])) {
			lastDigit = rune(s[i]) - '0'
			break
		}
	}

	return firstDigit, lastDigit
}

func main() {
	parseFile("./input.txt")
}
