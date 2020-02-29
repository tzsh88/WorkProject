using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web;
using System.Web.Http;
using WorkProject.Models;

namespace WorkProject.Controllers.BasicInfo
{
    public class ModifyPersonInfoController : ApiController
    {
       
        public HttpResponseMessage Get()
        {
            string sesName= UserSessionInfo.SessionName();
            //session为空 跳回登录界面
            if(sesName == "" )
            {
                HttpResponseMessage resp = new HttpResponseMessage(HttpStatusCode.Moved);
                string server_ip = ConfigSetting.GetIP();
                string server_port = ConfigSetting.GetIpPort();
                resp.Headers.Location = new Uri("https://" + server_ip + ":" + server_port);
                return resp;
            }
            else
            {

                using (WorkDataClassesDataContext db = new WorkDataClassesDataContext())
                {

                  List<Worker> workers = new List<Worker>();
                  workers.Add(db.Worker.Where(n => n.WorkName == sesName).First());

                  string json = JsonConvert.SerializeObject(workers);
                  return HttpResponseMessageToJson.ToJson(json);
                }
               

            }


        }

        /// <summary>
        /// 个人信息修改
        /// </summary>
        /// <param name="form">表单数据集合</param>
        /// <returns></returns>
        [HttpPost]
        [WebApiTracker]
        public void PostSessionUserName(FormDataCollection form)
        {
            //记得去除前后空格
            string name = form["name"].Trim();
            string phone = form["phone"].Trim();
            string card = form["card"].Trim();
            string workType = form["workType"].Trim();
            string sex = form["sex"].Trim();

            string sesName = UserSessionInfo.SessionName().Trim();
            //session为空 跳回登录界面
            if (sesName != "")
            {
                using (WorkDataClassesDataContext db = new WorkDataClassesDataContext())
                {

                    Worker woker = db.Worker.Where(n => n.WorkName == name).First();
                    woker.Remark = name + "+" + sex + "+" + phone + "+" + card + "+" + workType;
                    db.SubmitChanges();
                    
                    LogHelper.Monitor(name + "更新信息保存在Remark");
                }
            }

            


        }
    }
}
