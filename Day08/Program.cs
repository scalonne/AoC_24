var lines = File.ReadAllLines("input.txt");
var Height = lines.Length;
var Width = lines[0].Length;

var antennas = lines.SelectMany((l, y) => l.Select((o, x) => (c: o, x, y))
                                           .Where(o => o.c != '.'))
                    .GroupBy(o => o.c)
                    .ToDictionary(o => o.Key, o => o.Select(p => (p.x, p.y))
                                                    .ToList());

List<(int x, int y, int xDir, int yDir)> GetSignals() {
    var signals = new List<(int, int, int, int)>();

    foreach (var kvp in antennas) {
        var locations = kvp.Value;

        void getSignalsRec(int i) {
            if (i == locations.Count - 1)
                return;

            (var x1, var y1) = locations[i];

            foreach ((var x2, var y2) in locations[(i + 1)..]) {
                signals.Add((x1, y1, x1 - x2, y1 - y2));
                signals.Add((x2, y2, (x1 - x2) * -1, (y1 - y2) * -1));
            }

            getSignalsRec(i + 1);
        }

        getSignalsRec(0);
    }

    return signals;
}

bool IsInBound(int x, int y)
    => x >= 0 && y >= 0 && x < Width && y < Height;

int P1(List<(int x, int y, int xDir, int yDir)> signals)
    => signals.Select(o => (x: o.x + o.xDir, y: o.y + o.yDir))
              .Where(o => IsInBound(o.x, o.y))
              .Distinct()
              .Count();

int P2(List<(int x, int y, int xDir, int yDir)> signals) {
    var antinodes = new HashSet<(int, int)>();

    foreach (var signal in signals) {
        void echoRec(int x, int y) {
            antinodes.Add((x, y));

            x += signal.xDir;
            y += signal.yDir;

            if (IsInBound(x, y))
                echoRec(x, y);
        }

        echoRec(signal.x, signal.y);
    }

    return antinodes.Count();
}

var signals = GetSignals();

Console.WriteLine(P1(signals));
Console.WriteLine(P2(signals));