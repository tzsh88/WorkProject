
using WorkProject.Models;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using System.Web.Http;

namespace WorkProject.Controllers
{
    /// <summary>
    /// 利用session保存当前登陆者信息
    /// </summary>
    public class UserLoginController : ApiController
    {
       /// <summary>
       /// 判断用户是否存在，存在获取用户名
       /// </summary>
       /// <param name="id">固定Id</param>
       /// <returns></returns>
        public HttpResponseMessage GetSessionUserName(int id)
        {
            string json = "[\"session is not exist\"]";
            if (HttpContext.Current.Session["userName"] != null)
            {
                var ses = HttpContext.Current.Session["userName"];
                json = "[" + ses.ToString() + "]";
            }

            return HttpResponseMessageToJson.ToJson(json);
        }
        /// <summary>
        /// 用Session保存user信息
        /// </summary>
        /// <param name="form">表单数据集合</param>
        /// <returns></returns>
        [HttpPost] [WebApiTracker]
        public HttpResponseMessage PostSessionUserName(FormDataCollection form)
        {
            string json = "[\"用户名或密码不正确\"]";
            string name = form["name"];
            string pw = form["pw"];
            using (WorkDataClassesDataContext db = new WorkDataClassesDataContext())
            {
                int i = db.User.Where(n => n.UserName == name && n.UserPassword == pw).Count();
                if (i >= 1)
                {
                    var context = HttpContext.Current;
                    //session赋值
                    context.Session["userName"] = name;
                    json = "验证成功";

                    
                    LogHelper.Monitor(name + "登录成功");

                    HttpResponseMessage resp = new HttpResponseMessage(HttpStatusCode.Moved);
                    string server_ip = ConfigSetting.GetIP();
                    string server_port = ConfigSetting.GetIpPort();
                    resp.Headers.Location = new Uri("https://" + server_ip + ":" + server_port + "/Views/Master.html");
                    return resp;

                }
                else
                {
                    return HttpResponseMessageToJson.ToJson(json);
                }
            }


        }
        /// <summary>
        /// 需要加载的html页面的路径
        /// </summary>
        /// <param name="path">路径字符串</param>
        /// <returns></returns>
        public HttpResponseMessage GetHtml(string path)
        {
            var httpResponseMessage = new HttpResponseMessage();
            httpResponseMessage.Content = new StringContent(File.ReadAllText(System.Web.HttpRuntime.AppDomainAppPath + path), Encoding.UTF8);
            httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
            return httpResponseMessage;
        }
    }
}
