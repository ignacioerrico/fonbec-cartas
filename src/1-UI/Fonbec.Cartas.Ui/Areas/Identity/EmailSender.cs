using Fonbec.Cartas.Logic.Models;
using Fonbec.Cartas.Logic.Services;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Fonbec.Cartas.Ui.Areas.Identity
{
    public class EmailSender : IEmailSender
    {
        private readonly ILogger<EmailSender> _logger;
        private readonly IConfiguration _configuration;
        private readonly ICommunicationService _communicationService;

        public EmailSender(ILogger<EmailSender> logger,
            IConfiguration configuration,
            ICommunicationService communicationService)
        {
            _logger = logger;
            _configuration = configuration;
            _communicationService = communicationService;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            _logger.LogDebug("Sending email to {To} with subject '{Subject}' and body '{Body}'.",
                email,
                subject,
                htmlMessage);

            var emailMessageBuilder =
                new EmailMessageBuilder(_configuration,
                    new() { new(email) },
                    subject,
                    htmlMessage);

            var emailMessage = emailMessageBuilder.Build();

            await _communicationService.SendEmailAsync(emailMessage);
        }
    }
}
