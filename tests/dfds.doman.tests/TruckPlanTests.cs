using dfds.domain;
using dfds.domain.Interfaces;
using Moq;

namespace dfds.doman.tests;

public class TruckPlanTests
{
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
