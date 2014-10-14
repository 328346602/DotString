/*****************************************************
 * 创建人：王金河
 * 创建日期：2013-10-29 14:20
 * 内容：
 * 这个类主要用来操作政务库中的地块坐标信息表(DKZBXX)和地块坐标子表(DKZB_ZB)
 * 主要是一些静态方法，方便调用。
 * 注意：
 * (1)导入部坐标时，地块编号采用(地块编号-地块名称)，中间加"-",以后地块编号重复多了再修改
 * 
 * 
 *******************************************************/
using System;
using System.Collections.Generic;
using System.Web;
using System.IO;
using System.Collections;
using System.Text;

using System.Data;

namespace MapgisEgov.AnalyInput.Models
{
    public class DKZBXX
    {
        static string VsService = System.Configuration.ConfigurationManager.AppSettings["IGSLandService"];
        static string VsServicePathA = VsService + "/Analy.asmx"; //Analy.asmx地址
        static string VsServicePathF = VsService + "/Feature.asmx";//Feature.asmx地址
        static string VsServicePathB = VsService + "/BaseDataInfo.asmx";//BaseDataInfo.asmx地址

        #region     检查地块编号是否存在
        /// <summary>
        /// 检查地块编号是否存在
        /// </summary>
        /// <param name="iDb"></param>
        /// <param name="sPlotNo">地块编号</param>
        /// <returns>存在返回true</returns>
        public static bool IsPlotExists(GS.DataBase.IDbAccess iDb, string sPlotNo)
        {
            bool flag = true;
            string tableName = "DKZBXX";
            string sFilter = string.Format(" DKBH='{0}'", sPlotNo);
            flag = iDb.JudgeRecordExist(tableName, sFilter);
            return flag;
        }
        #endregion

        #region   解析txt文本坐标(单地块)，然后存入到表DKZBXX和表DKZB_ZB中

        /// <summary>
        /// 解析txt文本坐标(单地块)，然后存入到表DKZBXX和表DKZB_ZB中
        /// </summary>
        /// <param name="iDb"></param>
        /// <param name="filedata">文件</param>
        /// <param name="strCaseNo">案卷编号，可以为空</param>
        /// <param name="strPlotNo">地块编号，不可以为空</param>
        /// <param name="strPlotName">地块名称</param>
        /// <param name="sPoint">需要返回的坐标串</param>
        /// <param name="sMsg">返回的错误信息</param>
        public static void InputTxt(GS.DataBase.IDbAccess iDb, HttpPostedFile filedata, string strCaseNo, string strPlotNo, string strPlotName, out string sPoint, out string sMsg)
        {
            sMsg = "";          
            Stream objStream = filedata.InputStream;
            StreamReader sr = new StreamReader(objStream, Encoding.GetEncoding("GB2312"));
            string paramJZDH = string.Empty; //界址点号
          

            string paramDKQH = ""; //地块圈号
            sPoint = "";


            sPoint = ResolveTxtFile(sr, out paramJZDH, out paramDKQH, out sMsg);
            if (!String.IsNullOrEmpty(sMsg)) // 如果有错误就提前返回
            {
                return;
            }
            if(String.IsNullOrEmpty(sPoint) || sPoint.Length < 2)
            {
                sMsg = "解析得到的坐标串为空";
                return;
            }
            string[] JZDHList = paramJZDH.Split(',');
            string[] pointList = sPoint.Split(' ');

            Hashtable ht = new Hashtable();
            Hashtable htzb = new Hashtable();

            try
            {

                ht.Clear();
                ht.Add("CASENO", strCaseNo);
                ht.Add("DKBH", strPlotNo);
                ht.Add("DKMC", strPlotName);
                ht.Add("JZDS", JZDHList.Length - 1); // 坐标点的个数
                ht.Add("RiQi", System.DateTime.Now.ToString("yyyy-MM-dd"));
                iDb.AddData("DKZBXX", ht);

                for (int ii = 0; ii < pointList.Length; ii++)
                {
                    htzb.Clear();
                    htzb.Add("CASENO", strCaseNo);
                    htzb.Add("BJITEMID", iDb.SetID("DKZB_ZB", "BJITEMID"));
                    htzb.Add("XuHao", (ii + 1).ToString());
                    htzb.Add("JZDH", JZDHList[ii]);
                    htzb.Add("ZZBX", pointList[ii].ToString().Split(',')[0]);
                    htzb.Add("HZBY", pointList[ii].ToString().Split(',')[1]);
                    htzb.Add("DKQH", "1");
                    htzb.Add("DKBH", strPlotNo);
                    iDb.AddData("DKZB_ZB", htzb);
                }

            }
            catch(Exception ex)
            {
                sMsg = ex.Message;
            }
        }

