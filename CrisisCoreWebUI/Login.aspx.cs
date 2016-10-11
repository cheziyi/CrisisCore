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
    public partial class Login : System.Web.UI.Page
    {
        static HttpClient client;

        protected void Page_Load(object sender, EventArgs e)
        {
            Session.Remove("account");

            if (client == null)
            {
                client = new HttpClient();
                client.BaseAddress = new Uri("http://api.crisiscore.cczy.io/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            Account account = new Account();
            account.AccountId = txtAccountId.Text;
            account.Password = txtPassword.Text;

            account = ValidateLogin(account);
            if (account != null)
            {
                Session["account"] = account;
                Response.Redirect("~/Default");
            }
            else
            {
                Session.Remove("account");
            }

            
            // Return the URI of the created resource.
            //   return response.Headers.Location;
        }

        static Account ValidateLogin(Account account)
        {
            HttpResponseMessage response = client.PostAsJsonAsync("Account/Login", account).Result;
            response.EnsureSuccessStatusCode();

            account = response.Content.ReadAsAsync<Account>().Result;
            return account;
        }
    }
}