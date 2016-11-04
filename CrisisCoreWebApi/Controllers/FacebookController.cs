using Facebook;    //for Facebook Graph API, that allows app to read/login/write to Facebook on user's behalf 
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
       //declare user access token which provides CrisisCore facebook app permission to post
       string accessToken = "EAAUPbKRZCyPUBAGyMkowlpPkiQiuAUHPHboE5CAhhgDlpiiKrEjI3WgMgzf1mqOgl9pwBsyK8t4og3VRjbJbP9JafhMfUZAaO1JDZB6kOGG0TtZA0nZAUqH0nMTwFubg6BbWwBjsZBRyZCotbvoZBtgg6YfAfEjsxjJzYL6DqJLYc79Rwf93lLW1";
       
       //facebook page id for CrisisCore app
       string pageId = "1707287282924587";

        [HttpPost]
        public string PostToPage([FromBody] string update)
        {
            try
            {
                //delare instance of FacebookClient class for making requests to the Graph API through user access token
                var fb = new FacebookClient(accessToken);
                
                //posts message string to facebook
                dynamic result = fb.Post(pageId + "/feed", new { message = update });             
                
                //check for success message posted on facebook
                return result.id;
            }
            catch (Exception) { }

            return "";
        }
    }
}
