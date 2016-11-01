namespace CrisisCoreModels
{
    /// <summary>
    ///  This describes an set of latitude and longitude coordinates.
    /// </summary>
    public class GeoCoordinate
    {

        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public GeoCoordinate() { }

        public GeoCoordinate(double latitude, double longitude)
        {
            this.Latitude = latitude;
            this.Longitude = longitude;
        }

        public override string ToString()
        {
            return string.Format("{0},{1}", Latitude, Longitude);
        }
    }
}