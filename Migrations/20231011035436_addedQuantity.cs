using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace wangazon.Migrations
{
    /// <inheritdoc />
    public partial class addedQuantity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "MenuItems",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 1,
                column: "Quantity",
                value: 0);

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 2,
                column: "Quantity",
                value: 0);

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 3,
                column: "Quantity",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "OrderClosed", "OrderPlaced" },
                values: new object[] { new DateTime(2023, 10, 10, 22, 24, 36, 493, DateTimeKind.Local).AddTicks(1723), new DateTime(2023, 10, 10, 20, 54, 36, 493, DateTimeKind.Local).AddTicks(1716) });

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "OrderClosed", "OrderPlaced" },
                values: new object[] { new DateTime(2023, 10, 10, 22, 24, 36, 493, DateTimeKind.Local).AddTicks(1728), new DateTime(2023, 10, 10, 20, 54, 36, 493, DateTimeKind.Local).AddTicks(1727) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "MenuItems");

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "OrderClosed", "OrderPlaced" },
                values: new object[] { new DateTime(2023, 10, 10, 19, 34, 14, 137, DateTimeKind.Local).AddTicks(4478), new DateTime(2023, 10, 10, 18, 4, 14, 137, DateTimeKind.Local).AddTicks(4474) });

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "OrderClosed", "OrderPlaced" },
                values: new object[] { new DateTime(2023, 10, 10, 19, 34, 14, 137, DateTimeKind.Local).AddTicks(4483), new DateTime(2023, 10, 10, 18, 4, 14, 137, DateTimeKind.Local).AddTicks(4482) });
        }
    }
}
