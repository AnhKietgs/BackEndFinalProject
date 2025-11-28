
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;

namespace BackEndFinalEx.DTO.ThemDTO
{
    public class ThemSinhVienDTO
    {
     
            public required string MaSV { get; set; }
            public required string HoTen { get; set; }
            public required string DiaChi { get; set; }
            public required DateTime NgaySinh { get; set; }
            public required string SoDienThoai { get; set; }

            public required string GioiTinh { get; set; }
        
    }
}
