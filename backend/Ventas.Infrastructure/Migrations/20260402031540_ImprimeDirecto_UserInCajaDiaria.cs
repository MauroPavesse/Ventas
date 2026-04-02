using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ventas.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ImprimeDirecto_UserInCajaDiaria : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "DailyBox",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.InsertData(
                table: "Configuration",
                columns: new[] { "Id", "Active", "BoolValue", "Deleted", "Description", "NumericValue", "StringValue", "Variable" },
                values: new object[] { 11, 1, false, 0, "true: El ticket imprime directo / false: Muestra pantalla previa", 0m, "", "imprimeTicketDirecto" });

            migrationBuilder.CreateIndex(
                name: "IX_DailyBox_UserId",
                table: "DailyBox",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_DailyBox_User_UserId",
                table: "DailyBox",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DailyBox_User_UserId",
                table: "DailyBox");

            migrationBuilder.DropIndex(
                name: "IX_DailyBox_UserId",
                table: "DailyBox");

            migrationBuilder.DeleteData(
                table: "Configuration",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "DailyBox");
        }
    }
}
