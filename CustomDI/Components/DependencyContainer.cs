namespace CustomDI.Components;

public sealed class DependencyContainer // i think it`s bad, beacuse single responsibility 
{
    private readonly Dictionary<Type, InstanceInfo> _data = new Dictionary<Type, InstanceInfo>();
    
    public void AddTransient<T>()
    {
        var type = typeof(T);
        if(!IsRegistered(type))
            _data.Add(type, new InstanceInfo(DependencyType.Transient, type));
    }
    
    public bool IsRegistered(Type type)
    {
        return _data.ContainsKey(type);
    }
    
    public void AddSingleton<T>()
    {
        var type = typeof(T);
        if(!IsRegistered(type))
            _data.Add(type, new InstanceInfo(DependencyType.Singleton, type));
    }
    
    public bool IsSingleton(Type type)
    {
        return _data.ContainsKey(type) 
               && _data[type].DependencyType == DependencyType.Singleton;
    }
    
    public bool IsSingletonAndExists(Type type)
    {
        return _data.ContainsKey(type) 
               && _data[type].DependencyType == DependencyType.Singleton 
               && _data[type].Instance != null;
    }

    public void SaveSingleton(Type type, object instance)
    {
        Console.WriteLine($"Saving singleton for type {type.FullName}");
        _data[type].Instance = instance;
    }
    
    public Object GetSingletonInstance(Type type)
    {
        _data.TryGetValue(type, out var instanceInfo);
        Console.WriteLine($"Retrieving singleton for type {type.FullName}: {instanceInfo.Instance}");
        return instanceInfo.Instance;
    }

    public InstanceInfo GetInfo(Type type)
    {
        _data.TryGetValue(type, out var info);
        return info;
    }
}

