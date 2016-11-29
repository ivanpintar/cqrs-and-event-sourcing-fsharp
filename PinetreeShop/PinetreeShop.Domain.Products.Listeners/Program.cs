namespace PinetreeShop.Domain.Products.Listeners
{
    class Program
    {
        static void Main(string[] args)
        {
            var commandQueueListener = new CommandQueueListener();
            var readModelListener = new ReadModelListener();
            
            while(true)
            {
                commandQueueListener.ProcessCommands();
                readModelListener.ProcessEvents();
            }
        }
    }
}
