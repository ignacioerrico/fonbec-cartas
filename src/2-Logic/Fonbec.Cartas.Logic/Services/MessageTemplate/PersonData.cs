using Fonbec.Cartas.DataAccess.Entities.Enums;

namespace Fonbec.Cartas.Logic.Services.MessageTemplate;

public class PersonData
{
    public PersonData(string name, Gender gender)
    {
        Name = name;
        Gender = gender;
    }

    public string Name { get; set; }

    public Gender Gender { get; set; }
}