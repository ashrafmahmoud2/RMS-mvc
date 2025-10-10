using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RMS.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddSelectedToppingOptionRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_SelectedToppingOptions_ToppingOptionId",
                table: "SelectedToppingOptions",
                column: "ToppingOptionId");

            migrationBuilder.AddForeignKey(
                name: "FK_SelectedToppingOptions_ToppingOptions_ToppingOptionId",
                table: "SelectedToppingOptions",
                column: "ToppingOptionId",
                principalTable: "ToppingOptions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SelectedToppingOptions_ToppingOptions_ToppingOptionId",
                table: "SelectedToppingOptions");

            migrationBuilder.DropIndex(
                name: "IX_SelectedToppingOptions_ToppingOptionId",
                table: "SelectedToppingOptions");
        }
    }
}
