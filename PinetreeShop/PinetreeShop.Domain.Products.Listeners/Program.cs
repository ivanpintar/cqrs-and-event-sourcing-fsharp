using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PinetreeShop.CQRS.Infrastructure.Commands;
using System.Reflection;

namespace PinetreeShop.Domain.Products.Listeners
{
    class Program
    {
        static void Main(string[] args)
        {
            var commandQueueListener = new CommandQueueListeners();
            var readModelListener = new ReadModelListeners();
            
            while(true)
            {
                commandQueueListener.ProcessCommands();
                readModelListener.ProcessEvents();
            }
        }
    }
}
