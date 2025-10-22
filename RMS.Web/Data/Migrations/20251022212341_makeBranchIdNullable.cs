using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RMS.Web.Migrations
{
    /// <inheritdoc />
    public partial class makeBranchIdNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BranchWorkingHours_Branches_BranchId",
                table: "BranchWorkingHours");

            migrationBuilder.AlterColumn<int>(
                name: "BranchId",
                table: "BranchWorkingHours",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_BranchWorkingHours_Branches_BranchId",
                table: "BranchWorkingHours",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BranchWorkingHours_Branches_BranchId",
                table: "BranchWorkingHours");

            migrationBuilder.AlterColumn<int>(
                name: "BranchId",
                table: "BranchWorkingHours",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_BranchWorkingHours_Branches_BranchId",
                table: "BranchWorkingHours",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
