using Microsoft.Bot.Builder.Location;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UtilitySupportBot.Dialogs
{
    [Serializable]
    public class LocationDialogResourceManager : LocationResourceManager
    {
        public override string ConfirmationAsk
        {
            get
            {
                return "Thanks. I will use this address to check for current incidents. Is that ok?";
            }
        }

        public override string TitleSuffix
        {
            get
            {
                return "Just enter your post code (or address if you don't know it and I can try and find it for you)";
            }
        }

        public override string PostalCode
        {
            get
            {
                return "post code";
            }
        }
    }
}