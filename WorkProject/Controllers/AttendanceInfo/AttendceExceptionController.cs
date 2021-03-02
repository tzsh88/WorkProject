using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Linq.Dynamic;
using WorkProject.Models;

namespace WorkProject.Controllers.AttendanceInfo
{
    public class AttendceExceptionController : ApiController
    {
        [HttpGet]
        /// <summary>
        /// 工人出勤表格数据请求
        /// </summary>
        /// <param name="limit">页面大小</param>
        /// <param name="offset">偏移量</param>
        /// <param name="sort">排序字段</param>
        /// <param name="sortOrder">升序或降序</param>
        /// <returns></returns>
        public HttpResponseMessage ExceptionShow(int limit, int offset, string sort, string sortOrder)
        {
            string worker = HttpContext.Current.Request["worker"].Trim();
            string year = HttpContext.Current.Request["year"].Trim();
            string mon = HttpContext.Current.Request["mon"].Trim();
            string day = HttpContext.Current.Request["day"].Trim();
            bool workerEffect = false; bool monEffect = false; bool dayEffect = false;

            if (worker == "all") workerEffect = true;

            if (mon == "all") { monEffect = true; mon = "01"; }

            if (day == "all") { dayEffect = true; day = "01"; }

            using (WorkDataClassesDataContext db = new WorkDataClassesDataContext())
            {

                var data = from s in db.AttendanceException
                           where (workerEffect || s.Worker.WorkName == worker)
                                 && s.WorkDate.Value.Year == Convert.ToInt32(year)
                                 && (monEffect || s.WorkDate.Value.Month == Convert.ToInt32(mon))
                                 && (dayEffect || s.WorkDate.Value.Day == Convert.ToInt32(day))
                           select new
                           {
                               s.Worker.WorkName,
                               s.RecordTime,
                               WorkDate = Convert.ToString(s.WorkDate.Value),
                               s.Worker.WorkType,
                               s.WorkTime,
                               s.WorkMore,
                               totalWork = s.WorkTime + s.WorkMore,
                               s.Handled,
                               s.Worker.Affiliation,
                               s.WorkSiteCnt

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


        [HttpGet]
        /// <summary>
        /// 异常数据生成
        /// </summary>
        /// <param name="year">年</param>
        /// <param name="mon">月</param>
        /// <returns></returns>
        public HttpResponseMessage Generate(string year, string mon)
        {
            string[] days = {"01","02","03", "04", "05", "06","07","08","09","10",
                             "11","12","13", "14", "15", "16","17","18","19","20",
                             "21","22","23", "24", "25", "26","27","28","29","30","31"};
            bool monEffect = false;

            if (mon == "all") { monEffect = true; mon = "01"; }


            using (WorkDataClassesDataContext db = new WorkDataClassesDataContext())
            {
                for (int t = 0; t < days.Length; t++)
                {//数据按工地分类了，需要汇总下
                    var data = from s in db.Attendance
                               where s.WorkDate.Value.Year == Convert.ToInt32(year)
                                     && (monEffect || s.WorkDate.Value.Month == Convert.ToInt32(mon))
                                     &&  s.WorkDate.Value.Day == Convert.ToInt32(days[t])
                               orderby s.Worker.WorkName
                               select new
                               {
                                   s.WorkId,
                                   s.Worker.WorkName,
                                   s.Worker.Sex,
                                   WorkDate = Convert.ToString(s.WorkDate.Value),
                                   s.Worker.WorkType,
                                   s.Weather,
                                   s.WorkTime,
                                   s.WorkMore,
                                   totalWork = s.WorkTime + s.WorkMore,
                                   s.WorkQuality,
                                   s.WorkSite.WorkManage,
                                   s.WorkSiteId,
                                   s.WorkSite.WorkSiteName,
                                   s.Worker.Affiliation,
                                   sum_WorkTime = (from g in db.Attendance
                                                   where g.WorkDate.Value.Year == Convert.ToInt32(year) && g.WorkId == s.WorkId
                                                         && (monEffect || g.WorkDate.Value.Month == Convert.ToInt32(mon))
                                                          && g.WorkDate.Value.Day == Convert.ToInt32(days[t])
                                                   select g).Sum(n => n.WorkTime),
                                   Cnt = (from g in db.Attendance
                                          where g.WorkDate.Value.Year == Convert.ToInt32(year) && g.WorkId == s.WorkId
                                                && (monEffect || g.WorkDate.Value.Month == Convert.ToInt32(mon))
                                                && g.WorkDate.Value.Day == Convert.ToInt32(days[t])
                                          select g).Count()
                               };

                    var dataNew = data.Where(n => n.Cnt > 1 || n.sum_WorkTime > 1|| n.WorkMore>0.9).OrderByDescending(n => n.Cnt).ThenBy(c => c.WorkName).ThenBy(x => x.WorkDate);

                    string[] workers = dataNew.Select(n => n.WorkId).Distinct().ToArray();

                    for (int i = 0; i < workers.Length; i++)
                    {
                        string[] workDates = dataNew.Where(n => n.WorkId == workers[i]).Select(n => n.WorkDate).Distinct().ToArray();
                        for (int j = 0; j < workDates.Length; j++)
                        {
                            var limitData = dataNew.Where(n => n.WorkId == workers[i] && n.WorkDate == workDates[j]);
                            int[] workSites = limitData.Select(n => n.WorkSiteId).Distinct().ToArray();

                            AttendanceException ae = new AttendanceException
                            {
                                WorkId = workers[i],
                                RecordTime = DateTime.Now,
                                WorkDate = Convert.ToDateTime(workDates[j]),
                                WorkMore = Math.Round((double)limitData.Where(n => n.WorkId == workers[i]).Sum(n => n.WorkMore), 4),
                                WorkTime = Math.Round((double)limitData.Where(n => n.WorkId == workers[i]).Sum(n => n.WorkTime), 4),
                                Handled = false,
                                WorkSiteCnt= limitData.Where(n => n.WorkId == workers[i]).Select(n=>n.Cnt).FirstOrDefault()
                            };
                            InsertAttendceException(ae);
                            break;
                        }

                    }
                }


                string json = "ok";
                json = JsonConvert.SerializeObject(json);
                return HttpResponseMessageToJson.ToJson(json);


            }


        }


        /// <summary>
        /// 删除出勤异常数据
        /// </summary>
        /// <param name="year"></param>
        /// <param name="mon"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage MonRemove(string year, string mon)
        {
            using (WorkDataClassesDataContext db = new WorkDataClassesDataContext())
            {
                int m = Convert.ToInt32(mon);
                if (m <= 9)
                {
                    mon = "0" + mon;
                }
                //删除还没有复位的数据
                string sql = "delete from AttendanceException  where CONVERT(varchar(6), WorkDate, 112)='" + year + mon + "' and handled=0 ";
                //表示所执行命令修改的行数。
                int c = db.ExecuteCommand(sql);
                string json = "";
                if (c > 0)
                {
                    json = "ok";
                    LogHelper.Monitor("\r\n" + year + mon + "删除出勤异常数据" + "\r\nIP:" + new WebApiMonitorLog().GetIP() + "\r\nControllerName:AttendceExceptionController");

                }
                else
                {
                    json = "nothing is deleted";
                }

                json = JsonConvert.SerializeObject(json);
                return HttpResponseMessageToJson.ToJson(json);


            }


        }
        /// <summary>
        /// 异常数据导入
        /// </summary>
        /// <param name="ae"></param>
        private void InsertAttendceException(AttendanceException ae)
        {
            using (WorkDataClassesDataContext db = new WorkDataClassesDataContext())
            {
                int preCnt = db.AttendanceException.Where(n => n.WorkDate == ae.WorkDate && n.WorkId == ae.WorkId).Count();
                if (preCnt == 0)
                {
                    db.AttendanceException.InsertOnSubmit(ae);
                    db.SubmitChanges();
                }


            }
        }

        /// <summary>
        /// 更新异常状态
        /// </summary>
        /// <param name="worker"></param>
        /// <param name="workDate"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage UpdateWork(string worker, string workDate)
        {



            using (WorkDataClassesDataContext db = new WorkDataClassesDataContext())
            {
                string workId = BasicMethods.GetWorkerId(worker.Trim());
                DateTime dt = Convert.ToDateTime(workDate);
                AttendanceException ae = db.AttendanceException.Where(n => n.WorkId == workId & n.WorkDate.Value.Year == dt.Year && n.WorkDate.Value.Month == dt.Month && n.WorkDate.Value.Day == dt.Day).FirstOrDefault();

                ae.Handled = true;
                db.SubmitChanges();

                //此处应从数据库中取得数据
                string json = "ok";
                return HttpResponseMessageToJson.ToJson(json);



            }


        }
    }
}
