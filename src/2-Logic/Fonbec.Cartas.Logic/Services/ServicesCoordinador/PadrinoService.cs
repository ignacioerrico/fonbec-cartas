using Fonbec.Cartas.DataAccess.Entities;
using Fonbec.Cartas.DataAccess.Entities.Actors;
using Fonbec.Cartas.DataAccess.Repositories;
using Fonbec.Cartas.Logic.ViewModels.Coordinador;

namespace Fonbec.Cartas.Logic.Services.ServicesCoordinador
{
    public interface IPadrinoService
    {
        Task<PadrinoEditViewModel?> GetPadrinoAsync(int padrinoId, int filialId);
        Task<int> CreateAsync(PadrinoEditViewModel padrinoEditViewModel);
        Task<int> UpdateAsync(int id, PadrinoEditViewModel padrino);
    }

    public class PadrinoService : IPadrinoService
    {
        private readonly IPadrinoRepository _padrinoRepository;

        public PadrinoService(IPadrinoRepository padrinoRepository)
        {
            _padrinoRepository = padrinoRepository;
        }

        public async Task<PadrinoEditViewModel?> GetPadrinoAsync(int padrinoId, int filialId)
        {
            var padrino = await _padrinoRepository.GetPadrinoAsync(padrinoId, filialId);

            if (padrino is null)
            {
                return null;
            }

            var padrinoEditViewModel = new PadrinoEditViewModel
            {
                FirstName = padrino.FirstName,
                LastName = padrino.LastName,
                NickName = padrino.NickName ?? string.Empty,
                Gender = padrino.Gender,
                Email = padrino.Email,
                SendAlsoTo = padrino.SendAlsoTo is null
                    ? new List<PadrinoEditSendAlsoToViewModel>()
                    : padrino.SendAlsoTo.Select(sat =>
                        new PadrinoEditSendAlsoToViewModel
                        {
                            RecipientFullName = sat.RecipientFullName,
                            RecipientEmail = sat.RecipientEmail,
                            SendAsBcc = sat.SendAsBcc,
                        }).ToList(),
                Phone = padrino.Phone ?? string.Empty,
            };

            return padrinoEditViewModel;
        }

        public async Task<int> CreateAsync(PadrinoEditViewModel padrinoEditViewModel)
        {
            var padrino = new Padrino
            {
                FilialId = padrinoEditViewModel.FilialId,
                FirstName = padrinoEditViewModel.FirstName,
                LastName = padrinoEditViewModel.LastName,
                NickName = string.IsNullOrWhiteSpace(padrinoEditViewModel.NickName) ? null : padrinoEditViewModel.NickName,
                Gender = padrinoEditViewModel.Gender,
                Email = padrinoEditViewModel.Email,
                SendAlsoTo = padrinoEditViewModel.SendAlsoTo.Any()
                    ? padrinoEditViewModel.SendAlsoTo
                            .Select(sat => new SendAlsoTo
                                {
                                    RecipientFullName = sat.RecipientFullName,
                                    RecipientEmail = sat.RecipientEmail,
                                    SendAsBcc = sat.SendAsBcc,
                                }).ToList()
                    : null,
                Phone = string.IsNullOrWhiteSpace(padrinoEditViewModel.Phone) ? null : padrinoEditViewModel.Phone,
                CreatedByCoordinadorId = padrinoEditViewModel.CreatedByCoordinadorId,
            };

            var rowsAffected = await _padrinoRepository.CreateAsync(padrino);

            return rowsAffected;
        }

        public async Task<int> UpdateAsync(int id, PadrinoEditViewModel padrinoEditViewModel)
        {
            var padrino = new Padrino
            {
                FilialId = padrinoEditViewModel.FilialId,
                FirstName = padrinoEditViewModel.FirstName,
                LastName = padrinoEditViewModel.LastName,
                NickName = string.IsNullOrWhiteSpace(padrinoEditViewModel.NickName) ? null : padrinoEditViewModel.NickName,
                Gender = padrinoEditViewModel.Gender,
                Email = padrinoEditViewModel.Email,
                SendAlsoTo = padrinoEditViewModel.SendAlsoTo.Any()
                    ? padrinoEditViewModel.SendAlsoTo
                        .Select(sat => new SendAlsoTo
                        {
                            RecipientFullName = sat.RecipientFullName,
                            RecipientEmail = sat.RecipientEmail,
                            SendAsBcc = sat.SendAsBcc,
                        }).ToList()
                    : null,
                Phone = string.IsNullOrWhiteSpace(padrinoEditViewModel.Phone) ? null : padrinoEditViewModel.Phone,
                UpdatedByCoordinadorId = padrinoEditViewModel.UpdatedByCoordinadorId,
            };

            var rowsAffected = await _padrinoRepository.UpdateAsync(id, padrino);

            return rowsAffected;
        }
    }
}
