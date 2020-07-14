using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


// 속도 거리 구하는 class , 단위 변환 m/sec -> km/hour  이 자유로워야함.  
// 예상 도착시간 구하는 method, 평균 속도 구하는 method 필요

namespace ConsoleApplication_MyLibs
{

    class MyClass_SpeedCalc
    {
        public double getSpeed_m_sec(double start_distance, double end_distance, double start_time, double end_time)
        {
            double distanceInMeter = end_distance - start_distance;
            double timeInSecond = end_time - start_time;
            return distanceInMeter / timeInSecond;
        }
        public double getPredictionDistance(double average_speed, double start_time, double target_time, double start_pos)
        {
            return start_pos + average_speed * (target_time - start_time); // 예상 도착 지점 계산
        }

        static double sec_to_minute(double sec) // 초를 분으로 변경
        {
            return sec / 60;
        }
        static double sec_to_hour(double sec) //sec를 hour로 변경
        {
            return sec_to_minute(sec) / 60;
        }
        static double meter_to_kmeter(double meter) // m를 km로 변경
        {
            return meter / 1000;
        }

        static int kmph_to_mps(double kmph) // km/hour 를 m/sec로 변경
        {
            return (int)(0.277778 * kmph);
        }
        // function to convert speed 
        // in m/sec to km/hr 
        static int mps_to_kmph(double mps) // m/sec 를 km/hour로 변경
        {
            return (int)(3.6 * mps);
        }


    }
    // speedInKilometresPerHour
    //double speedInKmPerHour = distanceInKm / timeInHours;
    class MyClass_Math
    {
        // round 
        public double roundNumber()
        {
            double calc1 = 0.05d;
            // 소수 둘째자리 반올림
            double result = Math.Round(calc1, 1, MidpointRounding.AwayFromZero);
            return result;
        }
        // C 스타일 calc
        public int calcPercentage(double total, double used)
        {
            int calcResult = (int)(used / total);
            return calcResult;
        }
        // C# 스타일 calc
        public int calcPercentage2(double total, double used)
        {
            int calcResult = (int)Math.Truncate(used / total);
            return calcResult;
        }
  
        public static void testClass()
        {
            double log_e = Math.E; //자연로그 e
            double pi = Math.PI; //원주율 pi
            double x = 4;
            double y = 10;
            double res = Math.Log(x, y);//y를 밑으로 하는 x에 대한 로그값을 반환합니다.
            res = Math.Log(x);//e를 밑으로 하는 x에 대한 로그값을 반환합니다.
            res = Math.Log10(x);//10을 밑으로 하는 x에 대한 로그값을 반환합니다.

            res = Math.Pow(x, y); //x의 y승을 반환합니다.
            res = Math.Sqrt(x); //x의 제곱근을 반환합니다.

             x = 1.123;
            Math.Sign(x);// : x에 대해 부호를 반환합니다. 양수의 경우 1, 0의 경우 0, 음수의 경우 -1을 반환합니다.
            Math.Round(x);// : x에 대해 a 자릿수에서 반올림합니다.
            Math.Ceiling(x);// : x에 대해 올림합니다.
            Math.Floor(x);// : x에 대해 내림합니다.
            Math.Truncate(x);// : x에 대해 소수점을 제거합니다.
            Math.Abs(x);// : x에 대해 절대값을 취합니다.
            Math.Exp(x);// : 자연로그 e를 x만큼 거듭제곱합니다.
        }

    }
}
