using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


// 속도 거리 구하는 class , 단위 변환 m/sec -> km/hour  이 자유로워야함.  
// 예상 도착시간 구하는 method, 평균 속도 구하는 method 필요

namespace ConsoleApplication_MyLibs
{
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

    }
}
