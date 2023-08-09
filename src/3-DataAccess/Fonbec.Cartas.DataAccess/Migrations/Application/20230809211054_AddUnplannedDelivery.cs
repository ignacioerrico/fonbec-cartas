using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fonbec.Cartas.DataAccess.Migrations.Application
{
    /// <inheritdoc />
    public partial class AddUnplannedDelivery : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UnplannedDelivery_Apadrinamientos_ApadrinamientoId",
                schema: "model",
                table: "UnplannedDelivery");

            migrationBuilder.DropForeignKey(
                name: "FK_UnplannedDelivery_Deadlines_DeadlineId",
                schema: "model",
                table: "UnplannedDelivery");

            migrationBuilder.DropForeignKey(
                name: "FK_UnplannedDelivery_Revisores_DeliveryApprovedByRevisorId",
                schema: "model",
                table: "UnplannedDelivery");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UnplannedDelivery",
                schema: "model",
                table: "UnplannedDelivery");

            migrationBuilder.RenameTable(
                name: "UnplannedDelivery",
                schema: "model",
                newName: "UnplannedDeliveries",
                newSchema: "model");

            migrationBuilder.RenameIndex(
                name: "IX_UnplannedDelivery_DeliveryApprovedByRevisorId",
                schema: "model",
                table: "UnplannedDeliveries",
                newName: "IX_UnplannedDeliveries_DeliveryApprovedByRevisorId");

            migrationBuilder.RenameIndex(
                name: "IX_UnplannedDelivery_DeadlineId",
                schema: "model",
                table: "UnplannedDeliveries",
                newName: "IX_UnplannedDeliveries_DeadlineId");

            migrationBuilder.RenameIndex(
                name: "IX_UnplannedDelivery_ApadrinamientoId",
                schema: "model",
                table: "UnplannedDeliveries",
                newName: "IX_UnplannedDeliveries_ApadrinamientoId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UnplannedDeliveries",
                schema: "model",
                table: "UnplannedDeliveries",
                column: "Id");

            migrationBuilder.UpdateData(
                schema: "model",
                table: "Filiales",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedOnUtc",
                value: new DateTimeOffset(new DateTime(2023, 8, 9, 21, 10, 53, 662, DateTimeKind.Unspecified).AddTicks(1797), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddForeignKey(
                name: "FK_UnplannedDeliveries_Apadrinamientos_ApadrinamientoId",
                schema: "model",
                table: "UnplannedDeliveries",
                column: "ApadrinamientoId",
                principalSchema: "model",
                principalTable: "Apadrinamientos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UnplannedDeliveries_Deadlines_DeadlineId",
                schema: "model",
                table: "UnplannedDeliveries",
                column: "DeadlineId",
                principalSchema: "model",
                principalTable: "Deadlines",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UnplannedDeliveries_Revisores_DeliveryApprovedByRevisorId",
                schema: "model",
                table: "UnplannedDeliveries",
                column: "DeliveryApprovedByRevisorId",
                principalSchema: "model",
                principalTable: "Revisores",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UnplannedDeliveries_Apadrinamientos_ApadrinamientoId",
                schema: "model",
                table: "UnplannedDeliveries");

            migrationBuilder.DropForeignKey(
                name: "FK_UnplannedDeliveries_Deadlines_DeadlineId",
                schema: "model",
                table: "UnplannedDeliveries");

            migrationBuilder.DropForeignKey(
                name: "FK_UnplannedDeliveries_Revisores_DeliveryApprovedByRevisorId",
                schema: "model",
                table: "UnplannedDeliveries");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UnplannedDeliveries",
                schema: "model",
                table: "UnplannedDeliveries");

            migrationBuilder.RenameTable(
                name: "UnplannedDeliveries",
                schema: "model",
                newName: "UnplannedDelivery",
                newSchema: "model");

            migrationBuilder.RenameIndex(
                name: "IX_UnplannedDeliveries_DeliveryApprovedByRevisorId",
                schema: "model",
                table: "UnplannedDelivery",
                newName: "IX_UnplannedDelivery_DeliveryApprovedByRevisorId");

            migrationBuilder.RenameIndex(
                name: "IX_UnplannedDeliveries_DeadlineId",
                schema: "model",
                table: "UnplannedDelivery",
                newName: "IX_UnplannedDelivery_DeadlineId");

            migrationBuilder.RenameIndex(
                name: "IX_UnplannedDeliveries_ApadrinamientoId",
                schema: "model",
                table: "UnplannedDelivery",
                newName: "IX_UnplannedDelivery_ApadrinamientoId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UnplannedDelivery",
                schema: "model",
                table: "UnplannedDelivery",
                column: "Id");

            migrationBuilder.UpdateData(
                schema: "model",
                table: "Filiales",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedOnUtc",
                value: new DateTimeOffset(new DateTime(2023, 8, 8, 16, 50, 37, 4, DateTimeKind.Unspecified).AddTicks(3585), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddForeignKey(
                name: "FK_UnplannedDelivery_Apadrinamientos_ApadrinamientoId",
                schema: "model",
                table: "UnplannedDelivery",
                column: "ApadrinamientoId",
                principalSchema: "model",
                principalTable: "Apadrinamientos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UnplannedDelivery_Deadlines_DeadlineId",
                schema: "model",
                table: "UnplannedDelivery",
                column: "DeadlineId",
                principalSchema: "model",
                principalTable: "Deadlines",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UnplannedDelivery_Revisores_DeliveryApprovedByRevisorId",
                schema: "model",
                table: "UnplannedDelivery",
                column: "DeliveryApprovedByRevisorId",
                principalSchema: "model",
                principalTable: "Revisores",
                principalColumn: "Id");
        }
    }
}
