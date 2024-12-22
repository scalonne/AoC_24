var Input = File.ReadAllLines("input.txt");
var Towels = Input[0].Split(", ")
                     .ToArray();
var Designs = Input[2..];

Dictionary<string, HashSet<string>> GetTowelsByDesign() {
    var towelsByDesign = new Dictionary<string, HashSet<string>>();

    foreach (var design in Designs) {
        towelsByDesign[design] = [];

        foreach (var towel in Towels) {
            if (design.Contains(towel)) {
                towelsByDesign[design].Add(towel);
            }
        }
    }

    return towelsByDesign;
}

(int, long) P1() {
    var p1 = 0;
    var p2 = 0L;
    var suffixMemoDic = new Dictionary<string, long>();
    var towelsByDesign = GetTowelsByDesign();

    foreach (var kvp in towelsByDesign) {
        var design = kvp.Key;
        var towels = kvp.Value;
        var minSize = towels.Min(o => o.Length);
        var maxSize = towels.Max(o => o.Length);

        suffixMemoDic.Clear();

        var validDesigns = getValidDesignsRec(design, towels, minSize, maxSize);

        p1 += validDesigns > 0L ? 1 : 0;
        p2 += validDesigns;
    }

    return (p1, p2);

    long getValidDesignsRec(string design, HashSet<string> towels, int minSize, int maxTowelSize, int designPtr = 0) {
        if (designPtr == design.Length)
            return 1;

        var validDesigns = 0L;

        foreach (var towelSize in Enumerable.Range(minSize, maxTowelSize - minSize + 1)) {
            var nextPtr = designPtr + towelSize;

            if (nextPtr > design.Length)
                break;

            var prefix = design[designPtr..nextPtr];

            if (!towels.Contains(prefix))
                continue;

            var suffix = design[nextPtr..];

            if (suffixMemoDic.TryGetValue(suffix, out long value)) {
                validDesigns += value;
                continue;
            }

            var validDesignsRecCount = getValidDesignsRec(design, towels, minSize, maxTowelSize, designPtr + towelSize);

            suffixMemoDic[suffix] = validDesignsRecCount;
            validDesigns += validDesignsRecCount;
        }

        return validDesigns;
    }
}

(var p1, var p2) = P1();

Console.WriteLine(p1);
Console.WriteLine(p2);