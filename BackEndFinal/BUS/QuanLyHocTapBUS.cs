using BackEndFinal.DAO;
using BackEndFinal.Model;
using BackEndFinalEx.DTO.CapNhatDTO;
using BackEndFinalEx.DTO.DangNhapDTO;
using BackEndFinalEx.DTO.ThemDTO;
using BackEndFinalEx.DTO.XemDSachDTO;


namespace BackEndFinal.BUS
{
    public class QuanLyHocTapBUS
    {
        private readonly SinhVienDao _dao;//readonly nghĩa là:Chỉ được gán giá trị trong constructor,
                                          //Không ai có thể đổi sang một instance khác khi class đang hoạt động.
        private readonly UserDao _userDao;//Tránh lỗi thay đổi giữa chừng
                                          //An toàn hơn khi nhiều phương thức dùng chung 1 context
                                          //Đảm bảo đúng nguyên tắc DI(Dependency Injection)
        public QuanLyHocTapBUS(SinhVienDao dao, UserDao userDao)
        {
            _dao = dao;
            _userDao = userDao;
        }
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
        public void XuLyNhapDiem(ThemDiemDTO input)
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

        public void ThemSinhVienMoi(ThemSinhVienDTO input)
        {
            // Chuẩn hóa ngày sinh
            DateTime ngaySinhUtc = input.NgaySinh.Kind == DateTimeKind.Utc
                ? input.NgaySinh
                : DateTime.SpecifyKind(input.NgaySinh, DateTimeKind.Utc);

            // Mapping từ DTO → Entity
            var sv = new SinhVien
            {
                MaSV = input.MaSV,
                HoTen = input.HoTen,
                SoDienThoai = input.SoDienThoai,
                DiaChi = input.DiaChi,
                NgaySinh = ngaySinhUtc,
                GioiTinh = input.GioiTinh,
                KetQuaHocTaps = new List<KetQuaHocTap>(),
                KyLuats = new List<KyLuat>()
            };

            // Transaction đảm bảo toàn vẹn dữ liệu
            using (var transaction = _dao._context.Database.BeginTransaction())
            {
                var existed = _dao.GetSinhVienFullInfo(sv.MaSV);
                if (existed != null)
                    throw new Exception($"Sinh viên {sv.MaSV} đã tồn tại!");

                try
                {
                    // Tạo user
                    var newUser = new User
                    {
                        Username = sv.MaSV,
                        Password = sv.SoDienThoai,
                        Role = "SinhVien"
                    };

                    _userDao.AddUser(newUser);
                    _dao.AddSinhVien(sv);

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception("Lỗi thêm sinh viên: " + ex.Message);
                }
            }
        }


