using Fonbec.Cartas.DataAccess.Entities;

namespace Fonbec.Cartas.DataAccess.DataModels.Coordinador
{
    public class ApadrinamientoEditDataModel
    {
        public bool BecarioExists { get; set; }
        
        public string? BecarioFullName { get; set; }

        public string? BecarioFirstName { get; set; }
        
        public List<SelectableDataModel> SelectablePadrinos { get; set; } = new();

        public List<Apadrinamiento> Apadrinamientos { get; set; } = new();
    }
}
