// See https://aka.ms/new-console-template for more information


using CustomDI.Components;

var container = new DependencyContainer();
var resolver = new DependencyProvider(container);


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

public class StatelessService
{
    public void DoWork()
    {
        Console.WriteLine($"StatelessService: {Guid.NewGuid()}");
    }
}

