using Facebook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CrisisCoreWebApi.Controllers
{
    public class FacebookController : ApiController
    {
        string accessToken = "EAAUPbKRZCyPUBAGyMkowlpPkiQiuAUHPHboE5CAhhgDlpiiKrEjI3WgMgzf1mqOgl9pwBsyK8t4og3VRjbJbP9JafhMfUZAaO1JDZB6kOGG0TtZA0nZAUqH0nMTwFubg6BbWwBjsZBRyZCotbvoZBtgg6YfAfEjsxjJzYL6DqJLYc79Rwf93lLW1";
        string pageId = "1707287282924587";

        [HttpGet]
        public string PostToPage(string update)
        {
            try
            {
                var fb = new FacebookClient(accessToken);
                dynamic result = fb.Post(pageId + "/feed", new { message = update });
                return result.id;
            }
            catch (Exception) { }

            return "";
        }
    }
}
