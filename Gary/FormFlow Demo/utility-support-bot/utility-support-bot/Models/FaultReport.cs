using Microsoft.Bot.Builder.FormFlow;
using System;
using Microsoft.Bot.Builder.Dialogs;
using System.Threading.Tasks;

namespace UtilitySupportBot.Models
{
    [Serializable]
    public class FaultReport
    {
                [Describe(Title = "Are your neighbours or street lights affected?", SubTitle = "")]
        [Prompt("{||}", ChoiceStyle = ChoiceStyleOptions.Buttons)]
        public NeighboursOrStreetLightsAffectedStatus NeighboursOrStreetLightsAffected { get; set; }

        [Prompt("What's your full name?")]
        public string Name { get; set; }

        [Prompt("What is the house name / number and street where you are having the problem?")]
        public string Address { get; set; }

        [Prompt("What is the full post code for the affected address?")]
        public string PostCode { get; set; }

        [Prompt("Can you give me the best number to reach you on?")]
        public string ContactNumber { get; set; }

        [Prompt("What is your email address?")]
        public string Email { get; set; }

        [Describe(Title = "What is the best way to get in touch with you?", SubTitle = "")]
        [Prompt("{||}", ChoiceStyle = ChoiceStyleOptions.Buttons)]
        public ContactMethod PreferredContactMethod { get; set; }

        [Describe(Title = "Additional Information?", SubTitle = "Is there anything else you would like to tell us about the fault?")]
        [Prompt("{||}", ChoiceStyle = ChoiceStyleOptions.Buttons)]
        public bool ProvideAdditionalInformation { get; set; }

        [Prompt("Please provide the additional information")]
        public string AdditionalInformation { get; set; }

        public static IForm<FaultReport> BuildForm()
        {
            return new FormBuilder<FaultReport>()
                    .Message("Looks like you want to report a fault. Let's start a case for you. To do that I just need to take a few details.")
                    .Field(nameof(NeighboursOrStreetLightsAffected))
                    .Field(nameof(Name))
                    .Field(nameof(Address))
                    .Field(nameof(PostCode))
                    .Field(nameof(PreferredContactMethod))
                    .Field(nameof(ContactNumber), (s) => s.PreferredContactMethod == ContactMethod.SMS || s.PreferredContactMethod == ContactMethod.Telephone)
                    .Field(nameof(Email), (s) => s.PreferredContactMethod == ContactMethod.Email)
                    .Field(nameof(ProvideAdditionalInformation))
                    .Field(nameof(AdditionalInformation), (s) => s.ProvideAdditionalInformation)
                    .Confirm("Great. I have the following detail and I am ready to submit them.\r\r Name: {Name}\rAddress: {Address} {PostCode}\r\rWe will contact you via {PreferredContactMethod} using {?{Email}} {?{ContactNumber}}\r\rIs that all correct?")
                    .OnCompletion(FaultFormCompleted)
                    .Message("Thank you, we have submitted your fault report. You case ID is EGC987542, which you will need if you need to talk to us about this case.")
                    .Build();
        }

        private static Task FaultFormCompleted(IDialogContext context, FaultReport state)
        {
            // place code here for submitting a fault report.
            return null;
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
        [Terms("phone", "mobile", "telephone", "call", "call me", "ring me", "phone me")]
        Telephone,
        [Terms("text message", "text", "SMS")]
        SMS,
        [Terms("email", "email me", "by email")]
        Email
    }
}