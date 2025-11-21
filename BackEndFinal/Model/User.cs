using System.ComponentModel.DataAnnotations;

namespace BackEndFinal.Model
{
    public class User
    {
        [Key]
        public required string Username { get; set; } 
        public required string Password { get; set; } 
        public required string Role { get; set; }     
    }
}
