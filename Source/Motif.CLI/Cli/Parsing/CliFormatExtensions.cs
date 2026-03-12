namespace Motif.CLI;

internal static class CliFormatExtensions
{
    public static string ToToken(this CliFormat format)
        => format switch
        {
            CliFormat.Json => "json",
            CliFormat.GuitarPro => "gp",
            CliFormat.Gpif => "gpif",
            CliFormat.Motif => "motif",
            _ => throw new ArgumentOutOfRangeException(nameof(format))
        };
}
