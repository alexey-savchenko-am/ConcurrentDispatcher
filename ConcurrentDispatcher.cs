using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Default
{
    public class ConcurrentDispatcher<T>
    {

        public int DispatcherCount
        {
            get => _dispatcherCount;
        }
        
        public bool IsWorking
        {
            get => _running == 1;
        }
        
        private volatile ConcurrentQueue<QueuedObject> _queue =
            new ConcurrentQueue<QueuedObject>();

        private volatile int _running = 0;

        private volatile int _dispatcherCount = 0;
        
        
        public void Dispatch(T obj, Func<T, Task> handler)
        {
            var runningTmp = _running;
            
            Interlocked.Increment(ref _dispatcherCount);
            
            _queue.Enqueue(new QueuedObject(obj, handler));

            if (Interlocked.CompareExchange(ref _running, 1, runningTmp) == 0)
            {
                Task.Run(StartProcessor);
            }
        }

        private void StartProcessor()
        {
            SpinWait wait = new SpinWait();
            
            while (true)
            {
                if (!_queue.TryDequeue(out var queuedObj))
                {
                    var runningTmp = _running;
                    
                    if (Interlocked.CompareExchange(ref _running, 0, runningTmp) == 1)
                        return;
                    
                    continue;
                }

                try
                {
                    queuedObj.Handler(queuedObj.Obj);
                }
                finally
                {
                    Interlocked.Decrement(ref _dispatcherCount);
                }
                
                wait.SpinOnce();
            }
        }
        
        class QueuedObject
        {
            public QueuedObject(T obj, Func<T, Task> handler)
            {
                Obj = obj;
                Handler = handler;
            }

            internal T Obj { get; }

            internal Func<T, Task> Handler { get; }
        }
        
    }
}
