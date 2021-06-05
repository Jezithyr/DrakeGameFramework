using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DrakeFramework;

namespace DrakeFramework.Core {
	internal class CustomFixedUpdate
	{
		private float fixedTimeStep;
		private bool allowTimeMultiplier;
		private float timer = 0;
		private float timeMultiplier = 1;
		internal float TimeMultiplier
		{
			get { return timeMultiplier; }
			set { timeMultiplier = value; }
		}
		private OnFixedUpdateCallback callback;
		internal OnFixedUpdateCallback Callback
		{
			get { return callback; }
			set { callback = value; }
		}

		private float maxAllowedTimeStep = 0f;
		internal float MaxAllowedTimeStep
		{
			get { return maxAllowedTimeStep; }
			set { maxAllowedTimeStep = value; }
		}

		internal float deltaTime
		{
			get { return fixedTimeStep; }
			set { fixedTimeStep = Mathf.Max(value, 0.000001f); } // max rate: 1000000
		}
		internal float updateRate
		{
			get { return 1.0f / deltaTime; }
			set { deltaTime = 1.0f / value; }
		}

		internal CustomFixedUpdate(float aTimeStep, OnFixedUpdateCallback aCallback, bool _allowTimeMultiplier = true,float aMaxAllowedTimestep = 0)
		{
			aTimeStep = 1/aTimeStep;
			if (aCallback == null)
				throw new System.ArgumentException("CustomFixedUpdate needs a valid callback");
			if (aTimeStep <= 0f)
				throw new System.ArgumentException("TimeStep needs to be greater than 0");
			deltaTime = aTimeStep;
			callback = aCallback;
			allowTimeMultiplier = _allowTimeMultiplier;
			maxAllowedTimeStep = aMaxAllowedTimestep;
		}

		internal void Update(float aDeltaTime)
		{
			if (allowTimeMultiplier)
			{
				timer -= (aDeltaTime*timeMultiplier);
			} else{
				timer -= aDeltaTime;
			}
			
			if (maxAllowedTimeStep > 0)
			{
				float timeout = Time.realtimeSinceStartup + maxAllowedTimeStep;
				while (timer < 0f && Time.realtimeSinceStartup < timeout)
				{
					callback(fixedTimeStep);
					timer += fixedTimeStep;
				}
			}
			else
			{
				while (timer < 0f)
				{
					callback(fixedTimeStep);
					timer += fixedTimeStep;
				}
			}
		}

		internal void Update()
		{
			Update(Time.deltaTime);
		}
	}
}
