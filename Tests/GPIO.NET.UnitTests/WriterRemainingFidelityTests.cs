namespace GPIO.NET.UnitTests;

using FluentAssertions;
using GPIO.NET.Implementation;
using GPIO.NET.Models;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;

public class WriterRemainingFidelityTests
{
    private static async Task<GuitarProScore> DeserializeAndMap(string gpif)
    {
        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(gpif));
        var raw = await new XmlGpifDeserializer().DeserializeAsync(stream, TestContext.Current.CancellationToken);
        return await new DefaultScoreMapper().MapAsync(raw, TestContext.Current.CancellationToken);
    }

    private static async Task<XDocument> RoundTripThroughWrite(GuitarProScore score)
    {
        var result = await new DefaultScoreUnmapper().UnmapAsync(score, TestContext.Current.CancellationToken);
        await using var stream = new MemoryStream();
        await new XmlGpifSerializer().SerializeAsync(result.RawDocument, stream, TestContext.Current.CancellationToken);
        return XDocument.Parse(Encoding.UTF8.GetString(stream.ToArray()));
    }

    private static async Task<XDocument> RoundTripThroughJsonAndWrite(string gpif)
    {
        var score = await DeserializeAndMap(gpif);
        var json = score.ToJson(indented: false);
        var fromJson = JsonSerializer.Deserialize<GuitarProScore>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        fromJson.Should().NotBeNull();
        return await RoundTripThroughWrite(fromJson!);
    }

    [Fact]
    public async Task Score_and_master_bar_passthrough_round_trip_gp_revision_page_setup_whitespace_and_direction_shapes()
    {
        var gpif = """
        <GPIF>
          <GPVersion>7.0.0</GPVersion>
          <GPRevision>12015</GPRevision>
          <Score>
            <Title>T</Title>
            <Artist>A</Artist>
            <Album>B</Album>
            <SubTitle><![CDATA[ ]]></SubTitle>
            <Copyright><![CDATA[   ]]></Copyright>
            <Tabber><![CDATA[ ]]></Tabber>
            <PageSetup>
              <Width>210</Width>
              <Height>297</Height>
              <Orientation>Portrait</Orientation>
              <TopMargin>15</TopMargin>
              <LeftMargin>10</LeftMargin>
              <RightMargin>10</RightMargin>
              <BottomMargin>10</BottomMargin>
              <Scale>1</Scale>
            </PageSetup>
            <MultiVoice>0</MultiVoice>
          </Score>
          <MasterTrack><Tracks>0</Tracks></MasterTrack>
          <Tracks><Track id="0"><Name>Track</Name></Track></Tracks>
          <MasterBars>
            <MasterBar>
              <Time>4/4</Time>
              <FreeTime />
              <Section>
                <Letter><![CDATA[]]></Letter>
                <Text><![CDATA[]]></Text>
              </Section>
              <Directions>
                <Jump>DaCoda</Jump>
                <Jump>DaDoubleCoda</Jump>
                <Target>Segno</Target>
                <Target>SegnoSegno</Target>
              </Directions>
              <Bars>1</Bars>
            </MasterBar>
          </MasterBars>
          <Bars><Bar id="1"><Voices>10</Voices></Bar></Bars>
          <Voices><Voice id="10"><Beats>100</Beats></Voice></Voices>
          <Rhythms><Rhythm id="1000"><NoteValue>Quarter</NoteValue></Rhythm></Rhythms>
          <Beats><Beat id="100"><Rhythm ref="1000" /></Beat></Beats>
          <Notes />
        </GPIF>
        """;

        var score = await DeserializeAndMap(gpif);
        var measure = score.Tracks[0].Measures[0];

        score.Metadata.GpRevisionXml.Should().Contain("<GPRevision>12015</GPRevision>");
        score.Metadata.PageSetupXml.Should().Contain("<PageSetup>");
        score.Metadata.SubTitle.Should().Be(" ");
        score.Metadata.Copyright.Should().Be("   ");
        score.Metadata.Tabber.Should().Be(" ");
        measure.FreeTime.Should().BeTrue();
        measure.HasExplicitEmptySection.Should().BeTrue();
        measure.DirectionsXml.Should().Contain("<Jump>DaCoda</Jump>");
        measure.DirectionsXml.Should().Contain("<Target>SegnoSegno</Target>");

        var roundTrip = await RoundTripThroughJsonAndWrite(gpif);
        var root = roundTrip.Root!;
        var gpRevision = root.Element("GPRevision")!;
        var scoreElement = root.Element("Score")!;
        var masterBar = root.Element("MasterBars")!.Element("MasterBar")!;
        var directions = masterBar.Element("Directions")!;

        gpRevision.Attribute("required").Should().BeNull();
        gpRevision.Attribute("recommended").Should().BeNull();
        gpRevision.Value.Should().Be("12015");
        scoreElement.Element("SubTitle")!.Value.Should().Be(" ");
        scoreElement.Element("Copyright")!.Value.Should().Be("   ");
        scoreElement.Element("Tabber")!.Value.Should().Be(" ");
        scoreElement.Element("PageSetup")!.Element("Width")!.Value.Should().Be("210");
        masterBar.Element("FreeTime").Should().NotBeNull();
        masterBar.Element("Section")!.Element("Letter")!.Value.Should().BeEmpty();
        masterBar.Element("Section")!.Element("Text")!.Value.Should().BeEmpty();
        directions.Elements("Jump").Select(element => element.Value).Should().Equal("DaCoda", "DaDoubleCoda");
        directions.Elements("Target").Select(element => element.Value).Should().Equal("Segno", "SegnoSegno");
    }

    [Fact]
    public async Task Track_and_bar_notation_passthrough_round_trip_let_ring_throughout_and_simile_mark()
    {
        var gpif = """
        <GPIF>
          <Score><Title>T</Title><Artist>A</Artist><Album>B</Album></Score>
          <MasterTrack><Tracks>0</Tracks></MasterTrack>
          <Tracks>
            <Track id="0">
              <Name>Track</Name>
              <LetRingThroughout />
            </Track>
          </Tracks>
          <MasterBars><MasterBar><Time>4/4</Time><Bars>1</Bars></MasterBar></MasterBars>
          <Bars>
            <Bar id="1">
              <Voices>10</Voices>
              <SimileMark>Simple</SimileMark>
            </Bar>
          </Bars>
          <Voices><Voice id="10"><Beats>100</Beats></Voice></Voices>
          <Rhythms><Rhythm id="1000"><NoteValue>Quarter</NoteValue></Rhythm></Rhythms>
          <Beats><Beat id="100"><Rhythm ref="1000" /></Beat></Beats>
          <Notes />
        </GPIF>
        """;

        var score = await DeserializeAndMap(gpif);
        score.Tracks[0].Metadata.LetRingThroughout.Should().BeTrue();
        score.Tracks[0].Measures[0].SimileMark.Should().Be("Simple");

        var roundTrip = await RoundTripThroughJsonAndWrite(gpif);
        var track = roundTrip.Root!.Element("Tracks")!.Element("Track")!;
        var bar = roundTrip.Root.Element("Bars")!.Element("Bar")!;

        track.Element("LetRingThroughout").Should().NotBeNull();
        bar.Element("SimileMark")!.Value.Should().Be("Simple");
    }

    [Fact]
    public async Task Note_show_string_number_round_trips_through_json_and_write()
    {
        var gpif = """
        <GPIF>
          <Score><Title>T</Title><Artist>A</Artist><Album>B</Album></Score>
          <Tracks><Track id="0"><Name>Guitar</Name></Track></Tracks>
          <MasterBars><MasterBar><Time>4/4</Time><Bars>1</Bars></MasterBar></MasterBars>
          <Bars><Bar id="1"><Voices>10</Voices></Bar></Bars>
          <Voices><Voice id="10"><Beats>100</Beats></Voice></Voices>
          <Rhythms><Rhythm id="1000"><NoteValue>Quarter</NoteValue></Rhythm></Rhythms>
          <Beats>
            <Beat id="100">
              <Rhythm ref="1000" />
              <Notes>200</Notes>
            </Beat>
          </Beats>
          <Notes>
            <Note id="200">
              <Properties>
                <Property name="Midi"><Number>60</Number></Property>
                <Property name="ShowStringNumber"><Enable /></Property>
              </Properties>
            </Note>
          </Notes>
        </GPIF>
        """;

        var score = await DeserializeAndMap(gpif);
        score.Tracks[0].Measures[0].Beats[0].Notes[0].ShowStringNumber.Should().BeTrue();

        var roundTrip = await RoundTripThroughJsonAndWrite(gpif);
        var properties = roundTrip.Root!
            .Element("Notes")!
            .Element("Note")!
            .Element("Properties")!
            .Elements("Property")
            .ToDictionary(property => (string)property.Attribute("name")!, property => property);

        properties["ShowStringNumber"].Element("Enable").Should().NotBeNull();
    }

    [Fact]
    public async Task Unmapper_preserves_sparse_bar_voice_slots()
    {
        var score = new GuitarProScore
        {
            Tracks =
            [
                new TrackModel
                {
                    Id = 0,
                    Name = "Guitar",
                    Measures =
                    [
                        new MeasureModel
                        {
                            Index = 0,
                            TimeSignature = "4/4",
                            SourceBarId = 5,
                            Voices =
                            [
                                new MeasureVoiceModel
                                {
                                    VoiceIndex = 1,
                                    SourceVoiceId = 10,
                                    Beats =
                                    [
                                        new BeatModel
                                        {
                                            Id = 100,
                                            Duration = 0.25m
                                        }
                                    ]
                                },
                                new MeasureVoiceModel
                                {
                                    VoiceIndex = 3,
                                    SourceVoiceId = 11,
                                    Beats =
                                    [
                                        new BeatModel
                                        {
                                            Id = 101,
                                            Duration = 0.25m
                                        }
                                    ]
                                }
                            ]
                        }
                    ]
                }
            ]
        };

        var result = await new DefaultScoreUnmapper().UnmapAsync(score, TestContext.Current.CancellationToken);

        result.Diagnostics.Warnings.Should().BeEmpty();
        result.RawDocument.BarsById[5].VoicesReferenceList.Should().Be("-1 10 -1 11");
    }

    [Fact]
    public async Task Sparse_bar_voice_slots_round_trip_through_json_and_write()
    {
        var gpif = """
        <GPIF>
          <Score><Title>T</Title><Artist>A</Artist><Album>B</Album></Score>
          <Tracks><Track id="0"><Name>Guitar</Name></Track></Tracks>
          <MasterBars><MasterBar><Time>4/4</Time><Bars>1</Bars></MasterBar></MasterBars>
          <Bars><Bar id="1"><Voices>-1 10 -1 11</Voices></Bar></Bars>
          <Voices>
            <Voice id="10"><Beats>100</Beats></Voice>
            <Voice id="11"><Beats>101</Beats></Voice>
          </Voices>
          <Rhythms>
            <Rhythm id="1000"><NoteValue>Quarter</NoteValue></Rhythm>
          </Rhythms>
          <Beats>
            <Beat id="100"><Rhythm ref="1000" /></Beat>
            <Beat id="101"><Rhythm ref="1000" /></Beat>
          </Beats>
          <Notes />
        </GPIF>
        """;

        var roundTrip = await RoundTripThroughJsonAndWrite(gpif);
        roundTrip.Root!
            .Element("Bars")!
            .Element("Bar")!
            .Element("Voices")!
            .Value.Should().Be("-1 10 -1 11");
    }
}
