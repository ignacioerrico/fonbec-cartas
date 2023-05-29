namespace Fonbec.Cartas.DataAccess.Constants
{
    public static class FonbecRoles
    {
        public const string Admin = "Admin";
        public const string Coordinador = "Coordinador";
        public const string Mediador = "Mediador";
        public const string Revisor = "Revisor";

        public static string For(params string[] roles)
            => string.Join(",", roles);
    }
}
