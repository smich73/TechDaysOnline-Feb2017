using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using Microsoft.Bot.Builder.Dialogs;
using UtilitySupportBot.Dialogs;
using Microsoft.ApplicationInsights;
using System.Collections.Generic;

namespace UtilitySupportBot
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity.Type == ActivityTypes.Message)
            {
                try
                {
                    await SendTypingActivity(activity);
                    await Conversation.SendAsync(activity, MakeRoot);
                }
                catch (Exception ex)
                {

                }
            }
            else
            {
                HandleSystemMessage(activity);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        internal static IDialog<object> MakeRoot()
        {
            return Chain.From(() => new GlobalDialog());
        }

        private static async Task SendTypingActivity(Activity activity)
        {
            var connector = new ConnectorClient(new Uri(activity.ServiceUrl));
            Activity isTypingReply = activity.CreateReply();
            isTypingReply.Type = ActivityTypes.Typing;
            await connector.Conversations.ReplyToActivityAsync(isTypingReply);
        }

        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels

                // check if bot has been added to the conversation
                if(message.MembersAdded.Where(m => m.Id == message.Recipient.Id).Any())
                {
                    var connector = new ConnectorClient(new Uri(message.ServiceUrl));
                    var replyMessage = message.CreateReply("Hi! I am the Energy Ltd Support Bot. I can help you do things like report a power cut, find your supplier, check current faults or get answers to common questions. If you need anything then just ask!");
                    connector.Conversations.ReplyToActivityAsync(replyMessage);
                }
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }
    }
}