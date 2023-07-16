using Fonbec.Cartas.DataAccess.Entities.Actors;
using Fonbec.Cartas.DataAccess.Repositories.Admin.DataImport;
using Fonbec.Cartas.Logic.Services.Admin.DataImport.DataReader.Payloads;

namespace Fonbec.Cartas.Logic.Services.Admin.DataImport.DataReader
{
    public class PadrinosFileParser : FileParserBase<Padrino, Padrino>
    {
        private readonly IDataImportRepository _dataImportRepository;

        public PadrinosFileParser(IDataImportRepository dataImportRepository)
        {
            _dataImportRepository = dataImportRepository;
        }

        protected override Padrino CreateObjectFromFields(List<string> fields, IDataReaderPayload<Padrino> payload, int lineNumber, List<string> errors)
        {
            var padrinoPayload = (PadrinoPayload)payload;

            var firstName = fields[0];
            var lastName = string.IsNullOrEmpty(fields[1]) ? null : fields[1];
            var nickName = string.IsNullOrEmpty(fields[2]) ? null : fields[2];
            var genderString = fields[3];
            var email = fields[4];
            var phone = string.IsNullOrEmpty(fields[5]) ? null : fields[5];

            ValidateFirstName(firstName, errors, lineNumber);

            var gender = ValidateGender(genderString, errors, lineNumber);

            ValidateEmail(email, errors, lineNumber, isRequired: true);

            var padrino = new Padrino
            {
                FilialId = padrinoPayload.FilialId,
                CreatedByCoordinadorId = padrinoPayload.CreatedByCoordinadorId,
                FirstName = firstName,
                LastName = lastName,
                NickName = nickName,
                Gender = gender,
                Email = email,
                Phone = phone,
            };

            return padrino;
        }

        protected override async Task<IEnumerable<Padrino>> GetExistingObjectsAsync()
        {
            var padrinos = await _dataImportRepository.GetAllPadrinosAsync();
            return padrinos;
        }

        protected override string GetKey(Padrino t) => t.FullName();
    }
}
