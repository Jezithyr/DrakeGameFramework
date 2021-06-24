using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DrakeFramework.Core;
using Unity.Entities;
namespace DrakeFramework.Entities
{
	public class ECSService : Service, ITickable
	{
		private int priority;
		public override int Priority => priority;
		private World ECSWorld;
		public World World => ECSWorld;
		private EntityManager manager;
		public EntityManager Manager => manager;
		private string worldName;
		public string WorldName => worldName;
		private WorldFlags worldFlags = WorldFlags.Simulation;
		public ECSService(string _worldName, int _priority)
		{
			worldName = _worldName;
			priority = _priority;
		}
		public ECSService(string _worldName, int _priority, WorldFlags flags)
		{
			worldName = _worldName;
			priority = _priority;
			worldFlags = flags;
		}

		public Entity CreateRawEntity(EntityArchetype archetype)
		{
			return manager.CreateEntity(archetype);
		}
		public Entity CreateRawEntity(params ComponentType[] componentTypes)
		{
			return manager.CreateEntity(componentTypes);
		}

		public EntityData CreateEntity(EntityArchetype archetype)
		{
			return new EntityData(World, CreateRawEntity(archetype));
		}

		public EntityArchetype GetArchetype(params IComponentData[] components)
		{
			ComponentType[] temp = new ComponentType[components.Length];
			for (int i = 0; i < components.Length; i++)
			{
				temp[i] = components[i].GetType();
			}
			return manager.CreateArchetype(temp);
		}


		public EntityData CreateEntity(params ComponentType[] componentTypes)
		{
			return new EntityData(World, CreateRawEntity(componentTypes));
		}
		
		protected override void Dispose()
		{
			ECSWorld.Dispose();
		}

		protected override void Initialize()
		{
			ECSWorld = new World(worldName, worldFlags);
			manager = ECSWorld.EntityManager;
		}

		protected override void Reset()
		{
			Dispose();
			Initialize();
		}

		public void OnUpdate()
		{
			ECSWorld.Update();
		}
	}
}
