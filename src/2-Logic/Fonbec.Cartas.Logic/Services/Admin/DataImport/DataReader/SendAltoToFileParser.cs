using Fonbec.Cartas.DataAccess.Entities;
using Fonbec.Cartas.DataAccess.Entities.Actors;
using Fonbec.Cartas.Logic.Constants;
using Fonbec.Cartas.DataAccess.Entities.DataImport;
using Fonbec.Cartas.DataAccess.Repositories.Admin.DataImport;
using Fonbec.Cartas.Logic.Services.Admin.DataImport.DataReader.Payloads;

namespace Fonbec.Cartas.Logic.Services.Admin.DataImport.DataReader
{
    public class SendAltoToFileParser : FileParserBase<SendAlsoTo, SendAlsoTo>
    {
        private readonly IDataImportRepository _dataImportRepository;

        public SendAltoToFileParser(IDataImportRepository dataImportRepository)
        {
            _dataImportRepository = dataImportRepository;
        }

        protected override SendAlsoTo CreateObjectFromFields(List<string> fields, IDataReaderPayload<SendAlsoTo> payload, int lineNumber, List<string> errors)
        {
            var sendAlsoToPayload = (SendAlsoToPayload)payload;

            var padrino = fields[0];
            var recipientFullName = fields[1];
            var email = fields[2];
            var bcc = fields[3];

            if (string.IsNullOrEmpty(padrino))
            {
                AddError(errors, lineNumber, "padrino cannot be empty");
            }

            if (string.IsNullOrEmpty(recipientFullName))
            {
                AddError(errors, lineNumber, "recipient name cannot be empty");
            }

            ValidateEmail(email, errors, lineNumber, isRequired: true);

            if (!bool.TryParse(bcc, out var sendAsBcc))
            {
                AddError(errors, lineNumber, $"bcc must be either 'true' or 'false'; '{bcc}' was found");
            }

            var sendAlsoTo = new SendAlsoTo
            {
                RecipientFullName = recipientFullName,
                RecipientEmail = email,
                SendAsBcc = sendAsBcc,
            };

            if (string.IsNullOrEmpty(padrino))
            {
                return sendAlsoTo;
            }

            // If an ID is specified, it references an existing Padrino; otherwise, it references a name in padrinos.csv
            if (int.TryParse(padrino, out var padrinoId))
            {
                var existingPadrinoToUpdate = sendAlsoToPayload.PadrinosToUpdate.SingleOrDefault(p => p.Id == padrinoId);
                if (existingPadrinoToUpdate is null)
                {
                    var padrinoToUpdate = new PadrinoToUpdate { Id = padrinoId };
                    padrinoToUpdate.SendAlsoTo.Add(sendAlsoTo);

                    sendAlsoToPayload.PadrinosToUpdate.Add(padrinoToUpdate);
                }
                else
                {
                    existingPadrinoToUpdate.SendAlsoTo.Add(sendAlsoTo);
                }
            }
            else
            {
                var padrinoToCreate = sendAlsoToPayload.PadrinosToCreate.SingleOrDefault(p => p.FullName() == padrino);
                if (padrinoToCreate is null)
                {
                    AddError(errors, lineNumber, $"padrino '{padrino}' does not exist; make sure spelling matches that in {ImportFileNameConstants.FileNameOf(typeof(Padrino))}");
                }
                else
                {
                    padrinoToCreate.SendAlsoTo ??= new();
                    padrinoToCreate.SendAlsoTo.Add(sendAlsoTo);
                }
            }

            return sendAlsoTo;
        }

        protected override async Task<IEnumerable<SendAlsoTo>> GetExistingObjectsAsync()
        {
            var sendAlsoTos = await _dataImportRepository.GetAllSendAlsoTosAsync();
            return sendAlsoTos;
        }

        protected override string GetKey(SendAlsoTo t) => t.RecipientFullName;
    }
}
