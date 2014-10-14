using System;
using System.Collections.Generic;
//using System.Linq;
using System.Web;
using System.IO;
using System.Web.UI.WebControls;
using System.Collections;
using System.Text;
using CM.LC;
using System.Data;
using System.Data.OracleClient;
using System.Data.Common;
using WebGis;

namespace InputTextDotString
{
    public class InputText
    {
        #region 静态变量：服务地址，外部可以直接使用
        /// <summary>
        /// 在Web.config中配置的图形服务地址，如 http://192.168.27.119/IGSLandService
        /// </summary>
        public static string UrlService = System.Configuration.ConfigurationManager.AppSettings["IGSLandService"];
        /// <summary>
        /// 分析服务地址
        /// </summary>
        public static string UrlAnaly = UrlService + "/Analy.asmx"; //Analy.asmx地址
        /// <summary>
        /// 特性服务地址
        /// </summary>
        public static string UrlFeature = UrlService + "/Feature.asmx";//Feature.asmx地址
        /// <summary>
        /// 基础数据服务地址
        /// </summary>
        public static string UrlBaseDataInfo = UrlService + "/BaseDataInfo.asmx";//BaseDataInfo.asmx地址
        /// <summary>
        /// 临时基础数据服务地址(和UrlBaseDataInfo区别是：这个服务有获取比例尺的方法)
        /// </summary>
        public static string UrlTempBaseDataInfo = UrlService + "/TempBaseDataInfo.asmx";
        /// <summary>
        /// 临时图层名，入库时使用，如D430400JS2011GJSYD
        /// </summary>
        public static string ShareLayer = System.Configuration.ConfigurationManager.AppSettings["ShareLayer"];

        /// <summary>
        /// 查看图形的url，如http://192.168.27.119/webgis
        /// </summary>
        public static string UrlMap = System.Configuration.ConfigurationManager.AppSettings["AnalyUrl"];

        #endregion



        #region 读取文本内容
        /// <summary>
        /// 读取文本内容方法
        /// </summary>
        /// <param name="fu">要读取文件的路径</param>
        /// <returns></returns>
        //public BiaoZhunDotString InputDotText(/*StreamReader sr, string strString, */FileUpload fu/*, System.Text.Encoding txtEncoding*/)
        public BiaoZhunDotString InputDotText(FileUpload fu)
        {
            BiaoZhunDotString objDotString = new BiaoZhunDotString();
            #region 尝试解析坐标串文件
            try
            {
                //System.Text.Encoding te = System.Text.Encoding.GetEncoding(fu.PostedFile.FileName);                        //设置编码类型
                //te = System.Text.Encoding.UTF8;  
                //StreamReader sr = new StreamReader(fu.PostedFile.InputStream, te);
                System.Text.Encoding te = System.Text.Encoding.Unicode;
                StreamReader sr = new StreamReader(fu.PostedFile.FileName, System.Text.Encoding.GetEncoding(936), true);
                String line = "";
                String strDotString = "";
                String txtEncoding = te.ToString();
                int j = 1;
                int flag = 0;
                int index = 0;
                while ((line = sr.ReadLine()) != null)
                {
                    int n = 0;//当前行中第几个字符
                    int i = 0;//当前匹配到的分隔符出线的次数
                    if (line.Length != 0)
                    {
                        //WebUse.Logs.WriteLog(objDotString.sPath,"11111判断本行是否为空，若不为空，则继续下一个判断。");
                        string txtBJ = line.Substring(0, 1);
                        if (txtBJ == "J")
                        {
                            //WebUse.Logs.WriteLog(objDotString.sPath,"22222判断本行是否为坐标，若是则继续下一个判断。");
                            #region 根据开头J判断是坐标文件，截取其中有用部分组成字符串
                            while(n<line.Length && i<2)
                            {
                                if (line.Substring(n++, 1) == "," )//txtSplit等于分隔符时，跳出循环，strDotString赋值
                                {
                                    //WebUse.Logs.WriteLog(objDotString.sPath,"33333截取坐标串部分。");
                                    i=i+1;
                                    if (i == 1)
                                    {
                                        //WebUse.Logs.WriteLog(objDotString.sPath, "44444当读取到第二个分隔符时，记录。");
                                        if (line.Substring(0, n-1) == "J1")
                                        {
                                            //WebUse.Logs.WriteLog(objDotString.sPath, "55555当读取到第二个分隔符时，记录。");
                                            flag++;
                                        }
                                    }
                                    else if (i == 2)
                                    {
                                        //WebUse.Logs.WriteLog(objDotString.sPath,"66666当读取到第二个分隔符时，记录。");
                                        if (flag == 1)
                                        {
                                            //WebUse.Logs.WriteLog(objDotString.sPath,"77777判断这是第几个坐标点。");
                                            index++;
                                        }
                                    }
                                }
                                //strDotString = strDotString + line.Remove(0, n) + " ";
                                //string txtSplit = line.Substring(n, 1);//txtSplit等于第N个字符，从第一(0)位开始
                                //if (txtSplit == "," && i++==2)//txtSplit等于分隔符时，跳出循环，strDotString赋值
                                //{
                                //    strDotString = strDotString + line.Remove(0, n) + " ";
                                //}
                            }
                            line = line.Remove(0, n);
                            /*
                            if (strDotString.Contains(line.Remove(0, n)) == false)
                            {
                                //strDotString = strDotString + j++ + "," + line.ToString() + ",";
                                strDotString = strDotString + line.ToString() + " ";
                            }
                             * */
                            strDotString = strDotString + line.ToString() + " ";
                            #endregion
                        }
                    }
                }
                //strDotString = (flag/2)+","+index +","+ strDotString + "\nFlag=" + flag;
                //strDotString = (flag / 2) + "," + index + "," + strDotString;
                objDotString.strDots = strDotString;
                objDotString.txtEncoding = txtEncoding;
                return objDotString;
            }
            #endregion
            catch (Exception oExcept)
            {
                WebUse.Logs.WriteLog(objDotString.sPath, oExcept.Message);
                objDotString.strString = oExcept.Message;
                //objDotString.strString =fu.PostedFile.FileName;
                return objDotString;
            }
        }
        #endregion
        public class BiaoZhunDotString
        {
            public String strString = "";
            public String txtEncoding = "";
            public String strDots = "";
            public String strCaseNo = "";
            public String sPath = System.Web.HttpContext.Current.Server.MapPath("~/TempFile") + "/Plot" + (System.DateTime.Today.ToString("yyyy-MM-dd")).Replace("-", "") + ".txt";
            
        }

        #region 解析部坐标的文件
        //paramJZDH格式：J1,..J14,J1,J15..J18,J15,19,..J22,J19*J1,..J8,J1*J1,..J12,J1
        //paramDKQH格式：1,..1,1,@2,..2,2,@3,..,3,3*1..1*1..1
        //infoMsgParam格式：22,地块1..@8,地块2...@12,地块3...
        //paramDOT格式：X,Y X,Y...#X,Y XY...#
        //private static void ResolveDeptFile(StreamReader sr, out string paramJZDH, out string paramDKQH, out string infoMsgParam, out string paramDOT, out string errorMsg, out string sAllPlotNo,BiaoZhunDotString BZZB)
          //public static void resolveDeptFile(FileUpload fu,BiaoZhunDotString BZZB,string errorMsg)    
        public static void resolveDeptFile(StreamReader sr, BiaoZhunDotString BZZB, string errorMsg)    
         {
            //sAllPlotNo = "";
            errorMsg = string.Empty;
            //paramJZDH = string.Empty;
            //infoMsgParam = string.Empty;
            //paramDKQH = string.Empty;
            //paramDOT = string.Empty;
            
            //StreamReader sr = new StreamReader(fu.PostedFile.FileName, System.Text.Encoding.GetEncoding(936), true);

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
                        #region  原本用来判断坐标地块是否已存入数据库中（根据地块编号）,现注释掉
                        /*
                        if (Models.DKZBXX.IsPlotExists(iDb, sPlotNo))
                        {
                            errorMsg = "第" + iFlag.ToString() + "个地块的地块编号" + sPlotNo + "在数据库中已经存在，请检查坐标文件！";
                            return;
                        }
                        */
                        #endregion
                        //保存地块编号到全局变量
                        //sAllPlotNo += sPlotNo + ";";

                        temp = sr.ReadLine();
                        pointlist = temp.Split(',');

                        string aaa = "1";
                        sPointBul.Append("#");
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

                //paramJZDH = sJZDH.ToString();
                //paramDKQH = sDKQH.ToString();
                //paramDOT = sPointBul.ToString();
                //infoMsgParam = sInfoBul.ToString();
                BZZB.strDots = sPointBul.ToString();
                //sAllPlotNo = sAllPlotNo.TrimEnd(';');
                //WebUse.Logs.WriteLog(BZZB.sPath, BZZB.strDots);

            }
            catch (Exception ex)
            {
                errorMsg = "解析坐标异常:请检查坐标文件内容!";
                //Common.Log.Write("ex.Message:" + ex.Message);
                WebUse.Logs.WriteLog(BZZB.sPath, ex.Message);
                return;
            }
            finally
            {
                sr.Close();
            }
        }