        public void XoaSinhVien(string maSV)
        {
            
            using (var transaction = _dao._context.Database.BeginTransaction())
            {
                try
                {
                    // Xóa User
                    _userDao.DeleteUser(maSV);

                    // Xóa Sinh viên
                    _dao.DeleteSinhVien(maSV);

                    // Nếu cả 2 lệnh trên trôi chảy thì mới Lưu thật (Commit)
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
        public void ThemKyLuat(ThemKyLuatDTO kl)
        {
            var kyLuat = new KyLuat
            {
                MaSV = kl.MaSV,
                LyDo = kl.LyDo,
                HinhThuc = kl.HinhThuc,
                NgayQuyetDinh = kl.NgayQuyetDinh,
                HocKy = kl.HocKy,
                NamHoc = kl.NamHoc
            };
            _dao.AddKyLuat(kyLuat);
        }
        public ThongTinCaNhanDTO? LayThongTinCaNhan(string maSV)
        {
            var sv = _dao.GetSinhVienBasicInfor(maSV); // Gọi DAO lấy data gốc
            if (sv == null) return null;//ktra

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
        public List<XemDachTTSVienDTO> LayThongTinCaNhan()
        {
            List<SinhVien> listEntity = _dao.GetSinhVienInfor(); // Gọi DAO lấy data gốc
           
            var listDto= listEntity.Select(sv => new XemDachTTSVienDTO//dùng để map từng SinhVien thành một object kiểu XemDachTTSVienDTO.
            { 
                
                MaSV=sv.MaSV,
                HoTen=sv.HoTen,
                NgaySinh=sv.NgaySinh,
                GioiTinh=sv.GioiTinh,
                DiaChi=sv.DiaChi,
                SoDienThoai=sv.SoDienThoai
            })
            .OrderBy(x => x.MaSV)
            .ToList();

            return listDto;
        }
        public List<XemDSachDiemDTO> LayDiemCaNhan()
        {
            List<KetQuaHocTap> listEntity = _dao.GetSinhVienResult(); // Gọi DAO lấy data gốc


            var listDto = listEntity.Select(sv => new XemDSachDiemDTO//dùng để map từng SinhVien thành một object kiểu XemDachTTSVienDTO.
            {
                Id = sv.Id,
                MaSV = sv.MaSV,
                HocKy = sv.HocKy,
                NamHoc = sv.NamHoc,
                GPA = sv.GPA,
                DiemRenLuyen = sv.DiemRenLuyen,
                XepLoaiHocLuc = sv.XepLoaiHocLuc,
                HoTen = sv.SinhVien?.HoTen ?? ""
            })
                .OrderBy(x => x.MaSV)
                .ToList();

            return listDto;
        }
        public List<XemDSachKyLuatDTO> LayKyLuatCaNhan()
        {
            List<KyLuat> listEntity = _dao.GetSinhVienKyLuat();
            var listDto = listEntity.Select(sv => new XemDSachKyLuatDTO
            {
                Id = sv.Id,
                MaSV = sv.MaSV,
                HocKy = sv.HocKy,
                LyDo=sv.LyDo,
                NamHoc = sv.NamHoc,
                HinhThuc = sv.HinhThuc,
                NgayQuyetDinh = sv.NgayQuyetDinh,
                Hoten = sv.SinhVien?.HoTen ?? ""
            }).ToList();

            return listDto;
        }
        public KetQuaTraCuuDTO LayKetQuaHocTapTheoKy(string maSV, string hocKy, string namHoc)
        {
            // Lấy thông tin đầy đủ của sinh viên (bao gồm cả KetQuaHocTaps và KyLuats
            var sv = _dao.GetSinhVienFullInfo(maSV);
            if (sv == null) throw new Exception("Không tìm thấy sinh viên");//ktra

            //  Khởi tạo DTO trả về với thông tin kỳ học
            var ketQuaDTO = new KetQuaTraCuuDTO
            {
                HocKy = hocKy,
                NamHoc = namHoc,
                // Khởi tạo danh sách kỷ luật rỗng để tránh null nếu không có kỷ luật nào
                DanhSachKyLuat = new List<ChiTietKyLuatDTO>()
            };

          
            var diemCuaKy = sv.KetQuaHocTaps?.FirstOrDefault(k => k.HocKy.Trim() == hocKy.Trim() && k.NamHoc.Trim() == namHoc.Trim());
            //Lọc danh sách kết quả học tập trong entity sinh viên với điều kiện cùng học kỳ và năm học
            if (diemCuaKy != null)//tim được thì gán
            {
                ketQuaDTO.GPA = diemCuaKy.GPA;
                ketQuaDTO.DiemRenLuyen = diemCuaKy.DiemRenLuyen;
                ketQuaDTO.XepLoaiHocBong = diemCuaKy.XepLoaiHocBong;
                ketQuaDTO.XepLoaiHocLuc = diemCuaKy.XepLoaiHocLuc;
            }

            // Lọc danh sách kỷ luật của học kỳ và năm học đó
            // Kiểm tra null cho KyLuats
            if (sv.KyLuats != null)
            {
                var kyLuatCuaKy = sv.KyLuats
                                    .Where(kl => kl.HocKy.Trim() == hocKy.Trim() && kl.NamHoc.Trim() == namHoc.Trim())
                                    .ToList();//1 kỳ có thể có nhiều kỳ luật nên dùng vạy

                if (kyLuatCuaKy.Any())//Nếu có ít nhất 1 bản ghi
                {
                    ketQuaDTO.DanhSachKyLuat = kyLuatCuaKy.Select(kl => new ChiTietKyLuatDTO//map sang DTO
                    {
                        LyDo=kl.LyDo,
                        HinhThuc = kl.HinhThuc,
                        NgayQuyetDinh = kl.NgayQuyetDinh
                    }).ToList();
                }
            }

            return ketQuaDTO;
        }


        public List<XemDSHocBongDTO> LayDanhSachXetHocBong(string hocKy, string namHoc)
        {
            //  Lấy tất cả kết quả học tập trong kỳ đó
            var listKetQua = _dao.GetKetQuaByKy(hocKy, namHoc);

            // Lọc những người có học bổng (Khác "Không") và chuyển sang DTO hiển thị
            var danhSachXetDuyet = listKetQua
                .Where(kq => kq.XepLoaiHocBong != "Không" && kq.XepLoaiHocBong != null)
                .Select(kq => new XemDSHocBongDTO
                {
                    MaSV = kq.MaSV,
                    NamHoc=kq.NamHoc,
                    HocKy =kq.HocKy,
                    GPA = kq.GPA,
                    DiemRenLuyen = kq.DiemRenLuyen,
                    XepLoaiHocBong = TinhXepLoaiHocBong(kq.GPA,kq.DiemRenLuyen)
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
        public bool CapNhatDiem(string maSV, CapNhatDiemDTO input)//maSV — mã sinh viên cần cập nhật
                                                                  // input — DTO chứa GPA, điểm rèn luyện, học kỳ, năm học
        {
            var svToUpdate = new KetQuaHocTap()
            {
                MaSV = maSV,//Gán mã sinh viên cần cập nhật.
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
                LyDo=input.LyDo,
                HocKy = input.HocKy,
                NamHoc = input.NamHoc,
                HinhThuc = input.HinhThuc,
                NgayQuyetDinh= input.NgayQuyetDinh
            };

            return _dao.UpdateKyLuat(svToUpdate);
        }
        public void ResetPassword(ResetPasswordDTO input)
        {
            // Lấy thông tin sinh viên từ bảng SinhVien để kiểm tra SĐT
            var sv = _dao.GetSinhVienFullInfo(input.MaSV);

            if (sv == null)
            {
                throw new Exception("Mã sinh viên không tồn tại.");
            }

            //  So sánh SĐT nhập vào với SĐT trong hồ sơ (Bỏ khoảng trắng cho chắc ăn)
            if (sv.SoDienThoai?.Trim() != input.SoDienThoaiXacThuc.Trim())
            {
                throw new Exception("Số điện thoại xác thực không chính xác!");
            }

            // Nếu thông tin khớp, tiến hành cập nhật mật khẩu bên bảng User
            bool isUpdated = _userDao.UpdatePassword(input.MaSV, input.MatKhauMoi);

            if (!isUpdated)
            {
                // Trường hợp hiếm: Có trong bảng SV nhưng chưa có trong bảng User
                throw new Exception("Tài khoản người dùng chưa được kích hoạt.");
            }
        }

        public bool XoaKyLuat(int id)
        {
            // Kiểm tra tính hợp lệ của ID (ví dụ: ID phải là số dương)
            if (id <= 0)
            {
                throw new ArgumentException("ID kỷ luật không hợp lệ. ID phải lớn hơn 0.");
            }

            // Gọi xuống tầng DAO để thực hiện thao tác xóa
            return _dao.DeleteKyLuatById(id);
        }
        public bool XoaDiemTheoId(int id)
        {
            // Validate ID hợp lệ (ID trong DB thường bắt đầu từ 1)
            if (id <= 0)
            {
                throw new ArgumentException("ID bảng điểm không hợp lệ.");
            }

            // Gọi DAO để thực hiện xóa
            return _dao.DeleteScoreById(id);
        }
    }

}

