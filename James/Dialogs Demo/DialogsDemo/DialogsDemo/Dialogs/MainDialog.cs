using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using DialogsDemo.Dialogs.Balance;
using DialogsDemo.Dialogs.Payment;

namespace DialogsDemo.Dialogs
{
    [Serializable]
    public class MainDialog : IDialog<object>
    {

        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedStart);
        }
        public async Task MessageReceivedStart(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            await context.PostAsync($"Would you like to:{Environment.NewLine}- Check balance{Environment.NewLine}- Make payment");

            context.Wait(MessageReceivedOperationChoice);
        }


        public async Task MessageReceivedOperationChoice(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var message = await argument;

            if (message.Text.ToLower().Equals("check balance", StringComparison.InvariantCultureIgnoreCase))
            {
                context.Call<object>(new CheckBalanceDialog(), AfterChildDialogIsDone);
            }
            else if (message.Text.ToLower().Equals("make payment", StringComparison.InvariantCultureIgnoreCase))
            {
                context.Call<object>(new MakePaymentDialog(), AfterChildDialogIsDone);
            }
            else
            {
                context.Wait(MessageReceivedStart);
            }
        }

        private async Task AfterChildDialogIsDone(IDialogContext context, IAwaitable<object> result)
        {
            await context.PostAsync($"Anything else I can help with?:{Environment.NewLine}- Check balance{Environment.NewLine}- Make payment");

            context.Wait(MessageReceivedOperationChoice);
        }
    }
}