        #endregion

        /// <summary>
        /// 解析纯坐标文件,返回坐标字符串，如果不闭合，在最后添加第一个坐标点。
        /// </summary>
        public static string ResolveTxtFile(StreamReader sr, out string paramJZDH, out string paramDKQH, out string errorMsg)
        {
            paramJZDH = string.Empty;
            paramDKQH = string.Empty;
            errorMsg = string.Empty;
            StringBuilder sbuilder = new StringBuilder();
            StringBuilder JZDH = new StringBuilder();
            string temp;
            string[] pointlist;
            int count = 1;
            try
            {
                while (!sr.EndOfStream)
                {
                    temp = sr.ReadLine();
                    if (!temp.Equals("E"))
                    {
                        JZDH.Append(count);
                        JZDH.Append(",");
                        count++;
                        paramDKQH += "1,";
                        pointlist = temp.Split(',');
                        sbuilder.Append(pointlist[0]);
                        sbuilder.Append(",");
                        sbuilder.Append(pointlist[1]);
                        sbuilder.Append(" ");
                    }
                }
            }
            catch (Exception ex)
            {
                errorMsg = "解析坐标文件异常:请检查文件格式!信息:" + ex.Message;
                return null;
            }
            finally
            {
                sr.Close();
            }
            string rePoint = sbuilder.ToString().Trim(' ');
            StringBuilder pointBuilder = new StringBuilder();
            pointBuilder.Append(rePoint);

            paramJZDH = JZDH.ToString().TrimEnd(',');
            paramDKQH = paramDKQH.TrimEnd(',');
            sr.Close();
           
            return pointBuilder.ToString();
        }

