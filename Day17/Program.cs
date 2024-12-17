var Input = File.ReadAllText("input.txt")
                .Split($"{Environment.NewLine}{Environment.NewLine}");
var Registers = Input[0].Split(Environment.NewLine)
                        .Select(o => Int64.Parse(o["Register X: ".Length..]))
                        .ToArray();
var Instructions = Input[1]["Program: ".Length..].Split(',')
                                                 .Select(o => o[0])
                                                 .ToArray();

string P1(long[] registers) {
    var ptr = 0L;

    (Func<long> get, Action<long> set) r(long ri)
        => (() => registers[ri], (long v) => registers[ri] = v);

    var ra = r(0);
    var rb = r(1);
    var rc = r(2);

    var outputs = new List<long>();

    while (ptr + 1 < Instructions.Length) {
        var inst = Instructions[ptr] - '0';
        var operand = Instructions[ptr + 1] - '0';

        exec(inst, operand);

        ptr += 2;
    }

    return String.Join(null, outputs);

    long opCombo(long operand)
        => operand switch {
            0 or 1 or 2 or 3 => operand,
            4 => ra.get(),
            5 => rb.get(),
            6 => rc.get(),
            _ => throw new NotImplementedException(),
        };

    void div(long operand, Action<long> rset)
        => rset((long)Math.Truncate(ra.get() / Math.Pow(2, opCombo(operand))));

    void adv(long operand)
        => div(operand, ra.set);

    void bdv(long operand)
        => div(operand, rb.set);

    void cdv(long operand)
        => div(operand, rc.set);

    void bxl(long operand)
        => rb.set(rb.get() ^ operand);

    void bst(long operand)
        => rb.set(opCombo(operand) % 8);

    void jnz(long operand)
        => ptr = ra.get() == 0 ? ptr : operand - 2;

    void bxc(long operand)
        => rb.set(rb.get() ^ rc.get());

    void out_(long operand)
        => outputs.Add(opCombo(operand) % 8);

    void exec(long inst, long operand) {
        Action<long> f = inst switch {
            0 => adv,
            1 => bxl,
            2 => bst,
            3 => jnz,
            4 => bxc,
            5 => out_,
            6 => bdv,
            7 => cdv,
            _ => throw new NotImplementedException()
        };

        f(operand);
    }
}

long P2(bool console = false) {
    long getOffset(int column)
        => (long)Math.Pow(2L, column * 3L);

    var target = new string(Instructions);
    var i = Instructions.Length - 1;
    var ra = getOffset(i);
    long? offset = null;
    string p1;

    while ((p1 = P1([ ra, 0, 0 ])) != target) {
        if (p1[i] == target[i]) {
            while (p1[i] == target[i])
                i--;

            offset = null;
        } else {
            offset ??= i == 0 ? 1 : getOffset(i - 1);
            ra += offset.Value;
        }

        if (console) {
            Console.CursorVisible = false;
            Console.SetCursorPosition(0, 1);
            Console.WriteLine($"{"ra:", 7} {ra}");
            Console.WriteLine($"{"i:", 7} {i,-2}");
            Console.WriteLine($"{"p1:", 7} {p1}");
            Console.WriteLine($"target: {target}");
            Console.ReadKey();
        }
    }

    return ra;
}

var p1 = String.Join(',', P1([.. Registers]));

Console.WriteLine(p1);
Console.WriteLine(P2(console: false));