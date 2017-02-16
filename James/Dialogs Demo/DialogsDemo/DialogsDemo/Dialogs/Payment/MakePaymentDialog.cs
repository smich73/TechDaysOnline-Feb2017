using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Threading.Tasks;

namespace DialogsDemo.Dialogs.Payment
{
    [Serializable]
    public class MakePaymentDialog : IDialog<object>
    {
        protected string payee;
        protected string amount;

        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync($"Who would you like to pay?");

            context.Wait(MessageReceivedPayee);
        }
        
        public async Task MessageReceivedPayee(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var message = await argument;
            this.payee = message.Text;

            await context.PostAsync($"{this.payee}, got it{Environment.NewLine}How much should I pay?");

            context.Wait(MessageReceivedAmount);
        }

        public async Task MessageReceivedAmount(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var message = await argument;
            this.amount = message.Text;

            await context.PostAsync($"Thank you, I've paid {this.amount} to {this.payee}");

            context.Done<object>(new object());
        }
    }
}