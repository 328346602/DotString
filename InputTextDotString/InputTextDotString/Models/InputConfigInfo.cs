/************************************
 * 创建人：王金河
 * 创建时间：2013-12-2 11:35
 * 内容：用来替代InputConfig.xml中配置入库字段；
 *       主要提供对表inputConfigInfo的操作
 * 注意：(1)虽然flow是主键，但是这样做只是方便根据flow取入库参数，实际以"subject"为主键，按照subject配置flow
 * 
 * 
 * 
 ***********************************/

using System;
using System.Collections.Generic;
using System.Web;
using System.Data;
using System.Collections;
using MapgisEgov.AnalyInput.Common;

namespace MapgisEgov.AnalyInput.Models
{
    /// <summary>
    /// 提供对表inputConfigInfo的操作
    /// </summary>
    public class InputConfigInfo
    {
      public string  ID { get;set;}
      public string  Flow {get;set;}
      public string Subject {get;set;}
      public string Year { get;set;}
      public string RegionCode { get; set; }
      public string Scale { get;set;}
      public string LayerShortName { get;set;}
      public string LayerCaseKey { get;set;}
      public string LayerPlotKey { get;set;}
      public string  TableCaseKey { get;set;}
      public string TablePlotKey { get;set;}
      public string TableName { get;set;}
      public string TableFields { get;set;}
      public string LayerFields { get;set;}
      public string SolutionName { get; set; }

      public InputConfigInfo()
      {

      }


      #region 和页面EditInputInfo.aspx关系大的方法，方便调用
      /// <summary>
      /// 获取所有的流程前缀
      /// </summary>
      /// <param name="iDb"></param>
      /// <returns></returns>
      public static DataSet GetAllFlow(GS.DataBase.IDbAccess iDb)
      {
          string sSql = "select distinct 编号前缀 as Flow from FLOWCTRLTBL where 编号前缀 is not null and 编号前缀!= "+"'CKQ'";
          DataSet ds = iDb.GetDataSet(sSql) ;
          return ds;
      }


       

      #endregion

      /// <summary>
      /// 按照流程查询所有信息
      /// </summary>
      /// <param name="iDb">数据库对象</param>
      /// <param name="sKey">流程标识</param>
      /// <returns></returns>
      public InputConfigInfo Search(GS.DataBase.IDbAccess iDb, string sFlow)
      {
          InputConfigInfo result = new InputConfigInfo();
          DataSet ds = iDb.GetDataSet("SELECT ID,Flow,Subject,Year,RegionCode,Scale,LayerShortName,LayerCaseKey,LayerPlotKey,TableCaseKey,TablePlotKey,TableName,TableFields,LayerFields,SolutionName FROM INPUTCONFIGINFO WHERE flow='" + sFlow + "'");
          if (ds != null && ds.Tables[0].Rows.Count > 0)
          {
              result.ID = ds.Tables[0].Rows[0]["ID"].ToString();
              result.Flow = ds.Tables[0].Rows[0]["Flow"].ToString();
              result.Subject = ds.Tables[0].Rows[0]["Subject"].ToString();
              result.Year = ds.Tables[0].Rows[0]["Year"].ToString();
              result.RegionCode = ds.Tables[0].Rows[0]["RegionCode"].ToString();
              result.Scale = ds.Tables[0].Rows[0]["Scale"].ToString();
              result.LayerShortName = ds.Tables[0].Rows[0]["LayerShortName"].ToString();
              result.LayerCaseKey = ds.Tables[0].Rows[0]["LayerCaseKey"].ToString();
              result.LayerPlotKey = ds.Tables[0].Rows[0]["LayerPlotKey"].ToString();
              result.TableCaseKey = ds.Tables[0].Rows[0]["TableCaseKey"].ToString();
              result.TablePlotKey = ds.Tables[0].Rows[0]["TablePlotKey"].ToString();
              result.TableName = ds.Tables[0].Rows[0]["TableName"].ToString();
              result.TableFields = ds.Tables[0].Rows[0]["TableFields"].ToString();
              result.LayerFields = ds.Tables[0].Rows[0]["LayerFields"].ToString();
              result.SolutionName = ds.Tables[0].Rows[0]["SolutionName"].ToString();
          }
          return result;
      }

      #region 和专题保持一致相关的方法
      

