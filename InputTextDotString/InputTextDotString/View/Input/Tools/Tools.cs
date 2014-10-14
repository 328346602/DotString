using System;
using System.Collections.Generic;
using System.Web;
using System.Data;

namespace Mapgis.LandWeb.GisAnalyOfMine.MapGis
{
    public class Tools
    {

        public static GS.DataBase.IDbAccess GetDBByCaseType(string caseType)
        {
            string strConn = caseType.ToUpper() == "CK" ? "CK_CONN" : "connectionString";
            string strDbType = caseType.ToUpper() == "CK" ? "CK_DBTYPE" : "DataServerType";

            if (caseType.ToUpper() == "TK")
            {
                strConn = "TK_CONN";
                strDbType = "DataServerType";
            }

            return GS.DataBase.DbAccessFactory.CreateInstance(System.Configuration.ConfigurationManager.AppSettings[strConn], System.Configuration.ConfigurationManager.AppSettings[strDbType]);


        }
        public static string GetDbField(DataSet ds, string field)
        {
            string result = "";
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                if (ds.Tables[0].Rows[i][field] != DBNull.Value && !string.IsNullOrEmpty(ds.Tables[0].Rows[i][field].ToString()))
                    result += ds.Tables[0].Rows[i][field] + ",";
            }
            if (result.Length > 0)
                result=result.TrimEnd(',');
            return result;
        }

        public static string GetCaseTypeName(string ckCaseType, string caseType)
        {
            string ckCaseTypeName = "";
            if (caseType.ToUpper() == "CK")
            {
                switch (ckCaseType)
                {
                    case "1"://新立
                        ckCaseTypeName = "采矿权新立";
                        break;
                    case "2"://变更
                        ckCaseTypeName = "采矿权变更";
                        break;
                    case "3"://延续
                        ckCaseTypeName = "采矿权延续";
                        break;
                    case "4"://注销
                        ckCaseTypeName = "采矿权注销";
                        break;
                    case "12"://变更范围预审
                        ckCaseTypeName = "采矿权变更范围预审";
                        break;
                    case "20"://
                        ckCaseTypeName = "casetype20";
                        break;
                    case "5":
                        ckCaseTypeName = "采矿权转让";
                        break;
                    case "11":
                        ckCaseTypeName = "采矿权划定矿区范围";
                        break;
                }
            }
            if (caseType.ToUpper() == "TK")
            {
                switch (ckCaseType)
                {
                    case "6":
                        ckCaseTypeName = "探矿权新立";
                        break;
                    case "7":
                        ckCaseTypeName = "探矿权变更、延续、保留";
                        break;
                    case "8":
                        ckCaseTypeName = "探矿权注销";
                        break;
                    case "50":
                        ckCaseTypeName = "地质调查项目备案";
                        break;
                }
            }
            return ckCaseTypeName;
        }
        public static string GetRegionName(string RegionCode)
        {
            string[] sRegioncode = RegionCode.Split('、');
            GS.DataBase.IDbAccess iDb = GS.DataBase.DbAccessFactory.CreateInstance();
            string sRegionName = "";
            for (int i = 0; i < sRegioncode.Length; i++)
            {
                string sql = "select t.名称 from SYSAREACODE t WHERE t.代码 = '" + sRegioncode[i] + "'";
                object o = iDb.GetFirstColumn(sql);
                if (o != null && o != DBNull.Value && !string.IsNullOrEmpty(o.ToString()))
                {
                    sRegionName += o.ToString() + ",";
                }
            }
            if (!string.IsNullOrEmpty(sRegionName))
                sRegionName = sRegionName.Substring(0, sRegionName.Length - 1);
            return sRegionName;
        }

