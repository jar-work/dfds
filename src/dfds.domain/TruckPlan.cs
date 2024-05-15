using System.Collections.ObjectModel;

namespace dfds.domain;

public class TruckPlan(Driver driver, Location start, Location end, TimeSpan duration)
{
    private readonly List<TrackingRecord> _trackingRecords = [];
    public Driver Driver { get; } = driver;
    public Location Start { get; } = start;
    public Location End { get; } = end;
    public TimeSpan Duration { get; } = duration;
    public ReadOnlyCollection<TrackingRecord> TrackingRecords => _trackingRecords.AsReadOnly();

    /// <summary>
    /// Adds a new register of a tracking record to the current TruckPlan
    /// </summary>
    /// <param name="trackingRecord"></param>
    public void AddTrackingRecord(TrackingRecord trackingRecord)
    {
        _trackingRecords.Add(trackingRecord);
    }
}