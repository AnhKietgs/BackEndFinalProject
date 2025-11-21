using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BackEndFinal.Migrations
{
    /// <inheritdoc />
    public partial class TaoBangLanDau : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SinhViens",
                columns: table => new
                {
                    MaSV = table.Column<string>(type: "text", nullable: false),
                    HoTen = table.Column<string>(type: "text", nullable: false),
                    Lop = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SinhViens", x => x.MaSV);
                });

            migrationBuilder.CreateTable(
                name: "KetQuaHocTaps",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MaSV = table.Column<string>(type: "text", nullable: false),
                    TenHocKy = table.Column<string>(type: "text", nullable: false),
                    GPA = table.Column<double>(type: "double precision", nullable: false),
                    DiemRenLuyen = table.Column<int>(type: "integer", nullable: false),
                    XepLoaiHocBong = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KetQuaHocTaps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KetQuaHocTaps_SinhViens_MaSV",
                        column: x => x.MaSV,
                        principalTable: "SinhViens",
                        principalColumn: "MaSV",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KyLuats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MaSV = table.Column<string>(type: "text", nullable: false),
                    NoiDung = table.Column<string>(type: "text", nullable: false),
                    NgayQuyetDinh = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KyLuats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KyLuats_SinhViens_MaSV",
                        column: x => x.MaSV,
                        principalTable: "SinhViens",
                        principalColumn: "MaSV",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_KetQuaHocTaps_MaSV",
                table: "KetQuaHocTaps",
                column: "MaSV");

            migrationBuilder.CreateIndex(
                name: "IX_KyLuats_MaSV",
                table: "KyLuats",
                column: "MaSV");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "KetQuaHocTaps");

            migrationBuilder.DropTable(
                name: "KyLuats");

            migrationBuilder.DropTable(
                name: "SinhViens");
        }
    }
}
