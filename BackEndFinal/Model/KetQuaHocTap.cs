using System.ComponentModel.DataAnnotations.Schema;

namespace BackEndFinal.Model
{
    public class KetQuaHocTap
    {
        public int Id { get; set; }
        public required string MaSV { get; set; }
        public required string TenHocKy { get; set; } // VD: "Học kỳ 1 - 2024"

        public double GPA { get; set; } // Hệ 4.0
        public int DiemRenLuyen { get; set; }

        public required string XepLoaiHocBong { get; set; } // "Xuất sắc", "Giỏi", "Khá", "Không"

        [ForeignKey("MaSV")]
        public  SinhVien? SinhVien { get; set; }
    }
}
