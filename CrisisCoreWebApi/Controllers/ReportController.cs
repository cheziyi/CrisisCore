using CrisisCoreModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CrisisCoreWebApi.Controllers
{
    public class ReportController : ApiController
    {
        [HttpGet]
        public string GenerateReport()
        {
            string content = "<font face=\"Myriad Pro\"><h1>Report for " + DateTime.Now + "</h1>";
            using (AreaController ac = new AreaController())
            using (IncidentTypeController itc = new IncidentTypeController())
            {
                List<Area> areas = ac.GetAllAreas();

                foreach (Area area in areas)
                {
                    List<IncidentType> incidentTypes = itc.GetIncidentTypesInArea(area.AreaId);
                    List<IncidentType> recentIncidentTypes = itc.GetRecentIncidentTypesInArea(area.AreaId);
                    content += "<h2>" + area.AreaName + "</h2>";
                    content += "<h3>New Incidents</h3><p>";
                    foreach (IncidentType type in recentIncidentTypes)
                    {
                        content += "<strong>" + type.IncidentTypeName + ":</strong> " + type.Severity + "<br />";
                    }
                    content += "</p><h3>Total Incidents</h3>";
                    foreach (IncidentType type in incidentTypes)
                    {
                        content += "<strong>" + type.IncidentTypeName + ":</strong> " + type.Severity + "<br />";
                    }
                    content += "</p>";

                }
            }
            content += "</font>";
            return content;
        }
    }
}