namespace BackEndFinalEx.DTO.XemDSachDTO
{
    public class XemDachTTSVienDTO
    {
        public required string MaSV { get; set; } // Khóa chính là Mã SV
        public required string HoTen { get; set; }
       

        public required DateTime NgaySinh { get; set; }

        public required string GioiTinh { get; set; }
        public required string DiaChi { get; set; }
        public required string SoDienThoai { get; set; }
    }
}
