var Map = File.ReadLines("input.txt")
               .Select((o, y) => o.Select((c, x) => new Cell(x, y, c))
                                  .ToArray())
               .ToArray();
var Cells = Map.SelectMany(o => o).ToList();

var Height = Map.Length;
var Width = Map[0].Length;

var North = new Direction(0, 1);
var South = new Direction(0, -1);
var East = new Direction(1, 0);
var West = new Direction(-1, 0);
var Directions = new Direction[] { North, East, South, West };

bool IsInboud(Position p)
    => p.X >= 0 && p.X < Width && p.Y >= 0 && p.Y < Height;

List<HashSet<Cell>> GetAreas() {
    var cells = new HashSet<Cell>(Cells);
    var areas = new List<HashSet<Cell>>();

    while (cells.Count > 0) {
        var cell = cells.First();
        var area = GetArea(cell);

        areas.Add(area);

        foreach (var c in area)
            cells.Remove(c);
    }

    return areas;

    HashSet<Cell> GetArea(Cell cell) {
        var area = new HashSet<Cell> { cell };
        
        void FillAreaRec(Position p) {
            foreach (var d in Directions) {
                var nextPos = p + d;

                if (!IsInboud(nextPos))
                    continue;

                var nextCell = Map[nextPos.Y][nextPos.X];

                if (nextCell.C == cell.C && area.Add(nextCell))
                    FillAreaRec(nextPos);
            }
        }

        FillAreaRec(cell);

        return area;
    }
}

int P1(List<HashSet<Cell>> areas) {
    var perimeter = 0;

    foreach (var area in areas) {
        perimeter += area.Count * GetAreaPerimeter(area);
    }

    return perimeter;

    int GetAreaPerimeter(IEnumerable<Cell> area) {
        int GetCellPerimeter(Cell cell) {
            var p = 4;

            foreach (var d in Directions) {
                var pos = cell + d;

                if (IsInboud(pos) && Map[pos.Y][pos.X].C == cell.C)
                    p--;
            }

            return p;
        }

        return area.Sum(GetCellPerimeter);
    }
}

int P2(List<HashSet<Cell>> areas) {
    var sides = 0;

    foreach (var area in areas) {
        sides += area.Count * GetSides(area);
    }

    return sides;

    int GetSides(IEnumerable<Cell> area) {
        IEnumerable<(Position wall, Direction toCellDirection)> GetWalls(Cell cell) {
            foreach (var d in Directions) {
                var pos = cell + d;

                if (!IsInboud(pos) || Map[pos.Y][pos.X].C != cell.C)
                    yield return (pos, new Direction(d.X * -1, d.Y * -1));
            }
        }

        var cellDirectionsByWall = new Dictionary<Position, HashSet<Direction>>();

        foreach (var cell in area) {
            foreach ((var wall, var cellDirection) in GetWalls(cell)) {
                if (!cellDirectionsByWall.ContainsKey(wall))
                    cellDirectionsByWall[wall] = [];

                cellDirectionsByWall[wall].Add(cellDirection);
            }
        }

        var sides = 0;
        var walls = new HashSet<Position>(cellDirectionsByWall.Keys);

        while (walls.Count > 0) {
            var wall = walls.First();

            walls.Remove(wall);

            sides += cellDirectionsByWall[wall].Count;

            // cell direction: wall + direction = cell
            // path direction: wall + direction = next wall
            var cellDirectionsSet = cellDirectionsByWall[wall];
            var cellDirectionsByPathDirection = new Dictionary<Direction, HashSet<Direction>>();

            foreach (var cellDirection in cellDirectionsByWall[wall]) {
                void addPath(Direction d) {
                    if (cellDirectionsSet.Contains(d))
                        return;

                    if (!cellDirectionsByPathDirection.ContainsKey(d))
                        cellDirectionsByPathDirection[d] = [];

                    cellDirectionsByPathDirection[d].Add(cellDirection);
                }

                if (cellDirection == North || cellDirection == South) {
                    addPath(East);
                    addPath(West);
                } else if (cellDirection == East || cellDirection == West) {
                    addPath(North);
                    addPath(South);
                }
            }

            foreach ((var pathDirection, var cellDirections) in cellDirectionsByPathDirection) {
                var nextWall = wall + pathDirection;

                while (cellDirections.Count > 0 && cellDirectionsByWall.TryGetValue(nextWall, out var nextDirections) && walls.Contains(nextWall)) {
                    cellDirections.RemoveWhere(o => !nextDirections.Remove(o));

                    if (nextDirections.Count == 0)
                        walls.Remove(nextWall);

                    nextWall += pathDirection;
                }
            }
        }

        return sides;
    }
}

var areas = GetAreas();

Console.WriteLine(P1(areas));
Console.WriteLine(P2(areas));

file record struct Direction(int X, int Y);
file record Position(int X, int Y) {
    public static Position operator +(Position p, Direction d) => new (p.X + d.X, p.Y + d.Y);
}
file record Cell(int X, int Y, char C) : Position(X, Y);