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
namespace WorkProject.Controllers.AttendanceInfo
{
    
    public class AttendanceDataController : ApiController
    {
        /// <summary>
        /// 工人出勤表格数据请求
        /// </summary>
        /// <param name="limit">页面大小</param>
        /// <param name="offset">偏移量</param>
        /// <param name="sort">排序字段</param>
        /// <param name="sortOrder">升序或降序</param>
        /// <returns></returns>
        public HttpResponseMessage GetAttendancs(int limit, int offset, string sort, string sortOrder)
        {
            string worker = HttpContext.Current.Request["worker"].Trim();
            string workSiteName = HttpContext.Current.Request["workSite"].Trim();
            string year = HttpContext.Current.Request["year"].Trim();
            string mon = HttpContext.Current.Request["mon"].Trim();
            string day = HttpContext.Current.Request["day"].Trim();
            bool siteEffect = false; bool workerEffect = false; bool monEffect = false; bool dayEffect = false;

            if (workSiteName == "all") siteEffect = true;
            if (worker == "all")       workerEffect = true; 

            if (mon == "all") { monEffect = true; mon = "01"; }

            if (day == "all") { dayEffect = true; day = "01"; }

            using (WorkDataClassesDataContext db = new WorkDataClassesDataContext())
            {

                var data = from s in db.Attendance
                           where (siteEffect || s.WorkSite.WorkSiteName == workSiteName)
                                 && (workerEffect || s.Worker.WorkName == worker)
                                 && s.WorkDate.Value.Year == Convert.ToInt32(year)
                                 && (monEffect || s.WorkDate.Value.Month == Convert.ToInt32(mon))
                                 && (dayEffect || s.WorkDate.Value.Day == Convert.ToInt32(day))
                           select new
                           {
                               s.Worker.WorkName,
                               s.Worker.Sex,
                               WorkDate = Convert.ToString(s.WorkDate.Value),
                               s.Worker.WorkType, s.Weather,
                               s.WorkTime,
                               s.WorkMore,
                               totalWork = s.WorkTime + s.WorkMore,
                               s.WorkQuality,
                               s.WorkSite.WorkManage,
                               s.WorkSite.WorkSiteName,
                               s.Worker.Affiliation,
                               Cnt = (from g in db.Attendance
                                      where (siteEffect || g.WorkSite.WorkSiteName == workSiteName)
                                            && (workerEffect || g.Worker.WorkName == worker)
                                            && g.WorkDate.Value.Year == Convert.ToInt32(year)
                                            && (monEffect || g.WorkDate.Value.Month == Convert.ToInt32(mon))
                                            && (dayEffect || g.WorkDate.Value.Day == Convert.ToInt32(day))
                                            && g.WorkId == s.WorkId
                                      select g).Count()
                                       
                           };

                if(sort== "WorkName")
                {
                    var dataNew = data.OrderByDescending(n =>n.Cnt).ThenBy(c => c.WorkName).ThenBy(x=>x.WorkSiteName);              
                    //此处应从数据库中取得数据：
                    string json = "{ \"total\":";
                    var total = data.Count();
                    json += total + ",\"rows\":";
                    var rows = dataNew.Skip(offset).Take(limit).ToList();
                    json += JsonConvert.SerializeObject(rows);
                    json += "}";
                    return HttpResponseMessageToJson.ToJson(json);
                }
                else
                {
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


        /// <summary>
        /// 工人成本数据明细 表格数据请求
        /// </summary>
        /// <param name="limit">页面大小</param>
        /// <param name="offset">偏移量</param>
        /// <param name="sort">排序字段</param>
        /// <param name="sortOrder">升序或降序</param>
        /// <returns></returns>
        public HttpResponseMessage GetCost(int limit, int offset, string sort, string sortOrder)
        {
            string worker = HttpContext.Current.Request["worker"].Trim();
            string workSiteName = HttpContext.Current.Request["workSite"].Trim();
            string year = HttpContext.Current.Request["year"].Trim();
            string mon = HttpContext.Current.Request["mon"].Trim();
            string day = HttpContext.Current.Request["day"].Trim();
            string swageStr = HttpContext.Current.Request["swage"].Trim();
            string bwageStr = HttpContext.Current.Request["bwage"].Trim();
            int swage = 0; int bwage = 0;
            if (swageStr != "") swage = Convert.ToInt32(swageStr);
            if (bwageStr != "") bwage = Convert.ToInt32(bwageStr);

            bool siteEffect = false; bool workerEffect = false; bool monEffect = false; bool dayEffect = false;

            if (workSiteName == "all") siteEffect = true;
            if (worker == "all") workerEffect = true;

            if (mon == "all") { monEffect = true; mon = "01"; }

            if (day == "all") { dayEffect = true; day = "01"; }
          
            using (WorkDataClassesDataContext db = new WorkDataClassesDataContext())
            {

                var data = from s in db.Attendance
                           where  //s.Worker.WorkType != "管理" //分包和系统 类别 是不会有工日这个概念的
                                (siteEffect || s.WorkSite.WorkSiteName == workSiteName)
                                 && (workerEffect || s.Worker.WorkName == worker)
                                 && s.WorkDate.Value.Year == Convert.ToInt32(year)
                                 && (monEffect || s.WorkDate.Value.Month == Convert.ToInt32(mon))
                                 && (dayEffect || s.WorkDate.Value.Day == Convert.ToInt32(day))
                           select new
                           {
                               s.Worker.WorkName,
                               s.Worker.Sex,
                               WorkDate = Convert.ToString(s.WorkDate.Value),
                               s.Worker.WorkType,
                               s.Weather,
                               s.WorkTime,
                               s.WorkMore,
                               totalWork = s.WorkTime + s.WorkMore,
                               //使用前提数据里面只含大、小工两者类型 做了类似映射 用WorkType1里面没有管理
                               spend =Convert.ToInt32(s.Worker.WorkType1=="小工"?(s.WorkTime + s.WorkMore)*swage: (s.WorkTime + s.WorkMore) * bwage),
                               s.WorkQuality,
                               s.WorkSite.WorkManage,
                               s.WorkSite.WorkSiteName,
                               s.Worker.Affiliation
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
        /// 工人月度成本数据 表格数据请求
        /// </summary>
        /// <param name="limit">页面大小</param>
        /// <param name="offset">偏移量</param>
        /// <param name="sort">排序字段</param>
        /// <param name="sortOrder">升序或降序</param>
        /// <returns></returns>
        public HttpResponseMessage GetMonCost(int limit, int offset, string sort, string sortOrder)
        {
            int wholePart =Convert.ToInt32( HttpContext.Current.Request["part"].Trim());
            string worker = HttpContext.Current.Request["worker"].Trim();
            string workSiteName = HttpContext.Current.Request["workSite"].Trim();
            string year = HttpContext.Current.Request["year"].Trim();
            string mon = HttpContext.Current.Request["mon"].Trim();
            string swageStr = HttpContext.Current.Request["swage"].Trim();
            string bwageStr = HttpContext.Current.Request["bwage"].Trim();
            int swage = 0; int bwage = 0;
            if (swageStr != "") swage = Convert.ToInt32(swageStr);
            if (bwageStr != "") bwage = Convert.ToInt32(bwageStr);

            bool siteEffect = false; bool workerEffect = false; bool monEffect = false;           
            if (workSiteName == "all") siteEffect = true;
            if (worker == "all") workerEffect = true;
            if (mon == "all") { monEffect = true; mon = "01"; }
         

            using (WorkDataClassesDataContext db = new WorkDataClassesDataContext())
            {

                var data = from s in db.PredictionWages
                           where s.WholePart==wholePart
                                 &&(siteEffect || s.WorkSite.WorkSiteName == workSiteName)
                                 && (workerEffect || s.Worker.WorkName == worker)
                                 && s.WorkYear == year
                                 && (monEffect ||Convert.ToInt32(s.WorkMon) == Convert.ToInt32(mon))
                              
                           select new
                           {
                               s.Worker.WorkName,
                               s.Worker.Sex,
                               WorkDate = s.WorkYear + "-" + s.WorkMon,
                               s.Worker.WorkType,                             
                               s.WorkTimeMon,
                               s.WorkMoreMon,
                               totalWork = s.WorkTimeMon + s.WorkMoreMon,
                               //使用前提数据里面只含大、小工两者类型 做了类似映射 用WorkType1里面没有管理
                               spend = (s.Worker.WorkType1 == "小工" ? (s.WorkTimeMon + s.WorkMoreMon) * swage : (s.WorkTimeMon + s.WorkMoreMon) * bwage),
                               s.WorkSite.WorkManage,
                               s.WorkSite.WorkSiteName,
                               s.Worker.Affiliation
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
