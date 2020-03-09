using Microsoft.EntityFrameworkCore.Migrations;

namespace FirstCore.Web.Migrations
{
    public partial class addModelBuilderEstensionSeed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "Mark");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "Mary");
        }
    }
}
