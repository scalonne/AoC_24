var parse = (string s) => Int32.Parse(s);
var Stones = File.ReadAllText("input.txt")
                 .Split(' ')
                 .Select(parse)
                 .ToList();

(bool split, long? next, long[]? nexts) ApplyRules(long stone) {
    if (stone == 0)
        return (false, 1, null);

    var s = stone.ToString();

    if (s.Length % 2 == 0) {
        var mid = s.Length / 2;
        var v1 = parse(s[..mid]);
        var v2 = parse(s[mid..]);

        return (true, null, [ v1, v2 ]);
    }

    return (false, stone * 2_024, null);
}

long GetWidth(int depth,
              Dictionary<long, (bool split, long? next, long[]? nexts)> nextByStone,
              Dictionary<long, Dictionary<int, long>> widthByDepth) {
    long GetWidthRec(long stone, int depthLeft) {
        if (depthLeft == 0)
            return 1;

        if (!widthByDepth.ContainsKey(stone)) {
            widthByDepth[stone] = [];
        } else if (widthByDepth[stone].TryGetValue(depthLeft, out var memoWidth)) {
            return memoWidth;
        }

        if (!nextByStone.TryGetValue(stone, out var next))
            next = nextByStone[stone] = ApplyRules(stone);

        var width = 0L;

        if (next.split) {
            width += GetWidthRec(next.nexts![0], depthLeft - 1);
            width += GetWidthRec(next.nexts![1], depthLeft - 1);
        } else {
            width += GetWidthRec(next.next!.Value, depthLeft - 1);
        }

        widthByDepth[stone][depthLeft] = width;

        return width;
    }

    return Stones.Sum(o => GetWidthRec(o, depth));
}

var cacheNextByStone = new Dictionary<long, (bool split, long? next, long[]? nexts)>();
var cacheWidthByDepth = new Dictionary<long, Dictionary<int, long>>();

long P1() => GetWidth(25, cacheNextByStone, cacheWidthByDepth);
long P2() => GetWidth(75, cacheNextByStone, cacheWidthByDepth);

Console.WriteLine(P1());
Console.WriteLine(P2());