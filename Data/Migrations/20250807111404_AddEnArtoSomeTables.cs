using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RMS.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddEnArtoSomeTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Governorates",
                newName: "NameEn");

            migrationBuilder.RenameIndex(
                name: "IX_Governorates_Name",
                table: "Governorates",
                newName: "IX_Governorates_NameEn");

            migrationBuilder.RenameColumn(
                name: "NarastBranchId",
                table: "CustomerAddresses",
                newName: "NearestBranchId");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Categories",
                newName: "NameEn");

            migrationBuilder.RenameColumn(
                name: "ItemsLayout",
                table: "Categories",
                newName: "ItemsCardsLayout");

            migrationBuilder.RenameIndex(
                name: "IX_Categories_Name",
                table: "Categories",
                newName: "IX_Categories_NameEn");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Areas",
                newName: "NameEn");

            migrationBuilder.RenameIndex(
                name: "IX_Areas_Name_GovernorateId",
                table: "Areas",
                newName: "IX_Areas_NameEn_GovernorateId");

            migrationBuilder.AddColumn<string>(
                name: "NameAr",
                table: "Governorates",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NameAr",
                table: "Categories",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NameAr",
                table: "Areas",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Governorates_NameAr",
                table: "Governorates",
                column: "NameAr",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Categories_NameAr",
                table: "Categories",
                column: "NameAr",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Areas_NameAr_GovernorateId",
                table: "Areas",
                columns: new[] { "NameAr", "GovernorateId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Governorates_NameAr",
                table: "Governorates");

            migrationBuilder.DropIndex(
                name: "IX_Categories_NameAr",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Areas_NameAr_GovernorateId",
                table: "Areas");

            migrationBuilder.DropColumn(
                name: "NameAr",
                table: "Governorates");

            migrationBuilder.DropColumn(
                name: "NameAr",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "NameAr",
                table: "Areas");

            migrationBuilder.RenameColumn(
                name: "NameEn",
                table: "Governorates",
                newName: "Name");

            migrationBuilder.RenameIndex(
                name: "IX_Governorates_NameEn",
                table: "Governorates",
                newName: "IX_Governorates_Name");

            migrationBuilder.RenameColumn(
                name: "NearestBranchId",
                table: "CustomerAddresses",
                newName: "NarastBranchId");

            migrationBuilder.RenameColumn(
                name: "NameEn",
                table: "Categories",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "ItemsCardsLayout",
                table: "Categories",
                newName: "ItemsLayout");

            migrationBuilder.RenameIndex(
                name: "IX_Categories_NameEn",
                table: "Categories",
                newName: "IX_Categories_Name");

            migrationBuilder.RenameColumn(
                name: "NameEn",
                table: "Areas",
                newName: "Name");

            migrationBuilder.RenameIndex(
                name: "IX_Areas_NameEn_GovernorateId",
                table: "Areas",
                newName: "IX_Areas_Name_GovernorateId");
        }
    }
}
