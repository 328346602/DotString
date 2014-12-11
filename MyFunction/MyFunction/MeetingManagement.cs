using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web.SessionState;
using System.Collections;
using System.Data;


namespace MeetingManagement
{
	/// <summary>
	/// Function 的摘要说明。
	/// </summary>
	public class MeetingManagement:Visual_Form_Designer.Class.IFunction
	{
		private string Rev="";
		private string ErrorMsg="";
        public MeetingManagement() { }

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
                   Hashtable arr=_CustomObject as Hashtable;
                   WebControl Sender;         //触发此调用的页面控件，如果是页面调用的话Sender为空；
                   StateBag ViewState;        //本页面的ViewState;
                    if(arr!=null)
			{
			Sender=arr["Sender"] as WebControl;
			ViewState=arr["ViewState"] as StateBag;  
			}   
                
                          
   
			
			return true;
		}

		public string LastError
		{
			get
			{
				return ErrorMsg;
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
                bool bSigned=(sSignedUser.StartsWith(sCurrentUserName))||(sSignedUser.Contains(sCurrentUserName) && sSignedUser.Contains(","+sCurrentUserName));//判断用户是否存在数据库中

                if (bSigned)
                {
                    Function.ShowMessage s = new Function.ShowMessage();
                    s.Alert("该用户已签收", _Page);
                }
                else
                {
                    Function.ShowMessage s = new Function.ShowMessage();
                    s.Alert("该用户未签收", _Page);
                }
                return true;
            }
            catch (Exception oExcept)
            {
                return false;
            }
        }
		
        
        #endregion

        public static string GetSignedUser(string CaseNo)
        {
            try
            {
                string sUser = "";
                string sSql = "select SIGNEDUSER from CM_LC_MEETINGMANAGEMENT where GUID='" + CaseNo + "'";
                Function.DatabaseORC db = new Function.DatabaseORC();
                sUser = db.GetDataSet(sSql).Tables[0].Rows[0][0].ToString();
                return sUser;
            }
            catch(Exception oExcept)
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
                Function.DatabaseORC db = new Function.DatabaseORC();
                sUser = db.GetDataSet(sSql).Tables[0].Rows[0][0].ToString();
                return sUser;
            }
            catch (Exception oExcept)
            {
                return oExcept.Message;
            }
        }
	}
}
