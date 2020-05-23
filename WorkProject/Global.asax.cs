using System;
using System.Timers;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.SessionState;

namespace WorkProject
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            ////注册 log4net
            //log4net.Config.XmlConfigurator.Configure(
            //   new System.IO.FileInfo(AppDomain.CurrentDomain.BaseDirectory + "\\Config\\log4net.config")
            //);

            ////WebApiTrackerAttribute  全局启用
            //GlobalConfiguration.Configuration.Filters.Add(new WebApiTrackerAttribute());                     
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            //FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            ////定时器
            //Timer myTimer = new Timer();
            //myTimer.Elapsed += new ElapsedEventHandler(TimerHelp.MyTimer_Elapsed);
            //// 设置引发时间的时间间隔　此处设置为１秒（１０００毫秒）,必须设置不然会出现触发次数的问题
            //myTimer.Interval = 1000;
            //myTimer.Enabled = true;
            //myTimer.AutoReset = true;
        }



        /// <summary>
        ///  开启Session功能
        /// </summary>

        public override void Init()
        {
            this.PostAuthenticateRequest += (sender, e) => HttpContext.Current.SetSessionStateBehavior(SessionStateBehavior.Required);
            base.Init();
        }
    }
}
