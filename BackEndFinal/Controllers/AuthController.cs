using BackEndFinal.BUS;
using BackEndFinal.DAO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BackEndFinalEx.DTO;
namespace BackEndFinal.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly QuanLyHocTapBUS _bus;

        public AuthController(QuanLyHocTapBUS bus)
        {
            _bus = bus;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto input)
        {
            // input.Username là MaSV, input.Password là SĐT
            var user = _bus.CheckLogin(input.Username, input.Password);

            if (user == null)
            {
                return Unauthorized("Sai tài khoản hoặc mật khẩu!");
            }

            // Đăng nhập thành công -> Trả về thông tin User (để Frontend biết đường chuyển trang)
            return Ok(new
            {
                message = "Đăng nhập thành công",
                role = user.Role,
                maSV = user.Username // Trả về MaSV để Frontend dùng nó gọi API xem điểm
            });
        }
        [HttpPost("reset-password")]
        public IActionResult ResetPassword([FromBody] ResetPasswordDTO input)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                // Gọi BUS để xử lý
                _bus.ResetPassword(input);
                return Ok(new { message = "Lấy lại mật khẩu thành công! Vui lòng đăng nhập lại." });
            }
            catch (Exception ex)
            {
                // Trả về lỗi (ví dụ: SĐT không đúng)
                return BadRequest(new { message = ex.Message });
            }
        }
    }

}
