using BackEndFinal.DAO;
using BackEndFinal.DTO;
using BackEndFinal.Model;

namespace BackEndFinal.BUS
{
    public class QuanLyHocTapBUS
    {
        private readonly SinhVienDao _dao;
        private readonly UserDao _userDao;
        public QuanLyHocTapBUS(SinhVienDao dao, UserDao userDao)
        {
            _dao = dao;
            _userDao = userDao;
        }

        // Hàm logic: Nhập điểm và Tự động xét học bổng
        public void XuLyNhapDiem(NhapDiemDTO input)
        {
            // 1. Tính toán loại học bổng
            string loaiHocBong = "Không";

            if (input.GPA >= 3.6 && input.DiemRenLuyen >= 90)
            {
                loaiHocBong = "Xuất sắc";
            }
            else if (input.GPA >= 3.2 && input.DiemRenLuyen >= 70)
            {
                loaiHocBong = "Giỏi";
            }
            else if (input.GPA >= 2.5 && input.DiemRenLuyen >= 70)
            {
                loaiHocBong = "Khá";
            }

            // 2. Tạo đối tượng Model để gửi xuống DAO
            var ketQua = new KetQuaHocTap
            {
                MaSV = input.MaSV,
                TenHocKy = input.TenHocKy,
                GPA = input.GPA,
                DiemRenLuyen = input.DiemRenLuyen,
                XepLoaiHocBong = loaiHocBong // Đã tự động tính
            };

            // 3. Gọi DAO để lưu
            _dao.SaveKetQua(ketQua);
        }

        public ThongTinSinhVienDTO? LayThongTinChoSinhVien(string maSV)
        {
            var sv = _dao.GetSinhVienFullInfo(maSV);
            if (sv == null) return null;

            // Chuyển đổi sang DTO để trả về (Mapping)
            return new ThongTinSinhVienDTO
            {
                MaSV = sv.MaSV,
                HoTen = sv.HoTen,
                Lop = sv.Lop,

                // Chuyển danh sách Điểm sang dạng rút gọn
                DanhSachDiem = sv.KetQuaHocTaps
                .OrderBy(k => k.TenHocKy) //Sắp xếp theo tên học kỳ (A->Z)
                .Select(kq => new KetQuaDTO
                {
                    TenHocKy = kq.TenHocKy,
                    GPA = kq.GPA,
                    DiemRenLuyen = kq.DiemRenLuyen,
                    XepLoaiHocBong = kq.XepLoaiHocBong
                }).ToList(),

                // Chuyển danh sách Kỷ luật sang dạng rút gọn
                DanhSachKyLuat = sv.KyLuats.Select(kl => new KyLuatDTO
                {
                    NoiDung = kl.NoiDung,
                    NgayQuyetDinh = kl.NgayQuyetDinh
                }).ToList()
            };

        }


        public void ThemSinhVienMoi(SinhVien sv)
        {
          
            // Khởi tạo Transaction
            using (var transaction = _dao._context.Database.BeginTransaction())
            {
                var KtSinhVien = _dao.GetSinhVienFullInfo(sv.MaSV);
                if (KtSinhVien != null)
                {
                    // Nếu đã có thì ném ra lỗi để Controller bắt được và báo cho người dùng
                    throw new Exception($"Sinh viên có mã {sv.MaSV} đã tồn tại trong hệ thống!");
                }
                try
                {
                    //Thêm User
                    var newUser = new User
                    {
                        Username = sv.MaSV,
                        Password = sv.SoDienThoai,
                        Role = "SinhVien"
                    };
                    _userDao.AddUser(newUser);
                    _dao.AddSinhVien(sv);
                    //  Nếu cả 2 bước trên đều ngon lành thì mới Lưu thật (Commit)
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    // Nếu có lỗi ở bất kỳ bước nào -> Hoàn tác lại tất cả (Rollback)
                    // Sinh viên vừa thêm cũng sẽ bị gỡ bỏ, database sạch sẽ.
                    transaction.Rollback();
                    throw new Exception("Lỗi thêm sinh viên: " + ex.Message);
                }
            }
            
        }

        public void XoaSinhVien(string maSV)
        {
            var user = _userDao.CheckLogin(maSV, ""); // Hoặc hàm FindUser
            if (user != null)
            {
                _userDao.DeleteUser(maSV);
            }
            // 1. Xóa thông tin sinh viên (Kèm điểm số, kỷ luật - EF Core thường tự xóa theo)
            _dao.DeleteSinhVien(maSV);

            // 2. Xóa luôn tài khoản đăng nhập của sinh viên đó
           
        }
        public User? CheckLogin(string username, string password)
        {
            // BUS gọi xuống DAO để kiểm tra
            return _userDao.CheckLogin(username, password);
        }
        // Trong file QuanLyHocTapBUS.cs
        public void ThemKyLuat(KyLuat kl)
        {
            // Gọi xuống DAO (để Controller không phải gọi trực tiếp)
            _dao.AddKyLuat(kl);
            // (Lưu ý: Bạn phải đảm bảo _dao ở đây là SinhVienDAO và nó có hàm AddKyLuat)
        }
    }

}
