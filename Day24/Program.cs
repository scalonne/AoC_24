var InputBlocs = File.ReadAllText("input.txt")
                     .Split($"{Environment.NewLine}{Environment.NewLine}");
var Wires = InputBlocs[0].Split(Environment.NewLine)
                         .Select(o => (id: o[..3], active: o[^1] == '1'))
                         .ToDictionary(o => o.id, o => o.active);
var GateById = InputBlocs[1].Split(Environment.NewLine)
                            .Select(o => o.Split(" -> "))
                            .Select(o => (op: o[0].Split(' '), dest: o[1]))
                            .Select(o => (x: o.op[0], op: o.op[1], y: o.op[2], o.dest))
                            .ToDictionary(o => o.dest, o => new Gate(o.x, o.op, o.y));

long P1() {
    var length = Wires.Count / 2;
    var cache = new Dictionary<string, bool>();
    var bits = new bool[length + 1];

    foreach (var i in Enumerable.Range(0, length)) {
        bits[i] = getBitRec($"z{length - i:0#}");
    }

    return Convert.ToInt64(new String(bits.Select(o => o ? '1' : '0').ToArray()), 2);

    bool getBitRec(string wire) {
        (var x, var op, var y) = GateById[wire];

        bool f(string wire)
            => Wires.TryGetValue(wire, out var bit) ? bit : getBitRec(wire);

        if (cache.TryGetValue(wire, out var bit))
            return bit;

        return cache[wire] = op switch {
            "AND" => f(x) & f(y),
            "OR" => f(x) | f(y),
            "XOR" => f(x) ^ f(y),
            _ => throw new NotImplementedException()
        };
    }
}

string P2() {
    // sum = (a ^ b) ^ cin;
    // cout = (a & b) | (cin & (a ^ b));

    // Last adder excepted, sum gates are always composed of two XOR...
    var check1 = GateById.Where(o => o.Key[0] == 'z' && o.Value.Op != "XOR" && o.Key != $"z{Wires.Count / 2}")
                         .Select(o => o.Key)
                         .ToArray();

    // ... and one of theses two XOR must be a^b while the other an OR (first adder excluded)
    var check2 = GateById.Where(o => o.Key[0] == 'z' && o.Value.Op == "XOR" && o.Key != "z00")
                         .Select(o => (z: o.Value, a: GateById[o.Value.A], b: GateById[o.Value.B]))
                         .Where(o => o.a.Op != "XOR" && o.b.Op != "XOR")
                         .Select(o => o.a.Op == "OR" ? o.z.B : o.z.A)
                         .ToArray();

    // OR gates are always chained with two AND
    var check3 = GateById.Where(o => o.Value.Op == "OR")
                         .SelectMany(o => (string[])[o.Value.A, o.Value.B])
                         .Where(o => GateById[o].Op != "AND")
                         .ToArray();

    // Non-sum XOR gates must be A^B
    var check4 = GateById.Where(o => o.Key[0] != 'z' && o.Value.Op == "XOR" && o.Value.A[0] != 'x' && o.Value.A[0] != 'y')
                         .Select(o => o.Key)
                         .ToArray();

    var wires = check1.Concat(check2)
                      .Concat(check3)
                      .Concat(check4)
                      .ToHashSet();

    return String.Join(',', wires.OrderBy(o => o));
}

Console.WriteLine(P1());
Console.WriteLine(P2());

record struct Gate(string A, string Op, string B);