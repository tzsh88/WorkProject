using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkProject.Models
{
    public class BasicMethods
    {
        /// <summary>
        /// 所有工人的姓名，后期如果太多可以在做筛选
        /// </summary>
        /// <returns></returns>
        public static List<string> GetWorkName()
        {
            using (WorkDataClassesDataContext db = new WorkDataClassesDataContext())
            {
                return db.Worker.Where(n=>n.Visual==1).OrderBy(n=>n.WorkName).Select(n => n.WorkName).ToList();
            }
        }

        /// <summary>
        /// 所有未结束的工地
        /// </summary>
        /// <returns></returns>
        public static List<string> GetWorkSite()
        {
            using (WorkDataClassesDataContext db = new WorkDataClassesDataContext())
            {
                return db.WorkSite.Where(n=>n.WorkSiteName!="系统"&& n.WorkSiteName != "整体月度月度汇总" && n.WorkSiteFinished==0).OrderBy(n => n.WorkSiteName).Select(n => n.WorkSiteName).ToList();
            }
        }
        /// <summary>
        /// 根据名字检索工人 ID
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetWorkerId(string name)
        {
            using (WorkDataClassesDataContext db = new WorkDataClassesDataContext())
            {
                return db.Worker.Where(n => n.WorkName == name.Trim()).Select(n => n.WorkId).First();
            }
        }

        /// <summary>
        /// 根据工地名字检索工地 ID
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static int GetWorkerSiteId(string workerSite)
        {
            using (WorkDataClassesDataContext db = new WorkDataClassesDataContext())
            {
                return db.WorkSite.Where(n => n.WorkSiteName != "系统" && n.WorkSiteName== workerSite.Trim()).Select(n => n.WorkSiteId).First();
            }
        }
    }
}