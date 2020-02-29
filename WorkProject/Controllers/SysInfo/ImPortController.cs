using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Linq.Dynamic;
using System.Web.Http;
using WorkProject.Models;

namespace WorkProject.Controllers.SysInfo
{
    public class ImPortController : ApiController
    {
        /// <summary>
        /// 获取基础信息工人名字，以及工地信息
        /// </summary>
        /// <returns></returns>
        public HttpResponseMessage GetBasicInfo()
        {
            List<List<string>> list = new List<List<string>>();
            using (WorkDataClassesDataContext db = new WorkDataClassesDataContext())
            {
                list.Add(BasicMethods.GetWorkName());
                list.Add(BasicMethods.GetWorkSite());
                string json = JsonConvert.SerializeObject(list);
                return HttpResponseMessageToJson.ToJson(json);
            }


        }

        /// <summary>
        /// 获取管理与系统人员名下所属的工人名字，以及工地信息
        /// </summary>
        /// <returns></returns>
        public HttpResponseMessage GetLimitBasicInfo()
        {
            //当前登录人员
            string sesName = UserSessionInfo.SessionName();
            if (sesName != "")
            {
                 
                List<List<string>> list = new List<List<string>>();
                using (WorkDataClassesDataContext db = new WorkDataClassesDataContext())
                {
                    //判断是否为系统人员
                    if(db.Worker.Where(n=>n.WorkName==sesName && n.WorkType=="系统").Count()>=1)
                    {
                        list.Add(BasicMethods.GetWorkName());
                        list.Add(BasicMethods.GetWorkSite());
                        string json = JsonConvert.SerializeObject(list);
                        return HttpResponseMessageToJson.ToJson(json);
                    }
                    else
                    {  //管理人员
                        List<string> workers = db.Attendance.Where(n => n.WorkSite.WorkManage == sesName)
                                               .Select(n => n.Worker.WorkName).Distinct().ToList();
                        List<string> workSites = db.WorkSite.Where(n => n.WorkManage == sesName)
                                                   .Select(n => n.WorkSiteName).Distinct().ToList();
                        list.Add(workers);
                        list.Add(workSites);
                        string json = JsonConvert.SerializeObject(list);
                        return HttpResponseMessageToJson.ToJson(json);
                    }
                   
                }

            }
            else
            {
                return HttpResponseMessageToJson.ToJson("not logged in");
            }


        }


        /// <summary>
        /// 导入工人当天工作信息
        /// </summary>
        /// <param name="selectName"></param>
        /// <param name="date"></param>
        /// <param name="selectWorkSite"></param>
        /// <param name="work">当天工时</param>
        /// <param name="workMore">加班工时</param>
        /// <param name="remark">工作日志</param>
        /// <returns></returns>
        public HttpResponseMessage GetImportWork(string selectWeather, string selectName, string date, string selectWorkSite,
                string work, string workMore, string remark)
        {

            using (WorkDataClassesDataContext db = new WorkDataClassesDataContext())
            {
                string workId = BasicMethods.GetWorkerId(selectName);
                int workSiteId = BasicMethods.GetWorkerSiteId(selectWorkSite);


                Attendance att = new Attendance
                {
                    RecordTime = DateTime.Now,
                    WorkId = workId,
                    WorkSiteId = workSiteId,
                    WorkDate = Convert.ToDateTime(date),
                    WorkTime = Convert.ToDouble(work),
                    WorkMore = Convert.ToDouble(workMore),
                    WorkQuality = remark,
                    Weather = selectWeather
                };

                string json = InsertAttendance(att);
                json = JsonConvert.SerializeObject(json);
                return HttpResponseMessageToJson.ToJson(json);
            }


        }


