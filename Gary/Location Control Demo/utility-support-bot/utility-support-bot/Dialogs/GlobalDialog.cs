using UtilitySupportBot.Models;
using UtilitySupportBot.OutageServiceReference;
using Microsoft.ApplicationInsights;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Configuration;
using Microsoft.Bot.Builder.Location;

namespace UtilitySupportBot.Dialogs
{
    [LuisModel("273e91e9-c0c3-481b-ab52-09cb4618a6dd", "bfa14f500a6c4f1b95b8e25a8d6dd95a")]
    [Serializable]
    public class GlobalDialog : LuisDialog<object>
    {
        [NonSerialized()]
        private TelemetryClient telemetry = new TelemetryClient();

        [LuisIntent("")]
        public async Task None(IDialogContext context, IAwaitable<IMessageActivity> message, LuisResult result)
        {
            LogIntentTelemetry("None", 0);

            await context.PostAsync("Sorry, I could not find an answer to your question / query. You can send a contact message if you like?");
            context.Wait(MessageReceived);
        }

        [LuisIntent("ReportFault")]
        public async Task ReportFault(IDialogContext context, LuisResult result)
        {
            LogIntentTelemetry("ReportFault", result.Intents.First().Score.Value);

            await context.PostAsync("It looks like you want to report a fault. I can help with that, but if you need to stop at any point then just type 'cancel'.");

            var dialog = new PromptDialog.PromptConfirm("Firstly, can I just check if you are reporting a dangerous situation, a risk to life or damage to our equipment?", "I just need to know if you are reporting a dangerous situation, a risk to life or damage to our equipment? You can say Yes or No", 3);
            context.Call(dialog, AfterCriticalFaultReportCheck);
        }

        [LuisIntent("FindSupplier")]
        public async Task FindSupplier(IDialogContext context, LuisResult result)
        {
            LogIntentTelemetry("FindSupplier", result.Intents.First().Score.Value);

            var apiKey = WebConfigurationManager.AppSettings["BingMapsApiKey"];
            var prompt = "I can look up your supplier for you. I just need your address.";
            var locationDialog = new LocationDialog(apiKey, context.Activity.ChannelId, prompt, LocationOptions.None, LocationRequiredFields.StreetAddress | LocationRequiredFields.PostalCode);
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
                await context.PostAsync($"Based on your address being, {name}, it looks like your supplier is Scottish Power.");
            }
            else
            {
                await context.PostAsync("OK, cancelled");
            }

            context.Wait(MessageReceived);
        }

        [LuisIntent("CheckCurrentIncidents")]
        public async Task CheckCurrentIncidents(IDialogContext context, LuisResult result)
        {
            LogIntentTelemetry("CheckCurrentIncidents", result.Intents.First().Score.Value);

            await context.PostAsync("You want to check current incidents");
            context.Wait(MessageReceived);
        }

        [LuisIntent("FAQMeterFaulty")]
        public async Task MeterFaulty(IDialogContext context, LuisResult result)
        {
            LogIntentTelemetry("FAQMeterFaulty", result.Intents.First().Score.Value);

            await context.PostAsync("You need to contact your supplier. The number is found on your electricity bill.");
            context.Wait(MessageReceived);
        }

        private async Task AfterCriticalFaultReportCheck(IDialogContext context, IAwaitable<bool> result)
        {
            var dangerousFault = await result;
            if (dangerousFault)
            {
                await context.PostAsync("Please call us on the number below to report this fault 0800 195 4141. Doing this will mean we can get help to you as quickly as possible.");
                context.Wait(MessageReceived);
            }
            else
            {
                var dialog = new PromptDialog.PromptString("Please start by entering your full post code I can check for any current incidents that might be affecting you.", "Please enter your full post code so that I can check for any current incidents.", 3);
                context.Call(dialog, AfterCollectPostCodeAsync);
            }
        }

        private async Task AfterCollectPostCodeAsync(IDialogContext context, IAwaitable<string> result)
        {
            var postCode = await result;

            await context.PostAsync("Thanks. I will just check for any current incidents affecting your post code.");

            OutageServiceSoapClient outageServiceClient = new OutageServiceSoapClient();
            var response = await outageServiceClient.GetOutageInformationAsync(true, false, false, 1, 800);
            var outagesResponse = JsonConvert.DeserializeObject<OutageDetail[]>(response);
            var outagesForPostCode = outagesResponse.Where(o => o.FullPostcode.Replace(" ", "").ToLower().Contains(postCode.Replace(" ", "").ToLower())).ToList();

            if (outagesForPostCode != null && outagesForPostCode.Any())
            {
                // show details of the current incident and offer regular updates
                await context.PostAsync(string.Format("I have found a current incident for {0}.", postCode));
                await context.PostAsync(string.Format("{0}", outagesForPostCode.First().CustomerInformation));
                var dialog = new PromptDialog.PromptConfirm("Would you like me to update you when new information about this incident becomes available?", "Would you like me to update you when new information about this incident becomes available? You can say Yes or No", 3);
                context.Call(dialog, AfterIncidentUpdateCheck);
            }
            else
            {
                // start the form flow for raising a case
                var faultForm = new FormDialog<FaultReport>(new FaultReport(), FaultReport.BuildForm, FormOptions.PromptInStart);
                context.Call(faultForm, AfterFaultReportForm);
            }
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

        public void LogIntentTelemetry(string intentName, double luisScore)
        {
            telemetry = new TelemetryClient();

            var properties = new Dictionary<string, string>
            { {"Intent", intentName}};

            var metrics = new Dictionary<string, double>
            { {"LuisScore", luisScore}};

            telemetry.TrackEvent("LuisResponseReceived", properties, metrics);
        }
    }
}