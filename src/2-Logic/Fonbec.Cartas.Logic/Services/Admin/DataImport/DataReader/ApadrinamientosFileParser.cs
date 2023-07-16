using Fonbec.Cartas.DataAccess.Entities;
using Fonbec.Cartas.DataAccess.Entities.Actors;
using Fonbec.Cartas.Logic.Constants;
using System.Globalization;
using Fonbec.Cartas.DataAccess.Repositories.Admin.DataImport;
using Fonbec.Cartas.Logic.Services.Admin.DataImport.DataReader.Payloads;

namespace Fonbec.Cartas.Logic.Services.Admin.DataImport.DataReader
{
    public class ApadrinamientosFileParser : FileParserBase<Apadrinamiento, Apadrinamiento>
    {
        private readonly IDataImportRepository _dataImportRepository;

        public ApadrinamientosFileParser(IDataImportRepository dataImportRepository)
        {
            _dataImportRepository = dataImportRepository;
        }

        protected override Apadrinamiento CreateObjectFromFields(List<string> fields, IDataReaderPayload<Apadrinamiento> payload, int lineNumber, List<string> errors)
        {
            var apadrinamientoPayload = (ApadrinamientoPayload)payload;

            var becario = fields[0];
            var padrino = fields[1];
            var desde = fields[2];
            var hasta = string.IsNullOrEmpty(fields[3]) ? null : fields[3];

            var desdeDate = DateTime.MinValue;
            if (string.IsNullOrEmpty(desde))
            {
                AddError(errors, lineNumber, "desde cannot be empty");
            }
            else if (!DateTime.TryParseExact(desde, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out desdeDate))
            {
                AddError(errors, lineNumber, $"desde is '{desde}', but must be specified as a date with this format: 'YYYY-MM-DD'");
            }

            var hastaDate = DateTime.MinValue;
            if (hasta is not null && !DateTime.TryParseExact(hasta, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out hastaDate))
            {
                AddError(errors, lineNumber, $"hasta is '{hasta}'; if known, it must be specified as a date with this format: 'YYYY-MM-DD'; otherwise leave blank");
            }

            // If an ID is specified, it references an existing Becario; otherwise, it references a name in becarios.csv
            if (int.TryParse(becario, out var becarioId))
            {
                var becarioIdExists = apadrinamientoPayload.ExistingBecarioIds.Contains(becarioId);
                if (!becarioIdExists)
                {
                    AddError(errors, lineNumber, $"becario ID {becarioId} does not exist in the database for the Filial specified");
                    becarioId = 0;
                }
            }
            else
            {
                var existingBecario = apadrinamientoPayload.ExistingBecarios.SingleOrDefault(p => p.FullName() == becario);
                if (existingBecario is null)
                {
                    AddError(errors, lineNumber, $"becario '{becario}' does not exist; make sure spelling matches that in {ImportFileNameConstants.FileNameOf(typeof(Becario))}");
                }
                else
                {
                    becarioId = existingBecario.Id;
                }
            }

            // If an ID is specified, it references an existing Padrino; otherwise, it references a name in padrinos.csv
            if (int.TryParse(padrino, out var padrinoId))
            {
                var padrinoIdExists = apadrinamientoPayload.ExistingPadrinoIds.Contains(padrinoId);
                if (!padrinoIdExists)
                {
                    AddError(errors, lineNumber, $"padrino ID {padrinoId} does not exist in the database for the Filial specified");
                    padrinoId = 0;
                }
            }
            else
            {
                var existingPadrino = apadrinamientoPayload.ExistingPadrinos.SingleOrDefault(p => p.FullName() == padrino);
                if (existingPadrino is null)
                {
                    AddError(errors, lineNumber, $"padrino '{padrino}' does not exist; make sure spelling matches that in {ImportFileNameConstants.FileNameOf(typeof(Padrino))}");
                }
                else
                {
                    padrinoId = existingPadrino.Id;
                }
            }

            var apadrinamiento = new Apadrinamiento
            {
                CreatedByCoordinadorId = apadrinamientoPayload.CreatedByCoordinadorId,
                From = desdeDate,
                To = hasta is null || hastaDate == DateTime.MinValue ? null : hastaDate,
                BecarioId = becarioId,
                PadrinoId = padrinoId,
            };

            return apadrinamiento;
        }

        protected override async Task<IEnumerable<Apadrinamiento>> GetExistingObjectsAsync()
        {
            var apadrinamientos = await _dataImportRepository.GetAllApadrinamientosAsync();
            return apadrinamientos;
        }

        protected override string GetKey(Apadrinamiento t) => $"{t.BecarioId}-{t.PadrinoId}";
    }
}
