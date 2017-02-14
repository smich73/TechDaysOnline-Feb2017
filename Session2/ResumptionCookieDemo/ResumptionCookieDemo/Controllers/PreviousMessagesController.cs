using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace ResumptionCookieDemo.Controllers
{
    public class PreviousMessagesController : ApiController
    {
        public string Get()
        {
            var filepath = System.Web.Hosting.HostingEnvironment.MapPath("~/lastmessage.json");

            if (File.Exists(filepath))
            {
                return File.ReadAllText(filepath);
            }
            else
            {
                return string.Empty;
            }
        }
    }
}
