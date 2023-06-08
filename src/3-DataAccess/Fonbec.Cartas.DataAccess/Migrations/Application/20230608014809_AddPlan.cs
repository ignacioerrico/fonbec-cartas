using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fonbec.Cartas.DataAccess.Migrations.Application
{
    /// <inheritdoc />
    public partial class AddPlan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Planes",
                schema: "model",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FilialId = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    MessageMarkdown = table.Column<string>(type: "nvarchar(max)", maxLength: 4096, nullable: false),
                    CreatedByCoordinadorId = table.Column<int>(type: "int", nullable: false),
                    UpdatedByCoordinadorId = table.Column<int>(type: "int", nullable: true),
                    DeletedByCoordinadorId = table.Column<int>(type: "int", nullable: true),
                    CreatedOnUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdatedOnUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    SoftDeletedOnUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Planes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Planes_Coordinadores_CreatedByCoordinadorId",
                        column: x => x.CreatedByCoordinadorId,
                        principalSchema: "model",
                        principalTable: "Coordinadores",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Planes_Coordinadores_DeletedByCoordinadorId",
                        column: x => x.DeletedByCoordinadorId,
                        principalSchema: "model",
                        principalTable: "Coordinadores",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Planes_Coordinadores_UpdatedByCoordinadorId",
                        column: x => x.UpdatedByCoordinadorId,
                        principalSchema: "model",
                        principalTable: "Coordinadores",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Planes_Filiales_FilialId",
                        column: x => x.FilialId,
                        principalSchema: "model",
                        principalTable: "Filiales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                schema: "model",
                table: "Filiales",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedOnUtc",
                value: new DateTimeOffset(new DateTime(2023, 6, 8, 1, 48, 8, 582, DateTimeKind.Unspecified).AddTicks(5830), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.CreateIndex(
                name: "IX_Planes_CreatedByCoordinadorId",
                schema: "model",
                table: "Planes",
                column: "CreatedByCoordinadorId");

            migrationBuilder.CreateIndex(
                name: "IX_Planes_DeletedByCoordinadorId",
                schema: "model",
                table: "Planes",
                column: "DeletedByCoordinadorId");

            migrationBuilder.CreateIndex(
                name: "IX_Planes_FilialId",
                schema: "model",
                table: "Planes",
                column: "FilialId");

            migrationBuilder.CreateIndex(
                name: "IX_Planes_UpdatedByCoordinadorId",
                schema: "model",
                table: "Planes",
                column: "UpdatedByCoordinadorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Planes",
                schema: "model");

            migrationBuilder.UpdateData(
                schema: "model",
                table: "Filiales",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedOnUtc",
                value: new DateTimeOffset(new DateTime(2023, 6, 3, 16, 32, 41, 225, DateTimeKind.Unspecified).AddTicks(4502), new TimeSpan(0, 0, 0, 0, 0)));
        }
    }
}
