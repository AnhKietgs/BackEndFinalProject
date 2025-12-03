namespace BackEndFinalEx.DTO.XemDSachDTO
{
    public class XemDSachKyLuatDTO
    {
        public int Id { get; set; }
        public required string MaSV { get; set; }
        public required string Hoten { get; set; }
        public required string HinhThuc { get; set; }
        public required string LyDo { get; set; }
        public DateTime NgayQuyetDinh { get; set; }
        public required string HocKy { get; set; } 
        public required string NamHoc { get; set; }
    }
}
