using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RuangKampus.API.Migrations
{
    /// <inheritdoc />
    public partial class AddPurposeOfBookingToRoomBooking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PurposeOfBooking",
                table: "RoomBookings",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PurposeOfBooking",
                table: "RoomBookings");
        }
    }
}
