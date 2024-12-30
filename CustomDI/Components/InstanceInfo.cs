namespace CustomDI.Components;

public enum DependencyType
{
    Transient = 0,
    Singleton = 1,
}

public sealed class InstanceInfo
{
    public InstanceInfo(DependencyType depType, Type instanceType)
    {
        DependencyType = depType;
        InstanceType = instanceType;
    }

    public DependencyType DependencyType { get; init; }
    public Type InstanceType { get; init; } //maybe i dont need that
    public Object? Instance { get; set; } = null;
}
