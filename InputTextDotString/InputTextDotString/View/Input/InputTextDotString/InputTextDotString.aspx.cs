using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.OracleClient;
using CM.LC;
using WebGis;
using MapgisEgov.AnalyInput.Models;
using System.IO;
using System.Collections;
using System.Text;

namespace InputTextDotString.InputTextDotString
{
    public partial class InputTextDotString : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
         #region 原方法
            /*
            if (!IsPostBack)
            {
                string strCaseNo = Request.QueryString["CASENO"];
            }
             * */
         #endregion

        }

        protected void InputDotString_Click(object sender, EventArgs e)
        {
            FileUpload fu = FileUpload;
            InputText it = new InputText();
            string errorMsg = "";
            if (FileUpload.PostedFile.FileName.ToString() == string.Empty)
            {
                //txtUrl.Text = "请选择坐标文件路径！";
                errorMsg = "请选择要导入的坐标文件！";
                ShowMessage(errorMsg);
            }
            #region  输入FileUpload不为空时，读取坐标
            else if (FileUpload.PostedFile.FileName.ToString() != string.Empty)
            {
                #region 原简单方法
                StreamReader sr = new StreamReader(fu.PostedFile.FileName, System.Text.Encoding.GetEncoding(936), true);
                InputText.BiaoZhunDotString BZZB = it.InputDotText(fu);
                string srRead = sr.ReadLine();
                if (srRead.Substring(0, 1) == "[")
                {
                    InputText.resolveDeptFile(sr, BZZB, errorMsg);
                }
                else
                {
                    InputText.ResolveTxtFile(sr, BZZB, errorMsg);
                }
                BZZB.strCaseNo = Request.QueryString["CASENO"];
                string strSql = "update CM_LC_CKQ set 矿区坐标='" + BZZB.strDots + "' where CASENO='" + BZZB.strCaseNo + "'";
                txtUrl.Text = strSql;
                string strSelectCKQ = "select t.* from CM_LC_CKQ t where CASENO='" + BZZB.strCaseNo + "'";


                txtUrl.Text = BZZB.strDots;
                #endregion
                #region 查询数据库
                const string FieldName = "坐标系统,CK_GUID,申请序号,许可证号,项目档案号,项目类型,申请人,电话,地址,邮编,矿山名称,经济类型,项目审批机关,批准文号,资金来源,设计年限,开采主矿种,设计规模,规模单位,开采方式,采矿方法,选矿方法,应缴纳采矿权价款,采深上限,采深下限,矿区面积,采矿权使用费,有效期限,有效期起,有效期止,矿区编码,原许可证号,发证机关名称,区域坐标,设计利用储量,其它主矿种,签发时间,";
                string[] arrFieldName = new string[] { "坐标系统", "CK_GUID", "申请序号", "许可证号", "项目档案号", "项目类型", "申请人", "电话", "地址", "邮编", "矿山名称", "经济类型", "项目审批机关", "批准文号", "资金来源", "设计年限", "开采主矿种", "设计规模", "规模单位", "开采方式", "采矿方法", "选矿方法", "应缴纳采矿权价款", "采深上限", "采深下限", "矿区面积", "采矿权使用费", "有效期限", "有效期起", "有效期止", "矿区编码", "原许可证号", "发证机关名称", "区域坐标", "设计利用储量", "其它主矿种", "签发时间" };
                string keyField = Request.QueryString["CK_GUID"];
                string[] arrResult = InputText.CKSQDJ(FieldName, keyField, errorMsg);
                StringBuilder sbString = new StringBuilder();
                for (int i = 0; i < arrResult.Length; )
                {
                    sbString.Append(arrResult[i].ToString()+"|");
                    i++;
                }
                WebUse.Logs.WriteLog(BZZB.sPath, "成功执行数据入库！\n" + sbString.ToString()+"共有"+arrResult.Length+"个字段数据取到");
                bool bSuccess = WebGisBase.AddFeatureNew("两矿", "subjectType=CK&layerShortName=CKQSQDJ", BZZB.strDots, arrResult, arrFieldName);
                if (bSuccess == true)
                {
                    WebUse.Logs.WriteLog(BZZB.sPath, "成功执行数据入库！\n" + sbString.ToString());
                    txtUrl.Text="成功执行数据入库!";
                    ShowMessage(txtUrl.Text);
                }
                else
                {
                    WebUse.Logs.WriteLog(BZZB.sPath, bSuccess+"数据入库失败！\n" + sbString.ToString());
                    txtUrl.Text="入库失败，请检查相关数据！";
                    ShowMessage(txtUrl.Text);
                }
                #endregion
            }
            /*
            if (Request.QueryString["Subject"] == "CK")
            {
                #region 查询数据库
                string FieldName = "CK_GUID,申请序号,许可证号,开采方式";
                string keyField = Request.QueryString["CK_GUID"];
                //MapgisEgov.AnalyInput.Common.Log.Write(keyField);
                WebUse.Logs.WriteLog(BZZB.sPath, "keyField=" + keyField);
                string strResult = InputText.CKSQDJ(FieldName, keyField, errorMsg);
                txtUrl.Text = BZZB.strDots + strResult;
                txtUrl.Text = strResult;
                ShowMessage(strResult);
            }
             * */
                /*
            else
            {
                errorMsg = "读取成功！";
                ShowMessage(errorMsg);
                #endregion
            }
                 * */
            #endregion
        }
           
           public void ShowMessage(string Message)
          {
              string strScript = "<script language='javascript'>alert('" + Message + "')</script>";
              this.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), strScript);
          }
    }
}