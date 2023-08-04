using System.IO.Compression;
using System.Text;
using Fonbec.Cartas.DataAccess.Entities.Actors;
using Fonbec.Cartas.DataAccess.Entities;
using Fonbec.Cartas.DataAccess.Entities.DataImport;
using Fonbec.Cartas.DataAccess.Repositories.Admin;
using Fonbec.Cartas.DataAccess.Repositories.Admin.DataImport;
using Fonbec.Cartas.Logic.Constants;
using Fonbec.Cartas.Logic.Models;
using Fonbec.Cartas.Logic.Models.Admin.DataImport;
using Fonbec.Cartas.Logic.Services.Admin.DataImport.DataReader;
using Fonbec.Cartas.Logic.Services.Admin.DataImport.DataReader.Payloads;
using Mapster;

namespace Fonbec.Cartas.Logic.Services.Admin.DataImport
{
    public interface IDataImportService
    {
        Task<byte[]> CreateZippedDataset();
        Task<List<SelectableModel<int>>> GetAllCoordinadoresForSelectionAsync(int filialId);
        bool CorrectFilesHaveBeenSelected(IReadOnlyCollection<string> uploadedFileNames);
        Task<ImportDataStreamsOutputModel> ImportData(ImportDataStreamsInputModel input);
    }

    public class DataImportService : IDataImportService
    {
        private readonly IUserWithAccountSharedService _userWithAccountSharedService;
        private readonly IUserWithAccountRepositoryBase<DataAccess.Entities.Actors.Coordinador> _coordinadorRepository;
        private readonly FileParserBase<DataAccess.Entities.Actors.Coordinador, UserWithAccountToCreate> _coordinadorFileParser;
        private readonly FileParserBase<DataAccess.Entities.Actors.Mediador, UserWithAccountToCreate> _mediadorFileParser;
        private readonly FileParserBase<Revisor, UserWithAccountToCreate> _revisorFileParser;
        private readonly FileParserBase<Padrino, Padrino> _padrinosFileParser;
        private readonly FileParserBase<SendAlsoTo, SendAlsoTo> _sendAltoToFileParser;
        private readonly FileParserBase<Becario, Becario> _becarioFileParser;
        private readonly FileParserBase<Apadrinamiento, Apadrinamiento> _apadrinamientosFileParser;
        private readonly ICreateUserWithAccountService<DataAccess.Entities.Actors.Coordinador> _createCoordinadorService;
        private readonly ICreateUserWithAccountService<DataAccess.Entities.Actors.Mediador> _createMediadorService;
        private readonly ICreateUserWithAccountService<Revisor> _createRevisorService;
        private readonly IDataImportRepository _dataImportRepository;

        public DataImportService(IUserWithAccountSharedService userWithAccountSharedService,
            IUserWithAccountRepositoryBase<DataAccess.Entities.Actors.Coordinador> coordinadorRepository,
            FileParserBase<DataAccess.Entities.Actors.Coordinador, UserWithAccountToCreate> coordinadorFileParser,
            FileParserBase<DataAccess.Entities.Actors.Mediador, UserWithAccountToCreate> mediadorFileParser,
            FileParserBase<Revisor, UserWithAccountToCreate> revisorFileParser,
            FileParserBase<Padrino, Padrino> padrinosFileParser,
            FileParserBase<SendAlsoTo, SendAlsoTo> sendAltoToFileParser,
            FileParserBase<Becario, Becario> becarioFileParser,
            FileParserBase<Apadrinamiento, Apadrinamiento> apadrinamientosFileParser,
            ICreateUserWithAccountService<DataAccess.Entities.Actors.Coordinador> createCoordinadorService,
            ICreateUserWithAccountService<DataAccess.Entities.Actors.Mediador> createMediadorService,
            ICreateUserWithAccountService<Revisor> createRevisorService,
            IDataImportRepository dataImportRepository)
        {
            _userWithAccountSharedService = userWithAccountSharedService;
            _coordinadorRepository = coordinadorRepository;
            _coordinadorFileParser = coordinadorFileParser;
            _mediadorFileParser = mediadorFileParser;
            _revisorFileParser = revisorFileParser;
            _padrinosFileParser = padrinosFileParser;
            _sendAltoToFileParser = sendAltoToFileParser;
            _becarioFileParser = becarioFileParser;
            _apadrinamientosFileParser = apadrinamientosFileParser;
            _createCoordinadorService = createCoordinadorService;
            _createMediadorService = createMediadorService;
            _createRevisorService = createRevisorService;
            _dataImportRepository = dataImportRepository;
        }

