using dfds.domain.Interfaces;

namespace dfds.domain.Services;

public class DistanceCalculatorService: IDistanceCalculatorService 
{
    private const double EarthRadius = 6371; // Radius of the Earth in kilometers

    public virtual double CalculateDistance(Location location1, Location location2)
    {
        var dLat = ToRadians(location2.Latitude - location1.Latitude);
        var dLon = ToRadians(location2.Longitude - location1.Longitude);

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRadians(location1.Latitude)) * Math.Cos(ToRadians(location2.Latitude)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        var distance = EarthRadius * c;

        return Math.Round(distance, 2);
    }

    public double CalculateDistance(TruckPlan truckPlan)
    {
        var trackingRecords = truckPlan.TrackingRecords;
        
        if (trackingRecords.Count == 0)
            return 0;
        
        // First distance is against the start location
        var distance = CalculateDistance(truckPlan.Start, truckPlan.TrackingRecords[0].Location);

        // The next distances are against the rest of the tracking records
        for (var i = 1; i < trackingRecords.Count; i++)
        {
            distance += CalculateDistance(trackingRecords[i - 1].Location, trackingRecords[i].Location);
        }
        
        return distance;
    }

    private static double ToRadians(double degrees)
    {
        return degrees * (Math.PI / 180);
    }
}