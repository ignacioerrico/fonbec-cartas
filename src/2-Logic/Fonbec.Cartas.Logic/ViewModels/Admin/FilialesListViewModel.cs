using Fonbec.Cartas.DataAccess.DataModels.Admin;
using Mapster;

namespace Fonbec.Cartas.Logic.ViewModels.Admin
{
    public class FilialesListViewModel : AuditableViewModel
    {
        public FilialesListViewModel(string filialName)
        {
            FilialName = string.IsNullOrWhiteSpace(filialName)
                ? "(sin nombre)"
                : filialName;
        }

        public int FilialId { get; set; }
        
        public string FilialName { get; set; }
        
        public List<string> Coordinadores { get; set; } = new();
        
        public int QtyMediadores { get; set; }
        
        public int QtyBecarios { get; set; }
        
        public int QtyPadrinos { get; set; }
        
        public int QtyRevisores { get; set; }
    }

    public class MapFilialesListDataModelToFilialesListViewModel : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<FilialesListDataModel, FilialesListViewModel>()
                .ConstructUsing(src => new FilialesListViewModel(src.FilialName))
                .Map(dest => dest.FilialId, src => src.FilialId)
                .Map(dest => dest.Coordinadores, src => src.Coordinadores)
                .Map(dest => dest.QtyMediadores, src => src.QtyMediadores)
                .Map(dest => dest.QtyRevisores, src => src.QtyRevisores)
                .Map(dest => dest.QtyPadrinos, src => src.QtyPadrinos)
                .Map(dest => dest.QtyBecarios, src => src.QtyBecarios);
        }
    }
}
