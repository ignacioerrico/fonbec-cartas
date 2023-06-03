using Fonbec.Cartas.DataAccess.Entities.Enums;

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
}
