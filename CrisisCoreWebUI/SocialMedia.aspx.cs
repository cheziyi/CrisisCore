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
    /// SocialMedia web ui class for posting to social media.
    /// </summary>
    public partial class SocialMedia : System.Web.UI.Page
    {
        /// <summary>
        /// For sending HTTP requests and receiving HTTP responses
        /// </summary>
        static public HttpClient client;

        /// <summary>
        /// Set up page for user to input message content.
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
                // If page is loaded for the first time, set values into the list of areas
                LoadAreas();
            }

        }

        /// <summary>
        /// Method to get areas for the list options.
        /// </summary>
        void LoadAreas()
        {
            // Clear any preloaded options
            lstAreas.Items.Clear();

            // Initialize a HttpResponseMessage object to get response from server
            HttpResponseMessage response = client.GetAsync("Area/GetAllAreas").Result;
            response.EnsureSuccessStatusCode();

            // Initialize an Area array, and set response values into it
            Area[] areas = response.Content.ReadAsAsync<Area[]>().Result;

            foreach (Area area in areas)
            {
                // Add each area into the list
                lstAreas.Items.Add(new ListItem(area.AreaName, area.AreaId));
            }
        }

        /// <summary>
        /// Method to get incident types in an area for list options.
        /// </summary>
        void LoadEmergencies()
        {
            // Clear any preloaded options
            lstEmergencies.Items.Clear();

            // Initialize a HttpResponseMessage object to get response from server
            HttpResponseMessage response = client.GetAsync("IncidentType/GetIncidentTypesInArea?areaId=" + lstAreas.SelectedValue).Result;
            response.EnsureSuccessStatusCode();

            // Initialize an IncidentType array, and set response values into it
            IncidentType[] incidentTypes = response.Content.ReadAsAsync<IncidentType[]>().Result;

            foreach (IncidentType type in incidentTypes)
            {
                // Add each incident type into the list
                lstEmergencies.Items.Add(new ListItem(type.IncidentTypeName + " (" + type.Severity + ")", type.IncidentTypeId));
            }
        }

        /// <summary>
        /// Method to detect changes on the selected area and display incident type accordingly.
        /// </summary>
        /// <param name="sender">Event data</param>
        /// <param name="e">Reference to control/object that raised the event</param>
        protected void lstAreas_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadEmergencies();
        }

        /// <summary>
        /// Method to detect selection on incident type to display default message content.
        /// </summary>
        /// <param name="sender">Event data</param>
        /// <param name="e">Reference to control/object that raised the event</param>
        protected void lstEmergencies_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Form message content according to selected area and incident type
            txtMessage.Text = "Dear " + lstAreas.SelectedItem.Text + " residents, there is a high occurrence of " + lstEmergencies.SelectedItem.Text + " incidents in your area. ";
            if (lstEmergencies.SelectedValue.Equals("HAZE"))
            {
                txtMessage.Text = "Dear " + lstAreas.SelectedItem.Text + " residents, the Haze PSI level in your area has reached " + lstEmergencies.SelectedItem.Text + ". ";
                txtMessage.Text += "Please stay indoors as much as possible, and if necessary, wear a 3M mask.";
            }
            else if (lstEmergencies.SelectedValue.Equals("DENGUE") || lstEmergencies.SelectedValue.Equals("ZIKA"))
            {
                txtMessage.Text += "Please make sure that you do not have any stagnant water as potential breeding grounds for mosquitoes.";
            }

            else
            {
                txtMessage.Text += "Assistance required for evacuation to nearest shelters. List of shelters available here: https://www.scdf.gov.sg/content/scdf_internet/en/building-professionals/cd-shelter/public-shelters.html";
            }
        }

        /// <summary>
        /// Method to handle on click event on the submit button.
        /// </summary>
        /// <param name="sender">Event data</param>
        /// <param name="e">Reference to control/object that raised the event</param>
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            // Initialize a HttpResponseMessage object to get response from server
            // Posting of message to Twitter
            HttpResponseMessage response = client.PostAsJsonAsync("Twitter/Tweet", txtMessage.Text.Split('.')[0] + '.').Result;
            response.EnsureSuccessStatusCode();

            // Posting of message to Facebook
            response = client.PostAsJsonAsync("Facebook/PostToPage", txtMessage.Text).Result;
            response.EnsureSuccessStatusCode();

            // Display success message
            litMessage.Text = "Social media platforms updated succesfully.";
            successMsg.Visible = true;
        }
    }
}