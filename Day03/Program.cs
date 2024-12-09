using System.Text.RegularExpressions;

var text = File.ReadAllText("input.txt");
var matches = Regex.Matches(text, @"mul\(\d+,\d+\)|do\(\)|don't\(\)")
                   .Select(o => (str: o.Value, op: o.Value[..o.Value.IndexOf('(')]))
                   .ToList();

int Multiply(string str)
    => str[4..^1].Split(',')
                 .Select(Int32.Parse)
                 .Aggregate((x, y) => x * y);

int P1()
    => matches.Where(o => o.op == "mul")
              .Select(o => Multiply(o.str))
              .Sum();

int P2() {
    var p2 = 0;
    var skip = false;

    foreach ((var str, var op) in matches) {
        if (op == "don't") {
            skip = true;
            continue;
        }

        if (skip) {
            if (op == "do")
                skip = false;
            continue;
        }

        if (op == "mul")
            p2 += Multiply(str);
    }

    return p2;
}

Console.WriteLine(P1());
Console.WriteLine(P2());