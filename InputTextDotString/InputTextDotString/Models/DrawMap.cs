﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Drawing;
using System.Drawing.Imaging;
using System.Data;

namespace InputTextDotString.Models
{
    public class DrawMap
    {
        /// <summary>
        /// 绘制图形，并且标注每个点的坐标
        /// </summary>
        /// <param name="strDotString"></param>
        /// <param name="context"></param>
        public void Draw(string strDotString,HttpContext context)
        {
            try
            {
                //string coordinate = GetCoordinate(caseNo, Tools.GetDBByCaseType(caseType.ToUpper()),caseType); //Tools.GetCKFormattedDotString("3,38,1,3346685.00,38583830.85,2,3346685.00,38584170.85,3,3346230.00,38585120.86,4,3345960.01,38585885.87,5,3346200.01,38585975.87,6,3345980.01,38586540.88,7,3345120.00,38587240.89,8,3345120.01,38588240.90,9,3343825.00,38588670.91,10,3342986.99,38588580.91,11,3342904.98,38587540.90,12,3343404.99,38587125.89,13,3343404.99,38586910.89,14,3344404.99,38586175.88,15,3344404.99,38585575.88,16,3343904.98,38585566.88,17,3343904.98,38585375.88,18,3343291.98,38585067.88,19,3343291.98,38585067.88,20,3343744.98,38584845.87,21,3344194.98,38584996.87,22,3344471.99,38584787.87,23,3344404.98,38584515.86,24,3344169.98,38584555.87,25,3344207.98,38583055.85,26,3344589.98,38582805.85,27,3345396.99,38584040.86,28,3345034.99,38584265.86,29,3345217.99,38584535.86,30,3344959.99,38584810.87,31,3344904.99,38584770.87,32,3344337.99,38585390.87,33,3344784.99,38585420.87,34,3344904.99,38584920.87,35,3345279.99,38584505.86,36,3345204.99,38584393.86,37,3345827.00,38583980.85,38,3346405.00,38583811.85,276,-280,,1,5,39,3342434.97,38586455.89,40,3342174.97,38586930.90,41,3341084.96,38586875.90,42,3341114.96,38586385.90,43,3341859.97,38586170.89,100,50,,1,5,44,3348930.02,38583730.84,45,3347755.02,38585120.85,46,3347105.01,38585270.86,47,3347535.01,38584560.85,48,3348645.02,38583465.83,101,50,,1,"); //GetCoordinate(caseNo, Tools.GetDBByCaseType(caseType.ToUpper()));
                string[] coordinates = strDotString.Split('@');
                //封闭每一个坐标 //处理数据
                MapgisEgov.AnalyInput.Common.Log.Write("取坐标结束，根据以下坐标串生成简图：" + strDotString);
                List<ArrayList> coordinateList = new List<ArrayList>();
                for (int i = 0; i < coordinates.Length; i++)
                {
                    if (!string.IsNullOrEmpty(coordinates[i]))//将所有地块存入List：coordinateList中
                    {
                        if (coordinates[i].Split(' ')[0] != coordinates[i].Split(' ')[coordinates[i].Split(' ').Length - 1])
                        {
                            coordinates[i] += " " + coordinates[i].Split(' ')[0];//封闭坐标串


                        }
                        ArrayList arrList = new ArrayList();
                        arrList = ArrayList.Adapter(coordinates[i].Split(' '));
                        coordinateList.Add(arrList);

                    }
                }
                double minX = 0, minY = 0, maxX = 0, maxY = 0;
                MapgisEgov.AnalyInput.Common.Log.Write("minX=" + minX + ";minY=" + minY + ";maxX=" + maxX + ";maxY=" + maxY);
                List<ArrayList> dotList = new List<ArrayList>(coordinateList);
                GetMinMaxCoordinate(coordinateList, ref minX, ref maxX, ref minY, ref maxY);
                MapgisEgov.AnalyInput.Common.Log.Write("minX=" + minX + ";minY=" + minY + ";maxX=" + maxX + ";maxY=" + maxY);
                for (int i = 0; i < coordinateList.Count; i++)
                {
                    coordinateList[i] = GetPlot(coordinateList[i], minX, maxX, minY, maxY);
                }

                Bitmap bm = new Bitmap(1000, 750);
                Graphics g = Graphics.FromImage(bm);
                g.Clear(Color.White);
                g.TranslateTransform(00.0f, 600);//绘制图形的坐标原点



                Pen p1 = new Pen(Color.Blue, 2);


                List<Point[]> pointList = new List<Point[]>();

                for (int iP = 0; iP < coordinateList.Count; iP++)
                {
                    Point[] points = new Point[coordinateList[iP].Count];
                    for (int iC = 0; iC < coordinateList[iP].Count; iC++)
                    {
                        points[iC] = (Point)coordinateList[iP][iC];
                    }

                    g.DrawLines(p1, points);
                    
                    #region 在点旁标注坐标点
                    try
                    {
                        for (int i = 0; i < points.Length; i++)
                        {
                            bool x1 = dotList[iP][i].ToString().Split(',')[0] == minY.ToString();
                            bool x2 = dotList[iP][i].ToString().Split(',')[0] == maxY.ToString();
                            bool y1 = dotList[iP][i].ToString().Split(',')[1] == minX.ToString();
                            bool y2 = dotList[iP][i].ToString().Split(',')[1] == maxX.ToString();
                            //bool bDraw = (dotList[iP][i].ToString().Split(',')[0] == minY.ToString() || dotList[iP][i].ToString().Split(',')[0] == maxY.ToString()) && (dotList[iP][i].ToString().Split(',')[1] == minX.ToString() || dotList[iP][i].ToString().Split(',')[1] == maxX.ToString());
                            bool bDraw=(x1||x2)||(y1||y2);
                            MapgisEgov.AnalyInput.Common.Log.Write(x1.ToString()+x2.ToString()+y1.ToString()+y2.ToString());
                            //MapgisEgov.AnalyInput.Common.Log.Write(dotList[iP][i].ToString().Split(',')[0] + "==" + minY.ToString() + "||" + dotList[iP][i].ToString().Split(',')[0] + "==" + maxY.ToString() + "&&" + dotList[iP][i].ToString().Split(',')[1] + "==" + minX.ToString() + "||" + dotList[iP][i].ToString().Split(',')[1] + " ==" + maxX.ToString());
                            if (bDraw)
                            {
                                MapgisEgov.AnalyInput.Common.Log.Write("=====================================================================");
                                //string str = points[i].ToString() + "," + points[1].ToString();
                                string str = dotList[iP][i].ToString().Replace(",", ",\n");
                                Font f = new Font("Lucida Sans Unicode", 8);
                                SolidBrush s = new SolidBrush(Color.Black);
                                //PointF drawPoint = new PointF(float.Parse(points[0].ToString()), float.Parse(points[1].ToString()));
                                PointF drawPoint = new PointF();
                                drawPoint.X = float.Parse(points[i].X.ToString());//400f;// float.Parse(points[0].ToString());
                                drawPoint.Y = float.Parse(points[i].Y.ToString());//400f;// float.Parse(points[0].ToString());
                                //RectangleF rectangle = new RectangleF(drawPoint.X, drawPoint.Y, 0, 50);
                                StringFormat format = new StringFormat();
                                //format.Alignment = StringAlignment.Center;
                                g.DrawString(str, f, s, drawPoint);
                                //g.DrawString(str, f, s, rectangle, format);//居中造成边缘坐标显示不完整，不采用
                            }
                        }
                    }
                    catch (Exception oExcept)
                    {
                        MapgisEgov.AnalyInput.Common.Log.Write("标记点坐标时出现错误：" + oExcept.Message);
                    }

                    #endregion
                }

                context.Response.ContentType = "image/jpeg";
                g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                bm.Save(context.Response.OutputStream, ImageFormat.Jpeg);
                MapgisEgov.AnalyInput.Common.Log.Write("==========================DrawMap.ashx执行完毕，成功生成简图！==========================");
            }
            catch (Exception oExcept)
            {
                MapgisEgov.AnalyInput.Common.Log.Write(oExcept.Message);
                MapgisEgov.AnalyInput.Common.Log.Write("==================================DrawMap.Draw方法报错==================================");
            }
        }


