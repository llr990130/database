using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MySql.Data.MySqlClient;
using System.Configuration;
using database.Models;

namespace database.Controllers
{
    [RoutePrefix("api/User")]
    public class userController : ApiController
    {
        //连接数据库
        public static MySqlConnection getMySqlConnection()
        {
            MySqlConnection mysql = new MySqlConnection(ConfigurationManager.ConnectionStrings["MySql"].ConnectionString);
            return mysql;
        }
        public static MySqlCommand getSqlCommand(string sql, MySqlConnection mysql)
        {
            MySqlCommand mySqlCommand = new MySqlCommand(sql, mysql);
            return mySqlCommand;
        }
        //登录
        [HttpGet]
        [HttpPost]
        public object Login(user user)
        {
             
        }
    }
}
