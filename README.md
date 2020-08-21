# ConcurrentDispatcher
Represents a variant of thread-safe dispatcher based on FIFO principle.

Put data which should be processed to dispatcher and specify handler.

Example:

```
ConcurrentDispatcher<int> _dispatcher = new ConcurrentDispatcher<int>();
 
public void DispatchItem()
{
   // Imitate range of tasks from separate threads
   Enumerable
       .Range(1, 100)
       .Select(x => Task.Run(
          () => _dispatcher.Dispatch(x, Handler)
        ));
            
}

public async Task Handler(int val)
{
   // process data...
}
        
```
