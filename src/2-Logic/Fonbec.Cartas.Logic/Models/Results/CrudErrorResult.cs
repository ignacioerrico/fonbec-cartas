namespace Fonbec.Cartas.Logic.Models.Results
{
    public class CrudErrorResult : CrudResult
    {
        public List<string> Errors { get; } = new();

        public bool AnyErrors => Errors.Any();

        public CrudErrorResult AddError(string error)
        {
            Errors.Add(error);
            return this;
        }

        public CrudErrorResult AddErrors(IEnumerable<string> errors)
        {
            Errors.AddRange(errors);
            return this;
        }

        public override CrudErrorResult SetRowsAffected(int rowsAffected)
        {
            return (CrudErrorResult)base.SetRowsAffected(rowsAffected);
        }
    }
}
