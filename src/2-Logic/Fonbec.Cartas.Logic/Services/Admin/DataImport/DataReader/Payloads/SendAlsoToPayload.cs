using Fonbec.Cartas.DataAccess.Entities;
using Fonbec.Cartas.DataAccess.Entities.Actors;
using Fonbec.Cartas.DataAccess.Entities.DataImport;

namespace Fonbec.Cartas.Logic.Services.Admin.DataImport.DataReader.Payloads
{
    public class SendAlsoToPayload : IDataReaderPayload<SendAlsoTo>
    {
        public List<PadrinoToUpdate> PadrinosToUpdate { get; set; } = new();

        public List<Padrino> PadrinosToCreate { get; set; } = default!;
    }
}