      /// <summary>
      /// 根据专题查询图层信息
      /// </summary>
      /// <param name="iDb">数据库对象</param>
      /// <param name="sSubject">专题</param>
      /// <returns></returns>
      public InputConfigInfo SearchSubject(GS.DataBase.IDbAccess iDb, string sSubject)
      {
          InputConfigInfo result = new InputConfigInfo();

          DataSet ds = iDb.GetDataSet("SELECT Subject,Year,RegionCode,Scale,LayerShortName FROM INPUTCONFIGINFO WHERE Subject='" + sSubject + "'");


              ///<summary>
              ///采矿权专题数据判断
              ///<summary>
          //DataSet ds = iDb.GetDataSet("SELECT Subject,Year,RegionCode,Scale,LayerShortName FROM INPUTCONFIGINFO WHERE Subject='" + sSubject + "'");
          if (ds != null && ds.Tables[0].Rows.Count > 0)
          {
              string SLN = "CK";
              if(result.SolutionName!=SLN)
              {
              result.Subject = ds.Tables[0].Rows[0]["Subject"].ToString();
              result.Year = ds.Tables[0].Rows[0]["Year"].ToString();
              result.RegionCode = ds.Tables[0].Rows[0]["RegionCode"].ToString();
              result.Scale = ds.Tables[0].Rows[0]["Scale"].ToString();
              result.LayerShortName = ds.Tables[0].Rows[0]["LayerShortName"].ToString(); 
              }
              else
              {
                  result.Subject = ds.Tables[0].Rows[0]["Subject"].ToString();
                  result.RegionCode = ds.Tables[0].Rows[0]["RegionCode"].ToString();
                  result.LayerShortName = ds.Tables[0].Rows[0]["LayerShortName"].ToString(); 
              }
          }
          return result;
      }

      /// <summary>
      /// 将和专题相关的信息全部更新
      /// </summary>
      /// <param name="iDb"></param>
      /// <param name="obj"></param>
      /// <returns></returns>
      public bool UpdateSubject(GS.DataBase.IDbAccess iDb, ref InputConfigInfo obj)
      {
          Hashtable ht = new Hashtable();       
        
          ht.Add("Subject", obj.Subject);
          ht.Add("Year", obj.Year);
          ht.Add("RegionCode", obj.RegionCode);
          ht.Add("Scale", obj.Scale);
          ht.Add("LayerShortName", obj.LayerShortName);
          return iDb.UpdateData("INPUTCONFIGINFO", ht, "Subject");
          
      }

      #endregion
      /// <summary>
      /// 保存
      /// </summary>
      /// <param name="iDb">数据库对象</param>
      /// <param name="obj">表对象</param>
      /// <returns>保存是否成功</returns>
      public bool Save(GS.DataBase.IDbAccess iDb, ref InputConfigInfo obj)
      {
          Hashtable ht = new Hashtable();         
          ht.Add("Flow", obj.Flow);
          ht.Add("Subject", obj.Subject);
          ht.Add("Year", obj.Year);
          ht.Add("RegionCode", obj.RegionCode);          
          ht.Add("Scale", obj.Scale);
          ht.Add("LayerShortName", obj.LayerShortName);
          ht.Add("LayerCaseKey", obj.LayerCaseKey);
          ht.Add("LayerPlotKey", obj.LayerPlotKey);
          ht.Add("TableCaseKey", obj.TableCaseKey);
          ht.Add("TablePlotKey", obj.TablePlotKey);
          ht.Add("TableName", obj.TableName);
          ht.Add("TableFields", obj.TableFields);
          ht.Add("LayerFields", obj.LayerFields);
          ht.Add("SolutionName", obj.SolutionName);
          return iDb.SaveData("INPUTCONFIGINFO", ht, "Flow");
      }

      /// <summary>
      /// 删除
      /// </summary>
      /// <param name="iDb">数据库对象</param>
      /// <param name="sKey">关键字的值</param>
      /// <returns>返回删除的记录行数</returns>
      public int Delete(GS.DataBase.IDbAccess iDb, string sKey)
      {
          return iDb.DeleteTableRow("INPUTCONFIGINFO", " flow='" + sKey+"'");
      }

      /// <summary>
      /// 获得指定过滤条件的记录行数
      /// </summary>
      /// <param name="iDb">数据库对象</param>
      /// <param name="sFilter">查询条件，以AND开头,可以为空</param>
      /// <returns>数据库中的记录行数</returns>
      public int GetDataCount(GS.DataBase.IDbAccess iDb, string sFilter)
      {
          string strSql = "select count(*) from INPUTCONFIGINFO where 1=1 " + sFilter;
          return Convert.ToInt32(iDb.GetFirstColumn(strSql));
      }

