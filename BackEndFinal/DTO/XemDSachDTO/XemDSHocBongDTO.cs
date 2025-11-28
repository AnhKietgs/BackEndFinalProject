namespace BackEndFinalEx.DTO.XemDSachDTO
{
    public class XemDSHocBongDTO
    {
        public required string MaSV { get; set; }
        public required string HocKy { get; set; }
        public required string NamHoc { get; set; } 
        public double GPA { get; set; }
        public int DiemRenLuyen { get; set; }
        public required string XepLoaiHocBong { get; set; } // Kết quả đã tính
    }
}
