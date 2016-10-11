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
    public partial class Incidents : System.Web.UI.Page
    {
        static public HttpClient client;
        IncidentType[] incidentTypes;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (client == null)
            {
                client = new HttpClient();
                client.BaseAddress = new Uri("http://api.crisiscore.cczy.io/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            }

            if (!IsPostBack)
            {
                LoadIncidentTypes();
            }
            LoadIncidents();
        }

        void LoadIncidentTypes()
        {
            ddlIncidenType.Items.Clear();

            HttpResponseMessage response = client.GetAsync("IncidentType/GetIncidentTypes").Result;
            response.EnsureSuccessStatusCode();

            incidentTypes = response.Content.ReadAsAsync<IncidentType[]>().Result;

            foreach (IncidentType type in incidentTypes)
            {
                ddlIncidenType.Items.Add(new ListItem(type.IncidentTypeName, type.IncidentTypeId));
            }
        }

        void LoadIncidents()
        {
            plcTable.Controls.Clear();

            HttpResponseMessage response = client.GetAsync("Incident/GetUnresolvedIncidents").Result;
            response.EnsureSuccessStatusCode();

            Incident[] incidents = response.Content.ReadAsAsync<Incident[]>().Result;

            foreach (Incident incident in incidents)
            {

                Literal lit1 = new Literal();
                lit1.Text = "<tr><td>" + ddlIncidenType.Items.FindByValue(incident.IncidentType.IncidentTypeId).Text +
                    "</td><td>" + incident.ReportName +
                    "</td><td>" + incident.ReportMobile +
                    "</td><td>" + incident.PostalCode +
                    "</td><td>" + incident.UnitNo +
                    "</td><td>" + incident.ReportDateTime +
                    // "</td><td><input type=\"checkbox\" value=\"" + incident.IncidentId + "\">" +
                    "</td></tr>";

                plcTable.Controls.Add(lit1);
                // ddlIncidenType.Items.Add(new ListItem(type.IncidentTypeName, type.IncidentTypeId));
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            Incident incident = new Incident();
            incident.IncidentType = new IncidentType();
            incident.IncidentType.IncidentTypeId = ddlIncidenType.SelectedValue;
            incident.PostalCode = txtPostal.Text;
            incident.ReportMobile = txtMobile.Text;
            incident.ReportName = txtName.Text;
            incident.UnitNo = txtUnitNo.Text;
            incident.AddInfo = txtAddInfo.Text;

            HttpResponseMessage response = client.PostAsJsonAsync("Incident/AddIncident", incident).Result;
            response.EnsureSuccessStatusCode();

            txtPostal.Text = "";
            txtMobile.Text = "";
            txtName.Text = "";
            txtUnitNo.Text = "";
            txtAddInfo.Text = "";

            LoadIncidents();
            LoadIncidentTypes();

            litMessage.Text = "Incident added succesfully";

            Agency agency = null;
            foreach (IncidentType type in incidentTypes)
                if (type.IncidentTypeId.Equals(incident.IncidentType.IncidentTypeId))
                    agency = type.Agency;

            if (agency != null)
                litMessage.Text += " and SMS sent to " + agency.AgencyName;

            litMessage.Text += ".";

            successMsg.Visible = true;
        }
    }
}