using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingWorkplace.DataBase.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUserIdFromIndexOfReservation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Reservations_UserId_WorkplaceId_TimeFrom_TimeTo",
                table: "Reservations");

            migrationBuilder.DropIndex(
                name: "IX_Reservations_WorkplaceId",
                table: "Reservations");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_UserId",
                table: "Reservations",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_WorkplaceId_TimeFrom_TimeTo",
                table: "Reservations",
                columns: new[] { "WorkplaceId", "TimeFrom", "TimeTo" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Reservations_UserId",
                table: "Reservations");

            migrationBuilder.DropIndex(
                name: "IX_Reservations_WorkplaceId_TimeFrom_TimeTo",
                table: "Reservations");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_UserId_WorkplaceId_TimeFrom_TimeTo",
                table: "Reservations",
                columns: new[] { "UserId", "WorkplaceId", "TimeFrom", "TimeTo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_WorkplaceId",
                table: "Reservations",
                column: "WorkplaceId");
        }
    }
}
