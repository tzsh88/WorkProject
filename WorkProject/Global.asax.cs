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
            ////ע�� log4net
            //log4net.Config.XmlConfigurator.Configure(
            //   new System.IO.FileInfo(AppDomain.CurrentDomain.BaseDirectory + "\\Config\\log4net.config")
            //);

            ////WebApiTrackerAttribute  ȫ������
            //GlobalConfiguration.Configuration.Filters.Add(new WebApiTrackerAttribute());                     
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            //FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            ////��ʱ��
            //Timer myTimer = new Timer();
            //myTimer.Elapsed += new ElapsedEventHandler(TimerHelp.MyTimer_Elapsed);
            //// ��������ʱ���ʱ�������˴�����Ϊ���루�����������룩,�������ò�Ȼ����ִ�������������
            //myTimer.Interval = 1000;
            //myTimer.Enabled = true;
            //myTimer.AutoReset = true;
        }



        /// <summary>
        ///  ����Session����
        /// </summary>

        public override void Init()
        {
            this.PostAuthenticateRequest += (sender, e) => HttpContext.Current.SetSessionStateBehavior(SessionStateBehavior.Required);
            base.Init();
        }
    }
}
