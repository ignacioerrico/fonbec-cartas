﻿using Fonbec.Cartas.Logic.ViewModels;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Fonbec.Cartas.Ui.Components
{
    public partial class AuditableItemDisplay
    {
        [Parameter]
        public int Id { get; set; }

        [Parameter]
        public string Name { get; set; } = string.Empty;

        [Parameter]
        public string? Email { get; set; }

        [Parameter]
        public Color Color { get; set; } = Color.Primary;

        [Parameter]
        public AuditableViewModel AuditableViewModel { get; set; } = default!;
    }
}
