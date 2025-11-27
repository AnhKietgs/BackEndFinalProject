namespace BackEndFinal.DTO
{
    public class NhapDiemDTO
    {
        public required string MaSV { get; set; }
        public required string HocKy { get; set; } // VD: "Học kỳ 1"
        public required string NamHoc { get; set; }
        public double GPA { get; set; }
        public int DiemRenLuyen { get; set; }
    }
}