      /// <summary>
      /// 返回全部数据List
      /// </summary>
      /// <param name="iDb">数据库对象</param>
      /// <param name="sFilter">查询条件，以AND开头,可以为空</param>
      /// <param name="sOrderColumn">排序字段，可以指定多个字段，以','分隔</param>
      /// <param name="sOrderType">排序类型 ASC/DESC</param>
      /// <returns>返回全部数据List</returns>
      public List<InputConfigInfo> GetFullList(GS.DataBase.IDbAccess iDb, string sFilter, string sOrderColumn, string sOrderType)
      {
          return GetListByPage(iDb, "flow", 0, 0, sFilter, sOrderColumn, sOrderType);
      }

      /// <summary>
      /// 返回分页数据List
      /// </summary>
      /// <param name="iDb">数据库对象</param>
      /// <param name="sKey">表关键字</param>
      /// <param name="iPageSize">单页记录行数</param>
      /// <param name="iPageIndex">分页索引</param>
      /// <param name="sFilter">查询条件，以AND开头,可以为空</param>
      /// <param name="sOrderColumn">排序字段，可以指定多个字段，以','分隔</param>
      /// <param name="sOrderType">排序类型 ASC/DESC</param>
      /// <returns>返回分页数据List</returns>
      public List<InputConfigInfo> GetListByPage(GS.DataBase.IDbAccess iDb, string sKey, int iPageSize, int iPageIndex, string sFilter, string sOrderColumn, string sOrderType)
      {
          string sOrderString = "";
          if (sOrderColumn != "")
          {
              sOrderString = " ORDER BY " + sOrderColumn;
              if (sOrderType.ToLower() == "desc")
              {
                  sOrderString += " DESC ";
              }
              else
              {
                  sOrderString += " ASC ";
              }
          }
          sFilter = " 1=1 " + sFilter;
          string strSql = "SELECT ID,Flow,Subject,Year,RegionCode,Scale,LayerShortName,LayerCaseKey,LayerPlotKey,TableCaseKey,TablePlotKey,TableName,TableFields,LayerFields,SolutionName FROM INPUTCONFIGINFO t WHERE " + sFilter + sOrderString;
          if (iPageSize > 0)
          {
              strSql = iDb.GetSqlForPageSize("INPUTCONFIGINFO", "flow", iPageSize, iPageIndex, sFilter, sOrderString);
          }
          DataSet ds = iDb.GetDataSet(strSql);
          List<InputConfigInfo> list = new List<InputConfigInfo>();
          for (int ii = 0; ii < ds.Tables[0].Rows.Count; ii++)
          {
              InputConfigInfo result = new InputConfigInfo();
              result.ID = ds.Tables[0].Rows[ii]["ID"].ToString();
              result.Flow = ds.Tables[0].Rows[ii]["Flow"].ToString();
              result.Subject = ds.Tables[0].Rows[ii]["Subject"].ToString();
              result.Year = ds.Tables[0].Rows[ii]["Year"].ToString();
              result.RegionCode = ds.Tables[0].Rows[ii]["RegionCode"].ToString();
              
              result.Scale = ds.Tables[0].Rows[ii]["Scale"].ToString();
              result.LayerShortName = ds.Tables[0].Rows[ii]["LayerShortName"].ToString();
              result.LayerCaseKey = ds.Tables[0].Rows[ii]["LayerCaseKey"].ToString();
              result.LayerPlotKey = ds.Tables[0].Rows[ii]["LayerPlotKey"].ToString();
              result.TableCaseKey = ds.Tables[0].Rows[ii]["TableCaseKey"].ToString();
              result.TablePlotKey = ds.Tables[0].Rows[ii]["TablePlotKey"].ToString();
              result.TableName = ds.Tables[0].Rows[ii]["TableName"].ToString();
              result.TableFields = ds.Tables[0].Rows[ii]["TableFields"].ToString();
              result.LayerFields = ds.Tables[0].Rows[ii]["LayerFields"].ToString();
              result.SolutionName = ds.Tables[0].Rows[ii]["SolutionName"].ToString();
              list.Add(result);
          }
          return list;
      }

