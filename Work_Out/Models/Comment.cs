using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using System.Linq;
using System.Threading.Tasks;
using Work_Out.Tables;

namespace Work_Out.Models
{
    public class Comment
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string CommentBody { get; set; } = "";
        public DateTime PublishedComment { get; set; }
        public string UserName { get; set; }
        public int AuthorId { get; set; }
        [JsonIgnore]
        public virtual Author ?Author { get; set; }
        public int PostId { get; set; }
       [JsonIgnore]
        public Post? Post { get; set; }

    }
}