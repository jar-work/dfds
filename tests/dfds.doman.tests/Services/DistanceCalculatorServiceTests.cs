using dfds.domain;
using dfds.domain.Services;

namespace dfds.doman.tests.Services;

public class DistanceCalculatorServiceTests
{
    [Fact]
    public void Locations_Provided_Should_Calculate_Distance_Correctly()
    {
        // arrange: using https://gps-coordinates.org/distance-between-coordinates.php to compare
        var location1 = new Location(52.2296756, 21.0122287); // Warsaw
        var location2 = new Location(41.8781136, -87.6297982); // Chicago
        var distanceCalculatorService = new DistanceCalculatorService();
        
        // act
        var distance = distanceCalculatorService.CalculateDistance(location1, location2);
        
        // assert
        Assert.Equal(7511.05, distance);
    }
}
