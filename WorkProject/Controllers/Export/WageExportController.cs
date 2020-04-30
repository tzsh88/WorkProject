using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using WorkProject.Models;

namespace WorkProject.Controllers.Export
{
    public class WageExportController : ApiController
    {
        //导出excel功能控制器

        public HttpResponseMessage GetExportData(string worker, string workSite, string year, string mon, string day)
        {
            var file = ExcelStream(worker, workSite, year, mon, day);
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
        private System.IO.Stream ExcelStream(string worker, string workSite, string year, string mon, string day)
        {

            HSSFWorkbook hssfworkbook = new HSSFWorkbook();

            ISheet sheet1 = hssfworkbook.CreateSheet("支付工资");

            IRow rowHeader = sheet1.CreateRow(0);

            //生成excel标题
            rowHeader.CreateCell(0).SetCellValue("工地");
            rowHeader.CreateCell(1).SetCellValue("姓名");
            rowHeader.CreateCell(2).SetCellValue("性别");
            rowHeader.CreateCell(3).SetCellValue("支付日期");
            rowHeader.CreateCell(4).SetCellValue("工种");
            rowHeader.CreateCell(5).SetCellValue("金额");
            rowHeader.CreateCell(6).SetCellValue("银行");
            rowHeader.CreateCell(7).SetCellValue("支付方式");
            rowHeader.CreateCell(8).SetCellValue("隶属");
            rowHeader.CreateCell(9).SetCellValue("备注");

            //设置单元格背景色
            ICellStyle cellStyle = hssfworkbook.CreateCellStyle();
            cellStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Grey25Percent.Index;
            cellStyle.FillPattern = FillPattern.SolidForeground;

            bool siteEffect = false; bool workerEffect = false; bool monEffect = false; bool dayEffect = false;
            if (workSite == "all") siteEffect = true;
            if (worker == "all") workerEffect = true;

            if (mon == "all") { monEffect = true; mon = "01"; }

            if (day == "all") { dayEffect = true; day = "01"; }

            using (WorkDataClassesDataContext db = new WorkDataClassesDataContext())
            {

                var data = from s in db.Payment
                           where (siteEffect || s.WorkSite.WorkSiteName == workSite)
                                 && (workerEffect || s.Worker.WorkName == worker)
                                 && s.WagePaymentDate.Value.Year == Convert.ToInt32(year)
                                 && (monEffect || s.WagePaymentDate.Value.Month == Convert.ToInt32(mon))
                                 && (dayEffect || s.WagePaymentDate.Value.Day == Convert.ToInt32(day))
                           orderby s.Worker.WorkName
                           select new
                           {
                               s.WorkSite.WorkSiteName, s.Worker.WorkName,
                               Sex = (s.Worker.Sex == 0 ? "女" : "男"),
                               payDate = Convert.ToString(s.WagePaymentDate.Value),
                               s.Worker.WorkType,s.WageAmount,
                               Card = (s.WageCard == "CCB" ? "建行" : s.WageCard),
                               PayType = s.PaymentType, s.Remark,s.Worker.Affiliation
                           };

                int rowIndex = 1;

                foreach (var oo in data)
                {
                    IRow r = sheet1.CreateRow(rowIndex);
                    if (rowIndex % 2 == 0)
                    {
                        r.RowStyle.FillBackgroundColor = IndexedColors.Grey25Percent.Index;
                    }
                    r.CreateCell(0).SetCellValue(oo.WorkSiteName);
                    r.CreateCell(1).SetCellValue(oo.WorkName);
                    r.CreateCell(2).SetCellValue(oo.Sex);
                    r.CreateCell(3).SetCellValue(oo.payDate);
                    r.CreateCell(4).SetCellValue(oo.WorkType);
                    r.CreateCell(5).SetCellValue((double)oo.WageAmount);
                    r.CreateCell(6).SetCellValue(oo.Card);
                    r.CreateCell(7).SetCellValue(oo.PayType);
                    r.CreateCell(8).SetCellValue(oo.Affiliation);
                    r.CreateCell(9).SetCellValue(oo.Remark);
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

                LogHelper.Monitor("\r\n预测工资导出" + "\r\nIP:" + new WebApiMonitorLog().GetIP() + "\r\nControllerName:WageExportController");

                MemoryStream file = new MemoryStream();
                hssfworkbook.Write(file);
                file.Seek(0, SeekOrigin.Begin);
                return file;


            }




        }
    }
}
