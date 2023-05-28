namespace Fonbec.Cartas.DataAccess.Constants
{
    public static class MaxLength
    {
        public static class Filial
        {
            public const int Name = 30;
        }

        public static class Actor
        {
            public const int FirstName = 30;
            public const int LastName = 30;
            public const int NickName = 16;
            public const int Email = 50;
            public const int Phone = 30;
            public const int AspNetUserId = 36; // Fixed (GUID)
            public const int Username = 20;
            public const int InitialPassword = 20;
        }
    }
}