        public void Draw(ref string strMessage, HttpContext context)
        {
            try
            {
                List<ArrayList> coordinateList = new List<ArrayList>();
                Bitmap bm = new Bitmap(600, 200);
                Graphics g = Graphics.FromImage(bm);
                g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                g.Clear(Color.WhiteSmoke);//定义画布颜色
                g.TranslateTransform(0f, 0f);//绘制图形的坐标原点
                Pen p1 = new Pen(Color.Blue, 4);//定义画笔
                string str = strMessage;
                Font f = new Font("宋体", 30);
                SolidBrush s = new SolidBrush(Color.Black);
                PointF drawPoint = new PointF();
                drawPoint.X = 100f;// float.Parse(points[0].ToString());
                drawPoint.Y = 100f;// float.Parse(points[0].ToString());
                //RectangleF rectangle = new RectangleF(100f, 100f, 0f, 18);
                //StringFormat format = new StringFormat();
                //format.Alignment = StringAlignment.Center;

                g.DrawString(str, f, s, drawPoint);
                //g.DrawString(str, f, s, rectangle, format);//居中造成边缘坐标显示不完整，不采用
                //context.Response.ContentType = "image/jpeg";
                //bm.Save(context.Response.OutputStream, ImageFormat.Jpeg);

                context.Response.ContentType = "image/png";
                bm.Save(context.Response.OutputStream, ImageFormat.Png);
                MapgisEgov.AnalyInput.Common.Log.Write("==========================DrawMap.ashx执行完毕，成功输出文字！==========================");
            }
            catch (Exception oExcept)
            {
                MapgisEgov.AnalyInput.Common.Log.Write(oExcept.Message);
                MapgisEgov.AnalyInput.Common.Log.Write("==================================DrawMap.Draw方法报错==================================");
            }
        }

