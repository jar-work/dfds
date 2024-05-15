using dfds.domain;
using dfds.infrastructure.Queries;

namespace dfds.infrastructure.tests.Queries;

public class GetCountryFromCoordinateQueryTests
{
    [Fact]
    public async Task Query_ShouldReturnCorrectValue()
    {
        // Arrange
        var location = new Location(52.3877830, 9.7334394);
        var query = new GetCountryFromCoordinateQuery();
        
        // Act
        var result = await query.Query(location);

        // Assert
        Assert.Equal("Germany", result);
    }
}
