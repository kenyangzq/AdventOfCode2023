

public class Program {
    public static void Main() {
        var instructions = ParseInput("input.txt");

        var (grid, curx, cury) = MeasureGrid(instructions);

        GridSolver solver = new GridSolver(grid);

        solver.MarkGrid(instructions, curx, cury);
        
        // Part 1
        // Count the boundary-connected .s
        // int count = solver.CountBoundaryConnectedDots();
        // solver.PrintGrid();

        // int answer = (grid.Count * grid[0].Count) - count;
        // Console.WriteLine($"Answer: {answer}");

        // Part 2
        var instructions2 = ConvertInput(instructions);
        var area = CalculateArea(instructions2);

        Console.WriteLine($"Area: {area}");
    }

    public enum Direction {
        R = 0,
        D = 1,
        L = 2,
        U = 3,
    }

    public static Dictionary<Direction, (int, int)> DirectionMap = new Dictionary<Direction, (int, int)> {
        {Direction.R, (0, 1)},
        {Direction.D, (1, 0)},
        {Direction.L, (0, -1)},
        {Direction.U, (-1, 0)},
    };

    public static List<(Direction, int, string)> ParseInput(string fileName) {
        var input = System.IO.File.ReadAllLines(fileName);

        List<(Direction, int, string)> instructions = new List<(Direction, int, string)>();

        foreach (var line in input) {
            var values = line.Split(" ", StringSplitOptions.RemoveEmptyEntries);
            Direction d = Enum.Parse<Direction>(values[0]);
            int value = int.Parse(values[1]);
            string color = values[2].Replace("(", "").Replace(")", "");

            instructions.Add((d, value, color));
        }

        return instructions;
    }

    public static List<(Direction, long)> ConvertInput(List<(Direction, int, string)> input) {
        List<(Direction, long)> instructions = new();

        foreach (var (_, _, hex) in input) {
            Direction d = (Direction)(int)char.GetNumericValue(hex[6]);
            long value = Convert.ToInt64(hex.Substring(1, 5), 16);

            instructions.Add((d, value));
        }

        return instructions;
    }

    // Assuming we start from 0, 0,
    // Return grid and starting point.
    private static (List<List<string>>, int, int) MeasureGrid(List<(Direction, int, string)> instructions) {
        int minx = 0;
        int miny = 0;
        int maxx = 0;
        int maxy = 0;

        int curx = 0, cury = 0;

        foreach (var (d, value, color) in instructions) {
            switch (d) {
                case Direction.R:
                    curx += value;
                    break;
                case Direction.D:
                    cury -= value;
                    break;
                case Direction.L:
                    curx -= value;
                    break;
                case Direction.U:
                    cury += value;
                    break;
            }

            if (curx < minx) {
                minx = curx;
            }

            if (curx > maxx) {
                maxx = curx;
            }

            if (cury < miny) {
                miny = cury;
            }

            if (cury > maxy) {
                maxy = cury;
            }
        }

        Console.WriteLine($"minx: {minx}, maxx: {maxx}, miny: {miny}, maxy: {maxy}");

        int width = maxx - minx + 1;
        int height = maxy - miny + 1;

        List<List<string>> grid = new List<List<string>>();
        for (int i = 0; i < height; i++) {
            grid.Add(Enumerable.Repeat(".", width).ToList());
        }

        return (grid, maxy, 0 - minx);
    }

    private static long CalculateArea(List<(Direction, long)> instructions) {
        long area = 0;

        // Calculate the area using Shoelace formula
        long curx = 0, cury = 0;
        long nextx = 0, nexty = 0;
        long pathSize = 0;
        
        // Start from 2nd point.
        for (int i = 0; i <  instructions.Count; i++) {
            var instruction = instructions[i];

            // find next point
            nextx = curx + DirectionMap[instruction.Item1].Item1 * instruction.Item2;
            nexty = cury + DirectionMap[instruction.Item1].Item2 * instruction.Item2;
            
            pathSize += instruction.Item2;

            // Double the area
            area += (nextx - curx) * (cury + nexty);

            curx = nextx;
            cury = nexty;    
        }

        // Last point to first point
        area  += (0 - curx)*(cury + 0);
        pathSize += cury;

        // half the area + half path size + 1;
        area = area / 2 + pathSize / 2 + 1;

        return area;
    }

    private class GridSolver {
        List<List<string>> grid;
        int width;
        int height;

        public GridSolver(List<List<string>> grid) {
            this.grid = grid;
            this.width = grid[0].Count;
            this.height = grid.Count;
        }

        public void MarkGrid(List<(Direction, int, string)> instructions, int x, int y) {
            
            grid[x][y] = "#";
            foreach (var instruction in instructions) {

                for (int i = 0; i < instruction.Item2; i++) {
                    x += DirectionMap[instruction.Item1].Item1;
                    y += DirectionMap[instruction.Item1].Item2;
                    
                    if (x < 0 || x >= height || y < 0 || y >= width) {
                        Console.WriteLine($"Out of bounds: {x}, {y}");
                        Console.Read();
                    }
                    
                    grid[x][y] = "#";
                }
            }
        }

        public int CountBoundaryConnectedDots() {
            for (int i = 0; i < height; i++) {
                MarkBoundaryConnectedDots(0, i);
                MarkBoundaryConnectedDots(height - 1, i);
            }

            for (int i = 0; i < width; i++) {
                MarkBoundaryConnectedDots(i, 0);
                MarkBoundaryConnectedDots(i, width - 1);
            }

            int count = 0;
            for (int i = 0; i < height; i++) {
                for (int j = 0; j < width; j++) {
                    count += grid[i][j] == "X" ? 1 : 0;
                }
            }

            return count;
        }

        private void MarkBoundaryConnectedDots(int x, int y) {
            if (x < 0 || x >= height || y < 0 || y >= width) {
                return;
            }

            if (grid[x][y] == "#" || grid[x][y] == "X") {
                return;
            }

            grid[x][y] = "X";

            MarkBoundaryConnectedDots(x + 1, y);
            MarkBoundaryConnectedDots(x - 1, y);
            MarkBoundaryConnectedDots(x, y + 1);
            MarkBoundaryConnectedDots(x, y - 1);
        }

        public void PrintGrid() {
            Console.WriteLine($"width: {width}, height: {height}");

            for (int i = 0; i < height; i++) {
                Console.WriteLine(string.Join("", grid[i]));
            }
        }
    }

}