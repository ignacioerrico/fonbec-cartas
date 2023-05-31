using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fonbec.Cartas.DataAccess.Migrations.Application
{
    /// <inheritdoc />
    public partial class AddPadrino : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Padrinos",
                schema: "model",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedByCoordinadorId = table.Column<int>(type: "int", nullable: false),
                    UpdatedByCoordinadorId = table.Column<int>(type: "int", nullable: true),
                    DeletedByCoordinadorId = table.Column<int>(type: "int", nullable: true),
                    CreatedOnUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdatedOnUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    SoftDeletedOnUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    FilialId = table.Column<int>(type: "int", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    NickName = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: true),
                    Gender = table.Column<byte>(type: "tinyint", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Padrinos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Padrinos_Coordinadores_CreatedByCoordinadorId",
                        column: x => x.CreatedByCoordinadorId,
                        principalSchema: "model",
                        principalTable: "Coordinadores",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Padrinos_Coordinadores_DeletedByCoordinadorId",
                        column: x => x.DeletedByCoordinadorId,
                        principalSchema: "model",
                        principalTable: "Coordinadores",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Padrinos_Coordinadores_UpdatedByCoordinadorId",
                        column: x => x.UpdatedByCoordinadorId,
                        principalSchema: "model",
                        principalTable: "Coordinadores",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Padrinos_Filiales_FilialId",
                        column: x => x.FilialId,
                        principalSchema: "model",
                        principalTable: "Filiales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SendAlsoTo",
                schema: "model",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RecipientFullName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RecipientEmail = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SendAsBcc = table.Column<bool>(type: "bit", nullable: false),
                    PadrinoId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SendAlsoTo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SendAlsoTo_Padrinos_PadrinoId",
                        column: x => x.PadrinoId,
                        principalSchema: "model",
                        principalTable: "Padrinos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                schema: "model",
                table: "Filiales",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedOnUtc",
                value: new DateTimeOffset(new DateTime(2023, 5, 31, 0, 43, 25, 794, DateTimeKind.Unspecified).AddTicks(3590), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.CreateIndex(
                name: "IX_Padrinos_CreatedByCoordinadorId",
                schema: "model",
                table: "Padrinos",
                column: "CreatedByCoordinadorId");

            migrationBuilder.CreateIndex(
                name: "IX_Padrinos_DeletedByCoordinadorId",
                schema: "model",
                table: "Padrinos",
                column: "DeletedByCoordinadorId");

            migrationBuilder.CreateIndex(
                name: "IX_Padrinos_FilialId",
                schema: "model",
                table: "Padrinos",
                column: "FilialId");

            migrationBuilder.CreateIndex(
                name: "IX_Padrinos_UpdatedByCoordinadorId",
                schema: "model",
                table: "Padrinos",
                column: "UpdatedByCoordinadorId");

            migrationBuilder.CreateIndex(
                name: "IX_SendAlsoTo_PadrinoId",
                schema: "model",
                table: "SendAlsoTo",
                column: "PadrinoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SendAlsoTo",
                schema: "model");

            migrationBuilder.DropTable(
                name: "Padrinos",
                schema: "model");

            migrationBuilder.UpdateData(
                schema: "model",
                table: "Filiales",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedOnUtc",
                value: new DateTimeOffset(new DateTime(2023, 5, 28, 20, 34, 46, 538, DateTimeKind.Unspecified).AddTicks(2095), new TimeSpan(0, 0, 0, 0, 0)));
        }
    }
}
