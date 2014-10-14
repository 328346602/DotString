/***********************************************
 * 创建人：王金河
 * 创建日期：2013-10-30
 * 内容：这个页面对应InputPlot.aspx中的(分析结果)按钮，用来显示不同专题下，不同年度的图形 
 * 
 * **********************************************/


using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using CM.LC;
using System.Collections;
using InputTextDotString;
using WebGis.Analy;


namespace InputTextDotString.View.Input.InputTextDotString
//namespace InputTextDotString.View.Input.InputTextDotString
{
    public partial class AnalyResult : System.Web.UI.Page       // System.Web.UI.Page
    {

     

        protected void Page_Load(object sender, EventArgs e)
		{

            if (!IsPostBack)
            {
                #region 传入GUID参数时
                /*
                if (Request.QueryString["GUID"] != string.Empty)
                {
                    //sAllSubject：如果在地块列表页面就从DKZBXX中获取；导入地块页面在分析时获取。
                    string sAllSubject = Request.QueryString["sAllSubject"]; //所有的专题和年度,传过来的格式为DC:2012;DC:2011;BS:2013 需要转为DC:2012;2011,BS:2013 
                    m_plotNo = Request.QueryString["GUID"]; //地块编号
                    
                    string strDotString = InputText.AnalyDotStringCTK(Request.QueryString["Subject"], Request.QueryString["GUID"]);
                    if (string.IsNullOrEmpty(strDotString))
                    {
                        ShowMessage("未能取到坐标，请保存数据后再进行分析");
                        return;
                    }

                    if (string.IsNullOrEmpty(sAllSubject))
                    {
                        ShowMessage("url传过来的专题参数为空");
                        return;
                    }
                    if (string.IsNullOrEmpty(m_plotNo))
                    {
                        ShowMessage("url传过来的地块编号为空");
                        return;
                    }
                    BindTree(treeView, sAllSubject);

                    //ShowMap(iDb,m_plotNo,sAllSubject);//默认显示汇总分析
                    ShowMap(strDotString, sAllSubject);
                }
                 * */
                #endregion


                #region 传入采矿权业务流程CASENO时
                //if (Request.QueryString["CASENO"] != string.Empty)
                {
                    //GS.DataBase.IDbAccess iDb = GS.DataBase.DbAccessFactory.CreateInstance();

                    //sAllSubject：如果在地块列表页面就从DKZBXX中获取；导入地块页面在分析时获取。
                    string sAllSubject = Request.QueryString["sAllSubject"]; //所有的专题和年度,传过来的格式为DC:2012;DC:2011;BS:2013 需要转为DC:2012;2011,BS:2013 
                    m_plotNo = Request.QueryString["CASENO"]; //地块编号
                    /*
                    DatabaseORC db=new DatabaseORC();
                    string SQL = "select 矿区坐标 from CM_LC_CKQ where CASENO='" + Request.QueryString["CASENO"] + "'";
                    MapgisEgov.AnalyInput.Common.Log.Write(SQL);
                    string strDotString = db.GetDataSet(SQL).Tables[0].Rows[0][0].ToString();
                     * */
                    string strDotString = getDotStringFromCKQFLOW(Request.QueryString["CASENO"]);
                    if (string.IsNullOrEmpty(strDotString))
                    {
                        ShowMessage("未能取到坐标，请保存数据后再进行分析");
                        MapgisEgov.AnalyInput.Common.Log.Write("strDotString="+strDotString);
                        return;
                    }

                    if (string.IsNullOrEmpty(sAllSubject))
                    {
                        ShowMessage("url传过来的专题参数为空");
                        return;
                    }
                    if (string.IsNullOrEmpty(m_plotNo))
                    {
                        ShowMessage("url传过来的地块编号为空");
                        return;
                    }
                    BindTree(treeView, sAllSubject);

                    //ShowMap(iDb,m_plotNo,sAllSubject);//默认显示汇总分析
                    ShowMap(strDotString, sAllSubject);
                }
                #endregion
            }
        }


        #region 绑定树
        /// <summary>
        /// 绑定树
        /// </summary>     
        /// <param name="tree"></param>
        /// <param name="sAllSubject">格式为DC:2012;DC:2013;BS:2013</param>
        public void BindTree(TreeView tree, string sAllSubject)
        {
            TreeNode root = new TreeNode("专题树", "0");
            tree.Nodes.Add(root); //添加根节点

            if (string.IsNullOrEmpty(sAllSubject))
            {
                return;
            }
            /*
            //第一个节点是汇总结果
            TreeNode node = new TreeNode();//TreeNode("汇总分析结果", sAllSubject); //专题节点
            root.ChildNodes.Add(node);

             * */

            //添加其他专题年度节点
            Hashtable table = GetSubjectYear(sAllSubject);
            string sSubject = "";
            string sSubjectName = "";
            foreach (DictionaryEntry dic in table)
            {
                sSubject = dic.Key.ToString();
                sSubjectName = WebGis.WebGisBase.GetSubjectName(sSubject);
                TreeNode node = new TreeNode(sSubjectName, sSubject + ":"); //专题节点

                AddYearNode(node, sSubject, table[sSubject].ToString());
                root.ChildNodes.Add(node);
            }
        }
        #endregion

        #region 绑定树的小函数

        #region 转化 专题年度 的数据结构       
       
        /// <summary>
        /// 将格式为DC:2012;DC:2013;BS:2013的字符串存为ht,key为subject，value为year(多个用分号)
        /// </summary>
        /// <param name="sSubjectYear"></param>
        /// <returns></returns>
        public Hashtable GetSubjectYear(string sSubjectYear)
        {
            string[] arrSubjectYear = sSubjectYear.Split(';');
            string sSubject = "";
            string sYear = "";

            Hashtable table = new Hashtable();
            for (int i = 0; i < arrSubjectYear.Length; i++)
            {
                sSubject = arrSubjectYear[i].Split(':')[0];
                sYear = arrSubjectYear[i].Split(':')[1];
                if (table.Contains(sSubject))
                {
                    sYear = table[sSubject].ToString() + ";" + sYear; //年度之间为分号
                    table[sSubject] = sYear; //重设
                }
                else
                {
                    table.Add(sSubject, sYear);
                }
            }
            return table;           
        }
        #endregion

        #region 将专题下的年度添加到专题节点下

        /// <summary>
        /// 将专题下的年度添加到专题节点下
        /// </summary>
        /// <param name="parent">专题节点</param>
        /// <param name="sSubject">专题缩写</param>
        /// <param name="sYears">年度，中间使用分号间隔如  2012;2013 </param>
        public void AddYearNode(TreeNode parent, string sSubject, string sYears)
        {
            string[] arrYear = sYears.Split(';');
            for (int i = 0, size = arrYear.Length; i < size; i++)
            {
                if (!String.IsNullOrEmpty(arrYear[i]))
                {
                    TreeNode node = new TreeNode(arrYear[i], sSubject + ":" + arrYear[i]);
                    parent.ChildNodes.Add(node);
                }
            }
        }
        #endregion

        #endregion


        /// <summary>
        /// 用脚本显示信息
        /// </summary>
        /// <param name="Message"></param>
        public void ShowMessage(string Message)
        {
            string strScript = "<script language='javascript'>alert('" + Message + "')</script>";
            this.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), strScript);
        }

        /// <summary>
        /// 关闭当前窗口
        /// </summary>
        /// <param name="bClose"></param>
        public void CloseWindow(bool bClose)
        {
            if (bClose == true)
            {
                string strScript = "<script language='javascript'>window.close();</script>";
                this.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), strScript);
            }
        }


        protected void treeView_SelectedNodeChanged(object sender, EventArgs e)
        {
            //GS.DataBase.IDbAccess iDb = GS.DataBase.DbAccessFactory.CreateInstance();
            
            string sValue = treeView.SelectedValue; //获取选择的值
            if (sValue.Contains(":"))
            {
                /*
                DatabaseORC db=new DatabaseORC();
                //string SQL = "select 区域坐标 from 采矿申请登记 where ck_guid='" + Request.QueryString["GUID"] + "'";
                string SQL = "select 区域坐标 from 采矿申请登记 where ck_guid='" + Request.QueryString["GUID"] + "'";
                DataSet ds = db.GetDataSet(SQL);
                string strDotString=ds.Tables[0].Rows[0][0].ToString();//根据传入的GUID唯一值查询到对应的坐标
                //ShowMap(iDb,m_plotNo, sValue);
                 * */
                string strDotString = getDotStringFromCKQFLOW(Request.QueryString["CASENO"]);
                string strSubject = sValue.Split(':')[0].ToString();
                //ShowMap(strDotString, sValue);
                ShowMap(strDotString, strSubject);
            }
           
        }

        #region 根据分析专题和年度显示地图        
     
        /// <summary>
        /// 根据分析专题和年度显示地图
        /// </summary>
        /// <param name="sValue"></param>
        public void ShowMap(GS.DataBase.IDbAccess iDb, string sPlotNo, string sSubjectYear)
        {
            string sPoint = "";

            sPoint = MapgisEgov.AnalyInput.Models.DKZBXX.GetPointForHuan(iDb, sPlotNo);

            MapgisEgov.AnalyInput.Common.Log.Write("坐标串第一次:" + sPoint);//查看sPoint参数

            string dispTargetList = ""; //勾选的底图，湘潭县要加 遥感影像的

            //dispTargetList = "RS2013G";

            dispTargetList = "INDEX2013G";
            string userName=Request.QueryString["user"];
            MapgisEgov.AnalyInput.Common.Log.Write("登录用户1:" + userName);
            //dispTargetList = "MPKA2008-2015J";
            //string strUrl = WebGis.WebGisBase.NewSetAnalyParam(sPlotNo, sSubjectYear, sPoint, dispTargetList);
            string strUrl = WebGis.WebGisBase.NewSetAnalyParam(sPlotNo, sSubjectYear, sPoint, dispTargetList,userName);
            MapgisEgov.AnalyInput.Common.Log.Write("查看图形url:" + strUrl);

            MapgisEgov.AnalyInput.Common.Log.Write("坐标串第二次:" + sPoint);//查看sPoint参数

            iframeContent.Attributes.Add("src", strUrl);

        }

        public void ShowMap(string strDotString, string sSubjectYear, string dispTargetList)
        {

            MapgisEgov.AnalyInput.Common.Log.Write("坐标串第一次:" + strDotString);//查看sPoint参数

            //string dispTargetList = ""; //勾选的底图，湘潭县要加 遥感影像的

            //dispTargetList = "RS2013G";

            //dispTargetList = "INDEX2013G";

            //dispTargetList = "MPKA2008-2015J";
            //string strUrl = WebGis.WebGisBase.NewSetAnalyParam(sPlotNo, sSubjectYear, sPoint, dispTargetList);
            string strTime = DateTime.Now.ToString();
            string strFolder = Guid.NewGuid().ToString();
            string userName = Request.QueryString["user"];
            string strUrl = WebGis.WebGisBase.NewSetAnalyParam(strTime + strFolder, sSubjectYear, strDotString, dispTargetList, userName);
            MapgisEgov.AnalyInput.Common.Log.Write("登录用户2:" + userName);
            MapgisEgov.AnalyInput.Common.Log.Write("查看图形url:" + strUrl);

            MapgisEgov.AnalyInput.Common.Log.Write("坐标串第二次:" + strDotString);//查看sPoint参数

            iframeContent.Attributes.Add("src", strUrl);

        }

        public void ShowMap(string strDotString, string sSubjectYear)
        {

            MapgisEgov.AnalyInput.Common.Log.Write("坐标串第一次:" + strDotString);//查看sPoint参数

            string dispTargetList = ""; //勾选的底图，湘潭县要加 遥感影像的

            //dispTargetList = "RS2013G";

            dispTargetList = "INDEX2013G";
            //string strUrl = WebGis.WebGisBase.NewSetAnalyParam(sPlotNo, sSubjectYear, sPoint, dispTargetList);
            string strTime = DateTime.Now.ToString().Replace("/", "-");
            string strFolder = Guid.NewGuid().ToString();
            string userName = getUserID(Request.QueryString["user"]);//AnalyResult.getUserID(Request.QueryString["user"]);
            MapgisEgov.AnalyInput.Common.Log.Write("登录用户3:" + userName);
            string strUrls = string.Empty;
            Analy.Analy a = new Analy.Analy();
            //调试用地址
            //a.Url = "http://192.168.21.100/IGSLandService/Analy.asmx";
            //Web.config中配置的地址，正式部署时使用
            a.Url = InputText.UrlAnaly;
            
            if (sSubjectYear.Split(',')[0].Split(':')[0] == "KC")
            {
                dispTargetList = "KCCK2013G,KCKB2013G,KCTP2013G";
                strUrls = a.NewSetAnalyParam(strFolder, strFolder, "410324", "", strDotString, "辖区", "subjectType=KC", "", "专题:KC,年度:2013,比例尺:G", "KC", userName, (Analy.EnumAnalyType)1, true, true, dispTargetList);
            }
            else if (sSubjectYear.Split(',')[0].Split(':')[0] == "CK")
            {
                dispTargetList = "CK";
                strUrls = a.NewSetAnalyParam(strFolder, strFolder, "410324", "", strDotString, "两矿", "subjectType=CK", "", "专题:CK", "CK", userName, (Analy.EnumAnalyType)1, true, true, dispTargetList);
            }
            else if (sSubjectYear.Split(',')[0].Split(':')[0] == "TK")
            {
                dispTargetList = "TK";
                strUrls = a.NewSetAnalyParam(strFolder, strFolder, "410324", "", strDotString, "两矿", "subjectType=TK", "", "专题:TK", "TK", userName, (Analy.EnumAnalyType)1, true, true, dispTargetList);
                MapgisEgov.AnalyInput.Common.Log.Write(strUrls);
            }
            else if (sSubjectYear.Split(',')[0].Split(':')[0] == "MP")
            {
                dispTargetList = "MPBH2008-2015J,MPCQ2008-2015J,MPKA2008-2015J,MPKC2008-2015J,MPKQ2008-2015J";
                strUrls = a.NewSetAnalyParam(strFolder, strFolder, "410324", "", strDotString, "规划", "subjectType=MP", "", "专题:MP,规划期:2008-2015,比例尺:J", "MP", userName, (Analy.EnumAnalyType)1, true, true, dispTargetList);
                MapgisEgov.AnalyInput.Common.Log.Write(strUrls);
            }
            else if (sSubjectYear.Split(',')[0].Split(':')[0] == "FA")
            {
                dispTargetList = "FA";
                strUrls = a.NewSetAnalyParam(strFolder, strFolder, "410324", "", strDotString, "两矿", "subjectType=FA", "", "专题:FA", "FA", userName, (Analy.EnumAnalyType)1, true, true, dispTargetList);
            }
            else
            {
                ShowMessage("传入专题参数有误，请检查！");
                CloseWindow(true);
            }
            iframeContent.Attributes.Add("src", "http://192.168.21.100/webgis/webgis.html#guid=" + strUrls + "&type=analy&user=" + userName);
            //string strUrl = WebGis.WebGisBase.NewSetAnalyParam(strTime + strFolder, sSubjectYear, strDotString, dispTargetList);

            

            //Analy.Analy a = new Analy.Analy();
            //a.Url = "http://192.168.21.100/IGSLandService/Analy.asmx";
            //string strUrl = a.NewSetAnalyParam(strTime, strFolder,"410324", "", strDotString, "两矿", "subjectType=CK", "", "专题:CK", "CK", userName, (Analy.EnumAnalyType)1, true, true, dispTargetList);

            /*
            string strUrl = WebGis.WebGisBase.NewSetAnalyParam(strFolder, sSubjectYear, strDotString, dispTargetList, userName);

            MapgisEgov.AnalyInput.Common.Log.Write("查看图形url:" + strUrl);
            MapgisEgov.AnalyInput.Common.Log.Write("坐标串第二次:" + strDotString);//查看sPoint参数

            iframeContent.Attributes.Add("src", strUrl + "&user=" + userName);
            */

        }

        #endregion

        private string getDotStringFromCKQFLOW(string CASENO)
        {
            try
            {
                string SQL="select 矿区坐标 from CM_LC_CKQ where CASENO='"+CASENO+"'";
                DatabaseORC db = new DatabaseORC();
                string dbValue = db.GetDataSet(SQL).Tables[0].Rows[0][0].ToString();
                /*
                if(dbValue.Substring(0,1)=="J")
                {
                    //m_dotString = InputText.AnalyDotStringCTK(dbValue);
                }
                return null;
                 * */
                return dbValue;
            }

            catch (Exception oExcept)
            {
                return oExcept.Message;
            }
        }

        #region 全局变量

        /// <summary>
        /// 专题
        /// </summary>
        public string m_subject
        {
            get
            {
                if (ViewState["m_subject"] == null)
                {
                    return "";
                }
                else
                {
                    return ViewState["m_subject"].ToString();
                }
            }
            set
            {
                ViewState["m_subject"] = value;
            }
        }

        /// <summary>
        /// 年度
        /// </summary>
        public string m_year
        {
            get
            {
                if (ViewState["m_year"] == null)
                {
                    return "";
                }
                else
                {
                    return ViewState["m_year"].ToString();
                }
            }
            set
            {
                ViewState["m_year"] = value;
            }
        }

        /// <summary>
        /// 地块编号
        /// </summary>
        public string m_plotNo
        {
            get
            {
                if (ViewState["m_plotNo"] == null)
                {
                    return "";
                }
                else
                {
                    return ViewState["m_plotNo"].ToString();
                }
            }
            set
            {
                ViewState["m_plotNo"] = value;
            }
        }

     

        /// <summary>
        /// 案卷编号
        /// </summary>
        public string m_caseNo
        {
            get
            {
                if (ViewState["m_caseNo"] == null)
                {
                    return "";
                }
                else
                {
                    return ViewState["m_caseNo"].ToString();
                }
            }
            set
            {
                ViewState["m_caseNo"] = value;
            }
        }

        ///
        public string m_dotString
        {
            get;
            set;
        }
        #endregion

        /// <summary>
        /// 获取用户登陆政务的ID
        /// </summary>
        /// <param name="userNo">传入用户对应的编号</param>
        /// <returns>用户登陆政务的ID</returns>
        public static string getUserID(string userNo)
        {
            try
            {
                DatabaseORC db = new DatabaseORC();
                string SQL="select t.工号 from flow_users t where t.用户ID='"+userNo+"'";
                string userID = db.GetDataSet(SQL).Tables[0].Rows[0][0].ToString();
                return userID;
            }
            catch (Exception oExcept)
            {
                return oExcept.Message;
            }
        }

    }
}