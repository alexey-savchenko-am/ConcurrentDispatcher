using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Dispatcher
{
    public class ContextBasedDispatcher<TContext, TItem>
    {

        public int ContextDispatcherCount
        {
            get => _contextDispatcherCount;
        }

        public bool IsWorking
        {
            get => _map.Values.Any(x => x.IsWorking);
        }

        private volatile int _contextDispatcherCount = 0;

        private readonly ConcurrentDictionary<TContext, ConcurrentDispatcher<TItem>> _map
            = new ConcurrentDictionary<TContext, ConcurrentDispatcher<TItem>>();

        public void Dispatch(TContext context, TItem item, Func<TItem, Task> handler)
        {
            if (!_map.TryGetValue(context, out var dispatcher))
            {
                var newDispatcher = new ConcurrentDispatcher<TItem>();
                
                dispatcher = _map.GetOrAdd(context, newDispatcher);
               
                if (dispatcher == newDispatcher)
                {
                    Interlocked.Increment(ref _contextDispatcherCount);
                }
       
            }
            
            dispatcher.Dispatch(item, handler);
        }
    }
}