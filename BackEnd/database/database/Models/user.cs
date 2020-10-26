using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace database.Models
{
    public class user
    {
        public string id { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public DateTime createTime { get; set; }
        public DateTime updateTime { get; set; }
        public string Ticket { get; set; }
        public bool result { get; set; }
    }

    public class user2
    {
        public string username { get; set; }
        public string password { get; set; }
    }





    public class userLog
    {
        public DateTime dateTime { get; set; }
        public string username { get; set; }
        public string methods { get; set; }
    }
}