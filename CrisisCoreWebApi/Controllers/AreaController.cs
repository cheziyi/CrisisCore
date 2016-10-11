using CrisisCoreModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Xml;

namespace CrisisCoreWebApi.Controllers
{
    public class AreaController : ApiController
    {
        private List<Area> areas;

        public AreaController()
        {
            areas = new List<Area>();

            GeoCoordinate[] seCoord = {
                new GeoCoordinate(1.374304, 103.876478),
                new GeoCoordinate(1.330716, 103.909177),
                new GeoCoordinate(1.390648, 103.966946),
                new GeoCoordinate(1.269817, 103.879155),
                new GeoCoordinate(1.342158, 104.097744),
                new GeoCoordinate(1.438593, 104.066134),
                new GeoCoordinate(1.432055, 103.916262)};

            GeoCoordinate[] neCoord = {
                new GeoCoordinate(1.433145, 103.904817),
                new GeoCoordinate(1.374304, 103.876478),
                new GeoCoordinate(1.330716, 103.909177),
                new GeoCoordinate(1.390648, 103.966946)};

            GeoCoordinate[] cCoord = {
                new GeoCoordinate(1.454847, 103.874566),
                new GeoCoordinate(1.318931, 103.794572),
                new GeoCoordinate(1.195281, 103.871530),
                new GeoCoordinate(1.269817, 103.879155),
                new GeoCoordinate(1.374304, 103.876478),
                new GeoCoordinate(1.433145, 103.904817)};

            GeoCoordinate[] nwCoord = {
                new GeoCoordinate(1.476368, 103.739166),
                new GeoCoordinate(1.318931, 103.794572),
                new GeoCoordinate(1.454847, 103.874566),
                new GeoCoordinate(1.491227, 103.803498)};

            GeoCoordinate[] swCoord = {
                new GeoCoordinate(1.476368, 103.739166),
                new GeoCoordinate(1.318931, 103.794572),
                new GeoCoordinate(1.195281, 103.871530),
                new GeoCoordinate(1.200467, 103.573841),
                new GeoCoordinate(1.390354, 103.598105)};

            areas.Add(new Area("C", "Central", cCoord));
            areas.Add(new Area("NE", "North East", neCoord));
            areas.Add(new Area("NW", "North West", nwCoord));
            areas.Add(new Area("SE", "South East", seCoord));
            areas.Add(new Area("SW", "South West", swCoord));
        }

        [HttpGet]
        public Area GetAreaFromPostalCode(string postalCode)
        {

            GeoCoordinate location = GetCoordinate(postalCode);
            if (location == null) return null;

            foreach (Area area in areas)
            {
                if (IsLocationInArea(area.AreaBoundary, location))
                {
                    return area;
                }
            }
            return null;
        }


        [HttpGet]
        public Area GetArea(string areaId)
        {
            foreach (Area area in areas)
            {
                if (area.AreaId.Equals(areaId)) return area;
            }
            return null;
        }

        [HttpGet]
        public IEnumerable<Area> GetAllAreas()
        {
            return areas;
        }

        private bool IsLocationInArea(GeoCoordinate[] area, GeoCoordinate location)
        {
            int i, j;
            bool c = false;
            for (i = 0, j = area.Length - 1; i < area.Length; j = i++)
            {
                if ((((area[i].Latitude <= location.Latitude) && (location.Latitude < area[j].Latitude))
                        || ((area[j].Latitude <= location.Latitude) && (location.Latitude < area[i].Latitude)))
                        && (location.Longitude < (area[j].Longitude - area[i].Longitude) * (location.Latitude - area[i].Latitude)
                            / (area[j].Latitude - area[i].Latitude) + area[i].Longitude))
                    c = !c;
            }
            return c;
        }

        [HttpGet]
        public GeoCoordinate GetCoordinate(string postalCode)
        {
            if (postalCode.Equals("")) return null;
            try
            {
                string uri = "https://maps.googleapis.com/maps/api/geocode/xml?";
                string key = "AIzaSyAx2cHrI8CjdzkiByY_FS1nV93CFx9LD54";
                uri += "key=" + key + "&address=Singapore+" + postalCode;

                XmlTextReader reader = new XmlTextReader(uri);
                reader.ReadToFollowing("location");
                reader.ReadToFollowing("lat");
                double lat = Convert.ToDouble(reader.ReadElementContentAsString());

                reader.ReadToFollowing("lng");
                double lng = Convert.ToDouble(reader.ReadElementContentAsString());

                return new GeoCoordinate(lat, lng);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
