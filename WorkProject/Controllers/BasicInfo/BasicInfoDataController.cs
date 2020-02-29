using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Linq.Dynamic;
using WorkProject.Models;
using Newtonsoft.Json;

namespace WorkProject.Controllers.BasicInfo
{
    public class BasicInfoDataController : ApiController
    {
        /// <summary>
        /// 人员信息表格数据请求
        /// </summary>
        /// <param name="limit">页面大小</param>
        /// <param name="offset">偏移量</param>
        /// <param name="sort">排序字段</param>
        /// <param name="sortOrder">升序或降序</param>
        /// <returns></returns>
        public HttpResponseMessage GetWorkers(int limit, int offset, string sort, string sortOrder)
        {
           
            string worker = HttpContext.Current.Request["worker"];
          
            bool workEffect = false;
            if (worker == "all")
            {
                workEffect = true;
                
            }
            

            using (WorkDataClassesDataContext db = new WorkDataClassesDataContext())
            {
                var data = from s in db.Worker
                           where (workEffect || s.WorkName == worker)
                           select new { 
                            s.WorkId,s.WorkName,s.Sex,s.Phone,s.WorkType,s.CCBPayCard 
                           };


                //sortName排序的名称 sortType排序类型 （desc asc）
                var orderExpression = string.Format("{0} {1}", sort, sortOrder);
                //此处应从数据库中取得数据：
                string json = "{ \"total\":";
                var total = data.Count();
                json += total + ",\"rows\":";
                var rows = data.OrderBy(orderExpression).Skip(offset).Take(limit).ToList();
                json += JsonConvert.SerializeObject(rows);
                json += "}";
                return HttpResponseMessageToJson.ToJson(json);
            }


        }

        /// <summary>
        /// 工地信息表格数据请求
        /// </summary>
        /// <param name="limit">页面大小</param>
        /// <param name="offset">偏移量</param>
        /// <param name="sort">排序字段</param>
        /// <param name="sortOrder">升序或降序</param>
        /// <returns></returns>
        public HttpResponseMessage GetWorkSites(int limit, int offset, string sort, string sortOrder)
        {
         
            string workSiteName = HttpContext.Current.Request["workSite"].Trim();

            bool siteEffect = false;
            if (workSiteName == "all")
            {
                siteEffect = true;

            }


            using (WorkDataClassesDataContext db = new WorkDataClassesDataContext())
            {
                var data = from s in db.WorkSite
                           where s.WorkSiteId!=0 && s.WorkSiteId != 4
                           && (siteEffect || s.WorkSiteName == workSiteName)
                           select new
                           {
                               s.WorkSiteId,s.WorkSiteName,s.WorkManage,s.Company,s.CompanyBoss
                           };

                //sortName排序的名称 sortType排序类型 （desc asc）
                var orderExpression = string.Format("{0} {1}", sort, sortOrder);
                //此处应从数据库中取得数据：
                string json = "{ \"total\":";
                var total = data.Count();
                json += total + ",\"rows\":";
                var rows = data.OrderBy(orderExpression).Skip(offset).Take(limit).ToList();
                json += JsonConvert.SerializeObject(rows);
                json += "}";
                return HttpResponseMessageToJson.ToJson(json);
            }


        }
    }
}
