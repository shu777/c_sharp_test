using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace ConsoleApplication_MyLibs
{
    class Program
    {
        static void Main(string[] args)
        {
            Project_example1.runExample1();
            Project_example2.runExample2();

            MyTest handle = new MyTest();
            handle.run_Test();
            
        }
    }
}
