using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RMS.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddENArInToppings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "ToppingOptions",
                newName: "NameEn");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "ToppingGroups",
                newName: "TitleEn");

            migrationBuilder.AddColumn<string>(
                name: "NameAr",
                table: "ToppingOptions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TitleAr",
                table: "ToppingGroups",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NameAr",
                table: "ToppingOptions");

            migrationBuilder.DropColumn(
                name: "TitleAr",
                table: "ToppingGroups");

            migrationBuilder.RenameColumn(
                name: "NameEn",
                table: "ToppingOptions",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "TitleEn",
                table: "ToppingGroups",
                newName: "Title");
        }
    }
}
