using System.ComponentModel.DataAnnotations;

namespace Nest.ViewModels
{
    public class UserLoginVM
    {
        [Required,StringLength(30)]
        public string Username { get; set; }
        [Required,StringLength(40),DataType(DataType.Password)]
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}
