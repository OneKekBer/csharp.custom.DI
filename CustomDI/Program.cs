using System.Diagnostics;
using System.Threading.Channels;
using CustomDI.Components;

var stop = Stopwatch.StartNew();

var container = new DependencyContainer();
var resolver = new DependencyProvider(container);

container.AddTransient<Service1>();
container.AddTransient<Service2>();
container.AddTransient<Service3>();
container.AddTransient<Service4>();


var service3 = resolver.GetService<Service4>();

service3.DoWork();
stop.Stop();
Console.WriteLine(stop.Elapsed);

container.AddSingleton<CounterService>();
container.AddTransient<StatelessService>();

var counter1 = resolver.GetService<CounterService>();
var counter2 = resolver.GetService<CounterService>();

Console.WriteLine("Counter 1: " + counter1.Increment()); // wait 1
Console.WriteLine("Counter 2: " + counter2.Increment()); // wait 2

var stateless1 = resolver.GetService<StatelessService>();
var stateless2 = resolver.GetService<StatelessService>();

stateless1.DoWork(); 
stateless2.DoWork(); 


public class CounterService
{
    private int _counter = 0;
    
    public int Increment()
    {
        return ++_counter;
    }
}

class StatelessService
{
    public void DoWork()
    {
        Console.WriteLine($"StatelessService: {Guid.NewGuid()}");
    }
}

class Service1
{
    public void DoWork()
    {
        Console.WriteLine("Service1");
    }
}

class Service2
{
    private readonly Service1 _service1;
    public Service2(Service1 service1)
    {
        _service1 = service1;
    }

    public void DoWork()
    {
        _service1.DoWork();
        Console.WriteLine("Service2");
    }
}

class Service3
{
    private readonly Service1 _service1;
    private readonly Service2 _service2;


    public Service3(Service1 service1, Service2 service2)
    {
        _service1 = service1;
        _service2 = service2;
    }

    public void DoWork()
    {
        _service1.DoWork();
        _service2.DoWork();
        Console.WriteLine("Service3");
    }
}

class Service4
{
    private readonly Service1 _service1;
    private readonly Service2 _service2;
    private readonly Service3 _service3;

    public Service4(Service1 service1, Service2 service2, Service3 service3)
    {
        _service1 = service1;
        _service2 = service2;
        _service3 = service3;
    }

    public void DoWork()
    {
        _service1.DoWork();
        _service2.DoWork();
        _service3.DoWork();
        Console.WriteLine("Service4");
    }
}
public class SharedCounterService
{
    private int _counter = 0;

    // This method is not thread-safe
    public int Increment()
    {
        // Simulate some work
        Thread.Sleep(10);
        return ++_counter;
    }

    public int GetCounterValue()
    {
        return _counter;
    }
}

public class ThreadUnsafeService
{
    private readonly List<int> _sharedList = new List<int>();

    // Simulating a thread-unsafe operation
    public void AddToSharedList(int value)
    {
        // Simulate some work
        Thread.Sleep(10);
        _sharedList.Add(value);  // No synchronization (thread-safety issue)
    }

    public int GetSharedListCount()
    {
        return _sharedList.Count;
    }
}
