using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RMS.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddPropDeliveryTimeInMinutesInBrachTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "FlatNumber",
                table: "CustomerAddresses",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10,
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DeliveryTimeInMinutes",
                table: "Branches",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeliveryTimeInMinutes",
                table: "Branches");

            migrationBuilder.AlterColumn<string>(
                name: "FlatNumber",
                table: "CustomerAddresses",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(30)",
                oldMaxLength: 30,
                oldNullable: true);
        }
    }
}
