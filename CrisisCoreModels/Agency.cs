namespace CrisisCoreModels
{
    /// <summary>
    ///  This describes a government agency for responding to incidents.
    /// </summary>
    public class Agency
    {
        public string AgencyId { get; set; }
        public string AgencyName { get; set; }
        public string AgencyContact { get; set; }

        public Agency() { }

        public Agency(string agencyId, string agencyName, string agencyContact)
        {
            AgencyId = agencyId;
            AgencyName = agencyName;
            AgencyContact = agencyContact;
        }
    }
}