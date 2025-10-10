using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RMS.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddnavigationpropertiesGovernorateAreaINCustomerAdress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_CustomerAddresses_AreaId",
                table: "CustomerAddresses",
                column: "AreaId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerAddresses_BranchId",
                table: "CustomerAddresses",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerAddresses_GovernrateId",
                table: "CustomerAddresses",
                column: "GovernrateId");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerAddresses_Areas_AreaId",
                table: "CustomerAddresses",
                column: "AreaId",
                principalTable: "Areas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerAddresses_Branches_BranchId",
                table: "CustomerAddresses",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerAddresses_Governorates_GovernrateId",
                table: "CustomerAddresses",
                column: "GovernrateId",
                principalTable: "Governorates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerAddresses_Areas_AreaId",
                table: "CustomerAddresses");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomerAddresses_Branches_BranchId",
                table: "CustomerAddresses");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomerAddresses_Governorates_GovernrateId",
                table: "CustomerAddresses");

            migrationBuilder.DropIndex(
                name: "IX_CustomerAddresses_AreaId",
                table: "CustomerAddresses");

            migrationBuilder.DropIndex(
                name: "IX_CustomerAddresses_BranchId",
                table: "CustomerAddresses");

            migrationBuilder.DropIndex(
                name: "IX_CustomerAddresses_GovernrateId",
                table: "CustomerAddresses");
        }
    }
}