        public async Task<byte[]> CreateZippedDataset()
        {
            var types = new[]
            {
                typeof(DataAccess.Entities.Actors.Coordinador),
                typeof(DataAccess.Entities.Actors.Mediador),
                typeof(Revisor),
                typeof(Padrino),
                typeof(SendAlsoTo),
                typeof(Becario),
                typeof(Apadrinamiento),
            };

            var memoryStream = new MemoryStream();

            using var zipArchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true);

            foreach (var type in types)
            {
                var zipArchiveEntry = zipArchive.CreateEntry(ImportFileNameConstants.FileNameOf(type), CompressionLevel.Optimal);
                await using var entryStream = zipArchiveEntry.Open();
                var fileContents = Encoding.UTF8.GetBytes(ImportFileNameConstants.HeaderOf(type));
                using var zipMemoryStream = new MemoryStream(fileContents);
                await zipMemoryStream.CopyToAsync(entryStream);
            }

            memoryStream.Seek(0, SeekOrigin.Begin);

            return memoryStream.ToArray();
        }

        public async Task<List<SelectableModel<int>>> GetAllCoordinadoresForSelectionAsync(int filialId)
        {
            var coordinadores = await _coordinadorRepository.GetAllInFilialAsync(filialId);
            return coordinadores.Select(f => new SelectableModel<int>(f.Id, f.FullName())).ToList();
        }

        public bool CorrectFilesHaveBeenSelected(IReadOnlyCollection<string> uploadedFileNames)
        {
            return uploadedFileNames.Count == ImportFileNameConstants.ExpectedFileNames.Length
                   && uploadedFileNames.All(fileName => ImportFileNameConstants.ExpectedFileNames.Contains(fileName.ToLower()));
        }

        public async Task<ImportDataStreamsOutputModel> ImportData(ImportDataStreamsInputModel input)
        {
            var output = new ImportDataStreamsOutputModel();

            output.CoordinadoresCreated = await ImportCoordinadoresAsync(input, output.Errors);

            output.MediadoresCreated = await ImportMediadoresAsync(input, output.Errors);

            output.RevisoresCreated = await ImportRevisoresAsync(input, output.Errors);

            output.PadrinosCreated = await ImportPadrinosAsync(input, output.Errors);

            output.PadrinosUpdated = await ImportSendAlsoToAsync(input, output.PadrinosCreated, output.Errors);

            output.BecariosCreated = await ImportBecariosAsync(input, output.MediadoresCreated, output.Errors);

            output.ApadrinamientosCreated = await ImportApadrinamientosAsync(input, output.BecariosCreated, output.PadrinosCreated, output.Errors);

            return output;
        }

        private async Task<List<DataAccess.Entities.Actors.Coordinador>> ImportCoordinadoresAsync(ImportDataStreamsInputModel input, List<string> errors)
        {
            var coordinadoresFile = await GetFileContentsAsync(input.Coordinadores, errors);

            var coordinadorPayload = new UserWithAccountPayload<DataAccess.Entities.Actors.Coordinador>
            {
                UserWithAccountService = _userWithAccountSharedService,
            };
            var coordinadores = await _coordinadorFileParser.ConvertToObjects(coordinadoresFile, coordinadorPayload, errors);

            var coordinadoresCreated = input.IsDryRun
                ? coordinadores.Adapt<List<DataAccess.Entities.Actors.Coordinador>>()
                : await _createCoordinadorService.CreateAsync(input.FilialId, coordinadores, errors);

            return coordinadoresCreated;
        }

        private async Task<List<DataAccess.Entities.Actors.Mediador>> ImportMediadoresAsync(ImportDataStreamsInputModel input, List<string> errors)
        {
            var mediadoresFile = await GetFileContentsAsync(input.Mediadores, errors);

            var mediadorPayload = new UserWithAccountPayload<DataAccess.Entities.Actors.Mediador>
            {
                UserWithAccountService = _userWithAccountSharedService
            };
            var mediadores = await _mediadorFileParser.ConvertToObjects(mediadoresFile, mediadorPayload, errors);

            var mediadoresCreated = input.IsDryRun
                ? mediadores.Adapt<List<DataAccess.Entities.Actors.Mediador>>()
                : await _createMediadorService.CreateAsync(input.FilialId, mediadores, errors);

            return mediadoresCreated;
        }

