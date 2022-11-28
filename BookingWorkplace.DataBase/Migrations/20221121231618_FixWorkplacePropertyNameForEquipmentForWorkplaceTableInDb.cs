using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingWorkplace.DataBase.Migrations
{
    /// <inheritdoc />
    public partial class FixWorkplacePropertyNameForEquipmentForWorkplaceTableInDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EquipmentForWorkplaces_EquipmentId_WorkplacesId",
                table: "EquipmentForWorkplaces");

            migrationBuilder.DropColumn(
                name: "WorkplacesId",
                table: "EquipmentForWorkplaces");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentForWorkplaces_EquipmentId_WorkplaceId",
                table: "EquipmentForWorkplaces",
                columns: new[] { "EquipmentId", "WorkplaceId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EquipmentForWorkplaces_EquipmentId_WorkplaceId",
                table: "EquipmentForWorkplaces");

            migrationBuilder.AddColumn<Guid>(
                name: "WorkplacesId",
                table: "EquipmentForWorkplaces",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentForWorkplaces_EquipmentId_WorkplacesId",
                table: "EquipmentForWorkplaces",
                columns: new[] { "EquipmentId", "WorkplacesId" },
                unique: true);
        }
    }
}
