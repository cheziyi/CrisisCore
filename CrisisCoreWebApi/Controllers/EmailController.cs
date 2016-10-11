using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CrisisCoreWebApi.Controllers
{
    public class EmailController : ApiController
    {
        [HttpGet]
        public bool SendEmail(string content, string emailAdd)
        {
            //send email and return success

            return true;
        }
    }
}