        /// <summary>
        /// 偏移每个点的坐标，使每个点都可以绘制在图片上
        /// </summary>
        /// <param name="arrayList"></param>
        /// <param name="minX"></param>
        /// <param name="maxX"></param>
        /// <param name="minY"></param>
        /// <param name="maxY"></param>
        /// <returns></returns>
        private ArrayList GetPlot(ArrayList arrayList, double minX, double maxX, double minY, double maxY)
        {
            string sb = "";
            int MAX_X = 600, MAX_Y = 600;//生成图形后的画布外围宽度
            ArrayList DotList = new ArrayList();//存放偏移后的坐标（Point型）
            if (arrayList == null || arrayList.Count < 2)
                return DotList;
            double x = 0, y = 0;

            double offsetX = maxX - minX, offsetY = maxY - minY;
            double nX = (double)offsetX / MAX_X;
            double nY = (double)offsetY / MAX_Y;
            double nDelta = nX > nY ? nX : nY;


            double deltaX = (MAX_X - ((maxX - minX)) / nDelta) / 2;
            double deltaY = (MAX_Y - ((maxY - minY)) / nDelta) / 2;

            // 计算偏移及缩放后的值
            foreach (string strDot in arrayList)
            {
                try
                {
                    x = Convert.ToDouble(strDot.Split(',')[1]);
                    y = Convert.ToDouble(strDot.Split(',')[0]);

                    x -= minX;
                    y -= minY;


                    x /= nDelta;
                    y /= nDelta;

                    // 原点偏移(使图形居中)
                    x += deltaX;
                    y += deltaY;

                    sb += x + "," + y + " ";
                }
                catch (Exception)
                { continue; }
                System.Drawing.Point pt = new System.Drawing.Point((int)x, (int)(-y));
                DotList.Add(pt);
            }
            return DotList;
        }

