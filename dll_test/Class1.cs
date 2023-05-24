using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dll_test
{
    using System;
    // dll type sample
    public class Class1
    {
        public void Output(string s)
        {
            Console.WriteLine(s);
        }
        public int method1(string input_str, out string output_str)
        {
            Console.WriteLine("input: {0}", input_str);
            output_str = input_str;
            return 3;
        }
        public int method2(string input_str, ref string output_str)
        {
            Console.WriteLine("input: {0}", input_str);
            output_str = input_str;
            return 3;
        }
    }
}
