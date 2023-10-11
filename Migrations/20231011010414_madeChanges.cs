using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace wangazon.Migrations
{
    /// <inheritdoc />
    public partial class madeChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "OrderClosed", "OrderPlaced" },
                values: new object[] { new DateTime(2023, 10, 9, 23, 35, 0, 163, DateTimeKind.Local).AddTicks(9925), new DateTime(2023, 10, 9, 22, 5, 0, 163, DateTimeKind.Local).AddTicks(9922) });

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "OrderClosed", "OrderPlaced" },
                values: new object[] { new DateTime(2023, 10, 9, 23, 35, 0, 163, DateTimeKind.Local).AddTicks(9933), new DateTime(2023, 10, 9, 22, 5, 0, 163, DateTimeKind.Local).AddTicks(9932) });
        }
    }
}
