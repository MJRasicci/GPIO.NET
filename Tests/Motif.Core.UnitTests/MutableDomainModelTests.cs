namespace Motif.Core.UnitTests;

using FluentAssertions;
using Motif.Models;

public class MutableDomainModelTests
{
    [Fact]
    public void Domain_model_supports_post_construction_mutation()
    {
        var score = new Score();
        var track = new TrackModel();
        var measure = new MeasureModel();
        var beat = new BeatModel();
        var note = new NoteModel();

        score.Title = "Motif";
        score.Artist = "Artist";
        score.PlaybackMasterBarSequence = [0, 1];

        track.Id = 1;
        track.Name = "Lead";

        measure.Index = 0;
        measure.TimeSignature = "4/4";
        measure.Voices =
        [
            new MeasureVoiceModel
            {
                VoiceIndex = 0
            }
        ];

        beat.Id = 10;
        beat.Dynamic = "mf";
        beat.FreeText = "quarter pulse";

        note.Id = 20;
        note.MidiPitch = 64;
        note.Duration = 1m;
        note.Articulation = new NoteArticulationModel
        {
            LetRing = true
        };

        beat.Notes = [note];
        measure.Beats = [beat];
        track.Measures = [measure];
        score.Tracks = [track];

        score.Title = "Mutable Motif";
        score.PlaybackMasterBarSequence = [0, 1, 2];

        track.Name = "Rhythm";

        measure.TimeSignature = "7/8";
        measure.AdditionalStaffBars =
        [
            new MeasureStaffModel
            {
                StaffIndex = 1,
                Clef = "Bass"
            }
        ];

        beat.Dynamic = "ff";
        beat.FreeText = "eighth pulse";

        note.MidiPitch = 67;
        note.ConcertPitch = new PitchValueModel
        {
            Step = "G",
            Octave = 4
        };
        note.Articulation.Bend = new BendModel
        {
            Enabled = true,
            DestinationValue = 1.5m
        };

        score.Title.Should().Be("Mutable Motif");
        score.PlaybackMasterBarSequence.Should().Equal(0, 1, 2);
        score.Tracks.Should().ContainSingle().Which.Should().BeSameAs(track);

        track.Name.Should().Be("Rhythm");
        track.Measures.Should().ContainSingle().Which.Should().BeSameAs(measure);

        measure.TimeSignature.Should().Be("7/8");
        measure.AdditionalStaffBars.Should().ContainSingle();
        measure.Beats.Should().ContainSingle().Which.Should().BeSameAs(beat);

        beat.Dynamic.Should().Be("ff");
        beat.FreeText.Should().Be("eighth pulse");
        beat.Notes.Should().ContainSingle().Which.Should().BeSameAs(note);

        note.MidiPitch.Should().Be(67);
        note.ConcertPitch.Should().NotBeNull();
        note.ConcertPitch!.Step.Should().Be("G");
        note.Articulation.LetRing.Should().BeTrue();
        note.Articulation.Bend.Should().NotBeNull();
        note.Articulation.Bend!.Enabled.Should().BeTrue();
        note.Articulation.Bend.DestinationValue.Should().Be(1.5m);
    }
}
