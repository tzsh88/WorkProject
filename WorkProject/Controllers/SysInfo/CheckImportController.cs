using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using WorkProject.Models;
using System.Linq.Dynamic;
namespace WorkProject.Controllers.SysInfo
{
    public class CheckImportController : ApiController
    {
        /// <summary>
        /// 稽核数据导入
        /// </summary>
        /// <returns></returns>
        public HttpResponseMessage GetMessage(DateTime date)
        {
            using (WorkDataClassesDataContext db = new WorkDataClassesDataContext())
            {

                CheckWork cw = new CheckWork
                {
                    RecordTime = DateTime.Now,
                    CheckDate=date,
                    CheckResult = 1,
                    Remark = "数据稽核完成"
                };
          
                string json = InsertCheckWork(cw);
                json = JsonConvert.SerializeObject(json);
                return HttpResponseMessageToJson.ToJson(json);
            }
        }

        /// <summary>
        /// 稽核数据导入
        /// </summary>
        /// <returns></returns>
        public HttpResponseMessage GetMessage(DateTime date,string workName, string workSite)
        {
            using (WorkDataClassesDataContext db = new WorkDataClassesDataContext())
            {

                CheckWork cw = new CheckWork
                {
                    RecordTime = DateTime.Now,
                    CheckDate = date,
                    WorkName=workName.Trim(),
                    WorkSiteName=workSite.Trim(),
                    CheckResult = 0,
                    Remark = "有数据需二次稽核"
                };

                string json = InsertCheckWork(cw);
                json = JsonConvert.SerializeObject(json);
                return HttpResponseMessageToJson.ToJson(json);
            }
        }

        //数据导入并检查重复 CheckWork
        private string InsertCheckWork(CheckWork p)
        {
            using (WorkDataClassesDataContext db = new WorkDataClassesDataContext())
            {
                int eleCnt = db.CheckWork.Where(n => n.CheckDate==p.CheckDate).Count();
                if (eleCnt == 0)
                {
                    db.CheckWork.InsertOnSubmit(p);
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
        /// 表格数据请求
        /// </summary>
        /// <param name="limit">页面大小</param>
        /// <param name="offset">偏移量</param>
        /// <param name="sort">排序字段</param>
        /// <param name="sortOrder">升序或降序</param>
        /// <returns></returns>
        public HttpResponseMessage GetCheckResult(int limit, int offset, string sort, string sortOrder)
        {
            string date = HttpContext.Current.Request["date"];
            string workSite = HttpContext.Current.Request["workSite"];
         
            bool siteEffect = false;
            if (workSite == "all")
            {
                siteEffect = true;             
            }
            
            using (WorkDataClassesDataContext db = new WorkDataClassesDataContext())
            {
                var data = from s in db.CheckWork
                           where  (siteEffect || s.CheckDate== Convert.ToDateTime(date))
                           select s;

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