        #region     解析部坐标文件，然后存入到表DKZBXX和表DKZB_ZB中
        /// <summary>
        /// 导入部坐标对应函数
        /// </summary>
        /// <param name="iDb"></param>
        /// <param name="filedata"></param>
        /// <param name="strCaseNo">案件编号</param>
        /// <param name="sMsg"></param>
        /// <param name="sAllPlotNo">保存所有的地块编号</param>
        /// <returns></returns>
        public static bool ImportDotStringTY(GS.DataBase.IDbAccess iDb, HttpPostedFile filedata, string strCaseNo, ref string sMsg, out string sAllPlotNo)
        {
            sAllPlotNo = "";//保存多个地块的编号，使用分号间隔
          
            bool bRev = true;
            sMsg = "";
            Stream objStream = filedata.InputStream;
            StreamReader sr = new StreamReader(objStream, Encoding.GetEncoding("GB2312"));
            string paramJZDH = string.Empty;
            string paramDKQH = string.Empty;
            string infoMsgParam = string.Empty;
            string errorMsg = string.Empty;
            string point = string.Empty;

            ResolveDeptFile(iDb,sr, out paramJZDH, out paramDKQH, out infoMsgParam, out point, out errorMsg,out sAllPlotNo);
          

            if(!string.IsNullOrEmpty(errorMsg))
            {
                sMsg = errorMsg;
            
                return false;
            }

            if (String.IsNullOrEmpty(point) || point.Length < 2)
            {
                sMsg = "解析得到的坐标串为空";
               
                return false;
            }

            if (errorMsg == "" || errorMsg == null)
            {
                string[] DKQHList = paramDKQH.Split('*');
                string[] JZDHList = paramJZDH.Split('*');
                string[] infoMegList = infoMsgParam.Split('@');
                string[] pointList = point.Split('#');
                int iCount = infoMegList.Length;
                Hashtable ht = new Hashtable();
                Hashtable htzb = new Hashtable();
                
                Common.Log.Write("iCount:" + iCount);
                string sPlotNo = ""; //临时保存地块编号 保存形式是  地块号-地块名称   
                for (int ii = 0; ii < iCount; ii++)
                {
                    string[] temp = infoMegList[ii].ToString().Split(',');
                    sPlotNo = temp[2] + "-" + temp[3]; //临时保存地块编号

                    ht.Clear();
                    Common.Log.Write("strCaseNo:" + strCaseNo);
                    ht.Add("CASENO", strCaseNo);
                    Common.Log.Write("temp[5]:" + temp[5]);
                    ht.Add("TuFuHao", temp[5]);
                    Common.Log.Write("temp[2]:" + temp[2]);
                    ht.Add("DKBH", sPlotNo); // temp[2]
                    Common.Log.Write("temp[3]:" + temp[3]);
                    ht.Add("DKMC", temp[3]);
                    Common.Log.Write("temp[6]:" + temp[6]);
                    ht.Add("DKYT", temp[6]);
                    Common.Log.Write("temp[0]:" + temp[0]);
                    ht.Add("JZDS", temp[0]);
                    Common.Log.Write("temp[1]:" + temp[1]);
                    ht.Add("DKMJ", temp[1]);
                    Common.Log.Write("temp[4]:" + temp[4]);
                    ht.Add("TXLX", temp[4]);
                    Common.Log.Write("RiQi:" + System.DateTime.Now.ToString("yyyy-MM-dd"));
                    ht.Add("RiQi", System.DateTime.Now.ToString("yyyy-MM-dd"));
                    try
                    {
                        iDb.AddData("DKZBXX", ht);
                    }
                    catch (Exception ex)
                    {
                        Common.Log.Write("ex:" + ex);
                        sMsg = ex.ToString();
                        return false;
                    }


                    string[] tempJZDH = JZDHList[ii].ToString().Split(',');
                    Common.Log.Write("1111111");
                    string[] tempDKQH = DKQHList[ii].ToString().Split(',');
                    Common.Log.Write("222222");
                    string[] tempDOT = pointList[ii].ToString().Split(' ');
                    Common.Log.Write("33333333");
                    int iNum = tempDKQH.Length;
                    Common.Log.Write("iNum:" + iNum);
                    int iXuHao = 0;
                    for (int jj = 0; jj < iNum; jj++)
                    {
                        string strDKQH = tempDKQH[jj];
                        if (strDKQH.Contains("@"))
                        {
                            iXuHao = 1;
                            strDKQH = strDKQH.TrimStart('@');
                        }
                        else
                        {
                            iXuHao++;
                        }

                        htzb.Clear();
                        htzb.Add("CASENO", strCaseNo);
                        htzb.Add("BJITEMID", iDb.SetID("DKZB_ZB", "BJITEMID"));
                        htzb.Add("XuHao", iXuHao.ToString());
                        htzb.Add("JZDH", tempJZDH[jj]);
                        htzb.Add("ZZBX", tempDOT[jj].ToString().Split(',')[0]);
                        htzb.Add("HZBY", tempDOT[jj].ToString().Split(',')[1]);
                        htzb.Add("DKQH", strDKQH);
                        htzb.Add("DKBH", sPlotNo); // temp[2]

                        iDb.AddData("DKZB_ZB", htzb);
                    }
                }
            }
            else
            {
                bRev = false;
                sMsg = errorMsg;

            }

            return bRev;
        }

