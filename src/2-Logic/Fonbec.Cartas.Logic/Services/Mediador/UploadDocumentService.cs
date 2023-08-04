using Fonbec.Cartas.DataAccess.Repositories.Mediador;
using Fonbec.Cartas.Logic.Models;
using Mapster;

namespace Fonbec.Cartas.Logic.Services.Mediador
{
    public interface IUploadDocumentService
    {
        Task<List<SelectableModel<int>>> GetBecariosAssignedToMediador(int mediadorId);
    }

    public class UploadDocumentService : IUploadDocumentService
    {
        private readonly IUploadDocumentRepository _uploadDocumentRepository;

        public UploadDocumentService(IUploadDocumentRepository uploadDocumentRepository)
        {
            _uploadDocumentRepository = uploadDocumentRepository;
        }

        public async Task<List<SelectableModel<int>>> GetBecariosAssignedToMediador(int mediadorId)
        {
            var becarios = await _uploadDocumentRepository.GetBecariosAssignedToMediador(mediadorId);
            return becarios.Adapt<List<SelectableModel<int>>>();
        }
    }
}
