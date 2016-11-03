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

namespace CrisisCoreWebApi.Controllers
{
    /// <summary>
    /// IncidentController web service class for retrieval and updating of incidents in database.
    /// </summary>
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class IncidentController : ApiController
    {
        /// <summary>
        /// SQL connection string from web.config
        /// </summary>
        String connString = ConfigurationManager.ConnectionStrings["conn"].ConnectionString;
        
        /// <summary>
        /// Method to get all unresolved incidents.
        /// </summary>
        /// <returns>A list of Incident objects</returns>
        [HttpGet]
        public List<Incident> GetUnresolvedIncidents()
        {
            // Initialize a list of Incident objects
            List<Incident> incidents = new List<Incident>();
            try
            {
                using (IncidentTypeController itc = new IncidentTypeController())
                using (AreaController ac = new AreaController())
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    // Query the databse for all incidents that are unresolved
                    String query = "SELECT * FROM Incidents WHERE ResolveDateTime IS NULL";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        conn.Open();

                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                // If a row is returned, create new Incident object with relevant values
                                Incident incident = new Incident();
                                incident.IncidentId = Convert.ToInt32(dr["IncidentId"]);
                                incident.IncidentType = new IncidentType(dr["IncidentTypeId"].ToString(), null, null);
                                incident.PostalCode = dr["PostalCode"].ToString();
                                incident.ReportDateTime = Convert.ToDateTime(dr["ReportDateTime"]);
                                incident.ReportMobile = dr["ReportMobile"].ToString();
                                incident.ReportName = dr["ReportName"].ToString();
                                incident.UnitNo = dr["UnitNo"].ToString();
                                incident.AddInfo = dr["AddInfo"].ToString();
                                incident.Area = new Area(dr["AreaId"].ToString(), null, null);
                                incident.Location = new GeoCoordinate(Convert.ToDouble(dr["LocLat"]), Convert.ToDouble(dr["LocLon"]));

                                // Add Incident object into the list of incidents
                                incidents.Add(incident);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            // Return the list of Incident objects
            return incidents;
        }

        /// <summary>
        /// Method to add an incident with relevant information into database.
        /// </summary>
        /// <param name="incident">An Incident object with relevant information</param>
        /// <returns>A Boolean true or false to indicidate status of insertion</returns>
        [HttpPost]
        public bool AddIncident(Incident incident)
        {
            try
            {
                using (ModemController mc = new ModemController())
                using (IncidentTypeController itc = new IncidentTypeController())
                using (AreaController ac = new AreaController())
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    // Query to insert incident information into database
                    String query = "INSERT INTO Incidents(PostalCode,UnitNo,AddInfo,IncidentTypeId,AreaId,ReportName,ReportMobile,ReportDateTime,LocLat,LocLon) VALUES(@PostalCode,@UnitNo,@AddInfo,@IncidentTypeId,@AreaId,@ReportName,@ReportMobile,GETDATE(),@LocLat,@LocLon)";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        // Add location of where the incident has occurred
                        GeoCoordinate location = ac.GetCoordinate(incident.PostalCode);
                        cmd.Parameters.AddWithValue("@PostalCode", incident.PostalCode);
                        cmd.Parameters.AddWithValue("@UnitNo", incident.UnitNo);
                        cmd.Parameters.AddWithValue("@AddInfo", incident.AddInfo);
                        cmd.Parameters.AddWithValue("@IncidentTypeId", incident.IncidentType.IncidentTypeId);
                        cmd.Parameters.AddWithValue("@AreaId", ac.GetAreaOfLocation(location).AreaId);
                        cmd.Parameters.AddWithValue("@ReportName", incident.ReportName);
                        cmd.Parameters.AddWithValue("@ReportMobile", incident.ReportMobile);
                        cmd.Parameters.AddWithValue("@LocLat", location.Latitude);
                        cmd.Parameters.AddWithValue("@LocLon", location.Longitude);
                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                    // Get incident type of the incident with GetIncidentType method
                    IncidentType type = itc.GetIncidentType(incident.IncidentType.IncidentTypeId);

                    // If incident type has an agency, form message and send sms to the agency
                    if (type.Agency != null)
                    {
                        string message = "CrisisCore: " + type.Agency.AgencyId + ", " + type.IncidentTypeName +
                            " assistance required at " + incident.PostalCode + ", " + incident.UnitNo + " (" + incident.AddInfo + ")";

                        return mc.SendSms(type.Agency.AgencyContact, message);
                    }
                    // Return true if there is no error
                    return true;
                }
            }
            catch (Exception ex)
            {
                // Return false if there is an error
                return false;
            }

        }

        /// <summary>
        /// Method to change the resolve date and time to current date and time to indicate an incident has been resolved.
        /// </summary>
        /// <param name="incidentId">The id of an incident that has been resolved</param>
        /// <returns>A Boolean true or false to indicate status of updatereturns>
        [HttpGet]
        public bool ResolveIncident(int incidentId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    // Query to update the particular incident's ResolveDateTime to current date and time to indicate that the incident is resolved
                    String query = "UPDATE Incidents SET ResolveDateTime=GETDATE() WHERE IncidentId=@IncidentId;";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@IncidentId", incidentId);
                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
                // Return true if success
                return true;
            }
            catch (Exception ex)
            {
                // Return false if there is an error
                return false;
            }

        }
    }
}
