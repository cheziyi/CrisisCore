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
    public partial class SocialMedia : System.Web.UI.Page
    {
        static public HttpClient client;

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
                LoadAreas();
            }

        }

        void LoadAreas()
        {
            lstAreas.Items.Clear();

            HttpResponseMessage response = client.GetAsync("Area/GetAllAreas").Result;
            response.EnsureSuccessStatusCode();

            Area[] areas = response.Content.ReadAsAsync<Area[]>().Result;

            foreach (Area area in areas)
            {
                lstAreas.Items.Add(new ListItem(area.AreaName, area.AreaId));
            }
        }

        void LoadEmergencies()
        {
            lstEmergencies.Items.Clear();

            HttpResponseMessage response = client.GetAsync("IncidentType/GetIncidentTypesInArea?areaId=" + lstAreas.SelectedValue).Result;
            response.EnsureSuccessStatusCode();

            IncidentType[] incidentTypes = response.Content.ReadAsAsync<IncidentType[]>().Result;

            foreach (IncidentType type in incidentTypes)
            {
                lstEmergencies.Items.Add(new ListItem(type.IncidentTypeName + " (" + type.Severity + ")", type.IncidentTypeId));
            }
        }

        protected void lstAreas_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadEmergencies();
        }

        protected void lstEmergencies_SelectedIndexChanged(object sender, EventArgs e)
        {
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

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            HttpResponseMessage response = client.GetAsync("Twitter/Tweet?status=" + txtMessage.Text.Split('.')[0] + '.').Result;
            response.EnsureSuccessStatusCode();

            response = client.GetAsync("Facebook/PostToPage?update=" + txtMessage.Text).Result;
            response.EnsureSuccessStatusCode();

            litMessage.Text = "Social media platforms updated succesfully.";

            successMsg.Visible = true;
        }
    }
}