using System;
using System.Collections.Generic;
using System.Web;
using System.Collections;

using System.Web.UI;

namespace InputTextDotString.View.Input.InputTextDotString
{
    
    /// <summary>
    /// DrawMap 的摘要说明
    /// </summary>
    public class DrawMap : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            try
            {
                string caseNo = context.Request["caseNo"];
                string caseType = context.Request["caseType"];
                //if (string.IsNullOrEmpty(caseType))
                //    caseType = "CK";

                #region 判断传参数类型，调取不同数据
                if (!string.IsNullOrEmpty(caseNo))
                {
                    string coordinate = string.Empty;
                    if (caseType == "CK")//从CM_LC_CKQ表中取坐标数据
                    {
                        coordinate = InputText.getDotsFromCMLCCKQ(caseNo);
                    }
                    else if (caseType == "CKSQDJ")
                    {
                        coordinate = InputText.getDotsFromCKSQDJ(caseNo);
                    }
                    else if (caseType == "TK")
                    {
                        coordinate = InputText.getDotsFromCMLCKCXKZ(caseNo);
                    }
                    else if (caseType == "KCXMDJ")
                    {
                        coordinate = InputText.getDotsFromKCXMDJ(caseNo);
                    }
                    Models.DrawMap dm = new Models.DrawMap();
                    dm.Draw(coordinate, context);
                    #region 原绘图方法代码
                    /*
                    //string coordinate = GetCoordinate(caseNo, Tools.GetDBByCaseType(caseType.ToUpper()),caseType); //Tools.GetCKFormattedDotString("3,38,1,3346685.00,38583830.85,2,3346685.00,38584170.85,3,3346230.00,38585120.86,4,3345960.01,38585885.87,5,3346200.01,38585975.87,6,3345980.01,38586540.88,7,3345120.00,38587240.89,8,3345120.01,38588240.90,9,3343825.00,38588670.91,10,3342986.99,38588580.91,11,3342904.98,38587540.90,12,3343404.99,38587125.89,13,3343404.99,38586910.89,14,3344404.99,38586175.88,15,3344404.99,38585575.88,16,3343904.98,38585566.88,17,3343904.98,38585375.88,18,3343291.98,38585067.88,19,3343291.98,38585067.88,20,3343744.98,38584845.87,21,3344194.98,38584996.87,22,3344471.99,38584787.87,23,3344404.98,38584515.86,24,3344169.98,38584555.87,25,3344207.98,38583055.85,26,3344589.98,38582805.85,27,3345396.99,38584040.86,28,3345034.99,38584265.86,29,3345217.99,38584535.86,30,3344959.99,38584810.87,31,3344904.99,38584770.87,32,3344337.99,38585390.87,33,3344784.99,38585420.87,34,3344904.99,38584920.87,35,3345279.99,38584505.86,36,3345204.99,38584393.86,37,3345827.00,38583980.85,38,3346405.00,38583811.85,276,-280,,1,5,39,3342434.97,38586455.89,40,3342174.97,38586930.90,41,3341084.96,38586875.90,42,3341114.96,38586385.90,43,3341859.97,38586170.89,100,50,,1,5,44,3348930.02,38583730.84,45,3347755.02,38585120.85,46,3347105.01,38585270.86,47,3347535.01,38584560.85,48,3348645.02,38583465.83,101,50,,1,"); //GetCoordinate(caseNo, Tools.GetDBByCaseType(caseType.ToUpper()));
                    string[] coordinates = coordinate.Split('@');
                    //封闭每一个坐标 //处理数据
                    MapgisEgov.AnalyInput.Common.Log.Write("取坐标结束，根据以下坐标串生成简图："+coordinate);
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
                    List<ArrayList> dotList = new List<ArrayList>(coordinateList);
                    GetMinMaxCoordinate(coordinateList, ref minX, ref maxX, ref minY, ref maxY);
                    //MapgisEgov.AnalyInput.Common.Log.Write("minX=" + minX + ";minY=" + minY + ";maxX" + maxX + ";maxY" + maxY);
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
                                //string str = points[i].ToString() + "," + points[1].ToString();
                                string str = dotList[iP][i].ToString().Replace(",",",\n");
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
                        catch(Exception oExcept)
                        {
                            MapgisEgov.AnalyInput.Common.Log.Write("标记点坐标时出现错误："+oExcept.Message);
                        }
                         
                        #endregion
                    }

                    context.Response.ContentType = "image/jpeg";
                    g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality; 
                    bm.Save(context.Response.OutputStream, ImageFormat.Jpeg);
                    MapgisEgov.AnalyInput.Common.Log.Write("==========================DrawMap.ashx执行完毕，成功生成简图！==========================");
                }
                     * */
                    #endregion
                }
                #endregion
                else if (string.IsNullOrEmpty(caseNo))
                {
                    Models.DrawMap dm = new Models.DrawMap();
                    string strMessage = "未查询到相关数据！";
                    
                    dm.Draw(ref strMessage, context);
                    MapgisEgov.AnalyInput.Common.Log.Write("未传入对应参数！");
                }
            }
            catch (Exception oExcept)
            {
                MapgisEgov.AnalyInput.Common.Log.Write(oExcept.Message);
                MapgisEgov.AnalyInput.Common.Log.Write("==================================DrawMap.ashx页面报错=================================");
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}