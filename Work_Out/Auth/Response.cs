using System.ComponentModel.DataAnnotations;
using Work_Out.Tables;

namespace Work_Out.User
{
    public class Response
    {
        public int Id { get; set; }
        public string Username { get; set; } = String.Empty;
        //public string UserName   { get; set; }
        public string token { get; set; } = "";
    }
}
