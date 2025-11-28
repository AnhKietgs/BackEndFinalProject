using Microsoft.VisualBasic;

namespace BackEndFinalEx.DTO.XemDSachDTO
{
    public class ThongTinCaNhanDTO
    {
        public required string MaSV { get; set; }
        public required string HoTen { get; set; }
        public required DateTime NgaySinh { get; set; } 
        public required string GioiTinh { get; set; }
        public required string DiaChi { get; set; }
        public required string SoDienThoai { get; set; }
    }
}
