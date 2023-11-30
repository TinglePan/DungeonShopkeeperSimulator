using System;
using System.Collections.Generic;
using DSS.Ecs.Components;

namespace DSS.Ecs;

public class Ecs
{
	public Dictionary<Guid, ComponentBase> Components = new();
	private Dictionary<Type, HashSet<Guid>> _componentIdsByType = new();
	
	public HashSet<Entity> Entities = new();

	public Entity CreateEntity(params ComponentBase[] components)
	{
		var entity = new Entity(components);
		Entities.Add(entity);
		foreach (ComponentBase component in components)
		{
			AddComponent(component, entity.Id);
		}
		return entity;
	}

	public void DestroyEntity(Entity entity)
	{
		Entities.Remove(entity);
		foreach (var compId in entity.ComponentIds)
		{
			if (Components.TryGetValue(compId, out var comp))
			{
				comp.EntityIds.Remove(entity.Id);
				if (comp.EntityIds.Count == 0)
				{
					RemoveComponent(comp);
				}
			}
		}
	}
	
	private void AddComponent(ComponentBase component, Guid entityId)
	{
		Components.TryAdd(component.Id, component);
		component.EntityIds.Add(entityId);
		
		var type = component.GetType();
		if (!_componentIdsByType.ContainsKey(type))
		{
			_componentIdsByType[type] = new HashSet<Guid>();
		}

		_componentIdsByType[type].Add(component.Id);
	}

	private void RemoveComponent(ComponentBase comp)
	{
		Components.Remove(comp.Id);
		_componentIdsByType[comp.GetType()].Remove(comp.Id);
	}
}
