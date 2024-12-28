var Instructions = File.ReadAllLines("input.txt");

long Exec(int depth) {
    char[][] NumericKeypad = [
        ['7', '8', '9'],
        ['4', '5', '6'],
        ['1', '2', '3'],
        [' ', '0', 'A'],
    ];
    char[][] DirectionalKeypad = [
        [' ', '^', 'A'],
        ['<', 'v', '>'],
    ];

    var res = 0L;
    var numericKeypadHelper = new KeypadHelper(NumericKeypad);
    var directionalKeypadHelper = new KeypadHelper(DirectionalKeypad);

    foreach (var inst in Instructions) {
        var baseMoves = numericKeypadHelper.GetSubSequences(inst);
        var shortestPath = directionalKeypadHelper.GetShortestPathLength(baseMoves, depth);

        res += shortestPath * Int32.Parse(inst.AsSpan()[..3]);
    }

    return res;
}

var p1 = Exec(2);
var p2 = Exec(25);

Console.WriteLine(p1);
Console.WriteLine(p2);

class KeypadHelper {
    static readonly Dictionary<(int, int), char> CharByDirection = new() {
        { (0, -1),  '^' },
        { (0, 1),   'v' },
        { (-1, 0),  '<' },
        { (1, 0),   '>' }
    };
    static readonly Dictionary<(string, int), long> ShortestPathCache = [];
    static readonly Dictionary<(char, char), HashSet<string>> PadMoveCache = [];

    char[][] Pad { get; }
    HashSet<(int x, int y)> PadPositions { get; }
    Dictionary<char, (int x, int y)> PadPositionByKey { get; }

    public KeypadHelper(char[][] keypad) {
        Pad = keypad;
        PadPositionByKey = Pad.SelectMany((o, y) => o.Select((c, x) => (x, y, c)))
                              .ToDictionary(o => o.c, o => (o.x, o.y));
        PadPositions = PadPositionByKey.Where(o => o.Key != ' ')
                                       .Select(o => o.Value)
                                       .ToHashSet();
    }

    HashSet<string> PadMove(char from, char to) {
        if (PadMoveCache.TryGetValue((from, to), out var sequences))
            return sequences;

        var visitedBuffer = new HashSet<(int, int)>() { PadPositionByKey[from] };
        var pathBuffer = new Stack<(int x, int y)>();

        sequences = [];

        padMoveRec(PadPositionByKey[from], PadPositionByKey[to], sequences);

        sequences = epurSequences(sequences).Select(o => o + 'A')
                                            .ToHashSet();

        PadMoveCache[(from, to)] = sequences;
        return sequences;

        void padMoveRec((int x, int y) currentPos, (int, int) toPos, HashSet<string> sequences) {
            if (sequences.Count > 0) {
                var currentLength = sequences.First().Length;

                if (pathBuffer.Count > currentLength)
                    return;
            }

            if (currentPos == toPos) {
                if (sequences.Count > 0 && pathBuffer.Count < sequences.First().Length) {
                    sequences.Clear();
                }

                sequences.Add(new string(pathBuffer.Reverse().Select(o => CharByDirection[o]).ToArray()));
                return;
            }

            foreach ((var dx, var dy) in CharByDirection.Keys) {
                var next = (x: currentPos.x + dx, y: currentPos.y + dy);

                if (PadPositions.Contains(next) && !visitedBuffer.Contains(next)) {
                    visitedBuffer.Add(next);
                    pathBuffer.Push((dx, dy));

                    padMoveRec(next, toPos, sequences);

                    visitedBuffer.Remove(next);
                    pathBuffer.Pop();
                }
            }
        }

        // keep <<^ remove <^<
        HashSet<string> epurSequences(HashSet<string> sequences) {
            if (sequences.Count <= 1)
                return sequences;

            var sequencesByContinuous = new Dictionary<int, HashSet<string>>();

            foreach (var seq in sequences) {
                var continuous = 0;

                foreach (var i in Enumerable.Range(1, seq.Length - 1)) {
                    if (seq[i - 1] == seq[i])
                        continuous++;
                }

                if (!sequencesByContinuous.ContainsKey(continuous))
                    sequencesByContinuous[continuous] = [];

                sequencesByContinuous[continuous].Add(seq);
            }

            return sequencesByContinuous.OrderByDescending(o => o.Key)
                                      .First()
                                      .Value;
        }
    }

    public List<HashSet<string>> GetSubSequences(string code) {
        var results = new List<HashSet<string>>();
        var from = 'A';

        foreach (var to in code) {
            results.Add(PadMove(from, to));
            from = to;
        }

        return results;
    }

    public long GetShortestPathLength(List<HashSet<string>> sequencesPossibilities, int depth = 1) {
        var pathLength = 0L;

        foreach (var possibilities in sequencesPossibilities) {
            var sequenceLength = Int64.MaxValue;

            foreach (var possibility in possibilities) {
                var possibilityLength = getShortestPathLengthRec(possibility, depth);

                if (possibilityLength < sequenceLength)
                    sequenceLength = possibilityLength;
            }

            pathLength += sequenceLength;
        }

        return pathLength;

        long getShortestPathLengthRec(string sequence, int depth) {
            if (depth == 0)
                return sequence.Length;

            var from = 'A';
            var cacheKey = (sequence, depth);

            if (ShortestPathCache.TryGetValue(cacheKey, out var length))
                return length;

            foreach (var to in sequence) {
                var subs = PadMove(from, to);
                var minSubLength = Int64.MaxValue;

                from = to;

                foreach (var sub in subs) {
                    var subCacheKey = (sub, depth - 1);

                    if (!ShortestPathCache.TryGetValue(subCacheKey, out var subLength)) {
                        subLength = getShortestPathLengthRec(sub, depth - 1);
                        ShortestPathCache[subCacheKey] = subLength;
                    }

                    if (subLength < minSubLength)
                        minSubLength = subLength;
                }

                length += minSubLength;
            }

            ShortestPathCache[cacheKey] = length;
            return length;
        }
    }
}
