namespace BackEndFinalEx.DTO
{
    public class CapNhatSinhVienDTO
    {
        public required string HoTen { get; set; }
        public required DateTime NgaySinh { get; set; } // Dạng string đã format dd/MM/yyyy
        public required string GioiTinh { get; set; }
        public required string DiaChi { get; set; }
        public required string SoDienThoai { get; set; }

    }
}
