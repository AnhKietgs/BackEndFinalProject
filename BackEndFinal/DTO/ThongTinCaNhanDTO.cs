namespace BackEndFinalEx.DTO
{
    public class ThongTinCaNhanDTO
    {
        public required string MaSV { get; set; }
        public required string HoTen { get; set; }
        public required string NgaySinh { get; set; } // Dạng string đã format dd/MM/yyyy
        public required string GioiTinh { get; set; }
        public required string DiaChi { get; set; }
        public required string SoDienThoai { get; set; }
    }
}
