var Map = File.ReadLines("input.txt")
              .Select((o, y) => o.Select((c, x) => (x, y, c))
                                 .ToArray())
              .ToArray();

var North = new Direction(0, 1);
var South = new Direction(0, -1);
var East = new Direction(1, 0);
var West = new Direction(-1, 0);
var Directions = new Direction[] { North, East, South, West };

var scorePaths = new Dictionary<int, HashSet<(int, int)>>();

int P1(bool console = false) {
    var lowestScore = Int32.MaxValue;
    var scoreDic = new Dictionary<(int, int, Direction), int>();
    var start = Map.SelectMany(o => o)
                   .First(o => o.c == 'S');

    moveRec(start.x, start.y, East, 0, [(start.x, start.y)]);

    return lowestScore;

    void moveRec(int x, int y, Direction dir, int score, HashSet<(int, int)> visited) {
        if (Map[y][x].c == 'E') {
            if (score <= lowestScore) {
                lowestScore = score;

                if (!scorePaths.ContainsKey(lowestScore))
                    scorePaths[lowestScore] = new(visited);

                foreach (var v in visited)
                    scorePaths[lowestScore].Add(v);

                if (console)
                    print(visited);
            }

            return;
        }

        foreach (var d in Directions) {
            var next = (x: x + d.X, y: y + d.Y);

            if (Map[next.y][next.x].c == '#' || visited.Contains(next) || (d != dir && (d.X != 0 && dir.X != 0 || d.Y != 0 && dir.Y != 0)))
                continue;

            var turnScore = d == dir ? 1 : 1_001;
            var nextScore = score + turnScore;
            var key = (next.x, next.y, d);

            if (nextScore > lowestScore || scoreDic.TryGetValue(key, out var savedScore) && savedScore < nextScore)
                continue;

            scoreDic[key] = nextScore;

            visited.Add(next);
            moveRec(next.x, next.y, d, nextScore, visited);
            visited.Remove(next);
        }
    }

    void print(IEnumerable<(int x, int y)> path) {
        Console.Clear();
        Console.SetCursorPosition(0, 0);
        Console.Write(lowestScore);

        foreach (var c in Map.SelectMany(o => o)) {
            Console.SetCursorPosition(c.x, c.y + 1);
            Console.Write(c.c == '.' ? ' ' : c.c);
        }

        foreach (var p in path) {
            Console.SetCursorPosition(p.x, p.y + 1);
            Console.Write('x');
        }

        Console.SetCursorPosition(0, Map.Length + 1);
        Console.ReadKey();
    }
}

var p1 = P1();
var p2 = scorePaths[p1].Count;

Console.WriteLine(p1);
Console.WriteLine(p2);

record struct Direction(int X, int Y);