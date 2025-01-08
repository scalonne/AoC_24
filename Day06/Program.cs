var Map = File.ReadLines("input.txt")
               .Select((o, y) => o.Select((o, x) => new Cell(new Pos(x, y), o))
                                  .ToArray())
               .ToArray();
var CellByPos = Map.SelectMany(o => o)
                   .ToDictionary(o => o.P, o => o.C);
var Height = Map.Length;
var Width = Map[0].Length;
var Directions = new Direction[] {
    new(0, -1), // N 
    new(1, 0),  // E
    new(0, 1),  // S
    new(-1, 0), // W
};

List<(Pos, Direction dir)>? Travel(List<(Pos pos, Direction dir)> path, bool fastForward) {
    var visited = new HashSet<(Pos pos, Direction dir)>(path);
    (var pos, var d) = path.Last();
    var i = Directions.Index().First(o => o.Item == d).Index;

    while (true) {
        var x = pos.X + d.Dx;
        var y = pos.Y + d.Dy;

        if (!IsInbound(x, y))
            break;

        if (fastForward && Map[y][x].C != '#') {
            x += d.Dx;
            y += d.Dy;

            while (Map[y][x].C != '#') {
                x += d.Dx;
                y += d.Dy;

                if (!IsInbound(x, y))
                    return path;
            }

            pos = new Pos(x - d.Dx, y - d.Dy);
        }

        var next = new Pos(pos.X + d.Dx, pos.Y + d.Dy);

        if (CellByPos[next] == '#') {
            i = i == 3 ? 0 : i + 1;
            d = Directions[i];
        } else {
            pos = next;
        }

        var node = (pos, d);

        if (!visited.Add(node))
            return null;

        if (!fastForward)
            path.Add(node);
    }

    return path;

    bool IsInbound(int x, int y)
        => x >= 0 && y >= 0 && x < Width && y < Height;
}

List<(Pos pos, Direction dir)> P1() {
    var start = Map.SelectMany(o => o).First(o => o.C == '^');

    return Travel([(start.P, Directions[0])], false)!;
}

int P2(List<(Pos pos, Direction dir)> path) {
    var loops = 0;
    var i = 0;
    var visited = new HashSet<Pos>();

    path.RemoveAt(0);

    while (i < path.Count - 1) {
        (var pos, _) = path[i];

        while (path[i].pos == pos)
            i++;

        (var next, _) = path[i];

        if (!visited.Add(next))
            continue;

        Map[next.Y][next.X].C = '#';
        CellByPos[next] = '#';

        if (Travel(path[..i], true) == null)
            loops++;

        Map[next.Y][next.X].C = '.';
        CellByPos[next] = '.';
    }

    return loops;
}

var path = P1();
var p1 = path!.Select(o => o.pos).Distinct().Count();
var p2 = P2(path);

Console.WriteLine(p1);
Console.WriteLine(p2);

record struct Direction(int Dx, int Dy);
record struct Pos(int X, int Y);
record struct Cell(Pos P, char C);