      /// <summary>
      /// 返回全部数据DataSet
      /// </summary>
      /// <param name="iDb">数据库对象</param>
      /// <param name="sFilter">查询条件，以AND开头,可以为空</param>
      /// <param name="sOrderColumn">排序字段，可以指定多个字段，以','分隔</param>
      /// <param name="sOrderType">排序类型 ASC/DESC</param>
      /// <returns>返回全部数据DataSet</returns>
      public DataSet GetFullData(GS.DataBase.IDbAccess iDb, string sFilter, string sOrderColumn, string sOrderType)
      {
          return GetDataByPage(iDb, "flow", 0, 0, sFilter, sOrderColumn, sOrderType);
      }

      /// <summary>
      /// 返回分页数据DataSet
      /// </summary>
      /// <param name="iDb">数据库对象</param>
      /// <param name="sKey">表关键字</param>
      /// <param name="iPageSize">单页记录行数</param>
      /// <param name="iPageIndex">分页索引</param>
      /// <param name="sFilter">查询条件，以AND开头,可以为空</param>
      /// <param name="sOrderColumn">排序字段，可以指定多个字段，以','分隔</param>
      /// <param name="sOrderType">排序类型 ASC/DESC</param>
      /// <returns>返回分页数据DataSet</returns>
      public DataSet GetDataByPage(GS.DataBase.IDbAccess iDb, string sKey, int iPageSize, int iPageIndex, string sFilter, string sOrderColumn, string sOrderType)
      {
          string sOrderString = "";
          if (sOrderColumn != "")
          {
              sOrderString = " ORDER BY " + sOrderColumn;
              if (sOrderType.ToLower() == "desc")
              {
                  sOrderString += " DESC ";
              }
              else
              {
                  sOrderString += " ASC ";
              }
          }
          sFilter = " 1=1 " + sFilter;
          string strSql = "SELECT ID,Flow,Subject,Year,RegionCode,Scale,LayerShortName,LayerCaseKey,LayerPlotKey,TableCaseKey,TablePlotKey,TableName,TableFields,LayerFields,SolutionName FROM INPUTCONFIGINFO t WHERE 1=1 " + sFilter + sOrderString;
          if (iPageSize > 0)
          {
              strSql = iDb.GetSqlForPageSize("INPUTCONFIGINFO", "flow", iPageSize, iPageIndex, sFilter, sOrderString);
          }
          DataSet ds = iDb.GetDataSet(strSql);
          return ds;
      }

      #region 入库功能
        
      /// <summary>
      ///获取要入库的字段和值
      /// </summary>
      /// <param name="sCaseNo">案卷编号</param>
      ///  <param name="sLandNo">地块编号</param>
      /// <param name="sFlow">流程</param>
      /// <param name="LayerFields">图层中的字段，以逗号间隔</param>
      /// <param name="LayerValues">对应图层字段的值，以逗号间隔</param>
      /// <returns>如果没有错误，返回""</returns>
      /// 
        /*
      private string GetFieldValue(GS.DataBase.IDbAccess iDb, string sCaseNo, string sLandNo, Models.InputConfigInfo info, out string LayerFields, out string LayerValues)
      {
          string sMsg = "";
          LayerFields = "";
          LayerValues = "";

          if (!String.IsNullOrEmpty(info.TableFields) && !String.IsNullOrEmpty(info.TableName) && !String.IsNullOrEmpty(info.TableCaseKey) && !String.IsNullOrEmpty(info.LayerFields))
          {
              LayerFields = info.LayerFields; //图形的字段直接从xml配置中获取，对应的值从表中查



              string sSql = string.Format("select {0} from {1} where {2}='{3}'", info.TableFields, info.TableName, info.TableCaseKey, sCaseNo);

              //if (!String.IsNullOrEmpty(sLandNo))
              //{
              //    //如果地块编号不为空，就添加上地块编号的查询条件
              //    sSql += string.Format(" and {0}='{1}'", info.TablePlotKey, sLandNo);
              //}


              Common.Log.Write(sSql);
              DataSet ds = iDb.GetDataSet(sSql);
              if (ds != null && ds.Tables[0].Rows.Count > 0)
              {
                  for (int i = 0, maxI = info.TableFields.Split(',').Length; i < maxI; i++)
                  {
                      LayerValues += ds.Tables[0].Rows[0][i] + ",";  //如果有多条记录，就取第一条的各个字段
                  }
                  LayerValues = LayerValues.Substring(0, LayerValues.Length - 1); //去掉最后一个逗号，不要用TrimEnd,防止有字段值为""
              }
              else
              {
                  sMsg = string.Format("表{0}没有查出入库的值", info.TableName);
                  Common.Log.Write(sMsg);
                  return sMsg;
              }

              //添加地块编号
              if (!String.IsNullOrEmpty(info.LayerPlotKey) || info.SolutionName == "两矿")
              {
                  LayerFields += "," + info.LayerPlotKey;
                  LayerValues += "," + sLandNo;
              }

          }
          else
          {
              sMsg = string.Format("流程{0}配置的信息不完整，请配置3。", info.Flow);
             Common.Log.Write(sMsg);
              return sMsg;
          }
          return sMsg; //如果没有错误，返回""
      }
    */

