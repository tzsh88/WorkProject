﻿using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Script.Serialization;

namespace WorkProject.Models
{
    public class HttpResponseMessageToJson
    {
        /// <summary>
        ///将返回json数据封装成一个类
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static HttpResponseMessage ToJson(Object obj)
        {
            String str;
            if (obj is String || obj is Char)
            {
                str = obj.ToString();
            }
            else
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                str = serializer.Serialize(obj);
            }
            HttpResponseMessage result = new HttpResponseMessage { Content = new StringContent(str, Encoding.GetEncoding("UTF-8"), "application/json") };
            
            return result;
        }

        
    }
}