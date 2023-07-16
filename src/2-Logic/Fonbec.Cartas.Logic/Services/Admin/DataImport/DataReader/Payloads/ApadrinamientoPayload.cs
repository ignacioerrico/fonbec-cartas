using Fonbec.Cartas.DataAccess.Entities;
using Fonbec.Cartas.DataAccess.Entities.Actors;

namespace Fonbec.Cartas.Logic.Services.Admin.DataImport.DataReader.Payloads
{
    public class ApadrinamientoPayload : IDataReaderPayload<Apadrinamiento>
    {
        public int CreatedByCoordinadorId { get; set; }

        public List<Becario> ExistingBecarios { get; set; } = default!;

        public List<Padrino> ExistingPadrinos { get; set; } = default!;

        public List<int> ExistingBecarioIds { get; set; } = default!;

        public List<int> ExistingPadrinoIds { get; set; } = default!;
    }
}
