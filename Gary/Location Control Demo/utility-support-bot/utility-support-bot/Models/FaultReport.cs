using Microsoft.Bot.Builder.FormFlow;
using System;
using Microsoft.Bot.Builder.Dialogs;
using System.Threading.Tasks;

namespace UtilitySupportBot.Models
{
    [Serializable]
    public class FaultReport
    {
        public NeighboursOrStreetLightsAffectedStatus NeighboursOrStreetLightsAffected { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        public string PostCode { get; set; }

        public string ContactNumber { get; set; }

        public string Email { get; set; }

        public ContactMethod PreferredContactMethod { get; set; }

        public bool ProvideAdditionalInformation { get; set; }

        public string AdditionalInformation { get; set; }

        public static IForm<FaultReport> BuildForm()
        {
            return new FormBuilder<FaultReport>().AddRemainingFields().Build();
        }
    }

    public enum NeighboursOrStreetLightsAffectedStatus
    {
        IGNORE,
        Yes,
        No,
        Maybe
    }

    public enum ContactMethod
    {
        IGNORE,
        Telephone,
        SMS,
        Email
    }
}