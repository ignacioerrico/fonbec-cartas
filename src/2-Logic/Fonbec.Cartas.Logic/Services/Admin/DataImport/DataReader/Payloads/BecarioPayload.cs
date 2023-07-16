using Fonbec.Cartas.DataAccess.Entities.Actors;

namespace Fonbec.Cartas.Logic.Services.Admin.DataImport.DataReader.Payloads
{
    public class BecarioPayload : IDataReaderPayload<Becario>
    {
        public int FilialId { get; set; }

        public int CreatedByCoordinadorId { get; set; }

        public List<Mediador> ExistingMediadores { get; set; } = default!;

        public List<int> ExistingMediadorIds { get; set; } = default!;
    }
}
