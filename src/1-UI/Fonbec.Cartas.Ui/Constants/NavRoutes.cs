﻿namespace Fonbec.Cartas.Ui.Constants
{
    public static class NavRoutes
    {
        public const string AdminRoot = "/admin";
        public const string CoordinadorRoot = "/coordinador";

        public const string AdminFiliales = $"{AdminRoot}/filiales";
        public const string AdminCoordinadores = $"{AdminRoot}/coordinadores";
        public const string AdminMediadores = $"{AdminRoot}/mediadores";
        public const string AdminRevisores = $"{AdminRoot}/revisores";
        public const string CoordinadorPadrinos = $"{CoordinadorRoot}/padrinos";
        public const string CoordinadorBecarios = $"{CoordinadorRoot}/becarios";
        public const string CoordinadorPlanes = $"{CoordinadorRoot}/planes";

        public const string New = "new";

        public const string AdminFilialNew = $"{AdminFiliales}/{New}";

        public const string AdminCoordinadorNew = $"{AdminCoordinadores}/{New}";
        public const string AdminMediadorNew = $"{AdminMediadores}/{New}";
        public const string AdminRevisorNew = $"{AdminRevisores}/{New}";
        public const string CoordinadorPadrinoNew = $"{CoordinadorPadrinos}/{New}";
        public const string CoordinadorBecariosNew = $"{CoordinadorBecarios}/{New}";
        public const string CoordinadorPlanNew = $"{CoordinadorPlanes}/{New}";

        public const string AdminFilialEdit_0 = $"{AdminFiliales}/{{0}}";

        public const string AdminCoordinadorEdit_0 = $"{AdminCoordinadores}/{{0}}";
        public const string AdminMediadorEdit_0 = $"{AdminMediadores}/{{0}}";
        public const string AdminRevisorEdit_0 = $"{AdminRevisores}/{{0}}";
        public const string CoordinadorPadrinoEdit_0 = $"{CoordinadorPadrinos}/{{0}}";
        public const string CoordinadorBecariosEdit_0 = $"{CoordinadorBecarios}/{{0}}";
        public const string CoordinadorBecario_0_AsignarPadrinos = $"{CoordinadorBecarios}/{{0}}/padrinos";
        public const string CoordinadorPlanEdit_0 = $"{CoordinadorPlanes}/{{0}}";
    }
}
