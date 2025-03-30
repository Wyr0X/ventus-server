using System;
using System.Collections.Generic;
using System.ComponentModel;

public class EntityManager
{
    private int nextId = 0;
    private readonly Dictionary<Type, List<(IComponent, Entity)>> _components = new Dictionary<Type, List<(IComponent, Entity)>>();
    private readonly Dictionary<object, List<ISystem>> _systems = new Dictionary<object, List<ISystem>>();
    private readonly Dictionary<EventType, Dictionary<Type, List<Listener<IComponent>>>> _events =
        new Dictionary<EventType, Dictionary<Type, List<Listener<IComponent>>>>();

    public Entity Create(IComponent[] components)
    {
        Entity entity = new Entity(nextId++);
        AddComponents(entity, components);
        return entity;
    }

    public void AddComponents(Entity entity, IComponent[] components)
    {
        foreach (var component in components)
        {
            Type componentType = component.GetType();
            if (entity.Has(componentType))
            {
                RemoveComponent(entity, componentType);
            }
            entity.Add(component);

            if (!_components.ContainsKey(componentType))
            {
                _components[componentType] = [];
            }

            _components[componentType].Add((component, entity));
            DispatchFor(EventType.ComponentAdded, componentType, entity);
        }
    }

    public void RemoveComponent(Entity entity, Type componentType)
    {
        entity.Remove(componentType);
        if (_components.TryGetValue(componentType, out var componentList))
        {
            componentList.RemoveAll(c => c.Item1.GetType() == componentType);
            DispatchFor(EventType.ComponentRemoved, componentType, entity);
        }
    }

    public void Remove(Entity entity)
    {
        entity.ForEachComponent(component =>
        {
            RemoveComponent(entity, component.GetType());
        });
    }

    // Singleton component
    public (T, Entity) GetOne<T>(Type componentType) where T : IComponent
    {
        // Obtener la lista de componentes para el tipo dado
        var componentsByType = _components.GetValueOrDefault(componentType);

        // Verificar si la lista existe y no está vacía
        if (componentsByType != null && componentsByType.Count > 0)
        {
            var firstComponent = componentsByType[0]; // Obtener el primer componente

            // Devolver el componente y la entidad, casteando el componente a T
            return ((T)firstComponent.Item1, firstComponent.Item2);
        }
        else
        {
            // Manejar el caso cuando no se encuentra el componente, por ejemplo lanzando una excepción o retornando un valor predeterminado
            throw new InvalidOperationException($"No components of type {componentType} found.");
        }
    }
    public List<(IComponent, Entity)> Get(Type componentType)
    {
        // Obtener la lista de componentes para el tipo dado, o una lista vacía si no existe
        var componentsByType = _components.GetValueOrDefault(componentType) ?? new List<(IComponent, Entity)>();

        // Castear la lista de componentes a List<(T, Entity)> y retornarla
        return componentsByType;
    }
    public void AddSystems(object runKey, List<ISystem> systemsToAdd)
    {
        if (!_systems.ContainsKey(runKey))
        {
            _systems[runKey] = new List<ISystem>();
        }
        _systems[runKey].AddRange(systemsToAdd);
    }

    public void RunSystems(object runKey)
    {
        if (_systems.ContainsKey(runKey))
        {
            foreach (var system in _systems[runKey])
            {
                system.Run(this);
            }
        }
    }

    public void AddListeners<T>(Type componentType, Listener<IComponent> listener, EventType[] eventsToAdd) where T : IComponent
    {
        foreach (var eventType in eventsToAdd)
        {
            if (!_events.ContainsKey(eventType))
            {
                _events[eventType] = new Dictionary<Type, List<Listener<IComponent>>>();
            }

            if (!_events[eventType].ContainsKey(componentType))
            {
                _events[eventType][componentType] = new List<Listener<IComponent>>();
            }

            _events[eventType][componentType].Add(listener);
        }
    }

    public void Dispatch(Enum eventType, Entity entity)
    {
        if (!_events.ContainsKey((EventType)eventType)) return;

        var listenersByType = _events[(EventType)eventType];

        foreach (var componentListeners in listenersByType)
        {
            Type componentType = componentListeners.Key;
            List<Listener<IComponent>> listeners = componentListeners.Value;

            // Obtener el componente de la entidad
            var component = entity.Get(componentType);

            // Si no se encuentra el componente, no hacer nada
            if (component == null) continue;

            // Invocar cada listener con el componente y la entidad
            foreach (var listener in listeners)
            {
                if (listener is Listener<IComponent> typedListener)
                {
                    typedListener(component, entity);
                }
            }
        }
    }
    public void DispatchFor(EventType eventType, Type componentType, Entity entity)
    {
        if (_events.ContainsKey(eventType))
        {
            if (_events[eventType].ContainsKey(componentType))
            {
                foreach (var listener in _events[eventType][componentType])
                {
                    // Invocar el listener
                    if (listener is Listener<IComponent> typedListener)
                    {
                        // Aquí se asume que el componente se obtiene de alguna forma de la entidad
                        IComponent? component = entity.Get(componentType); // Suponiendo que tienes un método para obtener el componente
                        if (component == null) continue;
                        typedListener(component, entity);
                    }
                }
            }
        }
    }
}

internal class ComponentType
{
}