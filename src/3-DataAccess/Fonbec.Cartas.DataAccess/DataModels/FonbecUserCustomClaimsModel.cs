namespace Fonbec.Cartas.DataAccess.DataModels;

public class FonbecUserCustomClaimsModel
{
    public string UserWithAccountId { get; set; } = default!;

    public string Gender { get; set; } = default!;

    public string NickName { get; set; } = default!;

    public string FilialId { get; set; } = default!;
    
    public string FilialName { get; set; } = default!;
}