var Secrets = File.ReadLines("input.txt")
                  .Select(Int64.Parse)
                  .ToList();

long Mix(long value, long secret)
    => value ^ secret;

long Prune(long value)
    => value % 16_777_216;

long Multiply(long secret)
    => Prune(Mix(secret * 64L, secret));

long Divide(long secret)
    => Prune(Mix((long)Math.Floor(secret / 32d), secret));

long Finaly(long secret)
    => Prune(Mix(secret * 2_048, secret));

(long, long) Exec() {
    var bananasDic = new Dictionary<(long a, long b, long c, long d), Dictionary<long, long>>();
    var changes = new long[4];
    var digits = new long[2_000 + 1];
    var secretCache = new Dictionary<long, long>();
    var p1 = 0L;

    foreach (var baseSecret in Secrets) {
        var nextSecret = baseSecret;

        digits[0] = nextSecret % 10;

        foreach (var i in Enumerable.Range(0, 2_000)) {
            if (!secretCache.ContainsKey(nextSecret)) {
                secretCache[nextSecret] = Finaly(Divide(Multiply(nextSecret)));
            }

            nextSecret = secretCache[nextSecret];

            var digit = nextSecret % 10;

            changes[0] = changes[1];
            changes[1] = changes[2];
            changes[2] = changes[3];
            changes[3] = digit - digits[i];

            digits[i + 1] = digit;

            var key = (changes[0], changes[1], changes[2], changes[3]);

            if (!bananasDic.ContainsKey(key))
                bananasDic[key] = [];

            if (!bananasDic[key].ContainsKey(baseSecret))
                bananasDic[key][baseSecret] = digit;
        }

        p1 += nextSecret;
    }

    var p2 = bananasDic.Values.Select(o => o.Values.Sum())
                              .Max();

    return (p1, p2);
}

var (p1, p2) = Exec();

Console.WriteLine(p1);
Console.WriteLine(p2);