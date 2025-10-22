using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RMS.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddTOIsClosedAllDayAndIsOpen24HoursBranchWorkingHourExceptionTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Date",
                table: "BranchWorkingHourExceptions",
                newName: "StartDate");

            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                table: "BranchWorkingHourExceptions",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "BranchWorkingHourExceptions",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateOnly>(
                name: "EndDate",
                table: "BranchWorkingHourExceptions",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<bool>(
                name: "IsClosedAllDay",
                table: "BranchWorkingHourExceptions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
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

            migrationBuilder.AddColumn<string>(
                name: "LastUpdatedById",
                table: "BranchWorkingHourExceptions",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdatedOn",
                table: "BranchWorkingHourExceptions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BranchWorkingHourExceptions_CreatedById",
                table: "BranchWorkingHourExceptions",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_BranchWorkingHourExceptions_LastUpdatedById",
                table: "BranchWorkingHourExceptions",
                column: "LastUpdatedById");

            migrationBuilder.AddForeignKey(
                name: "FK_BranchWorkingHourExceptions_AspNetUsers_CreatedById",
                table: "BranchWorkingHourExceptions",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BranchWorkingHourExceptions_AspNetUsers_LastUpdatedById",
                table: "BranchWorkingHourExceptions",
                column: "LastUpdatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BranchWorkingHourExceptions_AspNetUsers_CreatedById",
                table: "BranchWorkingHourExceptions");

            migrationBuilder.DropForeignKey(
                name: "FK_BranchWorkingHourExceptions_AspNetUsers_LastUpdatedById",
                table: "BranchWorkingHourExceptions");

            migrationBuilder.DropIndex(
                name: "IX_BranchWorkingHourExceptions_CreatedById",
                table: "BranchWorkingHourExceptions");

            migrationBuilder.DropIndex(
                name: "IX_BranchWorkingHourExceptions_LastUpdatedById",
                table: "BranchWorkingHourExceptions");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "BranchWorkingHourExceptions");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "BranchWorkingHourExceptions");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "BranchWorkingHourExceptions");

            migrationBuilder.DropColumn(
                name: "IsClosedAllDay",
                table: "BranchWorkingHourExceptions");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "BranchWorkingHourExceptions");

            migrationBuilder.DropColumn(
                name: "IsOpen24Hours",
                table: "BranchWorkingHourExceptions");

            migrationBuilder.DropColumn(
                name: "LastUpdatedById",
                table: "BranchWorkingHourExceptions");

            migrationBuilder.DropColumn(
                name: "LastUpdatedOn",
                table: "BranchWorkingHourExceptions");

            migrationBuilder.RenameColumn(
                name: "StartDate",
                table: "BranchWorkingHourExceptions",
                newName: "Date");
        }
    }
}