        #region 解析部坐标的文件
        //paramJZDH格式：J1,..J14,J1,J15..J18,J15,19,..J22,J19*J1,..J8,J1*J1,..J12,J1
        //paramDKQH格式：1,..1,1,@2,..2,2,@3,..,3,3*1..1*1..1
        //infoMsgParam格式：22,地块1..@8,地块2...@12,地块3...
        //paramDOT格式：X,Y X,Y...#X,Y XY...#
        private static void ResolveDeptFile(GS.DataBase.IDbAccess iDb,StreamReader sr, out string paramJZDH, out string paramDKQH, out string infoMsgParam, out string paramDOT, out string errorMsg, out string sAllPlotNo)
        {
            sAllPlotNo = "";
            errorMsg = string.Empty;
            paramJZDH = string.Empty;
            infoMsgParam = string.Empty;
            paramDKQH = string.Empty;
            paramDOT = string.Empty;

            //一、解析,按照地块取出来
            StringBuilder sPointBul = new StringBuilder();  //存点
            StringBuilder sDKQH = new StringBuilder();   //地块圈号
            StringBuilder sJZDH = new StringBuilder();      //界址点号

            //对于图符号、地类编码、地块名称等信息在这里不分开，一整块保存
            StringBuilder sInfoBul = new StringBuilder();

            string temp = sr.ReadLine();
            string[] pointlist;
            int iFlag = 0;

          
            string sPlotNo = ""; //地块编号，临时变量，按照  地块编号-地块名  保存
            try
            {
                while (!sr.EndOfStream)
                {
                    if (temp.EndsWith("@"))
                    {
                        iFlag++;
                        //信息,用其本身的结尾@区分
                        sInfoBul.Append(temp);
                        string[] checklist = temp.Split(',');

                        sPlotNo = checklist[2] + "-" + checklist[3]; //临时保存地块编号

                        if (checklist[2] == "")
                        {
                            errorMsg = "第" + iFlag.ToString() + "个地块的地块编号不存在，请检查坐标文件！";
                            return;
                        }
                        if (checklist[3] == "")
                        {
                            errorMsg = "第" + iFlag.ToString() + "个地块的地块名称不存在，请检查坐标文件！";
                            return;
                        }
                        if (Models.DKZBXX.IsPlotExists(iDb, sPlotNo))
                        {
                            errorMsg = "第" + iFlag.ToString() + "个地块的地块编号" + sPlotNo + "在数据库中已经存在，请检查坐标文件！";
                            return;
                        }
                        //保存地块编号到全局变量
                        sAllPlotNo += sPlotNo + ";";

                        temp = sr.ReadLine();
                        pointlist = temp.Split(',');

                        string aaa = "1";
                        while (temp != null && temp.Split(',').Length == 4)
                        {
                            pointlist = temp.Split(',');
                            //点
                            sPointBul.Append(pointlist[2]);
                            sPointBul.Append(",");
                            sPointBul.Append(pointlist[3]);
                            sPointBul.Append(" ");

                            //界址点
                            sJZDH.Append(pointlist[0]);
                            sJZDH.Append(",");

                            //地块圈号
                            if (aaa != pointlist[1])
                            {
                                sDKQH.Append("@");
                            }
                            aaa = pointlist[1];
                            sDKQH.Append(pointlist[1]);
                            sDKQH.Append(",");

                            temp = sr.ReadLine();
                        }
                        sPointBul.Remove(sPointBul.Length - 1, 1);
                        sPointBul.Append("#");
                        sJZDH.Remove(sJZDH.Length - 1, 1);
                        sJZDH.Append("*");
                        sDKQH.Remove(sDKQH.Length - 1, 1);
                        sDKQH.Append("*");
                    }
                    else
                    {
                        temp = sr.ReadLine();
                    }
                }

                sInfoBul.Remove(sInfoBul.Length - 1, 1);
                sPointBul.Remove(sPointBul.Length - 1, 1);
                sJZDH.Remove(sJZDH.Length - 1, 1);
                sDKQH.Remove(sDKQH.Length - 1, 1);

                paramJZDH = sJZDH.ToString();
                paramDKQH = sDKQH.ToString();
                paramDOT = sPointBul.ToString();
                infoMsgParam = sInfoBul.ToString();

                sAllPlotNo = sAllPlotNo.TrimEnd(';');

               
            }
            catch (Exception ex)
            {
                errorMsg = "解析坐标异常:请检查坐标文件内容!";
                Common.Log.Write("ex.Message:" + ex.Message);
                return;
            }
            finally
            {
                sr.Close();
            }
        }
        #endregion
        #endregion

        #region     获取X坐标和Y坐标

        /// <summary>
        /// 查找BJITEMID对应的坐标
        /// </summary>
        /// <param name="iDb"></param>
        /// <param name="BJITEMID">坐标的id,表DKZB_ZB中的字段</param>
        /// <param name="sPointX">返回的X坐标</param>
        /// <param name="sPointY">返回的Y坐标</param>
        /// <returns></returns>
        public static string GetXY(GS.DataBase.IDbAccess iDb, string BJITEMID, out string sPointX, out string sPointY)
        {
            sPointX = "";
            sPointY = "";
            string sMsg = "";
            string sSql = string.Format("select ZZBX,HZBY from DKZB_ZB  where BJITEMID ={0}", BJITEMID);
            DataSet ds = iDb.GetDataSet(sSql);
            if (ds != null)
            {
                sPointX = ds.Tables[0].Rows[0][0].ToString();
                sPointY = ds.Tables[0].Rows[0][1].ToString();
                sMsg = string.Format("查到BJITEMID={0}对应的X为:{1},Y为{2}", BJITEMID, sPointX, sPointY);
                Common.Log.Write(sMsg);
                return "";
            }
            else
            {
                sMsg = "没有找到BJITEMID为" + BJITEMID + "的坐标";
                return sMsg;
            }
        }

