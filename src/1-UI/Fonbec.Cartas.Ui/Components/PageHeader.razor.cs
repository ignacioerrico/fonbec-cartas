using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Components;

namespace Fonbec.Cartas.Ui.Components
{
    public partial class PageHeader
    {
        [Parameter]
        public string? Title { get; set; }

        protected override void OnParametersSet()
        {
            Title ??= string.Empty;
        }
    }
}
