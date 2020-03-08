using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Web;
using WorkProject.Models;

namespace WorkProject
{
    public static class TimerHelp
    {
        //　当时间发生的时候需要进行的逻辑处理等  
        // 在这里仅仅是一种方式，可以实现这样的方式很多．
        public static void MyTimer_Elapsed(object source, ElapsedEventArgs e)
        {
            int[] hourRange = { 6, 23 };//设置定时范围
            //得到 hour minute second　如果等于某个值就开始执行某个程序。  
            int intHour = e.SignalTime.Hour;
            int intMinute = e.SignalTime.Minute;
            int intSecond = e.SignalTime.Second;

            //定制时间； 比如 在19：30 ：00 的时候执行某个函数  
            int iHour = 20;
            int iMinute = 30;
            int iSecond = 00;
            try
            {
                // 设置　每个小时的３０分钟开始执行  
                //if (intHour >= hourRange[0] && intHour <= hourRange[1] && intMinute == iMinute && intSecond == iSecond)
                //{
                //    RunTheTask();
                //}

                //// 设置　每个小时的59分钟开始执行  
                //if (intHour >= hourRange[0] && intHour <= hourRange[1] && intMinute == iMinute1 && intSecond == iSecond)
                //{
                //    RunTheTask();
                //}

                ////用来判断程序每10分钟执行一次 hourRange
                //int res = intMinute % 10;
                //if (intHour >= hourRange[0] && intHour <= hourRange[1] && res == 0 && intSecond == iSecond)
                //{
                //    RunTheTask();
                //}

                // 设置　每天的20：３０：００开始执行程序  
                if (intHour == iHour && intMinute == iMinute && intSecond == iSecond)
                {
                    RunTheTask();

                }


            }
            catch (Exception)
            {

            }
        }

        private static void RunTheTask()
        {
            //在这里写你需要执行的任务
            //数据校验，主要是基于WorkersRelation, 通常情况下该表下的couple 结对出勤。
            using (WorkDataClassesDataContext db = new WorkDataClassesDataContext())
            {
                string yesday = DateTime.Now.AddDays(-1).ToString("yyyyMMdd");
                //配对关系出勤稽核
                string sql = " select b.WorkDate,a.WorkIdMain,a.WorkId" +
                            " from WorkersRelation a, Attendance b,Attendance e, Worker c,Worker d" +
                            " where a.Effect = 1 and CONVERT(varchar(100), b.WorkDate, 112)= '" + yesday + "'and CONVERT(varchar(100), e.WorkDate, 112)= '"+ yesday +"'" +
                            " and a.WorkIdMain = b.WorkId  and a.WorkId = e.WorkId and a.WorkIdMain = c.WorkId and a.WorkId = d.WorkId";
                var acData = db.ExecuteQuery<AttendanceCouple>(sql);
                       
                var wrData = from n in db.WorkersRelation
                             where n.Effect == true && !acData.Select(t=>t.WorkIdMain).ToArray().Contains(n.WorkIdMain)
                             select n;

                if (wrData.Count() > 0)
                {
                    foreach(var oo in wrData)
                    {
                        LogHelper.Error("Couple:" + oo.Worker.WorkName + " " + oo.Worker1.WorkName + " date:" + yesday);
                    }
                   
                }
                else
                {
                    LogHelper.Error( "date:" + yesday +"结对出勤正常");
                }
             
                


            }




        }

    }
    /// <summary>
    /// 结对出勤
    /// </summary>
    class AttendanceCouple
    {
        public DateTime WorkDate;
        public string WorkIdMain;
        public string WorkId;

    }

}