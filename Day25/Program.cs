var Grids = File.ReadAllText("input.txt")
                .Split($"{Environment.NewLine}{Environment.NewLine}")
                .Select(o => o.Split(Environment.NewLine)
                              .Select(o => o.ToCharArray())
                              .ToArray())
                .ToList();
int P1() {
    var locks = new List<int[]>();
    var keys = new List<int[]>();
    var height = Grids[0].Length;
    var width = Grids[0][0].Length;

    foreach (var grid in Grids) {
        var coords = grid.SelectMany((o, y) => o.Select((c, x) => (c, x, y)))
                         .Where(o => o.c == '#')
                         .GroupBy(o => o.x)
                         .OrderBy(o => o.Key)
                         .Select(o => o.Count())
                         .ToArray();
        var col = grid[0][0] == '#' ? locks : keys;

        col.Add(coords);
    }

    var matchCount = 0;

    foreach (var key in keys) {
        foreach (var l in locks) {
            var match = true;

            foreach (var i in Enumerable.Range(0, key.Length)) {
                if (key[i] + l[i] > height) {
                    match = false;
                    break;
                }
            }

            if (match)
                matchCount++;
        }
    }

    return matchCount;
}

Console.WriteLine(P1());
