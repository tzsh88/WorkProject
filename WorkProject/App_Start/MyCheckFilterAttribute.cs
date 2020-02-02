using System.Web.Mvc;

namespace WorkProject
{
    internal class MyCheckFilterAttribute : ActionFilterAttribute
    {
        public MyCheckFilterAttribute()
        {
        }

        public bool CheckFilter { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            if (CheckFilter)
            {
                //检测用户是否登录
                if (filterContext.HttpContext.Session["userName"] == null)
                {
                    filterContext.HttpContext.Response.Redirect("/Views/login.html");  //"/Home/AdminLogin"跳转的页面
                }
            }

        }
    }
}