namespace Fonbec.Cartas.Logic.Models.Results
{
    public class CrudDataResult<T> : CrudResult
    {
        public CrudDataResult(T data, int rowsAffected = 0) : base(rowsAffected)
        {
            Data = data;
        }

        public T Data { get; set; }
    }
}
