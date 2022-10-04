using System.ComponentModel.DataAnnotations;

namespace Nest.ViewModels
{
    public class UserRegistrationVM
    {
        [Required, StringLength(100)]
        public string Fullname { get; set; }
        [Required, StringLength(50),DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required,StringLength(30)]
        public string Username { get; set; }
        [Required,StringLength(40),DataType(DataType.Password)]
        public string Password { get; set; }
        [Required,StringLength(40),DataType(DataType.Password),Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }
    }
}
