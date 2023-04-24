using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Fonbec.Cartas.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class SeedRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "0019032a-b469-40d8-82f9-2b214591ac1d", null, "Admin", "ADMIN" },
                    { "0ad6cee9-6ef3-4761-8f60-23b22dd180c1", null, "Mediador", "MEDIADOR" },
                    { "1d9a9939-5c41-4e0b-9123-9350aa7270dd", null, "Voluntario", "VOLUNTARIO" },
                    { "3a147863-74fe-4063-bd3c-d2271b1a16bd", null, "Coordinador", "COORDINADOR" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0019032a-b469-40d8-82f9-2b214591ac1d");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0ad6cee9-6ef3-4761-8f60-23b22dd180c1");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1d9a9939-5c41-4e0b-9123-9350aa7270dd");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3a147863-74fe-4063-bd3c-d2271b1a16bd");
        }
    }
}
