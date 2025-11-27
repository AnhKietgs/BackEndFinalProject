using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackEndFinal.Migrations
{
    /// <inheritdoc />
    public partial class ThemXetHocLuc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "XepLoaiHocLuc",
                table: "KetQuaHocTaps",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "XepLoaiHocLuc",
                table: "KetQuaHocTaps");
        }
    }
}
