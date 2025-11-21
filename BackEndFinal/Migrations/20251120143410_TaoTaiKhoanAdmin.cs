using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackEndFinal.Migrations
{
    /// <inheritdoc />
    public partial class TaoTaiKhoanAdmin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Username", "Password", "Role" },
                values: new object[] { "admin", "@123", "CTSV" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Username",
                keyValue: "admin");
        }
    }
}
