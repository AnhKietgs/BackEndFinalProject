using System.ComponentModel.DataAnnotations;

namespace BackEndFinalEx.DTO.ThemDTO
{
    public class ThemKyLuatDTO
    {
        [Required]
        public required string MaSV { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập nội dung kỷ luật")]
        public required string HinhThuc { get; set; }

        public required string LyDo { get; set; }
        public DateTime NgayQuyetDinh { get; set; }

        // --- THÊM 2 TRƯỜNG MỚI ---
        [Required(ErrorMessage = "Vui lòng chọn học kỳ")]
        public required string HocKy { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn năm học")]
        public required string NamHoc { get; set; }
    }
}