        #endregion

        #region     修改X坐标和Y坐标

        /// <summary>
        /// 查找BJITEMID对应的坐标
        /// </summary>
        /// <param name="iDb"></param>
        /// <param name="BJITEMID">坐标的id,表DKZB_ZB中的字段</param>
        /// <param name="sPointX">X坐标</param>
        /// <param name="sPointY">Y坐标</param>
        /// <returns></returns>
        public static bool SetXY(GS.DataBase.IDbAccess iDb, string BJITEMID,  string sPointX, string sPointY)
        {
            string sSql = string.Format("update DKZB_ZB set ZZBX={0},HZBY={1} where BJITEMID = {2};", sPointX, sPointY, BJITEMID);
            bool flag = iDb.ExecuteSql(sSql);
            return flag;
        }

        #endregion


        #region   按照地块编号删除地块和坐标
        public static string DeletePlot(GS.DataBase.IDbAccess iDb,string sDKBH)
        {
            string sMsg = "";
            if(String.IsNullOrEmpty(sDKBH))
            {
                sMsg = string.Format("地块编号为空，没有删除");
                return sMsg;
            }
            //首先删除表DKZBXX中数据
            string sSql = string.Format("delete from DKZBXX where DKBH='{0}'", sDKBH);
            bool bSuccess = iDb.ExecuteSql(sSql);
            if (bSuccess == true)
            {
                sMsg = "表DKZBXX中地块删除成功!";
            }
            else
            {
                sMsg = "表DKZBXX中地块删除失败!";
            }

            sSql = string.Format("delete from DKZB_ZB where DKBH='{0}'", sDKBH);
            bSuccess = iDb.ExecuteSql(sSql);
            if (bSuccess == true)
            {
                sMsg += "表DKZB_ZB中地块删除成功!";
            }
            else
            {
                sMsg += "表DKZB_ZB中地块删除失败!";
            }
            return sMsg;
        }
        #endregion

        #region  删除表DKZBXX和DKZB_ZB中所有的地块

        /// <summary>
        /// 删除表DKZBXX和DKZB_ZB中满足条件的所有地块
        /// </summary>
        /// <param name="iDb"></param>
        /// <param name="sFilter">筛选条件</param>
        /// <returns>删除结果</returns>
        public static string DeleteAllPlot(GS.DataBase.IDbAccess iDb,string sFilter)
        {
           
            string sMsg = "";
            //先将满足条件的地块查出来
            string sSql = string.Format("select distinct DKBH from DKZBXX where 1=1 {0}", sFilter);
            DataSet ds = iDb.GetDataSet(sSql);
            string sPlotNo = "";
            if (ds != null)
            {
                for (int i = 0, maxI = ds.Tables[0].Rows.Count; i < maxI; i++)
                {
                    sPlotNo = ds.Tables[0].Rows[i][0].ToString();
                    DeletePlot(iDb, sPlotNo);
                }
            }
            sMsg = "删除完成";
            return sMsg;
        }
        #endregion

        #region 获取所有案件编号已知的地块号
        /// <summary>
        /// 获取所有案件编号已知的地块号
        /// </summary>
        /// <param name="iDb"></param>
        /// <param name="sCaseNo">案件编号</param>
        /// <returns>地块号列表</returns>
        public static ArrayList GetPlotNo(GS.DataBase.IDbAccess iDb,string sCaseNo)
        {
            ArrayList list = new ArrayList();
            string sSql = string.Format("select distinct DKBH  from DKZBXX where CASENO='{0}'", sCaseNo);
            DataSet ds = iDb.GetDataSet(sSql);
            if (ds == null || ds.Tables[0].Rows.Count == 0)
            {
                return list;
            }
            string sPlotNo = "";
            for (int i = 0, maxI = ds.Tables[0].Rows.Count; i < maxI; i++)
            {
                sPlotNo = ds.Tables[0].Rows[i][0].ToString();
                list.Add(sPlotNo);
            }
            return list;
        }
        #endregion

