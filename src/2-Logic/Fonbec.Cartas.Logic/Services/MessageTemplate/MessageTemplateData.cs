namespace Fonbec.Cartas.Logic.Services.MessageTemplate;

public class MessageTemplateData
{
    public DateTime Date { get; set; }

    public string Documents { get; set; } = default!;

    public PersonData Padrino { get; set; } = default!;

    public PersonData Becario { get; set; } = default!;
}