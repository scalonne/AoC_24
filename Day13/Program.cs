// https://en.wikipedia.org/wiki/Cramer%27s_rule
// https://www.reddit.com/r/adventofcode/comments/1hd7irq/2024_day_13_an_explanation_of_the_mathematics/

var machines = File.ReadAllText("input.txt")
                   .Split($"{Environment.NewLine}{Environment.NewLine}")
                   .Select(o => o.Split(Environment.NewLine)
                                 .Select((o, i) => o[(i <= 1 ? "Button A: " : "Prize: ").Length..])
                                 .Select(o => o.Split(", ")
                                               .Select(o => Int32.Parse(o[2..])))
                                 .Select(o => (x: o.First(), y: o.Last()))
                                 .ToArray())
                   .Select(o => (ax: o[0].x, ay: o[0].y, bx: o[1].x, by: o[1].y, px: o[2].x, py: o[2].y));

var p1 = 0L;
var p2 = 0L;

foreach ((var ax, var ay, var bx, var by, var px, var py) in machines) {
    long getTokens(long px, long py) {
        double pressA() => (px * by - py * bx) / (double)(ax * by - ay * bx);
        double pressB() => (ax * py - ay * px) / (double)(ax * by - ay * bx);

        var a = pressA();
        var b = pressB();

        if (!double.IsInteger(a) || !double.IsInteger(b))
            return 0;

        return (long)a * 3 + (long)b;
    }

    const long inc = 10000000000000;

    p1 += getTokens(px, py);
    p2 += getTokens(px + inc, py + inc);
}

Console.WriteLine(p1);
Console.WriteLine(p2);