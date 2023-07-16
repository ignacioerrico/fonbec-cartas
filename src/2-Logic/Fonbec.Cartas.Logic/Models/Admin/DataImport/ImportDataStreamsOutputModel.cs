using Fonbec.Cartas.DataAccess.Entities;
using Fonbec.Cartas.DataAccess.Entities.Actors;
using Fonbec.Cartas.DataAccess.Entities.DataImport;

namespace Fonbec.Cartas.Logic.Models.Admin.DataImport
{
    public class ImportDataStreamsOutputModel
    {
        public List<string> Errors { get; set; } = new();

        public List<DataAccess.Entities.Actors.Coordinador> CoordinadoresCreated { get; set; } = default!;

        public List<Mediador> MediadoresCreated { get; set; } = default!;

        public List<Revisor> RevisoresCreated { get; set; } = default!;

        public List<Padrino> PadrinosCreated { get; set; } = default!;

        public List<PadrinoToUpdate> PadrinosUpdated { get; set; } = default!;

        public List<Becario> BecariosCreated { get; set; } = default!;

        public List<Apadrinamiento> ApadrinamientosCreated { get; set; } = default!;
    }
}
