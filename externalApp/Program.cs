using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace externalApp
{
    internal class Program
    {
        static void Main(string[] args)
        {

            while(true)
            {
                string input = Console.ReadLine();
                if (input == null) continue;
                if(input.Equals("makeRestThread"))
                {
                    string input2 = Console.ReadLine();
                    //Console.WriteLine("process num{0}", args[0]);
                    Console.WriteLine("proc: {0} create thread done: {1}", args[0], input2);
                }
                //Console.WriteLine("process num{0}", args[0]);
                //Console.WriteLine(input);
            }
        }
    }
}
