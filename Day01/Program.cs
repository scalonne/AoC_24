var lines = File.ReadAllLines("input.txt")
                .Select(o => o.Split("   "))
                .Select(o => (x: Int32.Parse(o[0]), y: Int32.Parse(o[1])))
                .ToList();

var xList = lines.Select(o => o.x)
                 .OrderBy(o => o)
                 .ToList();

var yList = lines.Select(o => o.y)
                 .OrderBy(o => o)
                 .ToList();

int P1() {
    return xList.Select((x, i) => Math.Abs(x - yList[i]))
                .Sum();
}

int P2() {
    var xDic = xList.Distinct()
                    .ToDictionary(x => x, x => xList.Count(o => o == x));
    var yDic = yList.Distinct()
                    .ToDictionary(y => y, y => yList.Count(o => o == y));

    return xDic.Select(o => o.Key * o.Value * (yDic.TryGetValue(o.Key, out var y) ? y : 0))
               .Sum();
}

Console.WriteLine(P1());
Console.WriteLine(P2());