using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RMS.Web.Migrations
{
    /// <inheritdoc />
    public partial class MakeBranchTableInheritFromBaseModelTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                table: "Branches",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "Branches",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Branches",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "LastUpdatedById",
                table: "Branches",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdatedOn",
                table: "Branches",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Branches_CreatedById",
                table: "Branches",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Branches_LastUpdatedById",
                table: "Branches",
                column: "LastUpdatedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Branches_AspNetUsers_CreatedById",
                table: "Branches",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Branches_AspNetUsers_LastUpdatedById",
                table: "Branches",
                column: "LastUpdatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Branches_AspNetUsers_CreatedById",
                table: "Branches");

            migrationBuilder.DropForeignKey(
                name: "FK_Branches_AspNetUsers_LastUpdatedById",
                table: "Branches");

            migrationBuilder.DropIndex(
                name: "IX_Branches_CreatedById",
                table: "Branches");

            migrationBuilder.DropIndex(
                name: "IX_Branches_LastUpdatedById",
                table: "Branches");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "Branches");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "Branches");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Branches");

            migrationBuilder.DropColumn(
                name: "LastUpdatedById",
                table: "Branches");

            migrationBuilder.DropColumn(
                name: "LastUpdatedOn",
                table: "Branches");
        }
    }
}
