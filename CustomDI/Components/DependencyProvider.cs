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
        var constructor = type.GetConstructors().FirstOrDefault();
        if(constructor == null)
            throw new Exception($"Type '{type.FullName}' doesnt have constructor");

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
            throw new Exception($"Type '{type.FullName}' doesnt have constructor");
        
        var parameters = constructor.GetParameters();
       
        var instances = parameters.Select(param => GetInstance(param.ParameterType)).ToArray();
        var service = (T)Activator.CreateInstance(type, instances);
        
        if(_container.IsSingleton(type))
            _container.SaveSingleton(typeof(T), service);
        return service;
    }
}
