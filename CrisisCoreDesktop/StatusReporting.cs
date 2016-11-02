using CrisisCoreModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CrisisCoreDesktop
{
    /// <summary>
    /// Desktop UI for starting status reporting to PMO via email every 30 mins.
    /// </summary>
    public partial class StatusReporting : Form
    {
        /// <summary>
        /// HttpClient for connecting to web service.
        /// </summary>
        static public HttpClient client;

        /// <summary>
        /// Timer to trigger report generation and email sending every 30 mins.
        /// </summary>
        static Timer timer = new Timer();

        /// <summary>
        /// Class constructor.
        /// </summary>
        public StatusReporting()
        {
            // Initialize UI
            InitializeComponent();
            // Initialize HttpClient
            client = new HttpClient();
            // Web service Uri and headers
            client.BaseAddress = new Uri("http://api.crisiscore.cczy.io/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // Clear status label
            lblStatus.Text = "";
        }

        /// <summary>
        /// Event handler for email button click.
        /// </summary>
        /// <param name="sender">Email button</param>
        /// <param name="e">Event arguments</param>
        private void btnEmail_Click(object sender, EventArgs e)
        {
            // If textbox is not disabled (not running)
            if (txtEmailAddress.Enabled)
            {
                // Disable textbox and change button text
                txtEmailAddress.Enabled = false;
                btnEmail.Text = "Stop Email Timer";

                // Do first initial generate and send report
                GenerateSendReport();

                // Create and start timer that ticks every 30 mins
                timer.Tick += new EventHandler(timer_Tick); 
                timer.Interval = (1000) * (60) * (30);            
                timer.Enabled = true;                       
                timer.Start();
            }
            // If textbox is disabled (running)
            else
            {
                // Enable textbox and change button text
                txtEmailAddress.Enabled = true;
                btnEmail.Text = "Start Email Timer";
                // Stop timer
                timer.Stop();
            }

        }

        /// <summary>
        /// Event handler for timer tick every 30 mins.
        /// </summary>
        /// <param name="sender">Timer</param>
        /// <param name="e">Event arguments</param>
        void timer_Tick(object sender, EventArgs e)
        {
            // Generate and send report
            GenerateSendReport();
        }

        /// <summary>
        /// Method to generate and send report to the email address in the email textbox.
        /// </summary>
        void GenerateSendReport()
        {
            // Generate and get report from web service
            HttpResponseMessage response = client.GetAsync("Report/GenerateReport").Result;
            response.EnsureSuccessStatusCode();
            String content = response.Content.ReadAsAsync<String>().Result;

            // Send email with report to email address, and check for success
            response = client.PostAsJsonAsync("Email/SendEmail?emailAdd=" + txtEmailAddress.Text + "&subject=Status Report Update",content).Result;
            response.EnsureSuccessStatusCode();
            bool success = response.Content.ReadAsAsync<bool>().Result;

            // Display success / failure message
            if (success)
            {
                lblStatus.Text = "Email last sent at " + DateTime.Now;
            }
            else
            {
                lblStatus.Text = "Email failed to sent at " + DateTime.Now;
            }
        }
    }
}
