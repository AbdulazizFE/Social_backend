using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Work_Out.Tables;

namespace Work_Out.Models
{
    public class Author
    {
                [Key]
        public int Id { get; set; }
        // public string Gender { get; set; }= "";
        public string UserName { get; set; } = "";
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public string? VerificationToken { get; set; }
        public DateTime? VerifiedAt { get; set; }
        public string? PasswordRestToken { get; set; }
        public DateTime? RestTokenExpires { get; set; }
        [JsonIgnore]
        public List<Post> Posts { get; set; }
        [JsonIgnore]
        public List<Comment> Comments { get; set; }
        //[Required(ErrorMessage = "Password is Required")]
        //[RegularExpression("^((?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])|(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[^a-zA-Z0-9])|(?=.*?[A-Z])(?=.*?[0-9])(?=.*?[^a-zA-Z0-9])|(?=.*?[a-z])(?=.*?[0-9])(?=.*?[^a-zA-Z0-9])).{8,}$",
        //ErrorMessage = "Passwords must be minimum 8 characters and can contain upper case, lower case, number (0-9) and special character")]
        //public string Password { get; set; }
        //[Compare("Password", ErrorMessage = "The password and confirmation password do not match")]
        //public string ConfirmPassword { get; set; }
        // public string Email { get; set; }
    }
}