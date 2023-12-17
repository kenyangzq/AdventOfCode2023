public class Program {

    public static void Main(string[] args) {
        var matrix = ParseInput("input.txt");
        var simulator = new BeamSimulator(matrix);

        // Part 1
        // simulator.FireBeam(0, -1, Direction.Right);
        var part1_answer = simulator.GetCount(0, 0, Direction.Right);
        Console.WriteLine($"Energized count: {part1_answer}");

        // Part 2
        var part2_answer = 0;

        for (int i = 0; i < matrix.Count; i++)
        {
            part2_answer = Math.Max(part2_answer, simulator.GetCount(i, 0, Direction.Right));
            part2_answer = Math.Max(part2_answer, simulator.GetCount(i, matrix[0].Count - 1, Direction.Left));
        }

        for (int j =0; j < matrix[0].Count; j++) {
            part2_answer = Math.Max(part2_answer, simulator.GetCount(0, j, Direction.Down));
            part2_answer = Math.Max(part2_answer, simulator.GetCount(matrix.Count - 1, j, Direction.Up));
        }

        Console.WriteLine($"Max energized count: {part2_answer}");
    }

    private static List<List<string>> ParseInput(string fileName) {
        var lines = File.ReadAllLines(fileName);
        var data = lines.Select(l => l.Select(c => c.ToString()).ToList()).ToList();
        foreach (var line in data)
        {
            Console.WriteLine(string.Join(", ", line));
        }

        return data;
    }

    public enum Direction {
        Unknown,
        Up,
        Left,
        Right,
        Down,
    }



    private class BeamSimulator {
        List<List<string>> matrix;
        List<List<Direction>> energizedMark;

        int width;
        
        int height;

        private static Dictionary<Direction, List<int>> directionMap = new Dictionary<Direction, List<int>> {
            {Direction.Up, new List<int> {-1, 0}},
            {Direction.Left, new List<int> {0, -1}},
            {Direction.Right, new List<int> {0, 1}},
            {Direction.Down, new List<int> {1, 0}},
        };

        public BeamSimulator(List<List<string>> matrix) {
            this.matrix = matrix;
            this.width = matrix[0].Count;
            this.height = matrix.Count;

            // create a energized mark matrix of same size
            energizedMark = new List<List<Direction>>();
            for (int i = 0; i < height; i++)
            {
                energizedMark.Add(Enumerable.Repeat(Direction.Unknown, width).ToList());
            }
        }

        // Part 1
        public void FireBeam(int prevRow, int prevCol, Direction dir) {
            int curRow = directionMap[dir][0] + prevRow;
            int curCol = directionMap[dir][1] + prevCol;

            if (curRow < 0 || curRow >= height || curCol < 0 || curCol >= width) {
                return;
            }

            if (energizedMark[curRow][curCol] == dir) {
                // Console.WriteLine($"Already energized at {prevRow}, {prevCol} with same direction: {dir}");
                return;
            }

            if (energizedMark[curRow][curCol] != Direction.Unknown) {
                Console.WriteLine($"Previously energized at {curRow}, {curCol} with different direction: prev {energizedMark[curRow][curCol]}, current {dir}");
            }

            energizedMark[curRow][curCol] = dir;
            var curSymbol = matrix[curRow][curCol];

            GetNextDirections(curRow, curCol, dir).ForEach(d => FireBeam(curRow, curCol, d));
        }

        // Part 2 BFS
        public int GetCount(int row, int col, Direction dir) {
            var queue = new Queue<(int, int, Direction)>();
            queue.Enqueue((row, col, dir));

            var visited = new HashSet<(int, int, Direction)>();
            visited.Add((row, col, dir));

            while (queue.Any()) {
                var (curRow, curCol, curDir) = queue.Dequeue();
                var curSymbol = matrix[curRow][curCol];

                GetNextDirections(curRow, curCol, curDir).ForEach(d => {
                    var nextRow = directionMap[d][0] + curRow;
                    var nextCol = directionMap[d][1] + curCol;

                    if (nextRow < 0 || nextRow >= height || nextCol < 0 || nextCol >= width) {
                        return;
                    }

                    if (visited.Contains((nextRow, nextCol, d))) {
                        return;
                    }

                    visited.Add((nextRow, nextCol, d));
                    queue.Enqueue((nextRow, nextCol, d));
                });
            }
            
            var count = new HashSet<(int, int)>(visited.Select(v => (v.Item1, v.Item2))).Count();

            return count;
        }


        public List<Direction> GetNextDirections(int curRow, int curCol, Direction dir){
            var curSymbol = matrix[curRow][curCol];
            switch (curSymbol) {
                case ".":
                    return new List<Direction> {dir};
                case "|":
                    if (dir == Direction.Up || dir == Direction.Down) {
                        return new List<Direction> {dir};
                    } else {
                        return new List<Direction> {Direction.Up, Direction.Down};
                    }
                case "-":
                    if (dir == Direction.Left || dir == Direction.Right) {
                        return new List<Direction> {dir};
                    } else {
                        return new List<Direction> {Direction.Left, Direction.Right};
                    }
                case "/":
                    if (dir == Direction.Up) {
                        return new List<Direction> {Direction.Right};
                    } else if (dir == Direction.Right) {
                        return new List<Direction> {Direction.Up};
                    } else if (dir == Direction.Down) {
                        return new List<Direction> {Direction.Left};
                    } else { //(dir == Direction.Left)
                        return new List<Direction> {Direction.Down};
                    }
                case "\\":
                    if (dir == Direction.Up) {
                        return new List<Direction> {Direction.Left};
                    } else if (dir == Direction.Right) {
                        return new List<Direction> {Direction.Down};
                    } else if (dir == Direction.Down) {
                        return new List<Direction> {Direction.Right};
                    } else { //(dir == Direction.Left)
                        return new List<Direction> {Direction.Up};
                    }
                default:
                    throw new Exception("Invalid symbol");
            }
        }

        public int GetEnergizedCount() {
            int count = 0;
            for (int i = 0; i < height; i++)
            {
                count += energizedMark[i].Count(d => d != Direction.Unknown);
            }

            return count;
        }
    }



}