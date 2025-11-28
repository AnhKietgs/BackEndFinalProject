using System.ComponentModel.DataAnnotations;

namespace BackEndFinalEx.DTO
{
    public class ResetPasswordDTO
    {
        [Required(ErrorMessage = "Vui lòng nhập Mã sinh viên")]
        public required string MaSV { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập Số điện thoại xác thực")]
        public required string SoDienThoaiXacThuc { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập Mật khẩu mới")]
        public required string MatKhauMoi { get; set; }
    }
}
