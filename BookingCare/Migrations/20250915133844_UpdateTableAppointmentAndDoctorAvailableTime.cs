using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingCare.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTableAppointmentAndDoctorAvailableTime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_DoctorAvailableTimes_DoctorAvailableTimeId",
                table: "Appointments");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_DoctorAvailableTimeId",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "DoctorAvailableTimeId",
                table: "Appointments");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DoctorAvailableTimeId",
                table: "Appointments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_DoctorAvailableTimeId",
                table: "Appointments",
                column: "DoctorAvailableTimeId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_DoctorAvailableTimes_DoctorAvailableTimeId",
                table: "Appointments",
                column: "DoctorAvailableTimeId",
                principalTable: "DoctorAvailableTimes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