          //public static string resolveDeptFile(FileUpload fu, BiaoZhunDotString BZZB)
          public static string resolveDeptFile(StreamReader sr, BiaoZhunDotString BZZB)      
         {
              try
              {
                  StringBuilder sPointBul = new StringBuilder();  //存点
                  StringBuilder sInfoBul = new StringBuilder();//存点号，判断是否存在环
                  //StreamReader sr = new StreamReader(fu.PostedFile.FileName, System.Text.Encoding.GetEncoding(936), true);
                  string temp = sr.ReadLine();//将streamreader的数据保存到temp中进行读取
                  int iFlag = 0;//独立地块标记，独立地块之间用#号隔开
                  //string flag = string.Empty;//
                  int flag = 0;
                  while (!sr.EndOfStream)
                  {
                      //flag++;
                      temp = sr.ReadLine();
                      //MapgisEgov.AnalyInput.Common.Log.Write("遍历行数为为：" + flag++);
                      #region 读取地块
                      if (temp.EndsWith("@"))
                      {
                          iFlag++;
                          flag = System.Int32.Parse(temp.ToString().Split(',')[0]);
                          //if(iFlag>1)//判断是否为单地块，若不是，在前一个一个地块的最后加#隔开
                          //{
                              sPointBul.Append("#");
                          //}
                          temp = sr.ReadLine();
                          string[] pointlist = temp.Split(','); ;
                          sInfoBul.Append(pointlist[1]);
                              //while (temp != null && temp.Split(',').Length == 4)
                          if (temp != null && temp.Split(',').Length == 4)
                          {
                              int no = 0;
                              while (temp.Split(',').Length == 4)
                              {
                                  no++;
                                  pointlist = temp.Split(',');
                                  if(!(no<flag))
                                  {
                                      sPointBul.Append(pointlist[2]);
                                      sPointBul.Append(",");
                                      sPointBul.Append(pointlist[3]);
                                      sPointBul.Append(" ");
                                      break;
                                  }
                                      //点

                                      if (sInfoBul.ToString().Contains(pointlist[1]) == false)//判断是否为环
                                      {
                                          sPointBul.Remove(sPointBul.Length - 1, 1);
                                          sPointBul.Append("@");//环坐标用@隔开
                                          sInfoBul.Append(pointlist[1]);
                                      }
                                      sPointBul.Append(pointlist[2]);
                                      sPointBul.Append(",");
                                      sPointBul.Append(pointlist[3]);
                                      sPointBul.Append(" ");
                                      temp = sr.ReadLine();
                                      //no++;
                                  
                              }
                              
                          }
                              while (sPointBul.ToString().Substring(sPointBul.Length - 1, 1) == " ")//删除字符串最后的空格
                              {
                                  sPointBul.Remove(sPointBul.Length - 1, 1);
                              }
                      }
                      #endregion
                  }

                  while (sPointBul.ToString().Substring(sPointBul.Length - 1, 1) == " ")//删除字符串最后的空格
                  {
                      sPointBul.Remove(sPointBul.Length - 1, 1);
                  }
                  while (sPointBul.ToString().Substring(0, 1) == "#")//删除字符串开头的#
                  {
                      sPointBul.Remove(0, 1);
                  }
                  BZZB.strDots = sPointBul.ToString();
                  return sPointBul.ToString();
              }
              catch (Exception oExcept)
              {
                  return "部坐标解析过程中遇到问题:"+oExcept.Message;
              }
          }

