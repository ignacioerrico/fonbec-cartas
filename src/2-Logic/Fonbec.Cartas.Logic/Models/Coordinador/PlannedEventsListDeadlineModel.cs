using Fonbec.Cartas.DataAccess.Entities.Planning;
using Mapster;

namespace Fonbec.Cartas.Logic.Models.Coordinador
{
    public class PlannedEventsListDeadlineModel
    {
        public int DeadlineId { get; set; }
        
        public DateTime Date { get; set; }

        public int FilialId { get; set; }

        public int CoordinadorId { get; set; }
    }

    public class PlannedEventsListDeadlineModelMappingDefinitions : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<PlannedEventsListDeadlineModel, Deadline>()
                .Map(dest => dest.Id, src => src.DeadlineId)
                .Map(dest => dest.Date, src => src.Date)
                .Map(dest => dest.FilialId, src => src.FilialId)
                .Map(dest => dest.CreatedByCoordinadorId, src => src.CoordinadorId)
                .Map(dest => dest.UpdatedByCoordinadorId, src => src.CoordinadorId);
        }
    }
}
