using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InputTextDotString.Models
{
    public class ConvertDots
    {
        //package tms.base.systemlib;
        //@SuppressWarnings("unchecked")
        public class GaussXYDeal
        {
            #region 高斯坐标转换为经纬度
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
                ZoneWide = 6; ////6度带宽
                ProjNo = (int)(X / 1000000L); //查找带号
                longitude0 = (ProjNo - 1) * ZoneWide + ZoneWide / 2;
                longitude0 = longitude0 * iPI; //中央经线


                X0 = ProjNo * 1000000L + 500000L;
                Y0 = 0;
                xval = X - X0; yval = Y - Y0; //带内大地坐标
                e2 = 2 * f - f * f;
                e1 = (1.0 - Math.Sqrt(1 - e2)) / (1.0 + Math.Sqrt(1 - e2));
                ee = e2 / (1 - e2);
                M = yval;
                u = M / (a * (1 - e2 / 4 - 3 * e2 * e2 / 64 - 5 * e2 * e2 * e2 / 256));
                fai = u + (3 * e1 / 2 - 27 * e1 * e1 * e1 / 32) * Math.Sin(2 * u) + (21 * e1 * e1 / 16 - 55 * e1 * e1 * e1 * e1 / 32) * Math.Sin(
                4 * u)
                + (151 * e1 * e1 * e1 / 96) * Math.Sin(6 * u) + (1097 * e1 * e1 * e1 * e1 / 512) * Math.Sin(8 * u);
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
            #endregion

            #region 经纬度转化为高斯克吕格

            public static double[] GaussToBLToGauss(double longitude, double latitude)
            {
                int ProjNo=0; int ZoneWide; ////带宽
                double[] output = new double[2];
                double longitude1, latitude1, longitude0, X0, Y0, xval, yval; //latitude0;
                double a,f, e2,ee, NN, T,C,A, M, iPI;
                iPI = 0.0174532925199433; ////3.1415926535898/180.0;
                //ZoneWide = 6; ////6度带宽
                ZoneWide = 3;////3度带宽
                //a=6378245.0; f=1.0/298.3; //54年北京坐标系参数
                a=6378140.0; f=1/298.257; //80年西安坐标系参数
                ProjNo = (int)(longitude / ZoneWide) ;
                longitude0 = ProjNo * ZoneWide + ZoneWide / 2;
                longitude0 = longitude0 * iPI ;
                //latitude0 = 0;
                //System.out.println(latitude0);
                longitude1 = longitude * iPI ; //经度转换为弧度
                latitude1 = latitude * iPI ; //纬度转换为弧度
                e2=2*f-f*f;
                ee=e2*(1.0-e2);
                NN=a/Math.Sqrt(1.0-e2*Math.Sin(latitude1)*Math.Sin(latitude1));
                T=Math.Tan(latitude1)*Math.Tan(latitude1);
                C=ee*Math.Cos(latitude1)*Math.Cos(latitude1);
                A=(longitude1-longitude0)*Math.Cos(latitude1);
                M=a*((1-e2/4-3*e2*e2/64-5*e2*e2*e2/256)*latitude1-(3*e2/8+3*e2*e2/32+45*e2*e2
                *e2/1024)*Math.Sin(2*latitude1)
                +(15*e2*e2/256+45*e2*e2*e2/1024)*Math.Sin(4*latitude1)-(35*e2*e2*e2/3072)*Math.Sin(6*latitude1));
                xval = NN*(A+(1-T+C)*A*A*A/6+(5-18*T+T*T+72*C-58*ee)*A*A*A*A*A/120);
                yval = M+NN*Math.Tan(latitude1)*(A*A/2+(5-T+9*C+4*C*C)*A*A*A*A/24
                +(61-58*T+T*T+600*C-330*ee)*A*A*A*A*A*A/720);
                X0 = 1000000L*(ProjNo+1)+500000L;
                Y0 = 0;
                xval = xval+X0; yval = yval+Y0;
                //*X = xval;
                //*Y = yval;
                output[0] = xval;
                output[1] = yval;
                return output;
                //System.out.println("x："+xval);
                //System.out.println("y："+yval);
            }
            #endregion

        }


        #region 坐标转经纬度代码
        /* 功能说明： 将绝对高斯坐标(y,x)转换成绝对的地理坐标(wd,jd)。        */
        // double y;     输入参数: 高斯坐标的横坐标，以米为单位 
        // double x;  输入参数: 高斯坐标的纵坐标，以米为单位
        // short  DH;     输入参数: 带号，表示上述高斯坐标是哪个带的
        // double *L;     输出参数: 指向经度坐标的指针，其中经度坐标以秒为单位
        // double *B;     输出参数: 指向纬度坐标的指针，其中纬度坐标以秒为单位
        public static string[] GaussToGeo(double y, double x, short DH, double L, double B, double LP)
        {
            try
            {
                double l0;    //  经差
                double tf;    //  tf = tg(Bf0),注意要将Bf转换成以弧度为单位
                double nf;    //  n = y * sqrt( 1 + etf ** 2) / c, 其中etf = e'**2 * cos(Bf0) ** 2
                double t_l0;   //  l0，经差，以度为单位
                double t_B0;   //  B0，纬度，以度为单位
                double Bf0;    //  Bf0
                double etf;    //  etf,其中etf = e'**2 * cos(Bf0) ** 2
                double X_3;
                double PI = 3.14159265358979;
                double b_e2 = 0.0067385254147;
                double b_c = 6399698.90178271;
                string[] GaussToGeo=new string[2];
                X_3 = x / 1000000.00 - 3;      // 以兆米（1000000）为单位
                // 对于克拉索夫斯基椭球，计算Bf0
                Bf0 = 27.11115372595 + 9.02468257083 * X_3 - 0.00579740442 * Math.Pow(X_3, 2) - 0.00043532572 * Math.Pow(X_3, 3) + 0.00004857285 * Math.Pow(X_3, 4) + 0.00000215727 * Math.Pow(X_3, 5) - 0.00000019399 * Math.Pow(X_3, 6);
                tf = Math.Tan(Bf0 * PI / 180);       //  tf = tg(Bf),注意这里将Bf转换成以弧度为单位
                etf = b_e2 * Math.Pow(Math.Cos(Bf0 * PI / 180), 2);   //  etf = e'**2 * cos(Bf) ** 2
                nf = y * Math.Sqrt(1 + etf) / b_c;     //  n = y * sqrt( 1 + etf ** 2) / c
                // 计算纬度，注意这里计算出来的结果是以度为单位的
                t_B0 = Bf0 - (1.0 + etf) * tf / PI * (90.0 * Math.Pow(nf, 2) - 7.5 * (5.0 + 3 * Math.Pow(tf, 2) + etf - 9 * etf * Math.Pow(tf, 2)) * Math.Pow(nf, 4) + 0.25 * (61 + 90 * Math.Pow(tf, 2) + 45 * Math.Pow(tf, 4)) * Math.Pow(nf, 6));
                // 计算经差，注意这里计算出来的结果是以度为单位的
                t_l0 = (180 * nf - 30 * (1 + 2 * Math.Pow(tf, 2) + etf) * Math.Pow(nf, 3) + 1.5 * (5 + 28 * Math.Pow(tf, 2) + 24 * Math.Pow(tf, 4)) * Math.Pow(nf, 5)) / (PI * Math.Cos(Bf0 * PI / 180));
                l0 = (t_l0 * 3600.0);       //  将经差转成秒
                if (LP == -1000)
                {
                    L = (double)((DH * 6 - 3) * 3600.0 + l0);  // 根据带号计算出以秒为单位的绝对经度，返回指针
                }
                else
                {
                    L = LP * 3600.0 + l0;  // 根据带号计算出以秒为单位的绝对经度，返回指针
                }
                //----------------------------------
                B = (double)(t_B0 * 3600.0);     //  将纬差转成秒，并返回指针

                GaussToGeo[0]=L.ToString();
                GaussToGeo[1]=B.ToString();
                MapgisEgov.AnalyInput.Common.Log.Write("经度：" + GaussToGeo[0] + "纬度：" + GaussToGeo[1]);
                return GaussToGeo;
            }
            catch(Exception oExcept)
            {
                string[] arrException=new string[1];
                arrException[0]=oExcept.Message;
                return arrException;
            }
        }
        /* 功能说明： （1）将地理坐标(wd,jd)转换成绝对的高斯坐标(y,x)
            （2）本函数支持基于六度带（或三度带）、克拉索夫斯基椭球进行转换                             */
        /* 适用范围： 本函数适用于将地球东半球中北半球（即东经0度到东经180度，北纬0度至90度）范围
            内所有地理坐标到高斯坐标的转换            */
        /* 使用说明： 调用本函数后返回的结果应在满足精度的条件下进行四舍五入      */
        // double jd;         输入参数: 地理坐标的经度，以秒为单位
        // double wd;         输入参数: 地理坐标的纬度，以秒为单位
        // short  DH;      输入参数: 三度带或六度带的带号
        /*  六度带(三度带)的带号是这样得到的：从东经0度到东经180度自西向东按每6度(3度)顺序编号
         (编号从1开始)，这个顺序编号就称为六度带(三度带)的带号。因此，六度带的带号的范围是1-30，
         三度带的带号的范围是1-60。
          如果一个点在图号为TH的图幅中，那麽该点所处的六度带的带号就可以这样得到：将该图号的
         第3、4位组成的字符串先转换成数字，再减去30。例如某点在图幅06490701中，该点所在的带号就
         是49-30，即19。
          如果调用本函数去进行一般的从地理坐标到基于六度带高斯坐标的变换（非邻带转换），则参
         数DH的选取按前一段的方法去确定。                
          如果调用本函数去进行基于六度带邻带转换，则参数DH的选取先按上述方法去确定，然后看是
         往前一个带还是后一个带进行邻带转换再确定是加1还是减1。         */
        void GeoToGauss(double jd, double wd, short DH, short DH_width, double y, double x, double LP)
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
            tcos = Math.Cos(wd_hd);        // 计算cosB
            // 计算克拉索夫斯基椭球中子午弧长X
            X = 111134.8611 / 3600.0 * wd - (32005.7799 * tsin + 133.9238 * Math.Pow(tsin, 3)
                 + 0.6976 * Math.Pow(tsin, 5) + 0.0039 * Math.Pow(tsin, 7)) * tcos;
            et2 = b_e2 * Math.Pow(tcos, 2);      //  et2 = (e' ** 2) * (cosB ** 2)
            N = b_c / Math.Sqrt(1 + et2);      //  N = C / sqrt(1 + et2)
            t = Math.Tan(wd_hd);         //  t=tgB
            m = PI / 180 * l0 * tcos;       //  m = cosB * PI/180 * l0 
            x = X + N * t * (0.5 * Math.Pow(m, 2)
                     + (5.0 - Math.Pow(t, 2) + 9.0 * et2 + 4 * Math.Pow(et2, 2)) * Math.Pow(m, 4) / 24.0
                     + (61.0 - 58.0 * Math.Pow(t, 2) + Math.Pow(t, 4)) * Math.Pow(m, 6) / 720.0);
            y = N * (m + (1.0 - Math.Pow(t, 2) + et2) * Math.Pow(m, 3) / 6.0
                           + (5.0 - 18.0 * Math.Pow(t, 2) + Math.Pow(t, 4) + 14.0 * et2
                              - 58.0 * et2 * Math.Pow(t, 2)) * Math.Pow(m, 5) / 120.0);
        }


        #endregion
    }
}