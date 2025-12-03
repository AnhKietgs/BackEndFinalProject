using BackEndFinal.Model;
using Microsoft.EntityFrameworkCore;

namespace BackEndFinal.DAO
{
    public class SinhVienDao
    {
        public readonly AppDbContext _context; // Giả sử bạn đã config EF Core

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
                .AsSplitQuery()
                .Include(s => s.KetQuaHocTaps)
                .Include(s => s.KyLuats)
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
                .Include(k=>k.SinhVien)
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
            // _context là biến AppDbContext đã được khai báo trong DAO
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
                .FirstOrDefault(k => k.MaSV == kq.MaSV && k.HocKy == kq.HocKy);

            if (existing != null)
            {
                existing.GPA = kq.GPA;
                existing.DiemRenLuyen = kq.DiemRenLuyen;
                existing.XepLoaiHocBong = kq.XepLoaiHocBong;// Cập nhật loại học bổng mới
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
            // 1. Tìm bản ghi cần xóa trong DB theo khóa chính
            // Phương thức .Find() rất hiệu quả cho việc này.
            var kyLuatEntity = _context.KyLuats.Find(id);

            // 2. Nếu không tìm thấy bản ghi -> Trả về false
            if (kyLuatEntity == null)
            {
                return false;
            }

            // 3. Nếu tìm thấy -> Đánh dấu bản ghi để xóa
            _context.KyLuats.Remove(kyLuatEntity);

            // 4. Thực hiện lệnh commit thay đổi xuống database
            // SaveChanges() trả về số lượng dòng bị ảnh hưởng.
            int rowsAffected = _context.SaveChanges();

            // Nếu có ít nhất 1 dòng bị ảnh hưởng, nghĩa là xóa thành công.
            return rowsAffected > 0;
        }
        public bool DeleteScoreById(int id)
        {
            // 1. Tìm bản ghi cần xóa trong DB theo khóa chính ID
            // Hàm .Find() rất hiệu quả để tìm theo primary key
            var ketQuaEntity = _context.KetQuaHocTaps.Find(id);

            // 2. Nếu không tìm thấy -> Trả về false
            if (ketQuaEntity == null)
            {
                return false;
            }

            // 3. Nếu tìm thấy -> Đánh dấu để xóa
            _context.KetQuaHocTaps.Remove(ketQuaEntity);

            // 4. Thực hiện lệnh xóa xuống DB
            int rowsAffected = _context.SaveChanges();

            // Trả về true nếu có ít nhất 1 dòng bị ảnh hưởng
            return rowsAffected > 0;
        }
    }
}
