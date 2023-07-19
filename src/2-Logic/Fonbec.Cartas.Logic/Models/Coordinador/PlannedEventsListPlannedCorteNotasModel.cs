using Fonbec.Cartas.DataAccess.Entities.Enums;
using Fonbec.Cartas.DataAccess.Entities.Planning;
using Mapster;

namespace Fonbec.Cartas.Logic.Models.Coordinador
{
    public class PlannedEventsListPlannedCorteNotasModel
    {
        public int PlannedCorteNotasId { get; set; }
        
        public DateTime Date { get; set; }

        public int FilialId { get; set; }

        public int CreatedByCoordinadorId { get; set; }
    }

    public class PlannedEventsListCreatePlannedCorteNotasModelMappingDefinitions : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<PlannedEventsListPlannedCorteNotasModel, PlannedEvent>()
                .Map(dest => dest.Id, src => src.PlannedCorteNotasId)
                .Map(dest => dest.Date, src => src.Date)
                .Map(dest => dest.Type, src => PlannedEventType.Notas)
                .Map(dest => dest.FilialId, src => src.FilialId)
                .Map(dest => dest.CreatedByCoordinadorId, src => src.CreatedByCoordinadorId);
        }
    }
}
