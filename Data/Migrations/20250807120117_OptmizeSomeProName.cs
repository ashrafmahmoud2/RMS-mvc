using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RMS.Web.Migrations
{
    /// <inheritdoc />
    public partial class OptmizeSomeProName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ItemStatusEn",
                table: "Items",
                newName: "CardLabelsEn");

            migrationBuilder.RenameColumn(
                name: "ItemStatusAr",
                table: "Items",
                newName: "CardLabelsAr");

            migrationBuilder.RenameColumn(
                name: "ExceptionName",
                table: "BranchWorkingHourExceptions",
                newName: "ExceptionNameEn");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Branches",
                newName: "NameEn");

            migrationBuilder.RenameColumn(
                name: "Address",
                table: "Branches",
                newName: "AddressEn");

            migrationBuilder.RenameIndex(
                name: "IX_Branches_Name",
                table: "Branches",
                newName: "IX_Branches_NameEn");

            migrationBuilder.AddColumn<string>(
                name: "ExceptionNameAr",
                table: "BranchWorkingHourExceptions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AddressAr",
                table: "Branches",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NameAr",
                table: "Branches",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Branches_NameAr",
                table: "Branches",
                column: "NameAr",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Branches_NameAr",
                table: "Branches");

            migrationBuilder.DropColumn(
                name: "ExceptionNameAr",
                table: "BranchWorkingHourExceptions");

            migrationBuilder.DropColumn(
                name: "AddressAr",
                table: "Branches");

            migrationBuilder.DropColumn(
                name: "NameAr",
                table: "Branches");

            migrationBuilder.RenameColumn(
                name: "CardLabelsEn",
                table: "Items",
                newName: "ItemStatusEn");

            migrationBuilder.RenameColumn(
                name: "CardLabelsAr",
                table: "Items",
                newName: "ItemStatusAr");

            migrationBuilder.RenameColumn(
                name: "ExceptionNameEn",
                table: "BranchWorkingHourExceptions",
                newName: "ExceptionName");

            migrationBuilder.RenameColumn(
                name: "NameEn",
                table: "Branches",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "AddressEn",
                table: "Branches",
                newName: "Address");

            migrationBuilder.RenameIndex(
                name: "IX_Branches_NameEn",
                table: "Branches",
                newName: "IX_Branches_Name");
        }
    }
}
