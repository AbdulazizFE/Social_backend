using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using ServiceStack.DataAnnotations;
using Work_Out.Models;

namespace Work_Out.Tables
{
    public class Post
    {
        [Column("Id")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Content { get; set; } = "";
        // public byte[] Poster { get; set; } 
        public DateTime Published { get; set; }
        public string Title { get; set; } = "";
        [JsonIgnore]
        public virtual Author? Author { get; set; }
        public string UserName { get; set; }
        public int AuthorId { get; set; }

         // [JsonIgnore]
    public ICollection<Comment> ?Comment { get; set; }

    }
}
