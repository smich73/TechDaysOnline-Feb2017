using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace ResumptionCookieDemo.Controllers
{
    // Credit: Robin Osborne
    // http://robinosborne.co.uk/2017/01/02/sending-proactive-botframework-messages
    public class SimulateController : ApiController
    {
        // This HTTP Endpoint simulates a long running Proactive callback.  Use a HTTP client to simulate:
        // 
        // POST http://localhost:3979/api/simulate
        // Content-Type: application/json
        // 
        // { "Text" :"this is a proactive message!" }
        //
        // in reality, this code may be initiated by an event, a long running background task, or a schedule
        public async Task<HttpResponseMessage> Post([FromBody] ProactiveMessage message)
        {
            // For demonstration - read the cookie from disk.  For a real application
            // read from your persistent store - e.g. blob storage, table storage, document db, etc
            var resumeJson = File.ReadAllText(System.Web.Hosting.HostingEnvironment.MapPath("~/cookie.json"));

            dynamic resumeData = JsonConvert.DeserializeObject(resumeJson);
            string botId = resumeData.botId;
            string channelId = resumeData.channelId;
            string userId = resumeData.userId;
            string conversationId = resumeData.conversationId;
            string serviceUrl = resumeData.serviceUrl;
            string userName = resumeData.userName;
            bool isGroup = resumeData.isGroup;
            var resume = new ResumptionCookie(userId, botId, conversationId, channelId, serviceUrl, "en");

            var messageactivity = (Activity)resume.GetMessage();
            var reply = messageactivity.CreateReply();
            reply.Text = $"Proactive message: {message.Text}";
            var client = new ConnectorClient(new Uri(messageactivity.ServiceUrl));
            await client.Conversations.ReplyToActivityAsync(reply);

            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }
    }

    public class ProactiveMessage
    {
        public string Text { get; set; }
    }
}
