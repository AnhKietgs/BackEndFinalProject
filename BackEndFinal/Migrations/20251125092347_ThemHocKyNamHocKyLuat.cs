using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackEndFinal.Migrations
{
    /// <inheritdoc />
    public partial class ThemHocKyNamHocKyLuat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HocKy",
                table: "KyLuats",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NamHoc",
                table: "KyLuats",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HocKy",
                table: "KyLuats");

            migrationBuilder.DropColumn(
                name: "NamHoc",
                table: "KyLuats");
        }
    }
}
