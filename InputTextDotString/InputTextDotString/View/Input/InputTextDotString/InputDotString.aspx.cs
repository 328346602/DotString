using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Text;
using CM.LC;
using MapgisEgov.AnalyInput;

namespace InputTextDotString.View.Input.InputTextDotString
{
    public partial class InputDotString : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //GridViewBind();
            }
        }

        protected void btnAnaly_Click(object sender, EventArgs e)
        {
            //MapgisEgov.AnalyInput.Models.DKZBXX zb=new MapgisEgov.AnalyInput.Models.DKZBXX();
            /*
            HttpPostedFile filedata = FileUpload1.PostedFile;
            string strCaseNo = Request.QueryString["CASENO"];
            string sMsg = string.Empty;
            string sAllPlotNo = strCaseNo;
            bool bImportDotStringTY = InputText.ImportDotStringTY(m_iDb, filedata, strCaseNo, ref sMsg, out sAllPlotNo);
            MapgisEgov.AnalyInput.Common.Log.Write("断点1");
             * */

        }

        public void GridViewBind()
        {
            DataSet ds = new DataSet("DotString");
            DatabaseORC dORC = new DatabaseORC();
            StringBuilder sbSql = new StringBuilder();
            sbSql.Append("select JZDH,DKQH,ZZBX,HZBY from dkzb_zb where CASENO='" + Request.QueryString["CASENO"] + "'");
            ds = dORC.GetDataSet(sbSql.ToString());
            GridView1.DataSource = ds.Tables;
            MapgisEgov.AnalyInput.Common.Log.Write("断点2");
        }

        private GS.DataBase.IDbAccess m_iDb
        {
            get
            {
                object obj = Session["m_iDb"];
                if (null == obj)
                {
                    return null;
                }
                else
                {
                    return (GS.DataBase.IDbAccess)obj;
                }
            }
            set
            {
                Session["m_iDb"] = value;
            }
        }
    }
}