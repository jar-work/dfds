using System.Collections.ObjectModel;
using dfds.domain.Interfaces;
using dfds.domain.Services;

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

    /// <summary>
    /// Calculates the distance driven. It's the sum of all the distances between the pair of <seealso cref="_trackingRecords"/> and <seealso cref="Start"/>
    /// </summary>
    /// <returns></returns>
    public double CalculateDistanceDriven(IDistanceCalculatorService calculatorService)
    {
        if (_trackingRecords.Count == 0)
            return 0;
        
        // First distance is against the start location
        var distance = calculatorService.CalculateDistance(Start, _trackingRecords[0].Location);

        // The next distances are against the rest of the tracking records
        for (var i = 1; i < _trackingRecords.Count; i++)
        {
            distance += calculatorService.CalculateDistance(_trackingRecords[i - 1].Location, _trackingRecords[i].Location);
        }
        
        return distance;
    }
}