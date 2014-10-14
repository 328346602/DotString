/***************************
 * 探矿、采矿图层要素删除页面
 * 页面需要参数Subject,keyValue
 * 采矿Subject=CK&keyValue=项目档案号
 * 探矿Subject=TK&keyValue=探矿证号
 ***************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebGis;

namespace InputTextDotString.View.Input.InputTextDotString
{
    public partial class DelFeature : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void DeleteFeature_Click(object sender, EventArgs e)
        {
            try
            {
                /*
                string strSolutionName = Request.QueryString["SolutionName"];
                string strInputAtt ="subjectType="+Request.QueryString["subjectType"]+"&layerShortName="+Request.QueryString["layerShortName"];
                //subjectType=DC&year=2009&scale=G&layerShortName=DLTB
                long lFeatureID = 0;
                string keyField=Request.QueryString["keyField"];
                string keyValue=Request.QueryString["keyValue"];
                string sWhere = keyField + "='" + keyValue + "'";
                //bool bDelSuccess = WebGisBase.DelFeatureNew(strSolutionName, strInputAtt, sWhere);
                Feature.Feature f=new Feature.Feature();
                bool bDelSuccess=f.DelFeatureNew(strSolutionName, strInputAtt,lFeatureID, sWhere);
                if (bDelSuccess == true)
                {
                    ShowMessage("删除要素成功！");
                }
                else
                {
                    ShowMessage("删除要素失败！");
                }
                 * */
                if (Request.QueryString["Subject"] == "CK")
                {
                    string strSolutionName = "两矿";
                    string strInputAtt = "subjectType=CK&layerShortName=CKQSQDJ";
                    //subjectType=DC&year=2009&scale=G&layerShortName=DLTB
                    long lFeatureID = 0;
                    string keyValue = Request.QueryString["keyValue"];
                    string sWhere = "项目档案号='"+keyValue+"'";
                    //bool bDelSuccess = WebGisBase.DelFeatureNew(strSolutionName, strInputAtt, sWhere);
                    Feature.Feature f = new Feature.Feature();
                    bool bDelSuccess = f.DelFeatureNew(strSolutionName, strInputAtt, lFeatureID, sWhere);
                    if (bDelSuccess == true)
                    {
                        ShowMessage("删除要素成功！");
                    }
                    else
                    {
                        ShowMessage("删除要素失败！");
                    }
                }
                else if (Request.QueryString["Subject"] == "TK")
                {
                    string strSolutionName = "两矿";
                    string strInputAtt = "subjectType=TK&layerShortName=KCXMDJ";
                    //subjectType=DC&year=2009&scale=G&layerShortName=DLTB
                    long lFeatureID = 0;
                    string keyValue = Request.QueryString["keyValue"];
                    string sWhere = "许可证号='" + keyValue + "'";
                    //bool bDelSuccess = WebGisBase.DelFeatureNew(strSolutionName, strInputAtt, sWhere);
                    Feature.Feature f = new Feature.Feature();
                    bool bDelSuccess = f.DelFeatureNew(strSolutionName, strInputAtt, lFeatureID, sWhere);
                    if (bDelSuccess == true)
                    {
                        ShowMessage("删除要素成功！");
                    }
                    else
                    {
                        ShowMessage("删除要素失败！");
                    }
                }
            }

            catch(Exception oExcept)
            {
                MapgisEgov.AnalyInput.Common.Log.Write(oExcept.Message);
            }
        }

        protected void Cancel_Click(object sender, EventArgs e)
        {
            CloseWindow(true);
        }

        /// <summary>
        /// 用javascript关闭当前窗口
        /// </summary>
        /// <param name="bClose">是否关闭当前窗口</param>
        public void CloseWindow(bool bClose)
        {
            if (bClose == true)
            {
                string strScript = "<script language='javascript'>window.close();</script>";
                this.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), strScript);
            }
        }

        /// <summary>
        /// 用javascript弹出提醒信息
        /// </summary>
        /// <param name="Message">弹出信息内容</param>
        public void ShowMessage(string Message)
        {
            string strScript = "<script language='javascript'>alert('" + Message + "')</script>";
            this.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), strScript);
        }
    }
}