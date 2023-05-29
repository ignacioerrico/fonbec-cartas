namespace Fonbec.Cartas.DataAccess.Projections;

public class FilialesListProjection
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public List<string> Coordinadores { get; set; } = new();

    public int QtyMediadores { get; set; }

    public int QtyRevisores { get; set; }

    public DateTimeOffset CreatedOnUtc { get; set; }

    public DateTimeOffset? LastUpdatedOnUtc { get; set; }
}