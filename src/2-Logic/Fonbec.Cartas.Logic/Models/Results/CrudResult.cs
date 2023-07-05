namespace Fonbec.Cartas.Logic.Models.Results
{
    public class CrudResult
    {
        public CrudResult(int rowsAffected = 0)
        {
            RowsAffected = rowsAffected;
        }

        public int RowsAffected { get; protected set; }

        public bool AnyRowsAffected => RowsAffected > 0;

        public virtual CrudResult SetRowsAffected(int rowsAffected)
        {
            RowsAffected = rowsAffected;
            return this;
        }
    }
}
