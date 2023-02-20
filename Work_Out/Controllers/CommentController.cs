using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Work_Out.DataContext;
using Work_Out.Dto;
using Work_Out.Models;
using Work_Out.Tables;

namespace Work_Out.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommentController : ControllerBase
    {
        //Return data from database
        private readonly ApplicationContext _context;
        public static Author author = new Author();
        public static Post post = new Post();
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _hostEnvironment;
        public CommentController(ApplicationContext context, IConfiguration configuration, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _configuration = configuration;
            _hostEnvironment = hostEnvironment;
        }
        [HttpGet]
        public async Task<ActionResult<List<Comment>>> GetComments()
        {
            if (_context.Comments == null)
                return NotFound();
            // var pageResult = 6f;
            // var pageCount = Math.Ceiling(_context.Posts.Count() / pageResult);
            var comments = await _context.Comments
                    // .Take((int)pageResult)
                    .Include(u => u.Author)
                //.Include(u => u.Post)
                .Select(c => new
                {
                    c.Id,
                    c.CommentBody,
                    created_comment = c.PublishedComment.ToString("f"),
                    c.Author,
                    // c.Post,
        countcomment = c.Author.Comments.Count

                }).ToListAsync();

            return Ok(comments);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<List<Comment>>> GetCommentById(int id)
        {
            if (_context.Comments == null)
                return NotFound();
            var comments = await _context.Comments
                .Select(c => new
                {
                    c.Id,
                    c.CommentBody,
                    created_comment = c.PublishedComment.ToString("f"),
                    c.Author,
                    c.Post
                }).ToListAsync();

            return Ok(comments);
        }



        // [Authorize]
        [HttpPost]
        public async Task<ActionResult<List<Comment>>> Create([FromBody] CommentDto dto)
        {

            // var username = await _context.Comments.Where( x => x.UserName == dto.UserName).ToListAsync();
            // if(username == null)
            // return NotFound();
            var newComment = new Comment
            {
                CommentBody = dto.CommentBody,
                PublishedComment = DateTime.UtcNow,
                PostId = dto.PostId,
                UserName = dto.UserName,
                AuthorId = dto.AuthorId,
            };
            _context.Comments.Add(newComment);
            await _context.SaveChangesAsync();
            return Ok(newComment);
        }

        [HttpDelete("id")]
        public async Task<ActionResult<List<Comment>>> DeletePost(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
                return NotFound($"We don't have the {id} id, please try again!");
            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
            return Ok(await _context.Comments.ToListAsync());
        }
    }
}