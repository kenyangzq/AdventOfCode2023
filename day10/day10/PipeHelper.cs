
namespace Day10{

    /// <summary>
    /// The direction going into the current pipe
    /// </summary>
    public enum Direction {
        Up = 0,
        Left = 1,
        Right = 2,
        Down = 3,
    }

    /// <summary>
    /// A pipe that can be connected to other pipes
    /// </summary>
    public class Pipe
    {
        private static readonly Dictionary<Direction, List<string>> possibleDirectionToSymbols = new Dictionary<Direction, List<string>>()
            {
                { Direction.Up, new List<string>() { "|", "7", "F"} },
                { Direction.Down, new List<string>() { "|", "L", "J"} },
                { Direction.Left, new List<string>() { "-", "F", "L"} },
                { Direction.Right, new List<string>() { "-", "J", "7"} },
            };

        public string Symbol { get; set; }

        /// <summary>
        /// Possible Enter Directions
        /// </summary>
        public List<Direction> PossibleEnterDirections { get; set; }

        public Pipe()
        {
            Symbol = "";
            PossibleEnterDirections = new List<Direction>();
        }

        public List<string> GetPossibleNextSymbols(Direction enterDirection)
        {
            return possibleDirectionToSymbols[enterDirection];
        }

        public Direction GetNextDirection(Direction enterDirection)
        {
            if (Symbol == "S" || Symbol == ".") {
                throw new Exception("S or . is not a valid pipe to call this function");
            }

            var theOtherDirections = PossibleEnterDirections.Where(d => d != enterDirection).FirstOrDefault();

            // Return the opposite of the other possible enter direction
            return 3 - theOtherDirections;
        }
    }

    public static class PipeHelper {

        public static Dictionary<string, Pipe> CreatePipes()
        {
            var vertical = new Pipe{ Symbol = "|", PossibleEnterDirections = new List<Direction>() { Direction.Up, Direction.Down } };
            var horizontal = new Pipe{ Symbol = "-", PossibleEnterDirections = new List<Direction>() { Direction.Left, Direction.Right } };
            var f = new Pipe{ Symbol = "F", PossibleEnterDirections = new List<Direction>() { Direction.Left, Direction.Up } };
            var j = new Pipe{ Symbol = "J", PossibleEnterDirections = new List<Direction>() { Direction.Right, Direction.Down } };
            var seven = new Pipe{ Symbol = "7", PossibleEnterDirections = new List<Direction>() { Direction.Right, Direction.Up } };
            var l = new Pipe{ Symbol = "L", PossibleEnterDirections = new List<Direction>() { Direction.Left, Direction.Down } };
            var s = new Pipe{ Symbol = "S", PossibleEnterDirections = new List<Direction>() { Direction.Up, Direction.Down, Direction.Left, Direction.Right } };
            var dot = new Pipe{ Symbol = ".", PossibleEnterDirections = new List<Direction>() { } };

            return new Dictionary<string, Pipe> {
                { "|", vertical },
                { "-", horizontal },
                { "F", f },
                { "J", j },
                { "7", seven },
                { "L", l },
                { "S", s },
                { ".", dot },
            };
        }

        public static (int nextRow, int nextCol) Move(Direction dir, int curRow, int curCol) {
            switch (dir) {
                case Direction.Up:
                    return (curRow - 1, curCol);
                case Direction.Down:
                    return (curRow + 1, curCol);
                case Direction.Left:
                    return (curRow, curCol - 1);
                case Direction.Right:
                    return (curRow, curCol + 1);
                default:
                    throw new Exception("Invalid direction");
            }
        }
    }
}


