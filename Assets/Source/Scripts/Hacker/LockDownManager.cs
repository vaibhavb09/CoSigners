using UnityEngine;
using System.Collections;

public class LockDownManager : MonoBehaviour {
	
	GameObject[] _alarms;
	GameObject[] _lights;
	private static LockDownManager _instance;
	public GameObject MapPlane;	
	Color _red = new Color(1.0f, 0.0f, 0.0f, 1.0f);
	Color _white = new Color(1.0f, 1.0f, 1.0f, 1.0f);
	Color _trans = new Color(1.0f, 1.0f, 1.0f, 0.3f);
	
	
	bool _isAlarmTriggered = false;
	float _currentTime = 0.0f;
	float _flashingInterval = 0.5f;
	float _overrideInterval = 1.5f;
	
	Vector3 _orginalScale = new Vector3(0.2f, 0.2f, 0.2f);
	Vector3 _targetScale = new Vector3(1.0f, 1.0f, 1.0f);
	// Use this for initialization
	void Start () {
	 	_alarms = GameObject.FindGameObjectsWithTag("Alarm");
		_lights = GameObject.FindGameObjectsWithTag("LightForAlarm");
	}
	
	
	public static LockDownManager Manager
	{
		get
		{
			if(_instance == null)
			{
				_instance = new LockDownManager();			
			}
			return _instance;
		}
	}

	public LockDownManager () 
    { 
        _instance = this;
    }
	
	public void TriggerLockDownAnimation()
	{
		//Debug.Log("LockDown anim Triggered");
		_isAlarmTriggered = true;
		_currentTime = 0.0f;
		TriggerLights();
		TriggerAlarms();
	}
	
	public void DisableLockDownAnimation()
	{
		_isAlarmTriggered = false;
		DisableLights();
		DisableAlarms();
		MapPlane.renderer.material.color = _white;
		OverrideManager.Manager.GetOverrideNode().renderer.material.color = _white;
		
		// [SOUND_TAG] [Alarm_loop] [STOP]
		soundMan.soundMgr.silenceSource(null,GameManager.Manager.PlayerType);
		
		// Restarting the Background music
		soundMan.soundMgr.playOnSource(null,(GameManager.Manager.PlayerType == 1) ? "Theif_BGM_loop" : "Hacker_BGM_loop",true,GameManager.Manager.PlayerType,GameManager.Manager.PlayerType,0.6f);

	}
	
	
	private void TriggerLights()
	{
		//do nothing for now
		
		// [SOUND_TAG] [Alarm_loop] [START]
		GameObject infoNodeSource = GameObject.Find("InfoNodeSource");
		//soundMan.soundMgr.playOnSource(infoNodeSource.audio, "Lockdown_Music", true, -1, -1, 0.5f); Temporarily disabling Lockdown Music

		if( Application.loadedLevelName.CompareTo("JM_53") == 0 )
		{
			AudioSource source = GameObject.Find("InfoNodeSource").audio;
			soundMan.soundMgr.playOnSource(source,"Alarm_loop",true,GameManager.Manager.PlayerType,GameManager.Manager.PlayerType, 0.2f);
		}
		else
		{
			soundMan.soundMgr.playOnSource(null,"Alarm_loop",true,GameManager.Manager.PlayerType,GameManager.Manager.PlayerType, 0.2f);
		}
	}
	
	private void DisableLights()
	{
		if(_lights != null)
		{
			foreach(GameObject light in _lights)
			{
				light.light.color = _white;
			}
		}
		else
		{
			//Debug.Log("#Max: There is no alarm lights in this level!");
		}
		// [SOUND_TAG] [Alarm_loop] [STOP]

		if( Application.loadedLevelName.CompareTo("JM_53") == 0 )
		{
			AudioSource source = GameObject.Find("InfoNodeSource").audio;
			soundMan.soundMgr.silenceSource(source,GameManager.Manager.PlayerType);
		}
		else
		{
			soundMan.soundMgr.silenceSource(null,GameManager.Manager.PlayerType);
		}
//		GameObject infoNodeSource = GameObject.Find("InfoNodeSource");
//		soundMan.soundMgr.silenceSource( infoNodeSource.audio );
		// Restarting the Background music
		//soundMan.soundMgr.playOnSource(null,(GameManager.Manager.PlayerType == 1) ? "Theif_BGM_loop" : "Hacker_BGM_loop",true,GameManager.Manager.PlayerType,GameManager.Manager.PlayerType,0.6f);
		
	}
	
	private void TriggerAlarms()
	{
		if(_alarms != null)
		{
			foreach(GameObject alarm in _alarms)
			{
				alarm.GetComponentInChildren<Alarm>().Trigger();
			}
		}
	}
	
	private void DisableAlarms()
	{
		if(_alarms != null)
		{
			foreach(GameObject alarm in _alarms)
			{
				alarm.GetComponentInChildren<Alarm>().Disable();
			}
		}
		else
		{
			//Debug.Log("There are no alarming in this level?");
		}
	}
	
	// Update is called once per frame
	void Update () {
		
		if(_isAlarmTriggered)
		{
			_currentTime += Time.deltaTime;
			float warningPercentage = (_currentTime%_flashingInterval) / _flashingInterval;
			float overrideWarningPercentage = (_currentTime%_overrideInterval) / _overrideInterval;
			int counter = 0;
			if(warningPercentage < 0.4f)
			{
				warningPercentage = warningPercentage * 2.5f;
			}
			else if(warningPercentage >= 0.4f && warningPercentage <= 0.6f)
			{
				warningPercentage = 1.0f;
			}
			else if(warningPercentage > 0.6f)
			{
				warningPercentage = 2.5f - warningPercentage * 2.5f;
			}
			MapPlane.renderer.material.color = Color.Lerp(_white, _red, warningPercentage);

			OverrideManager.Manager.GetOverrideNode().renderer.material.color = Color.Lerp(_white, _trans, warningPercentage);


			//Debug.Log("MapFlaneColor Changed");
			foreach(GameObject _light in _lights)
			{
				_light.light.color = Color.Lerp(_white, _red, warningPercentage);
			}

			//overrideWarningPercentage = 1- overrideWarningPercentage;
			//OverrideManager.Manager.GetOverrideFrame().transform.localScale = Vector3.Lerp(_orginalScale, _targetScale, overrideWarningPercentage);

			if(overrideWarningPercentage < 0.25f)
			{
				overrideWarningPercentage = 1.0f - overrideWarningPercentage * 4.0f;
				if( OverrideManager.Manager.GetOverrideFrame() != null )
				{
					OverrideManager.Manager.GetOverrideFrame().renderer.enabled = true;
					OverrideManager.Manager.GetOverrideFrame().transform.localScale = Vector3.Lerp(_orginalScale, _targetScale, overrideWarningPercentage);
				}
			}
			else if(overrideWarningPercentage >= 0.25f && overrideWarningPercentage <= 0.75f)
			{
				overrideWarningPercentage = 0.0f;
			}
			else if(overrideWarningPercentage > 0.75f)
			{
				if( OverrideManager.Manager.GetOverrideFrame() != null )
				{
				   OverrideManager.Manager.GetOverrideFrame().renderer.enabled = false;
				   //OverrideManager.Manager.GetOverrideFrame().transform.localScale = Vector3.Lerp(_orginalScale, _targetScale, overrideWarningPercentage);
				}
			}
			
		}
	}
}
