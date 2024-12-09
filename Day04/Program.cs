var grid = File.ReadAllLines("input.txt");
var Height = grid.Length;
var Width = grid[0].Length;
var cells = Enumerable.Range(0, Height)
                       .SelectMany(y => Enumerable.Range(0, Width)
                                                  .Select(x => (x, y, c: grid[y][x])))
                       .ToList();

int P1() {
    var directions = new Direction[] {
        new(-1, -1),
        new(-1, 0),
        new(-1, 1),
        new(0, -1),
        new(0, 1),
        new(1, -1),
        new(1, 0),
        new(1, 1),
    };

    int CountXmas(int x, int y) {
        const string word = "MAS";
        var count = 0;

        foreach (var direction in directions) {
            bool isValid() {
                var j = y;
                var i = x;

                foreach (var c in word) {
                    j += direction.Y;
                    i += direction.X;

                    if (j < 0 || i < 0 || j >= Height || i >= Width || grid[j][i] != c)
                        return false;
                }

                return true;
            }

            if (isValid())
                count++;
        }

        return count;
    }

    return cells.Where(o => o.c == 'X')
                .Sum(o => CountXmas(o.x, o.y));
}

int P2() {
    bool IsCrossed(int x, int y) {
        bool isValid(int x, int y, char c) => x >= 0 && x < Width && y >= 0 && y < Height && grid[y][x] == c;

        return (isValid(x - 1, y - 1, 'M') && isValid(x + 1, y + 1, 'S') || isValid(x - 1, y - 1, 'S') && isValid(x + 1, y + 1, 'M')) &&
               (isValid(x + 1, y - 1, 'M') && isValid(x - 1, y + 1, 'S') || isValid(x + 1, y - 1, 'S') && isValid(x - 1, y + 1, 'M'));
    }

    return cells.Where(o => o.c == 'A' && IsCrossed(o.x, o.y))
                 .Count();
}

Console.WriteLine(P1());
Console.WriteLine(P2());

file record struct Direction(int Y, int X);