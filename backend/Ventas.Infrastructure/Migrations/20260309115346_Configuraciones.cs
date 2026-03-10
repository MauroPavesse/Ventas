using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Ventas.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Configuraciones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Configuration",
                columns: new[] { "Id", "Active", "BoolValue", "Deleted", "Description", "NumericValue", "StringValue", "Variable" },
                values: new object[,]
                {
                    { 1, 1, false, 0, "Identificador del sistema", 0m, "TEST", "M&MID" },
                    { 2, 1, false, 0, "Nombre de la empresa", 0m, "", "empresa" },
                    { 3, 1, false, 0, "Fecha inicio de la empresa", 0m, "", "fechaInicio" },
                    { 4, 1, false, 0, "CUIT / CUIL de la empresa", 0m, "", "cuit" },
                    { 5, 1, false, 0, "Condición fiscal de la empresa", 0m, "", "condicionFiscalId" },
                    { 6, 1, false, 0, "Alias de ARCA", 0m, "", "arcaAlias" },
                    { 7, 1, false, 0, "Dirección certificado de ARCA", 0m, "", "arcaCertificado" },
                    { 8, 1, false, 0, "Clave del certificado de ARCA", 0m, "", "arcaClave" },
                    { 9, 1, false, 0, "Token del sistema", 0m, "", "tokenSystem" },
                    { 10, 1, false, 0, "Último número usado en la caja diaria", 0m, "", "cajaDiariaNumero" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Configuration",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Configuration",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Configuration",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Configuration",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Configuration",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Configuration",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Configuration",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Configuration",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Configuration",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Configuration",
                keyColumn: "Id",
                keyValue: 10);
        }
    }
}
