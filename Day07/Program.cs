using System.Numerics;

var parse = (string s) => Int64.Parse(s);
var lines = File.ReadLines("input.txt")
                .Select(o => o.Split(':'))
                .Select(o => (target: parse(o[0]), values: o[1][1..].Split(' ')
                                                                    .Select(parse)
                                                                    .ToArray()))
                .ToList();

T Add<T>(T x, T y) where T : ISignedNumber<T> => x + y;
T Mult<T>(T x, T y) where T : ISignedNumber<T> => x * y;
T Concat<T>(T x, T y) => (T)(object)parse($"{x}{y}");

(long p1, long p2) Compute() {
    var p1 = 0L;
    var p2 = 0L;

    foreach ((var target, var values) in lines) {
        if (TryMatchRec(target, values, [Add, Mult], total: values[0])) {
            p1 += target;
        } else if (TryMatchRec(target, values, [Add, Mult, Concat], total: values[0])) {
            p2 += target;
        }
    }

    return (p1, p1 + p2);

    bool TryMatchRec<T>(T target, T[] values, Func<T, T, T>[] operators, int index = 0, T total = default!) where T : ISignedNumber<T>, IComparisonOperators<T, T, bool> {
        if (total > target)
            return false;

        if (index == values.Length - 1)
            return total == target;

        foreach (var op in operators) {
            if (TryMatchRec(target, values, operators, index + 1, op(total, values[index + 1])))
                return true;
        }

        return false;
    }
}

(var p1, var p2) = Compute();

Console.WriteLine(p1);
Console.WriteLine(p2);