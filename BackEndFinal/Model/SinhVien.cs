using System.ComponentModel.DataAnnotations;

namespace BackEndFinal.Model
{
    public class SinhVien
    {
        [Key]
        public required string MaSV { get; set; } // Khóa chính là Mã SV
        public required string HoTen { get; set; }
        public required string DiaChi { get; set; }

        public required DateTime NgaySinh { get; set; }

        public required string GioiTinh { get; set; }
        public required string SoDienThoai { get; set; }
        public required ICollection<KetQuaHocTap> KetQuaHocTaps { get; set; }
        public required ICollection<KyLuat> KyLuats { get; set; }
    }
}
