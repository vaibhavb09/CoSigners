using UnityEngine;
using System.Collections;
using System;

public class GenericTimer : MonoBehaviour {
	
	/// <summary>
	/// NOTES ON USING THE GENERIC TIMER
	/// </summary>
	/// To set up a new timer first create the delegate function that will be called when the timer goes off:
	/// 	Action timerEndAction = delegate(){MyFunction(param1, param2, param3...);};
	/// 
	/// Create a new Generic Timer game object and Set teh Timer
	/// 	GenericTimer myGenericTimer = gameObject.AddComponent<GenericTimer>();
	///		myGenericTimer.Set( durration, false, timerEndAction ); 
	/// 
	/// Once the timer is set You can run it and pause it at anytime
	/// 	myGenericTimer.Run();
		

	
	private float _timer;
	private float _durration;
	private bool _repeat;
	private bool _run;
	private Action _endAction;
	private float _delay;
	private bool _delayOn;
	
	
	// Use this for initialization
	void Start () {
	}
		
	/// -----------------------------------------------------------------------------
	/// SET
	/// <summary>Sets the Timer</summary>
	/// Params : (float) The amount of time before the timer goes off, 
	/// (bool) if true the timer will continue to repeat until explicetly told to stop
	/// (Action) a delegate function that will be called each time the timer goes off.
	/// Return : none
	/// -----------------------------------------------------------------------------
	public void Set(float i_durration, bool i_repeat, Action i_action=null, float i_delay=0)
	{
		_durration = i_durration;
		_repeat = i_repeat;
		_endAction = i_action;
		_delay = i_delay;
		
		if ( _delay > 0 )
			_delayOn = true;
		else
			_delayOn = false;
	}
	
	
	public void Run ()
	{
		_timer = _durration;
		_run = true;
	}
	
	
	public void Pause ()
	{
		_run = false;
	}
	
	
	public void SetDelay(float i_delay)
	{
		_delay = i_delay;
		_delayOn = true;
	}
	
	
	// Update is called once per frame
	void Update () {
		if ( _run )
		{
			if ( _delayOn )
			{
				_delay -= Time.deltaTime;
				if ( _delay <= 0 )
					_delayOn = false;
			}
			else
			{
				_timer -= Time.deltaTime;
				if ( _timer <= 0 )
				{
					Alarm ();
				}
			}
		}
	}
	
	
	public float PercentComplete()
	{
		if ( !_delayOn )
			return 1-(_timer/_durration);
		else 
			return 0.0f;
		
	}
	
	
	public float PercentCompleteSmooth()
	{
		if ( !_delayOn )
		{
			float actual = 1-(_timer/_durration);
			float modified = 1-((Mathf.Sin( (actual*Mathf.PI) + ((Mathf.PI)/2) )+1.0f)/2.0f);
			return modified;
		}
		else
		{
			return 0.0f;
		}
	}
	
	
	public float PercentCompleteEaseOut()
	{
		if ( !_delayOn )
		{
		float actual = 1-(_timer/_durration);
		float modified = (Mathf.Sin( (actual*(Mathf.PI/2)) ));
		return modified;
		}
		else{
			return 0.0f;
		}
	}
	
	public float PercentCompleteEaseIn()
	{
		if ( !_delayOn )
		{
		float actual = 1-(_timer/_durration);
		float modified = 1-(Mathf.Cos( (actual*(Mathf.PI/2)) ));
		return modified;
		}
		else{
			return 0.0f;
		}
	}
	
	private void Alarm()
	{
		if( _endAction != null)
		_endAction();
		
		if ( _repeat )
		{
			_timer = _durration;
		}
		else
		{
			_run = false;
			Destroy ( this );
		}
	}
	
	public void Reset()
	{
		_timer = _durration;
		_run = false;
	}
	
	public void Kill()
	{
		_run = false;
		Destroy ( this );
	}
}
