namespace CrisisCoreModels
{
    public class Area
    {
        public string AreaId { get; set; }
        public string AreaName { get; set; }
        public GeoCoordinate[] AreaBoundary { get; set; }

        public Area() { }

        public Area(string areaId, string areaName, GeoCoordinate[] areaBoundary)
        {
            AreaId = areaId;
            AreaName = areaName;
            AreaBoundary = areaBoundary;
        }
    }
}