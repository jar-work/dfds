namespace dfds.domain.Services;

public interface IDistanceCalculatorService
{
    double CalculateDistance(Location location1, Location location2);
}