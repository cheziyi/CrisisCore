using CrisisCoreModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CrisisCoreWebUI
{
    /// <summary>
    /// Login web ui class to prompt for user credentials to login.
    /// </summary>
    public partial class Login : System.Web.UI.Page
    {
        /// <summary>
        /// For sending HTTP requests and receiving HTTP responses
        /// </summary>
        static HttpClient client;

        /// <summary>
        /// Set up page for user to login.
        /// </summary>
        /// <param name="sender">Event data</param>
        /// <param name="e">Reference to control/object that raised the event</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            // Remove previous account information
            Session.Remove("account");

            if (client == null)
            {
                // If client is null, initialize new HttpClient object
                client = new HttpClient();
                client.BaseAddress = new Uri("http://api.crisiscore.cczy.io/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            }
        }

        /// <summary>
        /// Method to handle on click event on the submit button.
        /// </summary>
        /// <param name="sender">Event data</param>
        /// <param name="e">Reference to control/object that raised the event</param>
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            // Initialize an Account object and set user's input in accordingly
            Account account = new Account();
            account.AccountId = txtAccountId.Text;
            account.Password = txtPassword.Text;

            // Validation for the account information
            account = ValidateLogin(account);
            if (account != null)
            {
                // If account is not null, store account values in Session
                // Redirect user to next page
                Session["account"] = account;
                Response.Redirect("~/Default");
            }
            else
            {
                // If account is null, display error message and prompt for valid input
                litMessage.Text = "Please try again.";
                litStatus.Text = "Invalid username/password.";
                successMsg.CssClass = "alert alert-danger fade in";
                successMsg.Visible = true;
                Session.Remove("account");

            }
        }
        
        /// <summary>
        /// Method to validate login credentials.
        /// </summary>
        /// <param name="account">An Account object that store user's credentials</param>
        /// <returns>An Account object that is valid and with level of access permission</returns>
        static Account ValidateLogin(Account account)
        {
            // Initialize a HttpResponseMessage object to get response from server
            HttpResponseMessage response = client.PostAsJsonAsync("Account/Login", account).Result;
            response.EnsureSuccessStatusCode();

            // Set into Account object when there is a response
            account = response.Content.ReadAsAsync<Account>().Result;
            // Return Account object
            return account;
        }
    }
}