        private async Task<List<Revisor>> ImportRevisoresAsync(ImportDataStreamsInputModel input, List<string> errors)
        {
            var revisoresFile = await GetFileContentsAsync(input.Revisores, errors);

            var revisorPayload = new UserWithAccountPayload<Revisor>
            {
                UserWithAccountService = _userWithAccountSharedService
            };
            var revisores = await _revisorFileParser.ConvertToObjects(revisoresFile, revisorPayload, errors);

            var revisoresCreated = input.IsDryRun
                ? revisores.Adapt<List<Revisor>>()
                : await _createRevisorService.CreateAsync(input.FilialId, revisores, errors);

            return revisoresCreated;
        }

        private async Task<List<Padrino>> ImportPadrinosAsync(ImportDataStreamsInputModel input, List<string> errors)
        {
            var padrinosFile = await GetFileContentsAsync(input.Padrinos, errors);

            var padrinoPayload = new PadrinoPayload
            {
                FilialId = input.FilialId,
                CreatedByCoordinadorId = input.CreatedByCoordinadorId
            };
            var padrinosToCreate = await _padrinosFileParser.ConvertToObjects(padrinosFile, padrinoPayload, errors);

            if (!input.IsDryRun)
            {
                // This will set the ID, needed for apadrinamientos
                padrinosToCreate = await _dataImportRepository.CreatePadrinosAsync(padrinosToCreate);
            }

            return padrinosToCreate;
        }

        private async Task<List<PadrinoToUpdate>> ImportSendAlsoToAsync(ImportDataStreamsInputModel input, List<Padrino> padrinosCreated, List<string> errors)
        {
            var sendAlsoToFile = await GetFileContentsAsync(input.EnviarCopia, errors);

            var sendAlsoToPayload = new SendAlsoToPayload
            {
                PadrinosToCreate = padrinosCreated
            };
            _  =_sendAltoToFileParser.ConvertToObjects(sendAlsoToFile, sendAlsoToPayload, errors);

            if (!input.IsDryRun)
            {
                await _dataImportRepository.UpdatePadrinosAsync(sendAlsoToPayload.PadrinosToUpdate, errors);
            }

            return sendAlsoToPayload.PadrinosToUpdate;
        }

        private async Task<List<Becario>> ImportBecariosAsync(ImportDataStreamsInputModel input, List<DataAccess.Entities.Actors.Mediador> mediadoresCreated, List<string> errors)
        {
            var becariosFile = await GetFileContentsAsync(input.Becarios, errors);

            var existingMediadorIds = await _dataImportRepository.GetExistingMediadorIdsAsync(input.FilialId);

            var becarioPayload = new BecarioPayload
            {
                FilialId = input.FilialId,
                CreatedByCoordinadorId = input.CreatedByCoordinadorId,
                ExistingMediadores = mediadoresCreated,
                ExistingMediadorIds = existingMediadorIds,
            };
            var becarios = await _becarioFileParser.ConvertToObjects(becariosFile, becarioPayload, errors);

            if (!input.IsDryRun)
            {
                // This will set the ID, needed for apadrinamientos
                becarios = await _dataImportRepository.CreateBecariosAsync(becarios);
            }

            return becarios;
        }

        private async Task<List<Apadrinamiento>> ImportApadrinamientosAsync(ImportDataStreamsInputModel input, List<Becario> becariosCreated, List<Padrino> padrinosCreated, List<string> errors)
        {
            var apadrinamientosFile = await GetFileContentsAsync(input.Apadrinamientos, errors);

            var (existingBecarioIds, existingPadrinoIds) = await _dataImportRepository.GetExistingBecarioAndPadrinoIdsAsync(input.FilialId);

            var apadrinamientoPayload = new ApadrinamientoPayload
            {
                CreatedByCoordinadorId = input.CreatedByCoordinadorId,
                ExistingBecarios = becariosCreated,
                ExistingPadrinos = padrinosCreated,
                ExistingBecarioIds = existingBecarioIds,
                ExistingPadrinoIds = existingPadrinoIds,
            };
            var apadrinamientos = await _apadrinamientosFileParser.ConvertToObjects(apadrinamientosFile, apadrinamientoPayload, errors);

            if (!input.IsDryRun)
            {
                await _dataImportRepository.CreateApadrinamientoAsync(apadrinamientos);
            }

            return apadrinamientos;
        }

        private static async Task<string> GetFileContentsAsync(Stream stream, ICollection<string> errors)
        {
            var fileContents = string.Empty;
            try
            {
                using var streamReader = new StreamReader(stream);
                fileContents = await streamReader.ReadToEndAsync();
            }
            catch (Exception e)
            {
                errors.Add(e.Message);
            }

            return fileContents;
        }
    }
}
