namespace Fonbec.Cartas.Ui.Constants
{
    public static class NavRoutes
    {
        public const string Root = "/";

        public const string AdminRoot = "/admin";
        public const string CoordinadorRoot = "/coordinador";

        public const string AdminFiliales = $"{AdminRoot}/filiales";
        public const string AdminCoordinadores = $"{AdminRoot}/coordinadores";
        public const string AdminMediadores = $"{AdminRoot}/mediadores";
        public const string AdminRevisores = $"{AdminRoot}/revisores";
        public const string AdminImport = $"{AdminRoot}/import";
        public const string CoordinadorPadrinos = $"{CoordinadorRoot}/padrinos";
        public const string CoordinadorBecarios = $"{CoordinadorRoot}/becarios";
        public const string CoordinadorPlanificación = $"{CoordinadorRoot}/planes";

        public const string New = "new";

        public const string AdminFilialNew = $"{AdminFiliales}/{New}";

        public const string AdminCoordinadorNew = $"{AdminCoordinadores}/{New}";
        public const string AdminMediadorNew = $"{AdminMediadores}/{New}";
        public const string AdminRevisorNew = $"{AdminRevisores}/{New}";
        public const string CoordinadorPadrinoNew = $"{CoordinadorPadrinos}/{New}";
        public const string CoordinadorBecariosNew = $"{CoordinadorBecarios}/{New}";
        public const string CoordinadorPlanificaciónNew = $"{CoordinadorPlanificación}/{New}";

        public const string AdminFilialEdit0 = $"{AdminFiliales}/{{0}}";

        public const string AdminCoordinadorEdit0 = $"{AdminCoordinadores}/{{0}}";
        public const string AdminMediadorEdit0 = $"{AdminMediadores}/{{0}}";
        public const string AdminRevisorEdit0 = $"{AdminRevisores}/{{0}}";
        public const string CoordinadorPadrinoEdit0 = $"{CoordinadorPadrinos}/{{0}}";
        public const string CoordinadorBecariosEdit0 = $"{CoordinadorBecarios}/{{0}}";
        public const string CoordinadorBecario0AsignarPadrinos = $"{CoordinadorBecarios}/{{0}}/padrinos";
        public const string CoordinadorPlanificaciónCartaEdit0 = $"{CoordinadorPlanificación}/{{0}}";
        
        public const string CoordinadorPlanificaciónCartaPreview0 = $"{CoordinadorPlanificación}/{{0}}/ver";
    }
}
