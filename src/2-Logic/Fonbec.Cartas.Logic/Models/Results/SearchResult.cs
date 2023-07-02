namespace Fonbec.Cartas.Logic.Models.Results
{
    public class SearchResult<T>
    {
        public SearchResult(T? data)
        {
            Data = data;
            IsFound = data is not null;
        }

        public T? Data { get; }

        public bool IsFound { get; }
    }
}
