namespace WebApplication1.Utils;

public static class GeoUtils
{
    private const double EarthRadiusKm = 6371.0;

    // Distance calculation is based on Haversine Formula
    public static double Calculate(double latitude1, double longitude1, double latitude2, double longitude2)
    {
        double dLatitude = double.DegreesToRadians(latitude2 - latitude1);
        double dLongitude = double.DegreesToRadians(longitude2 - longitude1);

        double a = Math.Sin(dLatitude / 2) * Math.Sin(dLatitude / 2) +
                   Math.Cos(double.DegreesToRadians(latitude1)) * Math.Cos(double.DegreesToRadians(latitude2))
                                                                * Math.Sin(dLongitude / 2) * Math.Sin(dLongitude / 2);

        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        double distance = EarthRadiusKm * c;

        return Math.Round(distance, 2);
    }
}