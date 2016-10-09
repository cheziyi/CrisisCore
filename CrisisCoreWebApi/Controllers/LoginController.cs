using CrisisCoreModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web.Http;

namespace CrisisCoreWebApi.Controllers
{
    public class LoginController : ApiController
    {
        String connString = ConfigurationManager.ConnectionStrings["conn"].ConnectionString;

        [HttpPost]
        public Account GetIncidentType(Account account)
        {
            try
            {
                string hashedPassword = Hash(account.AccountId + account.Password);

                using (SqlConnection conn = new SqlConnection(connString))
                {
                    String query = "SELECT * FROM Accounts WHERE AccountId=@AccountId AND Password=@Password";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@AccountId", account.AccountId);
                        cmd.Parameters.AddWithValue("@Password", hashedPassword);
                        conn.Open();

                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                account.AccessLevel = Convert.ToInt32(dr["IncidentTypeId"]);
                                return account;
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




        private static string Hash(string stringToHash)
        {
            using (var sha1 = new SHA1Managed())
            {
                return BitConverter.ToString(sha1.ComputeHash(Encoding.UTF8.GetBytes(stringToHash)));
            }
        }
    }
}
