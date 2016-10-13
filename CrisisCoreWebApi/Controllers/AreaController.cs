using CrisisCoreModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Xml;

namespace CrisisCoreWebApi.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class AreaController : ApiController
    {
        private List<Area> areas;

        public AreaController()
        {
            areas = new List<Area>();

            GeoCoordinate[] seCoord = {
                new GeoCoordinate(1.4263587,103.9982986),
                new GeoCoordinate(1.4225833,103.9629364),
                new GeoCoordinate(1.4316785,103.9294625),
                new GeoCoordinate(1.4198376,103.9222527),
                new GeoCoordinate(1.3944394,103.9454271),
                new GeoCoordinate(1.3913503,103.9673996),
                new GeoCoordinate(1.3354046,103.9248276),
                new GeoCoordinate(1.3264806,103.9069748),
                new GeoCoordinate(1.3618331,103.8750458),
                new GeoCoordinate(1.3692125,103.8765907),
                new GeoCoordinate(1.3701563,103.8620853),
                new GeoCoordinate(1.3508498,103.856163),
                new GeoCoordinate(1.335576,103.868351),
                new GeoCoordinate(1.2767116,103.8781357),
                new GeoCoordinate(1.3017679,103.9509201),
                new GeoCoordinate(1.3038273,104.0394974),
                new GeoCoordinate(1.3635492,104.0556335),
                new GeoCoordinate(1.3844859,104.0868759),
                new GeoCoordinate(1.4140029,104.093399),
                new GeoCoordinate(1.4394009,104.0666199),
                new GeoCoordinate(1.4472948,104.0329742),
                new GeoCoordinate(1.4263587,103.9982986)};


            GeoCoordinate[] neCoord = {
                new GeoCoordinate(1.3913503,103.9673996),
                new GeoCoordinate(1.3944394,103.9454271),
                new GeoCoordinate(1.4198376,103.9222527),
                new GeoCoordinate(1.4277316,103.9043999),
                new GeoCoordinate(1.3692125,103.8765907),
                new GeoCoordinate(1.3618331,103.8750458),
                new GeoCoordinate(1.3264806,103.9069748),
                new GeoCoordinate(1.3354046,103.9248276),
                new GeoCoordinate(1.3913503,103.9673996)};

            GeoCoordinate[] cCoord = {
                new GeoCoordinate(1.4277316,103.9043999),
                new GeoCoordinate(1.4438627,103.8747024),
                new GeoCoordinate(1.4143461,103.8537598),
                new GeoCoordinate(1.4164054,103.8331604),
                new GeoCoordinate(1.3027977,103.7971122),
                new GeoCoordinate(1.2530282,103.8499832),
                new GeoCoordinate(1.2767116,103.8781357),
                new GeoCoordinate(1.335576,103.868351),
                new GeoCoordinate(1.3508498,103.856163),
                new GeoCoordinate(1.3701563,103.8620853),
                new GeoCoordinate(1.3692125,103.8765907),
                new GeoCoordinate(1.4277316,103.9043999)};

            GeoCoordinate[] nwCoord = {
                new GeoCoordinate(1.4442059,103.7545395),
                new GeoCoordinate(1.3384937,103.7785721),
                new GeoCoordinate(1.3316291,103.7662125),
                new GeoCoordinate(1.3027977,103.7971122),
                new GeoCoordinate(1.4164054,103.8331604),
                new GeoCoordinate(1.4143461,103.8537598),
                new GeoCoordinate(1.4438627,103.8747024),
                new GeoCoordinate(1.4689174,103.8462069),
                new GeoCoordinate(1.4792134,103.8070679),
                new GeoCoordinate(1.4442059,103.7545395)};

            GeoCoordinate[] swCoord = {
                new GeoCoordinate(1.4442059,103.7545395),
                new GeoCoordinate(1.4596504,103.7222672),
                new GeoCoordinate(1.4363119,103.6776352),
                new GeoCoordinate(1.3858588,103.6487961),
                new GeoCoordinate(1.33815,103.6233897),
                new GeoCoordinate(1.2856358,103.6041641),
                new GeoCoordinate(1.2187039,103.5948944),
                new GeoCoordinate(1.1579488,103.7160874),
                new GeoCoordinate(1.1685896,103.7796021),
                new GeoCoordinate(1.211839,103.8719559),
                new GeoCoordinate(1.2530282,103.8499832),
                new GeoCoordinate(1.3027977,103.7971122),
                new GeoCoordinate(1.3316291,103.7662125),
                new GeoCoordinate(1.3384937,103.7785721),
                new GeoCoordinate(1.4442059,103.7545395)};

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
