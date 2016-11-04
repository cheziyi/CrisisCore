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
        // Declare string variable for username and password
        string username = "cz3003crisiscore@gmail.com";
        string password = "b514vAIsjLal";

        /// <summary>
        /// Method to set up email and send
        /// </summary>
        /// <param name="emailAdd">Add Recipient email address</param>
        /// <param name="subject">Add Subject of the email</param>
        /// <param name="content">Add Content of the email</param>
        /// <returns></returns>
        [HttpPost]
        public bool SendEmail(string emailAdd, string subject, [FromBody] string content)
        {

            // Set up the sender and recipient email address with subject and the content  
            string from = "cz3003crisiscore@gmail.com";
            MailMessage message = new MailMessage(from, emailAdd);
            message.Subject = subject;
            message.Body = content;
            message.IsBodyHtml = true;

            // Set up the email setting
            SmtpClient client = new SmtpClient();
            client.Port = 587;
            client.Host = "smtp.gmail.com";
            client.Timeout = 10000;
            client.EnableSsl = true;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.Credentials = new NetworkCredential(username, password);

            try
            {
                // Send successfully return true with message
                client.Send(message);
                return true;
            }
            catch (Exception ex)
            {
                // Send successfully return false with error message
                Console.WriteLine(ex.Message);
                return false;
            }

        }
    }
}