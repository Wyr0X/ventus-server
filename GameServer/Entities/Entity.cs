using System;
using System.Collections.Generic;
using System.ComponentModel;

public class Entity
{
    private readonly Dictionary<Type, IComponent> _components = new Dictionary<Type, IComponent>();

    public int Id { get; }

    public Entity(int id)
    {
        Id = id;
    }

    // Método para agregar un componente a la entidad
    public void Add(IComponent component)
    {
        // Usamos ComponentType<T> como clave
        Type componentType = component.GetType();
        _components[componentType] = component;
    }

    // Eliminar un componente de la entidad
    public void Remove(Type componentType)
    {
        _components.Remove(componentType);
    }

    // Verificar si la entidad tiene un componente específico
    public bool Has(Type componentType)
    {
        return _components.ContainsKey(componentType);
    }

    // Obtener un componente de la entidad
    public IComponent? Get(Type componentType)
    {
        return _components.GetValueOrDefault(componentType);
    }

    // Obtener un componente de manera opcional (si existe)
    public IComponent? MaybeGet(Type componentType)
    {
        if (_components.ContainsKey(componentType))
        {
            return _components.GetValueOrDefault(componentType);
        }
        return null;
    }

    // Ejecutar una acción sobre todos los componentes de la entidad
    public void ForEachComponent(Action<object> action)
    {
        foreach (var component in _components.Values)
        {
            action(component);
        }
    }
}
