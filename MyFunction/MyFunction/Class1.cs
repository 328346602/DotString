using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web.SessionState;
using System.Collections;
using System.Data;

namespace Function
{
    /// <summary>
    /// Function 的摘要说明。
    /// </summary>
    public class Function : Visual_Form_Designer.Class.IFunction
    {
        private string Rev = "";
        private string ErrorMsg = "";
        public Function() { }

        #region IFunction 成员

        public object ReturnValue
        {
            get
            {
                return Rev;
            }
        }
        
        public bool Exec(System.Web.UI.Page _Page, System.Web.HttpContext _Context, Visual_Form_Designer.Class.VFDServiceObject _Service, Visual_Form_Designer.Class.WebPageConfig _WebPageConfig, System.Collections.Hashtable ParamaterList, object _CustomObject)
        {
            try
            {
                Hashtable arr = _CustomObject as Hashtable;
                WebControl Sender;         //触发此调用的页面控件，如果是页面调用的话Sender为空；
                StateBag ViewState;        //本页面的ViewState;
                if (arr != null)
                {
                    Sender = arr["Sender"] as WebControl;
                    ViewState = arr["ViewState"] as StateBag;
                }

                return true;
            }
            catch(Exception oExcept)
            {
                return false;
            }
        }
        
        public bool 测试(System.Web.UI.Page _Page, System.Web.HttpContext _Context, Visual_Form_Designer.Class.VFDServiceObject _Service, Visual_Form_Designer.Class.WebPageConfig _WebPageConfig, System.Collections.Hashtable ParamaterList, object _CustomObject)
        {
            try
            {
                Hashtable arr = _CustomObject as Hashtable;
                WebControl Sender;         //触发此调用的页面控件，如果是页面调用的话Sender为空；
                StateBag ViewState;        //本页面的ViewState;
                if (arr != null)
                {
                    Sender = arr["Sender"] as WebControl;
                    ViewState = arr["ViewState"] as StateBag;
                }
                string sUserID = _Context.User.Identity.Name;//取用户ID
                string sUserName = "";//初始化用户名
                string strSql = string.Format("select 名称 from FLOW_USERS where 用户ID='{0}'", sUserID);//生成SQL
                DatabaseORC db = new DatabaseORC();//初始化数据连接
                DataSet ds = db.GetDataSet(strSql);//执行SQL获取查询结果
                if (ds.Tables[0].Rows.Count > 0)//若数据不为空，读取用户名
                {
                    sUserName = ds.Tables[0].Rows[0][0].ToString();
                }
                else
                {
                    sUserName = "未取到用户名!";//若查询结果为空，则提示错误
                }
                ShowMessage s = new ShowMessage();//初始化Alert对象
                TextBox tbuser = (TextBox)_Page.FindControl("txtUser");//初始化页面中ID为"txtUser"的TextBox控件
                tbuser.Text = sUserName;//将用户名字段赋予页面中ID为"txtUser"的控件
                TextBox tbAlert = (TextBox)_Page.FindControl("txtAlert");//初始化页面中ID为"txtAlert"的TextBox控件
                if (tbAlert.Text != "")//当"txtAlert"控件中无任何内容时，不弹出提示，若有内容，则提示内容
                {
                    //string sUrl = System.Web.HttpContext.Current.Server.MapPath("\\XinXiFaBu").ToString();
                    string sUrl = System.Web.HttpContext.Current.Server.MapPath("~/");
                    string x = System.Configuration.ConfigurationManager.AppSettings["IGSLandService"];
                    Tool.ShowModalDialog(sUrl + "\\XXFB.VFD", _Page);
                    //Tool.ShowModalDialog(sUrl + "..\\..\\SubSysFlowFile\\XinXiFaBu\\XXFB_YGD.VFD", _Page);
                    Tool.WriteLog(sUrl + "SubSysFlowFile\\XinXiFaBu\\XXFB_YGD.VFD");
                    try
                    {
                        if (System.Int32.Parse(tbAlert.Text.ToString()) > 0)
                        {
                            Tool.WriteLog(System.Int32.Parse(tbAlert.Text).ToString());
                            Tool.Recurrence(System.Int32.Parse(tbAlert.Text));
                            Tool.Alert(tbAlert.Text,_Page);
                        }
                        else return false;
                    }
                    catch(Exception oExcept)
                    {
                        Tool.WriteLog(oExcept.Message);
                    }
                }
                else
                {
                    //Tool.ShowModalDialog("开发科.VFD", _Page);
                    //Tool.ShowModalDialog("..\\..\\SubSysFlowFile\\开发科.VFD", _Page);
                    Tool.Alert("未传递数字",_Page);
                }
                Tool.WriteLog(_Page.ToString() + "\n" + _Context.ToString() + "\n" + _Service.ToString() + "\n" + //换行
                    _WebPageConfig.ToString() + "\n" + ParamaterList.ToString() + "\n" + _CustomObject.ToString());
                return true;
            }
            catch (Exception oExcept)
            {
                Tool.WriteLog(oExcept.Message);
                return false;
            }

        }


