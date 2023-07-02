namespace Fonbec.Cartas.DataAccess.DataModels.Admin;

public class FilialesListDataModel
{
    public int FilialId { get; set; }

    public string FilialName { get; set; } = string.Empty;

    public List<string> Coordinadores { get; set; } = new();

    public int QtyMediadores { get; set; }

    public int QtyRevisores { get; set; }

    public int QtyPadrinos { get; set; }

    public int QtyBecarios { get; set; }

    public DateTimeOffset CreatedOnUtc { get; set; }

    public DateTimeOffset? LastUpdatedOnUtc { get; set; }
}