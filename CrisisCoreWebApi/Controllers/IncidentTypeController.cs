using CrisisCoreModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Xml;

namespace CrisisCoreWebApi.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class IncidentTypeController : ApiController
    {
        String connString = ConfigurationManager.ConnectionStrings["conn"].ConnectionString;

        //private List<IncidentType> incidentTypes;
        //public IncidentTypeController()
        //{
        //    incidentTypes = new List<IncidentType>();

        //    Agency scdf = new Agency("SCDF", "Singapore Civil Defence Force", "92299962");
        //    Agency sp = new Agency("SP", "Singapore Power", "92299962");

        //    incidentTypes.Add(new IncidentType("AMBL", "Emergency Ambulance", scdf));
        //    incidentTypes.Add(new IncidentType("EVAC", "Rescue and Evacuation", scdf));
        //    incidentTypes.Add(new IncidentType("FIRE", "Fire-Fighting", scdf));
        //    incidentTypes.Add(new IncidentType("GAS", "Gas Leak Control", sp));
        //    incidentTypes.Add(new IncidentType("DENGUE", "Dengue Cluster", null));
        //    incidentTypes.Add(new IncidentType("ZIKA", "Zika Cluster", null));
        //}

        [HttpGet]
        public IncidentType GetIncidentType(string id)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    String query = "SELECT IncidentTypes.*, Agencies.* FROM IncidentTypes LEFT JOIN Agencies ON Agencies.AgencyId = IncidentTypes.AgencyId WHERE IncidentTypes.IncidentTypeId=@IncidentTypeId";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@IncidentTypeId", id);
                        conn.Open();

                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                IncidentType incidentType = new IncidentType();
                                incidentType.IncidentTypeId = dr["IncidentTypeId"].ToString();
                                incidentType.IncidentTypeName = dr["IncidentTypeName"].ToString();
                                if (!(dr["AgencyId"] is DBNull))
                                {
                                    incidentType.Agency = new Agency();
                                    incidentType.Agency.AgencyId = dr["AgencyId"].ToString();
                                    incidentType.Agency.AgencyName = dr["AgencyName"].ToString();
                                    incidentType.Agency.AgencyContact = dr["AgencyContact"].ToString();
                                }
                                return incidentType;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return null;
        }


        [HttpGet]
        public IEnumerable<IncidentType> GetIncidentTypes()
        {
            List<IncidentType> incidentTypes = new List<IncidentType>();
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    String query = "SELECT IncidentTypes.*, Agencies.* FROM IncidentTypes LEFT JOIN Agencies ON Agencies.AgencyId = IncidentTypes.AgencyId";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        conn.Open();

                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                IncidentType incidentType = new IncidentType();
                                incidentType.IncidentTypeId = dr["IncidentTypeId"].ToString();
                                incidentType.IncidentTypeName = dr["IncidentTypeName"].ToString();
                                if (!(dr["AgencyId"] is DBNull))
                                {
                                    incidentType.Agency = new Agency();
                                    incidentType.Agency.AgencyId = dr["AgencyId"].ToString();
                                    incidentType.Agency.AgencyName = dr["AgencyName"].ToString();
                                    incidentType.Agency.AgencyContact = dr["AgencyContact"].ToString();
                                }
                                incidentTypes.Add(incidentType);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return incidentTypes;
        }



        [HttpGet]
        public IEnumerable<IncidentType> GetIncidentTypesInArea(string areaId)
        {
            List<IncidentType> incidentTypes = new List<IncidentType>();
            try
            {
                using (AreaController ac = new AreaController())
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    String query = "SELECT IncidentTypes.IncidentTypeId, COUNT(Incidents.IncidentId) AS IncidentCount FROM IncidentTypes LEFT JOIN Incidents ON IncidentTypes.IncidentTypeId = Incidents.IncidentTypeId AND Incidents.AreaId=@AreaId AND Incidents.ResolveDateTime IS NULL GROUP BY IncidentTypes.IncidentTypeId";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@AreaId", areaId);
                        conn.Open();

                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                IncidentType incidentType = GetIncidentType(dr["IncidentTypeId"].ToString());
                                incidentType.Severity = Convert.ToInt32(dr["IncidentCount"]);
                                incidentTypes.Add(incidentType);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            incidentTypes.Add(GetPSIInArea(areaId));

            return incidentTypes;
        }

        [HttpGet]
        public IEnumerable<IncidentType> GetRecentIncidentTypesInArea(string areaId)
        {
            List<IncidentType> incidentTypes = new List<IncidentType>();
            try
            {
                using (AreaController ac = new AreaController())
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    String query = "SELECT IncidentTypes.IncidentTypeId, COUNT(Incidents.IncidentId) AS IncidentCount FROM IncidentTypes LEFT JOIN Incidents ON IncidentTypes.IncidentTypeId = Incidents.IncidentTypeId AND Incidents.AreaId=@AreaId AND ReportDateTime>DATEADD(minute, -30, GETDATE()) GROUP BY IncidentTypes.IncidentTypeId";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@AreaId", areaId);
                        conn.Open();

                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                IncidentType incidentType = GetIncidentType(dr["IncidentTypeId"].ToString());
                                incidentType.Severity = Convert.ToInt32(dr["IncidentCount"]);
                                incidentTypes.Add(incidentType);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }

            incidentTypes.Add(GetPSIInArea(areaId));

            return incidentTypes;
        }

        [HttpGet]
        public IncidentType GetPSIInArea(string areaId)
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
    }
}
