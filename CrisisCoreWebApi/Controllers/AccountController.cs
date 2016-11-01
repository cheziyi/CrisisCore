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
    /// <summary>
    /// Class to support logging in and retrieving access level of users.
    /// </summary>
    public class AccountController : ApiController
    {
        /// <summary>
        /// SQL connection string from web.config
        /// </summary>
        String connString = ConfigurationManager.ConnectionStrings["conn"].ConnectionString;

        /// <summary>
        /// Method to check user credentials.
        /// </summary>
        /// <param name="account">An account object with user input username and password</param>
        /// <returns>An Account object with access level of the user if login is successful</returns>
        [HttpPost]
        public Account Login(Account account)
        {
            try
            {
                // Concatenate and hash the username and password
                string hashedPassword = Hash(account.AccountId + account.Password);

                using (SqlConnection conn = new SqlConnection(connString))
                {
                    // Query the database with for matching username and hashed password
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
                                // If a row is returned, add access level of user into Account object
                                account.AccessLevel = Convert.ToInt32(dr["AccessLevel"]);
                                // Remove password for security reasons
                                account.Password = "";
                                // Return account
                                return account;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            // If there is no matching row or an error, return null
            return null;
        }

        /// <summary>
        /// Method to hash the password and username using SHA-1 encryption.
        /// </summary>
        /// <param name="stringToHash">The password and username concatenated together</param>
        /// <returns></returns>
        private static string Hash(string stringToHash)
        {
            using (var sha1 = new SHA1Managed())
            {
                // Convert string to bytes as UTF8 and hash using SHA-1, and return the result
                return BitConverter.ToString(sha1.ComputeHash(Encoding.UTF8.GetBytes(stringToHash))).Replace("-", "").ToLower();
            }
        }
    }
}
