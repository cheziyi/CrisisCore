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
    public class NeaController : ApiController
    {
        [HttpGet]
        public IncidentType GetPsiInArea(string areaId)
        {
            IncidentType incidentType = new IncidentType();
            incidentType.IncidentTypeId = "HAZE";
            incidentType.IncidentTypeName = "Haze PSI Level";

            try
            {
                string uri = "http://api.nea.gov.sg/api/WebAPI/?dataset=psi_update&keyref=781CF461BB6606AD1260F4D81345157F37A0C67E3301AC5F";

                XmlTextReader reader = new XmlTextReader(uri);

                string neaArea = "";

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
                    reader.ReadToFollowing("id");
                    if (reader.ReadElementContentAsString().Equals(neaArea))
                    {
                        reader.ReadToFollowing("reading");
                        reader.MoveToAttribute("value");
                        incidentType.Severity = Convert.ToInt32(reader.Value);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }
            return incidentType;
        }

        [HttpGet]
        public List<WeatherCondition> GetWeather()
        {
            List<WeatherCondition> weatherConditions = new List<WeatherCondition>();
            try
            {
                string uri = "http://api.nea.gov.sg/api/WebAPI/?dataset=2hr_nowcast&keyref=781CF461BB6606AD1260F4D81345157F37A0C67E3301AC5F";

                XmlTextReader reader = new XmlTextReader(uri);

                while (!reader.EOF)
                {
                    WeatherCondition weatherCond = new WeatherCondition();
                    weatherCond.Location = new GeoCoordinate();
                    reader.ReadToFollowing("area");
                    reader.MoveToAttribute("forecast");
                    weatherCond.ImageUri = "http://www.nea.gov.sg/Html/Nea/images/common/weather/50px/" + reader.Value + ".png";
                    reader.MoveToAttribute("lat");
                    weatherCond.Location.Latitude = Convert.ToDouble(reader.Value);
                    reader.MoveToAttribute("lon");
                    weatherCond.Location.Longitude = Convert.ToDouble(reader.Value);
                    reader.MoveToAttribute("name");
                    weatherCond.TownName = reader.Value;
                    weatherConditions.Add(weatherCond);
                }
            }
            catch (Exception ex)
            {
                // return null;
            }
            return weatherConditions;
        }

        public class WeatherCondition
        {
            public string TownName { get; set; }
            public GeoCoordinate Location { get; set; }
            public string ImageUri { get; set; }
        }
    }
}
