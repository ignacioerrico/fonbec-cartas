using Fonbec.Cartas.DataAccess.Entities.Actors;

namespace Fonbec.Cartas.Logic.Services.Admin.DataImport.DataReader.Payloads
{
    public class PadrinoPayload : IDataReaderPayload<Padrino>
    {
        public int FilialId { get; set; }

        public int CreatedByCoordinadorId { get; set; }
    }
}
