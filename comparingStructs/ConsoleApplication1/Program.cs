using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    struct A
    {
        public string Name { get; set; }
        public Inner Inner { get; set; }
    }

    struct B
    {
        public string Name { get; set; }
        public Inner Inner { get; set; }
    }

    struct Inner
    {
        public int Int { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var s1 = new A { Name = "a", Inner = new Inner { Int = 0 } };
            var s3 = new A { Name = "a", Inner = new Inner { Int = 1 } };
            Console.WriteLine((s1.Equals(s3)).ToString());


            Console.ReadKey();
        }
    }
}
