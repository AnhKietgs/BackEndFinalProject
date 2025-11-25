using BackEndFinal.DAO;
using BackEndFinal.DTO;
using BackEndFinal.Model;
using BackEndFinalEx.DTO;

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
                DiaChi = sv.DiaChi,
                NgaySinh = sv.NgaySinh,
                GioiTinh = sv.GioiTinh,
                SoDienThoai = sv.SoDienThoai,   
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
            //  Xóa thông tin sinh viên (Kèm điểm số, kỷ luật - EF Core thường tự xóa theo)
            _dao.DeleteSinhVien(maSV);

            //  Xóa luôn tài khoản đăng nhập của sinh viên đó
           
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
        public ThongTinCaNhanDTO? LayThongTinCaNhan(string maSV)
        {
            var sv = _dao.GetSinhVienFullInfo(maSV); // Gọi DAO lấy data gốc
            if (sv == null) return null;

            // Map sang DTO thông tin cá nhân
            return new ThongTinCaNhanDTO
            {
                MaSV = sv.MaSV,
                HoTen = sv.HoTen,
                NgaySinh = sv.NgaySinh.ToString("dd/MM/yyyy"),
                GioiTinh = sv.GioiTinh,
                DiaChi = sv.DiaChi,
                SoDienThoai = sv.SoDienThoai
            };
        }
        public KetQuaTraCuuDTO LayKetQuaHocTapTheoKy(string maSV, string hocKy, string namHoc)
        {
            var sv = _dao.GetSinhVienFullInfo(maSV); // Lấy data gốc
            if (sv == null) throw new Exception("Không tìm thấy sinh viên");

            var ketQuaDTO = new KetQuaTraCuuDTO
            {
                HocKy = hocKy,
                NamHoc = namHoc
            };

            // a. Lọc tìm Điểm của kỳ đó (Sử dụng LINQ)
            // Giả sử trong bảng KetQuaHocTap bạn cũng đã có cột HocKy và NamHoc
            // Nếu chưa có thì bạn phải dựa vào TenHocKy để tách chuỗi, nhưng tốt nhất là nên có cột riêng.
            // Ở đây mình giả định bảng KetQuaHocTap có cột TenHocKy chứa thông tin dạng "Học kỳ 1 - 2024-2025" để so sánh tạm thời.
            // CÁCH CHUẨN NHẤT: Bảng KetQuaHocTap cũng nên tách 2 cột HocKy, NamHoc như bảng KyLuat.

            // Tạm dùng cách so sánh chuỗi (bạn nên điều chỉnh lại cho khớp DB của bạn)
            string tenHocKyCanTim = $"{hocKy} - {namHoc}";
            var diemCuaKy = sv.KetQuaHocTaps.FirstOrDefault(k => k.TenHocKy.Contains(hocKy) && k.TenHocKy.Contains(namHoc));

            if (diemCuaKy != null)
            {
                ketQuaDTO.GPA = diemCuaKy.GPA;
                ketQuaDTO.DiemRenLuyen = diemCuaKy.DiemRenLuyen;
                ketQuaDTO.XepLoaiHocBong = diemCuaKy.XepLoaiHocBong;
            }

            // b. Lọc tìm Kỷ luật của kỳ đó (Sử dụng LINQ)
            // (Dựa trên 2 cột HocKy, NamHoc vừa thêm ở bài trước)
            var kyLuatCuaKy = sv.KyLuats
                                .Where(kl => kl.HocKy == hocKy && kl.NamHoc == namHoc)
                                .ToList();

            if (kyLuatCuaKy.Any())
            {
                ketQuaDTO.DanhSachKyLuat = kyLuatCuaKy.Select(kl => new ChiTietKyLuatDTO
                {
                    NoiDung = kl.NoiDung,
                    NgayQuyetDinh = kl.NgayQuyetDinh.ToString("dd/MM/yyyy")
                }).ToList();
            }

            return ketQuaDTO;
        }
    }
}

