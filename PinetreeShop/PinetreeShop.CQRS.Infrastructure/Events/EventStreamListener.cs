//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace PinetreeShop.CQRS.Infrastructure.Events
//{
//    public class EventStreamListener
//    {
//        private int _lastEventId = 0;

//        private Dictionary<Type, object> _eventHandlers = new Dictionary<Type, object>();
//        private IEventStore _eventStore;

//        public EventStreamListener(IEventStore eventStore)
//        {
//            _eventStore = eventStore;
//        }

//        public void ReadAndHandleLatestEvents(string streamName)
//        {
//            var events = _eventStore.GetEvents(string streamName, int startingPoint)
//        }
//    }
//}
