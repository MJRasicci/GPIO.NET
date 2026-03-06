namespace GPIO.NET.Implementation;

using GPIO.NET.Abstractions;
using GPIO.NET.Models.Raw;
using GPIO.NET.Utilities;

public sealed class DefaultNavigationResolver : INavigationResolver
{
    private const string MarkerSegno = "Segno";
    private const string MarkerSegnoSegno = "SegnoSegno";
    private const string MarkerCoda = "Coda";
    private const string MarkerDoubleCoda = "DoubleCoda";
    private const string MarkerFine = "Fine";

    private enum PendingCodaRoute
    {
        None,
        Coda,
        DoubleCoda
    }

    public IReadOnlyList<int> BuildPlaybackSequence(IReadOnlyList<GpifMasterBar> masterBars)
    {
        var bars = masterBars.OrderBy(m => m.Index).ToArray();
        if (bars.Length == 0)
        {
            return Array.Empty<int>();
        }

        var result = new List<int>(bars.Length * 2);

        var markers = DirectionMarkers.FromBars(bars);

        var repeatStack = new Stack<int>();
        var repeatVisits = new Dictionary<int, int>();
        var consumedJumps = new HashSet<int>();
        var pendingCodaRoute = PendingCodaRoute.None;
        var stopAtFine = false;

        var cursor = 0;
        var guard = 0;
        var guardLimit = Math.Max(10_000, bars.Length * 128);

        while (cursor >= 0 && cursor < bars.Length && guard++ < guardLimit)
        {
            var bar = bars[cursor];

            if (bar.RepeatStart && (repeatStack.Count == 0 || repeatStack.Peek() != cursor))
            {
                repeatStack.Push(cursor);
            }

            var endingVisit = repeatVisits.TryGetValue(cursor, out var visit) ? visit + 1 : 1;
            if (!ShouldPlayAlternateEnding(bar.AlternateEndings, endingVisit))
            {
                cursor++;
                continue;
            }

            result.Add(cursor);

            if (stopAtFine && IsDirectionToken(bar, MarkerFine))
            {
                break;
            }

            if (!consumedJumps.Contains(cursor) &&
                TryResolveJump(
                    ResolveJumpToken(bar),
                    markers,
                    ref pendingCodaRoute,
                    ref stopAtFine,
                    out var jumpIndex))
            {
                consumedJumps.Add(cursor);
                cursor = jumpIndex;
                continue;
            }

            if (bar.RepeatEnd && repeatStack.Count > 0)
            {
                var start = repeatStack.Peek();
                var count = repeatVisits.TryGetValue(cursor, out var done) ? done : 0;
                var maxPasses = Math.Max(2, bar.RepeatCount <= 0 ? 2 : bar.RepeatCount);

                if (count < maxPasses - 1)
                {
                    repeatVisits[cursor] = count + 1;
                    cursor = start;
                    continue;
                }

                repeatStack.Pop();
            }

            cursor++;
        }

        return result;
    }

    private static bool ShouldPlayAlternateEnding(string alternateEndings, int repeatVisit)
    {
        var endings = ReferenceListParser.SplitRefs(alternateEndings);
        if (endings.Count == 0)
        {
            return true;
        }

        return endings.Contains(repeatVisit);
    }

    private static string ResolveJumpToken(GpifMasterBar bar)
    {
        if (!string.IsNullOrWhiteSpace(bar.Jump))
        {
            return bar.Jump;
        }

        if (bar.DirectionProperties.TryGetValue("Jump", out var jump) && !string.IsNullOrWhiteSpace(jump))
        {
            return jump;
        }

        foreach (var candidate in KnownJumpTokens)
        {
            if (IsDirectionToken(bar, candidate))
            {
                return candidate;
            }
        }

        return string.Empty;
    }

