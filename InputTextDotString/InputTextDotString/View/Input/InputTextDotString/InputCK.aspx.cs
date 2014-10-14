using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CM.LC;
using System.Data;
using System.Text;
using System.Collections;
using System.IO;
using InputTextDotString.Models;
using System.Diagnostics;
using WebGis;

namespace InputTextDotString.View.Input.InputTextDotString
{
    public partial class InputCK : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                try
                {
                    #region 采矿
                    if (Request.QueryString["Subject"] == "CK")
                    {
                        #region 采矿权审批流程中入库页面方法
                        if (Request.QueryString["CASENO"] != string.Empty&&Request.QueryString["Input"] == "no")
                        {
                            this.btnInputFeature.Visible = false;
                            InputCK.strDotString = new StringBuilder();
                            StringBuilder Sql = new StringBuilder();
                            Sql.Append("select t.矿区坐标 from CM_LC_CKQ t where CASENO='" + Request.QueryString["CASENO"] + "'");
                            DatabaseORC db = new DatabaseORC();
                            DataSet ds = new DataSet();
                            ds = db.GetDataSet(Sql.ToString());
                            if (ds.Tables[0].Rows[0][0].ToString() == "在位置 0 处没有任何行。")
                            {
                                MapgisEgov.AnalyInput.Common.Log.Write("在位置 0 处没有任何行。");
                                return;
                            }
                            else
                            {
                                //ShowMessage(ds.Tables[0].Rows[0][0].ToString());
                                DataTable dt = new DataTable();
                                //dt = ds.Tables[0];
                                StringBuilder sbDotString = new StringBuilder();
                                sbDotString.Append(ds.Tables[0].Rows[0][0].ToString());//给全局变量sreDotString赋值
                                //InputCK.strDotString = new StringBuilder();
                                //InputCK.strDotString.Append(ds.Tables[0].Rows[0][0].ToString());//给全局变量赋值
                                //TextArea1.InnerText = InputCK.strDotString.ToString();
                                InputCK.strDotString = sbDotString;
                                int iPlotNo = 1;
                                dt.Columns.Add("序号", typeof(string));
                                dt.Columns.Add("X坐标", typeof(string));
                                dt.Columns.Add("Y坐标", typeof(string));
                                string[] arrPlotDotString = sbDotString.ToString().Split('#');
                                for (int i = 0; i < arrPlotDotString.Length; i++)
                                {
                                    string[] arrPlotNo = arrPlotDotString[i].Split('@');
                                    int iPoint = 1;
                                    for (int n = 0; n < arrPlotNo.Length; n++)
                                    {

                                        string[] arrDotString = arrPlotNo[n].ToString().Split(' ');
                                        for (int j = 0; j < arrDotString.Count(); j++)
                                        {
                                            string[] xy = arrDotString[j].ToString().Split(',');
                                            DataRow newRow = dt.NewRow();
                                            newRow["序号"] = iPlotNo.ToString() + "-" + iPoint.ToString();
                                            newRow["X坐标"] = xy[0];
                                            newRow["Y坐标"] = xy[1];
                                            dt.Rows.Add(newRow);
                                            //MapgisEgov.AnalyInput.Common.Log.Write("x="+xy[0] + "y="+xy[0]);
                                        }
                                        DataRow atRow = dt.NewRow();
                                        atRow["序号"] = "@";
                                        atRow["X坐标"] = "@";
                                        atRow["Y坐标"] = "@";
                                        dt.Rows.Add(atRow);
                                        iPoint++;
                                    }
                                    dt.Rows.RemoveAt(dt.Rows.Count - 1);
                                    DataRow sharpRow = dt.NewRow();
                                    sharpRow["序号"] = "#";
                                    sharpRow["X坐标"] = "#";
                                    sharpRow["Y坐标"] = "#";
                                    dt.Rows.Add(sharpRow);
                                    iPlotNo++;
                                }
                                dt.Rows.RemoveAt(dt.Rows.Count - 1);
                                if (dt.Rows.Count > 1)
                                {
                                    bindGridView(dt);
                                }
                            }
                        }
                        #endregion

                        #region 采矿权数据单独入库时传入GUID参数时的方法
                        else if (Request.QueryString["GUID"] != string.Empty&&Request.QueryString["Input"]=="yes")
                        {
                            InputCK.strDotString = new StringBuilder();
                            //StringBuilder Sql = new StringBuilder();
                            //Sql.Append("select t.区域坐标 from 采矿申请登记 t where CK_GUID='" + Request.QueryString["GUID"] + "'");
                            DatabaseORC db = new DatabaseORC();
                            //DataSet ds = new DataSet();
                            //ds = db.GetDataSet(Sql.ToString());
                            MapgisEgov.AnalyInput.Common.Log.Write(InputText.AnalyDotStringCTK(Request.QueryString["Subject"], Request.QueryString["GUID"]));
                            InputCK.strDotString = new StringBuilder(InputText.AnalyDotStringCTK(Request.QueryString["Subject"], Request.QueryString["GUID"]));
                            MapgisEgov.AnalyInput.Common.Log.Write("根据关键字Subject=" + Request.QueryString["Subject"] +"以及关键字GUID="+Request.QueryString["GUID"]+"取得坐标串结果为"+ InputCK.strDotString.ToString());
                            if (InputCK.strDotString.ToString() == "在位置 0 处没有任何行。")
                            {
                                return;
                            }
                            else
                            {
                                int iPlotNo = 1;
                                DataTable dt = new DataTable();
                                dt.Columns.Add("序号", typeof(string));
                                dt.Columns.Add("X坐标", typeof(string));
                                dt.Columns.Add("Y坐标", typeof(string));
                                string[] arrPlotDotString = InputCK.strDotString.ToString().Split('#');
                                //MapgisEgov.AnalyInput.Common.Log.Write("#隔开元素中第一个为"+strDotString.ToString().Split('#'));
                                for (int i = 0; i < arrPlotDotString.Length; i++)
                                {
                                    string[] arrPlotNo = arrPlotDotString[i].Split('@');
                                    int iPoint = 1;
                                    for (int n = 0; n < arrPlotNo.Length; n++)
                                    {

                                        string[] arrDotString = arrPlotNo[n].ToString().Split(' ');
                                        for (int j = 0; j < arrDotString.Count(); j++)
                                        {
                                            string[] xy = arrDotString[j].ToString().Split(',');
                                            DataRow newRow = dt.NewRow();
                                            newRow["序号"] = iPlotNo.ToString() + "-" + iPoint.ToString();
                                            newRow["X坐标"] = xy[0];
                                            newRow["Y坐标"] = xy[1];
                                            dt.Rows.Add(newRow);
                                            //MapgisEgov.AnalyInput.Common.Log.Write("x="+xy[0] + "y="+xy[0]);
                                        }
                                        DataRow atRow = dt.NewRow();
                                        atRow["序号"] = "@";
                                        atRow["X坐标"] = "@";
                                        atRow["Y坐标"] = "@";
                                        dt.Rows.Add(atRow);
                                        iPoint++;
                                    }
                                    dt.Rows.RemoveAt(dt.Rows.Count - 1);
                                    DataRow sharpRow = dt.NewRow();
                                    sharpRow["序号"] = "#";
                                    sharpRow["X坐标"] = "#";
                                    sharpRow["Y坐标"] = "#";
                                    dt.Rows.Add(sharpRow);
                                    iPlotNo++;
                                }
                                dt.Rows.RemoveAt(dt.Rows.Count - 1);
                                if (dt.Rows.Count > 1 && InputCK.strDotString.ToString() != string.Empty)
                                {
                                    bindGridView(dt);
                                }
                            }
                        }
                        #endregion

                        else if(Request.QueryString["CASENO"]==""&&Request.QueryString["GUID"]=="")
                        {
                            ShowMessage("未传入相关参数！请检查");
                        }

                        /*
                        MapgisEgov.AnalyInput.Common.Log.Write(ds.Tables[0].ToString());
                        string[] xy = ds.Tables[0].ToString().Split(',');
                        string JW = InputText.toJW(xy);
                         * */
                    }
                    #endregion
              
