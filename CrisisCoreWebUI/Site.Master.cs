using CrisisCoreModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CrisisCoreWebUI
{
    public partial class SiteMaster : MasterPage
    {
        static public Account account;

        protected void Page_Load(object sender, EventArgs e)
        {
            //account = (Account)(Session["account"]);

            //if (account == null)
            //{
            //    Response.Redirect("~/Login.aspx");
            //}
            //else
            //{
            //    litUser.Text = account.AccountId;
            //}


        }
    }
}