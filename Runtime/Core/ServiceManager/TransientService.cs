using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DrakeFramework;
using DrakeFramework.Core;

public abstract class TransientService : Service
{
	public override int Priority => 0;
}
