namespace Fonbec.Cartas.Logic.Models.Admin.DataImport
{
    public class ImportDataStreamsInputModel
    {
        public bool IsDryRun { get; set; }

        public int FilialId { get; set; }

        public int CreatedByCoordinadorId { get; set; }

        public Stream Apadrinamientos { get; set; } = default!;

        public Stream Becarios { get; set; } = default!;

        public Stream Coordinadores { get; set; } = default!;

        public Stream EnviarCopia { get; set; } = default!;

        public Stream Mediadores { get; set; } = default!;

        public Stream Padrinos { get; set; } = default!;

        public Stream Revisores { get; set; } = default!;
    }
}
