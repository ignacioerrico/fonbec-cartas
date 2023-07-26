using Fonbec.Cartas.DataAccess.Entities;
using Mapster;

namespace Fonbec.Cartas.Logic.Models.Coordinador
{
    public class ApadrinamientoEditUpdateApadrinamientoModel
    {
        public int ApadrinamientoId { get; set; }

        public DateTime From { get; set; }

        public DateTime? To { get; set; }

        public int UpdatedByCoordinadorId { get; set; }
    }

    public class ApadrinamientoEditUpdateApadrinamientoModelMappingDefinitions : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<ApadrinamientoEditUpdateApadrinamientoModel, Apadrinamiento>()
                .Map(dest => dest.Id, src => src.ApadrinamientoId)
                .Map(dest => dest.From, src => src.From)
                .Map(dest => dest.To, src => src.To)
                .Map(dest => dest.UpdatedByCoordinadorId, src => src.UpdatedByCoordinadorId);
        }
    }
}
