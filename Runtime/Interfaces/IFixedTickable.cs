using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFixedTickable
{
    float UpdateRate{get;}
	bool CanBeMultiplied{get;}
	float MaxTimestep{get;}
	void OnUpdate(float deltaTime);
}
