using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace DialogsDemo.Dialogs.Balance.Current
{
    [Serializable]
    public class CheckBalanceCurrentDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync("...");
            context.Done<object>(new object());
        }
    }
}