// See https://aka.ms/new-console-template for more information


var container = new DependencyContainer();

// container.AddTransient<HelloService>();
// container.AddTransient<ProviderService>();
// container.AddTransient<WowService>();
// container.AddSingleton<MainService>();

var resolver = new DependencyProvider(container);

// var helloService = resolver.GetService<HelloService>();
// var providerService = resolver.GetService<ProviderService>();
// var mainService = resolver.GetService<MainService>();
//
// helloService.Print();
// providerService.Provide();
// mainService.Start();



// Регистрируем сервисы
container.AddSingleton<CounterService>();
container.AddTransient<StatelessService>();

var counter1 = resolver.GetService<CounterService>();
Console.WriteLine($"Singletons after first resolution: {string.Join(", ", container._singletons.Keys)}");

var counter2 = resolver.GetService<CounterService>();
Console.WriteLine($"Singletons after second resolution: {string.Join(", ", container._singletons.Keys)}");

Console.WriteLine("Counter 1: " + counter1.Increment()); // Ожидаем 1
Console.WriteLine("Counter 2: " + counter2.Increment()); // Ожидаем 2

// Проверяем Transient
var stateless1 = resolver.GetService<StatelessService>();
var stateless2 = resolver.GetService<StatelessService>();

stateless1.DoWork(); // Ожидаем новый экземпляр
stateless2.DoWork(); // Ожидаем другой новый экземпляр


public class DependencyProvider
{
    private readonly DependencyContainer _container;
    public DependencyProvider(DependencyContainer container)
    {
        _container = container;
    }

    private object GetInstance(Type type)
    {
        var constructor = type.GetConstructors().FirstOrDefault();
        if(constructor == null)
            throw new Exception($"Type '{type.FullName}' does not have a default constructor.");

        var parameters = constructor.GetParameters();
        
        var instances = parameters.Select(param =>
        {
            var parameterType = param.ParameterType;
            if (_container.IsSingletonAndExists(parameterType))
                return _container.GetSingleton(parameterType);
                    
            var instance = Activator.CreateInstance(parameterType);
            
            if(_container.IsSingleton(parameterType))
                _container.SaveSingleton(parameterType, instance);
            
            return instance;
        }).ToArray();
        
        return Activator.CreateInstance(type, instances);
    }
    
    public T GetService<T>()
    {
        var type = _container.GetType(typeof(T));

        if (_container.IsSingletonAndExists<T>())
            return _container.GetSingleton<T>();
        
        var constructor = type.GetConstructors().FirstOrDefault();
        if(constructor == null)
            throw new Exception($"Type '{type.FullName}' does not have a default constructor.");
        
        var parameters = constructor.GetParameters();
       
        var instances = parameters.Select(param => GetInstance(param.ParameterType)).ToArray();
        var service = (T)Activator.CreateInstance(type, instances);
        
        if(_container.IsSingleton(type))
            _container.SaveSingleton(typeof(T), service);
        return service;
    }
}

public class DependencyContainer // i think it`s bad, beacuse single responsibility 
{
    private readonly List<Type> _types = new List<Type>();
    public readonly Dictionary<Type, object> _singletons = new Dictionary<Type, object>();
    
    private void AddService<T>()
    {
        _types.Add(typeof(T));
    }

    public void AddTransient<T>()
    {
        AddService<T>();
    }
    
    public void AddSingleton<T>()
    {
        AddService<T>();
        _singletons.Add(typeof(T), null);
    }

    public bool IsSingleton<T>()
    {
        return _singletons.ContainsKey(typeof(T));
    }
    
    public bool IsSingleton(Type type)
    {
        return _singletons.ContainsKey(type);
    }
    
    public bool IsSingletonAndExists<T>()
    {
        return _singletons.ContainsKey(typeof(T)) && _singletons[typeof(T)] is not null;
    }
    
    public bool IsSingletonAndExists(Type type)
    {
        return _singletons.ContainsKey(type) && _singletons[type] is not null;
    }

    public void SaveSingleton(Type type, object instance)
    {
        Console.WriteLine($"Saving singleton for type {type.FullName}");
        _singletons[type] = instance;
    }
    
    public T GetSingleton<T>()
    {
        _singletons.TryGetValue(typeof(T), out var instance);
        return (T)instance;
    }
    
    public object GetSingleton(Type type)
    {
        _singletons.TryGetValue(type, out var instance);
        Console.WriteLine($"Retrieving singleton for type {type.FullName}: {instance}");
        return instance;
    }
    
    public Type GetType(Type type)
    {
        return _types.First(x=> x.Name == type.Name);
    }
    
    
}


public class MainService
{
    private readonly WowService _wowService;
    private readonly ProviderService _providerService;
    private readonly HelloService _helloService;
    
    public MainService(ProviderService providerService, WowService wowService, HelloService helloService)
    {
        _providerService = providerService;
        _wowService = wowService;
        _helloService = helloService;
    }

    public void Start()
    {
        _providerService.Provide();
        _wowService.Print();
        _helloService.Print();
    }
}

public class WowService
{
    public void Print()
    {
        Console.WriteLine("WOW!!");
    }
}
public class ProviderService
{
    private readonly HelloService _helloService;
    public ProviderService(HelloService helloService)
    {
        _helloService = helloService;
    }

    public void Provide()
    {
        _helloService.Print();
    }
}

public class HelloService
{
    public void Print()
    {
        Console.WriteLine("Hello, World!");
    }
}

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