    private static bool TryResolveJump(
        string jump,
        DirectionMarkers markers,
        ref PendingCodaRoute pendingCodaRoute,
        ref bool stopAtFine,
        out int index)
    {
        index = -1;
        var token = NormalizeToken(jump);
        if (string.IsNullOrWhiteSpace(token))
        {
            return false;
        }

        switch (token)
        {
            case "DaCapoAlCoda":
                pendingCodaRoute = PendingCodaRoute.Coda;
                stopAtFine = false;
                index = 0;
                return true;
            case "DaCapoAlDoubleCoda":
                pendingCodaRoute = PendingCodaRoute.DoubleCoda;
                stopAtFine = false;
                index = 0;
                return true;
            case "DaCapoAlFine":
                pendingCodaRoute = PendingCodaRoute.None;
                stopAtFine = true;
                index = 0;
                return true;
            case "DaCapo":
                pendingCodaRoute = PendingCodaRoute.None;
                stopAtFine = false;
                index = 0;
                return true;
            case "DaSegnoAlCoda":
                pendingCodaRoute = PendingCodaRoute.Coda;
                stopAtFine = false;
                index = markers.Segno;
                return true;
            case "DaSegnoAlDoubleCoda":
                pendingCodaRoute = PendingCodaRoute.DoubleCoda;
                stopAtFine = false;
                index = markers.Segno;
                return true;
            case "DaSegnoAlFine":
                pendingCodaRoute = PendingCodaRoute.None;
                stopAtFine = true;
                index = markers.Segno;
                return true;
            case "DaSegnoSegnoAlCoda":
                pendingCodaRoute = PendingCodaRoute.Coda;
                stopAtFine = false;
                index = markers.SegnoSegno;
                return true;
            case "DaSegnoSegnoAlDoubleCoda":
                pendingCodaRoute = PendingCodaRoute.DoubleCoda;
                stopAtFine = false;
                index = markers.SegnoSegno;
                return true;
            case "DaSegnoSegnoAlFine":
                pendingCodaRoute = PendingCodaRoute.None;
                stopAtFine = true;
                index = markers.SegnoSegno;
                return true;
            case "DaSegnoSegno":
                pendingCodaRoute = PendingCodaRoute.None;
                stopAtFine = false;
                index = markers.SegnoSegno;
                return true;
            case "DaSegno":
                pendingCodaRoute = PendingCodaRoute.None;
                stopAtFine = false;
                index = markers.Segno;
                return true;
            case "DaCoda":
                if (pendingCodaRoute == PendingCodaRoute.Coda && markers.Coda >= 0)
                {
                    pendingCodaRoute = PendingCodaRoute.None;
                    index = markers.Coda;
                    return true;
                }

                return false;
            case "DaDoubleCoda":
                if (pendingCodaRoute == PendingCodaRoute.DoubleCoda && markers.DoubleCoda >= 0)
                {
                    pendingCodaRoute = PendingCodaRoute.None;
                    index = markers.DoubleCoda;
                    return true;
                }

                return false;
            default:
                return false;
        }
    }

    private static bool IsDirectionToken(GpifMasterBar bar, string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return false;
        }

        if (TokenEquals(bar.Target, token) || TokenEquals(bar.Jump, token))
        {
            return true;
        }

        if (bar.DirectionProperties.TryGetValue("Target", out var targetValue) && TokenEquals(targetValue, token))
        {
            return true;
        }

        if (bar.DirectionProperties.TryGetValue("Jump", out var jumpValue) && TokenEquals(jumpValue, token))
        {
            return true;
        }

        foreach (var kvp in bar.DirectionProperties)
        {
            if (TokenEquals(kvp.Key, token) || TokenEquals(kvp.Value, token))
            {
                return true;
            }
        }

        return false;
    }

    private static bool TokenEquals(string? value, string token)
        => string.Equals(NormalizeToken(value), NormalizeToken(token), StringComparison.OrdinalIgnoreCase);

    private static string NormalizeToken(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        var chars = value.Where(char.IsLetterOrDigit).ToArray();
        return chars.Length == 0 ? string.Empty : new string(chars);
    }

    private static int FindMarkerIndex(IReadOnlyList<GpifMasterBar> bars, string token, int fallback)
    {
        for (var i = 0; i < bars.Count; i++)
        {
            if (IsDirectionToken(bars[i], token))
            {
                return i;
            }
        }

        return fallback;
    }

    private static readonly string[] KnownJumpTokens =
    {
        "DaCapo",
        "DaCapoAlCoda",
        "DaCapoAlDoubleCoda",
        "DaCapoAlFine",
        "DaSegno",
        "DaSegnoAlCoda",
        "DaSegnoAlDoubleCoda",
        "DaSegnoAlFine",
        "DaSegnoSegno",
        "DaSegnoSegnoAlCoda",
        "DaSegnoSegnoAlDoubleCoda",
        "DaSegnoSegnoAlFine",
        "DaCoda",
        "DaDoubleCoda"
    };

    private readonly record struct DirectionMarkers(int Segno, int SegnoSegno, int Coda, int DoubleCoda)
    {
        public static DirectionMarkers FromBars(IReadOnlyList<GpifMasterBar> bars)
        {
            var segno = FindMarkerIndex(bars, MarkerSegno, 0);
            var segnoSegno = FindMarkerIndex(bars, MarkerSegnoSegno, segno);
            var coda = FindMarkerIndex(bars, MarkerCoda, -1);
            var doubleCoda = FindMarkerIndex(bars, MarkerDoubleCoda, -1);
            return new DirectionMarkers(segno, segnoSegno, coda, doubleCoda);
        }
    }
}
