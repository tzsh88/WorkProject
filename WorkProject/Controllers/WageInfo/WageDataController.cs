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

namespace WorkProject.Controllers.WageInfo
{
    public class WageDataController : ApiController
    {
        /// <summary>
        /// 工人工资支付表格数据请求
        /// </summary>
        /// <param name="limit">页面大小</param>
        /// <param name="offset">偏移量</param>
        /// <param name="sort">排序字段</param>
        /// <param name="sortOrder">升序或降序</param>
        /// <returns></returns>
        public HttpResponseMessage GetPayment(int limit, int offset, string sort, string sortOrder)
        {
            string worker = HttpContext.Current.Request["worker"].Trim();
            string workSiteName = HttpContext.Current.Request["workSite"].Trim();
            string year = HttpContext.Current.Request["year"].Trim();
            string mon = HttpContext.Current.Request["mon"].Trim();
            string day = HttpContext.Current.Request["day"].Trim();
            bool siteEffect = false; bool workerEffect = false; bool monEffect = false; bool dayEffect = false;

            if (workSiteName == "all") siteEffect = true;
            if (worker == "all") workerEffect = true;

            if (mon == "all") { monEffect = true; mon = "01"; }

            if (day == "all") { dayEffect = true; day = "01"; }

            using (WorkDataClassesDataContext db = new WorkDataClassesDataContext())
            {

                var data = from s in db.Payment
                           where (siteEffect || s.WorkSite.WorkSiteName == workSiteName)
                                 && (workerEffect || s.Worker.WorkName == worker)
                                 && s.WagePaymentDate.Value.Year == Convert.ToInt32(year)
                                 && (monEffect || s.WagePaymentDate.Value.Month == Convert.ToInt32(mon))
                                 && (dayEffect || s.WagePaymentDate.Value.Day == Convert.ToInt32(day))
                           select new
                           {
                               s.WorkSite.WorkSiteName,
                               s.Worker.WorkName,
                               s.Worker.Sex,
                               s.Worker.Affiliation,
                               payDate = Convert.ToString(s.WagePaymentDate.Value),
                               s.Worker.WorkType,
                               s.WageAmount,
                               Card = (s.WageCard == "CCB" ? "建行" : s.WageCard),
                               PayType = s.PaymentType,
                               Remark = s.Remark
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
