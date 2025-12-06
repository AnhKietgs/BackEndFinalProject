using BackEndFinal.Model;
using Microsoft.EntityFrameworkCore;

namespace BackEndFinal.DAO
{
    public class SinhVienDao
    {
        public readonly AppDbContext _context; //truy cập data base

        public SinhVienDao(AppDbContext context)
        {
            _context = context;
        }

        // Lấy thông tin đầy đủ (cho chức năng Xem)
        public SinhVien? GetSinhVienFullInfo(string maSV)
        {
            // Chuyển mã sinh viên cần tìm về chữ hoa (hoặc thường)
            string maSVUpper = maSV.Trim().ToUpper();

            return _context.SinhViens
                .AsSplitQuery()//giúp EF Core tách nhiều Include thành nhiều query nhỏ, tránh:
                                //SQL JOIN khổng lồ
                                //Lặp dữ liệu
                                //Nặng bộ nhớ
                .Include(s => s.KetQuaHocTaps)//lấy danh sách điểm của sinh viên
                .Include(s => s.KyLuats)//lấy danh sách kỷ luật
                                        // So sánh với mã trong database đã được chuyển về cùng dạng
                .FirstOrDefault(s => s.MaSV.Trim().ToUpper() == maSVUpper);
        }
        public SinhVien? GetSinhVienBasicInfor(string maSV)
        {
            // 1. Kiểm tra đầu vào
            if (string.IsNullOrWhiteSpace(maSV))
            {
                return null;
            }

            // Chuẩn hóa chuỗi tìm kiếm: Loại bỏ khoảng trắng thừa và chuyển về chữ hoa
            string maSVChuanHoa = maSV.Trim().ToUpper();

            // Thực hiện truy vấn
            // Sử dụng .FirstOrDefault() để tìm bản ghi đầu tiên khớp hoặc trả về null nếu không thấy.
            var sinhVien = _context.SinhViens
                // So sánh mã trong DB (cũng được chuẩn hóa tương tự) với mã tìm kiếm
                .FirstOrDefault(s => s.MaSV.Trim().ToUpper() == maSVChuanHoa);

            return sinhVien;
        }
        public List<SinhVien> GetSinhVienInfor()
        {
          
            return _context.SinhViens.ToList();
         
        }
        public List<KetQuaHocTap> GetSinhVienResult()
        {

            return _context.KetQuaHocTaps
                .Include(k=>k.SinhVien)//để có thể lấy thông tin liên quan trong bảng sinh viên
                                       //(Load thêm dữ liệu từ bảng SinhVien)
                .ToList();

        }
        public List<KyLuat> GetSinhVienKyLuat()
        {

            return _context.KyLuats
                .Include(k => k.SinhVien)
                .ToList();

        }
        public List<KetQuaHocTap> GetKetQuaByKy(string hocKy, string namHoc)
        {
            return _context.KetQuaHocTaps
                           .Include(k => k.SinhVien)
                           // Lọc theo đúng kỳ và năm được yêu cầu
                           .Where(k => k.HocKy == hocKy && k.NamHoc == namHoc)
                           // Thực thi câu lệnh và chuyển thành danh sách
                           .ToList();
        }
        public void AddSinhVien(SinhVien sv)
        {
            _context.SinhViens.Add(sv);
            _context.SaveChanges();
        }

        // Lưu kết quả học tập (cho CTSV)
        public void SaveKetQua(KetQuaHocTap kq)
        {
            // Kiểm tra xem kỳ này có chưa, nếu có thì update, chưa thì thêm mới
            var existing = _context.KetQuaHocTaps
                .FirstOrDefault(k => k.MaSV == kq.MaSV && k.HocKy == kq.HocKy&&k.NamHoc==kq.NamHoc);

            if (existing != null)
            {
                existing.GPA = kq.GPA;
                existing.DiemRenLuyen = kq.DiemRenLuyen;
                existing.XepLoaiHocBong = kq.XepLoaiHocBong;
                existing.XepLoaiHocLuc= kq.XepLoaiHocLuc;
            }
            else
            {
                _context.KetQuaHocTaps.Add(kq);
            }
            _context.SaveChanges();
        }
        //Xoa sinh vien
        public void DeleteSinhVien(string maSV)
        {
            // Tìm sinh viên theo mã
            var sv = _context.SinhViens.Find(maSV);
            if (sv != null)
            {
                _context.SinhViens.Remove(sv);
                _context.SaveChanges();
            }
            else
            {
                throw new Exception("Không tìm thấy sinh viên này!");
            }
        }
        // Thêm kỷ luật

