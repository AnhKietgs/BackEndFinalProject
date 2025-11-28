using BackEndFinal.BUS;
using BackEndFinal.Model;
using BackEndFinalEx.DTO.CapNhatDTO;
using BackEndFinalEx.DTO.ThemDTO;
using Microsoft.AspNetCore.Mvc;

namespace BackEndFinal.Controllers
{
    [Route("api/ctsv")]
    [ApiController]
    public class CtsvController : ControllerBase
    {
        // 1. Chỉ khai báo duy nhất BUS, bỏ DAO đi
        private readonly QuanLyHocTapBUS _bus;

        // 2. Sửa Constructor: Chỉ nhận BUS
        public CtsvController(QuanLyHocTapBUS bus)
        {
            _bus = bus;
        }

        // API: POST api/ctsv/add-student
        [HttpPost("add-student")]
        public IActionResult ThemSinhVien([FromBody] ThemSinhVienDTO input)
        {
            DateTime ngaySinhUtc = input.NgaySinh.Kind == DateTimeKind.Utc
                           ? input.NgaySinh
                           : DateTime.SpecifyKind(input.NgaySinh, DateTimeKind.Utc);
            try
            {//mapping
                var svEntity = new SinhVien
                {
                    MaSV = input.MaSV,
                    HoTen = input.HoTen,
                    SoDienThoai = input.SoDienThoai,
                    DiaChi = input.DiaChi,
                    NgaySinh = ngaySinhUtc,
                    GioiTinh = input.GioiTinh,

                    // Các danh sách con để rỗng hoặc null, vì mình không nhập lúc này
                    KetQuaHocTaps = new List<KetQuaHocTap>(),
                    KyLuats = new List<KyLuat>()
                };
                _bus.ThemSinhVienMoi(svEntity);
                return Ok("Thêm sinh viên và tự động tạo tài khoản thành công.");
            }
            catch (Exception ex)
            {
                return BadRequest("Lỗi: " + ex.Message);
            }
        }

        // API: POST api/ctsv/nhap-diem
        [HttpPost("nhap-diem")]
        public IActionResult NhapDiem([FromBody] ThemDiemDTO input)
        {
            try
            {
                _bus.XuLyNhapDiem(input); // Đã dùng BUS -> Chuẩn
                return Ok("Đã nhập điểm và xét học bổng thành công.");
            }
            catch (Exception ex)
            {
                return BadRequest("Lỗi: " + ex.Message);
            }
        }

        // API: POST api/ctsv/ky-luat
        [HttpPost("ky-luat")]
        public IActionResult ThemKyLuat([FromBody] ThemKyLuatDTO input)
        {
            var kyLuatEntity = new KyLuat
            {
                MaSV = input.MaSV,
                NoiDung = input.NoiDung,
                NgayQuyetDinh = input.NgayQuyetDinh,
                // MAPPING THÊM 2 TRƯỜNG MỚI
                HocKy = input.HocKy,
                NamHoc = input.NamHoc
            };
            // 4. Gọi BUS (Bạn cần vào BUS viết thêm hàm ThemKyLuat nhé)
            // Đừng gọi _dao.AddKyLuat(kl) nữa
            try
            {
                _bus.ThemKyLuat(kyLuatEntity); // Giả sử bạn đã thêm hàm này bên BUS
                return Ok("Đã ghi nhận kỷ luật.");
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        // API: DELETE api/ctsv/delete-student/SV01
        [HttpDelete("delete-student/{maSV}")]
        public IActionResult XoaSinhVien(string maSV)
        {
            try
            {
                _bus.XoaSinhVien(maSV); // Đã dùng BUS -> Chuẩn
                return Ok($"Đã xóa thành công sinh viên {maSV}");
            }
            catch (Exception ex)
            {
                return BadRequest("Lỗi: " + ex.Message);
            }
        }
        [HttpGet("danh-sach-hoc-bong")]
        public IActionResult GetDanhSachHocBong([FromQuery] string hocKy, [FromQuery] string namHoc)
        {
            if (string.IsNullOrEmpty(hocKy) || string.IsNullOrEmpty(namHoc))
                return BadRequest("Vui lòng chọn kỳ và năm học.");

            var list = _bus.LayDanhSachXetHocBong(hocKy, namHoc);
            return Ok(list);
        }

        // API: PUT api/ctsv/update-student/
        [HttpPut("update-student/{maSV}")]

        public IActionResult CapNhatSinhVien([FromRoute] string maSV, [FromBody] CapNhatSinhVienDTO input)
        {
            try
            {
                var result = _bus.CapNhatSinhVien(maSV, input);
                if (result)
                {
                    return Ok($"Cập nhật thông tin sinh viên thành công!");
                }
                else
                {
                    return NotFound(new { Message = $"Không tìm thấy sinh viên có mã {maSV}." });
                }

            }
            catch (Exception ex)
            {
                return BadRequest("Lỗi: " + ex.Message);
            }
        }
        [HttpPut("update-diem/{maSV}")]
        public IActionResult CapNhatDiem([FromRoute] string maSV, [FromBody] CapNhatDiemDTO input)
        {
            try
            {
                var result = _bus.CapNhatDiem(maSV, input);
                if (result)
                {
                    return Ok($"Cập nhật điểm sinh viên thành công!");
                }
                else
                {
                    return NotFound(new { Message = $"Không tìm thấy sinh viên có mã {maSV}." });
                }

            }
            catch (Exception ex)
            {
                return BadRequest("Lỗi: " + ex.Message);
            }
        }
        [HttpPut("update-kyluat/{maSV}")]
        public IActionResult CapNhatKyLuat([FromRoute] string maSV, [FromBody] CapNhatKyLuatDTO input)
        {
            try
            {
                var result = _bus.CapNhatKyLuat(maSV, input);
                if (result)
                {
                    return Ok($"Cập nhật kỷ luật của sinh viên thành công!");
                }
                else
                {
                    return NotFound(new { Message = $"Không tìm thấy sinh viên có mã {maSV}." });
                }

            }
            catch (Exception ex)
            {
                return BadRequest("Lỗi: " + ex.Message);
            }
        }
        // GET: api/ctsv/danh-sach-sinh-vien
        [HttpGet("danh-sach-sinh-vien")]
        public IActionResult GetAllSinhVien()
        {
            try
            {
                // Gọi BUS để lấy danh sách DTO
                var listData = _bus.LayThongTinCaNhan();
                return Ok(listData);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Lỗi server: " + ex.Message);
            }
        }
        [HttpGet("danh-sach-diem-sinh-vien")]
        public IActionResult GetAllAcademic()
        {
            try
            {
                // Gọi BUS để lấy danh sách DTO
                var listData = _bus.LayDiemCaNhan();
                return Ok(listData);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Lỗi server: " + ex.Message);
            }
        }
        [HttpGet("danh-sach-ky-luat-sinh-vien")]
        public IActionResult GetAllKyLuat()
        {
            try
            {
                // Gọi BUS để lấy danh sách DTO
                var listData = _bus.LayKyLuatCaNhan();
                return Ok(listData);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Lỗi server: " + ex.Message);
            }
        }
    }
}