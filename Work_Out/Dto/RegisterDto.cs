using System.ComponentModel.DataAnnotations;

namespace Work_Out.Dto
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "User Name is required")]
        public string UserName { get; set; } = string.Empty;
        //[EmailAddress]
        //[Required(ErrorMessage = "Email is required")]
        //public String Email { get; set; } = String.Empty;
        // [Required(ErrorMessage = "Gender  is required")]
        // public string Gender { get; set; } = string.Empty;
        [Required(ErrorMessage = "Password is Required")]
        [RegularExpression("^((?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])|(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[^a-zA-Z0-9])|(?=.*?[A-Z])(?=.*?[0-9])(?=.*?[^a-zA-Z0-9])|(?=.*?[a-z])(?=.*?[0-9])(?=.*?[^a-zA-Z0-9])).{8,}$",
            ErrorMessage = "Passwords must be minimum 8 characters and can contain upper case, lower case, number (0-9) and special character")]
        public string Password { get; set; } = "";
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match")]
        public string ConfirmPassword { get; set; } = "";
    }
}
