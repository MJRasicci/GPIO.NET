namespace Motif.Extensions.GuitarPro.UnitTests;

using Motif.Models;
using System.Linq;

internal static class HierarchyTestHelpers
{
    public static TrackModel SingleStaffTrack(int id, string name, params StaffMeasureModel[] measures)
        => new()
        {
            Id = id,
            Name = name,
            Staves =
            [
                new StaffModel
                {
                    StaffIndex = 0,
                    Measures = measures
                }
            ]
        };

    public static StaffMeasureModel PrimaryMeasure(this TrackModel track, int measureIndex = 0)
        => track.StaffMeasure(staffIndex: 0, measureIndex);

    public static StaffMeasureModel StaffMeasure(this TrackModel track, int staffIndex, int measureIndex = 0)
        => track.Staves
            .Single(staff => staff.StaffIndex == staffIndex)
            .Measures[measureIndex];
}
