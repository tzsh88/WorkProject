using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.UI;

namespace WorkProject.Models
{
    public class ConfigSetting
    {


        /// <summary>
        /// 从当前读取登录用户workSiteName
        /// </summary>
        /// <returns></returns>
        [MyCheckFilter(CheckFilter = true)]//过滤检测登录
        public static string GetUserWorkSite()
        {

            string workSiteName = "";
            var ses = HttpContext.Current.Session["userName"];
            if (ses != null)
            {
                using (WorkDataClassesDataContext db = new WorkDataClassesDataContext())
                {
                    workSiteName = db.User.Where(n => n.UserName == ses.ToString()).Select(n => n.WorkSite.WorkSiteName).First();
                }
            }
            else
            {
                LoginGet();//如果用户登录状态失效，那就跳转
            }




            return workSiteName;

        }
        //如果用户登录状态失效，那就跳转
        public static HttpResponseMessage LoginGet()
        {

            using (WorkDataClassesDataContext db = new WorkDataClassesDataContext())
            {
                HttpResponseMessage result = new HttpResponseMessage();
                result.RequestMessage.CreateResponse(HttpStatusCode.Unauthorized, new { messages = "login information is useless", resultCode = 1 });

                return result;
            }

        }
       
        //从webconfig appsetting节点中读取数据
        public static string GetIP()
        {

            return ConfigurationManager.AppSettings["IP"];

        }
        public static string GetIpPort()
        {

            return ConfigurationManager.AppSettings["Port"];

        }
    }
}