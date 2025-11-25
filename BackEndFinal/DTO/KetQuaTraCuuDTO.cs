namespace BackEndFinalEx.DTO
{
    public class KetQuaTraCuuDTO
    {
       
            public required string HocKy { get; set; }
            public required string NamHoc { get; set; }

            // Phần Kết quả học tập
            public double? GPA { get; set; } // Dùng double? để cho phép null nếu chưa có điểm
            public int? DiemRenLuyen { get; set; }
            public  string? XepLoaiHocBong { get; set; }

            // Phần Kỷ luật (Danh sách nội dung kỷ luật trong kỳ đó)
            public List<ChiTietKyLuatDTO> DanhSachKyLuat { get; set; } = new List<ChiTietKyLuatDTO>();
        }

        // DTO con để hiện thị chi tiết kỷ luật cho gọn
        public class ChiTietKyLuatDTO
        {
            public required string NoiDung { get; set; }
            public required string NgayQuyetDinh { get; set; }
        }
    
}
