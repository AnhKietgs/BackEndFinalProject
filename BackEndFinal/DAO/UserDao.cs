using BackEndFinal.Model;

namespace BackEndFinal.DAO
{
    public class UserDao
    {
        private readonly AppDbContext _context;

        public UserDao(AppDbContext context)
        {
            _context = context;
        }

        // Hàm thêm tài khoản mới
        public void AddUser(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
        }
        public void DeleteUser(string username)
        {
            var user = _context.Users.Find(username);
            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
            }
        }
        // Hàm kiểm tra đăng nhập
        public User? CheckLogin(string username, string password)
        {
            return _context.Users.FirstOrDefault(u => u.Username == username && u.Password == password);
        }
    }
}
