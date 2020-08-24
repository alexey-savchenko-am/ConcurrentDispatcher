using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace pipeline_client
{
    public class ContextBasedDispatcher<TContext, TItem>
    {
        private readonly ConcurrentDictionary<TContext, ConcurrentDispatcher<TItem>> _map
            = new ConcurrentDictionary<TContext, ConcurrentDispatcher<TItem>>();

        public void Dispatch(TContext context, TItem item, Func<TItem, Task> handler)
        {
            if (!_map.TryGetValue(context, out var dispatcher))
            {
                var newDispatcher = new ConcurrentDispatcher<TItem>();
                dispatcher = _map.GetOrAdd(context, newDispatcher);
            }
            
            dispatcher.Dispatch(item, handler);
        }
    }
}