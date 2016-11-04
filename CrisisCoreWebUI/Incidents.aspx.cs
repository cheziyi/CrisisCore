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
    /// <summary>
    /// Incidents web ui class for adding new incidents and display of unresolved incidents information.
    /// </summary>
    public partial class Incidents : System.Web.UI.Page
    {
        /// <summary>
        /// For sending HTTP requests and receving HTTP responses
        /// Initialize an IncidentType array to store all incident types
        /// </summary>
        static public HttpClient client;
        IncidentType[] incidentTypes;

        /// <summary>
        /// Set up page for inputs and displays.
        /// </summary>
        /// <param name="sender">Event data</param>
        /// <param name="e">Reference to control/object that raised the event</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            // Initialize an Account object from Session
            Account account = (Account)(Session["account"]);
            if (account != null)
            {
                if (account.AccessLevel < 2)
                {
                    // If account is not null, and has a access level
                    // of less than 2, redirect back to login page
                    Response.Redirect("~/Login.aspx");
                }
            }
            
            if (client == null)
            {
                // If client is not null, initialize new HttpClient object
                client = new HttpClient();
                client.BaseAddress = new Uri("http://api.crisiscore.cczy.io/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            }

            if (!IsPostBack)
            {
                // If page is loaded for the first time, set values into the dropdown list
                LoadIncidentTypes();
            }
            // Load unresolved incidents information in table display
            LoadIncidents();
        }

        /// <summary>
        /// Method to get incident types for dropdown list options.
        /// </summary>
        void LoadIncidentTypes()
        {
            // Clear any preloaded options
            ddlIncidenType.Items.Clear();

            // Initialize a HttpResponseMessage object to get response from server
            HttpResponseMessage response = client.GetAsync("IncidentType/GetIncidentTypes").Result;
            response.EnsureSuccessStatusCode();

            // Set into IncidentType array when there is a response
            incidentTypes = response.Content.ReadAsAsync<IncidentType[]>().Result;

            foreach (IncidentType type in incidentTypes)
            {
                // Add each incident type into dropdown list
                ddlIncidenType.Items.Add(new ListItem(type.IncidentTypeName, type.IncidentTypeId));
            }
        }

        /// <summary>
        /// Method to get all unresolved incidents and display in a table form.
        /// </summary>
        void LoadIncidents()
        {
            // Clear any preload content
            plcTable.Controls.Clear();

            // Initialize a HttpResponseMessage object to get response from server
            HttpResponseMessage response = client.GetAsync("Incident/GetUnresolvedIncidents").Result;
            response.EnsureSuccessStatusCode();

            // Initialize an Incident array to store all incidents
            Incident[] incidents = response.Content.ReadAsAsync<Incident[]>().Result;

            foreach (Incident incident in incidents)
            {
                // Add each incident information into table
                Literal lit1 = new Literal();
                lit1.Text = "<tr><td>" + ddlIncidenType.Items.FindByValue(incident.IncidentType.IncidentTypeId).Text +
                    "</td><td>" + incident.ReportName +
                    "</td><td>" + incident.ReportMobile +
                    "</td><td>" + incident.PostalCode +
                    "</td><td>" + incident.UnitNo +
                    "</td><td>" + incident.ReportDateTime +
                    "</td><td>";

                // Initialize a Button object for user to click when an incident has been resolved
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
            }
        }

        /// <summary>
        /// Method to handle on click event on the submit button.
        /// </summary>
        /// <param name="sender">Event data</param>
        /// <param name="e">Reference to control/object that raised the event</param>
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            // Initialize an Incident object and set user's input in accordingly
            Incident incident = new Incident();
            incident.IncidentType = new IncidentType();
            incident.IncidentType.IncidentTypeId = ddlIncidenType.SelectedValue;
            incident.PostalCode = txtPostal.Text;
            incident.ReportMobile = txtMobile.Text;
            incident.ReportName = txtName.Text;
            incident.UnitNo = txtUnitNo.Text;
            incident.AddInfo = txtAddInfo.Text;

            // Initialize a HttpResponseMessage object to get response from server
            HttpResponseMessage response = client.PostAsJsonAsync("Incident/AddIncident", incident).Result;
            response.EnsureSuccessStatusCode();
            // Initialize a Boolean object to ensure success of posting
            Boolean success = response.Content.ReadAsAsync<Boolean>().Result;

            // Clear textboxes of inputs
            txtPostal.Text = "";
            txtMobile.Text = "";
            txtName.Text = "";
            txtUnitNo.Text = "";
            txtAddInfo.Text = "";

            if (success)
            {
                // If posting is succesful, display success message
                litMessage.Text = "Incident added succesfully";
                // Load dropdown list for any updates
                LoadIncidentTypes();
                // Initialize an Agency object
                Agency agency = null;
                foreach (IncidentType type in incidentTypes)
                    if (type.IncidentTypeId.Equals(incident.IncidentType.IncidentTypeId))
                        // If an agency is found for that incident type, set agency in
                        agency = type.Agency;

                if (agency != null)
                    // When agency is not null, the controller would send an SMS to that respective agency
                    litMessage.Text += " and SMS sent to " + agency.AgencyName;

                litMessage.Text += ".";
                litStatus.Text = "Success!";
                successMsg.CssClass = "alert alert-success fade in";
                successMsg.Visible = true;
            }
            else
            {
                // If posting is not succesful, display error message
                litMessage.Text = "Failed to add incident.";
                litStatus.Text = "Failure.";
                successMsg.CssClass = "alert alert-danger fade in";
                successMsg.Visible = true;
            }
            // Load unresolved incidents table for any updates
            LoadIncidents();
        }

        /// <summary>
        /// Method to handle on click event on resolve button.
        /// </summary>
        /// <param name="sender">Event data</param>
        /// <param name="e">Reference to control/object that raised the event</param>
        protected void btnResolve_Click(object sender, EventArgs e)
        {
            // Initialize a Button object
            Button btnResolve = (Button)sender;
            int incidentId = Convert.ToInt32(btnResolve.ID.Replace("resolve_", ""));

            // Initialize a HttpResponseMessage object to get response from server
            HttpResponseMessage response = client.GetAsync("Incident/ResolveIncident?incidentId=" + incidentId).Result;
            // Initialize a Boolean object to ensure success of posting
            Boolean success = response.Content.ReadAsAsync<Boolean>().Result;

            if (success)
            {
                // If posting is succesful, display success message
                litMessage.Text = "Incident resolved succesfully.";
                litStatus.Text = "Success!";
                successMsg.CssClass = "alert alert-success fade in";
                successMsg.Visible = true;
            }
            else
            {
                // If posting is not succesful, display error message
                litMessage.Text = "Failed to resolve incident.";
                litStatus.Text = "Failure.";
                successMsg.CssClass = "alert alert-danger fade in";
                successMsg.Visible = true;
            }
            // Load unresolved incidents table for any updates
            LoadIncidents();
        }
    }
}