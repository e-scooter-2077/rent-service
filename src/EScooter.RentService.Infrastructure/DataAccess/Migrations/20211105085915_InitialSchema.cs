using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EScooter.RentService.Infrastructure.DataAccess.Migrations;

public partial class InitialSchema : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.EnsureSchema(
            name: "entities");

        migrationBuilder.CreateTable(
            name: "Rents",
            schema: "entities",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ScooterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                RequestTimestamp = table.Column<DateTime>(type: "datetime", nullable: false),
                ConfirmationTimestamp = table.Column<DateTime>(type: "datetime", nullable: true),
                StopTimestamp = table.Column<DateTime>(type: "datetime", nullable: true),
                StopReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                CancellationReason = table.Column<string>(type: "nvarchar(max)", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Rents", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Customers",
            schema: "entities",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                OngoingRentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Customers", x => x.Id);
                table.ForeignKey(
                    name: "FK_Customers_Rents_OngoingRentId",
                    column: x => x.OngoingRentId,
                    principalSchema: "entities",
                    principalTable: "Rents",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "Scooters",
            schema: "entities",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                OngoingRentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                IsEnabled = table.Column<bool>(type: "bit", nullable: false),
                IsOutOfService = table.Column<bool>(type: "bit", nullable: false),
                IsInStandby = table.Column<bool>(type: "bit", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Scooters", x => x.Id);
                table.ForeignKey(
                    name: "FK_Scooters_Rents_OngoingRentId",
                    column: x => x.OngoingRentId,
                    principalSchema: "entities",
                    principalTable: "Rents",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateIndex(
            name: "IX_Customers_OngoingRentId",
            schema: "entities",
            table: "Customers",
            column: "OngoingRentId",
            unique: true,
            filter: "[OngoingRentId] IS NOT NULL");

        migrationBuilder.CreateIndex(
            name: "IX_Rents_CustomerId",
            schema: "entities",
            table: "Rents",
            column: "CustomerId");

        migrationBuilder.CreateIndex(
            name: "IX_Rents_ScooterId",
            schema: "entities",
            table: "Rents",
            column: "ScooterId");

        migrationBuilder.CreateIndex(
            name: "IX_Scooters_OngoingRentId",
            schema: "entities",
            table: "Scooters",
            column: "OngoingRentId",
            unique: true,
            filter: "[OngoingRentId] IS NOT NULL");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_Customers_Rents_OngoingRentId",
            schema: "entities",
            table: "Customers");

        migrationBuilder.DropForeignKey(
            name: "FK_Scooters_Rents_OngoingRentId",
            schema: "entities",
            table: "Scooters");

        migrationBuilder.DropTable(
            name: "Rents",
            schema: "entities");

        migrationBuilder.DropTable(
            name: "Customers",
            schema: "entities");

        migrationBuilder.DropTable(
            name: "Scooters",
            schema: "entities");
    }
}
