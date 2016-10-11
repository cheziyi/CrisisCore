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
        [HttpPost]
        public string GenerateReport()
        {
            string content = "";
            using (AreaController ac = new AreaController())
            using (IncidentTypeController itc = new IncidentTypeController())
            {
                //List<Area> areas = ac.GetAllAreas();

                //foreach(Area area in areas)
                //{
                //    List<IncidentTypes> incidentTypes = GetIncidentTypesInArea(area.AreaId);
                //    List<IncidentTypes> RecentIncidentTypes = GetIncidentTypesInArea(area.AreaId);


                //    content += "";
                    // Do whatever you want incidentTypes
                //}
            }
            //  GetIncidentTypesInArea(string areaId)
            //  GetRecentIncidentTypesInArea(string areaId)
            return content;
        }
    }
}