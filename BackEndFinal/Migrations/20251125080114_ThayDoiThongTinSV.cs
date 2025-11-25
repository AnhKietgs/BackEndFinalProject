using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackEndFinal.Migrations
{
    /// <inheritdoc />
    public partial class ThayDoiThongTinSV : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Lop",
                table: "SinhViens",
                newName: "GioiTinh");

            migrationBuilder.AddColumn<string>(
                name: "DiaChi",
                table: "SinhViens",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "NgaySinh",
                table: "SinhViens",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiaChi",
                table: "SinhViens");

            migrationBuilder.DropColumn(
                name: "NgaySinh",
                table: "SinhViens");

            migrationBuilder.RenameColumn(
                name: "GioiTinh",
                table: "SinhViens",
                newName: "Lop");
        }
    }
}
