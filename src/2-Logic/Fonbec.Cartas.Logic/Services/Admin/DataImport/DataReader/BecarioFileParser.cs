using Fonbec.Cartas.DataAccess.Entities.Actors;
using Fonbec.Cartas.DataAccess.Entities.Enums;
using Fonbec.Cartas.DataAccess.Repositories.Admin.DataImport;
using Fonbec.Cartas.Logic.Constants;
using Fonbec.Cartas.Logic.Services.Admin.DataImport.DataReader.Payloads;

namespace Fonbec.Cartas.Logic.Services.Admin.DataImport.DataReader
{
    public class BecarioFileParser : FileParserBase<Becario, Becario>
    {
        private readonly IDataImportRepository _dataImportRepository;

        public BecarioFileParser(IDataImportRepository dataImportRepository)
        {
            _dataImportRepository = dataImportRepository;
        }

        protected override Becario CreateObjectFromFields(List<string> fields, IDataReaderPayload<Becario> payload, int lineNumber, List<string> errors)
        {
            var becarioPayload = (BecarioPayload)payload;

            var firstName = fields[0];
            var lastName = string.IsNullOrEmpty(fields[1]) ? null : fields[1];
            var nickName = string.IsNullOrEmpty(fields[2]) ? null : fields[2];
            var genderString = fields[3];
            var nivelDeEstudioString = fields[4];
            var email = string.IsNullOrEmpty(fields[5]) ? null : fields[5];
            var phone = string.IsNullOrEmpty(fields[6]) ? null : fields[6];
            var mediador = fields[7];

            ValidateFirstName(firstName, errors, lineNumber);
            
            var gender = ValidateGender(genderString, errors, lineNumber);

            if (!Enum.TryParse(nivelDeEstudioString, ignoreCase: true, out NivelDeEstudio nivelDeEstudio))
            {
                var nivelesDeEstudio = Enum.GetNames(typeof(NivelDeEstudio)).Select(s => $"'{s}'");
                AddError(errors, lineNumber, $"nivel de estudio '{nivelDeEstudioString}' is invalid; it must be one of: {string.Join(", ", nivelesDeEstudio)}");
            }

            ValidateEmail(email, errors, lineNumber, isRequired: false);

            if (string.IsNullOrEmpty(mediador))
            {
                AddError(errors, lineNumber, "mediador cannot be empty");
            }

            var becario = new Becario
            {
                FilialId = becarioPayload.FilialId,
                CreatedByCoordinadorId = becarioPayload.CreatedByCoordinadorId,
                FirstName = firstName,
                LastName = lastName,
                NickName = nickName,
                Gender = gender,
                NivelDeEstudio = nivelDeEstudio,
                Email = email,
                Phone = phone,
            };

            if (string.IsNullOrWhiteSpace(mediador))
            {
                return becario;
            }

            // If an ID is specified, it references an existing Mediador; otherwise, it references a name in mediadores.csv
            if (int.TryParse(mediador, out var mediadorId))
            {
                var mediadorIdExists = becarioPayload.ExistingMediadorIds.Contains(mediadorId);
                if (mediadorIdExists)
                {
                    becario.MediadorId = mediadorId;
                }
                else
                {
                    AddError(errors, lineNumber, $"mediador ID {mediadorId} does not exist in the database for the Filial specified");
                }
            }
            else
            {
                var existingMediador = becarioPayload.ExistingMediadores.SingleOrDefault(p => p.FullName() == mediador);
                if (existingMediador is null)
                {
                    AddError(errors, lineNumber, $"mediador '{mediador}' does not exist; make sure spelling matches that in {ImportFileNameConstants.FileNameOf(typeof(Mediador))}");
                }
                else
                {
                    becario.MediadorId = existingMediador.Id;
                }
            }

            return becario;
        }

        protected override async Task<IEnumerable<Becario>> GetExistingObjectsAsync()
        {
            var becarios = await _dataImportRepository.GetAllBecariosAsync();
            return becarios;
        }

        protected override string GetKey(Becario t) => t.FullName();
    }
}
