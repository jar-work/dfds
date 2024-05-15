namespace dfds.domain.Interfaces;

public interface IGetCountryFromCoordinateQuery
{
    public Task<string> Query(Location location);
}