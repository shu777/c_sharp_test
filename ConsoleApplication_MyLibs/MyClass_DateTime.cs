using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication_MyLibs
{
    //internal class MyClass_DateTime
    //{
    //}
    public class MyClass_DateTime
    {
        public MyClass_DateTime()
        {

        }
        ~MyClass_DateTime()
        {

        }
        // 일력예 "2022-07-10 13:11:11" //
        public static void convertStringToDateTime(string stringTime)
        {
            DateTime dt_Res = DateTime.ParseExact(stringTime, "yyyy-MM-dd HH:mm:ss", null);
        }
        public static string getTimeDifference(string start_time, string end_time)
        {
            DateTime dt_Res_start = DateTime.ParseExact(start_time, "yyy-MM-dd HH:mm:ss", null);
            DateTime dt_Res_end = DateTime.ParseExact(start_time, "yyy-MM-dd HH:mm:ss", null);
            TimeSpan res_span = dt_Res_end - dt_Res_start;
            return res_span.TotalSeconds.ToString(); // 초 차이 반환
            //res_span.TotalMinutes.ToString();   // 분 차이 반환
        }
        public static string getCurrentTimeString()
        {
            DateTime time_now = DateTime.Now;
            return time_now.ToString("yyyy/MM/dd HH:mm:ss.fff"); // fff -> msec
        }
    }
    }
