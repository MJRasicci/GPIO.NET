using GPIO.NET;

if (args.Length == 0)
{
    Console.WriteLine("Usage: gpio-net-tool <input.gp> [output.json]");
    return 1;
}

var inputPath = Path.GetFullPath(args[0]);
if (!File.Exists(inputPath))
{
    Console.Error.WriteLine($"Input file not found: {inputPath}");
    return 2;
}

var outputPath = args.Length > 1
    ? Path.GetFullPath(args[1])
    : Path.ChangeExtension(inputPath, ".mapped.json")!;

var reader = new GuitarProReader();
var score = await reader.ReadAsync(inputPath).ConfigureAwait(false);
var json = score.ToJson(indented: true);

var outputDirectory = Path.GetDirectoryName(outputPath);
if (!string.IsNullOrWhiteSpace(outputDirectory))
{
    Directory.CreateDirectory(outputDirectory);
}

await File.WriteAllTextAsync(outputPath, json).ConfigureAwait(false);

Console.WriteLine($"Mapped score written: {outputPath}");
Console.WriteLine($"Title: {score.Title}");
Console.WriteLine($"Tracks: {score.Tracks.Count}");
Console.WriteLine($"Playback bars: {score.PlaybackMasterBarSequence.Count}");

return 0;
