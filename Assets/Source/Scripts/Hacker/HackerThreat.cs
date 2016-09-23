using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ThreatRateModifier
{
	public GenericTimer modifyTime;
	public float modifyRate;
	public bool active;
}


public class HackerThreat : Photon.MonoBehaviour {
	
	private static HackerThreat _instance;
	
	public bool active;									// Whether or not the threat meter is currently active and running
	public float baseRate;									// Rows released per second
	public float maxThreatLevel;							// The maximum number of rows that will trigger a lockdown.
	public bool isInLockdown;
	public bool _threatDisabled = false;
	//private ThreatAnimation myAnimation;						// The animation script for displaying the threat meter
		
	private float threatLevel;								// The current cumulated Threat Level
	private float _ticker;									// The countdown ticker for releasing blocks
	private float _timer;									// The current timer threshold based on the block creation rate. ( second/rate )
	private static float _blockReleaseBase = 1.0f;  		// Time is measured in 1 second increments.  	
	private List<ThreatRateModifier> _timedModifiers = new List<ThreatRateModifier>();		// The list of timed rate modifiers
	private float _indefiniteModifier;		// The current accumulaiton of all indefinite rate modifiers
	private float _powerMultiplier;         // The current multiplier based on power consumed.

	
	#region Properties
	public static HackerThreat Manager
	{
		get
		{
			if(_instance == null)
			{
				_instance = new HackerThreat();			
			}
			return _instance;	
		}
	}
	
	public float Threat
	{
		get{
		return threatLevel;	
		}
	}
	
	public float MaxThreat
	{
		get{
		return maxThreatLevel;	
		}
	}
	
	public bool Active
	{
		get{
			return active;
		}
		set{
			active = value;
		}
	}
	#endregion
	
	// Use this for initialization
	void Start ()
	{		
		//_timedModifiers = new List<ThreatRateModifier>();
		_timer = 1.0f;
		_ticker = 0;
		_powerMultiplier = 1;
		isInLockdown = false;
		//myAnimation = (ThreatAnimation) GameObject.Find("TopDownCamera").GetComponent("ThreatAnimation");
	}
	
	
	// Update is called once per frame
	void Update () 
	{
		if (active && !ThiefManager.Manager.gameIsPaused && !HackerManager.Manager.gameIsPaused)
		{
			float delta = Time.deltaTime;		
			float currentRate = CalculateCurrentRate();
				
			// current Rate right now is in rows per second.
			// We need to change this to seconds per row.
			if ( currentRate != 0 )
			{
				_timer = 1/currentRate;
				
				// Increment the ticker.  When Ticker reaches timer rate
				// release annother row of blocks.
				_ticker += delta;
				if ( _ticker >= _timer )
				{
					if( !(_threatDisabled) && Application.loadedLevel > 3) // No Threat in level 1. Make sure that Level 1 is first in the build settings
					threatLevel += 1;
					_ticker = 0; // reset ticker.
					//Debug.Log ("Threat Level: " + threatLevel + " of " + maxThreatLevel);
				}
			}
			
			//This should not be here. Blame Vaibhav if this is not removed. Added this just for testing
			if( Input.GetKeyDown(KeyCode.Keypad5) )
				BoostAlertLevelForTime( 0.25f, 20.0f);
			
			// Check to see if the Threat Level has reached Max Level
			if ( threatLevel >= maxThreatLevel)
			{
				// Initalize LockDown Sequence on Both Sides
				ThreadTriggerEvent();
				active = false;
			}
		}
	}
	
	
	void ThreadTriggerEvent()
	{
		OverrideManager.Manager.CreateOverrideBasedOnPlayerPosition();
	}

		
	// Initialization
	public HackerThreat()
	{
		_instance = this;
		threatLevel = 0.0f;
	}
	
	#region Public PhotonNetwork API
	
	// Resets the threat amount to some specified amount
	// If you do not send any parameter it will reset to 0
	public void ResetThreatLevel(int i_amount=0)
	{
		int amountToRemove = 0;
		if ( threatLevel > i_amount )
			amountToRemove = (int)threatLevel - i_amount -1;
		
		DecreaseThreatAmount( amountToRemove );
		//myAnimation.ResetCubes();
		//threatLevel = 0.0f;
		active = true;		
	}
	
	// Legacy
	//Sets the number of connected nodes
	public void SetConnectedCount( int i_count )
	{
		//connectedNodes = i_count;
	}
	
	//modifies the threat rate by a certain amount (rows)   
	public void BoostAlertLevelForTime( float i_time, float i_amount )
	{
		ThreatRateModifier tempModifier = new ThreatRateModifier();
		
		Action timerEndAction = delegate(){RemoveTimedModifier(tempModifier);};
		GenericTimer modifyTimer = gameObject.AddComponent<GenericTimer>();
		modifyTimer.Set( i_time, false, timerEndAction);
		modifyTimer.Run();
		
		tempModifier.modifyTime = modifyTimer;
		tempModifier.modifyRate = i_amount/i_time;
		tempModifier.active = true;
		
		_timedModifiers.Add( tempModifier );
	}
	
	public void SetPowerMultiplier( float i_multiplier )
	{
		_powerMultiplier = i_multiplier;
	}
	
	// This will modify the threat reate by some amount/second indefinitly
	// Rate is represented as rows per second. ( out of 100 rows possible )
	public void ModifyAlertRate(float i_rate)
	{
		_indefiniteModifier += i_rate;
	}
	
	
	// Removes a specific timed modifier from the timed modifiers list.
	public void RemoveTimedModifier( ThreatRateModifier i_modifier )
	{
		_timedModifiers.Remove(i_modifier);
	}
	
	
	// Decreases the current threat amount.  Used mostly by SAP's
	public void DecreaseThreatAmount(int i_rows)
	{
		if ( i_rows >= threatLevel )
		{
			i_rows = (int)threatLevel-1;
		}
		
		//myAnimation.RemoveCubes( i_rows );
		threatLevel -= i_rows;
	}
	
	public void IncreaseThreatAmount(int i_rows)
	{
		if ( i_rows + threatLevel > maxThreatLevel )
		{
			threatLevel = maxThreatLevel;
		}
		else
		threatLevel += i_rows;
	}
	
	#endregion
	
	// Calculates the current threat rate.
	private float CalculateCurrentRate()
	{
		float currentRate = baseRate + _indefiniteModifier;
		if(_timedModifiers != null)
		{
			for ( int i=0 ; i<_timedModifiers.Count ; i++ )
			{
					currentRate += _timedModifiers[i].modifyRate;
			}
		}
			
		if ( currentRate < 0 )
			currentRate = 0;
		
		//Temp
		currentRate *= _powerMultiplier;
		
		return currentRate;
	}
	
}
