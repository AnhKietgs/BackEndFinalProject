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
            return _context.SinhViens
                .Include(sv => sv.KetQuaHocTaps)
                .Include(sv => sv.KyLuats)
                .FirstOrDefault(sv => sv.MaSV == maSV);
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
                .FirstOrDefault(k => k.MaSV == kq.MaSV && k.TenHocKy == kq.TenHocKy);

            if (existing != null)
            {
                existing.GPA = kq.GPA;
                existing.DiemRenLuyen = kq.DiemRenLuyen;
                existing.XepLoaiHocBong = kq.XepLoaiHocBong; // Cập nhật loại học bổng mới
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
    }
}
