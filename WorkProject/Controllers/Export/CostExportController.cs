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
using WorkProject.Models;

namespace WorkProject.Controllers.Export
{

    public class CostExportController : ApiController
    {
        //导出excel功能控制器
        /// <summary>
        /// 个人明细成本数据（精确到天）
        /// </summary>
        /// <param name="worker"></param>
        /// <param name="workSite"></param>
        /// <param name="year"></param>
        /// <param name="mon"></param>
        /// <param name="day"></param>
        /// <param name="swageStr"></param>
        /// <param name="bwageStr"></param>
        /// <returns></returns>
        public HttpResponseMessage GetExportData(string worker, string workSite, string year, string mon, string day,string swageStr, string bwageStr)
        {
           
            int swage = 0; int bwage = 0;
            if (swageStr != "") swage = Convert.ToInt32(swageStr.Trim());
            if (bwageStr != "") bwage = Convert.ToInt32(bwageStr.Trim());
            var file = ExcelStream(worker, workSite, year, mon, day, swage, bwage);
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
        private System.IO.Stream ExcelStream(string worker, string workSite, string year, string mon, string day,int swage, int bwage)
        {

            HSSFWorkbook hssfworkbook = new HSSFWorkbook();

            ISheet sheet1 = hssfworkbook.CreateSheet("成本明细数据");

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
            rowHeader.CreateCell(8).SetCellValue("人工费");
            rowHeader.CreateCell(9).SetCellValue("工作日志");
            rowHeader.CreateCell(10).SetCellValue("工地");
            rowHeader.CreateCell(11).SetCellValue("管理员");

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
                           where //s.Worker.WorkType != "管理" //分包和系统 类别 是不会有工日这个概念的
                                 (siteEffect || s.WorkSite.WorkSiteName == workSite)
                                 && (workerEffect || s.Worker.WorkName == worker)
                                 && s.WorkDate.Value.Year == Convert.ToInt32(year)
                                 && (monEffect || s.WorkDate.Value.Month == Convert.ToInt32(mon))
                                 && (dayEffect || s.WorkDate.Value.Day == Convert.ToInt32(day))
                           orderby s.WorkDate
                           select new
                           {
                               s.Worker.WorkName,
                               Sex = (s.Worker.Sex == 0 ? "女" : "男"),
                               WorkDate = Convert.ToString(s.WorkDate.Value),
                               s.Weather,
                               s.Worker.WorkType,
                               s.WorkTime,
                               s.WorkMore,
                               totalWork = s.WorkTime + s.WorkMore,
                               //使用前提数据里面只含大、小工两者类型
                               spend = (s.Worker.WorkType1 == "小工" ? (s.WorkTime + s.WorkMore) * swage : (s.WorkTime + s.WorkMore) * bwage),
                               s.WorkQuality,
                               s.WorkSite.WorkManage,
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
                    r.CreateCell(8).SetCellValue((double)oo.spend);
                    r.CreateCell(9).SetCellValue(oo.WorkQuality);
                    r.CreateCell(10).SetCellValue(oo.WorkSiteName);
                    r.CreateCell(11).SetCellValue(oo.WorkManage);
                    rowIndex++;
                }




                MemoryStream file = new MemoryStream();
                hssfworkbook.Write(file);
                file.Seek(0, SeekOrigin.Begin);

                return file;


            }




        }

