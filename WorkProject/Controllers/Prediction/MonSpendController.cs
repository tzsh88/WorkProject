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
namespace WorkProject.Controllers.Prediction
{
    public class MonSpendController : ApiController
    {

        /// <summary>
        /// 生成月度工地总成本
        /// </summary>
        /// <param name="year"></param>
        /// <param name="mon"></param>
        /// <returns></returns>
        [HttpGet]

        public HttpResponseMessage MonWorkSiteSpend(int limit, int offset, string sort, string sortOrder)
        {
            using (WorkDataClassesDataContext db = new WorkDataClassesDataContext())
            {
                string year = HttpContext.Current.Request["year"].Trim();
                string mon = HttpContext.Current.Request["mon"].Trim();
                var data = from s in db.PredictionWages
                           where s.WorkYear == year && Convert.ToInt32(s.WorkMon) == Convert.ToInt32(mon)
                           select s;

                int[] workSites = data.Select(n => n.WorkSiteId).Distinct().ToArray();

                List<WorkSiteMonSpend> list = new List<WorkSiteMonSpend>();
                //分工地月度
                for (int i = 0; i < workSites.Length; i++)
                {
                    var limitData = data.Where(n => n.WorkSiteId == workSites[i]);
                    //人员类型
                    string[] affiliation = limitData.Where(n => n.WorkSiteId == workSites[i]).Select(n => n.Worker.Affiliation).Distinct().ToArray();

                    for (int j = affiliation.Length-1; j >=0 ; j--)
                    {
                        string[] worktype = limitData.Where(n => n.WorkSiteId == workSites[i] && n.Worker.Affiliation == affiliation[j])
                            .Select(n => n.Worker.WorkType1).Distinct().ToArray();
            
                        for (int p = 0; p < worktype.Length; p++)
                        {

                            double m = limitData.Where(n => n.WorkSiteId==workSites[i]&& n.Worker.Affiliation==affiliation[j]&& n.Worker.WorkType1 == worktype[p]).Select(n => (double)n.WorkTimeMon).Sum();
                            double t = limitData.Where(n => n.WorkSiteId == workSites[i] && n.Worker.Affiliation == affiliation[j] && n.Worker.WorkType1 == worktype[p]).Select(n => (double)n.WorkMoreMon).Sum();
    
                            WorkSiteMonSpend wsp = new WorkSiteMonSpend
                            {
                                workSiteName = db.WorkSite.Where(n => n.WorkSiteId == workSites[i]).Select(n => n.WorkSiteName).First(),
                                affiliation = affiliation[j],
                                worke_type= worktype[p],
                                value = (m + t).ToString("N2")
                              
                            };
                            list.Add(wsp);
                        }
                    }



                }
                //sortName排序的名称 sortType排序类型 （desc asc）
                var orderExpression = string.Format("{0} {1}", sort, sortOrder);
                string json = "{ \"total\":";
                var total = list.Count();
                json += total + ",\"rows\":";
                var rows = list.OrderBy(orderExpression).Skip(offset).Take(limit).ToList();
                json += JsonConvert.SerializeObject(rows);
                json += "}";
                return HttpResponseMessageToJson.ToJson(json);
              


            }


        }


        private class WorkSiteMonSpend
        {
            public string workSiteName;
            public string affiliation;
            public string worke_type;
            public string value;
        }








    }


}
