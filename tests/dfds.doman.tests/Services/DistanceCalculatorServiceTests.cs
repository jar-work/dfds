using dfds.domain;
using dfds.domain.Interfaces;
using dfds.domain.Services;
using Moq;

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
    
    [Fact]
    public void Without_TrackingRecords_ShouldReturn_0()
    {
        // arrange
        var truckPlan = new TruckPlan(
            new Driver("John Snow", new DateOnly(1990, 4, 12)), 
            new Location(1, 2), 
            new Location(3, 41), 
            new TimeSpan(0, 7, 0, 0));
        var distanceCalculatorService = new DistanceCalculatorService();

        // act
        var distance = distanceCalculatorService.CalculateDistance(truckPlan);

        // assert
        Assert.Equal(0, distance);
    }

    [Fact]
    public async Task With_TrackingRecords_ShouldReturn_ExpectedDistance()
    {
        // arrange
        var start = new Location(1, 2);
        var end = new Location(3, 4);
        var location1 = new Location(5, 6);
        var location2 = new Location(7, 8);
        var driver = new Driver("John Snow", new DateOnly(1990, 4, 12));
        var duration = new TimeSpan(0, 7, 0, 0);
        var truckPlan = new TruckPlan(driver, start, end,duration);
        var getCountryFromCoordinateMock = new Mock<IGetCountryFromCoordinateQuery>();
        getCountryFromCoordinateMock
            .Setup(x => x.Query(It.IsAny<Location>()))
            .ReturnsAsync("DE");
        var mockDistanceCalculator = new Mock<DistanceCalculatorService>();
        mockDistanceCalculator
            .Setup(x => x.CalculateDistance(start, location1))
            .Returns(1);
        mockDistanceCalculator
            .Setup(x => x.CalculateDistance(location1, location2))
            .Returns(2);
        await truckPlan.AddTrackingRecord(location1, DateTime.Now.AddHours(-3), getCountryFromCoordinateMock.Object);
        await truckPlan.AddTrackingRecord(location2, DateTime.Now.AddHours(-2), getCountryFromCoordinateMock.Object);
        
        // act
        var distance = mockDistanceCalculator.Object.CalculateDistance(truckPlan);

        // assert 
        Assert.Equal(3, distance);
    }
}
