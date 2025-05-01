public interface IComponent { }

public interface IComponentType<T> where T : IComponent
{
    Type Type { get; }
}

public class Component : IComponent { }

public class ComponentType<T> : IComponentType<T> where T : IComponent
{
    public Type Type => typeof(T);
}