      #region 获取入库的对应字段和值

      /// <summary>
      ///获取要入库的字段和值
      ///注意：以前通过返回使用逗号间隔的字符串，然后split转为数组，但是从数据库中取的值中有可能有逗号，因此改为返回out List<string>类型。
      /// </summary>
      /// <param name="sCaseNo">案卷编号</param>
      ///  <param name="sLandNo">地块编号</param>
      /// <param name="sFlow">流程</param>
      /// <param name="LayerFields">图层中的字段，以逗号间隔</param>
      /// <param name="LayerValues">对应图层字段的值，以逗号间隔</param>
      /// <returns>如果没有错误，返回""</returns>
      private string GetFieldValue(GS.DataBase.IDbAccess iDb, string sCaseNo, string sLandNo, Models.InputConfigInfo info, out List<string> listLayerFields, out List<string> listLayerValues)
      {
          string sMsg = "";
          listLayerFields = new List<string>();
          listLayerValues = new List<string>();

          if (!String.IsNullOrEmpty(info.TableFields) && !String.IsNullOrEmpty(info.TableName) && !String.IsNullOrEmpty(info.TableCaseKey) && !String.IsNullOrEmpty(info.LayerFields))
          {

              string sSql = string.Format("select {0} from {1} where {2}='{3}'", info.TableFields, info.TableName, info.TableCaseKey, sCaseNo);


              Log.Write(sSql);
              DataSet ds = iDb.GetDataSet(sSql);
              if (ds != null && ds.Tables[0].Rows.Count > 0)
              {
                  string[] arrTableFields = info.TableFields.Split(',');
                  string sValue = ""; // 临时存值
                  for (int i = 0, maxI = arrTableFields.Length; i < maxI; i++)
                  {
                      listLayerFields.Add(arrTableFields[i]);
                      sValue = ds.Tables[0].Rows[0][i] == null ? "" : ds.Tables[0].Rows[0][i].ToString();
                      listLayerValues.Add(sValue); //如果有多条记录，就取第一条的各个字段                       
                  }
              }
              else
              {
                  sMsg = string.Format("表{0}没有查出入库的值", info.TableName);
                  Log.Write(sMsg);
                  return sMsg;
              }

              //如果配置了图层中地块关键字，添加地块编号
              if (!String.IsNullOrEmpty(info.LayerPlotKey) || info.SolutionName == "两矿")
              {
                  listLayerFields.Add(info.LayerPlotKey);
                  listLayerValues.Add(sLandNo);
              }

          }
          else
          {
              sMsg = string.Format("流程{0}配置的信息不完整，请配置（GetFieldValue）。", info.Flow);
              Log.Write(sMsg);
              return sMsg;
          }
          return sMsg; //如果没有错误，返回""
      }
      #endregion



