using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Specialized;
using System.Data;
using System.Drawing.Imaging;
using System.IO;

namespace InputTextDotString.View.Input.InputTextDotString
{
    public partial class DrawMap1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //form1.i<img src="DrawMap.ashx?caseNo=CKQBG-2014-0000009&caseType=CK" />
            //this.imgMap.Src = "DrawMap.ashx?caseNo=CKQBG-2014-0000009&caseType=CK";
            
            string caseNo=Request.QueryString["caseNo"];
            string caseType=Request.QueryString["caseType"];
            if (caseNo != null && caseType != null)
            {
                ShowMap(caseNo, caseType);
            }
            else
            {
                //ShowMessage("传入参数不正确，请检查页面设置。");
                this.imgMap.Width = 600;
                this.imgMap.Height = 200;
            }
        }
        /// <summary>
        /// Draws the map.
        /// </summary>
        /// <param name="msg">The MSG.</param>
        /// <returns></returns>
        public int DrawMapFun(GS.DataBase.IDbAccess iDb, ref string msg, ref Bitmap bm, string tblname, string key1, string key2, string keyval1, string XX, string YY, string LAND_GROUP, int MAX_X, int MAX_Y)
        {
            ArrayList landArea = new ArrayList();
            int nErrPoint = 0, RV = 1;
            int nMaxX = 0, nMaxY = 0;
            double nArea = 1.0;

            ArrayList landList = GetData(iDb, ref nMaxX, ref nMaxY, ref nErrPoint, ref nArea, tblname, key1, key2, keyval1, XX, YY, LAND_GROUP, MAX_X, MAX_Y);
            
            MapgisEgov.AnalyInput.Common.Log.Write("---landList----");
            if (landList == null)
            {
                MapgisEgov.AnalyInput.Common.Log.Write("landList is null");
                throw new Exception("ArrayList landList 为 null! 位置: DrawMap().");
            }
            else if (landList.Count < 1)
            {
                MapgisEgov.AnalyInput.Common.Log.Write("无合法数据");
                msg = "无合法数据!";
                return 0;
            }
            else if (nMaxX > MAX_X || nMaxY > MAX_Y)
            {
                MapgisEgov.AnalyInput.Common.Log.Write("画布不可大于{0}*{1}. \n 位置:DrawMap().");
                throw new Exception(string.Format("画布不可大于{0}*{1}. \n 位置:DrawMap().", MAX_X, MAX_Y));
            }
            if (nErrPoint > 0)
            {
                MapgisEgov.AnalyInput.Common.Log.Write(string.Format("含有非法数据 {0} 条,已忽略.", nErrPoint));
                msg = string.Format("含有非法数据 {0} 条,已忽略.", nErrPoint);
                RV = -1;
            }




            //由此Bitmap实例创建Graphic实例
            Graphics g;
            g = Graphics.FromImage(bm);
            //用Snow色彩为背景色填充此绘画图面
            g.Clear(Color.Snow);


            g.TranslateTransform(0.0F, MAX_Y); //平移(垂直向下：MAX_Y)
            //g.RotateTransform(90.0F);


            //绘图; 计算面积
            double sumArea = 0.0;

            Pen p1 = new Pen(Color.Blue, 2);
            MapgisEgov.AnalyInput.Common.Log.Write("landList.Count:" + landList.Count);
            for (int i = 0; i < landList.Count; i++)
            {
                ArrayList subGroupList = (ArrayList)landList[i];
                for (int ii = 0; ii < subGroupList.Count; ii++)
                {

                    ArrayList dotList = new ArrayList();
                    dotList.Clear();

                    ArrayList tmpArr = (ArrayList)subGroupList[ii];
                    int n = tmpArr.Count;
                    Point[] ptList = new Point[n + 1]; // +1
                    for (int j = 0; j < n; j++)
                    {
                        ptList[j] = (Point)tmpArr[j];
                        dotList.Add(ptList[j]);
                    }
                    ptList[n] = (Point)tmpArr[0]; // first point
                    dotList.Add(ptList[n]);

                    //判断数组是否少于2个Point (非法,因为无法DrawLines(Point[]),且无法CalculateOneArea())
                    if (ptList.Length < 2)
                        continue;
                    g.DrawLines(p1, ptList);

                }

            }
            MapgisEgov.AnalyInput.Common.Log.Write("---_CalculateOneArea---");
            //----landArea = _CalculateOneArea(iDb, tblname, key1, key2, keyval1, XX, YY, LAND_GROUP);

            //sumArea = Math.Abs(_GetAreaData(landArea));

            //----sumArea = Math.Abs(_GetAreaDataHuBei(landArea));
            MapgisEgov.AnalyInput.Common.Log.Write("sumArea:" + sumArea.ToString());
            //对面积四舍五入
            sumArea = Math.Round(sumArea, 4);
            MapgisEgov.AnalyInput.Common.Log.Write("sumArea:" + sumArea.ToString());
            //边框
            Pen p = new Pen(Color.Black, 2);
            g.DrawRectangle(p, 10, 10, MAX_X, MAX_Y);
            //消除锯齿
            g.SmoothingMode = SmoothingMode.AntiAlias;

            MapgisEgov.AnalyInput.Common.Log.Write("11111111111111");


            Font textFont = new Font("Lucida Sans Unicode", 18);
            //黄科 2008-5-23 修改，使面积能出来
            RectangleF rectangle = new RectangleF(-10, -MAX_Y, 500, 50);
            g.FillRectangle(new SolidBrush(Color.Gainsboro), rectangle);
            StringFormat format = new StringFormat();
            format.Alignment = StringAlignment.Center;

            //隐藏面积
            string flowedText = "面积:" + (Convert.ToDouble(sumArea.ToString()) / 10000).ToString("f4") + "(公顷)";
            try
            {
                g.DrawString(flowedText, textFont, new SolidBrush(Color.Blue), rectangle, format);
            }
            catch (System.Exception e)
            {
                string err = e.Message;
                MapgisEgov.AnalyInput.Common.Log.Write("err:" + err);
            }

            MapgisEgov.AnalyInput.Common.Log.Write("简图生成结束！");
            return RV;
        }


        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <param name="nMaxX">The n max X.</param>
        /// <param name="nMaxY">The n max Y.</param>
        /// <param name="nErrPoint">The n err point.</param>
        /// <param name="nArea">The n area.</param>
        /// <returns></returns>
        private ArrayList GetData(GS.DataBase.IDbAccess iDb, ref int nMaxX, ref int nMaxY, ref int nErrPoint, ref double nArea, string tblname, string key1, string key2, string keyval1, string XX, string YY, string LAND_GROUP, int MAX_X, int MAX_Y)
        {
            //同一地块中数据再次分组(环状地块):对应的数据库字段名称 --此名称对任何表都固定不变
            string LAND_SUBGROUP_FLD_DB = "DKQH";
            const string LAND_SUBGROUP_FLD = "LAND_SUBGROUP_FLD";

            if (tblname == null || key1 == null || key2 == null || keyval1 == null || XX == null || YY == null)
                return null;
            if (tblname.ToUpper() == "BJ_14_02_BCGDJZDB" || tblname.ToUpper() == "BJ_40_02_BCGDJZDB")
                LAND_SUBGROUP_FLD_DB = LAND_GROUP;
            nErrPoint = 0;
            nArea = 1;//面积缩放因子
            int cnt = 0;//合法记录数目

            string sql = "";
            if (iDb.DataBaseType.ToString().ToUpper() == "SQLSERVER")
                sql = String.Format("select  {0} as XX,{1} as YY,{2} as LAND_GROUP, {3} as LAND_SUBGROUP_FLD from {4} where {5}='{6}' order by convert(int,{7})", XX, YY, LAND_GROUP, LAND_SUBGROUP_FLD_DB, tblname, key1, keyval1, key2);
            else if (iDb.DataBaseType.ToString().ToUpper() == "MSACCESS")
                sql = String.Format("select  {0} as XX,{1} as YY,{2} as LAND_GROUP, {3} as LAND_SUBGROUP_FLD from {4} where {5}='{6}' order by val({7})", XX, YY, LAND_GROUP, LAND_SUBGROUP_FLD_DB, tblname, key1, keyval1, key2);
            else if (iDb.DataBaseType.ToString().ToUpper() == "ORACLE")
            {
                //tblname = tblname.Substring(1, tblname.Length - 2); //////将＂［］＂去掉．
                sql = String.Format("select  {0} as XX,{1} as YY,{2} as LAND_GROUP, {3} as LAND_SUBGROUP_FLD from {4} where {5}='{6}' ORDER BY TO_NUMBER({7})", XX, YY, LAND_GROUP, LAND_SUBGROUP_FLD_DB, tblname, key1, keyval1, key2);
            }
            MapgisEgov.AnalyInput.Common.Log.Write("sql:" + sql);
            DataTable dt = iDb.GetDataSet(sql).Tables[0];

            if (dt == null)
                return null;
            else if (dt.Rows.Count < 1)
            {
                ArrayList nullList = new ArrayList();
                nullList.Clear();
                return nullList;
            }

            // 生成坐标点集合(按地块分组): landList, ptList //
            double x = 0, y = 0, minX = 0, maxX = 0, minY = 0, maxY = 0;
            int scaleX = 0, scaleY = 0;
            //int nScale=1,tmp=0;

            ArrayList landList = new ArrayList();
            landList.Clear();

            // 计算偏移量
            DataRow r = null;
            MapgisEgov.AnalyInput.Common.Log.Write("dt.Rows.Count:" + dt.Rows.Count.ToString());
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                r = dt.Rows[i];
                try
                {
                    x = Convert.ToDouble(r["XX"].ToString());
                    y = Convert.ToDouble(r["YY"].ToString());
                }
                catch (Exception)
                { continue; }

                if (minX == 0) // first one 
                {
                    minX = x;
                    minY = y;
                }
                minX = Math.Min(x, minX);
                maxX = Math.Max(x, maxX);
                minY = Math.Min(y, minY);
                maxY = Math.Max(y, maxY);
            }
            double offsetX = maxX - minX, offsetY = maxY - minY;

            double nX = (double)offsetX / MAX_X;
            double nY = (double)offsetY / MAX_Y;
            double nDelta = nX > nY ? nX : nY;
            //面积缩放
            nArea = nDelta == 0.0 ? 1 : nDelta * nDelta;

            double deltaX = (MAX_X - ((maxX - minX)) / nDelta) / 2;
            double deltaY = (MAX_Y - ((maxY - minY)) / nDelta) / 2;


            // 计算偏移及缩放后的值 ,填充 landList,ptList
            NameValueCollection nvc = new NameValueCollection();
            ArrayList nvcList_SubGroup = new ArrayList();
            string strLandGroup = "";//地块名
            string strSubGroup = ""; //子组

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                r = dt.Rows[i];
                try
                {
                    x = Convert.ToDouble(r["XX"].ToString());

                    y = Convert.ToDouble(r["YY"].ToString());

                    x -= minX;
                    y -= minY;


                    x /= nDelta;
                    y /= nDelta;

                    // 原点偏移(使图形居中)
                    x += deltaX;
                    y += deltaY;

                    strLandGroup = r["LAND_GROUP"].ToString();
                    strSubGroup = r[LAND_SUBGROUP_FLD].ToString();
                }
                catch (Exception)
                { continue; }
                System.Drawing.Point pt; pt = new Point((int)x, (int)(-y));

                //按地块存储
                if (strLandGroup.Trim().Length < 1)
                    continue;

                if (nvc.Count < 1)
                {
                    ArrayList subGroupList = new ArrayList(), ptList = new ArrayList();

                    ptList.Add(pt);
                    subGroupList.Add(ptList);
                    landList.Add(subGroupList);


                    NameValueCollection nvcSubGroup = new NameValueCollection();
                    nvcSubGroup.Add(strSubGroup, "0");
                    nvcList_SubGroup.Add(nvcSubGroup);
                    nvc.Add(strLandGroup, "0");

                }
                else
                {
                    string[] tmp = nvc.GetValues(strLandGroup);
                    if (tmp != null)
                    {
                        if (tmp.Length < 1)
                            continue;
                        int nIndex = Convert.ToInt32(tmp[0]);
                        NameValueCollection tmpNvc = ((NameValueCollection)nvcList_SubGroup[nIndex]);
                        string[] tmp2 = tmpNvc.GetValues(strSubGroup);
                        if (tmp2 != null)
                        {
                            if (tmp2.Length < 1)
                                continue;
                            int nIndex2 = Convert.ToInt32(tmp2[0]);
                            ((ArrayList)((ArrayList)landList[nIndex])[nIndex2]).Add(pt);
                        }
                        else
                        {
                            tmpNvc.Add(strSubGroup, tmpNvc.Count.ToString());

                            ArrayList ptList3 = new ArrayList();
                            ptList3.Add(pt);
                            ArrayList subGroupList3 = ((ArrayList)landList[nIndex]);
                            subGroupList3.Add(ptList3);
                            //landList.Add(subGroupList3);
                        }
                    }
                    else
                    {
                        ArrayList subGroupList = new ArrayList(), ptList = new ArrayList();

                        ptList.Add(pt);
                        subGroupList.Add(ptList);
                        landList.Add(subGroupList);


                        NameValueCollection nvcSubGroup = new NameValueCollection();
                        nvcSubGroup.Add(strSubGroup, "0");
                        nvcList_SubGroup.Add(nvcSubGroup);
                        nvc.Add(strLandGroup, nvc.Count.ToString());
                    }
                }
                cnt++;
            }
            nErrPoint = dt.Rows.Count - cnt;
            return landList;
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

        public void ShowMap(string caseNo,string caseType)
        {
            string strScript = "<script language='javascript'>document.getElementById(\"imgMap\").src=\"DrawMap.ashx?caseNo=" + caseNo + "&caseType=" + caseType + "\"</script>";
            this.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), strScript);
            MapgisEgov.AnalyInput.Common.Log.Write("===============================开始调用DrawMap.ashx生成简图=============================");
            MapgisEgov.AnalyInput.Common.Log.Write("ShowMap参数为：" + strScript);
        }
    }
}