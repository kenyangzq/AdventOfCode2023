using Day10;

public static class Program {
    public static void Main() {
        List<List<string>> maze = ParseInput("input.txt");
        if (maze == null) {
            Console.WriteLine("Could not parse input");
            return;
        }

        var pipes = PipeHelper.CreatePipes();

        if (!TryFindStartingPlace(maze, out int row, out int col)) {
            Console.WriteLine("Could not find starting place");
            return;
        }

        List<Direction> currentDirections = new List<Direction>() { Direction.Up, Direction.Down, Direction.Left, Direction.Right };

        MazeSolver mazeSolver = new MazeSolver(maze, row, col);

        Console.WriteLine($"Starting at {row}, {col}");

        // try every direction
        foreach (var dir in currentDirections) {
            var distance = mazeSolver.TryNextMove(row, col, dir, 0);
            if (distance != -1) {
                Console.WriteLine($"Found distance: {distance}");
                Console.WriteLine($"Answer: {(distance + 1)/ 2}");
                break;
            }
        }
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

    private static bool TryFindStartingPlace(List<List<string>> maze, out int row, out int col) {
        row = -1;
        col = -1;
        for (int i = 0; i < maze.Count; i++)
        {
            for (int j = 0; j < maze[i].Count; j++)
            {
                if (maze[i][j] == "S")
                {
                    row = i;
                    col = j;
                    return true;
                }
            }
        }

        return false;
    }

    private class MazeSolver {
        private List<List<string>> maze;
        private int mazeWidth;
        private int mazeHeight;
        private int startingRow;
        private int startingCol;

        private Dictionary<string, Pipe> pipes;

        public MazeSolver(List<List<string>> maze, int startingRow, int startingCol) {
            this.maze = maze;
            this.mazeWidth = maze[0].Count;
            this.mazeHeight = maze.Count;
            this.startingRow = startingRow;
            this.startingCol = startingCol;

            this.pipes = PipeHelper.CreatePipes();
        }

        public int TryNextMove(int currentRow, int currentCol, Direction dir, int distance)
        {
            Console.WriteLine($"Trying {dir} from {currentRow}, {currentCol}");

            Pipe currentPipe = pipes[maze[currentRow][currentCol]];
            (int nextRow, int nextCol) = PipeHelper.Move(dir, currentRow, currentCol);

            if (nextRow < 0 || nextRow >= mazeHeight || nextCol < 0 || nextCol >= mazeWidth) {
                return -1;
            }

            if (nextRow == startingRow && nextCol == startingCol) {
                return distance;
            }

            List<string> nextSymbols = currentPipe.GetPossibleNextSymbols(dir);
            string nextSymbol = maze[nextRow][nextCol];
            
            // If the current symbol in the maze is not possible for move, give up this search
            if (!nextSymbols.Contains(nextSymbol)) {
                return -1;
            }

            distance++;
            Direction nextDirection = pipes[nextSymbol].GetNextDirection(dir);

            return TryNextMove(nextRow, nextCol, nextDirection, distance);
        }
    }


}