
public class Program {

    public static void Main(string[] args) {
        var data = ParseInput("test.txt");
        var solver = new Solver(data);
        
        // Part 1
        var answer = solver.Solve(0, 0, Direction.Right);
        Console.WriteLine($"Result: {answer}");
    }

    private static List<List<int>> ParseInput(string fileName) {
        var lines = File.ReadAllLines(fileName);
        var data = lines.Select(l => l.Select(c => int.Parse(c.ToString())).ToList()).ToList();
        foreach (var line in data)
        {
            Console.WriteLine(string.Join(", ", line));
        }

        return data;
    }

    public enum Direction {
        Up = 0,
        Left = 1,
        Right = 2,
        Down = 3,
    }

    private static Dictionary<Direction, List<int>> DirectionMap = new Dictionary<Direction, List<int>> {
        {Direction.Up, new List<int> {-1, 0}},
        {Direction.Left, new List<int> {0, -1}},
        {Direction.Right, new List<int> {0, 1}},
        {Direction.Down, new List<int> {1, 0}},
    };

    public class Solver {
        List<List<int>> matrix;
        int width;
        int height;
        const int StraightCountLimit = 3;

        public Solver(List<List<int>> matrix) {
            this.matrix = matrix;
            this.width = matrix[0].Count;
            this.height = matrix.Count;
        }

        // Dijkstra's algorithm
        public int Solve(int row, int col, Direction dir) {
            // row, col, direction, straightCount, valueCount
            PriorityQueue<(int, int, Direction, int, int), int> heapq = new PriorityQueue<(int, int, Direction, int, int), int>();
            HashSet<(int, int, Direction, int)> visited = new();

            heapq.Enqueue((row, col, dir, 1, 0), 0);

            while (heapq.TryDequeue(out var values, out _)) {
                var (curRow, curCol, curDir, straightCount, valueCount) = values;

                if (curRow == height - 1 && curCol == width - 1) {
                    return valueCount;
                }

                visited.Add((curRow, curCol, curDir, straightCount));

                foreach (Direction direction in Enum.GetValues(typeof(Direction)))
                {
                    // If straightCount > limit, skip
                    if (curDir == direction && straightCount >= StraightCountLimit) {
                        continue;
                    }

                    // opposite direction not allowed
                    if ((int)direction + (int)curDir == 3) {
                        continue;
                    }

                    var nextRow = curRow + DirectionMap[direction][0];
                    var nextCol = curCol + DirectionMap[direction][1];

                    // Out of bound
                    if (nextRow < 0 || nextRow >= height || nextCol < 0 || nextCol >= width) {
                        continue;
                    }

                    // next value count
                    var nextValueCount = valueCount + matrix[nextRow][nextCol];

                    // if has been visited then skip
                    if (visited.Contains((nextRow, nextCol, direction, straightCount))) {
                        continue;
                    }
                    heapq.Enqueue((nextRow, nextCol, direction, direction == curDir ? straightCount + 1 : 1, nextValueCount), nextValueCount);
                }
            }

            return -1;
        }
    }
}

