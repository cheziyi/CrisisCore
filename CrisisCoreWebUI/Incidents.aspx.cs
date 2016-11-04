﻿using CrisisCoreModels;
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
            Account account = (Account)(Session["account"]);
            if (account != null)
            {
                if (account.AccessLevel < 2)
                {
                    Response.Redirect("~/Login.aspx");
                }
            }


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
                    "</td><td>";
                Button btnResolve = new Button();
                btnResolve.Text = "Resolve";
                btnResolve.ID = "resolve_" + incident.IncidentId.ToString();
                btnResolve.ControlStyle.CssClass = "btn btn-success btn-sm";
                btnResolve.Attributes.Add("formnovalidate", "formnovalidate");
                btnResolve.Click += new EventHandler(btnResolve_Click);
                btnResolve.OnClientClick = "if ( ! ResolveIncidentConfirmation()) return false;";

                Literal lit2 = new Literal();
                lit2.Text = "</td></tr>";
                plcTable.Controls.Add(lit1);
                plcTable.Controls.Add(btnResolve);
                plcTable.Controls.Add(lit2);
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
            Boolean success = response.Content.ReadAsAsync<Boolean>().Result;

            txtPostal.Text = "";
            txtMobile.Text = "";
            txtName.Text = "";
            txtUnitNo.Text = "";
            txtAddInfo.Text = "";
            if (success)
            {
                litMessage.Text = "Incident added succesfully";
                LoadIncidentTypes();
                Agency agency = null;
                foreach (IncidentType type in incidentTypes)
                    if (type.IncidentTypeId.Equals(incident.IncidentType.IncidentTypeId))
                        agency = type.Agency;

                if (agency != null)
                    litMessage.Text += " and SMS sent to " + agency.AgencyName;

                litMessage.Text += ".";
                litStatus.Text = "Success!";
                successMsg.CssClass = "alert alert-success fade in";
                successMsg.Visible = true;
            }
            else
            {
                litMessage.Text = "Failed to add incident.";
                litStatus.Text = "Failure.";
                successMsg.CssClass = "alert alert-danger fade in";
                successMsg.Visible = true;
            }
            LoadIncidents();
        }

        protected void btnResolve_Click(object sender, EventArgs e)
        {
            Button btnResolve = (Button)sender;
            int incidentId = Convert.ToInt32(btnResolve.ID.Replace("resolve_", ""));

            HttpResponseMessage response = client.GetAsync("Incident/ResolveIncident?incidentId=" + incidentId).Result;
            Boolean success = response.Content.ReadAsAsync<Boolean>().Result;

            if (success)
            {
                litMessage.Text = "Incident resolved succesfully.";
                litStatus.Text = "Success!";
                successMsg.CssClass = "alert alert-success fade in";
                successMsg.Visible = true;
            }
            else
            {
                litMessage.Text = "Failed to resolve incident.";
                litStatus.Text = "Failure.";
                successMsg.CssClass = "alert alert-danger fade in";
                successMsg.Visible = true;
            }
            LoadIncidents();
        }
    }
}