        //数据导入并检查重复
        private string InsertAttendance(Attendance attendance)
        {
            using (WorkDataClassesDataContext db = new WorkDataClassesDataContext())
            {
                int eleCnt = db.Attendance.Where(n => n.WorkDate == attendance.WorkDate
                                  && n.WorkId == attendance.WorkId && n.WorkSiteId == attendance.WorkSiteId).Count();
                if (eleCnt == 0)
                {
                    db.Attendance.InsertOnSubmit(attendance);
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
        /// 导入工人工资支付情况
        /// </summary>
        /// <param name="selectName"></param>
        /// <param name="date"></param>
        /// <param name="selectWorkSite"></param>
        /// <param name="wageCard">CCB、JS、Cash </param>
        /// <param name="pay">金额</param>
        /// <param name="payType">支付方式</param>
        /// <param name="remark">缘由</param>
        /// <returns></returns>
        public HttpResponseMessage GetImportPay(string selectName, string date, string selectWorkSite,
               string wageCard, string pay, string payType, string remark)
        {

            using (WorkDataClassesDataContext db = new WorkDataClassesDataContext())
            {
                string workId = BasicMethods.GetWorkerId(selectName);
                int workSiteId = BasicMethods.GetWorkerSiteId(selectWorkSite);


                Payment p = new Payment
                {
                    RecordTime = DateTime.Now,
                    WorkId = workId,
                    WorkSiteId = workSiteId,
                    WagePaymentDate = Convert.ToDateTime(date),
                    WageAmount = Convert.ToInt16(pay),
                    PaymentType = payType,
                    WageCard = wageCard,
                    Remark = remark

                };

                string json = InsertPayment(p);
                json = JsonConvert.SerializeObject(json);
                return HttpResponseMessageToJson.ToJson(json);
            }


        }

        //数据导入并检查重复Payment
        private string InsertPayment(Payment p)
        {
            using (WorkDataClassesDataContext db = new WorkDataClassesDataContext())
            {
                int eleCnt = db.Payment.Where(n => n.WagePaymentDate == p.WagePaymentDate
                                  && n.WorkId == p.WorkId && n.WorkSiteId == p.WorkSiteId && n.WageAmount == p.WageAmount).Count();
                if (eleCnt == 0)
                {
                    db.Payment.InsertOnSubmit(p);
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
        /// 修改工人当天工作信息
        /// </summary>
        /// <param name="selectName"></param>
        /// <param name="date"></param>
        /// <param name="selectWorkSite"></param>
        /// <param name="work">当天工时</param>
        /// <param name="workMore">加班工时</param>
        /// <param name="remark">工作日志</param>
        /// <returns></returns>
        public HttpResponseMessage GetUpdateWork(string name, string date, string selectWorkSite,
                string work, string workMore, string remark)
        {

            using (WorkDataClassesDataContext db = new WorkDataClassesDataContext())
            {
                string workId = BasicMethods.GetWorkerId(name);
                int workSiteId = BasicMethods.GetWorkerSiteId(selectWorkSite);


                Attendance att = new Attendance
                {
                    RecordTime = DateTime.Now,
                    WorkId = workId,
                    WorkSiteId = workSiteId,
                    WorkDate = Convert.ToDateTime(date),
                    WorkTime = Convert.ToDouble(work),
                    WorkMore = Convert.ToDouble(workMore),
                    WorkQuality = remark

                };
                Attendance modifyAtt = db.Attendance.Where(n => n.WorkDate == att.WorkDate
                                       && n.WorkId == att.WorkId && n.WorkSiteId == att.WorkSiteId).SingleOrDefault();
               
                string json;
                if (modifyAtt != null)
                {
                    //不能直接更新在原表里，放在remark 
                    //modifyAtt.WorkTime = att.WorkTime;
                    //modifyAtt.WorkMore = att.WorkMore;
                    modifyAtt.Remark = att.RecordTime + "+"+ att.WorkTime + "+" + att.WorkMore + "+" + att.WorkQuality;
                    db.SubmitChanges();
                    json = "ok";
                }
                else
                {
                    json = "error";
                }

                json = JsonConvert.SerializeObject(json);
                return HttpResponseMessageToJson.ToJson(json);
            }


        }


        /// <summary>
        /// 导入数据稽核表格数据请求
        /// </summary>
        /// <param name="limit">页面大小</param>
        /// <param name="offset">偏移量</param>
        /// <param name="sort">排序字段</param>
        /// <param name="sortOrder">升序或降序</param>
        /// <returns></returns>
        public HttpResponseMessage GetCheckImport(int limit, int offset, string sort, string sortOrder)
        {
            string date = HttpContext.Current.Request["date"];
            string workSite = HttpContext.Current.Request["workSite"];
            int workSiteId = 0;
            bool siteEffect = false;
            if (workSite == "all")
            {
                siteEffect = true;
                workSiteId = 0;
            }
            else
            {
                workSiteId = BasicMethods.GetWorkerSiteId(workSite);
            }

            using (WorkDataClassesDataContext db = new WorkDataClassesDataContext())
            {
                var data = from s in db.Attendance
                           where s.WorkDate == Convert.ToDateTime(date) && (siteEffect || s.WorkSiteId == workSiteId)
                           select new
                           {
                               wSiteName = s.WorkSite.WorkSiteName,
                               wName = s.Worker.WorkName,
                               wDate = s.WorkDate.ToString(),
                               work = s.WorkTime,
                               workMore = s.WorkMore,
                               workQua = s.WorkQuality,
                               s.Weather
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
        /// 新增工人信息
        /// </summary>
        /// <param name="workName"></param>
        /// <param name="sex"></param>
        /// <param name="workType"></param>
        /// <param name="cardId"></param>
        /// <param name="phone"></param>
        /// <param name="ccb"></param>
        /// <param name="js"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage InsertWorker(string workName, int sex, string workType,
        string cardId, string phone, string ccb, string js)
        {
            using (WorkDataClassesDataContext db = new WorkDataClassesDataContext())
            {
                Worker w = new Worker
                {
                    WorkId = cardId,WorkName = workName,
                    Sex = sex,WorkType = workType,
                    Phone = phone, CCBPayCard = ccb,
                    JSPayCard = js
                };
                int wCnt = db.Worker.Where(n => n.WorkId == w.WorkId).Count();
                string json;
                if (wCnt == 0)
                {
                    db.Worker.InsertOnSubmit(w);
                    db.SubmitChanges();
                    json = "ok";
                }
                else
                {
                    json = "error";
                }

                json = JsonConvert.SerializeObject(json);
                return HttpResponseMessageToJson.ToJson(json);
            }


        }
        [HttpGet]
        public HttpResponseMessage InsertWorkSite(int workSiteId, string workSiteName,
                string com, string comBoss, string workManage, string remark)
        {
            using (WorkDataClassesDataContext db = new WorkDataClassesDataContext())
            {
                WorkSite wt = new WorkSite
                {
                    WorkSiteId = workSiteId,WorkSiteName = workSiteName,
                    Company = com,CompanyBoss = comBoss,
                    WorkManage = workManage,Remark = remark
                };
                int wCnt = db.WorkSite.Where(n => n.WorkSiteId == wt.WorkSiteId).Count();
                string json;
                if (wCnt == 0)
                {
                    db.WorkSite.InsertOnSubmit(wt);
                    db.SubmitChanges();
                    json = "ok";
                }
                else
                {
                    json = "error";
                }

                json = JsonConvert.SerializeObject(json);
                return HttpResponseMessageToJson.ToJson(json);
            }


        }

    }







}
