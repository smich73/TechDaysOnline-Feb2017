using QnAMakerDialog;
using System;
using Microsoft.Bot.Builder.Dialogs;
using System.Threading.Tasks;

namespace QnaMakerDemo.Dialogs
{
    [Serializable]
    [QnAMakerService("8f833e867b25443b95d8c23cd367f7ce", "f7cf425a-2801-4854-bb24-75a4b33f9d2d")]
    public class QnADialog : QnAMakerDialog<object>
    {
        public override async Task NoMatchHandler(IDialogContext context, string originalQueryText)
        {
            await context.PostAsync("Sorry, I could't find an answer to your question.");
            context.Wait(MessageReceived);
        }
    }
}