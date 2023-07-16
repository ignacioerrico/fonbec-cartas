using Fonbec.Cartas.DataAccess.Entities.Actors.Abstract;

namespace Fonbec.Cartas.Logic.Services.Admin.DataImport.DataReader.Payloads
{
    public class UserWithAccountPayload<T> : IDataReaderPayload<T> where T : UserWithAccount
    {
        public IUserWithAccountSharedService UserWithAccountService { get; set; } = default!;
    }
}