        public bool MeetingSign(System.Web.UI.Page _Page, System.Web.HttpContext _Context, Visual_Form_Designer.Class.VFDServiceObject _Service, Visual_Form_Designer.Class.WebPageConfig _WebPageConfig, System.Collections.Hashtable ParamaterList, object _CustomObject)
        {
            try
            {
                TextBox tbCurrentUserName = (TextBox)_Page.FindControl("txtCurrentUserName");
                string sCurrentUserName = tbCurrentUserName.Text;
                TextBox tbCaseNo = (TextBox)_Page.FindControl("txtCaseNo");
                string sCaseNo = tbCaseNo.Text;
                string sSignedUser = GetSignedUser(sCaseNo);
                string sUnsignedUser = GetUnsignedUser(sCaseNo);
                bool bSigned = (sSignedUser.StartsWith(sCurrentUserName)) || (sSignedUser.Contains(sCurrentUserName) && sSignedUser.Contains("," + sCurrentUserName));//判断用户是否存在数据库中

                if (bSigned)
                {
                    ShowMessage s = new ShowMessage();
                    //s.Alert("该用户已签收", _Page);
                    ShowMessage.Alert("该用户已签收", "1", "签收状态", _Page);
                }
                else
                {
                    ShowMessage s = new ShowMessage();
                    //s.Alert("该用户未签收", _Page);
                    ShowMessage.Alert("该用户未签收", "1", "签收状态", _Page);
                }
                return true;
            }
            catch (Exception oExcept)
            {
                ShowMessage s = new ShowMessage();
                ShowMessage.Alert("插件执行错误", "1", "错误状态", _Page);
                Tool.WriteLog(oExcept.Message);
                return false;
            }
        }


        public bool SendMsg(System.Web.UI.Page _Page, System.Web.HttpContext _Context, Visual_Form_Designer.Class.VFDServiceObject _Service, Visual_Form_Designer.Class.WebPageConfig _WebPageConfig, System.Collections.Hashtable ParamaterList, object _CustomObject)
        {
            try
            {
                TextBox tbCurrentUserName = (TextBox)_Page.FindControl("txtCurrentUserName");
                string sCurrentUserName = tbCurrentUserName.Text;
                TextBox tbCaseNo = (TextBox)_Page.FindControl("txtCaseNo");
                string sCaseNo = tbCaseNo.Text;
                TextBox tbUserList = (TextBox)_Page.FindControl("txtUserID");
                string sUserList = tbUserList.Text;
                TextBox tbMsg = (TextBox)_Page.FindControl("txtContent");
                string sMsg = tbMsg.Text;
                string sMsgType = "会议通知";
                string sUrl="SubSysFlowFile/BanGongShi/MeetingManagement/MeetingInfo.vfd?CASENO=";
                //string sSignedUser = GetSignedUser(sCaseNo);
                //string sUnsignedUser = GetUnsignedUser(sCaseNo);
                //bool bSigned = (sSignedUser.StartsWith(sCurrentUserName)) || (sSignedUser.Contains(sCurrentUserName) && sSignedUser.Contains("," + sCurrentUserName));//判断用户是否存在数据库中
                if (sCurrentUserName == null || sCaseNo == null || sUserList == null || sMsg == null)
                {
                    Tool.Alert("请完整填写信息",_Page);
                }
                else
                {
                    Tool.WriteLog("开始发送消息");
                    Tool.Alert(Tool.SendMessage(sUserList, sMsg,sUrl ,sMsgType, _Page),_Page);
                }
                return true;
            }
            catch (Exception oExcept)
            {
                ShowMessage s = new ShowMessage();
                //Tool.Alert(oExcept.Message,_Page);
                Tool.WriteLog("+++++++++报错+++++++++\n" + oExcept.Message);
                ErrorMsg = oExcept.Message;
                return false;
            }
        }
		

        public string LastError
        {
            get
            {
                return ErrorMsg;
            }
        }
        #endregion
        public static string GetSignedUser(string CaseNo)
        {
            try
            {
                string sUser = "";
                string sSql = "select SIGNEDUSER from CM_LC_MEETINGMANAGEMENT where GUID='" + CaseNo + "'";
                DatabaseORC db = new DatabaseORC();
                sUser = db.GetDataSet(sSql).Tables[0].Rows[0][0].ToString();
                return sUser;
            }
            catch (Exception oExcept)
            {
                return oExcept.Message;
            }
        }

