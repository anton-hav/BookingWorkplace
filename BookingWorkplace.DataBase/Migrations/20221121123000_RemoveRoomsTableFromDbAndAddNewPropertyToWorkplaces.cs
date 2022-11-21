using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingWorkplace.DataBase.Migrations
{
    /// <inheritdoc />
    public partial class RemoveRoomsTableFromDbAndAddNewPropertyToWorkplaces : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Workplaces_Rooms_RoomId",
                table: "Workplaces");

            migrationBuilder.DropTable(
                name: "Rooms");

            migrationBuilder.DropIndex(
                name: "IX_Workplaces_RoomId_DeskNumber",
                table: "Workplaces");

            migrationBuilder.DropColumn(
                name: "RoomId",
                table: "Workplaces");

            migrationBuilder.AddColumn<int>(
                name: "Floor",
                table: "Workplaces",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Room",
                table: "Workplaces",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Workplaces_Room_Floor_DeskNumber",
                table: "Workplaces",
                columns: new[] { "Room", "Floor", "DeskNumber" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Workplaces_Room_Floor_DeskNumber",
                table: "Workplaces");

            migrationBuilder.DropColumn(
                name: "Floor",
                table: "Workplaces");

            migrationBuilder.DropColumn(
                name: "Room",
                table: "Workplaces");

            migrationBuilder.AddColumn<Guid>(
                name: "RoomId",
                table: "Workplaces",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "Rooms",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Floor = table.Column<int>(type: "int", nullable: false),
                    Number = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rooms", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Workplaces_RoomId_DeskNumber",
                table: "Workplaces",
                columns: new[] { "RoomId", "DeskNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_Number_Floor",
                table: "Rooms",
                columns: new[] { "Number", "Floor" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Workplaces_Rooms_RoomId",
                table: "Workplaces",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
