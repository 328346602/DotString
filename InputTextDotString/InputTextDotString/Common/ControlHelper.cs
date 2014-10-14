/********************************************************
 * 创建日期：2013-11-4 15:49
 * 创建人:王金河 
 * 内容:
 * 和控件有关的一些方法，方便操作控件使用
 * 
 * 
 * 
 *******************************************************/

using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI.WebControls;

namespace MapgisEgov.AnalyInput.Common
{
    /// <summary>
    /// 控件助手类
    /// </summary>
    public class ControlHelper
    {
        #region 清空gridView中所有选中的项
        /// <summary>
        /// 清空gridView中所有选中的项
        /// </summary>
        /// <param name="grid">gridView</param>
        /// <param name="checkID">控件checkbox的id</param>
        public static void SetUnselected(GridView grid, string checkID)
        {
            for (int i = 0, maxI = grid.Rows.Count; i < maxI; i++)
            {
                CheckBox cb = (CheckBox)grid.Rows[i].FindControl(checkID);
                cb.Checked = false;//设置为没有选中
            }
        }
        #endregion
    }
}