using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;

namespace DialogsDemo.Dialogs.Balance.Savings
{
    [Serializable]
    public class CheckBalanceSavingsDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync("...");
            context.Done<object>(new object());
        }
    }
}