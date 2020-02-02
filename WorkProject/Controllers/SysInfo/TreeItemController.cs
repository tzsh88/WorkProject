using WorkProject.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WorkProject.Controllers
{
    public class TreeItemController : ApiController
    {
        /// <summary>
        /// 菜单树形结构Json数据
        /// </summary>
        /// <returns></returns>
        public HttpResponseMessage Get()
        {
            return HttpResponseMessageToJson.ToJson(FindMeanList());

        }


        /// <summary>
        /// 找到菜单树形结构
        /// </summary>
        /// <returns>菜单栏树形结构json数据</returns>
        private string FindMeanList()
        {
            Cliet clite = new Cliet();
            JieDian root = new JieDian();
            root.name = "主页";
            root.url = "../Views/AppViews/Index.html";
            root.id = "F000";
            clite.creatTheTree("F000", root);  //根节点的pid值为"0"

            string json = JsonConvert.SerializeObject(root);
            return json;


        }
    }
}
