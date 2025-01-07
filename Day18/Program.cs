var Bytes = File.ReadLines("input.txt")
                .Select(o => o.Split(','))
                .Select(o => new Pos(Int32.Parse(o[0]), Int32.Parse(o[1])))
                .ToList();
const int MapSize = 70;
const int Width = MapSize + 1;
const int Height = MapSize + 1;
const int FeltBytes = 1_024;

var End = new Pos(Width - 1, Height - 1);
var Directions = new (int dx, int dy)[] {
    (0, 1),
    (0, -1),
    (1, 0),
    (-1 , 0)
};

List<List<Pos>> Escape(int feltBytes) {
    int? steps = null;
    var bytes = new HashSet<Pos>(Bytes[..feltBytes]);
    var paths = new List<List<Pos>>();
    var visitedSet = new HashSet<Pos>();
    var shortestByCell = new Dictionary<Pos, int>();

    escapeRec(new(0, 0), 0);

    return paths;

    void escapeRec(Pos pos, int currentSteps) {
        if (pos == End) {
            paths.Add(visitedSet.ToList());

            if (steps == null || currentSteps < steps) {
                steps = currentSteps;
            }

            return;
        }

        if (currentSteps == steps || shortestByCell.TryGetValue(pos, out var cellShortest) && cellShortest <= currentSteps)
            return;

        shortestByCell[pos] = currentSteps;

        foreach ((var dx, var dy) in Directions) {
            var x = pos.X + dx;
            var y = pos.Y + dy;
            var next = new Pos(x, y);

            if (visitedSet.Contains(next)
                || bytes.Contains(next)
                || x < 0 || y < 0 || x == Width || y == Height)
                continue;

            visitedSet.Add(next);
            escapeRec(next, currentSteps + 1);
            visitedSet.Remove(next);
        }
    }
}

int P1()
    => Escape(FeltBytes).Min(o => o.Count);

string P2() {
    var i = (Bytes.Count - FeltBytes) / 2;
    var start = FeltBytes + i;
    var paths = Escape(start);

    while (i > 1) {
        start += paths.Count == 0 ? -i : i;

        paths = Escape(start);

        i /= 2;
    }

    while (paths.Count == 0) {
        paths = Escape(--start);
    }

    return $"{Bytes[start].X},{Bytes[start].Y}";
}

Console.WriteLine(P1());
Console.WriteLine(P2()); 

record struct Pos(int X, int Y);
