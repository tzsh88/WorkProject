using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.SessionState;
using WorkProject.Models;

namespace WorkProject
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            //ע�� log4net
            log4net.Config.XmlConfigurator.Configure(
               new System.IO.FileInfo(AppDomain.CurrentDomain.BaseDirectory + "\\Config\\log4net.config")
            );
           //log4net ȫ������
            GlobalConfiguration.Configuration.Filters.Add(new WebApiTrackerAttribute());          
           
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            //FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
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
