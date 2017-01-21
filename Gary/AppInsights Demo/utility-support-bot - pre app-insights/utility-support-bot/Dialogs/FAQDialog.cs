using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace EnwAssistBot.Dialogs
{
    [LuisModel("18950797-4eab-44e2-8da5-c4bc055ba36c", "bfa14f500a6c4f1b95b8e25a8d6dd95a")]
    [Serializable]
    public class FAQDialog : LuisDialog<object>
    {
        [LuisIntent("")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            context.Done<bool>(false);
        }

        [LuisIntent("MeterFaulty")]
        public async Task MeterFaulty(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("You need to contact your supplier. The number is found on your electricity bill.");
            context.Done<bool>(true);
        }
    }
}