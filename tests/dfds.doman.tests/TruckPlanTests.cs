using dfds.domain;
using dfds.domain.Interfaces;
using dfds.domain.Services;
using Moq;

namespace dfds.doman.tests;

public class TruckPlanTests
{
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
        var distance = truckPlan.CalculateDistanceDriven(distanceCalculatorService);

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
        var mockDistanceCalculator = new Mock<IDistanceCalculatorService>();
        mockDistanceCalculator
            .Setup(x => x.CalculateDistance(start, location1))
            .Returns(1);
        mockDistanceCalculator
            .Setup(x => x.CalculateDistance(location1, location2))
            .Returns(2);
        await truckPlan.AddTrackingRecord(location1, DateTime.Now.AddHours(-3), getCountryFromCoordinateMock.Object);
        await truckPlan.AddTrackingRecord(location2, DateTime.Now.AddHours(-2), getCountryFromCoordinateMock.Object);
        
        // act
        var distance = truckPlan.CalculateDistanceDriven(mockDistanceCalculator.Object);

        // assert 
        Assert.Equal(3, distance);
    }
    
    [Fact]
    public async Task With_TrackingRecords_ValidateCountries()
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
            .Setup(x => x.Query(location1))
            .ReturnsAsync("DE");
        getCountryFromCoordinateMock
            .Setup(x => x.Query(location2))
            .ReturnsAsync("DK");
        await truckPlan.AddTrackingRecord(location1, DateTime.Now.AddHours(-3), getCountryFromCoordinateMock.Object);
        await truckPlan.AddTrackingRecord(location2, DateTime.Now.AddHours(-2), getCountryFromCoordinateMock.Object);

        // Assert
        Assert.Equal("DE", truckPlan.TrackingRecords[0].Country);
        Assert.Equal("DK", truckPlan.TrackingRecords[1].Country);
    }
}