        public static string GetUnsignedUser(string CaseNo)
        {
            try
            {
                string sUser = "";
                string sSql = "select UNSIGNEDUSER from CM_LC_MEETINGMANAGEMENT where GUID='" + CaseNo + "'";
                DatabaseORC db = new DatabaseORC();
                sUser = db.GetDataSet(sSql).Tables[0].Rows[0][0].ToString();
                return sUser;
            }
            catch (Exception oExcept)
            {
                return oExcept.Message;
            }
        }
    }
           
    public class ShowMessage
    {
        public void Alert(string Message, System.Web.UI.Page _Page)
        {
            string strScript = "<script language='javascript'>alert('" + Message + "')</script>";
            _Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), strScript);
        }

         public static void Alert(string Message,string id,string title, System.Web.UI.Page _Page)
        {
            string strScript = "<script language='javascript'>$(function(){var obj = { id: \"" + id + "\", title: \"" + title + "\", text: \"" + Message + "\"};showMessage(obj);})</script>";
            _Page.ClientScript.RegisterStartupScript(_Page.GetType(), Guid.NewGuid().ToString(), strScript);
            Tool.WriteLog(strScript);
            //var obj = { id: "message", title: "提示", text: "请选择接收人！"};
            //showMessage(obj);
        }
    }

    public class Tool
    {
        public static void WriteLog(string sMsg)
        {
            try
            {
                string sUrl = System.Web.HttpContext.Current.Server.MapPath("~/TempFile");
                string sPath = sUrl + "/Test" + System.DateTime.Today.ToString("yyyy-MM-dd")+".txt";
                WebUse.Logs.WriteLog(sPath, sMsg);
            }
            catch (Exception oExcept)
            {
                WebUse.Logs.WriteLog(System.Web.HttpContext.Current.Server.MapPath("~/TempFile")+".txt", "写日志方法出错--------" + oExcept.Message);
            }
        }

        public static void ShowModalDialog(string sUrl,System.Web.UI.Page _Page)
        {
            try
            {
                string strScript = "<script language='javascript'>window.showModalDialog('" + sUrl + "')</script>";
                _Page.ClientScript.RegisterStartupScript(_Page.GetType(), Guid.NewGuid().ToString(), strScript);
            }
            catch(Exception oExcept)
            {
                //Tool.WriteLog(oExcept.Message);
            }
        }

        public static void Recurrence(int iNum)
        {
            try
            {
                //WriteLog(iNum.ToString());
                int iNumA = iNum;
                iNum = iNum-1;
                if (iNum > 0)
                {
                    Tool.Recurrence(iNum);
                }
                else return;
            }
            catch (Exception oExcept)
            {
                WriteLog("=================递归算法出错=================\n" + oExcept.Message);
            }
        }

        public static void Alert(string Message, System.Web.UI.Page _Page)
        {
            try
            {
                string strScript = "<script language='javascript'>alert('" + Message + "')</script>";
                _Page.ClientScript.RegisterStartupScript(_Page.GetType(), Guid.NewGuid().ToString(), strScript);
            }
            catch (Exception oExcept)
            {
                Tool.WriteLog(oExcept.Message);
            }
        }

        /// <summary>
        /// 发送系统消息方法
        /// </summary>
        /// <param name="sUserList">用户机构列表，例如"P-1,P-1-1,U-78,U-79,U-80"</param>
        /// <param name="sMsg">要发送的消息内容，例如"栾川县地质矿产局产学研基地竣工仪式通知"</param>
        /// <param name="sUrl">要发送的消息中包含的超链接地址，例如"SubSysFlowFile\BanGongShi\MeetingManagement\MeetingInfo.vfd?CASENO="</param>
        /// <param name="sCaseNo">传递进来的关键字，可与sUrl写在一起</param>
        /// <param name="sMsgType">发送消息备注类型</param>
        /// <param name="_Page"></param>
        /// <returns></returns>
        public static string SendMessage(string sUserList, string sMsg, string sUrl, string sMsgType, System.Web.UI.Page _Page)
        {
            try
            {
                ArrayList arrSendedUsers = new ArrayList();//保存已经发送过的用户ID，避免重复发送
                ArrayList arrUserInGroup = new ArrayList();//保存根据机构代码查到的用户ID，分别发送
                string sSql = "select 用户ID from flow_user_role where 机构ID='";//根据机构查找用户代码的SQL
                DataSet ds = new DataSet();
                string sDateTime = DateTime.Now.ToString();
                DatabaseORC db = new DatabaseORC();
                string sReturn = string.Empty;
                #region sUserList包含逗号时
                if (sUserList.Contains(","))
                {
                    Tool.WriteLog("有逗号");
                    string[] strUserList = sUserList.Split(',');
                    for (int i = 0; i < strUserList.Length; i++)
                    {
                        if (strUserList[i].Split('-')[0] == "P")
                        {
                            ds = db.GetDataSet(sSql + strUserList[i] + "'");
                            //arrUserInGroup = db.GetDataSet(sSql+strUserList[i]+"'").Tables[0].Columns[0];
                            for (int j = 0; j < ds.Tables[0].Columns.Count; j++)//循环向用户发送消息，并用户ID存入arrUserInGroup
                            {
                                if (!arrSendedUsers.Contains(ds.Tables[0].Rows[j][0]))//判断用户是否已收到消息，向未收到用户发送
                                {
                                    sReturn = Tool.SendMessage(db, ds.Tables[0].Rows[j][0].ToString(), sMsg, sDateTime, sMsgType, sUrl, _Page);
                                    arrSendedUsers.Add("@" + ds.Tables[0].Rows[j][0] + "@");
                                }
                                else
                                {
                                    continue;
                                }
                            }
                        }
                        else if (strUserList[i].Split('-')[0] == "U")
                        {
                            sReturn = Tool.SendMessage(db, strUserList[i], sMsg, sDateTime, sMsgType, sUrl, _Page);
                            arrSendedUsers.Add("@" + strUserList[i] + "@");
                        }
                    }
                }
                #endregion

                #region sUserList中无逗号
                else
                {
                    Tool.WriteLog("无逗号");
                        if (sUserList.Split('-')[0] == "P")
                        {
                            ds = db.GetDataSet(sSql + sUserList + "'");
                            //arrUserInGroup = db.GetDataSet(sSql+strUserList[i]+"'").Tables[0].Columns[0];
                            for (int j = 0; j < ds.Tables[0].Columns.Count; j++)//循环向用户发送消息，并用户ID存入arrUserInGroup
                            {
                                if (!arrSendedUsers.Contains(ds.Tables[0].Rows[j][0]))//判断用户是否已收到消息，向未收到用户发送
                                {
                                    sReturn = Tool.SendMessage(db, ds.Tables[0].Rows[j][0].ToString(), sMsg, sDateTime, sMsgType, sUrl, _Page);
                                    arrSendedUsers.Add("@" + ds.Tables[0].Rows[j][0] + "@");
                                }
                                else
                                {
                                    continue;
                                }
                            }
                        }
                        else if (sUserList.Split('-')[0] == "U")
                        {
                            sReturn = Tool.SendMessage(db, sUserList, sMsg, sDateTime, sMsgType, sUrl, _Page);
                            arrSendedUsers.Add("@" + sUserList + "@");
                        }
                    }
                #endregion
                return sReturn;
            }
            catch (Exception oExcept)
            {
                //ShowMessage.Alert(oExcept.Message, "1", "发送消息失败", _Page);
                Alert(oExcept.Message,_Page);
                WriteLog("===================================报错===================================\n"+oExcept.Message);
                return "SendMessage错误：" + oExcept.Message;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sUserID">用户ID</param>
        /// <param name="sMsg">发送消息的具体内容</param>
        /// <param name="sDateTime">发送消息的时间</param>
        /// <param name="sMsgType">发送消息的类型</param>
        /// <param name="sUrl">消息提示超链接</param>
        /// <param name="_Page"></param>
        /// <returns></returns>
        public static string SendMessage(DatabaseORC db, string sUserID, string sMsg, string sDateTime, string sMsgType, string sUrl, System.Web.UI.Page _Page)
        {
            try
            {
                string sMsgInfo = "&nbsp;&nbsp;&nbsp;&nbsp;<a class=''grid-pager'' href=''#'' onclick=''top.redirectUrl(\"" + sUrl + "\"); return false;''>" + sMsg + "</a>";
                //DatabaseORC db = new DatabaseORC();
                string sSql = "insert into messagesys(RECEIVEID,MESSAGE,COMETIME,MSGTYPE,MESSAGEINFO) values('" + sUserID + "','" + sMsg + "',to_date('" + sDateTime + "','yyyy-mm-dd hh24:mi:ss'),'" + sMsgType + "','" + sMsgInfo + "')";
                WriteLog(sSql);
                db.ExecuteSql(sSql);
                //ShowMessage.Alert("发送消息成功", "1", "提示", _Page);
                //Tool.Alert("发送消息成功",_Page);
                return "发送消息成功！";
            }
            catch (Exception oExcept)
            {
                //ShowMessage.Alert(oExcept.Message, "1", "发送消息失败", _Page);
                //Alert(oExcept.Message, _Page);
                WriteLog("===================================报错===================================\n" + oExcept.Message);
                return "发送失败："+oExcept.Message;
            }
        }
    }
}
