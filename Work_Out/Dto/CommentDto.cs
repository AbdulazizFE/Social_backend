using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Work_Out.Dto
{
    public class CommentDto
    {
        public string CommentBody { get; set; } = "";
        public int PostId { get; set; }
        public string UserName { get; set; } = "";
        public int AuthorId { get; set; }
    }
}