using System.Diagnostics;

var RootDirectory = @$"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\devs\AoC_24\";

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
    var sw = new Stopwatch();

    sw.Start();
    process.Start();
    process.WaitForExit();
    sw.Stop();
    Console.WriteLine($"{day}: {sw.ElapsedMilliseconds}ms");
}