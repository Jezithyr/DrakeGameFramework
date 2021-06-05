using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DrakeFramework;

namespace DrakeFramework.Core
{
    public class ModuleManager
    {
		private Dictionary<System.Type,Module> moduleTypeLookup = new Dictionary<System.Type, Module>();
		private List<Module> loadedModules = new List<Module>();
		public IReadOnlyList<Module> LoadedModules {get => loadedModules;}

		private Session.SessionEventDelegate onSessionEnd;
		private Session.SessionEventDelegate onSessionStart;
		private Action  onSessionInit;
		internal Session.SessionEventDelegate OnSessionEnd{get=>onSessionEnd;}
		internal Session.SessionEventDelegate OnSessionStart{get=>onSessionStart;}
		internal Action OnSessionInit{get=>onSessionInit;}

		internal ModuleManager(){
			//load all default modules in order of priority
			foreach (var module in ReflectHelper.CreateInstancesOfTypes<Module>(ReflectHelper.GetSubclassesInAllAssemblies(typeof(Module))))
			{
				PreLoadModule(module);
				onSessionInit += module.internal_Initialize;
				onSessionStart += module.internal_SessionStart;
				onSessionEnd += module.internal_SessionEnd;
			}
		}

		internal void LoadModules()
		{
			foreach (var module in loadedModules)
			{
				module.internal_Load();
				ITickable tickableModule = module as ITickable;
				if (tickableModule != null)
				{
					Debug.Log("Registering Update for:" +module.ToString());
					Game.Ticking.RegisterUpdateEvent(tickableModule.OnUpdate);
				} else 
				{
					IFixedTickable fixedTickableModule = module as IFixedTickable;
					if (fixedTickableModule != null)
					{
						Debug.Log("Registering FixedUpdate for:" +module.ToString());
						Game.Ticking.RegisterFixedUpdateEvent(fixedTickableModule.UpdateRate, fixedTickableModule.OnUpdate, fixedTickableModule.CanBeMultiplied, fixedTickableModule.MaxTimestep);
					}
				}
			}
		}

		private void PreLoadModule(Module module)
		{
			moduleTypeLookup.Add(module.GetType(),module);
			InsertModuleSorted(module);
		}
		public T GetModule<T>() where T:Module
		{
			return (T)moduleTypeLookup[typeof(T)];
		}
		private void InsertModuleSorted(Module module)
		{
			int index = loadedModules.FindLastIndex(e => e.Priority < module.Priority);
            	if (index == 0 || index == -1)
            	{
                	loadedModules.Insert(0, module);
                	return;
            	}
            	loadedModules.Insert(index + 1, module);	
		}

		~ModuleManager()
		{
			foreach (var module in loadedModules)
			{
				module.internal_Unload();
				ITickable tickableModule = module as ITickable;
				if (tickableModule != null)
				{
					Debug.Log("Removing Update for Module:" +module.ToString());
					Game.Ticking.RemoveUpdateEvent(tickableModule.OnUpdate);
				} else 
				{
					IFixedTickable fixedTickableModule = module as IFixedTickable;
					if (fixedTickableModule != null)
					{
						Debug.Log("Removing Fixed Update for Module:" +module.ToString());
						Game.Ticking.RemoveFixedUpdateEvent(fixedTickableModule.UpdateRate, fixedTickableModule.OnUpdate);
					}
				}
			}
		}
    }
}
