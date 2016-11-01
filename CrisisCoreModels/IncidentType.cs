namespace CrisisCoreModels
{
    /// <summary>
    ///  This describes a type of incident.
    /// </summary>
    public class IncidentType
    {

        public string IncidentTypeId { get; set; }
        public string IncidentTypeName { get; set; }
        public int Severity { get; set; }
        public Agency Agency { get; set; }

        public IncidentType() { }

        public IncidentType(string incidentTypeId, string incidentTypeName, Agency agency)
        {
            IncidentTypeId = incidentTypeId;
            IncidentTypeName = incidentTypeName;
            Agency = agency;
        }
    }
}