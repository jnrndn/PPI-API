using System.ComponentModel.DataAnnotations;

namespace PPI.API.Dtos
{
    public class UserRegisterDto
    {
        [Required]
        public string Username { get; set; }
        
        [Required]
        [StringLength(20 ,MinimumLength = 8, ErrorMessage = "Password doesn't match requirements")]
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }
}