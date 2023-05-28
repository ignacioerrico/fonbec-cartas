namespace Fonbec.Cartas.Ui.Constants
{
    public static class NavRoutes
    {
        public const string AdminFilialesList = "/admin/filiales";
        
        public const string AdminCoordinadoresList = "/admin/coordinadores";
        public const string AdminMediadoresList = "/admin/mediadores";
        public const string AdminRevisoresList = "/admin/revisores";

        public const string New = "new";

        public const string AdminCoordinadorNew = $"{AdminCoordinadoresList}/{New}";
        public const string AdminMediadorNew = $"{AdminMediadoresList}/{New}";
        public const string AdminRevisorNew = $"/admin/revisores/{New}";

        public const string AdminCoordinadorEdit_0 = $"{AdminCoordinadoresList}/{{0}}";
        public const string AdminMediadorEdit_0 = $"{AdminMediadoresList}/{{0}}";
        public const string AdminRevisorEdit_0 = $"{AdminRevisoresList}/{{0}}";
    }
}
