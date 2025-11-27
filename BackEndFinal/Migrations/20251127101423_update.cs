using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackEndFinal.Migrations
{
    /// <inheritdoc />
    public partial class update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TenHocKy",
                table: "KetQuaHocTaps",
                newName: "NamHoc");

            migrationBuilder.AddColumn<string>(
                name: "HocKy",
                table: "KetQuaHocTaps",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HocKy",
                table: "KetQuaHocTaps");

            migrationBuilder.RenameColumn(
                name: "NamHoc",
                table: "KetQuaHocTaps",
                newName: "TenHocKy");
        }
    }
}
