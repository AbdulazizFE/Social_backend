using System.ComponentModel.DataAnnotations.Schema;
namespace Work_Out.Dto
{
    public class PostDto
    {
        public string Content { get; set; } = "";
        public string Title { get; set; } = "";
        // public IFormFile Poster  { get; set; }
        public int AuthorId { get; set; }
        public string UserName { get; set; }
    }
}
