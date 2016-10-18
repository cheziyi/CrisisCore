using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Web.Http;

namespace CrisisCoreWebApi.Controllers
{
    public class EmailController : ApiController
    {
        string username = "cz3003crisiscore@gmail.com";
        string password = "b514vAIsjLal";

        [HttpPost]
        public bool SendEmail(string emailAdd, string subject, [FromBody] string content)
        {
            string from = "cz3003crisiscore@gmail.com";
            MailMessage message = new MailMessage(from, emailAdd);
            message.Subject = subject;
            message.Body = content;
            message.IsBodyHtml = true;

            SmtpClient client = new SmtpClient();
            client.Port = 587;
            client.Host = "smtp.gmail.com";
            client.Timeout = 10000;
            client.EnableSsl = true;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.Credentials = new NetworkCredential(username, password);

            try
            {
                client.Send(message);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }

        }
    }
}