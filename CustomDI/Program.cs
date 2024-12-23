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
        var constructor = type.GetConstructors().FirstOrDefault();
        if(constructor == null)
            throw new Exception($"Type '{type.FullName}' does not have a default constructor.");

        var parameters = type.GetConstructors().First().GetParameters();
        
        if (parameters.Count() > 0)
        {
            object[] instances = new object[parameters.Count()];
            for (int i = 0; i < parameters.Count(); i++)
            {
                var parameterType = parameters[i].ParameterType;
                object instance = null;

                if (_container.IsSingletonAndExists(parameterType))
                {
                    instance = _container.GetSingleton(parameterType);
                    instances[i] = instance;
                    continue;   
                }
               
                instance = Activator.CreateInstance(parameterType);
                
                if(_container.IsSingleton(parameterType))
                    _container.SaveSingleton(parameterType, instance);
                    
                instances[i] = instance;
            }
            
            return Activator.CreateInstance(type, instances);
        }
        return Activator.CreateInstance(type);
    }
    
    public T GetService<T>()
    {
        var type = _container.GetType(typeof(T));
        
        if (_container.IsSingletonAndExists<T>())
            return (T)_container.GetSingleton<T>();
        
        var constructor = type.GetConstructors().FirstOrDefault();
        if(constructor == null)
            throw new Exception($"Type '{type.FullName}' does not have a default constructor.");
        
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
    private readonly Dictionary<Type, object> _singletons = new Dictionary<Type, object>();
    
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
        _singletons[type] = instance;
    }
    
    public object GetSingleton<T>()
    {
        return _singletons.TryGetValue(typeof(T), out var instance);
    }
    
    public object GetSingleton(Type type)
    {
        return _singletons.TryGetValue(type, out var instance);
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