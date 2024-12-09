var lines = File.ReadAllLines("input.txt")
                .Select((l, y) => l.Select((c, x) => (c, x, y))
                                   .ToList())
                .ToList();
var Height = lines.Count;
var Width = lines[0].Count;

var start = lines.SelectMany(o => o)
                 .First(o => o.c == '^');

var walls = lines.SelectMany(o => o)
                 .Where(o => o.c == '#')
                 .Select(o => (o.x, o.y));

var xWalls = new Dictionary<int, (List<int> yAsc, List<int> yDesc)>();
var yWalls = new Dictionary<int, (List<int> xAsc, List<int> xDesc)>();

void Init() {
    foreach ((var x, var y) in walls) {
        AddWall(x, y);
    }
}

void AddWall(int x, int y) {
    if (!xWalls.ContainsKey(x))
        xWalls[x] = (new List<int>(), new List<int>());

    if (!yWalls.ContainsKey(y))
        yWalls[y] = (new List<int>(), new List<int>());

    void insert(List<int> l, int v, bool asc = true) {
        var i = 0;
        while (i < l.Count) {
            if (asc && l[i] > v || !asc && l[i] < v)
                break;
            i++;
        }
        l.Insert(i, v);
    }

    insert(xWalls[x].yAsc, y);
    insert(xWalls[x].yDesc, y, false);
    insert(yWalls[y].xAsc, x);
    insert(yWalls[y].xDesc, x, false);
}

void RemoveWall(int x, int y) {
    xWalls[x].yAsc.Remove(y);

    if (xWalls[x].yAsc.Count == 0)
        xWalls.Remove(x);
    else
        xWalls[x].yDesc.Remove(y);

    yWalls[y].xAsc.Remove(x);

    if (yWalls[y].xAsc.Count == 0)
        yWalls.Remove(y);
    else
        yWalls[y].xDesc.Remove(x);
}

(bool isLooping, List<Position>? path) TryEscape(Position start, List<Position>? existingPath = null) {
    var path = new List<Position>(existingPath ?? []);
    var visited = new HashSet<Position>(path);
    (var x, var y, var direction) = start;
    var isLooping = false;

    while (true) {
        void updateVisited(int from, int to, Func<int, (int x, int y)> getPos) {
            var j = from < to ? 1 : -1;

            to += j;

            for (var i = from; i != to; i += j) {
                var pos = getPos(i);
                var node = new Position(pos.x, pos.y, direction);

                if (!visited.Add(node)) {
                    isLooping = true;
                    return;
                }

                path.Add(node);
            }
        }

        int? newX = x;
        int? newY = y;

        if (direction == Direction.North) {
            newY = (!xWalls.TryGetValue(x, out var l) || l.yAsc[0] >= y) ? null : l.yDesc.First(o => o < y) + 1;
            updateVisited(y, newY ?? 0, o => (x, o));
        } else if (direction == Direction.East) {
            newX = (!yWalls.TryGetValue(y, out var l) || l.xDesc[0] <= x) ? null : l.xAsc.First(o => o > x) - 1;
            updateVisited(x, newX ?? (Width - 1), o => (o, y));
        } else if (direction == Direction.South) {
            newY = (!xWalls.TryGetValue(x, out var l) || l.yDesc[0] <= y) ? null : l.yAsc.First(o => o > y) - 1;
            updateVisited(y, newY ?? (Height - 1), o => (x, o));
        } else {
            newX = (!yWalls.TryGetValue(y, out var l) || l.xAsc[0] >= x) ? null : l.xDesc.First(o => o < x) + 1;
            updateVisited(x, newX ?? 0, o => (o, y));
        }

        if (isLooping)
            return (true, null);

        if (!newX.HasValue || !newY.HasValue)
            return (false, path);

        x = newX!.Value;
        y = newY!.Value;
        direction = direction == Direction.West ? Direction.North : direction + 1;
    }
}

int P2(List<Position> p1) {
    var loopCount = 0;
    var path = new List<Position>();
    var walls = new HashSet<(int, int)>();
    var cpy = p1.ToList();

    while (cpy.Count > 1) {
        var start = cpy[0];
        (var x, var y, _) = cpy[1];

        cpy.RemoveAt(0);

        if (walls.Add((x, y))) {
            AddWall(x, y);

            if (TryEscape(start, path).isLooping)
                loopCount++;

            RemoveWall(x, y);
        }

        path.Add(start);
    }

    return loopCount;
}

Init();

var p1 = TryEscape(new(start.x, start.y, Direction.North));
var p2 = P2(p1.path!);

Console.WriteLine(p1.path!.Select(o => (o.X, o.Y)).Distinct().Count());
Console.WriteLine(p2);

enum Direction { North = 0, East = 1, South = 2, West = 3 }
file record struct Position(int X, int Y, Direction Direction);