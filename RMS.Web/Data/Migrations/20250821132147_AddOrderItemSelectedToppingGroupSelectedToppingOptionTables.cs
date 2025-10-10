using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RMS.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderItemSelectedToppingGroupSelectedToppingOptionTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderItemToppings");

            migrationBuilder.DropColumn(
                name: "OrderedDate",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "TimeFromOpenToBuy",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "CurrentPrice",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "Label",
                table: "CustomerAddresses");

            migrationBuilder.RenameColumn(
                name: "NearestBranchId",
                table: "CustomerAddresses",
                newName: "BranchId");

            migrationBuilder.AlterColumn<decimal>(
                name: "FeesTotal",
                table: "Orders",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "DiscountAmount",
                table: "Orders",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "DeliveryFees",
                table: "Orders",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "CashbackUsedAmount",
                table: "Orders",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "CashbackPercent",
                table: "Orders",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(5,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "CashbackPercent",
                table: "OrderItems",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(5,2)");

            migrationBuilder.AddColumn<decimal>(
                name: "PriceAtOrderTime",
                table: "OrderItems",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "SelectedToppingGroups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ToppingGroupId = table.Column<int>(type: "int", nullable: false),
                    OrderItemId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SelectedToppingGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SelectedToppingGroups_OrderItems_OrderItemId",
                        column: x => x.OrderItemId,
                        principalTable: "OrderItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SelectedToppingGroups_ToppingGroups_ToppingGroupId",
                        column: x => x.ToppingGroupId,
                        principalTable: "ToppingGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SelectedToppingOptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ToppingOptionId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    PriceAtOrderTime = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ToppingGroupId = table.Column<int>(type: "int", nullable: false),
                    SelectedToppingGroupId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SelectedToppingOptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SelectedToppingOptions_SelectedToppingGroups_SelectedToppingGroupId",
                        column: x => x.SelectedToppingGroupId,
                        principalTable: "SelectedToppingGroups",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SelectedToppingOptions_ToppingGroups_ToppingGroupId",
                        column: x => x.ToppingGroupId,
                        principalTable: "ToppingGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Branches_AreaId",
                table: "Branches",
                column: "AreaId");

            migrationBuilder.CreateIndex(
                name: "IX_Branches_GovernorateId",
                table: "Branches",
                column: "GovernorateId");

            migrationBuilder.CreateIndex(
                name: "IX_SelectedToppingGroups_OrderItemId",
                table: "SelectedToppingGroups",
                column: "OrderItemId");

            migrationBuilder.CreateIndex(
                name: "IX_SelectedToppingGroups_ToppingGroupId",
                table: "SelectedToppingGroups",
                column: "ToppingGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_SelectedToppingOptions_SelectedToppingGroupId",
                table: "SelectedToppingOptions",
                column: "SelectedToppingGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_SelectedToppingOptions_ToppingGroupId",
                table: "SelectedToppingOptions",
                column: "ToppingGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_Branches_Areas_AreaId",
                table: "Branches",
                column: "AreaId",
                principalTable: "Areas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Branches_Governorates_GovernorateId",
                table: "Branches",
                column: "GovernorateId",
                principalTable: "Governorates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Branches_Areas_AreaId",
                table: "Branches");

            migrationBuilder.DropForeignKey(
                name: "FK_Branches_Governorates_GovernorateId",
                table: "Branches");

            migrationBuilder.DropTable(
                name: "SelectedToppingOptions");

            migrationBuilder.DropTable(
                name: "SelectedToppingGroups");

            migrationBuilder.DropIndex(
                name: "IX_Branches_AreaId",
                table: "Branches");

            migrationBuilder.DropIndex(
                name: "IX_Branches_GovernorateId",
                table: "Branches");

            migrationBuilder.DropColumn(
                name: "PriceAtOrderTime",
                table: "OrderItems");

            migrationBuilder.RenameColumn(
                name: "BranchId",
                table: "CustomerAddresses",
                newName: "NearestBranchId");

            migrationBuilder.AlterColumn<decimal>(
                name: "FeesTotal",
                table: "Orders",
                type: "decimal(10,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "DiscountAmount",
                table: "Orders",
                type: "decimal(10,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "DeliveryFees",
                table: "Orders",
                type: "decimal(10,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "CashbackUsedAmount",
                table: "Orders",
                type: "decimal(10,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "CashbackPercent",
                table: "Orders",
                type: "decimal(5,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AddColumn<DateTime>(
                name: "OrderedDate",
                table: "Orders",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "TimeFromOpenToBuy",
                table: "Orders",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<decimal>(
                name: "CashbackPercent",
                table: "OrderItems",
                type: "decimal(5,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AddColumn<decimal>(
                name: "CurrentPrice",
                table: "OrderItems",
                type: "decimal(10,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Label",
                table: "CustomerAddresses",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "OrderItemToppings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderItemId = table.Column<int>(type: "int", nullable: false),
                    ToppingOptionId = table.Column<int>(type: "int", nullable: false),
                    CashbackPercent = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    CurrentPrice = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItemToppings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderItemToppings_OrderItems_OrderItemId",
                        column: x => x.OrderItemId,
                        principalTable: "OrderItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderItemToppings_ToppingOptions_ToppingOptionId",
                        column: x => x.ToppingOptionId,
                        principalTable: "ToppingOptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderItemToppings_OrderItemId",
                table: "OrderItemToppings",
                column: "OrderItemId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItemToppings_ToppingOptionId",
                table: "OrderItemToppings",
                column: "ToppingOptionId");
        }
    }
}
