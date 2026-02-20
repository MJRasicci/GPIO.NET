namespace GPIO.NET.UnitTests;

using FluentAssertions;
using GPIO.NET.Implementation;
using System.Text;

public class PropertyArticulationMappingTests
{
    [Fact]
    public async Task Mapper_captures_property_based_articulations()
    {
        const string gpif = """
<GPIF>
  <Score><Title>T</Title><Artist>A</Artist><Album>B</Album></Score>
  <Tracks><Track id="0"><Name>Guitar</Name></Track></Tracks>
  <MasterBars><MasterBar><Time>4/4</Time><Bars>1</Bars></MasterBar></MasterBars>
  <Bars><Bar id="1"><Voices>10</Voices></Bar></Bars>
  <Voices><Voice id="10"><Beats>100</Beats></Voice></Voices>
  <Rhythms><Rhythm id="1000"><NoteValue>Quarter</NoteValue></Rhythm></Rhythms>
  <Beats><Beat id="100"><Rhythm ref="1000" /><Notes>200</Notes></Beat></Beats>
  <Notes>
    <Note id="200">
      <Properties>
        <Property name="PalmMuted"><Enable /></Property>
        <Property name="Muted"><Enable /></Property>
        <Property name="Tapped"><Enable /></Property>
        <Property name="LeftHandTapped"><Enable /></Property>
        <Property name="HopoOrigin"><Enable /></Property>
        <Property name="HopoDestination"><Enable /></Property>
        <Property name="Slide"><Flags>32</Flags></Property>
        <Property name="Pitch"><Pitch><Step>E</Step><Octave>4</Octave></Pitch></Property>
      </Properties>
    </Note>
  </Notes>
</GPIF>
""";

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(gpif));
        var deserializer = new XmlGpifDeserializer();
        var raw = await deserializer.DeserializeAsync(stream, TestContext.Current.CancellationToken);
        var mapper = new DefaultScoreMapper();
        var score = await mapper.MapAsync(raw, TestContext.Current.CancellationToken);

        var articulation = score.Tracks[0].Measures[0].Beats[0].Notes[0].Articulation;
        articulation.PalmMuted.Should().BeTrue();
        articulation.Muted.Should().BeTrue();
        articulation.Tapped.Should().BeTrue();
        articulation.LeftHandTapped.Should().BeTrue();
        articulation.HopoOrigin.Should().BeTrue();
        articulation.HopoDestination.Should().BeTrue();
        articulation.SlideFlags.Should().Be(32);
    }
}
