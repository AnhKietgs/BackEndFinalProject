using Microsoft.EntityFrameworkCore;
namespace BackEndFinal.Model
{
    //cổng kết nối giữa code và database
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        
        }

       
        public DbSet<SinhVien> SinhViens { get; set; }
        public DbSet<KetQuaHocTap> KetQuaHocTaps { get; set; }
        public DbSet<KyLuat> KyLuats { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Tạo sẵn tài khoản CTSV
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Username = "admin",    // Tên đăng nhập
                    Password = "@123",      // Mật khẩu (để đơn giản cho đồ án)
                    Role = "CTSV"          // Quyền hạn
                }
            );
        }
    }
}
