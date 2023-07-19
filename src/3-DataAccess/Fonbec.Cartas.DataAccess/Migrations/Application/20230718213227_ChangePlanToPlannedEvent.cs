using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fonbec.Cartas.DataAccess.Migrations.Application
{
    /// <inheritdoc />
    public partial class ChangePlanToPlannedEvent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Planes",
                schema: "model");

            migrationBuilder.CreateTable(
                name: "CartaObligatoria",
                schema: "model",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Subject = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    MessageMarkdown = table.Column<string>(type: "nvarchar(max)", maxLength: 4096, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartaObligatoria", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PlannedEvents",
                schema: "model",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FilialId = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Type = table.Column<byte>(type: "tinyint", nullable: false),
                    CartaObligatoriaId = table.Column<int>(type: "int", nullable: true),
                    CreatedByCoordinadorId = table.Column<int>(type: "int", nullable: false),
                    UpdatedByCoordinadorId = table.Column<int>(type: "int", nullable: true),
                    DeletedByCoordinadorId = table.Column<int>(type: "int", nullable: true),
                    CreatedOnUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdatedOnUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    SoftDeletedOnUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlannedEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlannedEvents_CartaObligatoria_CartaObligatoriaId",
                        column: x => x.CartaObligatoriaId,
                        principalSchema: "model",
                        principalTable: "CartaObligatoria",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlannedEvents_Coordinadores_CreatedByCoordinadorId",
                        column: x => x.CreatedByCoordinadorId,
                        principalSchema: "model",
                        principalTable: "Coordinadores",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PlannedEvents_Coordinadores_DeletedByCoordinadorId",
                        column: x => x.DeletedByCoordinadorId,
                        principalSchema: "model",
                        principalTable: "Coordinadores",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PlannedEvents_Coordinadores_UpdatedByCoordinadorId",
                        column: x => x.UpdatedByCoordinadorId,
                        principalSchema: "model",
                        principalTable: "Coordinadores",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PlannedEvents_Filiales_FilialId",
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
                value: new DateTimeOffset(new DateTime(2023, 7, 18, 21, 32, 27, 44, DateTimeKind.Unspecified).AddTicks(2557), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.CreateIndex(
                name: "IX_PlannedEvents_CartaObligatoriaId",
                schema: "model",
                table: "PlannedEvents",
                column: "CartaObligatoriaId",
                unique: true,
                filter: "[CartaObligatoriaId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_PlannedEvents_CreatedByCoordinadorId",
                schema: "model",
                table: "PlannedEvents",
                column: "CreatedByCoordinadorId");

            migrationBuilder.CreateIndex(
                name: "IX_PlannedEvents_DeletedByCoordinadorId",
                schema: "model",
                table: "PlannedEvents",
                column: "DeletedByCoordinadorId");

            migrationBuilder.CreateIndex(
                name: "IX_PlannedEvents_FilialId",
                schema: "model",
                table: "PlannedEvents",
                column: "FilialId");

            migrationBuilder.CreateIndex(
                name: "IX_PlannedEvents_UpdatedByCoordinadorId",
                schema: "model",
                table: "PlannedEvents",
                column: "UpdatedByCoordinadorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlannedEvents",
                schema: "model");

            migrationBuilder.DropTable(
                name: "CartaObligatoria",
                schema: "model");

            migrationBuilder.CreateTable(
                name: "Planes",
                schema: "model",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedByCoordinadorId = table.Column<int>(type: "int", nullable: false),
                    DeletedByCoordinadorId = table.Column<int>(type: "int", nullable: true),
                    FilialId = table.Column<int>(type: "int", nullable: false),
                    UpdatedByCoordinadorId = table.Column<int>(type: "int", nullable: true),
                    CreatedOnUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdatedOnUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    MessageMarkdown = table.Column<string>(type: "nvarchar(max)", maxLength: 4096, nullable: false),
                    SoftDeletedOnUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false)
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
                value: new DateTimeOffset(new DateTime(2023, 7, 8, 23, 27, 58, 276, DateTimeKind.Unspecified).AddTicks(1277), new TimeSpan(0, 0, 0, 0, 0)));

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
    }
}
