using Fonbec.Cartas.DataAccess.Entities.Enums;

namespace Fonbec.Cartas.DataAccess.Entities.Actors.Abstract
{
    public abstract class EntityBase : Auditable
    {
        public int Id { get; set; }

        public int FilialId { get; set; }
        public Filial Filial { get; set; } = default!;

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string? NickName { get; set; }

        public Gender Gender { get; set; }

        public string? Phone { get; set; }

        public string FullName(bool includeNickName = false)
        {
            var values = new List<string>
            {
                FirstName,
                LastName
            };

            if (includeNickName && !string.IsNullOrWhiteSpace(NickName))
            {
                values.Add($"(\"{NickName}\")");
            }

            return string.Join(" ", values);
        }
    }
}
