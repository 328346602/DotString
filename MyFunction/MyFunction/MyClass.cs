using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web.SessionState;


namespace MyFunctionSamples
{

    public class 测试 : Visual_Form_Designer.Class.IFunction
    {
        private string Rev = "";
        private string ErrorMsg = "";
        public 测试() { }

        #region IFunction 成员

        public object ReturnValue
        {
            get
            {
                return Rev;
            }
        }

        public bool Exec(System.Web.UI.Page _Page, System.Web.HttpContext _Context, Visual_Form_Designer.Class.VFDServiceObject _Service, Visual_Form_Designer.Class.WebPageConfig _WebPageConfig, System.Collections.Hashtable ParamaterList, object _CustomObject)
        {

            return true;
        }

        /// <summary>
        /// 测试方法
        /// </summary>
        /// <param name="_Page">1</param>
        /// <param name="_Context">2</param>
        /// <param name="_Service">3</param>
        /// <param name="_WebPageConfig">4</param>
        /// <param name="ParamaterList">5</param>
        /// <param name="_CustomObject">6</param>
        /// <returns>7</returns>
        private bool test(System.Web.UI.Page _Page, System.Web.HttpContext _Context, Visual_Form_Designer.Class.VFDServiceObject _Service, Visual_Form_Designer.Class.WebPageConfig _WebPageConfig, System.Collections.Hashtable ParamaterList, object _CustomObject)
        {

            return true;
        }

        public string LastError
        {
            get
            {
                return ErrorMsg;
            }
        }

        #endregion

        public bool InputDots(System.Web.UI.Page _Page, System.Web.HttpContext _Context, Visual_Form_Designer.Class.VFDServiceObject _Service, Visual_Form_Designer.Class.WebPageConfig _WebPageConfig, System.Collections.Hashtable ParamaterList, object _CustomObject)
        {
            return false;
        }
    }


    public class 测试3
    {
        /// <summary>
        /// 测试方法3
        /// </summary>
        /// <param name="_Page"></param>
        /// <param name="_Context"></param>
        public void GetUserName(System.Web.UI.Page _Page,System.Web.HttpContext _Context)
        {
            string sUserID=_Context.User.Identity.Name;
            string sUserName="";
            string strSql=string.Format("select 名称 from FLOW_USER where 用户ID='{0}'",sUserID);
            Function.ShowMessage s = new Function.ShowMessage();
            s.Alert("测试成功！",_Page);
        }
    }

    



    /// <summary>
    /// 测试编写插件
    /// </summary>
    /// *
    /// 
    
    /*
    class MyClass
    {
        #region
        Dictionary<string,string> dc=new Dictionary<string,string>(32);
        Hashtable hstb = new Hashtable();
        public HashSet<string> hs = new HashSet<string>();
        public static void test()
        {
            Hashtable ht= new Hashtable();
            ht.Add("A", "1");
            ht.Add("B", "2");
            ht.Add("C", "3");
            ht.Add("D", "4");
            HashSet<string> hs = new HashSet<string>();
            DayOfWeek day = new DayOfWeek();
            string dyaOfWeek=day.ToString();
        }

        static void Main(string[] args)
        {
            // 创建一个Hashtable实例
            Hashtable ht = new Hashtable();

            // 添加keyvalue键值对
            ht.Add("A", "1");
            ht.Add("B", "2");
            ht.Add("C", "3");
            ht.Add("D", "4");

            // 遍历哈希表
            foreach (DictionaryEntry de in ht)
            {
                Console.WriteLine("Key -- {0}; Value --{1}.", de.Key, de.Value);
            }

            // 哈希表排序
            ArrayList akeys = new ArrayList(ht.Keys);
            akeys.Sort();
            foreach (string skey in akeys)
            {
                Console.WriteLine("{0, -15} {1, -15}", skey, ht[skey]);

            }

            // 判断哈希表是否包含特定键,其返回值为true或false
            if (ht.Contains("A"))
                Console.WriteLine(ht["A"]);

            // 给对应的键赋值
            ht["A"] = "你好";

            // 移除一个keyvalue键值对
            ht.Remove("C");

            // 遍历哈希表
            foreach (DictionaryEntry de in ht)
            {
                Console.WriteLine("Key -- {0}; Value --{1}.", de.Key, de.Value);
            }

            // 移除所有元素
            ht.Clear();

            // 此处将不会有任何输出
            Console.WriteLine(ht["A"]);
            Console.ReadKey();
        }
        #endregion


        

    }

    public class Function : Visual_Form_Designer.Class.IFunction
    {
        private string Rev = "";
        private string ErrorMsg = "";
        public Function() { }
        #region IFunction 成员

        public object ReturnValue
        {
            get
            {
                return Rev;
            }
        }

        public bool Exec(System.Web.UI.Page _Page, System.Web.HttpContext _Context, Visual_Form_Designer.Class.VFDServiceObject _Service, Visual_Form_Designer.Class.WebPageConfig _WebPageConfig, System.Collections.Hashtable ParamaterList, object _CustomObject)
        {

            return true;
        }



        public string LastError
        {
            get
            {
                return ErrorMsg;
            }
        }

        #endregion
    }

    public class 测试 : Visual_Form_Designer.Class.IFunction
    {
        private string Rev = "";
        private string ErrorMsg = "";
        public 测试() { }

        #region IFunction 成员

        public object ReturnValue
        {
            get
            {
                return Rev;
            }
        }

        public bool Exec(System.Web.UI.Page _Page, System.Web.HttpContext _Context, Visual_Form_Designer.Class.VFDServiceObject _Service, Visual_Form_Designer.Class.WebPageConfig _WebPageConfig, System.Collections.Hashtable ParamaterList, object _CustomObject)
        {

            return true;
        }



        public string LastError
        {
            get
            {
                return ErrorMsg;
            }
        }

        #endregion
    }

    **/
}
