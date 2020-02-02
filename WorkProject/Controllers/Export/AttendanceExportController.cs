using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;

namespace WorkProject.Controllers.Export
{

 
    public class AttendanceExportController : ApiController
    {



        //导出excel功能控制器
        
        public HttpResponseMessage GetExportData(string worker, string workSite, string year, string mon, string day)
        {
            var file = ExcelStream(worker,workSite,year,mon,day);
            //string csv = _service.GetData(model);
            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            result.Content = new StreamContent(file);
            //a text file is actually an octet-stream (pdf, etc)
            //result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.ms-excel");
            //we used attachment to force download
            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
            result.Content.Headers.ContentDisposition.FileName = "file.xls";
            return result;
        }

        //string worker, string workSite,string year, string mon, string day
        //得到excel文件流
        private System.IO.Stream ExcelStream(string worker, string workSite,string year, string mon, string day)
        {

            HSSFWorkbook hssfworkbook = new HSSFWorkbook();

            ISheet sheet1 = hssfworkbook.CreateSheet("出勤数据");

            IRow rowHeader = sheet1.CreateRow(0);

            //生成excel标题
            rowHeader.CreateCell(0).SetCellValue("姓名");
            rowHeader.CreateCell(1).SetCellValue("性别");
            rowHeader.CreateCell(2).SetCellValue("工种");
            rowHeader.CreateCell(3).SetCellValue("出勤日期");
            rowHeader.CreateCell(4).SetCellValue("天气");
            rowHeader.CreateCell(5).SetCellValue("工日");
            rowHeader.CreateCell(6).SetCellValue("加班工时");
            rowHeader.CreateCell(7).SetCellValue("总工时（天）");
            rowHeader.CreateCell(8).SetCellValue("工作日志");
            rowHeader.CreateCell(9).SetCellValue("工地");
            rowHeader.CreateCell(10).SetCellValue("管理员");

            //string worker = HttpContext.Current.Request["worker"].Trim();
            //string workSiteName = HttpContext.Current.Request["workSite"].Trim();
            //string year = HttpContext.Current.Request["year"].Trim();
            //string mon = HttpContext.Current.Request["mon"].Trim();
            //string day = HttpContext.Current.Request["day"].Trim();
            bool siteEffect = false; bool workerEffect = false; bool monEffect = false; bool dayEffect = false;
            if (workSite == "all") siteEffect = true;
            if (worker == "all") workerEffect = true;

            if (mon == "all") { monEffect = true; mon = "01"; }

            if (day == "all") { dayEffect = true; day = "01"; }

            using (WorkDataClassesDataContext db = new WorkDataClassesDataContext())
            {

                var data = from s in db.Attendance
                           where (siteEffect || s.WorkSite.WorkSiteName == workSite)
                                 && (workerEffect || s.Worker.WorkName == worker)
                                 && s.WorkDate.Value.Year == Convert.ToInt32(year)
                                 && (monEffect || s.WorkDate.Value.Month == Convert.ToInt32(mon))
                                 && (dayEffect || s.WorkDate.Value.Day == Convert.ToInt32(day))
                           orderby s.WorkDate
                           select new
                           {
                               s.Worker.WorkName, 
                               Sex=(s.Worker.Sex==0?"女":"男" ),
                               WorkDate = Convert.ToString(s.WorkDate.Value),
                               s.Weather,s.Worker.WorkType,s.WorkTime,
                               s.WorkMore,totalWork = s.WorkTime + s.WorkMore,
                               s.WorkQuality,s.WorkSite.WorkManage,
                               s.WorkSite.WorkSiteName
                           };
                var rowIndex = 1;

                foreach (var oo in data)
                {
                    IRow r = sheet1.CreateRow(rowIndex); ;
                    r.CreateCell(0).SetCellValue(oo.WorkName);
                    r.CreateCell(1).SetCellValue(oo.Sex);
                    r.CreateCell(2).SetCellValue(oo.WorkType);
                    r.CreateCell(3).SetCellValue(oo.WorkDate);
                    r.CreateCell(4).SetCellValue(oo.Weather);
                    r.CreateCell(5).SetCellValue((double)oo.WorkTime);
                    r.CreateCell(6).SetCellValue((double)oo.WorkMore);
                    r.CreateCell(7).SetCellValue((double)oo.totalWork);
                    r.CreateCell(8).SetCellValue(oo.WorkQuality);
                    r.CreateCell(9).SetCellValue(oo.WorkSiteName);
                    r.CreateCell(10).SetCellValue(oo.WorkManage);
                    rowIndex++;
                }




                MemoryStream file = new MemoryStream();
                hssfworkbook.Write(file);
                file.Seek(0, SeekOrigin.Begin);

                return file;


            }




        }
    }
}


