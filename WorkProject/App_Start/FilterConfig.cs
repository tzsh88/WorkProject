using System.Web;
using System.Web.Mvc;

namespace WorkProject
{
    public class FilterConfig
    {
        //原来问题在FilterConfig 这个类里面，这个类只是对MVC配置起效,webapi的不生效巨坑
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            //filters.Add(new HandleErrorAttribute());//原始声明的Action验证器
            filters.Add(new MyCheckFilterAttribute() { CheckFilter = true });
        }
    }
}
