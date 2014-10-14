using System;
using System.Collections.Generic;
using System.Web;
using System.IO;

namespace MapgisEgov.AnalyInput.Common
{
    public class Log
    {
        public static void Write(string sLog)
        {
            #region 判断是否写日志
            string sIsLog = System.Configuration.ConfigurationManager.AppSettings["IsLog"]; //是否要记录
            if (!String.IsNullOrEmpty(sIsLog) && sIsLog == "0")
            {
                //如果配置了<add key="IsLog" value="0" />就指明不需要写日志；默认写日志
                return;
            }
            #endregion

            #region 日志内容按习惯写到TempFile文件夹下
            string sURL = System.Web.HttpContext.Current.Server.MapPath("~/TempFile");
            string sPath = sURL + "/Plot" + (System.DateTime.Today.ToString("yyyy-MM-dd")).Replace("-", "") + ".txt";
            if (!File.Exists(sPath))
            {
                StreamWriter sw = new StreamWriter(sPath);
                sw.Close();
            }
            WebUse.Logs.WriteLog(sPath, sLog);



            #endregion


        }
    }
}