          private static void ResolveDeptFile(GS.DataBase.IDbAccess iDb, StreamReader sr, out string paramJZDH, out string paramDKQH, out string infoMsgParam, out string paramDOT, out string errorMsg, out string sAllPlotNo)
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
              //MapgisEgov.AnalyInput.Common.Log.Write("代码执行5");
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
                              MapgisEgov.AnalyInput.Common.Log.Write(errorMsg);
                              return;
                          }
                          if (checklist[3] == "")
                          {
                              errorMsg = "第" + iFlag.ToString() + "个地块的地块名称不存在，请检查坐标文件！";
                              MapgisEgov.AnalyInput.Common.Log.Write(errorMsg);
                              return;
                          }
                          if (MapgisEgov.AnalyInput.Models.DKZBXX.IsPlotExists(iDb, sPlotNo))
                          {
                              errorMsg = "第" + iFlag.ToString() + "个地块的地块编号" + sPlotNo + "在数据库中已经存在，请检查坐标文件！";
                              MapgisEgov.AnalyInput.Common.Log.Write(errorMsg);
                              return;
                          }
                          MapgisEgov.AnalyInput.Common.Log.Write("代码执行5");
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
                  //MapgisEgov.AnalyInput.Common.Log.Write("代码执行6");
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
                  MapgisEgov.AnalyInput.Common.Log.Write("ex.Message:" + ex.Message);
                  return;
              }
              finally
              {
                  sr.Close();
              }
          }
        #endregion

          #region 解析简单坐标串格式文本
          /// <summary>
          /// 解析纯坐标文件,返回坐标字符串，如果不闭合，在最后添加第一个坐标点。
          /// </summary>
          public static string ResolveTxtFile(StreamReader sr, BiaoZhunDotString BZZB,string errorMsg)
          {
              string paramJZDH = string.Empty;
              string paramDKQH = string.Empty;
              errorMsg = string.Empty;
              StringBuilder sbuilder = new StringBuilder();
              StringBuilder JZDH = new StringBuilder();
              //StreamReader sr = new StreamReader(fu.PostedFile.FileName, System.Text.Encoding.GetEncoding(936), true);
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
              BZZB.strDots = pointBuilder.ToString();
              return pointBuilder.ToString();
          }

          //重载原方法
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
          #endregion


          /// <summary>
          /// 坐标文件解析，根据文本内容判断采用部标准格式坐标或是简单坐标对应方法解析
          /// </summary>
          /// <param name="fu">FileUpload上传的值</param>
          /// <param name="BZZB">包含坐标的BiaoZhunDotString对象</param>
          /// <param name="errorMsg">异常信息</param> 
          /// <returns></returns>
          //public static void resolveFile(FileUpload fu, BiaoZhunDotString BZZB, string errorMsg)
          public static void resolveFile(StreamReader sr, BiaoZhunDotString BZZB, string errorMsg)
          {
              try
              {
                  //StreamReader sr = new StreamReader(fu.PostedFile.FileName, System.Text.Encoding.GetEncoding(936), true);
                  string srRead = sr.ReadLine();
                  if (srRead.Substring(0, 1) == "[")
                  {
                      //InputText.resolveDeptFile(fu, BZZB, errorMsg);  //8月3日测试新方法
                      string strDots=InputText.resolveDeptFile(sr,BZZB);
                      MapgisEgov.AnalyInput.Common.Log.Write("调用部坐标解析方法成功！");
                  }
                  else
                  {
                      InputText.ResolveTxtFile(sr, BZZB, errorMsg);
                      MapgisEgov.AnalyInput.Common.Log.Write("调用简单坐标解析方法成功！");
                  }
              }
              catch (Exception oExcept)
              {
                  WebUse.Logs.WriteLog(BZZB.sPath, "解析错误，出错原因：" + oExcept.Message);
              }
          }

          public static string SaveDotString(string strDotString,string keyValue)
          {
              try
              {
                  DatabaseORC db = new DatabaseORC();
                  InputText.BiaoZhunDotString BZDS = new InputText.BiaoZhunDotString();
                  //StringBuilder sbSql = new StringBuilder("update CM_LC_CKQ set 矿区坐标=" + TextArea1.InnerText + "where CASENO='" + Request.QueryString["CASENO"] + "'");
                  string sbSql = "update CM_LC_CKQ set 矿区坐标='" + strDotString + "' where CASENO='" + keyValue + "'";
                  WebUse.Logs.WriteLog(BZDS.sPath, sbSql);
                  db.ExecuteSql(sbSql.ToString());
                  string sMsg="保存成功！";
                  WebUse.Logs.WriteLog(BZDS.sPath,sMsg+sbSql);
                  return sMsg;
              }
              catch (Exception oExcept)
              {
                  return oExcept.Message;
              }
          }

          public static string SaveDotString(string Sql)
          {
              try
              {
                  DatabaseORC db = new DatabaseORC();
                  db.ExecuteSql(Sql);
                  string sMsg = "保存成功！";
                  return sMsg;
              }
              catch (Exception oExcept)
              {
                  return oExcept.Message;
              }
          }


          /// <summary>
          /// 采矿专题数据入库方法,首先根据关键字判断要素是否存在，若存在则更新，若不存在则插入
          /// </summary>
          /// <param name="strSolutionName">命名规则(必选),例如"两矿","辖区","规划"等</param>
          /// <param name="strInputAtt"> <![CDATA[图层信息(例如: subjectType=DC&year=2009&scale=G)(必选)]]></param>
          /// <param name="strDotString">坐标点，例如：x1,y1 x2,y2 x3,y3 x1,y1@x1,y1 x2,y2 x3,y3 x1,y1#x1,y1 x2,y2 x3,y3 x1,y1(必选)</param>
          /// <param name="attField">字段名数组(必选)</param>
          /// <param name="attValue">字段值数组(必选)</param>
          /// <param name="keyField">strKeyWord：关键字段名(必选),例如"CK_GUID","项目档案号"等</param>
          /// <param name="keyValue">strKeyWord：关键字段值(必选)</param>  
          /// <returns></returns>
          public static string InputDotStringCK(string strSolutionName, string strInputAtt, string strDotString, string[] attField, string[] attValue,string keyField, string keyValue)
          {
              try
              {
                  StringBuilder sbMsg = new StringBuilder();
                  //string strWhere=keyField+"="+keyValue;
                  DatabaseORC db=new DatabaseORC();
                  DataSet ds=db.GetDataSet("select 项目档案号 from 采矿申请登记 where "+keyField+"='"+keyValue+"'");
                  string strWhere = "项目档案号='" + ds.Tables[0].Rows[0][0].ToString()+"'";
                  MapgisEgov.AnalyInput.Common.Log.Write("==============================更新关键字==============================");
                  MapgisEgov.AnalyInput.Common.Log.Write(strWhere);
                  MapgisEgov.AnalyInput.Common.Log.Write("==============================更新关键字==============================");
                  bool bExist = WebGisBase.IsFeatureExistNew(strSolutionName, strInputAtt, strWhere);
                   
                  int j=attField.Length;
                  if (bExist)//根据关键字判断当前要素是否已存在，true表示已存在时采用Update方法
                  {
                      MapgisEgov.AnalyInput.Common.Log.Write("根据关键字判断当前要素已存在，采用Update方法");
                      sbMsg.Append("================要素已存在，采用Update方法================");
                      long IFeatureID = 0;
                      //bool bSuccess = WebGisBase.UpdateFeatureNew(strSolutionName, strInputAtt, strDotString, attField, attValue, strWhere);
                     
                          bool bSuccessDel = WebGisBase.DelFeatureNew(strSolutionName, strInputAtt, strWhere);
                          Log.Write("删除结果：" + bSuccessDel);
                          bool bSuccessAdd = WebGisBase.AddFeatureNew(strSolutionName, strInputAtt, strDotString, attField, attValue);

                      Log.Write("入库结果：" + bSuccessDel);
                      if (bSuccessDel&&bSuccessAdd)
                      {
                          sbMsg = new StringBuilder("更新成功");
                      }
                      else
                      {
                          sbMsg = new StringBuilder("更新失败");
                      }
                  }
                  else//要素不存在，采用Add方法
                  {
                      MapgisEgov.AnalyInput.Common.Log.Write("根据关键字判断当前要素不存在，采用New方法");
                      /*
                      for (int i = 0; i < attField.Length; i++)
                      {
                          MapgisEgov.AnalyInput.Common.Log.Write(attField[i]+"\n");
                      }
                      for (int i = 0; i < attValue.Length; i++)
                      {
                          MapgisEgov.AnalyInput.Common.Log.Write(attValue[i]+"\n");
                      }
                       * */
                          sbMsg.Append("==================要素不存在，采用Add方法==================\n");
                      bool bSuccess = WebGisBase.AddFeatureNew(strSolutionName, strInputAtt, strDotString, attField, attValue);
                      if(bSuccess)
                      {
                          sbMsg=new StringBuilder("插入成功");
                      }
                      else
                      {
                          sbMsg = new StringBuilder("插入失败");
                      }
                  }
                  return sbMsg.ToString();
              }
              catch(Exception oExcept)
              {
                  return oExcept.Message;
              }
          }


          #region 重载采矿方法
        /*
          public static string InputDotStringTK(string strSolutionName, string strInputAtt, string strDotString, string[] attField, string[] attValue, string keyField, string keyValue)
          {
              try
              {
                  StringBuilder sbMsg = new StringBuilder();
                  //string strWhere=keyField+"="+keyValue;
                  DatabaseORC db = new DatabaseORC();
                  DataSet ds = db.GetDataSet("select KCZH from lc_kcxkz where " + keyField + "='" + keyValue + "'");
                  string strWhere = "许可证号='" + ds.Tables[0].Rows[0][0].ToString() + "'";
                  MapgisEgov.AnalyInput.Common.Log.Write("==============================更新关键字==============================");
                  MapgisEgov.AnalyInput.Common.Log.Write(strWhere);
                  MapgisEgov.AnalyInput.Common.Log.Write("==============================更新关键字==============================");
                  bool bExist = WebGisBase.IsFeatureExistNew(strSolutionName, strInputAtt, strWhere);

                  int j = attField.Length;
                  if (bExist)//根据关键字判断当前要素是否已存在，true表示已存在时采用Update方法
                  {
                      MapgisEgov.AnalyInput.Common.Log.Write("根据关键字判断当前要素已存在，采用Update方法");
                      sbMsg.Append("================要素已存在，采用Update方法================");
                      long IFeatureID = 0;
                      //bool bSuccess = WebGisBase.UpdateFeatureNew(strSolutionName, strInputAtt, strDotString, attField, attValue, strWhere);

                      bool bSuccessDel = WebGisBase.DelFeatureNew(strSolutionName, strInputAtt, strWhere);
                      Log.Write("删除结果：" + bSuccessDel);
                      bool bSuccessAdd = WebGisBase.AddFeatureNew(strSolutionName, strInputAtt, strDotString, attField, attValue);

                      Log.Write("入库结果：" + bSuccessDel);
                      if (bSuccessDel && bSuccessAdd)
                      {
                          sbMsg = new StringBuilder("更新成功");
                      }
                      else
                      {
                          sbMsg = new StringBuilder("更新失败");
                      }
                  }
                  else//要素不存在，采用Add方法
                  {
                      MapgisEgov.AnalyInput.Common.Log.Write("根据关键字判断当前要素不存在，采用New方法");
                      sbMsg.Append("==================要素不存在，采用Add方法==================\n");
                      bool bSuccess = WebGisBase.AddFeatureNew(strSolutionName, strInputAtt, strDotString, attField, attValue);
                      if (bSuccess)
                      {
                          sbMsg = new StringBuilder("插入成功");
                      }
                      else
                      {
                          sbMsg = new StringBuilder("插入失败");
                      }
                  }
                  return sbMsg.ToString();
              }
              catch (Exception oExcept)
              {
                  return oExcept.Message;
              }
          }
        */
          #endregion

          /// <summary>
          /// 采探矿数据库坐标串解析方法
          /// </summary>
          /// <param name="strSolutionName">命名规则(必选),例如"两矿","辖区","规划"等</param>
          /// <param name="strInputAtt"> <![CDATA[图层信息(例如: subjectType=DC&year=2009&scale=G)(必选)]]></param>
          /// <param name="strDotString">坐标点，例如：x1,y1 x2,y2 x3,y3 x1,y1@x1,y1 x2,y2 x3,y3 x1,y1#x1,y1 x2,y2 x3,y3 x1,y1(必选)</param>
          /// <param name="attField">字段名数组(必选)</param>
          /// <param name="attValue">字段值数组(必选)</param>
          /// <param name="keyField">strKeyWord：关键字段名(必选),例如"CK_GUID","项目档案号"等</param>
          /// <param name="keyValue">strKeyWord：关键字段值(必选)</param>  
          /// <returns></returns>
          public static string AnalyDotStringCTK(string Subject,string KeyValue)
          {
              try
              {
                  DatabaseORC db = new DatabaseORC();
                  StringBuilder Sql = new StringBuilder();
                  string strDotString = string.Empty;
                  #region 采矿
                  if (Subject=="CK")
                  {
                      Sql.Append("select 区域坐标 from 采矿申请登记 where ck_guid='" + KeyValue + "'");
                      //MapgisEgov.AnalyInput.Common.Log.Write("1111111111122222222222"+db.GetDataSet(Sql.ToString()).Tables[0].Rows[0][0].ToString()+"==="+Sql.ToString());
                      DataSet ds = db.GetDataSet(Sql.ToString());
                      //MapgisEgov.AnalyInput.Common.Log.Write(ds.Tables[0].Rows[0][0].ToString());
                      if (ds.Tables[0].Rows[0][0].ToString() != string.Empty)
                      {
                          strDotString = ds.Tables[0].Rows[0][0].ToString();
                          strDotString = GetCKFormattedDotString(strDotString);
                          //MapgisEgov.AnalyInput.Common.Log.Write(strDotString);
                          return strDotString;
                      }
                      else
                      {
                          //MapgisEgov.AnalyInput.Common.Log.Write("ds.Tables[0].Rows[0][0].ToString() == string.Empty");
                          return string.Empty;
                          
                      }
                  }
                  #endregion

                  #region 探矿
                  else if (Subject=="TK")
                  {
                      //Sql.Append("select 区域坐标 from 勘查项目登记 where 申请序号='" + KeyValue + "'");
                      Sql.Append("select KCQYGDZB from LC_KCXKZ where CASENO='" + KeyValue + "'");
                      DataSet ds = db.GetDataSet(Sql.ToString());
                      strDotString = ds.Tables[0].Rows[0][0].ToString();
                      strDotString = GetTKFormattedDotString(strDotString);
                      StringBuilder sb = new StringBuilder();
                      string[] sPlot = strDotString.Split('#');
                      for (int i = 0; i < sPlot.Length; i++)
                      {
                          string[] sHuan = sPlot[i].Split('@');
                          for (int j = 0; j < sHuan.Length; j++)
                          {
                              string[] sDot = sHuan[j].Split(' ');
                              for (int n = 0; n < sDot.Length; n++)
                              {
                                  string[] xy = sDot[n].Split(',');
                                  //xy[0] = InputText.TranDegreeToDMs(Double.Parse(xy[0]));
                                  //xy[1] = InputText.TranDegreeToDMs(Double.Parse(xy[1]));

                                  xy[0] = InputText.DMsToTranDegree(Double.Parse(xy[0])).ToString();
                                  xy[1] = InputText.DMsToTranDegree(Double.Parse(xy[1])).ToString();
                                  double jd = double.Parse(xy[0]);
                                  double wd = double.Parse(xy[1]);
                                  xy = InputText.GeoToGauss(jd, wd).Split(',');
                                  sb.Append(xy[1]+","+xy[0]+" ");//根据入库坐标格式调换xy
                                  //MapgisEgov.AnalyInput.Common.Log.Write(sb.ToString());
                              }
                              if (sb.ToString().EndsWith(" "))
                              {
                                  sb.Remove(sb.Length - 1, 1);
                              }
                              sb.Append("@");
                          }
                          if (sb.ToString().EndsWith("@"))
                          {
                              sb.Remove(sb.Length - 1, 1);
                          }
                          sb.Append("#");
                      }
                      //strDotString = sb.ToString();
                      if (sb.ToString().EndsWith("#"))
                      {
                          sb.Remove(sb.Length - 1, 1);
                      }
                      strDotString = sb.ToString();
                          return strDotString;
                  }
                  #endregion

                  return strDotString;

                  //string strDotString = GetCKFormattedDotString("3,5,1,3810331.42,38446311.66,2,3810329.42,38446592.66,3,3810835.42,38449863.66,4,3809233.42,38449855.66,5,3809252.42,38446305.66,285,8,,1,5,1,3810051.42,38449707.66,2,3809763.42,38449679.66,3,3809738.42,38449770.66,4,3809823.42,38449837.66,5,3810016.42,38449879.66,192,185,扣除新密市栗树岗建材厂范围,1,4,1,3809471.42,38446617.66,2,3809411.42,38446667.66,3,3809389.42,38446633.66,4,3809451.42,38446583.66,230,200,扣除新密市超化镇楚岭鑫东石料厂范围,-1,");
                  
                  /*
                  DataSet ds = db.GetDataSet(Sql.ToString());
                  DataTable dtDotString = new DataTable();
                  string[] returnValue = ds.Tables[0].Rows[0][0].ToString().Split(',');
                  for (int i = 0; i + 5 < returnValue.Length;i++ )//采矿权坐标串后五位是标高等信息，剔除
                  {
                      if (i + 6 < returnValue.Length)
                      {
                          strDotString.Append(returnValue[i]+",");
                          dtDotString.Rows.Add(returnValue[i]);
                      }
                      else
                      {
                          strDotString.Append(returnValue[i]);
                          dtDotString.Rows.Add(returnValue[i]);
                      }
                  }
                  string[] DotString = strDotString.ToString().Split(',');
                  int areaNum = Int32.Parse(DotString[0]); 
                  for (int j = 1; j < areaNum;j++ )
                  {
                      int begin = 3;
                      int end = Int32.Parse(DotString[2])*3;
                  }
                  */
                  //return strDotString.ToString();
              }
              catch(Exception oExcept)
              {
                  return oExcept.Message;
              }
          }

          public static string[] CKSQDJ(string FieldName, string keyValue, string sMsg)
          {
              try
              {
                  int flag = 0;
                  int i = 0;
                  StringBuilder strFieldName = new StringBuilder(FieldName);
                  InputText.BiaoZhunDotString ib = new BiaoZhunDotString();
                  List<string> ls=new List<string>();
                  //string[] arrStr = strFieldName.ToString().Split('|');
                  string[] arrStr = strFieldName.ToString().Split('|');
                  //WebUse.Logs.WriteLog(ib.sPath,"CKSQDJ");
                  if (FieldName.Length != 0)
                  {
                      //WebUse.Logs.WriteLog(ib.sPath, "if (FieldName.Length != 0);");
                      string[] attField = FieldName.Split(',');
                      for (int j = 0; j < attField.Length; j++)
                      {
                          string str = getCKQData(attField[j], keyValue, "string str = getCKQData报错！");
                              ls.Add(str);
                              arrStr = ls.ToArray();
                      }
                      /*
                      for (flag = 0; flag < FieldName.Length; flag++)
                      {
                          if (FieldName.Substring(flag, 1) == ",")//每个字段名后面加逗号隔开，当查询到逗号的时候取字段名
                          {
                              string str = getCKQData(FieldName.Substring(i, flag-i), keyValue, "string str = getCKQData报错！");
                              WebUse.Logs.WriteLog(ib.sPath, "getCKQData" + "=" + FieldName.Substring(i, flag-i)+"\nFieldName.Substring=" + str);
                              ls.Add(str);
                              //strFieldName = strFieldName.Append(str);
                              i = 1+flag++;
                              arrStr = ls.ToArray();
                          }
                      }
                       * */
                  }
                  return arrStr;
              }
              catch (Exception oExcept)
              {
                  InputText.BiaoZhunDotString ib = new BiaoZhunDotString();
                  WebUse.Logs.WriteLog(ib.sPath, "取数据错误" + oExcept.Message);
                  string[] arrExcept = new string[] { oExcept.Message };
                  return arrExcept;
              }
          }

          public static string getCKQData(string FieldName,string keyValue,string sMsg)
          {
              try
              {
                  //MapgisEgov.AnalyInput.Common.Log.Write(FieldName+keyValue);
                  string strSql = "select " + FieldName + " from 采矿申请登记 where CK_GUID='" + keyValue + "'";
                  BiaoZhunDotString ib=new BiaoZhunDotString();
                  //WebUse.Logs.WriteLog(ib.sPath, "getCKQData执行"+strSql);
                  bool bl = FieldName != string.Empty && keyValue != string.Empty;
                  if (bl==true)
                  {
                      //WebUse.Logs.WriteLog(ib.sPath, "FieldName="+FieldName);
                      DatabaseORC db = new DatabaseORC();
                      DataSet ds = db.GetDataSet(strSql);
                      StringBuilder strData = new StringBuilder();
                      strData.Append(ds.Tables[0].Rows[0][0].ToString());
                      return strData.ToString();

                  }
                  else
                  {
                      sMsg = "请检查传入页面参数是否正确！" + "字段名" + FieldName + "关键字" + keyValue;
                      WebUse.Logs.WriteLog(ib.sPath, "getCKQData出错，"+sMsg);
                      return sMsg;
                  }
              }
              catch (Exception oExcept)
              {
                  //WebUse.Logs.WriteLog(objDotString.sPath, oExcept.Message);
                  MapgisEgov.AnalyInput.Common.Log.Write(oExcept.Message);
                  //objDotString.strString = oExcept.Message;
                  //objDotString.strString =fu.PostedFile.FileName;
                  return oExcept.Message;
              }
          }

          #region 从探矿"lc_kcxkz"表中取到所需字段的值，用逗号隔开返回
          public static string getTKQDataFromKCXKZ(string FieldName, string keyField,string keyValue, string sMsg)
          {
              try
              {
                  string strSql = "select " + FieldName + " from lc_kcxkz where "+keyField+"='" +keyValue  + "'";
                  bool bl = FieldName != string.Empty && keyField != string.Empty;
                  if (bl == true)
                  {
                      DatabaseORC db = new DatabaseORC();
                      DataSet ds = db.GetDataSet(strSql);
                      StringBuilder strData = new StringBuilder();
                      //strData.Append(ds.Tables[0].Rows[0][0].ToString());
                      for (int i = 0; i < ds.Tables[0].Columns.Count; i++)
                      {
                          strData.Append(ds.Tables[0].Rows[0][i].ToString()+",");
                      }
                      if(strData.ToString().EndsWith(","))
                      {
                          strData.Remove(strData.Length - 1, 1);
                      }
                      return strData.ToString();
                  }
                  else
                  {
                      sMsg = "请检查传入页面参数是否正确！" + "字段名" + FieldName + "关键字" + keyField;
                      MapgisEgov.AnalyInput.Common.Log.Write("getCKQData出错，" + sMsg);
                      return sMsg;
                  }
              }
              catch (Exception oExcept)
              {
                  //WebUse.Logs.WriteLog(objDotString.sPath, oExcept.Message);
                  MapgisEgov.AnalyInput.Common.Log.Write("=============================getTKQDataFromKCXKZ错误=============================");
                  MapgisEgov.AnalyInput.Common.Log.Write(oExcept.Message);
                  MapgisEgov.AnalyInput.Common.Log.Write("=============================getTKQDataFromKCXKZ错误=============================");
                  //objDotString.strString = oExcept.Message;
                  //objDotString.strString =fu.PostedFile.FileName;
                  return oExcept.Message;
              }
          }
          #endregion

          public static string InputDotStringTK(string strSolutionName, string strInputAtt, string strDotString, string[] attField, string[] attValue, string keyField, string keyValue)
          {
              try
              {
                  string sMsg = string.Empty;
                  //string sWhere=keyField+"='"+keyValue+"'";
                  DatabaseORC db = new DatabaseORC();
                  string Sql = "select KCZH from lc_kcxkz where CASENO='" + keyValue + "'";
                  keyValue = db.GetDataSet(Sql).Tables[0].Rows[0][0].ToString();
                  string sWhere = "许可证号='" + keyValue + "'";
                  //MapgisEgov.AnalyInput.Common.Log.Write(strSolutionName + strInputAtt + sWhere);
                  //MapgisEgov.AnalyInput.Common.Log.Write("------bExist开始执行！------");
                  //bool bExist=WebGisBase.IsFeatureExistNew(strSolutionName,strInputAtt,sWhere);
                  Feature.Feature f = new Feature.Feature();
                  bool bExist = f.IsFeatureExistNew(strSolutionName, strInputAtt, sWhere);
                  //MapgisEgov.AnalyInput.Common.Log.Write("------bExist执行成功！------");
                  if (bExist == true)//要素已存在，update方法有问题，先删除要素再新增
                  {
                      //MapgisEgov.AnalyInput.Common.Log.Write("------DelFeatureNew开始执行！------");
                      f.DelFeatureNew(strSolutionName, strInputAtt,0, sWhere);
                      //WebGisBase.DelFeatureNew(strSolutionName, strInputAtt,sWhere);
                      //MapgisEgov.AnalyInput.Common.Log.Write("------DelFeatureNew执行成功！------");
                  }
                  //MapgisEgov.AnalyInput.Common.Log.Write("------AddFeatureNew开始执行！------");
                  bool bSuccess = f.AddFeatureNew(strSolutionName, strInputAtt, strDotString, attField, attValue);
                  //bool bSuccess = WebGisBase.AddFeatureNew(strSolutionName, strInputAtt, strDotString, attField, attValue);
                  //MapgisEgov.AnalyInput.Common.Log.Write("------AddFeatureNew执行成功！------");
                  if(bSuccess==true)
                  {
                      sMsg = "入库成功！";
                  }
                  else
                  {
                      sMsg = "入库失败！";
                  }
                  return sMsg;
              }
              catch(Exception oExcept)
              {
                  return oExcept.Message;
              }
          }

          #region 采探矿数据库坐标格式化
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
                      int iPointNo = int.Parse (strDots[iCurIndex]);
                      iCurIndex += 2;


                      for (int j = 0; j < iPointNo; j++)
                      {
                          strDotString += strDots[iCurIndex] + "," + strDots[iCurIndex + 1] + " ";
                          //strDotString += strDots[iCurIndex+1] + "," + strDots[iCurIndex] + " ";
                          iCurIndex += 3;
                      }

                      if (strDotString.EndsWith(" "))
                      { strDotString = strDotString.Substring(0, strDotString.Length - 1); }

                      if ((iCurIndex + 3+1) < strDots.Length)
                      {
                          int iPointTag = int.Parse(strDots[iCurIndex + 3]);
                          if ((iCurIndex + iPointTag * 3 + 7 + 1) < strDots.Length)
                          {
                              if (strDots[iCurIndex + iPointTag * 3 + 7] == "-1")
                              {
                                  strDotString += "@";
                              }
                              else
                              {
                                  strDotString += "#";
                              }
                          }
                      }
                      //strDotString += "@";
                      iCurIndex += 3;
                  }
                  catch(Exception oExcept)
                  { MapgisEgov.AnalyInput.Common.Log.Write("采矿权数据库坐标解析问题："+oExcept.Message); }
              }

              if (strDotString.EndsWith("@"))
              { strDotString = strDotString.Substring(0, strDotString.Length - 1); }
              else if (strDotString.EndsWith(" "))
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
                          strDotString += strDots[iCurIndex] + "," + strDots[iCurIndex + 1] + " ";
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

                      //strDotString += "@";
                      if ((iCurIndex + 3+1) < strDots.Length)
                      {
                          int iPointTag = int.Parse(strDots[iCurIndex + 3]);
                          if ((iCurIndex + iPointTag * 2 + 4+1) < strDots.Length)
                          {
                              if (strDots[iCurIndex + iPointTag * 2 + 4] == "-1")
                              {
                                  strDotString += "@";
                              }
                              else
                              {
                                  strDotString += "#";
                              }
                              iCurIndex += 3;
                          }
                      }
                  }
                  catch(Exception oExcecpt)
                  { MapgisEgov.AnalyInput.Common.Log.Write("TK数据解析报错："+oExcecpt.Message); }
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
          #endregion

          #region 解析简单坐标串并保存至数据库
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


          #region 保存部坐标
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
              //MapgisEgov.AnalyInput.Common.Log.Write("代码执行1");
              ResolveDeptFile(iDb, sr, out paramJZDH, out paramDKQH, out infoMsgParam, out point, out errorMsg, out sAllPlotNo);
              //MapgisEgov.AnalyInput.Common.Log.Write("代码执行2");

              if (!string.IsNullOrEmpty(errorMsg))
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

                  //MapgisEgov.AnalyInput.Common.Log.Write("iCount:" + iCount);
                  string sPlotNo = ""; //临时保存地块编号 保存形式是  地块号-地块名称   
                  for (int ii = 0; ii < iCount; ii++)
                  {
                      string[] temp = infoMegList[ii].ToString().Split(',');
                      sPlotNo = temp[2] + "-" + temp[3]; //临时保存地块编号

                      ht.Clear();
                      MapgisEgov.AnalyInput.Common.Log.Write("strCaseNo:" + strCaseNo);
                      ht.Add("CASENO", strCaseNo);
                      MapgisEgov.AnalyInput.Common.Log.Write("temp[5]:" + temp[5]);
                      ht.Add("TuFuHao", temp[5]);
                      MapgisEgov.AnalyInput.Common.Log.Write("temp[2]:" + temp[2]);
                      ht.Add("DKBH", sPlotNo); // temp[2]
                      MapgisEgov.AnalyInput.Common.Log.Write("temp[3]:" + temp[3]);
                      ht.Add("DKMC", temp[3]);
                      MapgisEgov.AnalyInput.Common.Log.Write("temp[6]:" + temp[6]);
                      ht.Add("DKYT", temp[6]);
                      MapgisEgov.AnalyInput.Common.Log.Write("temp[0]:" + temp[0]);
                      ht.Add("JZDS", temp[0]);
                      MapgisEgov.AnalyInput.Common.Log.Write("temp[1]:" + temp[1]);
                      ht.Add("DKMJ", temp[1]);
                      MapgisEgov.AnalyInput.Common.Log.Write("temp[4]:" + temp[4]);
                      ht.Add("TXLX", temp[4]);
                      MapgisEgov.AnalyInput.Common.Log.Write("RiQi:" + System.DateTime.Now.ToString("yyyy-MM-dd"));
                      ht.Add("RiQi", System.DateTime.Now.ToString("yyyy-MM-dd"));
                      try
                      {
                          iDb.AddData("DKZBXX", ht);
                      }
                      catch (Exception ex)
                      {
                          MapgisEgov.AnalyInput.Common.Log.Write("ex:" + ex);
                          sMsg = ex.ToString();
                          return false;
                      }


                      string[] tempJZDH = JZDHList[ii].ToString().Split(',');
                      //MapgisEgov.AnalyInput.Common.Log.Write("1111111");
                      string[] tempDKQH = DKQHList[ii].ToString().Split(',');
                      //MapgisEgov.AnalyInput.Common.Log.Write("222222");
                      string[] tempDOT = pointList[ii].ToString().Split(' ');
                      //MapgisEgov.AnalyInput.Common.Log.Write("33333333");
                      int iNum = tempDKQH.Length;
                      //MapgisEgov.AnalyInput.Common.Log.Write("iNum:" + iNum);
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
          #endregion


          #region 高斯坐标转换经纬度坐标 转换
          //  由高斯投影坐标反算成经纬度
          public static double[] GaussToBL(double X, double Y)//, double *longitude, double *latitude)
          {
              int ProjNo; int ZoneWide; ////带宽
              double[] output = new double[2];
              double longitude1, latitude1, longitude0, X0, Y0, xval, yval;//latitude0,
              double e1, e2, f, a, ee, NN, T, C, M, D, R, u, fai, iPI;
              iPI = 0.0174532925199433; ////3.1415926535898/180.0;
              //a = 6378245.0; f = 1.0/298.3; //54年北京坐标系参数
              a = 6378140.0; f = 1 / 298.257; //80年西安坐标系参数
              //ZoneWide = 6; ////6度带宽
              ZoneWide = 3; ////3度带宽
              ProjNo = (int)(X / 1000000L); //查找带号
              //longitude0 = (ProjNo - 1) * ZoneWide + ZoneWide / 2; //6度带宽中央经线计算方法
              longitude0 = ProjNo * ZoneWide;//3度带宽中央经线
              longitude0 = longitude0 * iPI; //中央经线


              X0 = ProjNo * 1000000L + 500000L;
              Y0 = 0;
              xval = X - X0; yval = Y - Y0; //带内大地坐标
              e2 = 2 * f - f * f;
              e1 = (1.0 - Math.Sqrt(1 - e2)) / (1.0 + Math.Sqrt(1 - e2));
              ee = e2 / (1 - e2);
              M = yval;
              u = M / (a * (1 - e2 / 4 - 3 * e2 * e2 / 64 - 5 * e2 * e2 * e2 / 256));
              fai = u + (3 * e1 / 2 - 27 * e1 * e1 * e1 / 32) * Math.Sin(2 * u) + (21 * e1 * e1 / 16 - 55 * e1 * e1 * e1 * e1 / 32) * Math.Sin(4 * u) + (151 * e1 * e1 * e1 / 96) * Math.Sin(6 * u) + (1097 * e1 * e1 * e1 * e1 / 512) * Math.Sin(8 * u);
              C = ee * Math.Cos(fai) * Math.Cos(fai);
              T = Math.Tan(fai) * Math.Tan(fai);
              NN = a / Math.Sqrt(1.0 - e2 * Math.Sin(fai) * Math.Sin(fai));
              R = a * (1 - e2) / Math.Sqrt((1 - e2 * Math.Sin(fai) * Math.Sin(fai)) * (1 - e2 * Math.Sin(fai) * Math.Sin(fai)) * (1 - e2 * Math.Sin
              (fai) * Math.Sin(fai)));
              D = xval / NN;
              //计算经度(Longitude) 纬度(Latitude)
              longitude1 = longitude0 + (D - (1 + 2 * T + C) * D * D * D / 6 + (5 - 2 * C + 28 * T - 3 * C * C + 8 * ee + 24 * T * T) * D
              * D * D * D * D / 120) / Math.Cos(fai);
              latitude1 = fai - (NN * Math.Tan(fai) / R) * (D * D / 2 - (5 + 3 * T + 10 * C - 4 * C * C - 9 * ee) * D * D * D * D / 24
              + (61 + 90 * T + 298 * C + 45 * T * T - 256 * ee - 3 * C * C) * D * D * D * D * D * D / 720);
              //转换为度 DD
              output[0] = longitude1 / iPI;
              output[1] = latitude1 / iPI;
              return output;
              //*longitude = longitude1 / iPI;
              //*latitude = latitude1 / iPI;
          }
        ////  由经纬度反算成高斯投影坐标
        /// <summary>
        /// 平面坐标转换为经纬度坐标，返回值为字符串，x/y用逗号隔开
        /// </summary>
        /// <param name="xy"></param>
        /// <returns></returns>
          public static string toJW(string[] xy)
          {
              try
              {
                  double x = Convert.ToDouble(xy[1]);
                  double y = Convert.ToDouble(xy[0]);
                  double[] jw = InputText.GaussToBL(x, y);
                  string JW = "经度:" + jw[0].ToString() + ",纬度:" + jw[1].ToString();
                  MapgisEgov.AnalyInput.Common.Log.Write(JW);
                  string result = jw[0].ToString() + "," + jw[1].ToString();
                  //return JW;
                  return result;
              }
              catch (Exception oExcept)
              {
                  return oExcept.Message;
              }
          }
          #endregion

          #region 经纬度坐标转换为高斯坐标
        /// <summary>
        /// 经纬度坐标转换为高斯坐标
        /// </summary>
        /// <param name="jd">经度</param>
        /// <param name="wd">纬度</param>
        /// <param name="DH">带号</param>
        /// <param name="DH_width">带宽</param>
        /// <param name="LP">如果不设中央经线（缺省参数: -1000），则计算中央经线，否则，使用传入的中央经线，不再使用带号和带宽参数</param>
        /// <returns></returns>
          public static string GeoToGauss(double jd, double wd, short DH, short DH_width,  double LP)
          {
              double t;     //  t=tgB
              double L;     //  中央经线的经度
              double l0;    //  经差
              double jd_hd, wd_hd;  //  将jd、wd转换成以弧度为单位
              double et2;    //  et2 = (e' ** 2) * (cosB ** 2)
              double N;     //  N = C / sqrt(1 + et2)
              double X;     //  克拉索夫斯基椭球中子午弧长
              double m;     //  m = cosB * PI/180 * l0 
              double tsin, tcos;   //  sinB,cosB
              double PI = 3.14159265358979;
              double b_e2 = 0.0067385254147;
              double b_c = 6399698.90178271;
              jd_hd = jd / 3600.0 * PI / 180.0;    // 将以秒为单位的经度转换成弧度
              wd_hd = wd / 3600.0 * PI / 180.0;    // 将以秒为单位的纬度转换成弧度

              // 如果不设中央经线（缺省参数: -1000），则计算中央经线，
              // 否则，使用传入的中央经线，不再使用带号和带宽参数
              //L = (DH - 0.5) * DH_width ;      // 计算中央经线的经度
              if (LP == -1000)
              {
                  L = (DH - 0.5) * DH_width;      // 计算中央经线的经度
              }
              else
              {
                  L = LP;
              }
              l0 = jd / 3600.0 - L;       // 计算经差
              tsin = Math.Sin(wd_hd);        // 计算sinB
              tcos =Math.Cos(wd_hd);        // 计算cosB
              // 计算克拉索夫斯基椭球中子午弧长X
              X = 111134.8611 / 3600.0 * wd - (32005.7799 * tsin + 133.9238 * Math.Pow(tsin, 3) + 0.6976 * Math.Pow(tsin, 5) + 0.0039 * Math.Pow(tsin, 7)) * tcos;
              et2 = b_e2 * Math.Pow(tcos, 2);      //  et2 = (e' ** 2) * (cosB ** 2)
              N = b_c / Math.Sqrt(1 + et2);      //  N = C / sqrt(1 + et2)
              t = Math.Tan(wd_hd);         //  t=tgB
              m = PI / 180 * l0 * tcos;       //  m = cosB * PI/180 * l0 

              string GaussX = (X + N * t * (0.5 * Math.Pow(m, 2)+ (5.0 - Math.Pow(t, 2) + 9.0 * et2 + 4 * Math.Pow(et2, 2)) * Math.Pow(m, 4) / 24.0 + (61.0 - 58.0 * Math.Pow(t, 2) + Math.Pow(t, 4)) * Math.Pow(m, 6) / 720.0)).ToString();
              string GaussY = (N * (m + (1.0 - Math.Pow(t, 2) + et2) * Math.Pow(m, 3) / 6.0 + (5.0 - 18.0 * Math.Pow(t, 2) + Math.Pow(t, 4) + 14.0 * et2 - 58.0 * et2 * Math.Pow(t, 2)) * Math.Pow(m, 5) / 120.0)).ToString();
              string GaussXY = GaussX + "," + GaussY;
              return GaussXY;
          }

        /// <summary>
        /// 经纬度反算高斯坐标
        /// </summary>
        /// <param name="longitude"></param>
        /// <param name="latitude"></param>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <returns></returns>
         public static string GeoToGauss(double longitude, double latitude) 
         {
              int ProjNo=0; 
              int ZoneWide; ////带宽 
              double longitude1,latitude1, longitude0,latitude0, X0,Y0, xval,yval;
              double a,f, e2,ee, NN, T,C,A, M, iPI;
              iPI = 0.0174532925199433; ////3.1415926535898/180.0; 
              //ZoneWide = 6; ////6度带宽 
              ZoneWide = 3; ////3度带宽 
              //a=6378245.0; f=1.0/298.3; //54年北京坐标系参数 
              a=6378140.0; f=1/298.257; //80年西安坐标系参数 
              ProjNo = (int)(longitude / ZoneWide) ;
              //longitude0 = ProjNo * ZoneWide + ZoneWide / 2; 
              longitude0 = ProjNo * ZoneWide; 
              longitude0 = longitude0 * iPI ;
              latitude0=0; 
              longitude1 = longitude * iPI ; //经度转换为弧度
              latitude1 = latitude * iPI ; //纬度转换为弧度
              e2=2*f-f*f;
              ee=e2*(1.0-e2);
              NN=a/Math.Sqrt(1.0-e2*Math.Sin(latitude1)*Math.Sin(latitude1));
              T=Math.Tan(latitude1)*Math.Tan(latitude1);
              C=ee*Math.Cos(latitude1)*Math.Cos(latitude1);
              A=(longitude1-longitude0)*Math.Cos(latitude1); 
              
              M=a*((1-e2/4-3*e2*e2/64-5*e2*e2*e2/256)*latitude1-(3*e2/8+3*e2*e2/32+45*e2*e2*e2/1024)*Math.Sin(2*latitude1)+(15*e2*e2/256+45*e2*e2*e2/1024)*Math.Sin(4*latitude1)-(35*e2*e2*e2/3072)*Math.Sin(6*latitude1));
              xval = NN*(A+(1-T+C)*A*A*A/6+(5-18*T+T*T+72*C-58*ee)*A*A*A*A*A/120);
              yval = M + NN * Math.Tan(latitude1) * (A * A / 2 + (5 - T + 9 * C + 4 * C * C) * A * A * A * A / 24 + (61 - 58 * T + T * T + 600 * C - 330 * ee) * A * A * A * A * A * A / 720);
              //X0 = 1000000L*(ProjNo+1)+500000L; 
              X0 = 1000000L * (ProjNo) + 500000L; 
              Y0 = 0; 
              xval = xval+X0;
              yval = yval+Y0; 
              string X = xval.ToString();
              string Y = yval.ToString();
              string XY = X + "," + Y;
              //MapgisEgov.AnalyInput.Common.Log.Write("public static string GeoToGauss(double longitude, double latitude)调用成功！");
              return XY;
         }

         public static string GeoToGauss(string strDotString)
         {
             StringBuilder sb = new StringBuilder();
             string[] sPlot = strDotString.Split('#');
             for (int i = 0; i < sPlot.Length; i++)
             {
                 string[] sHuan = sPlot[i].Split('@');
                 for (int j = 0; j < sHuan.Length; j++)
                 {
                     string[] sDot = sHuan[j].Split(' ');
                     for (int n = 0; n < sDot.Length; n++)
                     {
                         string[] xy = sDot[n].Split(',');
                         xy[0] = InputText.DMsToTranDegree(Double.Parse(xy[0])).ToString();
                         xy[1] = InputText.DMsToTranDegree(Double.Parse(xy[1])).ToString();
                         //xy[0] = InputText.TranDegreeToDMs(Double.Parse(xy[0]));
                         //xy[1] = InputText.TranDegreeToDMs(Double.Parse(xy[1]));
                         double jd = double.Parse(xy[0]);
                         double wd = double.Parse(xy[1]);
                         xy = InputText.GeoToGauss(jd, wd).Split(',');
                         sb.Append(xy[1] + "," + xy[0] + " ");//根据入库坐标格式调换xy
                         //MapgisEgov.AnalyInput.Common.Log.Write(sb.ToString());
                     }
                     if (sb.ToString().EndsWith(" "))
                     {
                         sb.Remove(sb.Length - 1, 1);
                     }
                     sb.Append("@");
                 }
                 if (sb.ToString().EndsWith("@"))
                 {
                     sb.Remove(sb.Length - 1, 1);
                 }
                 sb.Append("#");
             }
             //strDotString = sb.ToString();
             if (sb.ToString().EndsWith("#"))
             {
                 sb.Remove(sb.Length - 1, 1);
             }

             return sb.ToString();
         }

          #endregion

          #region 度分秒/度 坐标转换
          /// <summary>
          /// 将度转换为度分秒 by yl landgis@126.com
          /// </summary>
          /// <param name="d"></param>
          /// <returns></returns>
          public static string TranDegreeToDMs(double d)
          {

              int Degree = Convert.ToInt16(Math.Truncate(d));//度

              d = d - Degree;
              int M = Convert.ToInt16(Math.Truncate((d) * 60));//分
              
              int S = Convert.ToInt16(Math.Round((d * 60 - M) * 60));

              if (S == 60)
              {

                  M = M + 1;
                  S = 0;
              }
              if (M == 60)
              {

                  M = 0;
                  Degree = Degree + 1;
              }
              string rstr = Degree.ToString() + ".";
              if (M < 10)
              {
                  rstr = rstr + "0" + M.ToString();
              }
              else
              {
                  rstr = rstr + M.ToString();
              }

              if (S < 10)
              {
                  rstr = rstr + "0" + S.ToString();
              }
              else
              {
                  rstr = rstr + S.ToString();
              }
              return rstr;
          }

          public static double DMsToTranDegree(double d)
          {
              /*
              //double Degree = Convert.ToInt16(Math.Truncate(d));//度
              int Degree = (int)(Math.Truncate(d));//度
              d = 100 * (d-Degree);
              //double Minute = Convert.ToInt16(Math.Truncate(d));//分
              int Minute = (int)(Math.Truncate(d));//分
              d = 100 * (d - Minute);
              //double Second = Convert.ToInt16(Math.Truncate(d));//秒
              int Second = (int)(Math.Truncate(d));//秒
              d = 100*(d - Second);
              double Remainder = d / 1000000;
              //double Value = Degree + Minute / 60 + Second / 6000 + Remainder;
              double Value = Degree + (Minute +(Second / 60))/ 60  + Remainder;
              MapgisEgov.AnalyInput.Common.Log.Write("Degree="+Degree+",Minute="+Minute+",Second="+Second+",Reminder="+Remainder);
              return Value;
               * */
              decimal dc = decimal.Parse(d.ToString()) ;
              //MapgisEgov.AnalyInput.Common.Log.Write(dc.ToString());
              decimal Degree = (int)(dc);//度
              dc = 100 * (dc - Degree);
              //MapgisEgov.AnalyInput.Common.Log.Write(Degree+"----"+dc);
              decimal Minute = (int)(dc);//分
              dc = 100 * (dc - Minute);
              //MapgisEgov.AnalyInput.Common.Log.Write(Minute+"----"+dc);
              decimal Second = dc;//秒
              //MapgisEgov.AnalyInput.Common.Log.Write(Second + "----" + dc);
              //d = 100 * (d - Second);
              //double Value = Degree + Minute / 60 + Second / 6000 + Remainder;
              double Value = double.Parse((Degree + (Minute + (Second / 60)) / 60).ToString());
              return Value;
          }

          #endregion

          /// <summary>
          /// 将坐标串保存至lc_kcxkz表
          /// </summary>
          /// <param name="strDotString">坐标串</param>
          /// <param name="keyField">upadte关键字</param>
          /// <param name="keyValue">upadte关键字的值</param>
          public static void saveDotStringToKCXKZ(string strDotString,string keyField,string keyValue)
          {
              try
              {
                  string sMsg = string.Empty;
                  string Sql = "update lc_kcxkz set kcqygdzb='" + strDotString + "' where " + keyField + "='" + keyValue + "'";//UPDATE 表名称 SET 列名称 = 新值 WHERE 列名称 = 某值
                  DatabaseORC db = new DatabaseORC();
                  db.ExecuteSql(Sql);
              }
              catch (Exception oExcept)
              {
                  MapgisEgov.AnalyInput.Common.Log.Write(oExcept.Message);
              }
          }


        /// <summary>
        /// 将采矿权数据
        /// </summary>
          /// <param name="strDotString">坐标串,例如：x1,y1 x2,y2 x3,y3 x1,y1@x1,y1 x2,y2 x3,y3 x1,y1#x1,y1 x2,y2 x3,y3 x1,y1</param>
        /// <param name="strSubject">格式化方法,CK或TK</param>
        /// <returns></returns>
          public static string transFormattedCTK(string strDotString,string strSubject)
          {
              try
              {
                  string strFormattedDotString = string.Empty;
                  if (strSubject == "TK")
                  {
                      int plotNum = 1;
                      for (int m = 0; m < strDotString.Length; m++)
                      {
                          if (strDotString.Substring(m, 1) == "@" )
                          {
                              plotNum++;
                          }
                          else if (strDotString.Substring(m, 1) == "#")
                          {
                              plotNum++;
                          }
                      }
                      string[] sPlot = strDotString.Split('#');
                      //plotNum = plotNum + sPlot.Length;
                      //strFormattedDotString=plotNum+",";
                      for (int i = 0; i < sPlot.Length; i++)
                      {
                          string[] sHuan = sPlot[i].Split('@');
                          //plotNum = plotNum + sHuan.Length;


                          for (int j = 0; j < sHuan.Length; j++)
                          {
                              
                              string[] sDots = sHuan[j].Split(' ');
                              string dotNum=sDots.Length.ToString();
                              strFormattedDotString=strFormattedDotString+dotNum+",";
                              for(int n=0;n<sDots.Length;n++)
                              {
                                  strFormattedDotString=strFormattedDotString+sDots[n]+",";
                              }
                              if (j == 0)
                              {
                                  strFormattedDotString = strFormattedDotString + "0,0,0,";//@后面跟的坐标串为减去，后缀
                              }
                              else
                              {
                                  strFormattedDotString = strFormattedDotString + "-1,0,0,";//@后面跟的坐标串为减去，后缀
                              }
                          }
                      }
                      strFormattedDotString = plotNum + "," + strFormattedDotString;
                  }
                  else if (strSubject == "CK")
                  {
                      int plotNum = 1;
                      for (int m = 0; m < strDotString.Length; m++)
                      {
                          if (strDotString.Substring(m, 1) == "@")
                          {
                              plotNum++;
                          }
                          else if (strDotString.Substring(m, 1) == "#")
                          {
                              plotNum++;
                          }
                      }
                      string[] sPlot = strDotString.Split('#');
                      //plotNum = plotNum + sPlot.Length;
                      //strFormattedDotString=plotNum+",";
                      for (int i = 0; i < sPlot.Length; i++)
                      {
                          string[] sHuan = sPlot[i].Split('@');
                          //plotNum = plotNum + sHuan.Length;


                          for (int j = 0; j < sHuan.Length; j++)
                          {

                              string[] sDots = sHuan[j].Split(' ');
                              string dotNum = sDots.Length.ToString();
                              strFormattedDotString = strFormattedDotString + dotNum + ",";
                              for (int n = 0; n < sDots.Length; n++)
                              {
                                  strFormattedDotString =(n+1).ToString()+","+ strFormattedDotString + sDots[n] + ",";
                              }
                              if (j == 0)
                              {
                                  strFormattedDotString = strFormattedDotString + "0,0,,1";//第一个坐标串为增加，后缀0,0,代表标高，空值代表说明，1代表增加
                              }
                              else
                              {
                                  strFormattedDotString = strFormattedDotString + "0,0,,-1";//@后面跟的坐标串为减去，0,0,代替标高，空值代表注释，-1代表减去
                              }
                          }
                      }
                      strFormattedDotString = plotNum + "," + strFormattedDotString;
                   }
                      return strFormattedDotString;
              }
              catch(Exception oExcept)
              {
                  return oExcept.Message;
              }
          }

        /// <summary>
        /// 根据CASENO取得对应采矿权流程所存的拐点坐标串
        /// </summary>
        /// <param name="CASENO"></param>
        /// <returns></returns>
          public static string getDotsFromCMLCCKQ(string CASENO)
          {
              try
              {
                  DatabaseORC db = new DatabaseORC();
                  string SQL = "select t.矿区坐标 from CM_LC_CKQ t where t.CASENO='" + CASENO + "'";
                  DataSet ds = db.GetDataSet(SQL);
                  //MapgisEgov.AnalyInput.Common.Log.Write("getDotsFromCMLCCKQ参数为"+CASENO);
                  string DotString = ds.Tables[0].Rows[0][0].ToString();
                  return DotString;
              }
              catch (Exception oExcept)
              {
                  return "未能正确取得坐标串，原因请参考：" + oExcept.Message;
              }
          }

          /// <summary>
          /// 根据Guid取得对应采矿权数据库所存的拐点坐标串
          /// </summary>
          /// <param name="Guid">采矿权唯一标识码</param>
          /// <returns></returns>
          public static string getDotsFromCKSQDJ(string ckGuid)
          {
              try
              {
                  DatabaseORC db = new DatabaseORC();
                  string SQL = "select t.区域坐标 from 采矿申请登记 t where t.CK_GUID='" + ckGuid + "'";
                  DataSet ds = db.GetDataSet(SQL);
                  //MapgisEgov.AnalyInput.Common.Log.Write("getDotsFromCKSQDJ参数为" + ckGuid);
                  string DotString = ds.Tables[0].Rows[0][0].ToString();
                  DotString = GetCKFormattedDotStringJT(DotString);
                  return DotString;
              }
              catch (Exception oExcept)
              {
                  return "未能正确取得坐标串，原因请参考：" + oExcept.Message;
              }
          }

          /// <summary>
          /// 根据CASENO取得对应探矿权数据库所存的拐点坐标串
          /// </summary>
          /// <param name="CASENO">探矿权数据唯一标识码</param>
          /// <returns></returns>
          public static string getDotsFromCMLCKCXKZ(string CASENO)
          {
              try
              {
                  DatabaseORC db = new DatabaseORC();
                  string SQL = "select t.KCQYGDZB from lc_kcxkz t where t.CASENO='" + CASENO + "'";
                  DataSet ds = db.GetDataSet(SQL);
                  //MapgisEgov.AnalyInput.Common.Log.Write("getDotsFromCMLCKCXKZ参数为" + CASENO);
                  string DotString = ds.Tables[0].Rows[0][0].ToString();
                  DotString = GetTKFormattedDotStringJT(DotString);//GetCKFormattedDotStringJT(DotString);
                  return DotString;
              }
              catch (Exception oExcept)
              {
                  return "未能正确取得坐标串，原因请参考：" + oExcept.Message;
              }
          }


          /// <summary>
          /// 根据CASENO取得对应探矿权数据库所存的拐点坐标串
          /// </summary>
          /// <param name="CASENO">探矿权数据唯一标识码 申请序号</param>
          /// <returns></returns>
          public static string getDotsFromKCXMDJ(string CASENO)
          {
              try
              {
                  /*
                  string Conn = System.Configuration.ConfigurationManager.ConnectionStrings["TK_CONN"].ToString();
                  DatabaseORC db = new DatabaseORC(Conn);
                   * */
                  Models.DatabaseORCTK db = new Models.DatabaseORCTK();
                  string SQL = "select t.区域坐标 from 勘查项目登记 t where t.申请序号='" + CASENO + "'";
                  DataSet ds = db.GetDataSet(SQL);
                  //MapgisEgov.AnalyInput.Common.Log.Write("getDotsFromKCXMDJ参数为" + CASENO);
                  string DotString = ds.Tables[0].Rows[0][0].ToString();
                  DotString = GetTKFormattedDotStringJT(DotString);//GetCKFormattedDotStringJT(DotString);
                  return DotString;
              }
              catch (Exception oExcept)
              {
                  return "未能正确取得坐标串，原因请参考：" + oExcept.Message;
              }
          }

    }

}