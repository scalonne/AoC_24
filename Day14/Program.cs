var Robots = File.ReadLines("input.txt")
                 .Select(o => o.Split(' ')
                               .Select(o => o[2..].Split(',')
                                                  .Select(Int32.Parse)
                                                  .ToArray())
                               .ToArray())
                 .Select(o => new Robot(new (o[0][0], o[0][1]), o[1][0], o[1][1]))
                 .ToList();

const int Height = 103;
const int Width = 101;

int P1(int ticks) {
    var movingRobots = new HashSet<Robot>();

    foreach (var robot in Robots) {
        var vx = robot.Vx * ticks;
        var vy = robot.Vy * ticks;
        var x = ((robot.X + vx) % Width + Width) % Width;
        var y = ((robot.Y + vy) % Height + Height) % Height;

        movingRobots.Add(new (new (x, y), vx, vy));
    }

    var areas = new (int x1, int x2, int y1, int y2)[] {
        (0, 49, 0, 50),
        (51, 101, 0, 50),
        (0, 49, 52, 103),
        (51, 101, 52, 103),
    };

    var res = 1;

    movingRobots.RemoveWhere(o => o.X == 50 || o.Y == 51);

    foreach ((var x1, var x2, var y1, var y2) in areas) {
        res *= movingRobots.RemoveWhere(o => o.X >= x1 && o.X <= x2 && o.Y >= y1 && o.Y <= y2);
    }

    return res;
}

int P2(bool console) {
    var movingRobots = Robots.ToList();

    Robot Move(Robot robot) {
        var x = (((robot.X + robot.Vx) % Width) + Width) % Width;
        var y = (((robot.Y + robot.Vy) % Height) + Height) % Height;

        return new (new (x, y), robot.Vx, robot.Vy);
    }

    var max = Width * Height;
    var elapsed = 0;
    var patternStart = 95; // visually found
    var patternLoop = 101;
    var consoleRobots = new Dictionary<int, Robot>();
    var posDic = new Dictionary<Position, int>();

    while (max-- > 0) {
        posDic.Clear();

        foreach (var idx in Enumerable.Range(0, movingRobots.Count)) {
            var robot = movingRobots[idx];

            robot = movingRobots[idx] = Move(movingRobots[idx]);

            posDic.TryGetValue(robot.P, out var count);
            posDic[robot.P] = count + 1;
        }

        elapsed++;

        if (console && elapsed == patternStart) {
            void print(Position pos, char c) {
                Console.SetCursorPosition(pos.X, pos.Y + 2);
                Console.Write(c);
            }

            foreach (var i in Enumerable.Range(0, movingRobots.Count)) {
                if (consoleRobots.TryGetValue(i, out var robot) && !posDic.ContainsKey(robot.P))
                    print(robot.P, ' ');
            
                print(movingRobots[i].P, '#');
                consoleRobots[i] = movingRobots[i];
            }

            Console.SetCursorPosition(0, 1);
            Console.Write(elapsed);

            patternStart += patternLoop;
        }

        if (posDic.Count == movingRobots.Count) {
            if (console)
                Console.SetCursorPosition(0, 1);

            return elapsed;
        }
    }

    throw new InvalidOperationException("tree not found");
}

Console.WriteLine(P1(100));
Console.WriteLine(P2(console: false));

file record struct Position(int X, int Y);
file record Robot(Position P, int Vx, int Vy) {
    public int X => P.X;
    public int Y => P.Y;
}