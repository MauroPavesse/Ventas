using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Ventas.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Enums : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Product",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.InsertData(
                table: "TaxCondition",
                columns: new[] { "Id", "Active", "Code", "Deleted", "Description" },
                values: new object[,]
                {
                    { 1, 1, 5, 0, "CONSUMIDOR FINAL" },
                    { 2, 1, 1, 0, "RESPONSABLE INSCRIPTO" },
                    { 3, 1, 4, 0, "SUJETO EXENTO" },
                    { 4, 1, 6, 0, "RESPONSABLE MONOTRIBUTO" },
                    { 5, 1, 7, 0, "SUJETO NO CATEGORIZADO" }
                });

            migrationBuilder.InsertData(
                table: "TaxRate",
                columns: new[] { "Id", "Active", "Code", "Deleted", "Description", "Percentage" },
                values: new object[,]
                {
                    { 1, 1, "0005", 0, "IVA 21%", 21m },
                    { 2, 1, "0004", 0, "IVA 10,5%", 10.5m },
                    { 3, 1, "0003", 0, "IVA 0%", 0m },
                    { 4, 1, "0006", 0, "IVA 27%", 27m },
                    { 5, 1, "0008", 0, "IVA 5%", 5m },
                    { 6, 1, "0009", 0, "IVA 2,5%", 2.5m }
                });

            migrationBuilder.InsertData(
                table: "VoucherType",
                columns: new[] { "Id", "Active", "Code", "Deleted", "Description" },
                values: new object[,]
                {
                    { 1, 1, "OR", 0, "ORDEN DE COMPRA" },
                    { 2, 1, "001", 0, "FACTURA A" },
                    { 3, 1, "004", 0, "RECIBO A" },
                    { 4, 1, "006", 0, "FACTURA B" },
                    { 5, 1, "009", 0, "RECIBO B" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "TaxCondition",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "TaxCondition",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "TaxCondition",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "TaxCondition",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "TaxCondition",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "TaxRate",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "TaxRate",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "TaxRate",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "TaxRate",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "TaxRate",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "TaxRate",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "VoucherType",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "VoucherType",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "VoucherType",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "VoucherType",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "VoucherType",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.AlterColumn<int>(
                name: "Code",
                table: "Product",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);
        }
    }
}