        #region 获取地块已经关联的案卷号
        /// <summary>
        /// 获取地块已经关联的案卷号
        /// </summary>
        /// <param name="iDb"></param>
        /// <param name="sPlotNo">地块编号</param>
        /// <returns>案件编号</returns>
        public static string GetCaseNo(GS.DataBase.IDbAccess iDb, string sPlotNo)
        {
           
            string sSql = string.Format("select CASENO  from DKZBXX where DKBH='{0}'", sPlotNo);
            Common.Log.Write("查案件编号:"+sSql);
            DataSet ds = iDb.GetDataSet(sSql);
            if (ds == null)
            {
                return "";
            }
            else
            {
                return ds.Tables[0].Rows[0][0].ToString();
            }
        }
        #endregion


        #region 根据地块号设置案件号，即关联地块
        public static string SetCaseNo(GS.DataBase.IDbAccess iDb, string sCaseNo, ArrayList listSelected)
        {
            string sMsg = "";
            if (String.IsNullOrEmpty(sCaseNo))
            {
                return "案件编号";
            }

            if (listSelected == null || listSelected.Count == 0)
            {
                return "您选的地块号为空,这个案件现在没有关联的地块了";
            }
          
            for (int i = 0, maxI = listSelected.Count; i < maxI; i++)
            {   
                //先将以前关联的地块的caseno设为空
                string sSql = string.Format("update DKZBXX set CASENO='{0}' where DKBH='{1}'",sCaseNo,listSelected[i]);
                Common.Log.Write("关联地块的sql：" + sSql);
                iDb.ExecuteSql(sSql);
            }
            return sMsg;
        }
        #endregion

        #region 获取地块下面的坐标串


        /// <summary>
        /// 根据地块编号获取坐标，可能带环，带环的中间加@
        /// </summary>
        /// <param name="iDb"></param>
        /// <param name="sPlotNo">地块编号</param>
        /// <returns></returns>
        public static string GetPointForHuan(GS.DataBase.IDbAccess iDb, string sPlotNo)
        {
            string sSql = string.Format("select distinct DKQH from DKZB_ZB where DKBH ='{0}'",sPlotNo); //查询是否是多环
            Common.Log.Write("GetPointForHuan的sql1" + sSql);
            DataSet dsHuan = iDb.GetDataSet(sSql); //环的数据集
            string sPoint = ""; //保存点
            //在一个环或地块中，如果第一个点不等于最后一个，就是不闭合，就在后面添加上最后一个点
            string sFirstPoint = ""; //保存第一个点
            string sCurrPoint = ""; //当前的点
            if (dsHuan != null)
            {
                //下面针对多个环进行遍历
                for(int i = 0,maxI =dsHuan.Tables[0].Rows.Count; i <maxI ; i++)
                {
                    sSql = string.Format("select ZZBX,HZBY from DKZB_ZB   where DKBH='{0}'  and DKQH='{1}'  order by XUHAO ASC ", sPlotNo, dsHuan.Tables[0].Rows[i][0]);
                    Common.Log.Write("GetPointForHuan的sql2" + sSql);
                    DataSet dsPoint = iDb.GetDataSet(sSql); //获取点
                    //下面针对一个环遍历
                    if(dsPoint != null)
                    {
                        sFirstPoint = string.Format("{0},{1}", dsPoint.Tables[0].Rows[0][0], dsPoint.Tables[0].Rows[0][1]); //保存第一个点
                        for(int j = 0,maxJ = dsPoint.Tables[0].Rows.Count; j < maxJ; j++)
                        {
                            sCurrPoint = string.Format("{0},{1}", dsPoint.Tables[0].Rows[j][0], dsPoint.Tables[0].Rows[j][1]);
                            sPoint += string.Format("{0} ", sCurrPoint); //后面一个空格
                        }
                        sPoint = sPoint.TrimEnd(' '); //一定要去掉最后一个空格，否则提示 Input string was not in a correct format
                        if(sFirstPoint != sCurrPoint)  //没有闭合就添加上第一个点
                        {
                            sPoint += string.Format(" {0}@", sFirstPoint); //最后一个点前加空格
                        }
                        else   //否则最后添加一个@，代表环
                        {
                            sPoint += string.Format("@");
                        }
                    }
                }
                sPoint = sPoint.TrimEnd('@'); //去掉最后一个@
                return sPoint;
            }
            else
            {
                Common.Log.Write("没有取出坐标");
                return "";
            }               
        }

