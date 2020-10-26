using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Security;

namespace database.Providers
{
    //自定义此特性用于接口的身份验证
    public class RequestAuthorizeAttribute : AuthorizeAttribute
    {
        //重写基类的验证方式，加入我们自定义的Ticket验证
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            //从http请求的头里面获取身份验证信息，验证是否是请求发起方的ticket
            var authorization = actionContext.Request.Headers.Authorization;
            if((authorization != null)&&(authorization.Parameter != null))
            {
                //解密用户ticket，并校验用户名密码是否匹配
                var encryptTicket = authorization.Parameter;
                if (ValidateTicket(encryptTicket))
                {
                    base.IsAuthorized(actionContext);
                }
                else
                {
                    HandleUnauthorizedRequest(actionContext);
                }
            }
            //如果取不到验证信息，并且不允许匿名访问，则返回未验证401
            else
            {
                var attributes = actionContext.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().OfType<AllowAnonymousAttribute>();
                bool isAnonymous = attributes.Any(a => a is AllowAnonymousAttribute);
                if (isAnonymous) base.OnAuthorization(actionContext);
                else HandleUnauthorizedRequest(actionContext);
            }
        }

        //校验用户名密码
        private bool ValidateTicket(string encryptTicket)
        {
            //解密Ticket
            var strTicket = FormsAuthentication.Decrypt(encryptTicket).UserData;
            //从Ticket中获取用户名和密码
            var index = strTicket.IndexOf("&");
            string strUser = strTicket.Substring(0, index);
            string strPwd = strTicket.Substring(index + 1);

            MySqlConnection mysql = getMySqlConnection();
            try
            {
                mysql.Open();
                string getUser = "Select * from user where username = '" + strUser + "' and password = '" + strPwd + "'";
                MySqlCommand mySqlCommand = getSqlCommand(getUser, mysql);
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

        public static MySqlConnection getMySqlConnection()
        {
            MySqlConnection mysql = new MySqlConnection(ConfigurationManager.ConnectionStrings["MySql"].ConnectionString);
            return mysql;
        }

        public static MySqlCommand getSqlCommand(String sql, MySqlConnection mysql)
        {
            MySqlCommand mySqlCommand = new MySqlCommand(sql, mysql);
            return mySqlCommand;
        }
    }
}