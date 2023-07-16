using Fonbec.Cartas.DataAccess.Entities;
using Fonbec.Cartas.DataAccess.Entities.Actors;
using Fonbec.Cartas.Logic.Constants;
using Fonbec.Cartas.Logic.Models;
using Fonbec.Cartas.Logic.Models.Admin.DataImport;
using Fonbec.Cartas.Logic.Services.Admin;
using Fonbec.Cartas.Logic.Services.Admin.DataImport;
using Fonbec.Cartas.Ui.Constants;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using MudBlazor;

namespace Fonbec.Cartas.Ui.Pages.Admin
{
    public partial class DataImport
    {
        private ImportDataStreamsOutputModel? _model;

        private bool _loading;
        private bool _runningImport;

        private List<SelectableModel<int>> _filiales = new();
        private SelectableModel<int>? _selectedFilial;

        private List<SelectableModel<int>> _coordinadores = new();
        private SelectableModel<int>? _selectedCoordinador;

        private bool _isDryRun;
        private bool? _rightFilesUploaded;

        [Inject]
        public IFilialService FilialService { get; set; } = default!;

        [Inject]
        public IDataImportService DataImportService { get; set; } = default!;

        [Inject]
        public IJSRuntime JsRuntime { get; set; } = default!;

        [Inject]
        public ISnackbar Snackbar { get; set; } = default!;

        [Inject]
        public NavigationManager NavigationManager { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            _loading = true;

            _filiales = await FilialService.GetAllFilialesForSelectionAsync();

            if (!_filiales.Any())
            {
                Snackbar.Add("No hay filiales.", Severity.Warning);
                NavigationManager.NavigateTo(NavRoutes.AdminFiliales);
                return;
            }

            _selectedFilial = _filiales.First();

            await OnFilialChanged(_selectedFilial);

            _loading = false;
        }

        private async Task DownloadDataSet()
        {
            const string fileName = "dataset.zip";
            
            var zippedDatasetBytes = await DataImportService.CreateZippedDataset();

            using var memoryStream = new MemoryStream(zippedDatasetBytes);
            using var streamRef = new DotNetStreamReference(stream: memoryStream);
            await JsRuntime.InvokeVoidAsync("downloadFileFromStream", fileName, streamRef);
        }

        private async Task OnFilialChanged(SelectableModel<int> filial)
        {
            _selectedFilial = filial;

            _coordinadores = await DataImportService.GetAllCoordinadoresForSelectionAsync(filial.Id);

            if (_coordinadores.Any())
            {
                _selectedCoordinador = _coordinadores.First();
            }
            else
            {
                _selectedCoordinador = null;
                Snackbar.Add("Esta filial no tiene coordinadores.", Severity.Warning);
            }
        }

        private async Task UploadDataset(IReadOnlyList<IBrowserFile> files)
        {
            if (_selectedFilial is null)
            {
                Snackbar.Add("Seleccioná una filial.", Severity.Warning);
                return;
            }

            if (_selectedCoordinador is null)
            {
                Snackbar.Add("Seleccioná un coordinador.", Severity.Warning);
                return;
            }

            _runningImport = true;

            var correctFilesHaveBeenSelected = DataImportService.CorrectFilesHaveBeenSelected(files.Select(f => f.Name).ToList());
            if (!correctFilesHaveBeenSelected)
            {
                _rightFilesUploaded = false;
                _runningImport = false;
                return;
            }

            _rightFilesUploaded = true;

            var importDataStreamsModel = new ImportDataStreamsInputModel
            {
                IsDryRun = _isDryRun,
                FilialId = _selectedFilial.Id,
                CreatedByCoordinadorId = _selectedCoordinador.Id,
                Apadrinamientos = files.Single(f => f.Name.ToLower() == ImportFileNameConstants.FileNameOf(typeof(Apadrinamiento))).OpenReadStream(),
                Becarios = files.Single(f => f.Name.ToLower() == ImportFileNameConstants.FileNameOf(typeof(Becario))).OpenReadStream(),
                Coordinadores = files.Single(f => f.Name.ToLower() == ImportFileNameConstants.FileNameOf(typeof(DataAccess.Entities.Actors.Coordinador))).OpenReadStream(),
                EnviarCopia = files.Single(f => f.Name.ToLower() == ImportFileNameConstants.FileNameOf(typeof(SendAlsoTo))).OpenReadStream(),
                Mediadores = files.Single(f => f.Name.ToLower() == ImportFileNameConstants.FileNameOf(typeof(DataAccess.Entities.Actors.Mediador))).OpenReadStream(),
                Padrinos = files.Single(f => f.Name.ToLower() == ImportFileNameConstants.FileNameOf(typeof(Padrino))).OpenReadStream(),
                Revisores = files.Single(f => f.Name.ToLower() == ImportFileNameConstants.FileNameOf(typeof(DataAccess.Entities.Actors.Revisor))).OpenReadStream()
            };

            _model = await DataImportService.ImportData(importDataStreamsModel);

            _runningImport = false;
        }
    }
}
