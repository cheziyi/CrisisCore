using CrisisCoreModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CrisisCoreWebApi.Controllers
{
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
                    String query = "SELECT IncidentTypes.*, Agencies.* FROM dbo.IncidentTypes LEFT JOIN Agencies ON Agencies.AgencyId = IncidentTypes.AgencyId WHERE IncidentTypes.IncidentTypeId=@IncidentTypeId";
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
                    String query = "SELECT IncidentTypes.*, Agencies.* FROM dbo.IncidentTypes LEFT JOIN Agencies ON Agencies.AgencyId = IncidentTypes.AgencyId";
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
                    String query = "SELECT IncidentTypes.IncidentTypeId, COUNT(Incidents.IncidentId) AS IncidentCount FROM IncidentTypes LEFT JOIN Incidents ON IncidentTypes.IncidentTypeId = Incidents.IncidentTypeId AND Incidents.AreaId=@AreaId GROUP BY IncidentTypes.IncidentTypeId";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@AreaId", areaId);
                        conn.Open();

                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                IncidentType incidentType = GetIncidentType(dr["IncidentTypeId"].ToString());
                                incidentType.IncidentsCount = Convert.ToInt32(dr["IncidentCount"]);
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
    }
}
