namespace Fonbec.Cartas.Logic.Models.Results
{
    public class CrudResult
    {
        public CrudResult(int rowsAffected)
        {
            RowsAffected = rowsAffected;
        }

        public int RowsAffected { get; }

        public bool IsSuccess => RowsAffected > 0;
    }
}
