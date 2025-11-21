using System.ComponentModel.DataAnnotations;

namespace BackEndFinal.Model
{
    public class SinhVien
    {
        [Key]
        public required string MaSV { get; set; } // Khóa chính là Mã SV
        public required string HoTen { get; set; }
        public required string Lop { get; set; }

        public required string SoDienThoai { get; set; }
        public required ICollection<KetQuaHocTap> KetQuaHocTaps { get; set; }
        public required ICollection<KyLuat> KyLuats { get; set; }
    }
}
