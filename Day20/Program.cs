var Map = File.ReadLines("input.txt")
              .Select((l, y) => l.Select((c, x) => (x, y, c))
                                 .ToArray())
              .ToArray();
var Height = Map.Length;
var Width = Map[0].Length;
var Start = Map.SelectMany(o => o)
               .First(o => o.c == 'S');               
var End = Map.SelectMany(o => o)
             .First(o => o.c == 'E');

Dictionary<(int x, int y), int> GetDistanceByPosition() {
    var allDirections = new (int dx, int dy)[] {
        (0, -1),
        (0, 1),
        (-1 , 0),
        (1, 0),
    };
    var verticalDirections = allDirections.Where(o => o.dy != 0).ToArray();
    var horizontalDirections = allDirections.Where(o => o.dx != 0).ToArray();
    var distanceByPosition = new Dictionary<(int, int), int>();
    var length = 0;
    var directions = allDirections;
    var px = Start.x;
    var py = Start.y;

    distanceByPosition[(px, py)] = length++;

    while (px != End.x || py != End.y) {
        foreach (var (dx, dy) in directions) {
            var x = px + dx;
            var y = py + dy;

            if (Map[y][x].c == '#')
                continue;

            while (Map[y][x].c != '#') {
                distanceByPosition[(x, y)] = length++;
                x += dx;
                y += dy;
            }

            px = x - dx;
            py = y - dy;
            directions = dx != 0 ? verticalDirections : horizontalDirections;
            break;
        }
    }

    return distanceByPosition;
}

int Cheat(Dictionary<(int x, int y), int> distanceByPosition, int cheatDistance, int minCheatTime) {
    var cheatDic = new Dictionary<int, int>();
    var positions = new Queue<(int x, int y)>(distanceByPosition.Keys.OrderBy(o => distanceByPosition[o]));
    var positionsSet = positions.ToHashSet();
    var res = 0;
    
    while (positions.TryDequeue(out var pos)) {
        var d1 = distanceByPosition[pos];
        var cheatCandidates = getCellArea(pos, cheatDistance).Where(positionsSet.Contains);

        foreach (var next in cheatCandidates) {
            var d = Math.Abs(next.x - pos.x) + Math.Abs(next.y - pos.y);
            var d2 = distanceByPosition[next];

            if (d2 - (d1 + d) >= minCheatTime)
                res++;
        }
    }

    return res;

    IEnumerable<(int x, int y)> getCellArea((int x, int y) pos, int distance) {
        foreach (var dy in Enumerable.Range(-distance, distance * 2 + 1)) {
            var y = pos.y + dy;
            var xRange = distance - Math.Abs(dy);

            foreach (var dx in Enumerable.Range(-xRange, xRange * 2 + 1))
                yield return (pos.x + dx, y);
        }
    }
}

var distanceByPosition = GetDistanceByPosition();
var p1 = Cheat(distanceByPosition, 2, 100);
var p2 = Cheat(distanceByPosition, 20, 100);

Console.WriteLine(p1);
Console.WriteLine(p2);