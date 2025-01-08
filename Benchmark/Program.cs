using System.Diagnostics;

var RootDirectory = @$"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\devs\AoC_24\";
var Retry = 3;

foreach (var i in Enumerable.Range(1, 25)) {
    var day = $"Day{i:0#}";
    var path = Path.Combine(RootDirectory, day, "bin", "release", "net9.0", $"{day}.exe");
    var process = new Process {
        StartInfo = new ProcessStartInfo {
            FileName = path,
            WorkingDirectory = Path.GetDirectoryName(path),
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        }
    };
    var min = Int64.MaxValue;

    foreach (var _ in Enumerable.Range(0, Retry)) {
        var sw = new Stopwatch();

        sw.Start();
        process.Start();
        process.WaitForExit();
        sw.Stop();

        if (sw.ElapsedMilliseconds < min)
            min = sw.ElapsedMilliseconds;
    }

    Console.Write($"{day}: ");
    Console.ForegroundColor = min switch {
        >= 800 => ConsoleColor.Red,
        >= 450 => ConsoleColor.Yellow,
        _ => ConsoleColor.Green
    };
    Console.WriteLine($"{min}ms");
    Console.ForegroundColor = ConsoleColor.White;
}