      /// <summary>
      /// 得到入库时需要的strInputAtt，这个参数指明了要入库的图层，如果信息不精确，可能入库到多个图层
      /// </summary>
      /// <param name="sFlow"></param>
      /// <returns></returns>
      /// 
      private string GetInputAtt(Models.InputConfigInfo info)
      {
          string strInputAtt = "";
          ///<summary>
          ///采矿权判断
          ///<summary>
          if (info.SolutionName != "两矿")
          {
              if (!String.IsNullOrEmpty(info.Subject))
              {
                  strInputAtt += string.Format("subjectType={0}&", info.Subject);
              }
              if (!String.IsNullOrEmpty(info.Year))
              {
                  strInputAtt += string.Format("year={0}&", info.Year);
              }
              if (!String.IsNullOrEmpty(info.Scale))
              {
                  strInputAtt += string.Format("scale={0}&", info.Scale);
              }
              //不取行政区代码，加上这个后，删除图层和添加都报错。
              //if (!String.IsNullOrEmpty(info.RegionCode))
              //{
              //    strInputAtt += string.Format("regionCode={0}&", info.RegionCode);
              //}
              if (!String.IsNullOrEmpty(info.LayerShortName))
              {
                  strInputAtt += string.Format("layerShortName={0}&", info.LayerShortName);
              }
              strInputAtt = strInputAtt.TrimEnd('&');
              //return strInputAtt;
          }
          else
          {
              if (!String.IsNullOrEmpty(info.Subject))
              {
                  strInputAtt += string.Format("subjectType={0}&", info.Subject);
              }
              //不取行政区代码，加上这个后，删除图层和添加都报错。
              if (!String.IsNullOrEmpty(info.RegionCode))
              {
                  strInputAtt += string.Format("regionCode={0}&", info.RegionCode);
              }
              if (!String.IsNullOrEmpty(info.LayerShortName))
              {
                  strInputAtt += string.Format("layerShortName={0}&", info.LayerShortName);
              }
              strInputAtt = strInputAtt.TrimEnd('&');
              //return strInputAtt;
          }
          return strInputAtt;
          /*
          if (!String.IsNullOrEmpty(info.Subject))
          {
              strInputAtt += string.Format("subjectType={0}&", info.Subject);
          }
          if (!String.IsNullOrEmpty(info.Year))
          {
              strInputAtt += string.Format("year={0}&", info.Year);
          }
          if (!String.IsNullOrEmpty(info.Scale))
          {
              strInputAtt += string.Format("scale={0}&", info.Scale);
          }
          //不取行政区代码，加上这个后，删除图层和添加都报错。
          //if (!String.IsNullOrEmpty(info.RegionCode))
          //{
          //    strInputAtt += string.Format("regionCode={0}&", info.RegionCode);
          //}
          if (!String.IsNullOrEmpty(info.LayerShortName))
          {
              strInputAtt += string.Format("layerShortName={0}&", info.LayerShortName);
          }
          strInputAtt = strInputAtt.TrimEnd('&');
          return strInputAtt;
           */
      }


      /// <summary>
      /// 入库
      /// </summary>
      /// <param name="iDb"></param>
      /// <param name="sCaseNo">案卷编号</param>
      /// <param name="sFlow">流程</param>
      /// <returns>入库结果</returns>
      /// 


              /// <summary>
      /*
      public  string InputNew(GS.DataBase.IDbAccess iDb, string sCaseNo, string sFlow)
      {
          string sMsg = "";
          Common.Log.Write("Input开始执行......");
          Models.InputConfigInfo info = new Models.InputConfigInfo();
          info = info.Search(iDb, sFlow);
          //下面两个参数，入库公用
          string strSolutionName = info.SolutionName;
          string strInputAtt = GetInputAtt(info);
          Common.Log.Write("strInputAtt=" + strInputAtt);
          if (strInputAtt == "")
          {
              sMsg = string.Format("流程{0}配置的信息不完整，请配置2。", sFlow);
              return sMsg;
          }

          bool bSuccess = true;

          //先删除原来图层的信息

          string sWhere = string.Format(" {0}='{1}' ", info.LayerCaseKey, sCaseNo);
          bSuccess = WebGis.WebGisBase.DelFeatureNew(strSolutionName, strInputAtt, sWhere);             
          if (bSuccess == false)
          {
              Log.Write("删除图层中信息失败。");
          }

          //下面按照地块编号获取其它参数，分别入库，最后提示成功入库地块个数。
          ArrayList plotList = Models.DKZBXX.GetPlotNo(iDb, sCaseNo); //获取所有的地块编号
          string sPoint = "";
          string LayerFields = "";
          string LayerValues = "";
        
          int iSucCnt = 0; //记录入库成功的地块个数

          for (int i = 0, maxI = plotList.Count; i < maxI; i++)
          {
              string sPlotNo = plotList[i].ToString();
              sPoint = Models.DKZBXX.GetPointForHuan(iDb, sPlotNo);
              GetFieldValue(iDb, sCaseNo, sPlotNo, info, out List<string> LayerFields, out List<string> LayerValues);

              Log.Write("入库参数LayerFields=" + LayerFields + ",LayerValues=" + LayerValues + ",strInputAtt=" + strInputAtt);
              ///<summary>
              ///修改采矿权入库
              ///<summary>
              bSuccess = WebGis.WebGisBase.AddFeatureNew(strSolutionName, strInputAtt, sPoint, LayerFields.Split(','), LayerValues.Split(','));
              ///*
               * if(strSolutionName!="CKQ")
              {
                  bSuccess = WebGis.WebGisBase.AddFeatureNew(strSolutionName, strInputAtt, sPoint, LayerFields.Split(','), LayerValues.Split(','));
              }
              else
              {
                  bSuccess = WebGis.WebGisBase.AddFeatureNew(strSolutionName, strInputAtt, sPoint, LayerFields.Split(','), LayerValues.Split(','));
              }
              bSuccess = WebGis.WebGisBase.AddFeatureNew(strSolutionName, strInputAtt, sPoint, LayerFields.Split(','), LayerValues.Split(','));
              // */
        /*
              iSucCnt = (bSuccess == true) ? (iSucCnt + 1) : iSucCnt;
          }
          if (plotList.Count == iSucCnt)
          {
              sMsg = string.Format("案卷{0}关联的{1}个地块入库全部成功", sCaseNo, plotList.Count);
          }
          else
          {
              sMsg = string.Format("案卷{0}关联的{1}个地块入库成功{2}个", sCaseNo, plotList.Count, iSucCnt);
          }

          return sMsg;
      }
        */


