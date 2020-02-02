using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkProject
{
    using System.Web;
    using System.Text.RegularExpressions;
    public class UrlRewriterFilter : IHttpModule
    {
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 负责在第一个管道事件上注册一个重写 index/1的url为 index.aspx?id=1
        /// </summary>
        /// <param name= "context" ></param>
        public void Init(HttpApplication context)
        {
            context.BeginRequest += context_BeginRequest;
        }

        void context_BeginRequest(object sender, EventArgs e)
        {
            //01.获取当前请求的原始url  index/1
            string url = HttpContext.Current.Request.RawUrl;
            //02.将当前url重写
            // 定义一个正则表达式来检查当前发送过来的url 是否为我要重写的index页面路径
            Regex reg = new Regex("/Views/Master.html");
            if (reg.IsMatch(url))
            {
                //https://localhost:44349/Views/Master.html
                string newUrl = reg.Replace(url, "/Views/Master.html");
                HttpContext.Current.RewritePath(newUrl);
            }
        }
    }

}
