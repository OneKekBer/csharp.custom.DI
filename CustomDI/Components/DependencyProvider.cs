namespace CustomDI.Components;

public sealed class DependencyProvider
{
    private readonly DependencyContainer _container;
    public DependencyProvider(DependencyContainer container)
    {
        _container = container;
    }
    
    private object CreateInstance(Type type, object[]? dependencies)
    {
        if (_container.IsSingletonAndExists(type))
            return _container.GetSingletonInstance(type);
        
        var instance = Activator.CreateInstance(type, dependencies);
        if (_container.IsSingleton(type))
            _container.SaveSingleton(instance);
        
        return instance;
    }
    
    public T GetService<T>()
    {
        var createdInstances = new Dictionary<Type, object>();
        var stack = new Stack<Type>();
        var stack2 = new Stack<Type>();
        
        stack2.Push(typeof(T));
        stack.Push(typeof(T));
    
        while (stack.Count > 0)
        {
            var type = stack.Pop();
            if(!_container.IsRegistered(type))
                throw new Exception($"Type '{type.FullName}' doesnt have a registered parameter");
            
            
            var constructor = type.GetConstructors().FirstOrDefault();
            if(constructor == null)
                throw new Exception($"Type '{type.FullName}' doesnt have constructor");
            
            var parameters = constructor.GetParameters();
    
            foreach (var param in parameters)
            {
                stack.Push(param.ParameterType);
                stack2.Push(param.ParameterType);
            }
        }

        while (stack2.Count > 0)
        {
            var type = stack2.Pop();
            if(createdInstances.ContainsKey(type))
                continue;

            var parameters = type.GetConstructors().FirstOrDefault().GetParameters();
            var dependencies = parameters.Select(param => createdInstances[param.ParameterType]).ToArray();
            
            var instance = CreateInstance(type, dependencies);
            createdInstances.Add(type, instance);
        }
        
        return (T)createdInstances[typeof(T)];
    }
}
