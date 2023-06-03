using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fonbec.Cartas.DataAccess.Migrations.Application
{
    /// <inheritdoc />
    public partial class AddApadrinamiento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Apadrinamientos",
                schema: "model",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BecarioId = table.Column<int>(type: "int", nullable: false),
                    PadrinoId = table.Column<int>(type: "int", nullable: false),
                    From = table.Column<DateTime>(type: "datetime2", nullable: false),
                    To = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedByCoordinadorId = table.Column<int>(type: "int", nullable: false),
                    UpdatedByCoordinadorId = table.Column<int>(type: "int", nullable: true),
                    DeletedByCoordinadorId = table.Column<int>(type: "int", nullable: true),
                    CreatedOnUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdatedOnUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    SoftDeletedOnUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Apadrinamientos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Apadrinamientos_Becarios_BecarioId",
                        column: x => x.BecarioId,
                        principalSchema: "model",
                        principalTable: "Becarios",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Apadrinamientos_Coordinadores_CreatedByCoordinadorId",
                        column: x => x.CreatedByCoordinadorId,
                        principalSchema: "model",
                        principalTable: "Coordinadores",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Apadrinamientos_Coordinadores_DeletedByCoordinadorId",
                        column: x => x.DeletedByCoordinadorId,
                        principalSchema: "model",
                        principalTable: "Coordinadores",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Apadrinamientos_Coordinadores_UpdatedByCoordinadorId",
                        column: x => x.UpdatedByCoordinadorId,
                        principalSchema: "model",
                        principalTable: "Coordinadores",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Apadrinamientos_Padrinos_PadrinoId",
                        column: x => x.PadrinoId,
                        principalSchema: "model",
                        principalTable: "Padrinos",
                        principalColumn: "Id");
                });

            migrationBuilder.UpdateData(
                schema: "model",
                table: "Filiales",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedOnUtc",
                value: new DateTimeOffset(new DateTime(2023, 6, 3, 16, 32, 41, 225, DateTimeKind.Unspecified).AddTicks(4502), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.CreateIndex(
                name: "IX_Apadrinamientos_BecarioId",
                schema: "model",
                table: "Apadrinamientos",
                column: "BecarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Apadrinamientos_CreatedByCoordinadorId",
                schema: "model",
                table: "Apadrinamientos",
                column: "CreatedByCoordinadorId");

            migrationBuilder.CreateIndex(
                name: "IX_Apadrinamientos_DeletedByCoordinadorId",
                schema: "model",
                table: "Apadrinamientos",
                column: "DeletedByCoordinadorId");

            migrationBuilder.CreateIndex(
                name: "IX_Apadrinamientos_PadrinoId",
                schema: "model",
                table: "Apadrinamientos",
                column: "PadrinoId");

            migrationBuilder.CreateIndex(
                name: "IX_Apadrinamientos_UpdatedByCoordinadorId",
                schema: "model",
                table: "Apadrinamientos",
                column: "UpdatedByCoordinadorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Apadrinamientos",
                schema: "model");

            migrationBuilder.UpdateData(
                schema: "model",
                table: "Filiales",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedOnUtc",
                value: new DateTimeOffset(new DateTime(2023, 6, 1, 18, 20, 31, 934, DateTimeKind.Unspecified).AddTicks(1195), new TimeSpan(0, 0, 0, 0, 0)));
        }
    }
}
