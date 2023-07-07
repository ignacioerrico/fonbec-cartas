using Fonbec.Cartas.DataAccess.Entities.Actors;
using Fonbec.Cartas.DataAccess.Entities.Enums;
using Mapster;

namespace Fonbec.Cartas.Logic.ViewModels.Coordinador
{
    public class BecariosListViewModel : AuditableWithAuthorNameViewModel
    {
        public int Id { get; set; }

        public string Mediador { get; set; } = default!;

        public List<string> PadrinosActivos { get; set; } = default!;
        
        public List<string> PadrinosFuturos { get; set; } = default!;

        public DateTime? LatestActiveAssignmentEndsOn { get; set; }

        public string NivelDeEstudio { get; set; } = default!;

        public string Name { get; set; } = default!;

        public string Email { get; set; } = default!;

        public string Phone { get; set; } = default!;

        public Gender Gender { get; set; }
    }

    public class BecariosListViewModelMappingDefinitions : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            Func<Becario, DateTime?> latestActiveAssignmentEndsOn = b =>
                !b.Apadrinamientos.Any(a => a.EsAsignaciónActiva)
                || b.Apadrinamientos.Where(a => a.EsAsignaciónActiva).Any(a => a.To is null)
                    ? null
                    : b.Apadrinamientos
                        .Where(a => a.EsAsignaciónActiva)
                        .Max(a => a.To);

            config.NewConfig<Becario, BecariosListViewModel>()
                .Map(dest => dest.Id, src => src.Id)
                .Map(dest => dest.Mediador, src => src.Mediador.FullName(false))
                .Map(dest => dest.PadrinosActivos,
                    src => src.Apadrinamientos.Where(a => a.EsAsignaciónActiva)
                        .Select(a => a.Padrino.FullName(false))
                        .OrderBy(name => name))
                .Map(dest => dest.PadrinosFuturos,
                    src => src.Apadrinamientos.Where(a => a.EsAsignaciónFutura)
                        .Select(a => a.Padrino.FullName(false))
                        .OrderBy(name => name))
                .Map(dest => dest.LatestActiveAssignmentEndsOn, src => latestActiveAssignmentEndsOn.Invoke(src))
                .Map(dest => dest.NivelDeEstudio, src => src.NivelDeEstudio.ToString())
                .Map(dest => dest.Name, src => src.FullName(true))
                .Map(dest => dest.Gender, src => src.Gender)
                .Map(dest => dest.Email, src => src.Email ?? string.Empty)
                .Map(dest => dest.Phone, src => src.Phone ?? string.Empty)
                .Map(dest => dest.CreatedOnUtc, src => src.CreatedOnUtc)
                .Map(dest => dest.CreatedBy, src => src.CreatedByCoordinador.FullName(false))
                .Map(dest => dest.UpdatedBy, src => src.UpdatedByCoordinador!.FullName(false),
                    srcCond => srcCond.UpdatedByCoordinador != null);
        }
    }
}
