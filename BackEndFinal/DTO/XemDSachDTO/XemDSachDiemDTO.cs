namespace BackEndFinalEx.DTO.XemDSachDTO
{
    public class XemDSachDiemDTO
    {
        public required string MaSV { get; set; }
        public required string HocKy { get; set; }
        public required string NamHoc { get; set; }

        public double GPA { get; set; } // Hệ 4.0
        public int DiemRenLuyen { get; set; }

        public required string XepLoaiHocLuc { get; set; }
    }
}
