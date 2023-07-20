using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fonbec.Cartas.DataAccess.Migrations.Application
{
    /// <inheritdoc />
    public partial class AddPlannedDelivery : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PlannedDeliveries",
                schema: "model",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlannedEventId = table.Column<int>(type: "int", nullable: false),
                    FromBecarioId = table.Column<int>(type: "int", nullable: false),
                    ToPadrinoId = table.Column<int>(type: "int", nullable: false),
                    SentOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlannedDeliveries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlannedDeliveries_Becarios_FromBecarioId",
                        column: x => x.FromBecarioId,
                        principalSchema: "model",
                        principalTable: "Becarios",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PlannedDeliveries_Padrinos_ToPadrinoId",
                        column: x => x.ToPadrinoId,
                        principalSchema: "model",
                        principalTable: "Padrinos",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PlannedDeliveries_PlannedEvents_PlannedEventId",
                        column: x => x.PlannedEventId,
                        principalSchema: "model",
                        principalTable: "PlannedEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                schema: "model",
                table: "Filiales",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedOnUtc",
                value: new DateTimeOffset(new DateTime(2023, 7, 20, 2, 17, 30, 796, DateTimeKind.Unspecified).AddTicks(1741), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.CreateIndex(
                name: "IX_PlannedDeliveries_FromBecarioId",
                schema: "model",
                table: "PlannedDeliveries",
                column: "FromBecarioId");

            migrationBuilder.CreateIndex(
                name: "IX_PlannedDeliveries_PlannedEventId",
                schema: "model",
                table: "PlannedDeliveries",
                column: "PlannedEventId");

            migrationBuilder.CreateIndex(
                name: "IX_PlannedDeliveries_ToPadrinoId",
                schema: "model",
                table: "PlannedDeliveries",
                column: "ToPadrinoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlannedDeliveries",
                schema: "model");

            migrationBuilder.UpdateData(
                schema: "model",
                table: "Filiales",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedOnUtc",
                value: new DateTimeOffset(new DateTime(2023, 7, 18, 21, 32, 27, 44, DateTimeKind.Unspecified).AddTicks(2557), new TimeSpan(0, 0, 0, 0, 0)));
        }
    }
}
