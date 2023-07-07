namespace Fonbec.Cartas.DataAccess.DataModels
{
    public class SelectableDataModel
    {
        public SelectableDataModel(int id, string displayName)
        {
            Id = id;
            DisplayName = displayName;
        }

        public int Id { get; }

        public string DisplayName { get; }
    }
}
