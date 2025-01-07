var Map = File.ReadLines("input.txt")
              .Select((o, y) => o.Select((c, x) => (x, y, c))
                                 .ToArray())
              .ToArray();

var Directions = new Direction[] {
    new(0, 1),  // N
    new(1, 0),  // S
    new(0, -1), // E
    new(-1, 0)  // W
};

(int pathScore, int pathLength) GetShortestPath() {
    var lowestScore = Int32.MaxValue;
    var directionScoreCache = new Dictionary<(Pos, Direction), int>();
    var directionsCache = new Dictionary<Pos, List<(Pos, Direction)>>();
    var shortestCells = new Dictionary<Pos, int>();
    var start = Map.SelectMany(o => o)
                   .First(o => o.c == 'S');
    var pos = new Pos(start.x, start.y);

    moveRec(pos, Directions[1], 0, [pos]);

    return (lowestScore, shortestCells.Count(o => o.Value == lowestScore));

    List<(Pos, Direction)> getDirections(Pos p) {
        if (directionsCache.TryGetValue(p, out var directions))
            return directions;

        directions = [];

        foreach (var d in Directions) {
            var x = p.X + d.Dx;
            var y = p.Y + d.Dy;
            
            if (Map[y][x].c != '#')
                directions.Add((new(x, y), d));
        }

        directionsCache[p] = directions;

        return directions;
    }

    void moveRec(Pos pos, Direction dir, int score, HashSet<Pos> visited) {
        if (Map[pos.Y][pos.X].c == 'E') {
            if (score < lowestScore) {
                lowestScore = score;
            }
            
            foreach (var v in visited) {
                shortestCells[v] = score;
            }

            return;
        }

        foreach (var direction in getDirections(pos)) {
            (var next, var d) = direction;

            if (visited.Contains(next) || (d != dir && (d.Dx != 0 && dir.Dx != 0 || d.Dy != 0 && dir.Dy != 0)))
                continue;

            var turnScore = d == dir ? 1 : 1_001;
            var nextScore = score + turnScore;

            if (nextScore > lowestScore || directionScoreCache.TryGetValue(direction, out var savedScore) && savedScore < nextScore)
                continue;

            directionScoreCache[direction] = nextScore;

            visited.Add(next);
            moveRec(next, d, nextScore, visited);
            visited.Remove(next);
        }
    }
}

(var p1, var p2) = GetShortestPath();

Console.WriteLine(p1);
Console.WriteLine(p2);

record struct Pos(int X, int Y);
record struct Direction(int Dx, int Dy);