using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fonbec.Cartas.DataAccess.Migrations.Application
{
    /// <inheritdoc />
    public partial class SeparatePlannedEventAndDeadline : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlannedEvents_CartaObligatoria_CartaObligatoriaId",
                schema: "model",
                table: "PlannedEvents");

            migrationBuilder.DropTable(
                name: "CartaObligatoria",
                schema: "model");

            migrationBuilder.DropIndex(
                name: "IX_PlannedEvents_CartaObligatoriaId",
                schema: "model",
                table: "PlannedEvents");

            migrationBuilder.DropColumn(
                name: "CartaObligatoriaId",
                schema: "model",
                table: "PlannedEvents");

            migrationBuilder.DropColumn(
                name: "Type",
                schema: "model",
                table: "PlannedEvents");

            migrationBuilder.RenameColumn(
                name: "Date",
                schema: "model",
                table: "PlannedEvents",
                newName: "StartsOn");

            migrationBuilder.AddColumn<string>(
                name: "MessageMarkdown",
                schema: "model",
                table: "PlannedEvents",
                type: "nvarchar(max)",
                maxLength: 4096,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Subject",
                schema: "model",
                table: "PlannedEvents",
                type: "nvarchar(120)",
                maxLength: 120,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Deadlines",
                schema: "model",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FilialId = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByCoordinadorId = table.Column<int>(type: "int", nullable: false),
                    UpdatedByCoordinadorId = table.Column<int>(type: "int", nullable: true),
                    DeletedByCoordinadorId = table.Column<int>(type: "int", nullable: true),
                    CreatedOnUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdatedOnUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    SoftDeletedOnUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Deadlines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Deadlines_Coordinadores_CreatedByCoordinadorId",
                        column: x => x.CreatedByCoordinadorId,
                        principalSchema: "model",
                        principalTable: "Coordinadores",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Deadlines_Coordinadores_DeletedByCoordinadorId",
                        column: x => x.DeletedByCoordinadorId,
                        principalSchema: "model",
                        principalTable: "Coordinadores",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Deadlines_Coordinadores_UpdatedByCoordinadorId",
                        column: x => x.UpdatedByCoordinadorId,
                        principalSchema: "model",
                        principalTable: "Coordinadores",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Deadlines_Filiales_FilialId",
                        column: x => x.FilialId,
                        principalSchema: "model",
                        principalTable: "Filiales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UnplannedDelivery",
                schema: "model",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DeadlineId = table.Column<int>(type: "int", nullable: false),
                    ApadrinamientoId = table.Column<int>(type: "int", nullable: false),
                    DeliveryApprovedByRevisorId = table.Column<int>(type: "int", nullable: true),
                    SentOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnplannedDelivery", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UnplannedDelivery_Apadrinamientos_ApadrinamientoId",
                        column: x => x.ApadrinamientoId,
                        principalSchema: "model",
                        principalTable: "Apadrinamientos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UnplannedDelivery_Deadlines_DeadlineId",
                        column: x => x.DeadlineId,
                        principalSchema: "model",
                        principalTable: "Deadlines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UnplannedDelivery_Revisores_DeliveryApprovedByRevisorId",
                        column: x => x.DeliveryApprovedByRevisorId,
                        principalSchema: "model",
                        principalTable: "Revisores",
                        principalColumn: "Id");
                });

            migrationBuilder.UpdateData(
                schema: "model",
                table: "Filiales",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedOnUtc",
                value: new DateTimeOffset(new DateTime(2023, 8, 8, 16, 50, 37, 4, DateTimeKind.Unspecified).AddTicks(3585), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.CreateIndex(
                name: "IX_Deadlines_CreatedByCoordinadorId",
                schema: "model",
                table: "Deadlines",
                column: "CreatedByCoordinadorId");

            migrationBuilder.CreateIndex(
                name: "IX_Deadlines_DeletedByCoordinadorId",
                schema: "model",
                table: "Deadlines",
                column: "DeletedByCoordinadorId");

            migrationBuilder.CreateIndex(
                name: "IX_Deadlines_FilialId",
                schema: "model",
                table: "Deadlines",
                column: "FilialId");

            migrationBuilder.CreateIndex(
                name: "IX_Deadlines_UpdatedByCoordinadorId",
                schema: "model",
                table: "Deadlines",
                column: "UpdatedByCoordinadorId");

            migrationBuilder.CreateIndex(
                name: "IX_UnplannedDelivery_ApadrinamientoId",
                schema: "model",
                table: "UnplannedDelivery",
                column: "ApadrinamientoId");

            migrationBuilder.CreateIndex(
                name: "IX_UnplannedDelivery_DeadlineId",
                schema: "model",
                table: "UnplannedDelivery",
                column: "DeadlineId");

            migrationBuilder.CreateIndex(
                name: "IX_UnplannedDelivery_DeliveryApprovedByRevisorId",
                schema: "model",
                table: "UnplannedDelivery",
                column: "DeliveryApprovedByRevisorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UnplannedDelivery",
                schema: "model");

            migrationBuilder.DropTable(
                name: "Deadlines",
                schema: "model");

            migrationBuilder.DropColumn(
                name: "MessageMarkdown",
                schema: "model",
                table: "PlannedEvents");

            migrationBuilder.DropColumn(
                name: "Subject",
                schema: "model",
                table: "PlannedEvents");

            migrationBuilder.RenameColumn(
                name: "StartsOn",
                schema: "model",
                table: "PlannedEvents",
                newName: "Date");

            migrationBuilder.AddColumn<int>(
                name: "CartaObligatoriaId",
                schema: "model",
                table: "PlannedEvents",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "Type",
                schema: "model",
                table: "PlannedEvents",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.CreateTable(
                name: "CartaObligatoria",
                schema: "model",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MessageMarkdown = table.Column<string>(type: "nvarchar(max)", maxLength: 4096, nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartaObligatoria", x => x.Id);
                });

            migrationBuilder.UpdateData(
                schema: "model",
                table: "Filiales",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedOnUtc",
                value: new DateTimeOffset(new DateTime(2023, 8, 3, 2, 16, 14, 295, DateTimeKind.Unspecified).AddTicks(922), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.CreateIndex(
                name: "IX_PlannedEvents_CartaObligatoriaId",
                schema: "model",
                table: "PlannedEvents",
                column: "CartaObligatoriaId",
                unique: true,
                filter: "[CartaObligatoriaId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_PlannedEvents_CartaObligatoria_CartaObligatoriaId",
                schema: "model",
                table: "PlannedEvents",
                column: "CartaObligatoriaId",
                principalSchema: "model",
                principalTable: "CartaObligatoria",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
