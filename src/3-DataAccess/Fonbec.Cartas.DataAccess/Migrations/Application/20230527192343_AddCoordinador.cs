using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fonbec.Cartas.DataAccess.Migrations.Application
{
    /// <inheritdoc />
    public partial class AddCoordinador : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Coordinadores",
                schema: "model",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AspNetUserId = table.Column<string>(type: "nchar(36)", fixedLength: true, maxLength: 36, nullable: false),
                    Username = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
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
                    table.PrimaryKey("PK_Coordinadores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Coordinadores_Filiales_FilialId",
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
                value: new DateTimeOffset(new DateTime(2023, 5, 27, 19, 23, 43, 19, DateTimeKind.Unspecified).AddTicks(6995), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.CreateIndex(
                name: "IX_Coordinadores_FilialId",
                schema: "model",
                table: "Coordinadores",
                column: "FilialId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Coordinadores",
                schema: "model");

            migrationBuilder.UpdateData(
                schema: "model",
                table: "Filiales",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedOnUtc",
                value: new DateTimeOffset(new DateTime(2023, 5, 26, 0, 22, 13, 426, DateTimeKind.Unspecified).AddTicks(6683), new TimeSpan(0, 0, 0, 0, 0)));
        }
    }
}
