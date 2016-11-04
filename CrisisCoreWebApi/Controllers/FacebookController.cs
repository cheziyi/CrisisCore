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
       //declare access token
       string accessToken = "EAAUPbKRZCyPUBAGyMkowlpPkiQiuAUHPHboE5CAhhgDlpiiKrEjI3WgMgzf1mqOgl9pwBsyK8t4og3VRjbJbP9JafhMfUZAaO1JDZB6kOGG0TtZA0nZAUqH0nMTwFubg6BbWwBjsZBRyZCotbvoZBtgg6YfAfEjsxjJzYL6DqJLYc79Rwf93lLW1";
       //declare page id
       string pageId = "1707287282924587";

        [HttpPost]
        public string PostToPage([FromBody] string update)
        {
            try
            {
            //sets the ability to pass in an access token
                var fb = new FacebookClient(accessToken);
            //post user's message to facebook
                dynamic result = fb.Post(pageId + "/feed", new { message = update });
            //returns the result's id
                return result.id;
            }
            catch (Exception) { }

            return "";
        }
    }
}
