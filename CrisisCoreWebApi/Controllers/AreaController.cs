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
    /// <summary>
    /// AreaController web service class for area based methods and geocoding.
    /// Areas are divided into "Central", "North East", "North West", "South East" and "South West".
    /// </summary>
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class AreaController : ApiController
    {
        /// <summary>
        /// List of areas.
        /// </summary>
        private List<Area> areas;

        /// <summary>
        /// Constructor for class.
        /// </summary>
        public AreaController()
        {
            // Initialize a list of Area objects
            areas = new List<Area>();

            // Coorindates for South East area boundary
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

            // Coordinates for North East area boundary
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

            // Coordinates for Central area boundary
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

            // Coordinates for North West area boundary
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

            // Coordinates for South West area boundary
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

            // Add initialized Area object with respective values into array
            areas.Add(new Area("C", "Central", cCoord));
            areas.Add(new Area("NE", "North East", neCoord));
            areas.Add(new Area("NW", "North West", nwCoord));
            areas.Add(new Area("SE", "South East", seCoord));
            areas.Add(new Area("SW", "South West", swCoord));
        }

        /// <summary>
        /// Method to get respective area of given GeoCoordinate.
        /// </summary>
        /// <param name="location">A GeoCoordinate object with latitude and longitude</param>
        /// <returns>An Area object if found within the area boundary, else return null</returns>
        [HttpGet]
        public Area GetAreaOfLocation(GeoCoordinate location)
        {
            // If location parameter is null, return null
            if (location == null) return null;

            // Loop through all the areas to find matching result
            foreach (Area area in areas)
            {
                // Using IsLocationInArea method to check if location lies within the area boundary
                if (IsLocationInArea(area.AreaBoundary, location))
                {
                    // If true, return area
                    return area;
                }
            }
            // If there is no matching area, return null
            return null;
        }

        /// <summary>
        /// Method to get respective area of given area id.
        /// </summary>
        /// <param name="areaId">A string with the acronym ("C", "NE", "NW", "SE" and SW") of an area.</param>
        /// <returns>An Area object if found matching result</returns>
        [HttpGet]
        public Area GetArea(string areaId)
        {
            // Loop through all areas to find matching result
            foreach (Area area in areas)
            {
                // If area object areaId matches the parameter areaId, return area
                if (area.AreaId.Equals(areaId)) return area;
            }
            // If there is no matching area id, return null
            return null;
        }

        /// <summary>
        /// Method to get all the areas.
        /// </summary>
        /// <returns>A list of area objects</returns>
        [HttpGet]
        public List<Area> GetAllAreas()
        {
            return areas;
        }

        /// <summary>
        /// Method to check if location lies within the area.
        /// Uses the PNPOLY algorithm based on the Jordan curve theorem.
        /// Source: https://www.ecse.rpi.edu/Homepages/wrf/Research/Short_Notes/pnpoly.html
        /// </summary>
        /// <param name="area">An array of GeoCoordinate of an area</param>
        /// <param name="location">A GeoCoordinate object with latitude and longitude variables</param>
        /// <returns></returns>
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

        /// <summary>
        /// Method to get GeoCoordinate of a postal code.
        /// </summary>
        /// <param name="postalCode">A string with the postal code values</param>
        /// <returns>A GeoCoordinate object with latitude and longitude variables</returns>
        [HttpGet]
        public GeoCoordinate GetCoordinate(string postalCode)
        {
            // If postalCode paramter is empty, return null
            if (postalCode.Equals("")) return null;
            try
            {
                // Form uri with Google Maps API, API Key and postal code
                string uri = "https://maps.googleapis.com/maps/api/geocode/xml?";
                string key = "AIzaSyAx2cHrI8CjdzkiByY_FS1nV93CFx9LD54";
                uri += "key=" + key + "&address=Singapore+" + postalCode;

                // To read result from uri
                XmlTextReader reader = new XmlTextReader(uri);
                reader.ReadToFollowing("location");
                // Get latitude
                reader.ReadToFollowing("lat");
                double lat = Convert.ToDouble(reader.ReadElementContentAsString());

                // Get longitude
                reader.ReadToFollowing("lng");
                double lng = Convert.ToDouble(reader.ReadElementContentAsString());

                // Return GeoCoordinate object with latitude and longitude values
                return new GeoCoordinate(lat, lng);
            }
            catch (Exception ex)
            {
                // Return null if there is an error
                return null;
            }
        }
    }
}
