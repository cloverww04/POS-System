using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace wangazon.Migrations
{
    /// <inheritdoc />
    public partial class includeCommentToOrderItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Comment",
                table: "MenuItems");

            migrationBuilder.AddColumn<string>(
                name: "Comment",
                table: "OrderMenuItems",
                type: "text",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "OrderClosed", "OrderPlaced" },
                values: new object[] { new DateTime(2023, 10, 15, 4, 18, 48, 66, DateTimeKind.Local).AddTicks(4715), new DateTime(2023, 10, 15, 2, 48, 48, 66, DateTimeKind.Local).AddTicks(4712) });

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "OrderClosed", "OrderPlaced" },
                values: new object[] { new DateTime(2023, 10, 15, 4, 18, 48, 66, DateTimeKind.Local).AddTicks(4721), new DateTime(2023, 10, 15, 2, 48, 48, 66, DateTimeKind.Local).AddTicks(4719) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Comment",
                table: "OrderMenuItems");

            migrationBuilder.AddColumn<string>(
                name: "Comment",
                table: "MenuItems",
                type: "text",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 1,
                column: "Comment",
                value: null);

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 2,
                column: "Comment",
                value: null);

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 3,
                column: "Comment",
                value: null);

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "OrderClosed", "OrderPlaced" },
                values: new object[] { new DateTime(2023, 10, 15, 4, 14, 36, 667, DateTimeKind.Local).AddTicks(4466), new DateTime(2023, 10, 15, 2, 44, 36, 667, DateTimeKind.Local).AddTicks(4462) });

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "OrderClosed", "OrderPlaced" },
                values: new object[] { new DateTime(2023, 10, 15, 4, 14, 36, 667, DateTimeKind.Local).AddTicks(4471), new DateTime(2023, 10, 15, 2, 44, 36, 667, DateTimeKind.Local).AddTicks(4470) });
        }
    }
}
