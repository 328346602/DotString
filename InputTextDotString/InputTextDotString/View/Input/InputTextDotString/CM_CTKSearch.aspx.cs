using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Data;
using CM.LC;

namespace InputTextDotString.View.Input.InputTextDotString
{
    public partial class CM_CTKSearch : System.Web.UI.Page
    {
        
        protected void Page_Load(object sender, EventArgs e)
        {

            ////判断页面是否是第一次加载
            //if (!Page.IsPostBack)
            //{
                //查询SQL。
                string strSQL = GetSql();
                DataTable dt = new DataTable();
                DatabaseORC db = new DatabaseORC();
                dt = db.GetDataSet(strSQL).Tables[0];
                //if (dt.Rows.Count > 0)
                //{
                    //GridView显示
                    ////清除列  
                    //this.GridView.Columns.Clear();  
                    this.GridView.DataSource = dt.DefaultView;
                    this.GridView.DataBind();
                //}
            //}
        }

        #region "查询事件"
        /// <summary>
        /// 查询事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btSearch_Click(object sender, EventArgs e)
        {
            //查询SQL。
            //string strSQL = GetSql();
            string strSQL = "SELECT 许可证号,矿山名称,CK_GUID  FROM 采矿申请登记  where 许可证号 like '%" + this.txtAJMC.Text + "%'";
            DataTable dt = new DataTable();
            DatabaseORC db = new DatabaseORC();
            dt = db.GetDataSet(strSQL).Tables[0];
            //if (dt.Rows.Count > 0)
            //{
                ////GridView显示
                ////清除列  
                //this.GridView.Columns.Clear();
                ////禁自动生成列  
                //this.GridView.AutoGenerateColumns = false;
                this.GridView.DataSource = dt.DefaultView;
                this.GridView.DataBind();
            //}
        }
        # endregion

        #region "获得执行的SQL"
        /// <summary>
        /// 获得执行的SQL
        /// </summary>
        /// <returns></returns>
        public string GetSql()
        {
            StringBuilder strb = new StringBuilder();
            //strb.AppendLine("SELECT 许可证号, 矿山名称 as \"案卷编号\" ");
            strb.AppendLine("SELECT 许可证号,矿山名称,CK_GUID ");
            strb.AppendLine(" FROM 采矿申请登记");
            strb.AppendLine(" where 项目类型 = '1010' or 项目类型 = '1020' or 项目类型 = '1030' or 项目类型 = '1040' or 项目类型 = '1050' or 项目类型 = '1060' or 项目类型 = '1070'--0代表未处理，1代表已经处理");
            return strb.ToString();
        }
        # endregion

        protected void GridView_SelectedIndexChanged(object sender, EventArgs e)
        {
            StringBuilder strScript = new StringBuilder();
            strScript.Append("<script language=\"javascript\" type=\"text/javascript\">");
            int Index = GridView.SelectedIndex + 1;
            strScript.Append("var rowIndex =" + Index + ";");
            strScript.Append("var gdview = document.getElementById(\"GridView\");");
            strScript.Append("var value1 = gdview.rows(rowIndex).cells(0).innerText;");
            strScript.Append("var value2 = gdview.rows(rowIndex).cells(1).innerText;");
            strScript.Append("var value3 = '"+GridView.SelectedValue.ToString()+"';");
            strScript.Append("parent.window.returnValue = value1 + \":\" + value2 + \":\" + value3;");
            //strScript.Append("alert(value1+value2+value3);");
            strScript.Append("window.close();");
            strScript.Append("</script>");
            this.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), strScript.ToString());
        }

        public void ShowMessage(string Message)
        {
            string strScript = "<script language='javascript'>alert('" + Message + "')</script>";
            this.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), strScript);
        }


        //分页 
        protected void GridViewHistory_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView.PageIndex = e.NewPageIndex;
            dataBinding();
        }
        protected void lb_firstpage_Click(object sender, EventArgs e)
        {
            this.GridView.PageIndex = 0;
            dataBinding();
        }
        protected void lb_previouspage_Click(object sender, EventArgs e)
        {
            if (this.GridView.PageIndex > 0)
            {
                this.GridView.PageIndex--;
                dataBinding();
            }
        }
        protected void lb_nextpage_Click(object sender, EventArgs e)
        {
            if (this.GridView.PageIndex < this.GridView.PageCount)
            {
                this.GridView.PageIndex++;
                dataBinding();
            }
        }
        protected void lb_lastpage_Click(object sender, EventArgs e)
        {
            this.GridView.PageIndex = this.GridView.PageCount;
            dataBinding();
        }

        private void dataBinding()
        {
            //查询SQL。
            string strSQL = GetSql();
            DataTable dt = new DataTable();
            DatabaseORC db = new DatabaseORC();
            dt = db.GetDataSet(strSQL).Tables[0];
            //if (dt.Rows.Count > 0)
            //{
            ////GridView显示
            ////清除列  
            //this.GridView.Columns.Clear();
            ////禁自动生成列  
            //this.GridView.AutoGenerateColumns = false;
            this.GridView.DataSource = dt.DefaultView;
            this.GridView.DataBind();
            //}
        }
    }
}