        /// <summary>
        /// 计算传入坐标的最大最小值
        /// </summary>
        /// <param name="arrayList"></param>
        /// <param name="minX"></param>
        /// <param name="maxX"></param>
        /// <param name="minY"></param>
        /// <param name="maxY"></param>
        private void GetMinMaxCoordinate(List<ArrayList> arrayList, ref double minX, ref double maxX, ref double minY, ref double maxY)
        {
            double x = 0, y = 0;
            for (int i = 0; i < arrayList.Count; i++)
            {
                foreach (string strDot in arrayList[i])
                {

                    x = Convert.ToDouble(strDot.Split(',')[1]);
                    y = Convert.ToDouble(strDot.Split(',')[0]);
                    if (minX == 0)
                    {
                        minX = x;
                        minY = y;
                    }

                    minX = Math.Min(x, minX);
                    maxX = Math.Max(x, maxX);
                    minY = Math.Min(y, minY);
                    maxY = Math.Max(y, maxY);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="caseno"></param>
        /// <param name="iDb"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public string GetCoordinate(string caseno, GS.DataBase.IDbAccess iDb, string type)
        {
            string caseType = "";
            string tableName = "";
            string columns = "";
            string dotString = "";
            if (type.ToUpper() == "CK")
            {
                DataSet ds = iDb.GetDataSet(string.Format("select CLASS4,CASETYPE from k_caseno t where t.caseno='{0}'", caseno));
                string ck_guid = "";
                if (ds.Tables[0].Rows.Count > 0)
                {
                    ck_guid = ds.Tables[0].Rows[0]["CLASS4"].ToString();
                    caseType = ds.Tables[0].Rows[0]["CASETYPE"].ToString();
                    columns = "区域坐标";
                    //根据流程不同对应不同的表和相应的字段
                    switch (caseType)
                    {
                        case "1"://新立
                        case "2"://变更
                        case "3"://延续
                        case "4"://注销
                        case "12"://变更范围预审
                        case "20"://
                            tableName = "采矿申请登记";
                            break;
                        case "5":
                            tableName = "采矿转让登记";
                            break;
                        case "11":
                            tableName = "划定矿区范围";

                            break;
                        default:
                            break;
                    }
                    if (!string.IsNullOrEmpty(tableName) && !string.IsNullOrEmpty(caseType))
                    {
                        //从数据库中取到字段值并格式化
                        string selectSql = string.Format("select {0} from {1} where CK_GUID='{2}'", columns, tableName, ck_guid);
                        DataSet dsCkData = iDb.GetDataSet(selectSql);
                        if (dsCkData.Tables[0].Rows.Count > 0)
                        {
                            //格式化坐标文件
                            dotString = InputText.GetCKFormattedDotStringJT(dsCkData.Tables[0].Rows[0][0].ToString());
                        }
                    }
                }
            }
            else
            {
                DataSet ds = iDb.GetDataSet(string.Format("select 申请序号,CASETYPE from k_caseno where caseno='{0}'", caseno));
                caseType = ds.Tables[0].Rows[0]["CASETYPE"].ToString();
                string applyNo = ds.Tables[0].Rows[0]["申请序号"].ToString();
                columns = "区域坐标";
                string selectSql = @"
select {0}
  from 勘查项目受理 t
 where t.申请序号 = '{1}'
UNION
select {0}
  from 勘查项目登记 t
 where t.申请序号 = '{1}'
UNION
select {0}
  from 项目档案 t
 where t.申请序号 = '{1}'";
                if (caseType == "50")
                {
                    selectSql = "select {0} from 地质调查 where 申请序号='{1}'";

                }
                DataSet dsTKData = iDb.GetDataSet(string.Format(selectSql, columns, applyNo));
                if (dsTKData.Tables[0].Rows.Count > 0)
                {
                    dotString = InputText.GetTKFormattedDotStringJT(dsTKData.Tables[0].Rows[0][0].ToString());
                }
            }

            return dotString;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="caseno"></param>
        /// <param name="iDb"></param>
        /// <returns></returns>
        public string GetCoordinate(string caseno, GS.DataBase.IDbAccess iDb)
        {
            DataSet ds = iDb.GetDataSet(string.Format("select CLASS4,CASETYPE from k_caseno t where t.caseno='{0}'", caseno));
            string ck_guid = "";
            string caseType = "";
            string tableName = "";
            string columns = "";
            string dotString = "";
            if (ds.Tables[0].Rows.Count > 0)
            {
                ck_guid = ds.Tables[0].Rows[0]["CLASS4"].ToString();
                caseType = ds.Tables[0].Rows[0]["CASETYPE"].ToString();
                columns = "区域坐标";
                //根据流程不同对应不同的表和相应的字段
                switch (caseType)
                {
                    case "1"://新立
                    case "2"://变更
                    case "3"://延续
                    case "4"://注销
                    case "12"://变更范围预审
                    case "20"://
                        tableName = "采矿申请登记";
                        break;
                    case "5":
                        tableName = "采矿转让登记";
                        break;
                    case "11":
                        tableName = "划定矿区范围";

                        break;
                    default:
                        break;
                }
                if (!string.IsNullOrEmpty(tableName) && !string.IsNullOrEmpty(caseType))
                {
                    //从数据库中取到字段值并格式化
                    string selectSql = string.Format("select {0} from {1} where CK_GUID='{2}'", columns, tableName, ck_guid);
                    DataSet dsCkData = iDb.GetDataSet(selectSql);
                    if (dsCkData.Tables[0].Rows.Count > 0)
                    {
                        //格式化坐标文件
                        dotString = InputText.GetCKFormattedDotString(dsCkData.Tables[0].Rows[0][0].ToString());
                    }
                }
            }
            return dotString;
        }

    }
}