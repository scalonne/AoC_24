var levels = File.ReadLines("input.txt")
                 .Select(o => o.Split(' ')
                               .Select(Int32.Parse)
                               .ToList())
                 .Select(o => (level: o, isSafe: IsLevelSafe(o)));

bool IsLevelSafe(List<int> l)
    => l.Select((x, i) => (x, y: i < l.Count - 1 ? l[i + 1] : (int?)null, asc: l[0] > l[1]))
        .All(o => o.y == null || ((o.asc ? o.x > o.y : o.x < o.y) && Math.Abs(o.x - o.y.Value) <= 3));

bool IsLevelSafeWithTolerance(List<int> level) {
    var cpy = level.ToList();

    foreach (var i in Enumerable.Range(0, cpy.Count)) {
        var x = cpy[i];

        cpy.RemoveAt(i);

        if (IsLevelSafe(cpy))
            return true;

        cpy.Insert(i, x);
    }

    return false;
}

var p1 = levels.Where(o => o.isSafe).Count();
var p2 = p1 + levels.Where(o => !o.isSafe && IsLevelSafeWithTolerance(o.level)).Count();

Console.WriteLine(p1);
Console.WriteLine(p2);