        /// <summary>
        /// 月度成本数据导出
        /// </summary>
        /// <param name="worker"></param>
        /// <param name="workSite"></param>
        /// <param name="year"></param>
        /// <param name="mon"></param>
        /// <param name="swageStr"></param>
        /// <param name="bwageStr"></param>
        /// <returns></returns>
        public HttpResponseMessage GetExportMonData(int part,string worker, string workSite, string year, string mon,  string swageStr, string bwageStr)
        {

            int swage = 0; int bwage = 0;
            if (swageStr != "") swage = Convert.ToInt32(swageStr.Trim());
            if (bwageStr != "") bwage = Convert.ToInt32(bwageStr.Trim());
            var file = ExcelStream(part,worker, workSite, year, mon, swage, bwage);
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
        private System.IO.Stream ExcelStream(int part, string worker, string workSite, string year, string mon, int swage, int bwage)
        {

            HSSFWorkbook hssfworkbook = new HSSFWorkbook();

            ISheet sheet1 = hssfworkbook.CreateSheet("月度成本数据");

            IRow rowHeader = sheet1.CreateRow(0);

            //生成excel标题
            rowHeader.CreateCell(0).SetCellValue("姓名");
            rowHeader.CreateCell(1).SetCellValue("性别");
            rowHeader.CreateCell(2).SetCellValue("工种");
            rowHeader.CreateCell(3).SetCellValue("出勤月份");         
            rowHeader.CreateCell(4).SetCellValue("工日（月）");
            rowHeader.CreateCell(5).SetCellValue("加班工时（月）");
            rowHeader.CreateCell(6).SetCellValue("总工日（月）");
            rowHeader.CreateCell(7).SetCellValue("人工费");
            rowHeader.CreateCell(8).SetCellValue("工地");
            rowHeader.CreateCell(9).SetCellValue("隶属");


            //设置单元格背景色
            ICellStyle cellStyle = hssfworkbook.CreateCellStyle();
            cellStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Grey25Percent.Index;
            cellStyle.FillPattern = FillPattern.SolidForeground;

            bool siteEffect = false; bool workerEffect = false; bool monEffect = false; 
            if (workSite == "all") siteEffect = true;
            if (worker == "all") workerEffect = true;

            if (mon == "all") { monEffect = true; mon = "01"; }

           

            using (WorkDataClassesDataContext db = new WorkDataClassesDataContext())
            {
                var data = from s in db.PredictionWages
                           where s.WholePart == part 
                                 &&(siteEffect || s.WorkSite.WorkSiteName == workSite)
                                 && (workerEffect || s.Worker.WorkName == worker)
                                 && s.WorkYear == year
                                 && (monEffect || s.WorkMon == mon)

                           select new
                           {
                               s.Worker.WorkName,
                               Sex = (s.Worker.Sex == 0 ? "女" : "男"),
                               WorkDate = s.WorkYear +"-"+ s.WorkMon,
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
              
                var rowIndex = 1;

                foreach (var oo in data)
                {
                    IRow r = sheet1.CreateRow(rowIndex);
                    
                    r.CreateCell(0).SetCellValue(oo.WorkName);
                    r.CreateCell(1).SetCellValue(oo.Sex);
                    r.CreateCell(2).SetCellValue(oo.WorkType);
                    r.CreateCell(3).SetCellValue(oo.WorkDate);
                    r.CreateCell(4).SetCellValue((double)oo.WorkTimeMon);
                    r.CreateCell(5).SetCellValue((double)oo.WorkMoreMon);
                    r.CreateCell(6).SetCellValue((double)oo.totalWork);
                    r.CreateCell(7).SetCellValue((double)oo.spend);
                    r.CreateCell(8).SetCellValue(oo.WorkSiteName);
                    r.CreateCell(9).SetCellValue(oo.Affiliation);
                    rowIndex++;
                    if (rowIndex % 2 == 0)
                    {
                        r.GetCell(0).CellStyle = cellStyle;
                        r.GetCell(1).CellStyle = cellStyle;
                        r.GetCell(2).CellStyle = cellStyle;
                        r.GetCell(3).CellStyle = cellStyle;
                        r.GetCell(4).CellStyle = cellStyle;
                        r.GetCell(5).CellStyle = cellStyle;
                        r.GetCell(6).CellStyle = cellStyle;
                        r.GetCell(7).CellStyle = cellStyle;
                        r.GetCell(8).CellStyle = cellStyle;
                        r.GetCell(9).CellStyle = cellStyle;
                        r.GetCell(10).CellStyle = cellStyle;
                    }
                    
                }

                LogHelper.Monitor("\r\n预测数据导出" + "\r\nIP:" + new WebApiMonitorLog().GetIP() + "\r\nControllerName:CostExportController");

                MemoryStream file = new MemoryStream();
                hssfworkbook.Write(file);
                file.Seek(0, SeekOrigin.Begin);

                return file;


            }




        }
    }
}
