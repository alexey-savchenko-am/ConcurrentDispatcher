# ConcurrentDispatcher
Represents a variant of thread-safe dispatcher based on FIFO principle.
Put data which should be processed to dispatcher and specify handler:

```
ConcurrentDispatcher dispatcher = new ConcurrentDispatcher<int>();
 
public void DispatchItem(int item)
{
    dispatcher.Dispatch(item, Handler);
}

public async Task Handler(int val)
{
   Console.WriteLine(val);
}
        
```
