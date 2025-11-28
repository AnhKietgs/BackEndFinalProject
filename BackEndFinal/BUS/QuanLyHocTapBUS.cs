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
        // Hàm phụ trợ: Tự động tính xếp loại dựa trên GPA và ĐRL
        private string TinhXepLoaiHocBong(double gpa, int diemRenLuyen)
        {
            if (gpa >= 3.6 && diemRenLuyen >= 90) return "Xuất sắc";
            if (gpa >= 3.2 && diemRenLuyen >= 80) return "Giỏi";
            if (gpa >= 2.5 && diemRenLuyen >= 70) return "Khá";
            return "Không";
        }
        private string TinhXeploaiHocLuc(double gpa)
        {

            if (gpa >= 3.6) return "Xuất sắc";
            if (gpa >= 3.2 ) return "Giỏi";
            if (gpa >= 2.5 ) return "Khá";
            if (gpa >= 2.0) return "Trung bình";
                return "Yếu";
        }
        // Hàm logic: Nhập điểm và Tự động xét học bổng
        public void XuLyNhapDiem(NhapDiemDTO input)
        {

            //  Tạo đối tượng Model để gửi xuống DAO
            var ketQua = new KetQuaHocTap
            {
                MaSV = input.MaSV,
                HocKy = input.HocKy,
                NamHoc=input.NamHoc,
                GPA = input.GPA,
                DiemRenLuyen = input.DiemRenLuyen,
                XepLoaiHocBong = TinhXepLoaiHocBong(input.GPA, input.DiemRenLuyen),
                XepLoaiHocLuc=TinhXeploaiHocLuc(input.GPA)
            };

            //  Gọi DAO để lưu
            _dao.SaveKetQua(ketQua);
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
            // Bắt đầu một giao dịch
            // Lưu ý: Cần đảm bảo _svDao._context là public hoặc có property truy cập
            using (var transaction = _dao._context.Database.BeginTransaction())
            {
                try
                {
                    // 1. Xóa User
                    _userDao.DeleteUser(maSV);

                    // 2. Xóa Sinh viên
                    _dao.DeleteSinhVien(maSV);

                    // 3. Nếu cả 2 lệnh trên trôi chảy thì mới Lưu thật (Commit)
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    // Nếu có bất kỳ lỗi gì xảy ra ở bước 1 hoặc 2
                    // Hoàn tác lại tất cả, không xóa gì cả.
                    transaction.Rollback();
                    // Ném lỗi ra ngoài để Controller biết
                    throw new Exception("Lỗi khi xóa sinh viên và tài khoản: " + ex.Message);
                }
            }

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
            var sv = _dao.GetSinhVienBasicInfor(maSV); // Gọi DAO lấy data gốc
            if (sv == null) return null;

            // Map sang DTO thông tin cá nhân
            return new ThongTinCaNhanDTO
            {
                MaSV = sv.MaSV,
                HoTen = sv.HoTen,
                NgaySinh = sv.NgaySinh,
                GioiTinh = sv.GioiTinh,
                DiaChi = sv.DiaChi,
                SoDienThoai = sv.SoDienThoai
            };
        }
        public KetQuaTraCuuDTO LayKetQuaHocTapTheoKy(string maSV, string hocKy, string namHoc)
        {
            // Lấy thông tin đầy đủ của sinh viên (bao gồm cả KetQuaHocTaps và KyLuats)
            // Đảm bảo hàm GetSinhVienById trong DAO đã sử dụng .Include() để tải dữ liệu liên quan.
            var sv = _dao.GetSinhVienFullInfo(maSV);
            if (sv == null) throw new Exception("Không tìm thấy sinh viên");

            // 2. Khởi tạo DTO trả về với thông tin kỳ học
            var ketQuaDTO = new KetQuaTraCuuDTO
            {
                HocKy = hocKy,
                NamHoc = namHoc,
                // Khởi tạo danh sách kỷ luật rỗng để tránh null nếu không có kỷ luật nào
                DanhSachKyLuat = new List<ChiTietKyLuatDTO>()
            };

          
            var diemCuaKy = sv.KetQuaHocTaps?.FirstOrDefault(k => k.HocKy.Trim() == hocKy.Trim() && k.NamHoc.Trim() == namHoc.Trim());

            if (diemCuaKy != null)
            {
                ketQuaDTO.GPA = diemCuaKy.GPA;
                ketQuaDTO.DiemRenLuyen = diemCuaKy.DiemRenLuyen;
                ketQuaDTO.XepLoaiHocBong = diemCuaKy.XepLoaiHocBong;
                // Nếu DTO có trường XepLoaiHocLuc thì gán thêm vào đây
                // ketQuaDTO.XepLoaiHocLuc = diemCuaKy.XepLoaiHocLuc;
            }

            // Lọc danh sách kỷ luật của học kỳ và năm học đó
            // Kiểm tra null cho KyLuats
            if (sv.KyLuats != null)
            {
                var kyLuatCuaKy = sv.KyLuats
                                    .Where(kl => kl.HocKy.Trim() == hocKy.Trim() && kl.NamHoc.Trim() == namHoc.Trim())
                                    .ToList();

                if (kyLuatCuaKy.Any())
                {
                    ketQuaDTO.DanhSachKyLuat = kyLuatCuaKy.Select(kl => new ChiTietKyLuatDTO
                    {
                        NoiDung = kl.NoiDung,
                        // Định dạng ngày tháng năm
                        NgayQuyetDinh = kl.NgayQuyetDinh.ToString("dd/MM/yyyy")
                    }).ToList();
                }
            }

            return ketQuaDTO;
        }


        public List<XemDSHocBongDTO> LayDanhSachXetHocBong(string hocKy, string namHoc)
        {
            // 1. Lấy tất cả kết quả học tập trong kỳ đó
            // (Giả sử DAO có hàm lấy list này, kèm theo thông tin SinhVien)
            var listKetQua = _dao.GetKetQuaByKy(hocKy, namHoc);

            // 2. Lọc những người có học bổng (Khác "Không") và chuyển sang DTO hiển thị
            var danhSachXetDuyet = listKetQua
                .Where(kq => kq.XepLoaiHocBong != "Không" && kq.XepLoaiHocBong != null)
                .Select(kq => new XemDSHocBongDTO
                {
                    MaSV = kq.MaSV,
                    NamHoc=kq.NamHoc,
                    HocKy = hocKy,
                    GPA = kq.GPA,
                    DiemRenLuyen = kq.DiemRenLuyen,
                    XepLoaiHocBong = kq.XepLoaiHocBong
                })
                .OrderByDescending(x => x.GPA) // Sắp xếp điểm từ cao xuống thấp cho đẹp
                .ToList();

            return danhSachXetDuyet;
        }

        // hàm cập nhật thông tin sinh viên
        public bool CapNhatSinhVien(string maSV, CapNhatSinhVienDTO input)
        {
            var svToUpdate = new SinhVien()
            {
                MaSV = maSV,
                HoTen = input.HoTen,
                NgaySinh = input.NgaySinh,
                GioiTinh = input.GioiTinh,
                DiaChi = input.DiaChi,
                SoDienThoai = input.SoDienThoai,

                // khởi tạo các thuộc tính quan hệ bắt buộc trong model SinhVien thành danh sách rỗng
                KetQuaHocTaps = new List<KetQuaHocTap>(),
                KyLuats = new List<KyLuat>()
            };


            return _dao.UpdateSinhVien(svToUpdate);
        }
        public bool CapNhatDiem(string maSV, CapNhatDiemDTO input)
        {
            var svToUpdate = new KetQuaHocTap()
            {
                MaSV = maSV,
                HocKy = input.HocKy,
                NamHoc= input.NamHoc,
                GPA = input.GPA,
                DiemRenLuyen = input.DiemRenLuyen,
                XepLoaiHocBong = TinhXepLoaiHocBong(input.GPA, input.DiemRenLuyen),
                XepLoaiHocLuc = TinhXeploaiHocLuc(input.GPA)

            };

            return _dao.UpdateDiem(svToUpdate);
        }
        public bool CapNhatKyLuat(string maSV, CapNhatKyLuatDTO input)
        {
            var svToUpdate = new KyLuat()
            {
                MaSV = maSV,
                HocKy = input.HocKy,
                NamHoc = input.NamHoc,
                NoiDung = input.NoiDung,
                NgayQuyetDinh= input.NgayQuyetDinh
            };

            return _dao.UpdateKyLuat(svToUpdate);
        }
    }

}

