using QnAMakerDialog;
using System;
using Microsoft.Bot.Builder.Dialogs;
using System.Threading.Tasks;

namespace QnaMakerDemo.Dialogs
{
    [Serializable]
    [QnAMakerService("YOUR_SUBSCRIPTION_ID", "YOUR_KNOWLEDGE_BASE_ID")]
    public class QnADialog : QnAMakerDialog<object>
    {
        public override async Task NoMatchHandler(IDialogContext context, string originalQueryText)
        {
            await context.PostAsync("Sorry, I could't find an answer to your question.");
            context.Wait(MessageReceived);
        }
    }
}