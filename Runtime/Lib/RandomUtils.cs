using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace DrakeFramework
{
	public static class RandomUtils
	{
		public static T DictGetRandomWeighted<T>(Dictionary<T, float> dict) //TODO: Redo this to use actual percentages
		{ //this randomization doesn't use true percentages
			List<T> output = new List<T>();
			float randomNum = Random.Range(0, 1f);
			while (output.Count < 1)
			{
				foreach (var keypair in dict)
				{
					if (keypair.Value >= randomNum)
					{
						output.Add(keypair.Key);
					}
				}
			}
			return output[Random.Range(0, output.Count)];
		}
		public static T GetRandom<T>(List<T> Input)
		{
			return (T)Input[Random.Range(0, Input.Count)];
		}
	}
}