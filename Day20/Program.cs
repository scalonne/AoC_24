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
    var directions = new (int dx, int dy)[] {
        (0, 1),
        (0, -1),
        (1, 0),
        (-1 , 0)
    };
    var pos = (Start.x, Start.y);
    var end = (End.x, End.y);
    var prev = pos;
    var distanceByPosition = new Dictionary<(int, int), int>();
    var length = 0;

    distanceByPosition[pos] = length++;

    while (pos != end) {
        foreach (var (dx, dy) in directions) {
            var next = (x: pos.x + dx, y: pos.y + dy);

            if (Map[next.y][next.x].c == '#' || next == prev)
                continue;

            distanceByPosition[next] = length++;
            prev = pos;
            pos = next;
        }
    }

    return distanceByPosition;
}

int Cheat(Dictionary<(int x, int y), int> distanceByPosition, int cheatDistance, int minCheatTime) {
    var cheatDic = new Dictionary<int, int>();

    foreach (var pos in distanceByPosition.Keys) {
        var posDistance = distanceByPosition[pos];
        var area = getCellArea(pos, cheatDistance);
        var nexts = area.Where(o => distanceByPosition.ContainsKey(o) && distanceByPosition[o] > posDistance)
                        .ToList();

        foreach (var next in nexts) {
            var d1 = distanceByPosition[next] - posDistance;
            var d2 = Math.Abs(next.x - pos.x) + Math.Abs(next.y - pos.y);

            if (d1 > d2) {
                var key = d1 - d2;

                if (!cheatDic.ContainsKey(key))
                    cheatDic[key] = 0;

                cheatDic[key]++;
            }
        }
    }

    return cheatDic.Where(o => o.Key >= minCheatTime)
                   .Sum(o => o.Value);

    HashSet<(int x, int y)> getCellArea((int x, int y) pos, int distance) {
        var area = new HashSet<(int, int)>();

        foreach (var dy in Enumerable.Range(-distance, distance * 2 + 1)) {
            var y = pos.y + dy;
            var xRange = distance - Math.Abs(dy);
 
            foreach (var dx in Enumerable.Range(-xRange, xRange * 2 + 1)) {
                var x = pos.x + dx;

                area.Add((x, y));
            }
        }

        return area;
    }
}

var distanceByPosition = GetDistanceByPosition();
var p1 = Cheat(distanceByPosition, 2, 100);
var p2 = Cheat(distanceByPosition, 20, 100);

Console.WriteLine(p1);
Console.WriteLine(p2);