using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RMS.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddNewDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Items_Categories_CategoryId",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "BasePrice",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "CashbackPercent",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "DiscountPercent",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "IsAvailable",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "MaxOrderQuantity",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "PurchaseCount",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "StatusFlags",
                table: "Items");

            migrationBuilder.RenameColumn(
                name: "MaxSelection",
                table: "ToppingGroups",
                newName: "Type");

            migrationBuilder.RenameColumn(
                name: "Ingredients",
                table: "Items",
                newName: "ItemStatusEn");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "Branches",
                newName: "IsOpen");

            migrationBuilder.RenameColumn(
                name: "GovernrateId",
                table: "Branches",
                newName: "GovernorateId");

            migrationBuilder.AddColumn<int>(
                name: "MaxAllowedQuantity",
                table: "ToppingOptions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MaxAllowedOptions",
                table: "ToppingGroups",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MinAllowedOptions",
                table: "ToppingGroups",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<decimal>(
                name: "Protein",
                table: "Items",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(5,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CategoryId",
                table: "Items",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Carbs",
                table: "Items",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(5,2)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IngredientsAr",
                table: "Items",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IngredientsEn",
                table: "Items",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ItemStatusAr",
                table: "Items",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsBusy",
                table: "Branches",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "MaxAllowedOrdersInDay",
                table: "Branches",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MaxCashOnDeliveryAllowed",
                table: "Branches",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "AspNetUsers",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "Allergy",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NameAr = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Allergy", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Items_AllergyId",
                table: "Items",
                column: "AllergyId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_FullName_PhoneNumber",
                table: "AspNetUsers",
                columns: new[] { "FullName", "PhoneNumber" },
                unique: true,
                filter: "[PhoneNumber] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Items_Allergy_AllergyId",
                table: "Items",
                column: "AllergyId",
                principalTable: "Allergy",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Items_Categories_CategoryId",
                table: "Items",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Items_Allergy_AllergyId",
                table: "Items");

            migrationBuilder.DropForeignKey(
                name: "FK_Items_Categories_CategoryId",
                table: "Items");

            migrationBuilder.DropTable(
                name: "Allergy");

            migrationBuilder.DropIndex(
                name: "IX_Items_AllergyId",
                table: "Items");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_FullName_PhoneNumber",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "MaxAllowedQuantity",
                table: "ToppingOptions");

            migrationBuilder.DropColumn(
                name: "MaxAllowedOptions",
                table: "ToppingGroups");

            migrationBuilder.DropColumn(
                name: "MinAllowedOptions",
                table: "ToppingGroups");

            migrationBuilder.DropColumn(
                name: "IngredientsAr",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "IngredientsEn",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "ItemStatusAr",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "IsBusy",
                table: "Branches");

            migrationBuilder.DropColumn(
                name: "MaxAllowedOrdersInDay",
                table: "Branches");

            migrationBuilder.DropColumn(
                name: "MaxCashOnDeliveryAllowed",
                table: "Branches");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "ToppingGroups",
                newName: "MaxSelection");

            migrationBuilder.RenameColumn(
                name: "ItemStatusEn",
                table: "Items",
                newName: "Ingredients");

            migrationBuilder.RenameColumn(
                name: "IsOpen",
                table: "Branches",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "GovernorateId",
                table: "Branches",
                newName: "GovernrateId");

            migrationBuilder.AlterColumn<decimal>(
                name: "Protein",
                table: "Items",
                type: "decimal(5,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CategoryId",
                table: "Items",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<decimal>(
                name: "Carbs",
                table: "Items",
                type: "decimal(5,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "BasePrice",
                table: "Items",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CashbackPercent",
                table: "Items",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DiscountPercent",
                table: "Items",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsAvailable",
                table: "Items",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "MaxOrderQuantity",
                table: "Items",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PurchaseCount",
                table: "Items",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StatusFlags",
                table: "Items",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Items_Categories_CategoryId",
                table: "Items",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id");
        }
    }
}
