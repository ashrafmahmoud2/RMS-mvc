using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RMS.Web.Migrations
{
    /// <inheritdoc />
    public partial class updatePropNameTimeSpanInTimeToCompleteStatusToStatusDuration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TimeToCompleteStatus",
                table: "OrderStatuses");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "StatusDuration",
                table: "OrderStatuses",
                type: "time",
                maxLength: 50,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StatusDuration",
                table: "OrderStatuses");

            migrationBuilder.AddColumn<string>(
                name: "TimeToCompleteStatus",
                table: "OrderStatuses",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
        }
    }
}
