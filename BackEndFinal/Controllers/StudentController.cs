using BackEndFinal.BUS;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BackEndFinal.Controllers
{
    [Route("api/student")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly QuanLyHocTapBUS _bus;

        public StudentController(QuanLyHocTapBUS bus)
        {
            _bus = bus;
        }

        // API: GET api/student/info/SV001
        [HttpGet("info/{maSV}")]
        public IActionResult XemThongTin(string maSV)
        {
            var data = _bus.LayThongTinChoSinhVien(maSV);
            if (data == null) return NotFound("Không tìm thấy sinh viên này");

            return Ok(data);
        }
    }
}
