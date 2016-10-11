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
    public class IncidentController : ApiController
    {
        String connString = ConfigurationManager.ConnectionStrings["conn"].ConnectionString;

        [HttpGet]
        public IEnumerable<Incident> GetUnresolvedIncidents()
        {
            List<Incident> incidents = new List<Incident>();
            try
            {
                using (IncidentTypeController itc = new IncidentTypeController())
                using (AreaController ac = new AreaController())
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    String query = "SELECT * FROM Incidents WHERE ResolveDateTime IS NULL";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        conn.Open();

                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
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
                                incident.Location = ac.GetCoordinate(incident.PostalCode);

                                incidents.Add(incident);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return incidents;
        }

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
                    String query = "INSERT INTO Incidents(PostalCode,UnitNo,AddInfo,IncidentTypeId,AreaId,ReportName,ReportMobile,ReportDateTime) VALUES(@PostalCode,@UnitNo,@AddInfo,@IncidentTypeId,@AreaId,@ReportName,@ReportMobile,GETDATE())";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@PostalCode", incident.PostalCode);
                        cmd.Parameters.AddWithValue("@UnitNo", incident.UnitNo);
                        cmd.Parameters.AddWithValue("@AddInfo", incident.AddInfo);
                        cmd.Parameters.AddWithValue("@IncidentTypeId", incident.IncidentType.IncidentTypeId);
                        cmd.Parameters.AddWithValue("@AreaId", ac.GetAreaFromPostalCode(incident.PostalCode).AreaId);
                        cmd.Parameters.AddWithValue("@ReportName", incident.ReportName);
                        cmd.Parameters.AddWithValue("@ReportMobile", incident.ReportMobile);
                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                    IncidentType type = itc.GetIncidentType(incident.IncidentType.IncidentTypeId);

                    string message = "CrisisCore: " + type.Agency.AgencyId + ", " + type.IncidentTypeName +
                        " assistance required at " + incident.PostalCode + ", " + incident.UnitNo + " (" + incident.AddInfo + ")";

                    return mc.SendSms(type.Agency.AgencyContact, message);
                }
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        [HttpPost]
        public bool ResolveIncident(Incident incident)
        {
            try
            {
                using (AreaController ac = new AreaController())
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    String query = "UPDATE Incidents SET ResolveDateTime=GETDATE() WHERE IncidentId=@IncidentId;";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@IncidentId", incident.IncidentId);
                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }
    }
}
