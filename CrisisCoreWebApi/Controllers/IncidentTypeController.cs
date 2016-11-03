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
    /// <summary>
    /// IncidentTypeController web service class for retrieval of incident types in database.
    /// </summary>
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class IncidentTypeController : ApiController
    {
        /// <summary>
        /// SQL connection string from web.config
        /// </summary>
        String connString = ConfigurationManager.ConnectionStrings["conn"].ConnectionString;

        /// <summary>
        /// Method to get particular incident type from given id.
        /// id can be in the format of "AWBL", "DENGUE", "EVAC", "FIRE", "GAS", "ZIKA" and "HAZE".
        /// </summary>
        /// <param name="id">The id of an incident type</param>
        /// <returns>An IncidentType object with relevant information</returns>
        [HttpGet]
        public IncidentType GetIncidentType(string id)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    // Query the database for all incident types and matching agencies for the incident type
                    String query = "SELECT IncidentTypes.*, Agencies.* FROM IncidentTypes LEFT JOIN Agencies ON Agencies.AgencyId = IncidentTypes.AgencyId WHERE IncidentTypes.IncidentTypeId=@IncidentTypeId";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@IncidentTypeId", id);
                        conn.Open();

                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                // If a row is returned, create new IncidentType object with relevant values
                                IncidentType incidentType = new IncidentType();
                                incidentType.IncidentTypeId = dr["IncidentTypeId"].ToString();
                                incidentType.IncidentTypeName = dr["IncidentTypeName"].ToString();
                                if (!(dr["AgencyId"] is DBNull))
                                {
                                    // If agency is found, add Agency object with relevant values to the retrieved IncidentType object
                                    incidentType.Agency = new Agency();
                                    incidentType.Agency.AgencyId = dr["AgencyId"].ToString();
                                    incidentType.Agency.AgencyName = dr["AgencyName"].ToString();
                                    incidentType.Agency.AgencyContact = dr["AgencyContact"].ToString();
                                }
                                // Return IncidentType object
                                return incidentType;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            // Return null if there is no matching row or an error
            return null;
        }

        /// <summary>
        /// Method to get all incident types.
        /// </summary>
        /// <returns>A list of IncidentType object</returns>
        [HttpGet]
        public List<IncidentType> GetIncidentTypes()
        {
            // Initialize a list of IncidentType objects
            List<IncidentType> incidentTypes = new List<IncidentType>();
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    // Query the database for all incident types and matching agencies for the incident type
                    String query = "SELECT IncidentTypes.*, Agencies.* FROM IncidentTypes LEFT JOIN Agencies ON Agencies.AgencyId = IncidentTypes.AgencyId";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        conn.Open();

                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                // If a row is returned, create new IncidentType object with relevant values
                                IncidentType incidentType = new IncidentType();
                                incidentType.IncidentTypeId = dr["IncidentTypeId"].ToString();
                                incidentType.IncidentTypeName = dr["IncidentTypeName"].ToString();
                                if (!(dr["AgencyId"] is DBNull))
                                {
                                    // If agency is found, add Agency object with relevant values to the retrieved IncidentType object
                                    incidentType.Agency = new Agency();
                                    incidentType.Agency.AgencyId = dr["AgencyId"].ToString();
                                    incidentType.Agency.AgencyName = dr["AgencyName"].ToString();
                                    incidentType.Agency.AgencyContact = dr["AgencyContact"].ToString();
                                }
                                // Add IncidentType object into the list of incident types
                                incidentTypes.Add(incidentType);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            // Return the list of IncidentType objects
            return incidentTypes;
        }

        /// <summary>
        /// Method to get incident types in a particular area.
        /// </summary>
        /// <param name="areaId">The id of an area</param>
        /// <returns>A list of IncidentType objects in the particular area</returns>
        [HttpGet]
        public List<IncidentType> GetIncidentTypesInArea(string areaId)
        {
            // Initialize a list of IncidentType objects
            List<IncidentType> incidentTypes = new List<IncidentType>();
            try
            {
                using (AreaController ac = new AreaController())
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    // Query the database for all incident types with matching area id
                    String query = "SELECT IncidentTypes.IncidentTypeId, COUNT(Incidents.IncidentId) AS IncidentCount FROM IncidentTypes LEFT JOIN Incidents ON IncidentTypes.IncidentTypeId = Incidents.IncidentTypeId AND Incidents.AreaId=@AreaId AND Incidents.ResolveDateTime IS NULL GROUP BY IncidentTypes.IncidentTypeId";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@AreaId", areaId);
                        conn.Open();

                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                // If a row is returned, create new IncidentType object with relevant values
                                IncidentType incidentType = GetIncidentType(dr["IncidentTypeId"].ToString());
                                incidentType.Severity = Convert.ToInt32(dr["IncidentCount"]);
                                // Add IncidentType object into the list of incident types
                                incidentTypes.Add(incidentType);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            using (NeaController nc = new NeaController())
                // Add haze incident type in the particular area into the list of incident types
                incidentTypes.Add(nc.GetPsiInArea(areaId));

            // Return the list of IncidentType objects
            return incidentTypes;
        }

        /// <summary>
        /// Method to get incident types of the past 30 minutes in a particular area.
        /// </summary>
        /// <param name="areaId">The id of an area</param>
        /// <returns>A list of IncidentType objects in the particular area</returns>
        [HttpGet]
        public List<IncidentType> GetRecentIncidentTypesInArea(string areaId)
        {
            // Initialize a list of IncidentType objects
            List<IncidentType> incidentTypes = new List<IncidentType>();
            try
            {
                using (AreaController ac = new AreaController())
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    // Query the database for all incident types with matching area id within the past 30 minutes
                    String query = "SELECT IncidentTypes.IncidentTypeId, COUNT(Incidents.IncidentId) AS IncidentCount FROM IncidentTypes LEFT JOIN Incidents ON IncidentTypes.IncidentTypeId = Incidents.IncidentTypeId AND Incidents.AreaId=@AreaId AND ReportDateTime>DATEADD(minute, -30, GETDATE()) GROUP BY IncidentTypes.IncidentTypeId";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@AreaId", areaId);
                        conn.Open();

                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                // If a row is returned, create new IncidentType object with relevant values
                                IncidentType incidentType = GetIncidentType(dr["IncidentTypeId"].ToString());
                                incidentType.Severity = Convert.ToInt32(dr["IncidentCount"]);
                                // Add IncidentType object into the list of incident types
                                incidentTypes.Add(incidentType);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }

            // Return the list of IncidentType objects
            return incidentTypes;
        }
    }
}
