using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingWorkplace.DataBase.Migrations
{
    /// <inheritdoc />
    public partial class RemoveCountPropertyFromEquipmentForWorkplace : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Count",
                table: "EquipmentForWorkplaces");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Count",
                table: "EquipmentForWorkplaces",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
