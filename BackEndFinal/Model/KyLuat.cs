using System.ComponentModel.DataAnnotations.Schema;

namespace BackEndFinal.Model
{
    public class KyLuat
    {
        public int Id { get; set; }
        public required string MaSV { get; set; }
        public required string HinhThuc { get; set; } // VD: "Cảnh cáo mức 1"

        public required string LyDo { get; set; }
        public DateTime NgayQuyetDinh { get; set; }
        public required string HocKy { get; set; } // VD: "Học kỳ 1"
        public required string NamHoc { get; set; } // VD: "2024-2025"

        [ForeignKey("MaSV")]
        public  SinhVien? SinhVien { get; set; }
    }
}
