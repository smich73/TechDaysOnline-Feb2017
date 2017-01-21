using Microsoft.Bot.Builder.FormFlow;
using System;
using Microsoft.Bot.Builder.Dialogs;
using System.Threading.Tasks;

namespace FormFlowDemo.Forms
{
    [Serializable]
    public class ContactMessage
    {
        [Prompt("What's your full name?")]
        public string Name { get; set; }

        [Prompt("What is your address?")]
        public string Address { get; set; }

        [Prompt("Can you give me the best number to reach you on?")]
        public string ContactNumber { get; set; }

        [Prompt("What is your email address?")]
        public string Email { get; set; }

        [Template(TemplateUsage.EnumSelectOne, "What is the best way to get in touch with you? {||}", "How would you like us to contact you? {||}")]
        public ContactMethod PreferredContactMethod { get; set; }

        [Prompt("What is your message?")]
        public string Message { get; set; }
            
        public static IForm<ContactMessage> BuildForm()
        {
            return new FormBuilder<ContactMessage>()
                    .Message("Ok, so you want to submit a contact message? No problem,  just need a few details from you.")
                    .Field(nameof(Name))
                    .Field(nameof(Address))
                    .Field(nameof(PreferredContactMethod))
                    .Field(nameof(ContactNumber), (s) => s.PreferredContactMethod == ContactMethod.SMS || s.PreferredContactMethod == ContactMethod.Telephone)
                    .Field(nameof(Email), IsEmailActive)
                    .AddRemainingFields()
                    .Confirm("Great. I have the following details and I am ready to submit your message.\r\r Name: {Name}\r\rAddress: {Address}\r\rWe will contact you via {PreferredContactMethod} using {?{Email}} {?{ContactNumber}}\r\rIs that all correct?")
                    .OnCompletion(ContactMessageSubmitted)
                    .Message("Thank you, I have submitted your message.")
                    .Build();
        }

        private static bool IsEmailActive(ContactMessage state)
        {
            return state.PreferredContactMethod == ContactMethod.Email;
        }

        private static Task ContactMessageSubmitted(IDialogContext context, ContactMessage state)
        {
            // place code here that you want to run when a contact message is submitted.
            // e.g. sending the contact message to a mailbox or a CRM system
            return null;
        }
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