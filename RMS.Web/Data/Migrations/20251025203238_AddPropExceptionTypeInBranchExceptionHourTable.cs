using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RMS.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddPropExceptionTypeInBranchExceptionHourTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsClosedAllDay",
                table: "BranchWorkingHourExceptions");

            migrationBuilder.DropColumn(
                name: "IsOpen24Hours",
                table: "BranchWorkingHourExceptions");

            migrationBuilder.AddColumn<int>(
                name: "ExceptionType",
                table: "BranchWorkingHourExceptions",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExceptionType",
                table: "BranchWorkingHourExceptions");

            migrationBuilder.AddColumn<bool>(
                name: "IsClosedAllDay",
                table: "BranchWorkingHourExceptions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsOpen24Hours",
                table: "BranchWorkingHourExceptions",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
