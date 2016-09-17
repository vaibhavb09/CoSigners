using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public delegate void AnimUnitDelegate();

public class AnimControl 
{
	private static Dictionary<string, List<AnimUnit>> s_animDict = new Dictionary<string, List<AnimUnit>>();

	public static AnimUnit StartAnimation(string i_animName, Vector3 i_position, Quaternion i_rotation, int i_repeatTimes = 1, AnimUnitDelegate i_onAnimationFinished = null)
	{
		GameObject animPrefab = (GameObject) Resources.Load("Prefabs/Animation/" + i_animName);
		if(animPrefab ==  null)
		{
			//Debug.LogError(i_animName + " doesn't exist, are you sure it is in the prefab folder?");
			return null;
		}

		GameObject animObj = GameObject.Instantiate(animPrefab, i_position, i_rotation) as GameObject;

		AnimUnit animScript = animObj.GetComponent(typeof(AnimUnit)) as AnimUnit;

		if(animScript ==  null)
		{
			Debug.LogError("AnimUnit doesn't exist, please make sure the AnimUnit is attached to your animation prefab");
			return null;
		}

		if(s_animDict.ContainsKey(i_animName))
		{
			s_animDict[i_animName].Add(animScript);
		}
		else
		{
			List<AnimUnit> animList = new List<AnimUnit>();
			animList.Add(animScript);
			s_animDict.Add(i_animName, animList);
		}

		if(i_onAnimationFinished != null)
		{
			animScript.OnAnimationFinished = i_onAnimationFinished;
		}

		animScript.Play(i_animName, i_repeatTimes);

		return animScript;
	}

	public static bool StopAnimationsByName(string i_animName)
	{
		if(!s_animDict.ContainsKey(i_animName))
		{
			Debug.LogError("Animation Name: " + i_animName + " doesn't exist, why are you trying to delete it?");
			return false;
		}
		else
		{
			foreach(AnimUnit unit in s_animDict[i_animName])
			{
				unit.Stop();
			}
			s_animDict.Remove(i_animName);
		}
		return true;
	}


	/// <summary>
	/// Removes the animation.
	/// </summary>
	/// <param name="i_animName">I_anim name.</param>
	/// <param name="i_instance">I_instance.</param>
	public static void RemoveAnimation(string i_animName, AnimUnit i_instance)
	{
		if(!s_animDict.ContainsKey(i_animName))
		{
			Debug.LogError("Animation Name: " + i_animName + " doesn't exist, why are you trying to delete it?");
			return;
		}
		else
		{
			//we don't need to call this, but it doesn't hurt I guess.
			i_instance.Stop();
			s_animDict[i_animName].Remove(i_instance);
			i_instance = null;

			if(s_animDict[i_animName].Count == 0)
			{
				s_animDict[i_animName].Clear();
				s_animDict[i_animName] = null;
			}
		}
	}
}