      #endregion



      #region 入库

      /// <summary>
      /// 入库
      /// </summary>
      /// <param name="iDb"></param>
      /// <param name="sCaseNo">案卷编号</param>
      /// <param name="sFlow">流程</param>
      /// <returns>入库结果</returns>
      public string InputNew(GS.DataBase.IDbAccess iDb, string sCaseNo, string sFlow)
      {
          string sMsg = "";
          Log.Write("Input开始执行......");
          Models.InputConfigInfo info = new Models.InputConfigInfo();
          info = info.Search(iDb, sFlow);



          //下面两个参数，入库公用
          string strSolutionName = info.SolutionName;
          string strInputAtt = GetInputAtt(info);
          Log.Write("strInputAtt=" + strInputAtt);
          if (strInputAtt == "")
          {
              sMsg = string.Format("流程{0}配置的信息不完整，请配置。", sFlow);
              return sMsg;
          }

          bool bSuccess = true;

          //先删除原来图层的信息

          string sWhere = string.Format(" {0}='{1}' ", info.LayerCaseKey, sCaseNo);
          bSuccess = WebGis.WebGisBase.DelFeatureNew(strSolutionName, strInputAtt, sWhere);
          if (bSuccess == false)
          {
              Log.Write("删除图层中信息失败。");
          }

          //下面按照地块编号获取其它参数，分别入库，最后提示成功入库地块个数。
          ArrayList plotList = Models.DKZBXX.GetPlotNo(iDb, sCaseNo); //获取所有的地块编号
          string sPoint = "";
          List<string> listLayerFields = new List<string>();
          List<string> listLayerValues = new List<string>();

          int iSucCnt = 0; //记录入库成功的地块个数

          for (int i = 0, maxI = plotList.Count; i < maxI; i++)
          {
              string sPlotNo = plotList[i].ToString();
              sPoint = Models.DKZBXX.GetPointForHuan(iDb, sPlotNo);
              GetFieldValue(iDb, sCaseNo, sPlotNo, info, out listLayerFields, out listLayerValues);

              Log.Write("入库参数LayerFields=" + listLayerFields.ToString() + ",LayerValues=" + listLayerValues.ToString() + ",strInputAtt=" + strInputAtt);

              bSuccess = WebGis.WebGisBase.AddFeatureNew(strSolutionName, strInputAtt, sPoint, listLayerFields.ToArray(), listLayerValues.ToArray());
              if (bSuccess == true)
              {

                  iSucCnt = iSucCnt + 1;
              }
          }
          if (plotList.Count == iSucCnt)
          {
              sMsg = string.Format("案卷{0}关联的{1}个地块入库全部成功", sCaseNo, plotList.Count);
              Log.Write(sMsg);
              sMsg = "入库成功";
          }
          else
          {
              sMsg = string.Format("案卷{0}关联的{1}个地块入库成功{2}个", sCaseNo, plotList.Count, iSucCnt);
              Log.Write(sMsg);
              sMsg = "入库失败";
          }

          return sMsg;
      }

      #endregion

    }
}