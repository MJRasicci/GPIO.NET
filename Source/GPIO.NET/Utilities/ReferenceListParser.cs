namespace GPIO.NET.Utilities;

internal static class ReferenceListParser
{
    public static List<int> SplitRefs(string refs)
        => refs
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Select(value => int.TryParse(value, out var parsed) ? parsed : -1)
            .Where(value => value >= 0)
            .ToList();
}
