using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fonbec.Cartas.DataAccess.Migrations.Application
{
    /// <inheritdoc />
    public partial class UseApadrinamientoInPlannedDeliveriesAndAddWhoApprovedIt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlannedDeliveries_Becarios_FromBecarioId",
                schema: "model",
                table: "PlannedDeliveries");

            migrationBuilder.DropForeignKey(
                name: "FK_PlannedDeliveries_Padrinos_ToPadrinoId",
                schema: "model",
                table: "PlannedDeliveries");

            migrationBuilder.DropIndex(
                name: "IX_PlannedDeliveries_FromBecarioId",
                schema: "model",
                table: "PlannedDeliveries");

            migrationBuilder.DropColumn(
                name: "FromBecarioId",
                schema: "model",
                table: "PlannedDeliveries");

            migrationBuilder.RenameColumn(
                name: "ToPadrinoId",
                schema: "model",
                table: "PlannedDeliveries",
                newName: "ApadrinamientoId");

            migrationBuilder.RenameIndex(
                name: "IX_PlannedDeliveries_ToPadrinoId",
                schema: "model",
                table: "PlannedDeliveries",
                newName: "IX_PlannedDeliveries_ApadrinamientoId");

            migrationBuilder.AddColumn<int>(
                name: "DeliveryApprovedByRevisorId",
                schema: "model",
                table: "PlannedDeliveries",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                schema: "model",
                table: "Filiales",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedOnUtc",
                value: new DateTimeOffset(new DateTime(2023, 8, 3, 2, 16, 14, 295, DateTimeKind.Unspecified).AddTicks(922), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.CreateIndex(
                name: "IX_PlannedDeliveries_DeliveryApprovedByRevisorId",
                schema: "model",
                table: "PlannedDeliveries",
                column: "DeliveryApprovedByRevisorId");

            migrationBuilder.AddForeignKey(
                name: "FK_PlannedDeliveries_Apadrinamientos_ApadrinamientoId",
                schema: "model",
                table: "PlannedDeliveries",
                column: "ApadrinamientoId",
                principalSchema: "model",
                principalTable: "Apadrinamientos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlannedDeliveries_Revisores_DeliveryApprovedByRevisorId",
                schema: "model",
                table: "PlannedDeliveries",
                column: "DeliveryApprovedByRevisorId",
                principalSchema: "model",
                principalTable: "Revisores",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlannedDeliveries_Apadrinamientos_ApadrinamientoId",
                schema: "model",
                table: "PlannedDeliveries");

            migrationBuilder.DropForeignKey(
                name: "FK_PlannedDeliveries_Revisores_DeliveryApprovedByRevisorId",
                schema: "model",
                table: "PlannedDeliveries");

            migrationBuilder.DropIndex(
                name: "IX_PlannedDeliveries_DeliveryApprovedByRevisorId",
                schema: "model",
                table: "PlannedDeliveries");

            migrationBuilder.DropColumn(
                name: "DeliveryApprovedByRevisorId",
                schema: "model",
                table: "PlannedDeliveries");

            migrationBuilder.RenameColumn(
                name: "ApadrinamientoId",
                schema: "model",
                table: "PlannedDeliveries",
                newName: "ToPadrinoId");

            migrationBuilder.RenameIndex(
                name: "IX_PlannedDeliveries_ApadrinamientoId",
                schema: "model",
                table: "PlannedDeliveries",
                newName: "IX_PlannedDeliveries_ToPadrinoId");

            migrationBuilder.AddColumn<int>(
                name: "FromBecarioId",
                schema: "model",
                table: "PlannedDeliveries",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                schema: "model",
                table: "Filiales",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedOnUtc",
                value: new DateTimeOffset(new DateTime(2023, 7, 26, 3, 58, 12, 676, DateTimeKind.Unspecified).AddTicks(6364), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.CreateIndex(
                name: "IX_PlannedDeliveries_FromBecarioId",
                schema: "model",
                table: "PlannedDeliveries",
                column: "FromBecarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_PlannedDeliveries_Becarios_FromBecarioId",
                schema: "model",
                table: "PlannedDeliveries",
                column: "FromBecarioId",
                principalSchema: "model",
                principalTable: "Becarios",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PlannedDeliveries_Padrinos_ToPadrinoId",
                schema: "model",
                table: "PlannedDeliveries",
                column: "ToPadrinoId",
                principalSchema: "model",
                principalTable: "Padrinos",
                principalColumn: "Id");
        }
    }
}