                    #region 探矿
                    else if (Request.QueryString["Subject"] == "TK")
                    {
                        InputCK.strDotString = new StringBuilder();

                        //MapgisEgov.AnalyInput.Common.Log.Write(jwDotString);

                        InputCK.strDotString = new StringBuilder(InputText.AnalyDotStringCTK(Request.QueryString["Subject"], Request.QueryString["CASENO"]));
                        
                        DatabaseORC db = new DatabaseORC();
                        string Sql="select KCQYGDZB from LC_KCXKZ where CASENO='" + Request.QueryString["CASENO"] + "'";
                        jwDotString = db.GetDataSet(Sql).Tables[0].Rows[0][0].ToString();
                        
                        MapgisEgov.AnalyInput.Common.Log.Write(jwDotString);
                        //MapgisEgov.AnalyInput.Common.Log.Write(InputCK.strDotString.ToString());
                        DataTable dt = new DataTable();
                        dt.Columns.Add("序号", typeof(string));
                        dt.Columns.Add("X坐标", typeof(string));
                        dt.Columns.Add("Y坐标", typeof(string));
                        int iPlotNo = 1;
                        string[] arrPlotDotString = InputCK.strDotString.ToString().Split('#');
                        for (int i = 0; i < arrPlotDotString.Length; i++)
                        {
                            string[] arrPlotNo = arrPlotDotString[i].Split('@');
                            int iPoint = 1;
                            for (int n = 0; n < arrPlotNo.Length; n++)
                            {

                                string[] arrDotString = arrPlotNo[n].ToString().Split(' ');
                                for (int j = 0; j < arrDotString.Count(); j++)
                                {
                                    string[] xy = arrDotString[j].ToString().Split(',');
                                    DataRow newRow = dt.NewRow();
                                    /*
                                    xy[0] = InputText.TranDegreeToDMs(Double.Parse(xy[0]));
                                    xy[1] = InputText.TranDegreeToDMs(Double.Parse(xy[1]));

                                    double jd = double.Parse(xy[0]);
                                    double wd = double.Parse(xy[1]);
                                    short DH = 37;
                                    short DH_width = 3;
                                    double LP = 111;
                                    xy = InputText.GeoToGauss(jd, wd).Split(',');
                                     * */
                                    newRow["序号"] = iPlotNo.ToString() + "-" + iPoint.ToString();
                                    newRow["X坐标"] = xy[0];
                                    newRow["Y坐标"] = xy[1];
                                    dt.Rows.Add(newRow);
                                    //MapgisEgov.AnalyInput.Common.Log.Write("x="+xy[0] + "y="+xy[0]);
                                }
                                DataRow atRow = dt.NewRow();
                                atRow["序号"] = "@";
                                atRow["X坐标"] = "@";
                                atRow["Y坐标"] = "@";
                                dt.Rows.Add(atRow);
                                iPoint++;
                            }
                            dt.Rows.RemoveAt(dt.Rows.Count - 1);
                            DataRow sharpRow = dt.NewRow();
                            sharpRow["序号"] = "#";
                            sharpRow["X坐标"] = "#";
                            sharpRow["Y坐标"] = "#";
                            dt.Rows.Add(sharpRow);
                            iPlotNo++;
                        }
                        dt.Rows.RemoveAt(dt.Rows.Count - 1);
                        bindGridView(dt);
                        MapgisEgov.AnalyInput.Common.Log.Write("探矿数据解析结果：" + InputCK.strDotString.ToString());
                    }
                    #endregion

                }
                catch (Exception oExcept)
                {
                    MapgisEgov.AnalyInput.Common.Log.Write(oExcept.Message);
                    return;
                    //ShowMessage("页面载入错误："+oExcept.Message);
                    /*
                    CloseWindow(true);
                     * */
                }
            }
        }

        protected void btnAnaly_Click(object sender, EventArgs e)
        {
            try
            {
                #region 打开本地采矿权管理系统功能测试
                //System.Diagnostics.Process.Start(@"C:\采矿权管理系统\采矿登记.exe");
                //System.Diagnostics.Process.Start("notepad.exe");//

                //ProcessStartInfo pInfo = new ProcessStartInfo();
                //pInfo.Arguments = "-ssh wp@192.168.1.188 -pw wp";
                //pInfo.FileName = Server.MapPath("C:/采矿权管理系统/采矿登记.exe");

                /*
                pInfo.FileName = "C:/采矿权管理系统/采矿登记.exe";
                Process p = new Process();
                p.StartInfo = pInfo;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardInput = true;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.CreateNoWindow = true;
                Process.Start(p.StartInfo);
                 * /
                /*
                StreamWriter myStreamWriter = p.StandardInput;
                myStreamWriter.WriteLine("sudo useradd -d /home/wp-s /bin/bash -m wp");
                myStreamWriter.WriteLine("sudo su root -c 'echo \"wp:wp\" | chpasswd'");

                myStreamWriter.WriteLine("exit");
                StreamReader ss = p.StandardOutput;

                //pand = ss.ReadLine().ToString();
                p.StandardInput.Flush();
                p.WaitForExit();
                p.Close();
                 * */
                
                /*
                System.Diagnostics.Process p = new System.Diagnostics.Process();
                p.StartInfo.UseShellExecute = true;
                p.StartInfo.FileName = @"C:\采矿权管理系统\采矿登记.exe";
                p.Start();*/
                #endregion

                #region 高斯数据解析
                //if (Request.QueryString["Subject"] == "CK")
                //{
                
                    if (FileUpload.PostedFile.FileName != string.Empty&&Request.QueryString["Subject"]=="CK")
                    {
                        
                        FileUpload fu = FileUpload;
                        //fu.SaveAs("../tempfile/UpLoadDot/" + DateTime.Now.Ticks.ToString() + fu.FileName.ToString());
                        //ShowMessage(Server.MapPath(".//TempFile//" + System.DateTime.Now.ToString("yyy-MM-dd-hh-mm-ss") + fu.FileName).ToString());//+ "/tempfile/UpLoadDot/" + System.DateTime.Now.Year + System.DateTime.Now.Month + System.DateTime.Now.Day + System.DateTime.Now.Hour + System.DateTime.Now.Minute + System.DateTime.Now.Second+System.DateTime.Now.Millisecond+ fu.FileName.ToString());
                        string sURL = System.Web.HttpContext.Current.Server.MapPath("~/TempFile/Dot");
                        string sPath = sURL + "/" + (Guid.NewGuid().ToString()).Replace("-", "") + ".txt";
                        fu.SaveAs(sPath);
                        InputText.BiaoZhunDotString BZDS = new InputText.BiaoZhunDotString();
                        string sMsg = string.Empty;
                        #region  根据坐标文件开头样式判断是简单坐标还是标准坐标，进行解析
                        StreamReader sr = new StreamReader(sPath, System.Text.Encoding.GetEncoding(936), true);
                        InputText.resolveFile(sr, BZDS, sMsg);
                        //ShowMessage(BZDS.strDots);
                        #endregion
                        TextArea1.InnerText = BZDS.strDots;
                        InputCK.strDotString = new StringBuilder(BZDS.strDots);
                        //WebUse.Logs.WriteLog(BZDS.sPath,BZDS.strDots);
                        StringBuilder sbMsg = new StringBuilder("解析成功！");
                        //ShowMessage(sbMsg.ToString());
                        #region 单地块简单坐标绑定方法
                        /*
                        DataTable dt = new DataTable();
                        dt.Columns.Add("序号", typeof(string));
                        dt.Columns.Add("X坐标", typeof(string));
                        dt.Columns.Add("Y坐标", typeof(string));
                        string[] arrDotString = strDotString.ToString().Split(' ');
                        for (int i = 0; i < arrDotString.Count(); i++)
                        {
                            string[] xy = arrDotString[i].ToString().Split(',');
                            #region 测试经纬度-平面坐标转换方法
                            double x = Convert.ToDouble(xy[0]);
                            double y = Convert.ToDouble(xy[1]);
                            double LP1 = -1000;
                            double x1=0;
                            double y1=0;
                            //string[] arrDot = ConvertDots.GaussToGeo(x, y, 3, x1, y1, LP1);
                            double[] arrDot = InputText.GaussToBL(y,x);
                            MapgisEgov.AnalyInput.Common.Log.Write(arrDot[0] + "||||||" + arrDot[1]);
                            #endregion
                            DataRow newRow = dt.NewRow();
                            newRow["序号"] = i + 1;
                            newRow["X坐标"] = xy[0];
                            newRow["Y坐标"] = xy[1];
                            dt.Rows.Add(newRow);
                        }
                        //MapgisEgov.AnalyInput.Common.Log.Write("PageLoad="+dt.ToString());
                        bindGridView(dt);
                        * */
                        #endregion

                        int iPlotNo = 1;
                        DataTable dt = new DataTable();
                        dt.Columns.Add("序号", typeof(string));
                        dt.Columns.Add("X坐标", typeof(string));
                        dt.Columns.Add("Y坐标", typeof(string));
                        string[] arrPlotDotString = InputCK.strDotString.ToString().Split('#');
                        for (int i = 0; i < arrPlotDotString.Length; i++)
                        {
                            string[] arrPlotNo = arrPlotDotString[i].Split('@');
                            int iPoint = 1;
                            for (int n = 0; n < arrPlotNo.Length; n++)
                            {

                                string[] arrDotString = arrPlotNo[n].ToString().Split(' ');
                                for (int j = 0; j < arrDotString.Count(); j++)
                                {
                                    string[] xy = arrDotString[j].ToString().Split(',');
                                    DataRow newRow = dt.NewRow();
                                    newRow["序号"] = iPlotNo.ToString() + "-" + iPoint.ToString();
                                    newRow["X坐标"] = xy[0];
                                    newRow["Y坐标"] = xy[1];
                                    dt.Rows.Add(newRow);
                                    //MapgisEgov.AnalyInput.Common.Log.Write("x="+xy[0] + "y="+xy[0]);
                                }
                                DataRow atRow = dt.NewRow();
                                atRow["序号"] = "@";
                                atRow["X坐标"] = "@";
                                atRow["Y坐标"] = "@";
                                dt.Rows.Add(atRow);
                                iPoint++;
                            }
                            dt.Rows.RemoveAt(dt.Rows.Count - 1);
                            DataRow sharpRow = dt.NewRow();
                            sharpRow["序号"] = "#";
                            sharpRow["X坐标"] = "#";
                            sharpRow["Y坐标"] = "#";
                            dt.Rows.Add(sharpRow);
                            iPlotNo++;
                        }
                        dt.Rows.RemoveAt(dt.Rows.Count - 1);
                        bindGridView(dt);

                    }
                #endregion

                    #region 探矿权经纬度坐标转为高斯坐标
                    else if (FileUpload.PostedFile.FileName != string.Empty && Request.QueryString["Subject"] == "TK")
                    {

                        FileUpload fu = FileUpload;
                        string sURL = System.Web.HttpContext.Current.Server.MapPath("~/TempFile/Dot");
                        string sPath = sURL + "/" + (Guid.NewGuid().ToString()).Replace("-", "") + ".txt";
                        fu.SaveAs(sPath);
                        InputText.BiaoZhunDotString BZDS = new InputText.BiaoZhunDotString();
                        string sMsg = string.Empty;
                        #region  根据坐标文件开头样式判断是简单坐标还是标准坐标，进行解析
                        StreamReader sr = new StreamReader(sPath, System.Text.Encoding.GetEncoding(936), true);
                        InputText.resolveFile(sr, BZDS, sMsg);
                        #endregion
                        TextArea1.InnerText = BZDS.strDots;
                        jwDotString = InputText.transFormattedCTK(BZDS.strDots,"TK");
                        MapgisEgov.AnalyInput.Common.Log.Write(jwDotString);
                        InputCK.strDotString = new StringBuilder(InputText.GeoToGauss(BZDS.strDots));
                        //WebUse.Logs.WriteLog(BZDS.sPath,BZDS.strDots);
                        StringBuilder sbMsg = new StringBuilder("解析成功！");
                        //ShowMessage(sbMsg.ToString());
                        #region 单地块简单坐标绑定方法
                        /*
                        DataTable dt = new DataTable();
                        dt.Columns.Add("序号", typeof(string));
                        dt.Columns.Add("X坐标", typeof(string));
                        dt.Columns.Add("Y坐标", typeof(string));
                        string[] arrDotString = strDotString.ToString().Split(' ');
                        for (int i = 0; i < arrDotString.Count(); i++)
                        {
                            string[] xy = arrDotString[i].ToString().Split(',');
                            #region 测试经纬度-平面坐标转换方法
                            double x = Convert.ToDouble(xy[0]);
                            double y = Convert.ToDouble(xy[1]);
                            double LP1 = -1000;
                            double x1=0;
                            double y1=0;
                            //string[] arrDot = ConvertDots.GaussToGeo(x, y, 3, x1, y1, LP1);
                            double[] arrDot = InputText.GaussToBL(y,x);
                            MapgisEgov.AnalyInput.Common.Log.Write(arrDot[0] + "||||||" + arrDot[1]);
                            #endregion
                            DataRow newRow = dt.NewRow();
                            newRow["序号"] = i + 1;
                            newRow["X坐标"] = xy[0];
                            newRow["Y坐标"] = xy[1];
                            dt.Rows.Add(newRow);
                        }
                        //MapgisEgov.AnalyInput.Common.Log.Write("PageLoad="+dt.ToString());
                        bindGridView(dt);
                        * */
                        #endregion

                        int iPlotNo = 1;
                        DataTable dt = new DataTable();
                        dt.Columns.Add("序号", typeof(string));
                        dt.Columns.Add("X坐标", typeof(string));
                        dt.Columns.Add("Y坐标", typeof(string));
                        string[] arrPlotDotString = InputCK.strDotString.ToString().Split('#');
                        for (int i = 0; i < arrPlotDotString.Length; i++)
                        {
                            string[] arrPlotNo = arrPlotDotString[i].Split('@');
                            int iPoint = 1;
                            for (int n = 0; n < arrPlotNo.Length; n++)
                            {

                                string[] arrDotString = arrPlotNo[n].ToString().Split(' ');
                                for (int j = 0; j < arrDotString.Count(); j++)
                                {
                                    string[] xy = arrDotString[j].ToString().Split(',');
                                    DataRow newRow = dt.NewRow();
                                    newRow["序号"] = iPlotNo.ToString() + "-" + iPoint.ToString();
                                    newRow["X坐标"] = xy[0];
                                    newRow["Y坐标"] = xy[1];
                                    dt.Rows.Add(newRow);
                                    //MapgisEgov.AnalyInput.Common.Log.Write("x="+xy[0] + "y="+xy[0]);
                                }
                                DataRow atRow = dt.NewRow();
                                atRow["序号"] = "@";
                                atRow["X坐标"] = "@";
                                atRow["Y坐标"] = "@";
                                dt.Rows.Add(atRow);
                                iPoint++;
                            }
                            dt.Rows.RemoveAt(dt.Rows.Count - 1);
                            DataRow sharpRow = dt.NewRow();
                            sharpRow["序号"] = "#";
                            sharpRow["X坐标"] = "#";
                            sharpRow["Y坐标"] = "#";
                            dt.Rows.Add(sharpRow);
                            iPlotNo++;
                        }
                        dt.Rows.RemoveAt(dt.Rows.Count - 1);
                        bindGridView(dt);

                    }

                    else
                    {
                        ShowMessage("请选择坐标文件后点击解析！");
                        //return;
                    }
                    #endregion

            }
            catch (Exception oExcept)
            {
                //ShowMessage("解析坐标失败，错误原因:"+oExcept.Message);
                ShowMessage("解析故障："+oExcept.Message);
            }
        }

        protected void btnInput_Click(object sender, EventArgs e)
        {
            try
            {
                if(Request.QueryString["Subject"]=="CK")
                {
                StringBuilder strKeyValue = new StringBuilder(Request.QueryString["CASENO"]);
                StringBuilder sbSql = new StringBuilder();
                sbSql.Append("update CM_LC_CKQ set 矿区坐标='" + InputCK.strDotString.ToString() + "' where CASENO='" + strKeyValue.ToString() + "'");
                string sMsg = InputText.SaveDotString(sbSql.ToString());
                ReturnDotString(InputCK.strDotString.ToString());
                ShowMessage(sMsg);
                //CloseWindow(true);
                }
                else if(Request.QueryString["Subject"]=="TK")
                {
                    /*
                    StringBuilder strKeyValue = new StringBuilder(Request.QueryString["CASENO"]);
                    StringBuilder sbSql = new StringBuilder();
                    sbSql.Append("update lc_kcxkz set KCQYGDZB='" + InputCK.strDotString.ToString() + "' where CASENO='" + strKeyValue.ToString() + "'");
                    string sMsg = InputText.SaveDotString(sbSql.ToString());
                    //MapgisEgov.AnalyInput.Common.Log.Write(sMsg);
                     * */
                    //string strSubject="TK";//用探矿方法格式化坐标串
                    //string strDotString = InputText.transFormattedCTK(jwDotString, strSubject);
                    InputText.saveDotStringToKCXKZ(jwDotString, "CASENO", Request.QueryString["CASENO"]);
                    ReturnDotString(jwDotString);
                    //ShowMessage(sMsg);
                }
            }
            catch (Exception oExcept)
            {
                ShowMessage("保存失败，错误原因:"+oExcept.Message);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnInputFeature_Click(object sender, EventArgs e)
        {
            try
            {
                #region 采矿权数据入库
                //流程
                if (Request.QueryString["Subject"] == "CK"&&Request.QueryString["Input"]=="yes")//采矿权入库
                {
                    string errorMsg = string.Empty;
                    string strSolutionName = "两矿";
                    string strInputAtt = "subjectType=CK&layerShortName=CKQSQDJ";
                    string strDotString = InputCK.strDotString.ToString();
                    string FieldName = "CK_GUID,申请序号,许可证号,项目档案号,项目类型,申请人,电话,地址,邮编,矿山名称,经济类型,项目审批机关,批准文号,资金来源,设计年限,开采主矿种,设计规模,规模单位,开采方式,采矿方法,选矿方法,应缴纳采矿权价款,采深上限,采深下限,矿区面积,采矿权使用费,有效期限,有效期起,有效期止,矿区编码,原许可证号,发证机关名称,区域坐标,设计利用储量,其它主矿种,签发时间";
                    //string[] attField = new string[] { "坐标系统", "CK_GUID", "申请序号", "许可证号", "项目档案号", "项目类型", "申请人", "电话", "地址", "邮编", "矿山名称", "经济类型", "项目审批机关", "批准文号", "资金来源", "设计年限", "开采主矿种", "设计规模", "规模单位", "开采方式", "采矿方法", "选矿方法", "应缴纳采矿权价款", "采深上限", "采深下限", "矿区面积", "采矿权使用费", "有效期限", "有效期起", "有效期止", "矿区编码", "原许可证号", "发证机关名称", "区域坐标", "设计利用储量", "其它主矿种", "签发时间" };
                    string[] attField = FieldName.Split(',');
                    //ShowMessage(attField.Length.ToString()+"个字段，"+strDotString);
                    DatabaseORC db = new DatabaseORC();
                    //string sql = "select CK_GUID from 采矿申请登记 where CK_GUID='" + Request.QueryString["CK_GUID"] + "'";
                    //string keyValue = db.GetDataSet(sql).Tables[0].Rows[0][0].ToString();

                    #region 根据CK_GUID查询项目档案号，更新图形参数
                    //*
                    string keyField = "CK_GUID";
                    string keyValue = Request.QueryString["GUID"];
                    //*/
                    #endregion

                    #region 根据《项目档案号》查询数据，更新图形参数
                    /*
                    string keyField = "项目档案号";
                    string keyValue=Request.QueryString["GUID"];
                    */
                    #endregion

                    string[] attValue = InputText.CKSQDJ(FieldName, keyValue, errorMsg);
                    if (strDotString == "在位置 0 处没有任何行。")
                    {
                        ShowMessage("请检查坐标是否已上传保存！");
                    }
                    else
                    {
                        string InputAttString = "strSolutionName:" + strSolutionName + "\n" + "strInputAtt:" + strInputAtt + "\n" + "strDotString:" + strDotString + "\n" + "attField:" + attField + "\n" + "attValue:" + attValue + "\n" + "keyField:" + keyField + "\n" + "keyValue" + keyValue + "\n";
                        MapgisEgov.AnalyInput.Common.Log.Write(InputAttString);
                        string strMsg = InputText.InputDotStringCK(strSolutionName, strInputAtt, strDotString, attField, attValue, keyField, keyValue);
                        if (strMsg == "在位置 0 处没有任何行。")
                        {
                            strMsg = "未查询到办理的采矿权数据，请检查。";
                        }
                        ShowMessage(strMsg);
                    }
                }

                    //页面
                else if (Request.QueryString["Subject"] == "CK" && Request.QueryString["Input"] != "yes")//采矿权入库
                {
                    string errorMsg = string.Empty;
                    string strSolutionName = "两矿";
                    string strInputAtt = "subjectType=CK&layerShortName=CKQSQDJ";
                    string strDotString = InputCK.strDotString.ToString();
                    string FieldName = "CK_GUID,申请序号,许可证号,项目档案号,项目类型,申请人,电话,地址,邮编,矿山名称,经济类型,项目审批机关,批准文号,资金来源,设计年限,开采主矿种,设计规模,规模单位,开采方式,采矿方法,选矿方法,应缴纳采矿权价款,采深上限,采深下限,矿区面积,采矿权使用费,有效期限,有效期起,有效期止,矿区编码,原许可证号,发证机关名称,区域坐标,设计利用储量,其它主矿种,签发时间,";
                    //string[] attField = new string[] { "坐标系统", "CK_GUID", "申请序号", "许可证号", "项目档案号", "项目类型", "申请人", "电话", "地址", "邮编", "矿山名称", "经济类型", "项目审批机关", "批准文号", "资金来源", "设计年限", "开采主矿种", "设计规模", "规模单位", "开采方式", "采矿方法", "选矿方法", "应缴纳采矿权价款", "采深上限", "采深下限", "矿区面积", "采矿权使用费", "有效期限", "有效期起", "有效期止", "矿区编码", "原许可证号", "发证机关名称", "区域坐标", "设计利用储量", "其它主矿种", "签发时间" };
                    string[] attField = FieldName.Split(',');
                    ShowMessage(attField.Length.ToString() + "个字段，" + strDotString);
                    string keyField = "项目档案号";
                    string[] attValue = InputText.CKSQDJ(FieldName, keyField, errorMsg);
                    DatabaseORC db = new DatabaseORC();
                    string sql = "select 项目档案号 from 采矿申请登记 where CK_GUID='" + Request.QueryString["CK_GUID"] + "'";
                    string keyValue = db.GetDataSet(sql).Tables[0].Rows[0][0].ToString();
                    //string keyValue = Request.QueryString["CK_GUID"];
                    InputText.InputDotStringCK(strSolutionName, strInputAtt, strDotString, attField, attValue, keyField, keyValue);
                }
                #endregion

                #region 探矿权数据入库
                 
                else if (Request.QueryString["Subject"] == "TK")//探矿权入库
                {
                    /*
                    Feature.Feature f = new Feature.Feature();
                    //BaseDataInfo.BaseDataInfo b = new BaseDataInfo.BaseDataInfo();
                    string strInputAttName = WebGisBase.GetInputAttr("TK410324KCXMDJ");
                    string strInputAttName = WebGisBase.GetInputAttr("CK410324CKQSQDJ");
                     * 
                    //string strInputAttName = b.GetInputAttName("两矿","申请序号");
                    ShowMessage(strInputAttName);
                     * */
                    //BaseDataInfo.BaseDataInfo b = new BaseDataInfo.BaseDataInfo();
                    //ShowMessage(b.GetInputAttName("两矿", "TK410324KCXMDJ"));
                    string errorMsg = string.Empty;
                    string strSolutionName = "两矿";
                    string strInputAtt = "subjectType=TK&layerShortName=KCXMDJ";
                    string strDotString = InputCK.strDotString.ToString();
                    string FieldName = "申请序号,许可证号,项目名称,申请人,勘查单位,经济类型,勘查矿种,签发时间,有效期起,有效期止,总面积,所在行政区";
                    string[] attField = FieldName.Split(',');
                    FieldName = "CASENO,KCZH,KCXMNC,TKQR,KCDW,SX,KCKZ,SCFZSJ,KCZYXQ1,KCZYXQ2,KCMJ,FZJG";
                    //string[] attField = new string[] { "坐标系统", "CK_GUID", "申请序号", "许可证号", "项目档案号", "项目类型", "申请人", "电话", "地址", "邮编", "矿山名称", "经济类型", "项目审批机关", "批准文号", "资金来源", "设计年限", "开采主矿种", "设计规模", "规模单位", "开采方式", "采矿方法", "选矿方法", "应缴纳采矿权价款", "采深上限", "采深下限", "矿区面积", "采矿权使用费", "有效期限", "有效期起", "有效期止", "矿区编码", "原许可证号", "发证机关名称", "区域坐标", "设计利用储量", "其它主矿种", "签发时间" };
                    string keyField = "CASENO";//关键字段名
                    string keyValue = Request.QueryString[keyField];//页面传入关键字
                    //string[] attValue = InputText.CKSQDJ(FieldName, keyField, errorMsg);//根据关键字查出要更新图形的数据
                    string[] attValue = InputText.getTKQDataFromKCXKZ(FieldName, keyField,keyValue, errorMsg).Split(','); ;
                    //string keyValue = Request.QueryString["CASENO"];
                    //InputText.InputDotStringTK(strSolutionName, strInputAtt, strDotString, attField, attValue, keyField, keyValue);
                    string sMsg = InputText.InputDotStringTK(strSolutionName, strInputAtt, strDotString, attField, attValue, keyField, keyValue);
                    MapgisEgov.AnalyInput.Common.Log.Write(sMsg);
                    for (int i = 0; i < attValue.Length;i++ )
                    {
                        sMsg = sMsg + attField[i]+attValue[i];
                    }
                    MapgisEgov.AnalyInput.Common.Log.Write(sMsg);
                }
                #endregion

            }
            catch (Exception oExcept)
            {
                MapgisEgov.AnalyInput.Common.Log.Write("=======================================================================");
                MapgisEgov.AnalyInput.Common.Log.Write("入库失败，错误原因:" + oExcept.Message);
                MapgisEgov.AnalyInput.Common.Log.Write("=======================================================================");
            }
        }

        public void CloseWindow(bool bClose)
        {
            if (bClose == true)
            {
                string strScript = "<script language='javascript'>window.close();</script>";
                this.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), strScript);
            }
        }

        public void ShowMessage(string Message)
        {
            string strScript = "<script language='javascript'>alert('" + Message + "')</script>";
            this.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), strScript);
        }

        public void ReturnDotString(string strDotString)
        {
            //string strScript = "<script language='javascript'>parent.window.returnValue ='" + strDotString + "'</script>";
            string strScript = "<script language='javascript'>window.opener.document.getElementById(\"ZB\").value='" + strDotString + "'</script>";
            this.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), strScript);
        }

        public void ReloadParentPage()
        {
            string strScript = "<script language='javascript'>window.opener.location.reload();</script>";
            this.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), strScript);
        }

        protected void GridView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void GridView1_RowEditing(object sender, GridViewEditEventArgs e)
        {
            GridView1.EditIndex = e.NewEditIndex;
            DataTable dt = new DataTable();
            dt.Columns.Add("序号", typeof(string));
            dt.Columns.Add("X坐标", typeof(string));
            dt.Columns.Add("Y坐标", typeof(string));
            /*
            string[] arrDotString = strDotString.ToString().Split(' ');
            for (int i = 0; i < arrDotString.Count(); i++)
            {
                
                string[] xy = arrDotString[i].ToString().Split(',');
                DataRow newRow = dt.NewRow();
                newRow["序号"] = i + 1;
                newRow["X坐标"] = xy[0];
                newRow["Y坐标"] = xy[1];
                dt.Rows.Add(newRow);
            }
            //MapgisEgov.AnalyInput.Common.Log.Write("RowEdit=" + dt.ToString());
            bindGridView(dt);
             * */
            int iPlotNo = 1;
            string[] arrPlotDotString = InputCK.strDotString.ToString().Split('#');
            for (int i = 0; i < arrPlotDotString.Length; i++)
            {
                string[] arrPlotNo = arrPlotDotString[i].Split('@');
                int iPoint = 1;
                for (int n = 0; n < arrPlotNo.Length; n++)
                {

                    string[] arrDotString = arrPlotNo[n].ToString().Split(' ');
                    for (int j = 0; j < arrDotString.Count(); j++)
                    {
                        string[] xy = arrDotString[j].ToString().Split(',');
                        DataRow newRow = dt.NewRow();
                        newRow["序号"] = iPlotNo.ToString() + "-" + iPoint.ToString();
                        newRow["X坐标"] = xy[0];
                        newRow["Y坐标"] = xy[1];
                        dt.Rows.Add(newRow);
                        //MapgisEgov.AnalyInput.Common.Log.Write("x="+xy[0] + "y="+xy[0]);
                    }
                    DataRow atRow = dt.NewRow();
                    atRow["序号"] = "@";
                    atRow["X坐标"] = "@";
                    atRow["Y坐标"] = "@";
                    dt.Rows.Add(atRow);
                    iPoint++;
                }
                dt.Rows.RemoveAt(dt.Rows.Count - 1);
                DataRow sharpRow = dt.NewRow();
                sharpRow["序号"] = "#";
                sharpRow["X坐标"] = "#";
                sharpRow["Y坐标"] = "#";
                dt.Rows.Add(sharpRow);
                iPlotNo++;
            }
            dt.Rows.RemoveAt(dt.Rows.Count - 1);
            bindGridView(dt);
             
        }

        protected void GridView1_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {

        }

        protected void GridView1_RowDeleting(object sender,GridViewDeleteEventArgs e)
        {

        }

        protected void GridView1_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            
            GridView1.EditIndex = e.RowIndex;
            int j = GridView1.EditIndex;
            //GridViewRow editRow = GridView1.Rows[GridView1.EditIndex];
            //MapgisEgov.AnalyInput.Common.Log.Write("Index="+GridView1.EditIndex.ToString());
            string[] arrDotString = InputCK.strDotString.ToString().Split(' ');//将坐标串拆分成点
            DataTable dt = new DataTable();
            dt.Columns.Add("序号", typeof(string));
            dt.Columns.Add("X坐标", typeof(string));
            dt.Columns.Add("Y坐标", typeof(string));
            for (int i = 0; i < arrDotString.Count(); i++)
            {
                if (i != j)//循环到索引行时，将修改后的值插入表中
                {
                    string[] xy = arrDotString[i].ToString().Split(',');
                    DataRow newRow = dt.NewRow();
                    newRow["序号"] = i + 1;
                    newRow["X坐标"] = xy[0];
                    newRow["Y坐标"] = xy[1];
                    dt.Rows.Add(newRow);
                }
                else
                {
                    DataRow newRow = dt.NewRow();
                    newRow["序号"] = i + 1;
                    newRow["X坐标"] = e.NewValues[1];
                    newRow["Y坐标"] = e.NewValues[2];
                    //MapgisEgov.AnalyInput.Common.Log.Write("X坐标=" + e.NewValues[1] + "||||Y坐标=" + e.NewValues[2]);
                    dt.Rows.Add(newRow);
                }
            }
            StringBuilder sbStringDot = new StringBuilder();
            for (int i = 0; i < dt.Rows.Count;i++ )//行循环
            {
                if (i > 0)
                {
                    sbStringDot.Append(" ");
                    sbStringDot.Append(dt.Rows[i]["X坐标"]);
                    sbStringDot.Append(",");
                    sbStringDot.Append(dt.Rows[i]["Y坐标"]);
                }
                else
                {
                    sbStringDot.Append(dt.Rows[i]["X坐标"]);
                    sbStringDot.Append(",");
                    sbStringDot.Append(dt.Rows[i]["Y坐标"]);
                }
            }
            //MapgisEgov.AnalyInput.Common.Log.Write("Updating=" + sbStringDot.ToString());
            InputCK.strDotString = sbStringDot;// new StringBuilder();
            //InputCK.strDotString.Append(sbStringDot.ToString());
            bindGridView(dt);
        }

        public static StringBuilder strDotString
        {
            get;
            set;
        }

        /// <summary>
        /// 探矿经纬度坐标
        /// </summary>
        public static string jwDotString
        {
            get;
            set;
        }

        public void bindGridView(DataTable dt)
        {
            #region 生成table并绑定GridView
            GridView1.DataSource = dt;
            GridView1.DataBind();
            #endregion
        }

        /*
        public static DataTable dt
        {
            get;
            set;
        }
        */

    }
}