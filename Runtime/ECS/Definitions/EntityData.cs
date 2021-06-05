using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Entities;
namespace DrakeFramework.Entities
{
	public struct EntityData
	{
		public World World;
		public Entity Entity;
		public EntityManager Manager => World.EntityManager;
		public EntityData(World world, Entity entity)
		{
			Entity = entity;
			World = world;
		}
		public void AddComponent<T>() where T:struct, IComponentData
		{
			World.EntityManager.AddComponent<T>(Entity);
		}
		public void RemoveComponent<T>() where T:struct,IComponentData
		{
			World.EntityManager.RemoveComponent<T>(Entity);
		}
		public void SetComponent<T>(T componentData) where T:struct,IComponentData
		{
			World.EntityManager.SetComponentData<T>(Entity,componentData);
		}
		public void AddComponentData<T>(T componentData) where T:struct, IComponentData
		{
			World.EntityManager.AddComponentData<T>(Entity, componentData);
		}
		public T GetComponentData<T>() where T:struct,IComponentData
		{
			return World.EntityManager.GetComponentData<T>(Entity);
		}
		public static implicit operator EntityData(ValueTuple<World, Entity> input) => new EntityData(input.Item1, input.Item2);
		public static implicit operator Entity(EntityData data) => data.Entity;
	}
}
