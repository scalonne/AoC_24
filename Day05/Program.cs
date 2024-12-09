var groups = File.ReadAllText("input.txt")
                 .Split($"{Environment.NewLine}{Environment.NewLine}");
var rules = groups[0].Split(Environment.NewLine)
                     .Select(o => o.Split('|'))
                     .Select(o => (x: Int32.Parse(o[0]), y: Int32.Parse(o[1])));
var pages = groups[1].Split(Environment.NewLine)
                     .Select(o => o.Split(',')
                                   .Select(Int32.Parse)
                                   .ToArray());
var rulesDic = new Dictionary<int, (HashSet<int> before, HashSet<int> after)>();

void Init() {
    foreach ((var x, var y) in rules) {
        if (!rulesDic.ContainsKey(x))
            rulesDic[x] = (new HashSet<int>(), new HashSet<int>());

        if (!rulesDic.ContainsKey(y))
            rulesDic[y] = (new HashSet<int>(), new HashSet<int>());

        rulesDic[x].after.Add(y);
        rulesDic[y].before.Add(x);
    }
}

List<(int[] update, bool isValid)> TryUpdatePages() {
    var updates = new List<(int[], bool)>();

    foreach (var update in pages) {
        var isValid = true;

        foreach (var i in Enumerable.Range(0, update.Length - 1)) {
            if (!rulesDic.TryGetValue(update[i], out var pageRule))
                continue;

            if (update[(i + 1)..].Any(pageRule.before.Contains)) {
                isValid = false;
                break;
            }
        }

        updates.Add((update, isValid));
    }

    return updates;
}

int P1(List<(int[] update, bool isValid)> updates)
    => updates.Where(o => o.isValid)
              .Sum(o => o.update[(int)Math.Floor(o.update.Length / 2m)]);

int P2(List<(int[] update, bool isValid)> updates) {
    var p2 = 0;

    foreach ((var update, _) in updates.Where(o => !o.isValid)) {
        var cpy = update.ToList();

        cpy.Sort((x, y) => {
            if (rulesDic.TryGetValue(x, out var pageRules)) {
                if (pageRules.after.Contains(y))
                    return 1;

                if (pageRules.before.Contains(y))
                    return -1;
            }

            return 0;
        });

        p2 += cpy[(int)Math.Floor(cpy.Count / 2m)];
    }

    return p2;
}

Init();

var updates = TryUpdatePages();

Console.WriteLine(P1(updates));
Console.WriteLine(P2(updates));