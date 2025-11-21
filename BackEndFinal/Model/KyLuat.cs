using System.ComponentModel.DataAnnotations.Schema;

namespace BackEndFinal.Model
{
    public class KyLuat
    {
        public int Id { get; set; }
        public required string MaSV { get; set; }
        public required string NoiDung { get; set; } // VD: "Cảnh cáo mức 1"
        public DateTime NgayQuyetDinh { get; set; }

        [ForeignKey("MaSV")]
        public  SinhVien? SinhVien { get; set; }
    }
}
