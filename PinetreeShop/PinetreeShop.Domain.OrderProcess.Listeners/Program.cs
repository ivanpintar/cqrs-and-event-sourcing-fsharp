using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PinetreeShop.Domain.OrderProcess.Listeners
{
    class Program
    {
        static void Main(string[] args)
        {
            var orderProcessEventHandler = new EventListener();

            while (true)
            {
                orderProcessEventHandler.ProcessEvents();
            }
        }
    }
}
