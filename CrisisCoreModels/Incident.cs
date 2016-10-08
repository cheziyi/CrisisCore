using System;

namespace CrisisCoreModels
{
    public class Incident
    {
        public int IncidentId { get; set; }
        public string PostalCode { get; set; }
        public string UnitNo { get; set; }
        public string AddInfo { get; set; }
        public IncidentType IncidentType { get; set; }
        public Area Area { get; set; }
        public string ReportName { get; set; }
        public string ReportMobile { get; set; }
        public DateTime ReportDateTime { get; set; }
        public DateTime ResolveDateTime { get; set; }
    }
}