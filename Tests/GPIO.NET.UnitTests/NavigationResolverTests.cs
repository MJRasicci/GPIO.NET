namespace GPIO.NET.UnitTests;

using FluentAssertions;
using GPIO.NET.Implementation;
using GPIO.NET.Models.Raw;

public class NavigationResolverTests
{
    [Fact]
    public void Resolver_handles_simple_repeat_with_two_endings()
    {
        var bars = new[]
        {
            new GpifMasterBar { Index = 0, RepeatStart = true },
            new GpifMasterBar { Index = 1, RepeatEnd = true, RepeatCount = 2 },
            new GpifMasterBar { Index = 2, AlternateEndings = "1" },
            new GpifMasterBar { Index = 3, AlternateEndings = "2" },
        };

        var resolver = new DefaultNavigationResolver();
        var seq = resolver.BuildPlaybackSequence(bars);

        seq.Should().Equal(0, 1, 0, 1, 2);
    }

    [Fact]
    public void Resolver_stops_infinite_loops_with_guard_limit()
    {
        var bars = new[]
        {
            new GpifMasterBar { Index = 0, RepeatStart = true, RepeatEnd = true, RepeatCount = 100000 }
        };

        var resolver = new DefaultNavigationResolver();
        var seq = resolver.BuildPlaybackSequence(bars);

        seq.Count.Should().BeLessThan(20000);
        seq.Should().NotBeEmpty();
    }

    [Fact]
    public void Resolver_handles_da_capo_al_fine()
    {
        var bars = new[]
        {
            new GpifMasterBar { Index = 0 },
            new GpifMasterBar { Index = 1, Jump = "DaCapoAlFine" },
            new GpifMasterBar { Index = 2, Target = "Fine" },
            new GpifMasterBar { Index = 3 }
        };

        var resolver = new DefaultNavigationResolver();
        var seq = resolver.BuildPlaybackSequence(bars);

        seq.Should().Equal(0, 1, 0, 1, 2);
    }

    [Fact]
    public void Resolver_handles_da_capo_al_coda_with_conditional_da_coda_jump()
    {
        var bars = new[]
        {
            new GpifMasterBar { Index = 0 },
            new GpifMasterBar { Index = 1, Jump = "DaCapoAlCoda" },
            new GpifMasterBar { Index = 2, Jump = "DaCoda" },
            new GpifMasterBar { Index = 3, Target = "Coda" },
            new GpifMasterBar { Index = 4 }
        };

        var resolver = new DefaultNavigationResolver();
        var seq = resolver.BuildPlaybackSequence(bars);

        seq.Should().Equal(0, 1, 0, 1, 2, 3, 4);
    }

    [Fact]
    public void Resolver_handles_da_segno_segno_al_double_coda()
    {
        var bars = new[]
        {
            new GpifMasterBar { Index = 0 },
            new GpifMasterBar { Index = 1, Target = "SegnoSegno" },
            new GpifMasterBar { Index = 2, Jump = "DaSegnoSegnoAlDoubleCoda" },
            new GpifMasterBar { Index = 3, Jump = "DaDoubleCoda" },
            new GpifMasterBar { Index = 4, Target = "DoubleCoda" },
            new GpifMasterBar { Index = 5 }
        };

        var resolver = new DefaultNavigationResolver();
        var seq = resolver.BuildPlaybackSequence(bars);

        seq.Should().Equal(0, 1, 2, 1, 2, 3, 4, 5);
    }

    [Fact]
    public void Resolver_ignores_da_coda_without_pending_al_coda_route()
    {
        var bars = new[]
        {
            new GpifMasterBar { Index = 0 },
            new GpifMasterBar { Index = 1, Jump = "DaCoda" },
            new GpifMasterBar { Index = 2, Target = "Coda" }
        };

        var resolver = new DefaultNavigationResolver();
        var seq = resolver.BuildPlaybackSequence(bars);

        seq.Should().Equal(0, 1, 2);
    }
}
