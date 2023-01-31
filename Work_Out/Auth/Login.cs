using System.ComponentModel.DataAnnotations;

namespace Work_Out.User
{
    public class Login
    {
        [Required(ErrorMessage = "Username is required")]
        public String UserName { get; set; } = String.Empty;
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = String.Empty;
    }
}
