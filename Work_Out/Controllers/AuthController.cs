using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
using Work_Out.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Work_Out.DataContext;
using Work_Out.Tables;

using Microsoft.EntityFrameworkCore;
using Work_Out.Dto;
using Work_Out.Users;
using Work_Out.Models;

namespace Work_Out.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        // Instance new User
        //public static Userr user = new Tables.Userr();
        public static Author author = new Author();
        private readonly IConfiguration _configuration;
        private readonly ApplicationContext _context;
        public AuthController(IConfiguration configuration, ApplicationContext context)
        {
            _configuration = configuration;
            _context = context;
        }
        // Register new User
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult<Response>> Register([FromBody] RegisterDto dto)
        {
    
             author.PasswordHash = null;
             author.PasswordSalt = null;
             author.UserName = null;
             author.Id = 0;

            CreatePasswordHash(dto.Password, out byte[] passwordHash, out byte[] passwordSalt);
            // Validate Users Email
            List<string> UsreName = await _context.Authors.Select(x => x.UserName).ToListAsync();
            if ((UsreName.Contains(dto.UserName)))
            {

                return BadRequest("The Email is already register!");
            }
            //Add New Values for new user
            author.PasswordHash = passwordHash;
            author.PasswordSalt = passwordSalt;
            author.UserName = dto.UserName;

            Response res = new Response();
            string token = CreateTokenForRegister(dto);
            res.Username = author.UserName;
            res.Id = author.Id;
            res.token = token;
            _context.Authors.Add(author);

            await _context.SaveChangesAsync();

 //return Ok($"The user {dto.Username} has been created");
   return Ok(res);
        }
        //Get All user
        [HttpGet("AllUser")]
        public async Task<ActionResult<Author>> GetAllUser()
        {
            var users = await _context.Authors.ToListAsync();
            return Ok(users);
        }

        //Update te user
        [HttpPut("updateUser")]
        public async Task<ActionResult<Author>> UpdateUser(int id, [FromBody] RegisterDto request)
        {
            var user = await _context.Authors.FindAsync(id);
            if (user == null)
                return NotFound($"We don't have the {id} id, please try again!");

            //CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);
            //Add New Values for new user
            user.UserName = request.UserName;
            //user.Password = request.Password;
            //user.ConfirmPassword = request.ConfirmPassword;
            _context.Authors.Update(user);
            await _context.SaveChangesAsync();
            return Ok(user);
        }
        // login User
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<Response>> Login([FromBody] Login request)
        {
            // Validate Users Email
            var user = await _context.Authors.FirstOrDefaultAsync(u => u.UserName == request.UserName);
            if (user == null)
            {
                return BadRequest("User not found!");
            }

            if (!VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
            {
                return BadRequest("please check again your email or Password!");
            }
            else
            {
                string token = CreateToken(request);
                Response response = new Response();
                response.Username = user.UserName;
                response.Id = user.Id;
                response.token = token;
                return Ok(response);
            }
        }
        //Get user by Id
        [HttpGet("{id}")]
        public async Task<ActionResult<Author>> GetUser(int id)
        {
            var getUserInfo = await _context.Authors.Where(x => x.Id == id).Include(p => p.Posts).Include(c => c.Comments).Select(c => new
                  {
                      c.Id,
                      c.UserName,
                  countPost = c.Posts.Count,
                  countComment = c.Comments.Count,
               
          
                post =  c.Posts.Select(x => new{
                    x.Id,
                    x.UserName,
                    x.Content,
                    x.Title,
                    x.AuthorId,
                   created=  x.Published.ToString("f"),
                    comment = x.Comment,

                 }).ToList()
                   
                  }).ToListAsync();
           
            return Ok(getUserInfo);
        }


         //Get user posts
        // [HttpGet("{id}")]
        // public async Task<ActionResult<Author>> GetUserPosts(int id)
        // {
        //     var GetUser = await _context.Authors.Where(x => x.Id == id).Include(p => p.Posts).Include(c => c.Comments).Select(c => new
        //           {
        //               c.Id,
        //               c.UserName,
        //          c.Comments,
        //          c.Posts,
                   
        //           }).ToListAsync();
           
        //     return Ok(GetUser);
        // }
        [HttpDelete("DeleteUser")]
        public async Task<IActionResult> DeleteUser([FromHeader] int id)
        {
            var deleteUser = await _context.Authors.FindAsync(id);
            if (deleteUser == null)
            {
                return NotFound("User not found");
            }
            _context.Authors.Remove(deleteUser);
            _context.SaveChanges();
            return Ok("The delete is completed :)");
        }
        // Forgot Password
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(string username)
        {
            var user = await _context.Authors.FirstOrDefaultAsync(u => u.UserName == username);
            if (user == null)
            {
                return BadRequest("User not found!");
            }

            user.PasswordRestToken = CreateRandomToken();
            user.RestTokenExpires = DateTime.Now.AddDays(1);
            await _context.SaveChangesAsync();
            return Ok("You may now reset your password");
        }
        //verify the token
        [HttpPost("verify")]
        public async Task<IActionResult> Verify(string token)
        {

            var user = await _context.Authors.FirstOrDefaultAsync(u => u.VerificationToken == token);
            // Validate Users Email
            if (user == null)
            {
                return BadRequest("Invalid token!");
            }
            user.VerifiedAt = DateTime.Now;
            await _context.SaveChangesAsync();
            return Ok("User verified! :)");
        }
        // Reset Password
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequest request)
        {
            var user = await _context.Authors.FirstOrDefaultAsync(u => u.PasswordRestToken == request.Token);
            if (user == null || user.RestTokenExpires < DateTime.Now)
            {
                return BadRequest("Invalid Token :(");
            }
            CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            user.PasswordRestToken = null;
            user.RestTokenExpires = null;
            await _context.SaveChangesAsync();
            return Ok("Password successfuly reset! :)");
        }
        private string CreateToken(Login user)
        {
            // Claim: Authanticate user
            List<Claim> claims = new List<Claim>
            {
                // define which properity to be check
                new Claim(ClaimTypes.Name, user.UserName),
            };
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            // Create token
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),// Days For token's valid
                signingCredentials: creds);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

        private string CreateTokenForRegister(RegisterDto user)
        {
            // Claim: Authanticate user
            List<Claim> claims = new List<Claim>
            {
                // define which properity to be check
                new Claim(ClaimTypes.Name, user.UserName),
            };
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            // Create token
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),// Days For token's valid
                signingCredentials: creds);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }
        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }
        private string CreateRandomToken()
        {
            return Convert.ToHexString(RandomNumberGenerator.GetBytes(64));
        }
        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
    }
}



//public async Task<IActionResult> Login([FromBody] Login request)
//public async Task<IActionResult> UserRegister(Register request)


//{

//    var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
//    // Validate Users Email
//    if (user == null)
//    {
//        return BadRequest("User not found!");
//    }


//    // Validate Users Password
//    if (!VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
//    {

//        return BadRequest("please check again your email or Password!");
//    }


//    if (user.VerifiedAt == null)
//    {
//        return BadRequest("Not verified!");
//    }
//    return Ok($"Welcome back, {user.Email}! :)");
//}

//{


//    if (_context.Users.Any(u => u.Email == request.Email))
//    {
//        return BadRequest("User Alreade exists!");
//    }

//    //Hashing the password 
//    CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);


//    //Create user
//    var user = new Client
//    {
//        Email = request.Email,
//        PasswordHash = passwordHash,
//        PasswordSalt = passwordSalt,
//        VerificationToken = CreateRandomToken()
//    };

//    _context.Users.Add(user);
//    await _context.SaveChangesAsync();
//    return Ok("User successfully created!");

//}