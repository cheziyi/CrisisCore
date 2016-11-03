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
    /// NeaController web service for the retrieval of PSI and weather information from NEA api.
    /// </summary>
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class NeaController : ApiController
    {
        /// <summary>
        /// Method to get PSI of a particular area.
        /// </summary>
        /// <param name="areaId">The id of an area</param>
        /// <returns></returns>
        [HttpGet]
        public IncidentType GetPsiInArea(string areaId)
        {
            // Initialize an IncidentType object
            IncidentType incidentType = new IncidentType();
            incidentType.IncidentTypeId = "HAZE";
            incidentType.IncidentTypeName = "Haze PSI Level";

            try
            {
                // The url to call to access NEA API for PSI information
                string uri = "http://api.nea.gov.sg/api/WebAPI/?dataset=psi_update&keyref=781CF461BB6606AD1260F4D81345157F37A0C67E3301AC5F";

                XmlTextReader reader = new XmlTextReader(uri);

                string neaArea = "";

                // Matching area id to NEA area format
                if (areaId.Equals("SW"))
                    neaArea = "rWE";
                else if (areaId.Equals("NW"))
                    neaArea = "rNO";
                else if (areaId.Equals("C"))
                    neaArea = "rSO";
                else if (areaId.Equals("NE"))
                    neaArea = "rCE";
                else if (areaId.Equals("SE"))
                    neaArea = "rEA";

                while (!reader.EOF)
                {
                    // If not at the end of file, read until "id" has been found
                    reader.ReadToFollowing("id");
                    if (reader.ReadElementContentAsString().Equals(neaArea))
                    {
                        // If the read value matches the area id, read until "reading" has been found
                        reader.ReadToFollowing("reading");
                        // Access the "value" attribute to get PSI
                        reader.MoveToAttribute("value");
                        // Set value into the incident type
                        incidentType.Severity = Convert.ToInt32(reader.Value);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                // Return null if there is an error
                return null;
            }
            // Return IncidentType object
            return incidentType;
        }

        /// <summary>
        /// Method to get weather informatiaon.
        /// </summary>
        /// <returns>A list of WeatherCondition objects</returns>
        [HttpGet]
        public List<WeatherCondition> GetWeather()
        {
            // Initialize a list of WeatherCondition objects
            List<WeatherCondition> weatherConditions = new List<WeatherCondition>();
            try
            {
                // The url to call to access NEA API for weather information
                string uri = "http://api.nea.gov.sg/api/WebAPI/?dataset=2hr_nowcast&keyref=781CF461BB6606AD1260F4D81345157F37A0C67E3301AC5F";

                XmlTextReader reader = new XmlTextReader(uri);

                while (!reader.EOF)
                {
                    // If not at the end of the file, create new WeatherCondition object with relevant values
                    WeatherCondition weatherCond = new WeatherCondition();
                    weatherCond.Location = new GeoCoordinate();
                    reader.ReadToFollowing("area");
                    reader.MoveToAttribute("forecast");
                    weatherCond.ImageUri = "http://www.nea.gov.sg/Html/Nea/images/common/weather/23px/" + reader.Value + ".png";
                    reader.MoveToAttribute("lat");
                    weatherCond.Location.Latitude = Convert.ToDouble(reader.Value);
                    reader.MoveToAttribute("lon");
                    weatherCond.Location.Longitude = Convert.ToDouble(reader.Value);
                    reader.MoveToAttribute("name");
                    weatherCond.TownName = reader.Value;
                    // Add WeatherCondition object into the list of weather condition
                    weatherConditions.Add(weatherCond);
                }
            }
            catch (Exception ex)
            {
            }
            // Return the list of WeatherCondition objects
            return weatherConditions;
        }

        /// <summary>
        /// This describes the weather condition of a location.
        /// </summary>
        public class WeatherCondition
        {
            public string TownName { get; set; }
            public GeoCoordinate Location { get; set; }
            public string ImageUri { get; set; }
        }
    }
}
