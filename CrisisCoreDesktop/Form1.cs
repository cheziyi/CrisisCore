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
    public partial class Form1 : Form
    {
        static public HttpClient client;
        static Timer timer = new Timer();

        public Form1()
        {
            InitializeComponent();
            client = new HttpClient();
            client.BaseAddress = new Uri("http://api.crisiscore.cczy.io/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            lblStatus.Text = "";
        }

        private void btnEmail_Click(object sender, EventArgs e)
        {
            if (txtEmailAddress.Enabled)
            {
                txtEmailAddress.Enabled = false;
                btnEmail.Text = "Stop Email Timer";

                GenerateSendReport();

                timer.Tick += new EventHandler(timer_Tick); 
                timer.Interval = (1000) * (60) * (30);            
                timer.Enabled = true;                       
                timer.Start();
            }
            else
            {
                txtEmailAddress.Enabled = true;
                btnEmail.Text = "Start Email Timer";
                timer.Stop();
            }

        }


        void timer_Tick(object sender, EventArgs e)
        {
            GenerateSendReport();
        }

        void GenerateSendReport()
        {
            HttpResponseMessage response = client.GetAsync("Report/GenerateReport").Result;
            response.EnsureSuccessStatusCode();

            String content = response.Content.ReadAsAsync<String>().Result;

            response = client.PostAsJsonAsync("Email/SendEmail?emailAdd=" + txtEmailAddress.Text + "&subject=Status Report Update",content).Result;
            response.EnsureSuccessStatusCode();

            bool success = response.Content.ReadAsAsync<bool>().Result;

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
