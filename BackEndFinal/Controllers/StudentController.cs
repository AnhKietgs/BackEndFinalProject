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
        [HttpGet("basic-info/{maSV}")]
        public IActionResult GetBasicInfo(string MaSV)
        {
            var data = _bus.LayThongTinCaNhan(MaSV);
            if (data == null) return NotFound("Không tìm thấy sinh viên này");

            return Ok(data);
        }

        [HttpGet("academic-result/{maSV}")]
        public IActionResult GetAcademicResult(string MaSV, [FromQuery] string HocKy, [FromQuery] string NamHoc)
        {
            try
            {
                if (string.IsNullOrEmpty(HocKy) || string.IsNullOrEmpty(NamHoc))
                {
                    return BadRequest("Vui lòng cung cấp học kỳ và năm học.");
                }

                var result = _bus.LayKetQuaHocTapTheoKy(MaSV, HocKy, NamHoc);

                // Nếu cả điểm và kỷ luật đều không có gì thì báo chưa có dữ liệu
                if (result.GPA == null && result.DanhSachKyLuat.Count == 0)
                {
                    return Ok(new { message = $"Chưa có dữ liệu cho {HocKy}, năm học {NamHoc}" });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
