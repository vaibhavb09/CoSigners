using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tracer : MonoBehaviour {
	
	private enum TracerStates { Delaying=0, Calibrating=1, Active=2, Dead=3}
	
	int hexIndex;
	TracerCreator.creatorType type;
	public float delayTicker;
	public float delayTime;
	public float calibrationTicker;
	public float calibratingTime;
	public float activeTicker;
	public float activeTime;
	private TracerStates myState;
	public GameObject myPrefab;
	List<int> intersects;
	float alertMeter;
	float alertThreshold;
	bool _initialFrame;
	bool scrambled=false;
	int numberOfScramblersOnMe=0;
	private bool _hackerWarning = false;
	private bool _onScreen = false;
	private float _caliPercentage = 0.0f;
	private float _displayAngle = 0.0f;
	public TracerAnimationNew _animation;
	
	
	#region Properites
	public float CalibratePercentage
	{
		get{
			return _caliPercentage;
		}
	}
	
	public bool OnToHacker
	{
		get{
			return _hackerWarning;
		}
	}
	
	public float DisplayAngle
	{
		get{
			return _displayAngle;
		}
	}
	
	public bool OnScreen
	{
		get{
			return _onScreen;
		}
	}
	#endregion
	// Use this for initialization
	void Start () {
	}
	
	
	public void Set(int i_index, Vector3 i_pos, float i_delay, float i_calibratingTime, float i_activeTime)
	{
		//Debug.Log ("T******* SET TRACRER SETTINGS ");
		_initialFrame = true;
		hexIndex = i_index;
		//Debug.Log(delayTime);
		delayTime = i_delay;
		activeTime = i_activeTime;
		calibratingTime = (i_calibratingTime<1.0f)? 1.0f : i_calibratingTime;
		delayTicker = 0;
		calibrationTicker = 0;
		activeTicker = 0;
		
		numberOfScramblersOnMe=IsScramblerAround(hexIndex);
		if(numberOfScramblersOnMe>0)
		{
			scrambled=true;
		}
		else
			scrambled=false;
		
			
		if ( GameManager.Manager.PlayerType == 1) // If the player is the thief Change the position
		{
			//i_pos = new Vector3 (i_pos.x, 0.02f, i_pos.z);
			//gameObject.transform.position = i_pos;
			//gameObject.renderer.enabled = false;
		}
		else
		{
			gameObject.transform.position = i_pos;
		}
	
		_animation = gameObject.GetComponent<TracerAnimationNew>();
		_animation.Set(calibratingTime, activeTime, this);
		//_animation.SetTimer(delayTime, calibratingTime, activeTime);

		myState = TracerStates.Delaying;			
		intersects = HexGrid.Manager.GetHexIntersects(hexIndex);
		if ( intersects.Count != 6)
		{
			//Debug.LogError("Tracer Hex " + hexIndex + " is invalid");
		}
		
		alertMeter = 0.0f;
		alertThreshold = 1.0f;
		//myPrefab.renderer.enabled = false;
	}
	
	public void CalcPosition()
	{
		GameObject playerCamera = (GameObject)GameObject.Find ("TopDownCamera");
		Vector3 playerForward = playerCamera.transform.up;
		Vector3 playerRight = playerCamera.transform.right;
		Vector3 targetDir = transform.position - playerCamera.transform.position;
		targetDir.y = 0.0f;
		
		float angleFwd = Vector3.Angle(targetDir, playerForward);
		//Vector3 tracerPosition = new Vector3(transform.position.x, transform.position.y+1.5f, transform.position.z);
		Vector3 screenPos = playerCamera.camera.WorldToScreenPoint( transform.position );
		_onScreen = (screenPos.x<Screen.width && screenPos.x>0 && screenPos.y<Screen.height && screenPos.y>0);
			
		// Determine if Guard is to right or left
		float angleRt = Vector3.Angle(targetDir, playerRight);
		bool right = (angleRt<90.0f);
		
		// Adjust displayed angle Based on camera angle ( right now assumes Level)
		//float camAngle = 90; // Looking up decreases angle, looking down increases.
		//float displayAngle = (((angleFwd-20)/170.0f)*110)+70;
		float displayAngle = angleFwd;
		
		// Convert Angle to GUI Coordinates
		if ( right )
		{
			if ( displayAngle <= 90 )
			{
				_displayAngle = Mathf.Abs(displayAngle-90);
			}
			else
			{
				_displayAngle = Mathf.Abs(displayAngle-450);
			}
		}
		else
		{
			_displayAngle = displayAngle + 90;
		}
	}
	
	void Update () {
		

		if ( myState==TracerStates.Delaying )
		{
			delayTicker += Time.deltaTime;
			//_animation.SetDelayAnimation(delayTicker);
			GameManager.Manager.HackerCaught=false;
			
			if ( delayTicker >= delayTime )
			{
				//_animation.ReSetLoadBarProgress();
				//Debug.Log ("****** Setting Tracer to CALIBRATING");
				_animation.Run();
				myState = TracerStates.Calibrating;
				
			}
			_hackerWarning = false;
		}
		else if ( myState==TracerStates.Calibrating && calibratingTime != 0.0f)
		{
			calibrationTicker += Time.deltaTime;
			//_animation.SetCalibrateAnimation(calibrationTicker);
			GameManager.Manager.HackerCaught=false;
			
			if( ScanIntersects() )
			{
				// [NEED SOUND]
				// AudioManager.Manager.HackerConnectsToCalibratingTracer( true );	
				_hackerWarning = true;
			}
			_caliPercentage = calibrationTicker/calibratingTime;
			if ( calibrationTicker >= calibratingTime )
			{
				//_animation.ReSetLoadBarProgress();
				myState = TracerStates.Active;
				_animation.EndCalibrationTimer();
			}
		}
		else if ( myState==TracerStates.Calibrating && calibratingTime == 0.0f)
		{
			//_animation.ReSetLoadBarProgress();
			myState = TracerStates.Active;
		}
		else if ( myState==TracerStates.Active )
		{
			if(!scrambled && GameManager.Manager.PlayerType == 2)	
			{
				if (ScanIntersects())
				{
					// Hacker is Connected to active tracer
					if(_initialFrame)
					{
						// [NEED SOUND]
						// AudioManager.Manager.HackerPoweringActivePoweredTracer( true );
						_initialFrame = false;
					}
					alertMeter += Time.deltaTime;
					_hackerWarning = true;
					if ( alertMeter >= alertThreshold )
					{
							// hacker is Caught
							CatchHacker();
							_hackerWarning = false;
							alertMeter = 0.0f;
						
					}
				}
				else
				{
					// Hacker is not Connected.
					alertMeter = 0.0f;
					
					// [NEED SOUND]
					// AudioManager.Manager.HackerPoweringActiveTracerStop( true );//only stops the loop. No sound.
					_initialFrame = true;
				}
			}
			
			activeTicker += Time.deltaTime;
			//_animation.SetActiveAnimation(activeTicker);
			if ( activeTicker >= activeTime )
			{
				//_animation.ReSetLoadBarProgress();
				myState = TracerStates.Dead;
				//Debug.Log ("Tracer:" + hexIndex + "NOW DEAD");
				SelfDestruct();
			}
		 
		}
	}
	
	public int IsScramblerAround(int i_hexIndex)
	{
		
		List<int> surroundingHexes=HexGrid.Manager.GetSurroundingHexes(i_hexIndex);
		
		int scramblerCount=0;
		Transform tempScrambler=null;
		
		if ( GameManager.Manager.PlayerType == 1)
		{
			if(ThiefGrid.Manager.ActiveScramblers==null)
			   return 0;	 
		}
		else
		{
			if(HexGrid.Manager.scramblers==null)
			return 0;
		}
		
			
		if ( GameManager.Manager.PlayerType == 1)
		{
		   ThiefGrid.Manager.ActiveScramblers.TryGetValue(i_hexIndex,out tempScrambler); 
			if(tempScrambler!=null)
				scramblerCount++;
		}
		else
		{
			HexGrid.Manager.scramblers.TryGetValue(i_hexIndex,out tempScrambler); 
			if(tempScrambler!=null)
				scramblerCount++;
		}
		
		for(int i=0;i< surroundingHexes.Count;i++)
		{
			Transform _tempScrambler=null;
			
			if ( GameManager.Manager.PlayerType == 1)
			{
				ThiefGrid.Manager.ActiveScramblers.TryGetValue(surroundingHexes[i],out _tempScrambler); 
				if(_tempScrambler!=null)
					scramblerCount++;
			}
			else
			{
				HexGrid.Manager.scramblers.TryGetValue(surroundingHexes[i],out _tempScrambler); 
				if(_tempScrambler!=null)
					scramblerCount++;
			}
		}
		
		return scramblerCount;
	}
	
	public int getHexIndex()
	{
		return hexIndex;
	}
	
	public void Scramble()
	{
		numberOfScramblersOnMe++;
		//Debug.Log("Increasing count");
		if(numberOfScramblersOnMe>0)
			scrambled=true;
	}
	
	public void DeScramble()
	{
		numberOfScramblersOnMe--;
		//Debug.Log("Decreasing Count");
		if(numberOfScramblersOnMe==0)
		{
			numberOfScramblersOnMe=0;
			scrambled=false;
		}
	}
	
	
	private void SelfDestruct()
	{
		//myCreator.DestroyTracer(this);
		SecurityManager.Manager.DestroyTracer(hexIndex);
		_animation.RemoveTracer();
		Destroy (gameObject);
	}
	
	private bool ScanIntersects()
	{
		for ( int i=0 ; i<intersects.Count ; i++ )
		{
			if ( ConnectionManager.Manager.IsConnected( intersects[i]) )
			{
				return true;
			}
		}
		return false;
	}
	
	private void CatchHacker()
	{
		if( !OverrideManager.Manager.IsActive && !ThiefManager.Manager.gameIsPaused )
		{
			// AudioManager.Manager.HackerCaughtByTracer( true );
			
			// Tracer caught you [SOUND TAG] [Tracer_Capture_Hacker]
			soundMan.soundMgr.playOneShotOnSource(null,"Tracer_Capture_Hacker",GameManager.Manager.PlayerType,2);
			NetworkManager.Manager.SetTimesCaughtByTracer(1);
			//GA.API.Design.NewEvent("hacker:CaughtByTracer", Time.timeSinceLevelLoad, transform.position.x, transform.position.y, transform.position.z);
			
			HexGrid.Manager.RemovePivotsInRange(hexIndex);
			if(!GameManager.Manager.HackerCaught)
				NetworkManager.Manager.BoostAlertLevelForTime( 1.0f,  SecurityManager.Manager.Damage );
			
			if ( SecurityManager.Manager.TypeOfTracerOnHex( hexIndex ) != TracerCreator.creatorType.Static )
				GameManager.Manager.HackerCaught = true;
		//	Debug.Log ("HACKER IS CAUGHT!!!!!!!!!!!!!!!!!  " + GameObject.Find("TestingModeController").GetComponent<TestingMode>().testingMode);
			
		}
	}
	

}
