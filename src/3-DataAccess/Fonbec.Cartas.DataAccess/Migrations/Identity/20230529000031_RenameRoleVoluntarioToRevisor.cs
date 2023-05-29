using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Fonbec.Cartas.DataAccess.Migrations.Identity
{
    /// <inheritdoc />
    public partial class RenameRoleVoluntarioToRevisor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "25b59abc-63e8-4da9-82f6-5274f62129af", null, "Admin", "ADMIN" },
                    { "4b704585-53bd-4d7e-bacb-92145a2e58be", null, "Mediador", "MEDIADOR" },
                    { "60b64223-c4e0-45b2-8ca2-fbd5bf7616b7", null, "Revisor", "REVISOR" },
                    { "946d536c-44c3-4f22-92c9-da30f28ba8aa", null, "Coordinador", "COORDINADOR" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "25b59abc-63e8-4da9-82f6-5274f62129af");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4b704585-53bd-4d7e-bacb-92145a2e58be");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "60b64223-c4e0-45b2-8ca2-fbd5bf7616b7");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "946d536c-44c3-4f22-92c9-da30f28ba8aa");

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
    }
}
