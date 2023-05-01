using Azure;
using Azure.Communication.Email;
using Microsoft.Extensions.Logging;

namespace Fonbec.Cartas.Logic.Services
{
    public interface ICommunicationService
    {
        Task SendEmailAsync(EmailMessage emailMessage);
    }

    public class CommunicationService : ICommunicationService
    {
        private readonly ILogger<CommunicationService> _logger;
        private readonly EmailClient _emailClient;

        public CommunicationService(ILogger<CommunicationService> logger, EmailClient emailClient)
        {
            _logger = logger;
            _emailClient = emailClient;
        }

        public async Task SendEmailAsync(EmailMessage emailMessage)
        {
            var emailSendOperation = await _emailClient.SendAsync(
                WaitUntil.Started,
                emailMessage);

            try
            {
                while (true)
                {
                    await emailSendOperation.UpdateStatusAsync();
                    if (emailSendOperation.HasCompleted)
                    {
                        break;
                    }

                    await Task.Delay(100);
                }

                if (emailSendOperation.HasValue)
                {
                    _logger.LogDebug("Email queued for delivery. Status = {Status}", emailSendOperation.Value.Status);
                }
            }
            catch (RequestFailedException ex)
            {
                // OperationID is contained in the exception message and can be used for troubleshooting purposes
                _logger.LogError("Email send operation failed with error code: {ErrorCode}, message: {Message}", ex.ErrorCode, ex.Message);
            }

            // The OperationId can be used for tracking the message for troubleshooting
            _logger.LogDebug("Email operation id = {OperationId}", emailSendOperation.Id);
        }
    }
}
