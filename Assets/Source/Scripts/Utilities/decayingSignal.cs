//#define _DEBUG_SPEW

using UnityEngine;
using System.Collections;

public class decayingSignal : MonoBehaviour
{
	private float mTimeToDecay,mStartTime;
	public float timeToDecay
	{
		get 
		{
			return mTimeToDecay;
		}
		
		set
		{
			mTimeToDecay = value;
		}
	}
	
	// Indicates whether the Signal is active or not
	// negative values indicate inactivity
	// 0 indicates a decayed signal
	// 1 indicates a signal that is currently active
	// 2 indicates a signal that is currently paused
	private int mIsActive = -1;

	/// <summary>
	/// mPuaseDuration : Indicates how long the timer has been paused ever since it was activated (and before it decays)
	/// mPauseStarttime : Indicates what time the current pause started
	/// </summary>
	private float mPuaseDuration,mPauseStarttime;
	
	public bool IsActive
	{
		get{ 
			return ((mIsActive == -1 || mIsActive == 2)|| mIsActive == 0) ? false : true; 
		}
	}
	
	public void initialize(float iTimeToDecay)
	{
		mIsActive = 1;
		mStartTime = Time.time;
		mTimeToDecay = iTimeToDecay;
//		Debug.Log("Decaying signal started at : " + mStartTime);
//		Debug.Log("Time to decay : " + iTimeToDecay);
	}
	
	public void decayPrematurely()
	{
		mIsActive = -1;
	}
	
	public void addTime(float iTimeToBeAddedinSecs)
	{
		mTimeToDecay += iTimeToBeAddedinSecs;
	}
	
	// Use this for initialization
	void Start ()
	{
	}

	/// <summary>
	/// Pauses this Decaying signals timer till "Resume" is called
	/// </summary>
	/// <returns> true if the Timer was successfully paused 
	/// false if you try to pause a decayed or paused timer </returns>
	public bool Pause()
	{
		if((mIsActive == -1 || mIsActive == 2))
		{
			#if _DEBUG_SPEW
				Debug.Log("Trying to pause a " + ((mIsActive == -1)? "Decayed" : "Paused" )+ " signal");
			#endif
			return false;
		}
		else
		{
			// Indicate that the timer was just paused
			mIsActive = 2;

			mPauseStarttime = Time.time;

			return true;
		}
	}

	/// <summary>
	/// Resume a paused Timer
	/// </summary>
	/// <returns> true if the Timer was successfully resumed
	/// false if you try to pause a decayed or active timer </returns>
	public bool Resume()
	{
		if(mIsActive == -1 || mIsActive == 1)
		{
			#if _DEBUG_SPEW
				Debug.Log("Trying to resume a " + ((mIsActive == -1)? "Decayed" : "UnPaused" )+ " signal");
			#endif
			return false;
		}else
		{
			// Indicate that the timer was unpaused
			mIsActive = 1;

			// Add current pause duration to the total time that this timer was paused
			mPuaseDuration += (Time.time - mPauseStarttime);

			#if _DEBUG_SPEW
			Debug.Log("Timer was paused for " + mPuaseDuration + "units");
			#endif

			// reset the pause start time
			mPauseStarttime = 0;

			return true;
		}
	}


	// Update is called once per frame
	void Update ()
	{
		if(mIsActive == 1)
		{
			if(Time.time < (mStartTime + timeToDecay + mPuaseDuration))
				mIsActive = 1;
			else
			{
				mIsActive = -1;

				// Cleanup the pause data
				mPuaseDuration = 0;
				mPauseStarttime = 0;
			}
		}
	}
}