        #endregion

        #region  根据caseno从表DKZBXX中取DKBH,然后再从表DKZB_ZB中取坐标
 
       
        /// <summary>
        /// 根据caseno从表DKZBXX中取DKBH,然后再从表DKZB_ZB中取坐标
        /// </summary>
        /// <param name="iDb"></param>
        /// <param name="sCaseNo"></param>
        /// <returns></returns>
        public static string GetPoint(GS.DataBase.IDbAccess iDb, string sCaseNo)
        {          
            string sSql = string.Format("select distinct DKBH from DKZBXX where CASENO='{0}'",sCaseNo);
            Common.Log.Write("GetPoint执行：" + sSql );
            DataSet ds = iDb.GetDataSet(sSql);
            string sPoint = "";
            string sPlotNo = "";
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0, maxI = ds.Tables[0].Rows.Count; i < maxI; i++)
                {
                    sPlotNo = ds.Tables[0].Rows[i][0].ToString();
                    if (!String.IsNullOrEmpty(sPlotNo))
                    {
                        sPoint += GetPointForHuan(iDb, sPlotNo) + "#"; //多地块以"#"分隔
                    }
                }
            }
            sPoint = sPoint.Trim('#');
            return sPoint;
        }
        
        #endregion

        #region 获取地块名

        /// <summary>
        /// 获取地块名称
        /// </summary>
        /// <param name="iDb"></param>
        /// <param name="sPlotNo"></param>
        /// <returns></returns>
        public static string GetPlotName(GS.DataBase.IDbAccess iDb, string sPlotNo)
        {

            string sSql = string.Format("select DKMC from DKZBXX  where DKBH='{0}'", sPlotNo);
            DataSet ds = iDb.GetDataSet(sSql);
            if (ds != null)
            {
                return ds.Tables[0].Rows[0][0].ToString();
            }
            else
            {
                return "";
            }
        }

        #endregion




      

        #region     直接入正式库
        /// <summary>
        /// 直接入正式库
        /// </summary>
        /// <param name="isDeptPoint">如果是部坐标为true;是txt坐标为false</param>
        /// <param name="sPlotNo">地块编号</param>
        /// <param name="sPlotName">地块名称</param>
        /// <param name="sSubject">专题</param>
        /// <param name="sYear">年度</param>
        /// <returns>返回入库信息</returns>
        public static string InputFormal(GS.DataBase.IDbAccess iDb, bool isDeptPoint, string sPlotNo, string sPlotName, string sSubject, string sYear)
        {
            Common.Log.Write("入正式库开始");
            bool flag = true;
            String sMsg = string.Format("isDeptPoint={0},sPlotNo={1},sPlotName={2},sSubject={3},sYear={4}", isDeptPoint, sPlotNo, sPlotName, sSubject, sYear);
            Common.Log.Write(sMsg);
            if (String.IsNullOrEmpty(sPlotNo))
            {
                sMsg = string.Format("Error:地块编号为空");
                Common.Log.Write(sMsg);
                return sMsg;
            }
            if (String.IsNullOrEmpty(sSubject))
            {
                sMsg = string.Format("Error:专题为空");
                Common.Log.Write(sMsg);
                return sMsg;
            }
            if (String.IsNullOrEmpty(sYear))
            {
                sMsg = string.Format("Error:年度为空");
                Common.Log.Write(sMsg);
                return sMsg;
            }
            string sPoint = "";
           
            sPoint = Models.DKZBXX.GetPointForHuan(iDb, sPlotNo);
           
            if (String.IsNullOrEmpty(sPoint))
            {
                sMsg = string.Format("Error:地块编号为{0}的坐标为空", sPlotNo);
                Common.Log.Write(sMsg);
                return sMsg;
            }

          

            List<string> listKey = new List<string>();
            List<string> listValue = new List<string>();

            //面积
            double nArea = WebGis.WebGisBase.CalculateArea(sPoint);     
            sMsg = string.Format("点串面积为{0}", nArea);
            listKey.Add("mpArea");
            listValue.Add(nArea.ToString());

            //周长
            double nPerimeter = WebGis.WebGisBase.CalculatePerimeter(sPoint);
            sMsg = string.Format("点串周长为{0}", nPerimeter);
            listKey.Add("mpPerimeter");
            listValue.Add(nPerimeter.ToString());

            listKey.Add("时间");
            listValue.Add(System.DateTime.Now.ToString("yyyy-MM-dd"));

            listKey.Add("地块编号");
            listValue.Add(sPlotNo);

            listKey.Add("地块名称");
            listValue.Add(sPlotName);

            string strSolutionName = "辖区";
            string strInputAtts = string.Format("subjectType={0}&year={1}&scale=G", sSubject, sYear);  // subjectType=DC&year=2009&scale=G
            sMsg = string.Format("AddFeatureNew参数:strInputAtts是{0}", strInputAtts);
            Common.Log.Write(sMsg);
            try
            {
                flag = WebGis.WebGisBase.AddFeatureNew(strSolutionName, strInputAtts, sPoint, listKey.ToArray(), listValue.ToArray());        
                sMsg = flag ? "入库成功" : "入库失败";
                return sMsg;
            }
            catch (Exception ex)
            {
                Common.Log.Write(ex.Message);
                return "";
            }
        }
        #endregion

        #region  分析过的专题和年度

        #region 保存一个地块下的分析专题和年度

        /// <summary>
        /// 将分析过的专题和年保存到表
        /// </summary>
        /// <param name="iDb"></param>
        /// <param name="sPlotNo">地块号</param>
        /// <param name="sSubjectYear">形式是DC:2011;2012,CS:2012</param>
        /// <returns></returns>
        public static bool SaveSubjectYear(GS.DataBase.IDbAccess iDb, string sPlotNo, string sSubjectYear)
        {
            bool bSuccess = true;
            string sSql = string.Format("update DKZBXX set AnalySubjectYear='{0}' where DKBH = '{1}'", sSubjectYear, sPlotNo);
            Common.Log.Write("更新分析专题的sql:" + sSql);
            bSuccess = iDb.ExecuteSql(sSql);
            return bSuccess;
        }

        #endregion

        #region 保存一个案卷下面所有的分析专题和年度
        public static void SaveCaseSubjectYear(GS.DataBase.IDbAccess iDb, string sCaseNo, string sSubjectYear)
        {
            ArrayList list = GetPlotNo(iDb, sCaseNo); //获取所有地块           
            for (int i = 0, maxI = list.Count; i < maxI; i++)
            {
                SaveSubjectYear(iDb, list[i].ToString(), sSubjectYear);
            }
        }
        #endregion

        #region 获取一个地块下的专题年度        
        
        /// <summary>
        /// 获取地块对应的分析专题
        /// </summary>
        /// <param name="iDb"></param>
        /// <param name="sPlotNo">地块号</param>
        /// <returns>表DKZBXX中的AnalySubjectYear值</returns>
        public static string  GetSubjectYear(GS.DataBase.IDbAccess iDb, string sPlotNo)
        {
            string sSubjectYear = "";          
            string sSql = string.Format("select AnalySubjectYear from DKZBXX where DKBH = '{0}'",sPlotNo);
            Common.Log.Write("获取分析专题的sql:" + sSql);
            sSubjectYear = iDb.GetFirstColumn(sSql).ToString();
            Common.Log.Write("获取到的分析专题信息是:" + sSubjectYear);
            return sSubjectYear;
        }
        #endregion

        #region 获取一个案卷下面的专题年度
        public static string GetCaseSubjectYear(GS.DataBase.IDbAccess iDb, string sCaseNo)
        {
            ArrayList list = GetPlotNo(iDb, sCaseNo);
            for (int i = 0, maxI = list.Count; i < maxI; i++)
            {
                string sSubjectYear = GetSubjectYear(iDb, list[i].ToString());
                if (sSubjectYear != "")
                {
                    return sSubjectYear;
                }
            }
            return "";
        }
        #endregion

        #endregion


    

    }
}