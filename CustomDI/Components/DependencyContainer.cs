namespace CustomDI.Components;

public class DependencyContainer // i think it`s bad, beacuse single responsibility 
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

