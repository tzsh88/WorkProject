using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;

namespace WorkProject.Models
{
    public class UserSessionInfo
    {
        /// <summary>
        /// 获取当前用户权限等级
        /// </summary>
        /// <returns></returns>
        public static int UserRoleRank()
        {
            int line = 0;
            if (HttpContext.Current.Session["userName"] != null)
            {
                var ses = HttpContext.Current.Session["userName"];


                using (WorkDataClassesDataContext db = new WorkDataClassesDataContext())
                {
                    line = db.User.Where(n => n.UserName == ses.ToString()).Select(n => (int)n.LineN).First();
                }
            }
            
            return line;

        }

        /// <summary>
        /// 根据用户session状态返回信息 
        /// </summary>
        /// <returns></returns>
        public static string SessionName()
        {
            string visit =Convert.ToString(HttpContext.Current.Session["userName"]);
          
             return visit;
          
        }
    }
}