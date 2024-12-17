var Input = File.ReadAllText("input.txt")
                .Split($"{Environment.NewLine}{Environment.NewLine}");
var Instructions = Input[1].Replace(Environment.NewLine, null).ToList();

var DirectionByInstruction = new Dictionary<char, Direction>() {
    { '^', new (0, -1) },
    { 'v', new (0, 1) },
    { '>', new (1, 0) },
    { '<', new (-1, 0) },
};

int P1() {
    var map = Input[0].Split(Environment.NewLine)
                  .Select((o, y) => o.Select((c, x) => new Node(x, y, c))
                                     .ToArray())
                  .ToArray();
    var instructions = new Queue<char>(Instructions);
    var robot = map.SelectMany(o => o).First(o => o.C == '@');

    while (instructions.TryDequeue(out var inst)) {
        var dir = DirectionByInstruction[inst];

        if (findSpace(robot, dir, out var spacePos)) {
            robot = move(robot, spacePos!, dir);
        }

        bool findSpace(Node p, Direction dir, out Node? space) {
            space = null;

            while (p.C != '#') {
                p = map[p.Y + dir.Y][p.X + dir.X];

                if (p.C == '.') {
                    space = p;
                    return true;
                }
            }

            return false;
        }

        Node move(Node robot, Node space, Direction dir) {
            map[robot.Y][robot.X].C = '.';

            var nextCell = map[robot.Y + dir.Y][robot.X + dir.X];

            if (nextCell != space) {
                map[space.Y][space.X].C = nextCell.C;
            }

            map[nextCell.Y][nextCell.X].C = '@';

            return map[nextCell.Y][nextCell.X];
        }
    }

    return map.SelectMany(o => o)
              .Where(o => o.C == 'O')
              .Sum(o => 100 * o.Y + o.X);
}

int P2(bool console = false) {
    var map = Input[0].Replace("#", "##")
                      .Replace("O", "[]")
                      .Replace(".", "..")
                      .Replace("@", "@.")
                      .Split(Environment.NewLine)
                      .Select((o, y) => o.Select((c, x) => new Node(x, y, c))
                                         .ToArray())
                      .ToArray();
    var instructions = new Queue<char>(Instructions);
    var robot = map.SelectMany(o => o).First(o => o.C == '@');
    var consoleBuffer = map.Select(o => o.Select(p => ' ').ToArray()).ToArray();

    if (console) Console.ReadKey();

    while (instructions.TryDequeue(out var inst)) {
        print(inst);

        var dir = DirectionByInstruction[inst];
        var next = map[robot.Y + dir.Y][robot.X + dir.X];

        if (next.C == '#')
            continue;

        if (next.C == '.') {
            next.C = '@';
            robot.C = '.';
            robot = next;
            continue;
        }

        if (dir.Y == 0) {
            if (tryMoveHorizontallyRec(robot.X + dir.X, robot.Y, '@')) {
                robot.C = '.';
                robot = map[robot.Y][robot.X + dir.X];
            }

            bool tryMoveHorizontallyRec(int x, int y, char prevC) {
                var cell = map[y][x];

                if (cell.C == '.') {
                    cell.C = prevC;
                    return true;
                }

                if (cell.C == '#')
                    return false;

                if (tryMoveHorizontallyRec(x + dir.X, y, cell.C)) {
                    cell.C = prevC;
                    return true;
                }

                return false;
            }
        } else {
            var visited = new HashSet<Node>();

            if (tryMoveVerticallyRec(robot.X, robot.Y + dir.Y, visited)) {
                var topToBottom = dir.Y < 0 ? visited.OrderBy(o => o.Y) : visited.OrderByDescending(o => o.Y);

                foreach (var cell in topToBottom) {
                    var x = cell.X;
                    var y = cell.Y;

                    map[y + dir.Y][x].C = cell.C;
                    map[y][x].C = '.';
                }

                next.C = '@';
                robot.C = '.';
                robot = next;
            }

            bool tryMoveVerticallyRec(int x, int y, HashSet<Node> visited) {
                var cell = map[y][x];

                if (cell.C == '.' || visited.Contains(cell))
                    return true;

                if (cell.C == '#')
                    return false;

                var closureXDir = cell.C == '[' ? 1 : -1;

                visited.Add(cell);
                visited.Add(map[y][x + closureXDir]);

                return tryMoveVerticallyRec(x, y + dir.Y, visited)
                    && tryMoveVerticallyRec(x + closureXDir, y + dir.Y, visited);
            }
        }
    }

    print(' ');

    return map.SelectMany(o => o)
              .Where(o => o.C == '[')
              .Sum(o => 100 * o.Y + o.X);

    void print(char inst) {
        if (!console)
            return;
        Console.CursorVisible = false;
        Console.SetCursorPosition(Console.LargestWindowWidth - Console.LargestWindowWidth / 3, Console.LargestWindowHeight / 2);
        Console.Write(inst);
        foreach (var y in Enumerable.Range(0, map.Length)) {
            foreach (var x in Enumerable.Range(0, map[0].Length)) {
                if (consoleBuffer[y][x] != map[y][x].C) {
                    Console.SetCursorPosition(x, y + 1);
                    if (map[y][x].C == '@')
                        Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write(map[y][x].C == '.' ? ' ' : map[y][x].C);
                    Console.ForegroundColor = ConsoleColor.White;
                    consoleBuffer[y][x] = map[y][x].C;
                }
            }
        }
        Console.SetCursorPosition(0, map.Length + 1);
    }
}

Console.WriteLine(P1());
Console.WriteLine(P2());

record struct Direction(int X, int Y);
file class Node(int X, int Y, char C) {
    public int X { get; } = X;
    public int Y { get; } = Y;
    public char C { get; set; } = C;
}