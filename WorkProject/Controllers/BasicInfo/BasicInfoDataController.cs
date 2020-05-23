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
                            s.WorkId,s.WorkName,s.Sex,s.Phone,s.WorkType,s.CCBPayCard,s.IC,s.Visual,
                            s.Affiliation
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
                           where  (siteEffect || s.WorkSiteName == workSiteName)
                           select new
                           {
                               s.WorkSiteId,s.WorkSiteName,s.WorkManage,s.Company,s.CompanyBoss,s.WorkSiteFinished
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
        /// 更新工人可见状态
        /// </summary>
        /// <param name="worker"></param>   
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage UpdateWorkerVisual(string worker)
        {

            using (WorkDataClassesDataContext db = new WorkDataClassesDataContext())
            {
                string workId = BasicMethods.GetWorkerId(worker.Trim());
              
                Worker w = db.Worker.Where(n => n.WorkId == workId).FirstOrDefault();

                if(w.Visual==1)
                {
                    w.Visual = 0;
                    db.SubmitChanges();
                }
                else
                {
                    w.Visual = 1;
                    db.SubmitChanges();
                }

               //此处应从数据库中取得数据
                string json = "ok";
                return HttpResponseMessageToJson.ToJson(json);



            }


        }

        /// <summary>
        /// 更新工人可见状态
        /// </summary>
        /// <param name="worker"></param>   
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage UpdateWorkSIteFinshish(string workSiteId)
        {

            using (WorkDataClassesDataContext db = new WorkDataClassesDataContext())
            {

                WorkSite w = db.WorkSite.Where(n => n.WorkSiteId ==Convert.ToInt32(workSiteId.Trim())).FirstOrDefault();

                if (w.WorkSiteFinished == 1)
                {
                    w.WorkSiteFinished = 0;
                    db.SubmitChanges();
                }
                else
                {
                    w.WorkSiteFinished = 1;
                    db.SubmitChanges();
                }

                //此处应从数据库中取得数据
                string json = "ok";
                return HttpResponseMessageToJson.ToJson(json);



            }


        }

    }
}
