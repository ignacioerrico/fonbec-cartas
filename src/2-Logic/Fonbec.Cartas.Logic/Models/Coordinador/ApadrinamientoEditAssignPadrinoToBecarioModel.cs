﻿using Fonbec.Cartas.DataAccess.Entities;
using Mapster;

namespace Fonbec.Cartas.Logic.Models.Coordinador
{
    public class ApadrinamientoEditAssignPadrinoToBecarioModel
    {
        public int BecarioId { get; set; }

        public int PadrinoId { get; set; }

        public DateTime From { get; set; }

        public DateTime? To { get; set; }

        public int CreatedByCoordinadorId { get; set; }
    }

    public class ApadrinamientoEditAssignPadrinoToBecarioModelMappingDefinitions : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<ApadrinamientoEditAssignPadrinoToBecarioModel, Apadrinamiento>()
                .Map(dest => dest.BecarioId, src => src.BecarioId)
                .Map(dest => dest.PadrinoId, src => src.PadrinoId)
                .Map(dest => dest.From, src => src.From)
                .Map(dest => dest.To, src => src.To)
                .Map(dest => dest.CreatedByCoordinadorId, src => src.CreatedByCoordinadorId);
        }
    }
}
