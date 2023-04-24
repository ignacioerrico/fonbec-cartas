namespace Fonbec.Cartas.Ui.Options
{
    public class AdminUserOptions
    {
        public const string SectionName = "AdminUser";

        public bool AttemptToCreateAdminUser { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
    }
}