        public static string GetCKAreaCode(string caseno)
        {
            GS.DataBase.IDbAccess iDb = GetDBByCaseType("CK");
            string areaCode = "";
            //通过caseno取到ck_guid和caseType
            DataSet ds = iDb.GetDataSet(string.Format("select CLASS4,CASETYPE from k_caseno t where t.caseno='{0}'", caseno));
            string ck_guid = "";
            string caseType = "";
            string tableName = "";
            string columns = "";

            if (ds.Tables[0].Rows.Count > 0)
            {
                ck_guid = ds.Tables[0].Rows[0]["CLASS4"].ToString();
                caseType = ds.Tables[0].Rows[0]["CASETYPE"].ToString();
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
                        columns = "区域坐标,所在行政区";
                        break;
                    case "5":
                        tableName = "采矿转让登记";
                        columns = "区域坐标,所在行政区";
                        break;
                    case "11":
                        tableName = "划定矿区范围";
                        columns = "区域坐标,所在行政区";
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

                        //格式化行政区划
                        areaCode = GetAreaCode(dsCkData.Tables[0].Rows[0][1]);

                    }
                }
            }
            return areaCode;
        }

        public static string GetTKAreaCode(string caseno)
        {
            GS.DataBase.IDbAccess iDb = Tools.GetDBByCaseType("TK");

            DataSet ds = iDb.GetDataSet(string.Format("select 申请序号,CASETYPE from k_caseno where caseno='{0}'", caseno));
            string areaCode = "";
            if (ds.Tables[0].Rows.Count > 0)
            {
                string caseType = ds.Tables[0].Rows[0]["CASETYPE"].ToString();
                string columns = "区域坐标,所在行政区";
                string selectSql = @"
select {0}
  from 勘查项目受理 t
 where t.申请序号 = ({1})
UNION
select {0}
  from 勘查项目登记 t
 where t.申请序号 = ({1})
UNION
select {0}
  from 项目档案 t
 where t.申请序号 = ({1})";
                switch (caseType)
                {
                    case "50":
                        selectSql = "select {0} from 地质调查 where 申请序号=({1})";
                        columns = "区域坐标, null as 所在行政区";
                        break;
                }
                DataSet dsData = iDb.GetDataSet(string.Format(selectSql, columns, "(select 申请序号 from k_caseno where CASENO='" + caseno + "')"));
                if (dsData.Tables[0].Rows.Count > 0)
                {
                    areaCode = dsData.Tables[0].Rows[0][1] == DBNull.Value ? "" : dsData.Tables[0].Rows[0][1].ToString();

                }
            }
            return areaCode;
        }

        public static string GetAreaCode(object o)
        {
            if (o == DBNull.Value || o == null || o.ToString() == "" || o.ToString().Length < 6)
                return "";
            return o.ToString();

        }

        public static IGSAnaly.Analy GetAnalyService()
        {
            IGSAnaly.Analy analy = new IGSAnaly.Analy();
            if (System.Configuration.ConfigurationManager.AppSettings["IGSService"] != null)
                analy.Url = System.Configuration.ConfigurationManager.AppSettings["IGSService"].ToString();

            return analy;
        }

        public static IGSWpsServer.WPSServer GetWPSServer()
        {
            IGSWpsServer.WPSServer wpsServer = new IGSWpsServer.WPSServer();
            if (System.Configuration.ConfigurationManager.AppSettings["IGSWPSServer"] != null)
                wpsServer.Url = System.Configuration.ConfigurationManager.AppSettings["IGSWPSServer"].ToString();
            return wpsServer;
        }

        /// <summary>
        /// 格式化采矿权数据库中储存的坐标串
        /// </summary>
        /// <param name="strDot">坐标串</param>
        /// <returns>格式化后的坐标串</returns>
        public static string GetCKFormattedDotString(string strDot)
        {
            if (string.IsNullOrEmpty(strDot))
            {
                return "";
            }

            string[] strDots = strDot.Split(',');
            int iPlotNo = int.Parse(strDots[0]);        //圈数
            int iCurIndex = 1;
            string strDotString = "";

            for (int i = 0; i < iPlotNo; i++)
            {
                try
                {
                    int iPointNo = int.Parse(strDots[iCurIndex]);
                    iCurIndex += 2;

                    for (int j = 0; j < iPointNo; j++)
                    {
                        strDotString += strDots[iCurIndex] + "," + strDots[iCurIndex + 1] + " ";
                        //strDotString += strDots[iCurIndex+1] + "," + strDots[iCurIndex] + " ";
                        iCurIndex += 3;
                    }

                    if (strDotString.EndsWith(" "))
                    { strDotString = strDotString.Substring(0, strDotString.Length - 1); }

                    strDotString += "@";

                    iCurIndex += 3;
                }
                catch
                { }
            }

            if (strDotString.EndsWith("@"))
            { strDotString = strDotString.Substring(0, strDotString.Length - 1); }

            return strDotString;
        }
        /// <summary>
        /// 格式化采矿权数据库中储存的坐标串
        /// </summary>
        /// <param name="strDot">坐标串</param>
        /// <returns>格式化后的坐标串</returns>
        public static string GetCKFormattedDotStringJT(string strDot)
        {
            if (string.IsNullOrEmpty(strDot))
            {
                return "";
            }

            string[] strDots = strDot.Split(',');
            int iPlotNo = int.Parse(strDots[0]);        //圈数
            int iCurIndex = 1;
            string strDotString = "";

            for (int i = 0; i < iPlotNo; i++)
            {
                try
                {
                    int iPointNo = int.Parse(strDots[iCurIndex]);
                    iCurIndex += 2;

                    for (int j = 0; j < iPointNo; j++)
                    {
                        //strDotString += strDots[iCurIndex] + "," + strDots[iCurIndex + 1] + " ";
                        strDotString += strDots[iCurIndex] + "," + strDots[iCurIndex+1] + " ";
                        iCurIndex += 3;
                    }

                    if (strDotString.EndsWith(" "))
                    { strDotString = strDotString.Substring(0, strDotString.Length - 1); }

                    strDotString += "@";

                    iCurIndex += 3;
                }
                catch
                { }
            }

            if (strDotString.EndsWith("@"))
            { strDotString = strDotString.Substring(0, strDotString.Length - 1); }

            return strDotString;
        }

        /// <summary>
        /// 格式化探矿权数据库中储存的坐标串
        /// </summary>
        /// <param name="strDot">坐标串</param>
        /// <returns>格式化后的坐标串</returns>
        public static string GetTKFormattedDotString(string strDot)
        {
            if (string.IsNullOrEmpty(strDot))
            {
                return "";
            }

            string[] strDots = strDot.Split(',');
            int iPlotNo = int.Parse(strDots[0]);        //圈数
            int iCurIndex = 1;
            string strDotString = "";

            for (int i = 0; i < iPlotNo; i++)
            {
                try
                {
                    int iPointNo = int.Parse(strDots[iCurIndex]);
                    iCurIndex++;

                    for (int j = 0; j < iPointNo; j++)
                    {
                        strDotString += strDots[iCurIndex] + "," + strDots[iCurIndex + 1] + " ";
                        iCurIndex += 2;
                    }

                    if (strDotString.EndsWith(" "))
                    { strDotString = strDotString.Substring(0, strDotString.Length - 1); }

                    strDotString += "@";

                    iCurIndex += 3;
                }
                catch
                { }
            }

            if (strDotString.EndsWith("@"))
            { strDotString = strDotString.Substring(0, strDotString.Length - 1); }

            return strDotString;
        }

        /// <summary>
        /// 格式化探矿权数据库中储存的坐标串
        /// </summary>
        /// <param name="strDot">坐标串</param>
        /// <returns>格式化后的坐标串</returns>
        public static string GetTKFormattedDotStringJT(string strDot)
        {
            if (string.IsNullOrEmpty(strDot))
            {
                return "";
            }

            string[] strDots = strDot.Split(',');
            int iPlotNo = int.Parse(strDots[0]);        //圈数
            int iCurIndex = 1;
            string strDotString = "";

            for (int i = 0; i < iPlotNo; i++)
            {
                try
                {
                    int iPointNo = int.Parse(strDots[iCurIndex]);
                    iCurIndex++;

                    for (int j = 0; j < iPointNo; j++)
                    {
                        strDotString += strDots[iCurIndex + 1] + "," + strDots[iCurIndex] + " ";
                        iCurIndex += 2;
                    }

                    if (strDotString.EndsWith(" "))
                    { strDotString = strDotString.Substring(0, strDotString.Length - 1); }

                    strDotString += "@";

                    iCurIndex += 3;
                }
                catch
                { }
            }

            if (strDotString.EndsWith("@"))
            { strDotString = strDotString.Substring(0, strDotString.Length - 1); }

            return strDotString;
        }
        /// <summary>
        /// 获取
        /// </summary>
        /// <returns></returns>
        public static string GetWebGisUrl()
        {
            return System.Configuration.ConfigurationManager.AppSettings["GraphPage"];
        }
    }


    public class PointCheck
    {
        /// <summary>
        /// 坐标点结构体
        /// </summary>
        public struct Dot
        {
            public double Mx;
            public double My;
        }

        /// <summary>
        /// 错误的点的信息，包含点号和错误的点
        /// </summary>
        public struct WrongDotMsg
        {
            public int DotNo;
            public Dot WDot;
        }

        /// <summary>
        /// 线段，2个点的集合
        /// </summary>
        public struct LinSeg
        {
            public Dot A, B;
        }

        /// <summary>
        /// 坐标严格检查
        /// </summary>
        public class CoordinateCheck
        {
            private double dRange = 0.0000001;
            private double PI = 3.14159265;




            //TODO 线是否封闭
            /// <summary>
            /// Determines whether [is lin closed] [the specified ds].
            /// 线是否封闭
            /// </summary>
            /// <param name="ds">The ds.</param>
            /// <returns>
            /// 	<c>true</c> if [is lin closed] [the specified ds]; otherwise, <c>false</c>.
            /// </returns>
            public bool IsLinClosed(string strPoint)
            {
                string[] strPointList = strPoint.Split(',');
                int iCount = strPointList.Length / 2;
                return iCount > 2 && IsSameDot(strPointList);
            }

            /// <summary>
            /// Determines whether [is same dot] [the specified ds].
            /// </summary>
            /// <param name="dotA">The dot A.</param>
            /// <param name="dotB">The dot B.</param>
            /// <returns>
            /// 	<c>true</c> if [is same dot] [the specified ds]; otherwise, <c>false</c>.
            /// </returns>
            private bool IsSameDot(Dot dotA, Dot dotB)
            {
                if (IsSameDouble(dotA.Mx, dotB.Mx, dRange) && IsSameDouble(dotA.My, dotB.My, dRange))
                    return true;
                return false;
            }

            /// <summary>
            /// Determines whether [is same dot] [the specified ds].
            /// </summary>
            /// <param name="ds">The ds.</param>
            /// <returns>
            /// 	<c>true</c> if [is same dot] [the specified ds]; otherwise, <c>false</c>.
            /// </returns>
            private bool IsSameDot(string[] strPointList)
            {
                int rowCount = strPointList.Length;
                double x1 = Convert.ToDouble(strPointList[0]);
                double y1 = Convert.ToDouble(strPointList[1]);
                double x2 = Convert.ToDouble(strPointList[rowCount - 2]);
                double y2 = Convert.ToDouble(strPointList[rowCount - 1]);
                if (IsSameDouble(x1, x2, dRange) && IsSameDouble(y1, y2, dRange))
                    return true;
                return false;
            }

            /// <summary>
            /// Determines whether [is same double] [the specified a].
            /// </summary>
            /// <param name="a">A.</param>
            /// <param name="b">The b.</param>
            /// <param name="range">The range.</param>
            /// <returns>
            /// 	<c>true</c> if [is same double] [the specified a]; otherwise, <c>false</c>.
            /// </returns>
            private bool IsSameDouble(double a, double b, double range)
            {
                return Math.Abs(a - b) < range ? true : false;
            }

            //TODO 线是否自相交
            /// <summary>
            /// Determines whether [is line intersect].
            /// 线是否自相交
            /// </summary>
            /// <param name="ds">The ds.</param>
            /// <returns>
            /// 	<c>true</c> if [is line intersect]; otherwise, <c>false</c>.
            /// </returns>
            public bool IsLinSelfCross(string strPoint)
            {
                //WriteSMLogs("strPoint:" + strPoint);
                string[] strPointList = strPoint.Split(',');
                int rowCount = strPointList.Length / 2;
                if (rowCount < 4)
                    return false;
                rowCount = strPointList.Length;
                LinSeg[] linSeg = new LinSeg[2];
                for (int i = 0; i < rowCount - 2; )
                {
                    //WriteSMLogs("i:" + i);
                    Dot dotA = new Dot
                    {
                        Mx = Convert.ToDouble(strPointList[i]),
                        My = Convert.ToDouble(strPointList[i + 1])
                    };
                    //WriteSMLogs("dotA:" + dotA.Mx + " " + dotA.My);
                    i = i + 2;
                    //WriteSMLogs("i:" + i);
                    Dot dotB = new Dot
                    {
                        Mx = Convert.ToDouble(strPointList[i]),
                        My = Convert.ToDouble(strPointList[i + 1])
                    };
                    //WriteSMLogs("dotB:" + dotB.Mx + " " + dotB.My);
                    linSeg[0].A = dotA;
                    linSeg[0].B = dotB;
                    for (int j = i; j < rowCount - 2; )
                    {
                        //WriteSMLogs("j:" + j);
                        Dot dotC = new Dot
                        {
                            Mx = Convert.ToDouble(strPointList[j]),
                            My = Convert.ToDouble(strPointList[j + 1])
                        };
                        //WriteSMLogs("dotC:" + dotC.Mx + " " + dotC.My);
                        j = j + 2;
                        //WriteSMLogs("j:" + j);
                        Dot dotD = new Dot
                        {
                            Mx = Convert.ToDouble(strPointList[j]),
                            My = Convert.ToDouble(strPointList[j + 1])
                        };
                        //WriteSMLogs("dotD:" + dotD.Mx + " " + dotD.My);
                        linSeg[1].A = dotC;
                        linSeg[1].B = dotD;

                        if (IntersectJudge(linSeg[0], linSeg[1]) > 0)
                        {
                            return false;
                        }
                    }
                }
                return true;
            }

            /// <summary>
            /// Intersects the judge.
            /// 确定两条线段是否相交
            /// </summary>
            /// <param name="seg">The seg.</param>
            /// <param name="linSeg">The lin seg.</param>
            /// <returns></returns>
            private bool Intersect(LinSeg seg, LinSeg linSeg)
            {
                bool a = ((Math.Max(seg.A.Mx, seg.B.Mx) >= Math.Min(linSeg.A.Mx, linSeg.B.Mx)) &&
                          (Math.Max(linSeg.A.Mx, linSeg.B.Mx) >= Math.Min(seg.A.Mx, seg.B.Mx)) &&
                           (Math.Max(seg.A.My, seg.B.My) >= Math.Min(linSeg.A.My, linSeg.B.My)) &&
                           (Math.Max(linSeg.A.My, linSeg.B.My) >= Math.Min(seg.A.My, seg.B.My)) &&
                           (Multiply(linSeg.A, seg.B, seg.A) * Multiply(seg.B, linSeg.B, seg.A) >= 0) &&
                           (Multiply(seg.A, linSeg.B, linSeg.A) * Multiply(linSeg.B, seg.B, linSeg.A) >= 0));
                return a;
            }

            /// <summary>
            /// Intersects the judge.
            /// 一种线段相交判断函数，当且仅当seg,linSeg相交并且交点不是seg,linSeg的端点时函数为true
            /// </summary>
            /// <param name="seg">The seg.</param>
            /// <param name="linSeg">The lin seg.</param>
            /// <returns></returns>
            private int IntersectJudge(LinSeg seg, LinSeg linSeg)
            {
                bool a = ((Intersect(seg, linSeg)) && (!IsSameDot(seg.A, linSeg.A)) && (!IsSameDot(seg.A, linSeg.B)) && (!IsSameDot(seg.B, linSeg.A)) && (!IsSameDot(seg.B, linSeg.B)));

                return a ? 1 : 0;
            }

            /// <summary>
            /// Multiplies the specified p0.
            /// </summary>
            /// <param name="p0">The p0.</param>
            /// <param name="p1">The p1.</param>
            /// <param name="p2">The p2.</param>
            /// <returns></returns>
            private double Multiply(Dot p0, Dot p1, Dot p2)
            {
                double s = ((p1.Mx - p0.Mx) * (p2.My - p0.My) - (p2.Mx - p0.Mx) * (p1.My - p0.My));
                return s;
            }

            //TODO 点是否重叠
            /// <summary>
            /// Determines whether [is dot over lap] [the specified ds].
            /// 点是否重叠
            /// </summary>
            /// <param name="ds">The ds.</param>
            /// <returns>
            /// 	<c>true</c> if [is dot over lap] [the specified ds]; otherwise, <c>false</c>.
            /// </returns>
            public bool IsDotOverLap(string strPoint)
            {
                int wrongCount = 0;
                string[] strPointList = strPoint.Split(',');
                int rowCount = strPointList.Length;
                for (int i = 0; i < rowCount - 1; )
                {
                    for (int j = i + 2; j < rowCount; )
                    {
                        if (i == 0 && j == rowCount / 2 - 1)
                            continue;
                        Dot dotA = new Dot
                        {
                            Mx = Convert.ToDouble(strPointList[i]),
                            My = Convert.ToDouble(strPointList[i + 1])
                        };
                        Dot dotB = new Dot
                        {
                            Mx = Convert.ToDouble(strPointList[j]),
                            My = Convert.ToDouble(strPointList[j + 1])
                        };
                        if (IsSameDot(dotA, dotB))
                        {
                            wrongCount++;
                            //有重叠，需要记录重叠点信息
                        }
                        j = j + 2;
                    }
                    i = i + 2;
                }
                return wrongCount != 0;
            }

            /// <summary>
            /// Determines whether [is dot over lap] [the specified ds].
            /// 点是否重叠
            /// </summary>
            /// <param name="ds">The ds.</param>
            /// <param name="dotMsg">The dot MSG.</param>
            /// <returns>
            /// 	<c>true</c> if [is dot over lap] [the specified ds]; otherwise, <c>false</c>.
            /// </returns>
            public bool IsDotOverLap(string strPoint, ref WrongDotMsg[] dotMsg)
            {
                int wrongCount = 0;
                string[] strPointList = strPoint.Split(',');
                int rowCount = strPointList.Length;
                //WriteSMLogs("rowCount:" + rowCount.ToString());
                for (int i = 0; i < rowCount - 1; )
                {
                    //WriteSMLogs("i:" + i.ToString());
                    for (int j = i + 2; j < rowCount; )
                    {
                        //WriteSMLogs("j:" + j.ToString());
                        if (i == 0 && j == rowCount - 2)
                        {
                            j = j + 2;
                            continue;
                        }
                        Dot dotA = new Dot
                        {
                            Mx = Convert.ToDouble(strPointList[i]),
                            My = Convert.ToDouble(strPointList[i + 1])
                        };
                        Dot dotB = new Dot
                        {
                            Mx = Convert.ToDouble(strPointList[j]),
                            My = Convert.ToDouble(strPointList[j + 1])
                        };
                        //WriteSMLogs("dotA:" + dotA.Mx +" "+ dotA.My);
                        //WriteSMLogs("dotB:" + dotB.Mx +" "+ dotB.My);
                        if (IsSameDot(dotA, dotB))
                        {
                            //WriteSMLogs("有重叠");
                            //有重叠，需要记录重叠点信息
                            dotMsg[wrongCount++].DotNo = i / 2;
                            if (j == rowCount / 2 - 1)
                            {
                                dotMsg[wrongCount++].WDot.Mx = dotA.Mx;
                                dotMsg[wrongCount++].WDot.My = dotA.My;
                            }
                            else
                            {
                                dotMsg[wrongCount++].WDot.Mx = dotB.Mx;
                                dotMsg[wrongCount++].WDot.My = dotB.My;
                            }
                        }
                        j = j + 2;
                    }
                    i = i + 2;
                }
                if (wrongCount == 0)
                    return true;
                else
                    return false;
            }
            public static void WriteSMLogs(string sLog)
            {

            }

            #region//暂时用不到
            //TODO 区和区之间关系判断
            /// <summary>
            /// Determines whether [is dot in reg].
            /// 判断点与多边形的关系
            /// </summary>
            /// <param name="dots">The dots.多边形坐标串</param>
            /// <param name="dot">The dot.被判断坐标点</param>
            /// <returns>
            /// -1 失败,0  点在多边形外部,1  点在多边形内部,2  点在多边形端点，3  点在多边形的边上(不包括端点)
            /// </returns>
            private int IsDotInReg(Dot[] dots, Dot dot)
            {
                if (dots == null || dots.Length == 0)
                    return -1;
                double sumangle = 0.0;
                double dangle = 0.0;
                int dotCount = dots.Length;
                Dot p1;
                Dot p2;
                for (int i = 0; i < dotCount; i++)
                {
                    p1.Mx = dots[(i + 1) % dotCount].Mx;
                    p1.My = dots[(i + 1) % dotCount].My;
                    p2.Mx = dots[i % dotCount].Mx;
                    p2.My = dots[i % dotCount].My;
                    if (((Math.Abs(dot.Mx - p1.Mx) < dRange) && Math.Abs(dot.My - p1.My) < dRange) ||
                        ((Math.Abs(dot.Mx - p2.Mx) < dRange) && Math.Abs(dot.My - p2.My) < dRange))
                    {
                        return 2; //点在多边形边的端点上
                    }
                    dangle = Angle(dot, p1) - Angle(dot, p2); //计算p1到dot,p2到dot矢量的夹角
                    if (Math.Abs(Math.Abs(dangle) - PI) < 0.00001)
                    {
                        return 3; //点在多边形的边上（不包括端点）
                    }
                    if (dangle > PI)
                        dangle = dangle - 2 * PI;
                    if (dangle < -PI)
                        dangle = 2 * PI + dangle;
                    sumangle = sumangle + dangle;
                }
                if (Math.Abs(sumangle) < dRange)
                    return 0; //点在多边形的外面
                if (Math.Abs(Math.Abs(sumangle) - 2 * PI) < dRange || Math.Abs(Math.Abs(sumangle) - 4 * PI) < dRange)
                    return 1; //点在多边形的内部
                return 2;
            }

            /// <summary>
            /// Angles the specified dot A.
            /// 计算角度 
            /// </summary>
            /// <param name="dotA">The dot A.</param>
            /// <param name="dotB">The dot B.</param>
            /// <returns></returns>
            private double Angle(Dot dotA, Dot dotB)
            {
                double angle = Math.Atan2(dotB.My - dotA.My, dotB.Mx - dotA.Mx);
                return angle;
            }

            /// <summary>
            /// Distofs the dot to dot.
            /// 计算两点之间欧氏距离 
            /// </summary>
            /// <param name="dotA">The dot A.</param>
            /// <param name="dotB">The dot B.</param>
            /// <returns></returns>
            private double DistofDotToDot(Dot dotA, Dot dotB)
            {
                return (Math.Sqrt((dotA.Mx - dotB.Mx) * (dotA.Mx - dotB.Mx) + (dotA.My - dotB.My) * (dotA.My - dotB.My)));
            }

            /// <summary>
            /// Gets the max big segment.
            /// 计算线上最长段
            /// </summary>
            /// <param name="dot">The dot.</param>
            /// <returns></returns>
            private int GetMaxBigSegment(Dot[] dot)
            {
                int i;
                double bakLength = 0;
                if (dot == null || dot.Length <= 1)
                    return 0;
                int bakIdx = 0;
                bakLength = DistofDotToDot(dot[0], dot[1]);
                double length = 0;
                for (i = 0; i < dot.Length; i++)
                {
                    length = DistofDotToDot(dot[i], dot[i + 1]);
                    if (length > bakLength)
                    {
                        bakLength = length;
                        bakIdx = i;
                    }
                }
                return bakIdx;
            }

            /// <summary>
            /// Determines whether [is line on reg].
            /// 判断线在区内还是边上还是无关
            /// </summary>
            /// <param name="dots">The dots.</param>
            /// <param name="lineDots">The line dots.</param>
            /// <returns></returns>
            public int IsLineOnReg(Dot[] dots, Dot[] lineDots)
            {
                if (dots == null)
                    return 0;
                int flag = 0;
                int segNo = 0;
                if (lineDots == null || lineDots.Length < 2)
                    return 0;
                // 取线上最长段，返回段号
                segNo = GetMaxBigSegment(lineDots);
                //中点
                Dot dotxy;
                dotxy.Mx = (lineDots[segNo].Mx + lineDots[segNo + 1].Mx) / 2.0;
                dotxy.My = (lineDots[segNo].My + lineDots[segNo + 1].My) / 2.0;
                flag = IsDotInReg(dots, dotxy);
                if (flag > 0)
                {
                    //起点
                    flag = IsDotInReg(dots, lineDots[0]);
                    if (flag <= 0)
                    {
                        dotxy.Mx = lineDots[0].Mx;
                        dotxy.My = lineDots[0].My;
                    }
                    else
                    {
                        //终点
                        flag = IsDotInReg(dots, lineDots[lineDots.Length - 1]);
                        if (flag <= 0)
                        {
                            dotxy.Mx = lineDots[lineDots.Length - 1].Mx;
                            dotxy.My = lineDots[lineDots.Length - 1].My;
                        }
                    }
                }
                return flag;
            }

            /// <summary>
            /// Formats the err MSG.
            /// </summary>
            /// <returns></returns>
            private string FormatErrMsg()
            {
                return "";
            }
        }
            #endregion
    }

    public class LandCodeValue
    {
        private string _landCodeName;

        public string LandCodeName
        {
            get { return _landCodeName; }
            set { _landCodeName = value; }
        }
        private string _landCode;

        public string LandCode
        {
            get { return _landCode; }
            set { _landCode = value; }
        }
        private double _sumValue;

        private double _oldSumValue;

        public double OldSumValue
        {
            get { return _oldSumValue; }
            set { _oldSumValue = value; }
        }

        public double SumValue
        {
            get { return _sumValue; }
            set { _sumValue = value; }
        }

        public LandCodeValue(string landCode, string landCodeName)
        {
            _landCode = landCode;
            _landCodeName = landCodeName;
        }
        private List<LandCodeValue> _list = new List<LandCodeValue>();

        public List<LandCodeValue> List
        {
            get { return _list; }
            set { _list = value; }
        }

        public int GetChildLength()
        {
            int length = 1;
            if (List != null)
            {
                foreach (LandCodeValue landCode in List)
                {
                    if (landCode.LandCode.Length == 3)
                        length += 1;
                    else
                        length += landCode.GetChildLength();
                }
            }

            return length;
        }
        public bool CanAdd()
        {
            bool result = true;
            if (OldSumValue == 0 && OldSumValue == SumValue)
            {
                result = false;
            }
            return result;
        }
        public double GetTotalOld()
        {
            if (LandCode == "total" || LandCode == "totalS")
                return 0;
            double total = OldSumValue;
            if (List != null)
            {
                foreach (LandCodeValue landCode in List)
                {
                    if (landCode.LandCode.Length == 3)
                        total += landCode.OldSumValue;
                    else
                        total += landCode.GetTotalOld();
                }
            }
            return Math.Round(total, 4);
        }
        public double GetTotal()
        {
            if (LandCode == "total" || LandCode == "totalS")
                return 0;
            double total = SumValue;
            foreach (LandCodeValue landCode in List)
            {
                if (landCode.LandCode.Length == 3)
                    total += landCode.SumValue;
                else
                    total += landCode.GetTotal();
            }
            return Math.Round(total, 4);
        }

    }
}