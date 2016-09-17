using UnityEngine;
using System.Collections;

public class guardAnimation
{
	/// <summary>
	/// The name of the animation.
	/// </summary>
	string mAnimationName;
	public string AnimationName
	{
		get
		{
			return mAnimationName;
		}
	}

	/// <summary>
	/// Indicates if the Fangs must me in or out for this animation to play
	/// </summary>
	bool mFangsOut;

	public bool areFangsOut
	{
		get
		{
			return mFangsOut;
		}

		set
		{
			mFangsOut = value;
		}
	}

	public guardAnimation(string iAnimationName,bool iflags)
	{
		mAnimationName = iAnimationName;
		mFangsOut = iflags;
	}
}

