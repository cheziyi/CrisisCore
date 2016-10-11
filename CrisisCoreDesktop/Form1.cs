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

        //Timer item
        public Form1()
        {
            InitializeComponent();
            client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:2221/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        }

        private void btnEmail_Click(object sender, EventArgs e)
        {
            //timer start
        }


        //timer-tick
        void Timertick ()
        {
            HttpResponseMessage response = client.GetAsync("Report/GenerateRport").Result;
            response.EnsureSuccessStatusCode();

            String content = response.Content.ReadAsAsync<String>().Result;


            response = client.GetAsync("Email/SendEmail?content=" + content + "&emailAdd=calvin@cczy.io").Result;
            response.EnsureSuccessStatusCode();

            bool success = response.Content.ReadAsAsync<bool>().Result;

        }

    }
}
