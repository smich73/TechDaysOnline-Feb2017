using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using DialogsDemo.Dialogs.Payment;
using DialogsDemo.Dialogs.Balance.Current;
using DialogsDemo.Dialogs.Balance.Savings;

namespace DialogsDemo.Dialogs.Balance
{
    [Serializable]
    public class CheckBalanceDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync($"Which account?{Environment.NewLine}- Current{Environment.NewLine}- Savings");

            context.Wait(MessageReceivedOperationChoice);
        }

        public async Task MessageReceivedOperationChoice(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var message = await argument;

            if (message.Text.ToLower().Equals("current", StringComparison.InvariantCultureIgnoreCase))
            {
                context.Call<object>(new CheckBalanceCurrentDialog(), AfterChildDialogIsDone);
            }
            else if (message.Text.ToLower().Equals("savings", StringComparison.InvariantCultureIgnoreCase))
            {
                context.Call<object>(new CheckBalanceSavingsDialog(), AfterChildDialogIsDone);
            }
            else
            {
                context.Wait(MessageReceivedOperationChoice);
            }
        }

        private async Task AfterChildDialogIsDone(IDialogContext context, IAwaitable<object> result)
        {
            context.Done<object>(new object());
        }
    }
}