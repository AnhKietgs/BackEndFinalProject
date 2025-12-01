using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackEndFinal.Migrations
{
    /// <inheritdoc />
    public partial class DoiTenCotVaThemCotMoi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NoiDung",
                table: "KyLuats",
                newName: "LyDo");

            migrationBuilder.AddColumn<string>(
                name: "HinhThuc",
                table: "KyLuats",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HinhThuc",
                table: "KyLuats");

            migrationBuilder.RenameColumn(
                name: "LyDo",
                table: "KyLuats",
                newName: "NoiDung");
        }
    }
}
