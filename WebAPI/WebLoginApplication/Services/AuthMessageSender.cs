using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace WebLoginApplication.Services
{
    public class AuthMessageSender : ISmsSender
    {
        public AuthMessageSender(IOptions<SMSoptions> optionsAccessor)
        {
            Options = optionsAccessor.Value;
        }

        public SMSoptions Options { get; }  // definido apenas por meio do Secret Manager

        public Task SendEmailAsync(string email, string subject, string message)
        {
            // Conecte seu serviço de e-mail aqui para enviar um e-mail.
            return Task.FromResult(0);
        }

        public Task SendSmsAsync(string number, string message)
        {
            // Conecte seu serviço SMS aqui para enviar uma mensagem de texto.
            // Seu Account SID from twilio.com/console
            var accountSid = Options.SMSAccountIdentification;
            // Seu Auth Token from twilio.com/console
            var authToken = Options.SMSAccountPassword;

            TwilioClient.Init(accountSid, authToken);

            return MessageResource.CreateAsync(
              to: new PhoneNumber(number),
              from: new PhoneNumber(Options.SMSAccountFrom),
              body: message);
        }
    }
}
