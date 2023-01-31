using System;
using Work_Out.Tables;

namespace Work_Out
{
    public class PostResponse
    {
        public List<Post> Posts { get; set; } = new List<Post>();
        public int Pages { get; set; }
        public int CurrentPage { get; set; }
    }
}

