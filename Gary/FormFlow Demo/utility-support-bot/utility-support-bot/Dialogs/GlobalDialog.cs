using UtilitySupportBot.Models;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using System;
using System.Threading.Tasks;
using System.Web.Configuration;
using Microsoft.Bot.Builder.Location;

namespace UtilitySupportBot.Dialogs
{
    [LuisModel("273e91e9-c0c3-481b-ab52-09cb4618a6dd", "bfa14f500a6c4f1b95b8e25a8d6dd95a")]
    [Serializable]
    public class GlobalDialog : LuisDialog<object>
    {
        [LuisIntent("ReportFault")]
        public async Task ReportFault(IDialogContext context, LuisResult result)
        {
            var faultForm = new FormDialog<FaultReport>(new FaultReport(),
                FaultReport.BuildForm, FormOptions.PromptInStart);

            context.Call(faultForm, AfterFaultReportForm);
        }

        [LuisIntent("CheckCurrentIncidents")]
        public async Task CheckCurrentIncidents(IDialogContext context, LuisResult result)
        {
            var apiKey = WebConfigurationManager.AppSettings["BingMapsApiKey"];
            var prompt = "To check for current incidents I will just need your post code.";
            var locationDialog = new LocationDialog(apiKey, context.Activity.ChannelId, prompt, LocationOptions.None, LocationRequiredFields.PostalCode);
            context.Call(locationDialog, AfterLocationCollected);
        }

        private async Task AfterLocationCollected(IDialogContext context, IAwaitable<Place> result)
        {
            Place place = await result;
            if (place != null)
            {
                var address = place.GetPostalAddress();
                string name = address != null ?
                    $"{address.StreetAddress}, {address.Locality}, {address.Region}, {address.Country} ({address.PostalCode})" :
                    "the pinned location";

                if (address.PostalCode.ToUpper().StartsWith("L3"))
                {
                    await context.PostAsync($"It looks like there is an on going incident at the moment which is affecting {name}.  Our engineers have reported that they have found the cause of the problem and expect to have it resolved within the next 2 - 4 hours.");
                    var dialog = new PromptDialog.PromptConfirm("Would you like me to update you when new information about this incident becomes available?", "Would you like me to update you when new information about this incident becomes available? You can say Yes or No", 3);
                    context.Call(dialog, AfterIncidentUpdateCheck);
                }
                else
                {
                    await context.PostAsync($"It looks like there are no incidents affecting {name} right now");
                    context.Wait(MessageReceived);
                }
            }
            else
            {
                await context.PostAsync("OK, cancelled");
                context.Wait(MessageReceived);
            }
        }

        [LuisIntent("FAQMeterFaulty")]
        public async Task MeterFaulty(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("If things are not working properly then there are a few things you can check. Here is our quick checklist for you to workthrough.");

            var message = context.MakeMessage();
            Attachment attachment1 = new Attachment();
            attachment1.ContentType = "application/pdf";
            attachment1.ContentUrl = "http://fastandfluid.com/publicdownloads/AngularJSIn60MinutesIsh_DanWahlin_May2013.pdf";
            message.Attachments.Add(attachment1);
            message.Text = "Energy Ltd. Quick Support Checklist";

            await context.PostAsync(message);

            context.Wait(MessageReceived);
        }

        private async Task AfterIncidentUpdateCheck(IDialogContext context, IAwaitable<bool> result)
        {
            var enableUpdates = await result;
            if (enableUpdates)
            {
                await context.PostAsync("No problem, I will check for updates to this incidents every 15 minutes and let you know here if there is any new information.");
                context.Wait(MessageReceived);
            }
            else
            {
                await context.PostAsync("Ok, if you change your mind then you can enable updates by searching for this incident again later.");
                context.Wait(MessageReceived);
            }
        }

        private async Task AfterFaultReportForm(IDialogContext context, IAwaitable<object> result)
        {
            context.Wait(MessageReceived);
        }

        [LuisIntent("")]
        public async Task None(IDialogContext context, IAwaitable<IMessageActivity> message, LuisResult result)
        {
            await context.PostAsync("Sorry, I could not find an answer to your question / query. You can send a contact message if you like?");
            context.Wait(MessageReceived);
        }
    }
}