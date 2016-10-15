//using System;
//using System.Collections.Generic;
//using System.Linq;

//namespace PinetreeShop.CQRS.Infrastructure.Commands
//{
//    public class CommandQueueListener
//    {
//        private Dictionary<string, ICommandDispatcher> _commandDispatchers;
//        private IEventStore _eventStore;

//        public CommandQueueListener(IEventStore eventStore)
//        {
//            _eventStore = eventStore;
//        }

//        public void DequeueAndDispatchCommand()
//        {
//            foreach (var queueName in _commandDispatchers.Keys)
//            {
//                var commands = _eventStore.DeQueueCommands(queueName);
//                foreach (var cmd in commands)
//                {
//                    _commandDispatchers[queueName].ExecuteCommand(cmd);
//                }
//            }
//        }
//    }
//}
