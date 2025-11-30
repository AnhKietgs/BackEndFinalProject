using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackEndFinal.Model
{
    public class KetQuaHocTap
    {
        public int Id { get; set; }
        public required string MaSV { get; set; }
        public required string HocKy { get; set; } 
        public required string NamHoc { get; set; } 

        public double GPA { get; set; } // Hệ 4.0
        public int DiemRenLuyen { get; set; }
        public required string XepLoaiHocLuc { get; set; }
        public required string XepLoaiHocBong { get; set; } // "Xuất sắc", "Giỏi", "Khá", "Không"

        [ForeignKey("MaSV")]
        public virtual SinhVien? SinhVien { get; set; }
    }
}
