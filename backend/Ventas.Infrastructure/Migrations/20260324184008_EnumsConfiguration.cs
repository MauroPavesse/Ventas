using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Ventas.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class EnumsConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Entity",
                columns: new[] { "Id", "Active", "Deleted", "Name" },
                values: new object[] { 1, 1, 0, "COMPROBANTE" });

            migrationBuilder.InsertData(
                table: "StateEntity",
                columns: new[] { "Id", "Active", "Deleted", "EntityId", "State" },
                values: new object[,]
                {
                    { 1, 1, 0, 1, "INICIADO" },
                    { 2, 1, 0, 1, "ES ESPERA" },
                    { 3, 1, 0, 1, "FINALIZADO" },
                    { 4, 1, 0, 1, "CANCELADO" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "StateEntity",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "StateEntity",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "StateEntity",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "StateEntity",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Entity",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
