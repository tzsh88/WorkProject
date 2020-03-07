using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WorkProject.Models;

namespace WorkProject.Controllers.Prediction
{
 
    public class MonDataGenerationController : ApiController
    {
        /// <summary>
        /// 生成月度工人月度总工日的数据
        /// </summary>
        /// <param name="year"></param>
        /// <param name="mon"></param>
        /// <returns></returns>
        [HttpGet]
        
        public HttpResponseMessage MonGenerate(string year, string mon)
        {
            using (WorkDataClassesDataContext db = new WorkDataClassesDataContext())
            {
                var data = from s in db.Attendance
                           where s.WorkDate.Value.Year == Convert.ToInt32(year) && s.WorkDate.Value.Month == Convert.ToInt32(mon)
                           select new
                           {
                               s.WorkId, s.Worker.Sex,
                               WorkDate = Convert.ToString(s.WorkDate.Value),
                               s.Worker.WorkType,
                               s.WorkTime,
                               s.WorkMore,                         
                               s.WorkSite.WorkManage,
                               s.WorkSiteId
                           };

                string[] workers = data.Select(n => n.WorkId).Distinct().ToArray();

                try
                {   //分工地月度
                    for (int i = 0; i < workers.Length; i++)
                    {
                        var limitData = data.Where(n => n.WorkId == workers[i]);
                        int[] workSites = limitData.Select(n => n.WorkSiteId).Distinct().ToArray();
                        for (int j = 0; j < workSites.Length; j++)
                        {
                            PredictionWages pw = new PredictionWages
                            {
                                WorkerId = workers[i],
                                WorkMon = mon,
                                WorkYear = year,
                                WorkSiteId = workSites[j],
                                WholePart=0,
                                WorkMoreMon =Math.Round((double)limitData.Where(n => n.WorkSiteId == workSites[j]).Sum(n => n.WorkMore),4),
                                WorkTimeMon =Math.Round((double)limitData.Where(n => n.WorkSiteId == workSites[j]).Sum(n => n.WorkTime),4)
                            };
                            InsertPredictionWages(pw);
                        }

                    }
                    //整体月度
                    for (int i = 0; i < workers.Length; i++)
                    {
                        var limitData = data.Where(n => n.WorkId == workers[i]);
                        int[] workSites = limitData.Select(n => n.WorkSiteId).Distinct().ToArray();
                        
                            PredictionWages pw = new PredictionWages
                            {
                                WorkerId = workers[i],
                                WorkMon = mon,
                                WorkYear = year,                               
                                WholePart = 1,
                                WorkSiteId = -1,//整体月度汇总
                                WorkMoreMon = Math.Round((double)limitData.Where(n => n.WorkId == workers[i]).Sum(n => n.WorkMore), 4),
                                WorkTimeMon = Math.Round((double)limitData.Where(n => n.WorkId == workers[i]).Sum(n => n.WorkTime), 4)
                            };
                            InsertPredictionWages(pw);
                       

                    }
                    string json = "ok";
                    json = JsonConvert.SerializeObject(json);
                    return HttpResponseMessageToJson.ToJson(json);
                }
                catch (Exception e)
                {
                    string json = "error";
                    json = JsonConvert.SerializeObject(json);
                    return HttpResponseMessageToJson.ToJson(json);
                }

            }


        }

        /// <summary>
        /// 生成删除月度工人月度总工日的数据
        /// </summary>
        /// <param name="year"></param>
        /// <param name="mon"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage MonRemove(string year, string mon)
        {
            using (WorkDataClassesDataContext db = new WorkDataClassesDataContext())
            {
               string sql = "delete from PredictionWages  where WorkMon='" + mon + "' and  WorkYear='" + year + "' ";
                //表示所执行命令修改的行数。
                int c = db.ExecuteCommand(sql);
                string json = "";
                if (c > 0)
                {
                    json = "ok";
                    LogHelper.Monitor("\r\n"+year + mon + "月度数据删除" + "\r\nIP:" + new WebApiMonitorLog().GetIP() + "\r\nControllerName:MonDataGenerationController");
           
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
        /// 月度数据导入
        /// </summary>
        /// <param name="pw"></param>
        private void InsertPredictionWages(PredictionWages pw)
        {
            using (WorkDataClassesDataContext db = new WorkDataClassesDataContext())
            {
                int preCnt = db.PredictionWages.Where(n => n.WorkMon == pw.WorkMon && n.WorkYear == pw.WorkYear 
                                                        && n.WorkerId==pw.WorkerId && n.WorkSiteId==pw.WorkSiteId&& n.WholePart==pw.WholePart).Count();
                if (preCnt == 0)
                {
                    db.PredictionWages.InsertOnSubmit(pw);
                    db.SubmitChanges();
                    LogHelper.Monitor("\r\n"+pw.WorkYear + pw.WorkMon + "月度数据生成" + "\r\nIP:" + new WebApiMonitorLog().GetIP() + "\r\nControllerName:MonDataGenerationController");
                }
              
             
            }
        }


    }
}
