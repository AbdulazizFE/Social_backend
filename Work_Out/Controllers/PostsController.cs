using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Work_Out.DataContext;
using Work_Out.Dto;
using Work_Out.Models;
using Work_Out.Tables;
using Work_Out.User;

namespace Work_Out.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class PostsController : Controller
    {
        //Return data from database
        private readonly ApplicationContext _context;
        public static Post post = new Post();
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _hostEnvironment;
        public PostsController(ApplicationContext context, IConfiguration configuration, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _configuration = configuration;
            _hostEnvironment = hostEnvironment;
        }
        // private new List<string> _allowedExtenstions = new List<string>
        // {
        //    ".jpg" , ".png" , ".gif"
        // };
        // //byte
        // private long _maxAllowedPosterSize = 104857600;

    
        [HttpGet]
        public async Task<ActionResult<List<Post>>> ShowPost()
        {
            if (_context.Posts == null)
                return NotFound();
            // var pageResult = 6f;
            // var pageCount = Math.Ceiling(_context.Posts.Count() / pageResult);
            var posts = await _context.Posts
                // .Take((int)pageResult)
                .Include(u => u.Author)
                .Include(u => u.Comment)
                .Select(c => new
                {
                    c.Id,
                    c.Content,
                    c.Title,
                    Created = c.Published.ToString("f"),
             
                    c.Author,
                    c.Comment.Count
                }).OrderBy(x => x.Id).ToListAsync();

            return Ok(posts);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<List<Post>>> GetById(int id)
        {

            var post = await _context.Posts.Where(c => c.Id == id).

             Include(s => s.Comment)
            .Include(u => u.Author)
          .Select(c => new
          {
              c.Id,
              c.Content,
              c.Title,
              Created = c.Published.ToString("dddd, dd MMMM yyyy"),
              c.Author,
              c.Comment,

          }).ToListAsync();

            return Ok(post);
        }


        //Get by page
        [HttpGet("page{page}")]
        public async Task<ActionResult<List<Post>>> GetPagePost(int page)
        {

            if (_context.Posts == null)
                return NotFound();

            var pageResult = 6f;
            var pageCount = Math.Ceiling(_context.Posts.Count() / pageResult);
            var posts = await _context.Posts
                .Skip((page - 1) * (int)pageResult)
                .Take((int)pageResult)
                .ToListAsync();

            var response = new PostResponse
            {
                Posts = posts,
                CurrentPage = page,
                Pages = (int)pageCount
            };

            return Ok(posts);
        }


        [HttpPost("Create")]
        public async Task<ActionResult<List<Post>>> CreatePost([FromForm] PostDto dto)
        {
            //    var author = await _context.Authors.Select(x => x.Id).ToListAsync();
            //     if (author == null)
            //     {

            //         return NotFound("OBS! User not found.");
            //     }

            // if (dto.Poster == null)
            //     return BadRequest("Poster is reauired!");
            // if (!_allowedExtenstions.Contains(Path.GetExtension(dto.Poster.FileName).ToLower()))
            //     return BadRequest("Only .png and .jpg images are allowed!");
            // if (dto.Poster.Length > _maxAllowedPosterSize)
            //     return BadRequest("Max allowed size for poster is 1MB");
            // using var dataStream = new MemoryStream();
            // await dto.Poster.CopyToAsync(dataStream);

            var newPost = new Post
            {
                Content = dto.Content,
                AuthorId = dto.AuthorId,
                Published = DateTime.Now,
                Title = dto.Title,
                UserName = dto.UserName
            };

            //     // if (dto.Poster == null)
            //     //     return BadRequest("Poster is reauired!");
            //     // if (!_allowedExtenstions.Contains(Path.GetExtension(dto.Poster.FileName).ToLower()))
            //     //     return BadRequest("Only .png and .jpg images  and .gif are allowed!");
            //     // if (dto.Poster.Length > _maxAllowedPosterSize)
            //     //     return BadRequest("Max allowed size for poster is 1MB");
            _context.Posts.Add(newPost);
            await _context.SaveChangesAsync();
            return Ok(newPost);
            // return await GetById(newPost.AuthorId);
        }
        //[Authorize]
        [HttpDelete("DeletePost")]
        public async Task<ActionResult<List<Post>>> DeletePost(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post == null)
                return NotFound($"We don't have the {id} id, please try again!");
            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
            return Ok(await _context.Posts.ToListAsync());
        }

        // [Authorize]
        [HttpPut("edit")]
        public async Task<IActionResult> UpdatePost(int id, [FromForm] PostDto dto)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post == null)
                return NotFound($"We don't have the {id} id, please try again!");
            // if (dto.Poster != null)
            // {
            //     if (!_allowedExtenstions.Contains(Path.GetExtension(dto.Poster.FileName).ToLower()))
            //         return BadRequest("Only .png and .jpg images are allowed!");

            //     if (dto.Poster.Length > _maxAllowedPosterSize)
            //         return BadRequest("Max allowed size for poster is 1MB");

            //     using var dataStream = new MemoryStream();
            //     await dto.Poster.CopyToAsync(dataStream);

            //     post.Poster = dataStream.ToArray();
            // }

            if (dto.Content != null)
            {
                post.Content = dto.Content;

            }

            post.Title = dto.Title;
            // post.UserId = dto.UserId;
            _context.Update(post);
            _context.SaveChanges();
            return Ok("Successful!");
        }
    }
}