        public void AddKyLuat(KyLuat kl)
        {
            _context.KyLuats.Add(kl);
            _context.SaveChanges();
        }

        // Cập nhật thông tin sinh viên
        public bool UpdateSinhVien(SinhVien svNew)
        {
            var svCurrent = _context.SinhViens.FirstOrDefault(s => s.MaSV.Trim().ToUpper() == svNew.MaSV.Trim().ToUpper());

            if (svCurrent == null)
            {
                return false;
            }

            svCurrent.HoTen = svNew.HoTen;
            svCurrent.DiaChi = svNew.DiaChi;
            svCurrent.GioiTinh = svNew.GioiTinh;
            svCurrent.NgaySinh = svNew.NgaySinh;
            svCurrent.SoDienThoai = svNew.SoDienThoai;

            try
            {
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Không thể cập nhật thông tin sinh viên!" + ex.Message);
            }
        }
        public bool UpdateDiem(KetQuaHocTap diemNew)
        {
            var svCurrent = _context.KetQuaHocTaps.FirstOrDefault(s => s.MaSV.Trim().ToUpper() == diemNew.MaSV.Trim().ToUpper());

            if (svCurrent == null)
            {
                return false;
            }

            svCurrent.HocKy = diemNew.HocKy;
            svCurrent.NamHoc = diemNew.NamHoc;
            svCurrent.GPA = diemNew.GPA;
            svCurrent.DiemRenLuyen = diemNew.DiemRenLuyen;
            svCurrent.XepLoaiHocLuc = diemNew.XepLoaiHocLuc;
            svCurrent.XepLoaiHocBong= diemNew.XepLoaiHocBong;
            try
            {
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Không thể cập nhật thông tin sinh viên!" + ex.Message);
            }
        }
        public bool UpdateKyLuat(KyLuat KLNew)
        {
            var svCurrent = _context.KyLuats.FirstOrDefault(s => s.MaSV.Trim().ToUpper() == KLNew.MaSV.Trim().ToUpper());

            if (svCurrent == null)
            {
                return false;
            }
            svCurrent.LyDo=KLNew.LyDo;
            svCurrent.HocKy = KLNew.HocKy;
            svCurrent.NamHoc = KLNew.NamHoc;
            svCurrent.HinhThuc = KLNew.HinhThuc;
            svCurrent.NgayQuyetDinh = KLNew.NgayQuyetDinh;

            try
            {
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Không thể cập nhật thông tin sinh viên!" + ex.Message);
            }
        }

        public bool DeleteKyLuatById(int id)
        {
            
            var kyLuatEntity = _context.KyLuats.Find(id);

            if (kyLuatEntity == null)
            {
                return false;
            }

            // Nếu tìm thấy -> Đánh dấu bản ghi để xóa
            _context.KyLuats.Remove(kyLuatEntity);
            int rowsAffected = _context.SaveChanges();

            // Nếu có ít nhất 1 dòng bị ảnh hưởng, nghĩa là xóa thành công.
            return rowsAffected > 0;
        }
        public bool DeleteScoreById(int id)
        {
         
            var ketQuaEntity = _context.KetQuaHocTaps.Find(id);      
            if (ketQuaEntity == null)
            {
                return false;
            }
            _context.KetQuaHocTaps.Remove(ketQuaEntity);
            int rowsAffected = _context.SaveChanges();

            return rowsAffected > 0;
        }
    }
}
