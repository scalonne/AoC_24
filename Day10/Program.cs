var Map = File.ReadAllLines("input.txt")
              .Select((l, y) => l.Select((c, x) => (x, y, v: c - '0'))
                                 .ToArray())
              .ToArray();
var Height = Map.Length;
var Width = Map[0].Length;

List<(int, int)> GetTrails(int startX, int startY) {
    var tops = new List<(int, int)>();

    void FindTopsRec(int x, int y, int v) {
        var nexts = new (int x, int y)[] {
            (x, y - 1),
            (x + 1, y),
            (x, y + 1),
            (x - 1, y)
        };

        foreach (var next in nexts) {
            if (next.x < 0 || next.y < 0 || next.x >= Width || next.y >= Height)
                continue;

            var nv = Map[next.y][next.x].v;

            if (nv != v + 1)
                continue;

            if (nv == 9)
                tops.Add(next);
            else
                FindTopsRec(next.x, next.y, nv);
        }
    }

    FindTopsRec(startX, startY, 0);
    
    return tops;
}

var starts = Map.SelectMany(o => o)
                .Where(o => o.v == 0)
                .ToList();
var p1 = 0;
var p2 = 0;

foreach (var start in starts) {
    var trails = GetTrails(start.x, start.y);

    p1 += trails.Distinct().Count();
    p2 += trails.Count;
}

Console.WriteLine(p1);
Console.WriteLine(p2);