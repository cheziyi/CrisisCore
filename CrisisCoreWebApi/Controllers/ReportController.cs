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
        /// <summary>
        /// Generate a status report summarised of key indicators and trends for email
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public string GenerateReport()
        {
            // Header to show date and time for the report
            string content = "<font face=\"Myriad Pro\"><h1>Report for " + DateTime.Now + "</h1>";

            using (AreaController ac = new AreaController())
            using (IncidentTypeController itc = new IncidentTypeController())
            {
                // Method to get a a list of area in Singapore from the AreaController and initialize to the list areas object
                //("Central", "North East", "North West", "South East" and "South West")
                List<Area> areas = ac.GetAllAreas();

                // Loop through the list area object
                foreach (Area area in areas)
                {
                    // Method to get a a list of incident types from the incidentTypeController and initialize to the list incidentTypes object
                    List<IncidentType> incidentTypes = itc.GetIncidentTypesInArea(area.AreaId);

                    // Method to get a list of recent incident types from the incidentTypeController and initialize to the list recentIncidentTypes object
                    List<IncidentType> recentIncidentTypes = itc.GetRecentIncidentTypesInArea(area.AreaId);

                    // Display area name and "New Incidents"
                    content += "<h2>" + area.AreaName + "</h2>";
                    content += "<h3>New Incidents</h3><p>";

                    // Loop through the list recentIncidentTypes object
                    foreach (IncidentType type in recentIncidentTypes)
                    {
                        // Display recent incident and severity content
                        content += "<strong>" + type.IncidentTypeName + ":</strong> " + type.Severity + "<br />";
                    }

                    // Display "Total Incidents"
                    content += "</p><h3>Total Incidents</h3>";

                    // Loop through the list incidentTypes object
                    foreach (IncidentType type in incidentTypes)
                    {
                        // Display total incident and severity content
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