using BackEndFinal.Model;

namespace BackEndFinal.DTO
{
    public class ThongTinSinhVienDTO
    {
        public required string MaSV { get; set; }
        public required string HoTen { get; set; }
        public required string DiaChi { get; set; }
        public required DateTime NgaySinh { get; set; }

        public required string GioiTinh { get; set; }
        public required string SoDienThoai { get; set; }

        public required List<KetQuaDTO> DanhSachDiem { get; set; }
        public required List<KyLuatDTO> DanhSachKyLuat { get; set; }
    }
    public class KetQuaDTO
    {
        public required string TenHocKy { get; set; }
        public double GPA { get; set; }
        public int DiemRenLuyen { get; set; }
        public required string XepLoaiHocBong { get; set; }
    }
    public  class KyLuatDTO
    {
        public required string NoiDung { get; set; }
        public DateTime NgayQuyetDinh { get; set; }
    }
}
