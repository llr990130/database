using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MySql.Data.MySqlClient;
using System.Configuration;
using database.Models;
using System.Security.Cryptography;
using System.Text;
using System.Web.Security;

namespace database.Controllers
{
    [RoutePrefix("api/User")]
    public class userController : ApiController
    {
        //数据库相关操作
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
            //连接数据库
            MySqlConnection mysql = getMySqlConnection();
            try
            {
                //打开数据库
                mysql.Open();
                //对传入的密码进行MD5加密
                user userMD5 = new user();
                userMD5.username = user.username;
                userMD5.password = getMD5(user.password);
                //验证用户名和密码是否正确
                if (!ValidateUser(userMD5))
                {
                    return new { result = false };
                }
                FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(0, user.username, DateTime.Now,
                                DateTime.Now.AddHours(1), true, string.Format("{0}&{1}", userMD5.username, userMD5.password),
                                FormsAuthentication.FormsCookiePath);
                var oUser = new user { result = true, username = userMD5.username, password = userMD5.password, Ticket = FormsAuthentication.Encrypt(ticket)};
                return oUser;
            }
            catch(Exception ex)
            {
                string exception = ex.Message.ToString();
                /***********写入错误日志***************/
                return new { result = false };
            }
            finally
            {
                //关闭数据库
                mysql.Close();
            }
        }

        [HttpGet]
        [HttpPost]
        //注册
        public bool register(user user)
        {
            //连接数据库
            MySqlConnection mysql = getMySqlConnection();
            try
            {
                mysql.Open();
                //对密码进行MD5加密
                string passwordStore = getMD5(user.password);
                string sqlString = "insert into user(username,password,createTime) values ('" + user.username + "','" + passwordStore + "','" + user.createTime + "')";
                MySqlCommand insertUser = new MySqlCommand(sqlString, mysql);
                if (insertUser.ExecuteNonQuery() > 0)
                {
                    return true;
                }
                return false;
            }
            catch(Exception ex)
            {
                string exception = ex.Message.ToString();
                /***********写入错误日志***************/
                return false;
            }
            finally
            {
                mysql.Close();
            }
        }

        //记录用户登录的行为
        [HttpGet]
        [HttpPost]
        public void userLog(userLog log)
        {
            //连接数据库
            MySqlConnection mysql = getMySqlConnection();
            try
            {
                mysql.Open();
                string userLog = "insert into userLog(username,dateTime, methods) VALUES('" + log.username + "','" + log.dateTime + "','" + log.methods + "')";
                MySqlCommand insertUserLog = new MySqlCommand(userLog, mysql);
                if(insertUserLog.ExecuteNonQuery() > 0)
                {
                    return;
                }
            }
            catch(Exception ex)
            {
                string exception = ex.Message.ToString();
                /***********写入错误日志***************/
                return;
            }
            finally
            {
                mysql.Close();
            }
        }
        [HttpGet]
        [HttpPost]
        //验证用户名是否唯一
        public bool isUsernameUnique(string str)
        {
            //连接数据库
            MySqlConnection mysql = getMySqlConnection();
            try
            {
                mysql.Open();
                string sqlString = "select count(*) from user where username = '" + str + "'";
                MySqlCommand mySqlCommand = getSqlCommand(sqlString, mysql);
                MySqlDataReader reader = mySqlCommand.ExecuteReader();
                while (reader.Read())
                {
                    string num = reader[0].ToString();
                    if(num == "1")
                    {
                        return false;
                    }
                }
                return true;
            }
            catch(Exception ex)
            {
                string exception = ex.Message.ToString();
                /***********写入错误日志***************/
                return false;
            }
            finally
            {
                mysql.Close();
            }
        }

        //对密码进行MD5加密
        private string getMD5(string str)
        {
            //初始化MD5对象
            MD5 md5 = MD5.Create();
            //将源字符串转化为byte数组
            Byte[] sourcebyte = Encoding.Default.GetBytes(str);
            //将源字符串转化为md5的byte数组
            Byte[] md5bytes = md5.ComputeHash(sourcebyte);
            //将MD5的byte数组再转化为MD5数组
            StringBuilder md5Array = new StringBuilder();
            foreach (Byte b in md5bytes)
            {
                md5Array.Append(b.ToString("x2"));
            }
            return md5Array.ToString();
        }

        //用户名密码验证
        private bool ValidateUser(user user)
        {
            //连接数据库
            MySqlConnection mysql = getMySqlConnection();
            try
            {
                mysql.Open();
                string sqlString = "Select * from user where username = '" + user.username + "' and password = '" + user.password + "'";
                MySqlCommand mySqlCommand = getSqlCommand(sqlString, mysql);
                MySqlDataReader reader = mySqlCommand.ExecuteReader();
                if (reader.Read())
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
            finally
            {
                mysql.Close();
           }
        }
    }
}
