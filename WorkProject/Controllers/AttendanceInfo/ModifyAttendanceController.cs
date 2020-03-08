using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using WorkProject.Models;

namespace WorkProject.Controllers.AttendanceInfo
{
    [WebApiTracker]
    public class ModifyAttendanceController : ApiController
    {

        /// <summary>
        /// 删除工人工作信息
        /// </summary>
        /// <param name="selectName"></param>
        /// <param name="date"></param>
        /// <param name="selectWorkSite"></param>
        /// <param name="work">当天工时</param>
        /// <param name="workMore">加班工时</param>
        /// <param name="remark">工作日志</param>
        /// <returns></returns>
        [HttpGet]
        public  HttpResponseMessage DeleteWork(string name,  string site, string date)
        {

            using (WorkDataClassesDataContext db = new WorkDataClassesDataContext())
            {
                string workId = BasicMethods.GetWorkerId(name);
                int workSiteId = BasicMethods.GetWorkerSiteId(site);

                DateTime dt = Convert.ToDateTime(date);
                Attendance att = db.Attendance.Where(n => n.WorkId == workId && n.WorkSiteId == workSiteId && n.WorkDate.Value.Year==dt.Year && n.WorkDate.Value.Month == dt.Month && n.WorkDate.Value.Day == dt.Day).FirstOrDefault();

                ModifyAttendanceRecord mtr = new ModifyAttendanceRecord
                {
                    ModifyType = "delete",
                    ModifyTime = DateTime.Now,
                    WorkId = att.WorkId,
                    WorkDate = Convert.ToDateTime(date),
                    WorkSiteId = att.WorkSiteId,
                    WorkTime = att.WorkTime,
                    WorkMore = att.WorkMore,
                    WorkQuality = att.WorkQuality

                };

                //导入删除记录和数据
                string json = InsertModifyAttendanceRecord(mtr);

                if (json == "ok")//只有导入成功删除数据才能删除
                {
                    db.Attendance.DeleteOnSubmit(att);
                    db.SubmitChanges();
                }
                else
                {
                    json = "error";
                }

                json = JsonConvert.SerializeObject(json);
                return HttpResponseMessageToJson.ToJson(json);
            }


        }

        //数据导入并检查重复
        private  string InsertModifyAttendanceRecord(ModifyAttendanceRecord mtr)
        {
            using (WorkDataClassesDataContext db = new WorkDataClassesDataContext())
            {   //完全不一样
                int eleCnt = db.Attendance.Where(n => n.WorkDate == mtr.WorkDate
                                  && n.WorkId == mtr.WorkId && n.WorkSiteId == mtr.WorkSiteId&&n.WorkTime==mtr.WorkTime && n.WorkMore==mtr.WorkMore && n.WorkQuality==mtr.WorkQuality).Count();
                if (eleCnt == 0)
                {
                    db.ModifyAttendanceRecord.InsertOnSubmit(mtr);
                    db.SubmitChanges();

                    return "ok";
                }
                else
                {
                    return "repeate";
                }

            }
        }


        /// <summary>
        /// 更新工人工作信息
        /// </summary>
        /// <param name="selectName"></param>
        /// <param name="date"></param>
        /// <param name="selectWorkSite"></param>
        /// <param name="work">当天工时</param>
        /// <param name="workMore">加班工时</param>
        /// <param name="remark">工作日志</param>
        /// <returns></returns>
         [HttpGet]
        public HttpResponseMessage UpdateWork(string name,  string site, string date, string work, string more, string detail)
        {

            using (WorkDataClassesDataContext db = new WorkDataClassesDataContext())
            {
                string workId = BasicMethods.GetWorkerId(name);
                int workSiteId = BasicMethods.GetWorkerSiteId(site);
                DateTime dt = Convert.ToDateTime(date);

                Attendance att = db.Attendance.Where(n => n.WorkId == workId && n.WorkSiteId == workSiteId && n.WorkDate.Value.Year == dt.Year && n.WorkDate.Value.Month == dt.Month && n.WorkDate.Value.Day == dt.Day).FirstOrDefault();
           
                ModifyAttendanceRecord mtr = new ModifyAttendanceRecord
                {
                    ModifyType = "update",
                    ModifyTime = DateTime.Now,
                    WorkId = att.WorkId,
                    WorkDate = Convert.ToDateTime(date),
                    WorkSiteId = att.WorkSiteId,
                    WorkTime = att.WorkTime,
                    WorkMore = att.WorkMore,
                    WorkQuality = att.WorkQuality
                };


                //导入需更新的记录和数据
                string json = UpdateModifyAttendanceRecord(mtr);

                if (json == "ok")//只有导入需更新的记录和数据才能更新
                {
                    att.WorkMore = Convert.ToDouble(more.Trim());
                    att.WorkTime = Convert.ToDouble(work.Trim());
                    att.WorkQuality = detail.Trim();              
                    db.SubmitChanges();
                }
                else
                {
                    json = "error";
                }

                json = JsonConvert.SerializeObject(json);
                return HttpResponseMessageToJson.ToJson(json);
            }


        }

        //数据导入并检查重复
        private  string UpdateModifyAttendanceRecord(ModifyAttendanceRecord mtr)
        {
            using (WorkDataClassesDataContext db = new WorkDataClassesDataContext())
            {
                //完全不一样
                int eleCnt = db.Attendance.Where(n => n.WorkDate == mtr.WorkDate
                                  && n.WorkId == mtr.WorkId && n.WorkSiteId == mtr.WorkSiteId && n.WorkTime == mtr.WorkTime && n.WorkMore == mtr.WorkMore && n.WorkQuality == mtr.WorkQuality).Count();
                if (eleCnt == 0)
                {
                    db.ModifyAttendanceRecord.InsertOnSubmit(mtr);
                    db.SubmitChanges();

                    return "ok";
                }
                else
                {
                    return "repeate";
                }

            }
        }
    }
}
