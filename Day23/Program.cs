var ComputerPairs = File.ReadLines("input.txt")
                        .Select(o => (a: o[0..2], b: o[3..]));
var LeafsByNode = new Dictionary<string, HashSet<string>>();

foreach ((var a, var b) in ComputerPairs) {
    if (!LeafsByNode.ContainsKey(a))
        LeafsByNode[a] = [];

    if (!LeafsByNode.ContainsKey(b))
        LeafsByNode[b] = [];

    LeafsByNode[a].Add(b);
    LeafsByNode[b].Add(a);
}

int P1() {
    var threeSets = new HashSet<(string, string, string)>();

    foreach (var computer in LeafsByNode.Keys) {
        if (!computer.StartsWith('t'))
            continue;

        var combinaisons = GetAllCombinaisons(computer, [.. LeafsByNode[computer]], 3);

        combinaisons.ForEach(o => o.Sort());

        foreach (var combination in combinaisons) {
            var combinaisonWithLinks = combination.Select(o => (node: o, links: LeafsByNode[o]));

            if (combinaisonWithLinks.All(o => combination.All(p => p == o.node || o.links.Contains(p))))
                threeSets.Add((combination[0], combination[1], combination[2]));
        }
    }

    return threeSets.Count;

    List<List<string>> GetAllCombinaisons(string root, List<string> leafs, int size) {
        var combinaisons = new List<List<string>>();
        var stack = new Stack<string>();

        stack.Push(root);

        combineRec(stack, 0, size - 1);

        return combinaisons;

        void combineRec(Stack<string> currentCombinaison, int networkIndex, int depth) {
            if (depth == 0) {
                combinaisons.Add([.. currentCombinaison]);
                return;
            }

            if (networkIndex + depth > leafs.Count)
                return;

            foreach (var i in Enumerable.Range(networkIndex, leafs.Count - networkIndex)) {
                currentCombinaison.Push(leafs[i]);

                combineRec(currentCombinaison, i + 1, depth - 1);

                currentCombinaison.Pop();
            }
        }
    }
}

string P2() {
    var cache = new Dictionary<string, List<HashSet<string>>>();
    var visited = new HashSet<string>();

    foreach (var computer in LeafsByNode.Keys) {
        buildLanRec(computer, LeafsByNode[computer], visited);

        void buildLanRec(string currentNode, HashSet<string> nextComputers, HashSet<string> visited) {
            visited.Add(currentNode);

            if (nextComputers.Count == 0) {
                var network = visited.ToHashSet();

                foreach (var node in visited) {
                    if (!cache.ContainsKey(node))
                        cache[node] = [];

                    cache[node].Add(network);
                }
            }

            foreach (var computer in nextComputers) {
                var nexts = nextComputers.ToHashSet();

                nexts.Remove(computer);
                nexts.RemoveWhere(o => !LeafsByNode[computer].Contains(o));

                if (cache.TryGetValue(currentNode, out var currentSets) && currentSets.Any(o => o.Contains(computer) && visited.All(o.Contains)))
                    continue;

                buildLanRec(computer, nexts, visited);
            }

            visited.Remove(currentNode);
        }
    }

    var biggestNetwork = cache.Values.MaxBy(o => o.MaxBy(p => p.Count)!.Count)!
                                     .OrderByDescending(o => o.Count)
                                     .First();

    return String.Join(',', biggestNetwork.OrderBy(o => o));
}

Console.WriteLine(P1());
Console.WriteLine(P2());