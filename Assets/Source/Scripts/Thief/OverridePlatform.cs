using UnityEngine;
using System.Collections;

public class OverridePlatform : MonoBehaviour {
	
	private bool _startEnable = false;
	private bool _startDisable = false;
	private bool _isEnabled = false;
	private bool _isOverrided = false;
	private bool _isClosing = false;
	private bool _startActivate = false;
	private bool _finishActivate = false;
	private Vector3 _startPosition;
	
	private float _currentTimer = 0.0f;
	
	public bool Activated
	{
		get
		{
			return _isEnabled;
		}
	}
	
	public void Enable()
	{
		
		// Oh shit button Revealed [SOUND TAG] [OS_Button_Reveal]
		// soundMan.soundMgr.playOneShotOnSource(this.audio,"OS_Button_Reveal",GameManager.Manager.PlayerType);
		if(!_startEnable)
		{
			PlayEnableEffect();
			_startEnable = true;
			//_isEnabled = true;
			//Debug.Log("Trying to enable");

			foreach ( Transform child in transform )
			{
				if ( child.name.Contains("GlowPlane") )
					child.transform.renderer.enabled = true;



				if ( child.name.Contains("ST_Piston") )
				{
					Transform screen = child.FindChild("ST_Terminal").FindChild("GlowingPlanes");
					screen.transform.renderer.material = Resources.Load("Meshes/Materials/GlowScreens_Override_Active") as Material;
				}

				if ( child.name.Contains("ST_ComputerPiston") )
				{
					foreach( Transform overrideInterface in child )
					{
						if ( overrideInterface.name.Contains("ST_Computer") )
						{
							Transform screen = overrideInterface.FindChild("Override_Interface");
							screen.transform.renderer.material = Resources.Load("Meshes/Materials/Override_TouchPanel_Active") as Material;
						}
					}
				}
			}
		}
	}
	
	public void Disable()
	{
		//Debug.Log("Who is disabling this?");
		if(_startEnable || _isEnabled)
		{
			PlayDisableEffect();
			_startEnable = false;
			_isEnabled = false;

			foreach ( Transform child in transform )
			{
				if ( child.name.Contains("ST_Piston") )
				{
					Transform screen = child.FindChild("ST_Terminal").FindChild("GlowingPlanes");
					screen.transform.renderer.material = Resources.Load("Meshes/Materials/GlowScreens_Override_Inactive") as Material;
				}

				if ( child.name.Contains("ST_ComputerPiston") )
				{
					foreach( Transform overrideInterface in child )
					{
						if ( overrideInterface.name.Contains("ST_Computer") )
						{
							Transform screen = overrideInterface.FindChild("Override_Interface");
							screen.transform.renderer.material = Resources.Load("Meshes/Materials/Override_TouchPanel_Inactive") as Material;
						}
					}
				}
			}
		}
	}
	
	public void StartOverride()
	{
		// Oh shit button pressed [SOUND TAG] OS_Button_Reveal
		// soundMan.soundMgr.playOneShotOnSource(this.audio,"OS_Button_Reveal",GameManager.Manager.PlayerType);
		
		PlayOverrideEffect();
		_isClosing = true;
		_isOverrided = true;
	}
	
	private void FinishOverride()
	{
		//Debug.Log("Override Finished on the floor");
		NetworkManager.Manager.OverrideSuccess();
		//Destroy(gameObject);
	}
	
	private void PlayEnableEffect()
	{
		//_startPosition = transform.position;
		//if( !animation.IsPlaying("Close") )
		//{
		//	animation.Play("Open");
		//}
		//else
		//{
		//	animation.CrossFade("Open");
		//	Debug.Log("This happened?");
		//}
		
		//if(animation.IsPlaying("Open_Override"))
		//{
			
		//	animation.CrossFade("Activate_Override");
		//}
		
		//if(animation.IsPlaying("OpenLoop_Override"))
		//{
		//	Debug.Log("Activate Override Platform");
		//	animation.CrossFade("Activate_Override");
		//}
		_currentTimer = 0.0f;
		animation.Play("Activate_Override");
		_startActivate = true;
		//Debug.Log("Play Activate Override Animation");
	}
	
	private void PlayDisableEffect()
	{	
		//animation.Stop("LoopActivate_Override");
		animation.Play("Retract_Override");
		//Debug.Log("Play Retract_Override Animation");
	}
	
	private void PlayOverrideEffect()
	{
		//Debug.Log("Start Overriding...");
		animation.CrossFade("Close_Override");
		//Debug.Log("Play Close Override Animation");
		//should be in update loop, but we don't have the animation yet
		
	}
	
	// Use this for initialization
	void Start () {
		_startPosition = transform.position;
		animation.Play("Open_Override"); 

		foreach ( Transform child in transform )
		{
			if ( child.name.Contains("GlowPlane") )
				child.transform.renderer.enabled = false;
			//Debug.Log("Child is: " + child.name);
		}
		//Transform glow2 = transform.Find("OverridePlatform/Display/GlowPlane2");
		//Debug.Log("This Glow plane is: " + glow2.name);
		//glow2.renderer.enabled = false;
		//Transform glow3 = transform.Find("OverridePlatform/Display/GlowPlane3");
		//Debug.Log("This Glow plane is: " + glow2.name);
		//glow3.renderer.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		if(!_startEnable && !_isEnabled &&!_isOverrided && 
			!animation.IsPlaying("Open_Override") && !animation.IsPlaying("LoopOpen_Override")
			&& (!animation.IsPlaying("Retract_Override")))
		{
			animation.Play("LoopOpen_Override");
			//Debug.Log("Play Loop Open Override Animation");
		}
		
		//if(_startEnable && _startActivate == false)
		//{
		//	if(!animation.IsPlaying("Open_Override"))
		//	{
		//		_currentTimer = 0.0f;
		//		animation.Play("Activate_Override");
		//		_startActivate = true;
		//	}
		//}
		
		if(_startEnable && !animation.IsPlaying("LoopOpen_Override"))
		{
			
			if(!animation.IsPlaying("Activate_Override") && _currentTimer > 0.0f)
			{
				_isEnabled = true;
				animation.Play("LoopActivate_Override");
				_startEnable = false;
				//Debug.Log("Play Loop Activate Override Animation");
			}
		}
		//Code snippet to disable the override platform after the closing animation is done playing.
		if(_isClosing)
		{
			if( !animation.IsPlaying("Close_Override") )
			{
				OverrideManager.Manager.CloseOverridePlatform();
				_isClosing = false;
			}
		}
		if(_isOverrided) // && !animation.IsPlaying("Close_Override"))
		{
			FinishOverride();
			_isOverrided = false;
		}
		_currentTimer += Time.deltaTime;
	}
}
