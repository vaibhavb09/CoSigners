//#define _ANIM_DEBUG_SPEW
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GuardAnimationController {

	/// <summary>
	/// Unity's Animation interface
	/// </summary>
	private Animation mGuardAnimations = null;

	private bool mFangsOut;
	private bool mBanked;

	/// <summary>
	/// The queue for handling animations
	/// Any animations in this queue are played one after the other
	/// </summary>
	private Queue<guardAnimation> mPendingAnimationQueue;

	public GuardAnimationController(Animation iGuardAnimations)
	{
		mGuardAnimations = iGuardAnimations;

		mFangsOut = false;
		mBanked= false;

		mPendingAnimationQueue = new Queue<guardAnimation>();
	}

	public void guardIdling()
	{
//		if(!mFangsOut)
//			mGuardAnimations.CrossFade("normalIdle");
	}

	public void playNormalToAlert()
	{

		#if _ANIM_DEBUG_SPEW
			Debug.Log("NORMAL to DISTURBED");
		#endif

		mPendingAnimationQueue.Enqueue(new guardAnimation("normalToDisturbed",false));

		mFangsOut = true;
	}

	public void playAlertToNormal()
	{

		#if _ANIM_DEBUG_SPEW
			Debug.Log("DISTURBED to NORMAL");
		#endif

		mPendingAnimationQueue.Enqueue(new guardAnimation("disturbedToNormal",true));

		mFangsOut = false;

		//Debug.Log("FANGS **************************** IN ");
	}

	public void playAlertToChasing()
	{

		#if _ANIM_DEBUG_SPEW
			Debug.Log("CHASING FORWARDS");
		#endif

		mPendingAnimationQueue.Enqueue(new guardAnimation("chasingForward",true));

		mFangsOut = true;

		//Debug.Log("FANGS **************************** OUT ");
	}

	/// <summary>
	/// Queues up the left turn start Animation
	/// </summary>
	public void turnLeftStart()
	{

		if(mFangsOut)
		{
			#if _ANIM_DEBUG_SPEW
				Debug.Log("TURN LEFT START FANG OUT");
			#endif
			// If the guard is not banked
			if(!mBanked)
			{
				// play banking animation
				mPendingAnimationQueue.Enqueue(new guardAnimation("disturbedLefrTurnStart",true));

				// Indicate banking
				mBanked = true;
			}
		}
		else
		{
			#if _ANIM_DEBUG_SPEW
				Debug.Log("TURN LEFT START FANG IN");
			#endif

			// If the guard is not banked
			if(!mBanked)
			{
				mPendingAnimationQueue.Enqueue(new guardAnimation("leftTurnStart",false));

				// Indicate banking
				mBanked = true;
			}
		}
	}

	/// <summary>
	/// Queues up the left turn end Animation
	/// </summary>
	public void turnLeftEnd()
	{
		
		if(mFangsOut)
		{
			#if _ANIM_DEBUG_SPEW
				Debug.Log("TURN LEFT END FANG OUT");
			#endif

			if(mBanked)
			{
				mPendingAnimationQueue.Enqueue(new guardAnimation("disturbedLeftTurnEnd",true));

				// Indicate not banking
				mBanked = false;
			}
		}
		else
		{
			#if _ANIM_DEBUG_SPEW
				Debug.Log("TURN LEFT END FANG IN");
			#endif

			if(mBanked)
			{
				mPendingAnimationQueue.Enqueue(new guardAnimation("leftTurnEnd",false));

				// Indicate not banking
				mBanked = false;
			}
		}
	}


	/// <summary>
	/// Queues up the right turn start Animation
	/// </summary>
	public void rightTurnStart()
	{
		
		if(mFangsOut)
		{
			#if _ANIM_DEBUG_SPEW
				Debug.Log("TURN RIGHT START FANG OUT");
			#endif
			if(!mBanked)
			{
				mPendingAnimationQueue.Enqueue(new guardAnimation("disturbedRightTurnStart",true));

				// Indicate banking
				mBanked = true;
			}
		}
		else
		{
			#if _ANIM_DEBUG_SPEW
				Debug.Log("TURN RIGHT START FANG IN");
			#endif

			if(!mBanked)
			{
				mPendingAnimationQueue.Enqueue(new guardAnimation("rightTurnStart",false));

				// Indicate banking
				mBanked = true;
			}
		}
	}
	
	/// <summary>
	/// Queues up the right turn end Animation
	/// </summary>
	public void turnRightEnd()
	{
		
		if(mFangsOut)
		{
			#if _ANIM_DEBUG_SPEW
			Debug.Log("TURN RIGHT END FANG OUT");
			#endif
			if(mBanked)
			{
				mPendingAnimationQueue.Enqueue(new guardAnimation("disturbedRightTurnEnd",true));

				// Indicate not banking
				mBanked = false;
			}
		}
		else
		{
			#if _ANIM_DEBUG_SPEW
			Debug.Log("TURN RIGHT END FANG IN");
			#endif
			if(mBanked)
			{
				mPendingAnimationQueue.Enqueue(new guardAnimation("rightTurnEnd",false));

				// Indicate not banking
				mBanked = false;
			}
		}
	}

	public void updateAnimation()
	{
		if(!mGuardAnimations.isPlaying)
		{
			#if _ANIM_DEBUG_SPEW
				Debug.Log("SELECTING NEW ANIMATION");
			#endif

			if(mPendingAnimationQueue.Count > 0)
			{
		guardAnimation _nextAnimation = mPendingAnimationQueue.Dequeue();

				#if _ANIM_DEBUG_SPEW
					Debug.Log("PLAYING ANIMATION " + _nextAnimation.AnimationName);
				#endif


				mGuardAnimations.CrossFade(_nextAnimation.AnimationName);

			}
		}
	}


}
