namespace GPIO.NET.UnitTests;

using FluentAssertions;
using GPIO.NET.Abstractions;
using GPIO.NET.Models;
using GPIO.NET.Models.Raw;

public class PublicApiSurfaceTests
{
    [Fact]
    public void Public_abstractions_and_models_are_available()
    {
        typeof(IGuitarProReader).Should().NotBeNull();
        typeof(IGpArchiveReader).Should().NotBeNull();
        typeof(IGpifDeserializer).Should().NotBeNull();
        typeof(IScoreMapper).Should().NotBeNull();

        new GpReadOptions().Should().NotBeNull();
        new GuitarProScore().Tracks.Should().BeEmpty();
        new GpifDocument().Tracks.Should().BeEmpty();
    }
}
