using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web.Http;

namespace CrisisCoreWebApi.Controllers
{
    /// <summary>
    /// TwitterController web service for the authentication and posting of Twitter message using Twitter API
    /// </summary>
    public class TwitterController : ApiController
    {
        //Initialization of variables
        string TwitterApiBaseUrl = "https://api.twitter.com/1.1/";
        string consumerKey = "sUXovK3GAYMGJVMF6Gni471YS";
        string consumerKeySecret = "pWNOtefrnTBSfvt4fJGnVQaf2fmRjY85tqi8dYdvCXGo4rfh01";
        string accessToken = "775923962165415936-AOy9OvRiUr4CqMoiHxlqlfvTBDQitEA";
        string accessTokenSecret = "6BoLGSspbyr2RI1kUW8iiGMKunJYKTUfApssjoALrQDf1";
        HMACSHA1 sigHasher;
        readonly DateTime epochUtc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// Sends a tweet with the supplied text and returns the response from the Twitter API.
        /// </summary>
        /// <param name="status"></param>
        /// <returns>Response from Twitter API</returns>
        [HttpPost]
        public string Tweet([FromBody] string status)
        {
            sigHasher = new HMACSHA1(new ASCIIEncoding().GetBytes(string.Format("{0}&{1}", consumerKeySecret, accessTokenSecret)));

            var data = new Dictionary<string, string> {
            { "status", status },
            { "trim_user", "1" }
        };

            return SendRequest("statuses/update.json", data);
        }

        /// <summary>
        /// Add URL, form data and authentication
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <returns>URL, oAuthHeader, form data</returns>
        string SendRequest(string url, Dictionary<string, string> data)
        {
            var fullUrl = TwitterApiBaseUrl + url;

            // Timestamps are in seconds since 1/1/1970.
            var timestamp = (int)((DateTime.UtcNow - epochUtc).TotalSeconds);

            // Add all the OAuth headers we'll need to use when constructing the hash.
            data.Add("oauth_consumer_key", consumerKey);
            data.Add("oauth_signature_method", "HMAC-SHA1");
            data.Add("oauth_timestamp", timestamp.ToString());
            data.Add("oauth_nonce", "a"); // Required, but Twitter doesn't appear to use it, so "a" will do.
            data.Add("oauth_token", accessToken);
            data.Add("oauth_version", "1.0");

            // Generate the OAuth signature and add it to our payload.
            data.Add("oauth_signature", GenerateSignature(fullUrl, data));

            // Build the OAuth HTTP Header from the data.
            string oAuthHeader = GenerateOAuthHeader(data);

            // Build the form data (exclude OAuth stuff that's already in the header).
            var formData = new FormUrlEncodedContent(data.Where(kvp => !kvp.Key.StartsWith("oauth_")));

            return SendRequest(fullUrl, oAuthHeader, formData);
        }

        /// <summary>
        /// Generate an OAuth signature from OAuth header values.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <returns>OAuth signature</returns>
        string GenerateSignature(string url, Dictionary<string, string> data)
        {
            var sigString = string.Join(
                "&",
                data
                    .Union(data)
                    .Select(kvp => string.Format("{0}={1}", Uri.EscapeDataString(kvp.Key), Uri.EscapeDataString(kvp.Value)))
                    .OrderBy(s => s)
            );

            var fullSigData = string.Format(
                "{0}&{1}&{2}",
                "POST",
                Uri.EscapeDataString(url),
                Uri.EscapeDataString(sigString.ToString())
            );

            return Convert.ToBase64String(sigHasher.ComputeHash(new ASCIIEncoding().GetBytes(fullSigData.ToString())));
        }

        /// <summary>
        /// Generate the raw OAuth HTML header from the values (including signature).
        /// </summary>
        /// <param name="data"></param>
        /// <returns>OAuth HTML header</returns>
        string GenerateOAuthHeader(Dictionary<string, string> data)
        {
            return "OAuth " + string.Join(
                ", ",
                data
                    .Where(kvp => kvp.Key.StartsWith("oauth_"))
                    .Select(kvp => string.Format("{0}=\"{1}\"", Uri.EscapeDataString(kvp.Key), Uri.EscapeDataString(kvp.Value)))
                    .OrderBy(s => s)
            );
        }

        /// <summary>
        /// Send HTTP Request and return the response.
        /// </summary>
        /// <param name="fullUrl"></param>
        /// <param name="oAuthHeader"></param>
        /// <param name="formData"></param>
        /// <returns>Response</returns>
        string SendRequest(string fullUrl, string oAuthHeader, FormUrlEncodedContent formData)
        {
            using (var http = new HttpClient())
            {
                http.DefaultRequestHeaders.Add("Authorization", oAuthHeader);

                var httpResp = http.PostAsync(fullUrl, formData).Result;
                var respBody = httpResp.Content.ReadAsStringAsync().Result;

                return respBody;
            }
        }
    }
}
