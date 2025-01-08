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
    var bananasDic = new Dictionary<Key, long>();
    var keysDic = new Dictionary<Key, Key>();
    var changes = new short[4];
    var digits = new short[2_000 + 1];
    var secretCache = new Dictionary<long, long>();
    var p1 = 0L;

    foreach (var baseSecret in Secrets) {
        digits[0] = (short)(baseSecret % 10);

        var nextSecret = baseSecret;

        for (var i = 0; i < 2_000; i++) {
            if (!secretCache.TryGetValue(nextSecret, out long secret)) {
                secretCache[nextSecret] = secret = Finaly(Divide(Multiply(nextSecret)));
            }

            nextSecret = secret;

            var digit = (short)(nextSecret % 10);

            changes[0] = changes[1];
            changes[1] = changes[2];
            changes[2] = changes[3];
            changes[3] = (short)(digit - digits[i]);

            digits[i + 1] = digit;

            if (digit == 0)
                continue;

            var firstKey = new Key(baseSecret, changes[0], changes[1], changes[2], changes[3]);

            // changes have already been registered for that secret
            if (keysDic.TryGetValue(firstKey, out var secondKey))
                continue;

            secondKey = firstKey with { Secret = 0 };

            keysDic[firstKey] = secondKey;

            bananasDic.TryGetValue(secondKey, out var d);
            bananasDic[secondKey] = d + digit;
        }

        p1 += nextSecret;
    }

    var p2 = bananasDic.Values.Max();
    
    return (p1, p2);
}

var (p1, p2) = Exec();

Console.WriteLine(p1);
Console.WriteLine(p2);

record struct Key(long Secret, short A, short B, short C, short D);