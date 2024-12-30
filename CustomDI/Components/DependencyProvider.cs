namespace CustomDI.Components;

public sealed class DependencyProvider
{
    private readonly DependencyContainer _container;
    public DependencyProvider(DependencyContainer container)
    {
        _container = container;
    }

    private object GetInstance(Type type)
    {
        if(!_container.IsRegistered(type))
            throw new Exception($"Type '{type.FullName}' doesnt have a registered parameter");
        
        var constructor = type.GetConstructors().FirstOrDefault();
        if(constructor == null)
            throw new Exception($"Type '{type.FullName}' doesnt have constructor");

        var parameters = constructor.GetParameters();
        
        var instances = parameters.Select(param =>
        {
            var parameterType = param.ParameterType;
            
            if (_container.IsSingletonAndExists(parameterType))
                return _container.GetSingletonInstance(parameterType);
                    
            var instance = Activator.CreateInstance(parameterType);
            
            if(_container.IsSingleton(parameterType))
                _container.SaveSingleton(parameterType, instance);
            
            return instance;
        }).ToArray();
        
        return Activator.CreateInstance(type, instances);
    }
        
    public T GetService<T>()
    {
        var info = _container.GetInfo(typeof(T));
        var type = info.InstanceType;

        if (_container.IsSingletonAndExists(type))
            return (T)_container.GetSingletonInstance(type);;
        
        
        var constructor = type.GetConstructors().FirstOrDefault();
        if(constructor == null)
            throw new Exception($"Type '{type.FullName}' doesnt have constructor");
        
        var parameters = constructor.GetParameters();
       
        var instances = parameters.Select(param => GetInstance(param.ParameterType)).ToArray();
        var service = (T)Activator.CreateInstance(type, instances);
        
        if(_container.IsSingleton(type))
            _container.SaveSingleton(typeof(T), service);
        return service;
    }
    
}
