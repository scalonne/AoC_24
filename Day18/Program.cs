var Bytes = File.ReadLines("input.txt")
                .Select(o => o.Split(','))
                .Select(o => (x: Int32.Parse(o[0]), y: Int32.Parse(o[1])))
                .ToList();
const int MapSize = 70;
const int Width = MapSize + 1;
const int Height = MapSize + 1;
const int FeltBytes = 1_024;

var End = (Width - 1, Height - 1);
var Directions = new (int dx, int dy)[] {
    (0, 1),
    (0, -1),
    (1, 0),
    (-1 , 0)
};

List<List<(int, int)>> Escape(int feltBytes) {
    int? steps = null;
    var bytes = new HashSet<(int, int)>(Bytes[..feltBytes]);
    var paths = new List<List<(int, int)>>();
    var visitedSet = new HashSet<(int, int)>();
    var visitedStack = new Stack<(int, int)>();
    var shortestByCell = new Dictionary<(int, int), int>();

    escapeRec((0, 0), 0);

    return paths;

    void escapeRec((int x, int y) pos, int currentSteps) {
        if (pos == End) {
            paths.Add(new(visitedStack));

            if (steps == null || currentSteps < steps) {
                steps = currentSteps;
            }

            return;
        }

        // disabled for p2
        //if (currentSteps == steps)
        //    return;

        if (shortestByCell.TryGetValue(pos, out var cellShortest) && cellShortest <= currentSteps)
            return;

        shortestByCell[pos] = currentSteps;

        foreach ((var dx, var dy) in Directions) {
            var x = pos.x + dx;
            var y = pos.y + dy;
            var next = (x, y);

            if (visitedSet.Contains(next)
                || bytes.Contains(next)
                || x < 0 || y < 0 || x == Width || y == Height)
                continue;

            visitedSet.Add(next);
            visitedStack.Push(next);
            escapeRec(next, currentSteps + 1);
            visitedSet.Remove(next);
            visitedStack.Pop();
        }
    }
}

int P1(out List<List<(int, int)>> paths) {
    paths = Escape(FeltBytes);

    return paths.Min(o => o.Count);
}

string P2(List<List<(int, int)>> paths) {
    foreach (var i in Enumerable.Range(FeltBytes, Bytes.Count - FeltBytes)) {
        var fallingByte = Bytes[i - 1];

        paths.RemoveAll(o => o.Contains(fallingByte));

        if (paths.Count == 0) {
            paths = Escape(i);

            if (paths.Count == 0)
                return $"{fallingByte.x},{fallingByte.y}";
        }
    }

    throw new InvalidOperationException();
}

Console.WriteLine(P1(out var path));
Console.WriteLine(P2(path));