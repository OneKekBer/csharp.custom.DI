// See https://aka.ms/new-console-template for more information


var container = new DependencyContainer();

container.AddTransient<HelloService>();
container.AddTransient<ProviderService>();
container.AddTransient<WowService>();
container.AddSingleton<MainService>();

var resolver = new DependencyProvider(container);

var helloService = resolver.GetService<HelloService>();
var providerService = resolver.GetService<ProviderService>();
var mainService = resolver.GetService<MainService>();

helloService.Print();
providerService.Provide();

mainService.Start();

public class DependencyProvider
{
    private readonly DependencyContainer _container;
    public DependencyProvider(DependencyContainer container)
    {
        _container = container;
    }

    private object GetInstance(Type type)
    {
        var parameters = type.GetConstructors().First().GetParameters();
        
        if (parameters.Count() > 0)
        {
            object[] instances = new object[parameters.Count()];
            for (int i = 0; i < parameters.Count(); i++)
            {
                var parameterType = parameters[i].ParameterType;
                
                var instance = Activator.CreateInstance(parameterType);
                instances[i] = instance;
            }
            
            return Activator.CreateInstance(type, instances);
        }
        return Activator.CreateInstance(type);
    }
    
    public T GetService<T>()
    {
        var type = _container.GetType(typeof(T));
        var parameters = type.GetConstructors().First().GetParameters();
        if (parameters.Count() > 0)
        {
            object[] instances = new object[parameters.Count()];
            
            for (int i = 0; i < parameters.Count(); i++)
            {
                var parameterType = parameters[i].ParameterType;
                
                var instance = GetInstance(parameterType);
                instances[i] = instance;
            }
            
            return (T)Activator.CreateInstance(type, instances);
        }
        return (T)Activator.CreateInstance(type);
    }
    
}

public class DependencyContainer
{
    private readonly List<Type> _types = new List<Type>();

    public void AddTransient<T>()
    {
        _types.Add(typeof(T));
    }
    
    public void AddSingleton<T>()
    {
        
    }
    
    public Type GetType(Type type)
    {
        return _types.First(x=> x.Name == type.Name);
    }
}


/// <summary>
/// /////////////////////////////////////
/// </summary>
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