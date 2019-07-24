using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
