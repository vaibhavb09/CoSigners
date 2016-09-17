using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//#if UNITY_EDITOR
//using UnityEditor;
//#endif

public class AlertSystem : MonoBehaviour {
	
	#region Members
	public float m_level, m_posX, m_posZ, m_diffX, m_diffZ, m_angle;
	private GameObject m_player;
	private GuardPerception _myVision;
	public bool m_wasFull, isSeeingFirstTime;
	private bool _isAlert;
	private float _minAlertTime, m_timer;
	private bool _visible;
	private const float STRAIGHT_IN_FRONT = 0.0f;
	private const float DIRECTLY_BEHIND = 180.0f;
	private const float FULL_ALERT = 100.0f;
	private const float NOT_ALERT = 0.0f;
	public Material PatrolingMaterial;
	public Material SeenMaterial;
	public Material AlertMaterial;
	public Texture2D AlertBackground;
	public Texture2D AlertForeground;
	GUIStyle style1;
	GUIStyle style2;
	public float guardHeatMeter = 0.0f;
	public float guardHeatMeterMaxLvl = 150.0f;
	public float guardHeatMeterMultiplier = 1.0f;
	public float guardCoolDownTime = 20.0f;
	private float guardHeatAmount = 0.0f;
	private float lastSeenPlayerTime = 0.0f;
	#endregion
	
	# region properties
	public bool IsAlert
	{
		get{
			return _isAlert;
		}
	}
	
	public float AlertLevel
	{
		get{
			return m_level;
		}
	}
	#endregion
	
	// Use this for initialization
	void Start () {
		m_level = m_timer = 0.0f; //Default value
		m_wasFull = false;
		_isAlert = false;
		_minAlertTime = 1.0f;
		m_player = GameObject.Find("Playertheif(Clone)");
		_myVision = gameObject.GetComponent<GuardPerception>();
		
		PatrolingMaterial = Resources.Load ("Materials/Thief/GuardLightPatroling", typeof(Material)) as Material;
		SeenMaterial = Resources.Load ("Materials/Thief/GuardLightSeen", typeof(Material)) as Material;
		AlertMaterial = Resources.Load ("Materials/Thief/GuardLightAlert", typeof(Material)) as Material;
		AlertBackground = Resources.Load ("Textures/RedAlertBkg", typeof(Texture2D)) as Texture2D;
		AlertForeground = Resources.Load ("Textures/RedAlert", typeof(Texture2D)) as Texture2D;
		style1 = new GUIStyle();
		style1.normal.background = AlertBackground;
		style2 = new GUIStyle();
		style2.normal.background = AlertForeground;
	}
	
	// Update is called once per frame
	void Update () {

		if(GameManager.Manager.PlayerType == 1) // is thief
		{
			if (Time.time - lastSeenPlayerTime > guardCoolDownTime)
			{
				guardHeatMeter = 0.0f;
			}
			
			if ( _myVision.Seen || _myVision.wasPlayerHeard )
			{   
				lastSeenPlayerTime = Time.time;
				if ( _isAlert )
				{
					ChangeAlertMeter( true, 2.0f); // Increase Quickly
					 //change material to alert
				     Transform temp = this.transform.FindChild("soldier/Drone (1)/Drone_Red_Mesh");
				     //temp.renderer.material=AlertMaterial;
				}
				else
				{
					//Debug.Log("In else part of seen block");
					ChangeAlertMeter( true, (CalculateRate()/2) ); // Increase Alert Meter Based on Vision Angle
					
				}
				if( isSeeingFirstTime == true && !_isAlert )
				{
					
					//AudioManager.Manager.ThiefSeenByGuard( true );
					NetworkManager.Manager.SetTimesSeenByGuard(1);
					m_timer = Time.time;
					isSeeingFirstTime = false;
					
					//change material to seen
					Transform temp = this.transform.FindChild("soldier/Drone (1)/Drone_Red_Mesh");
					//temp.renderer.material=SeenMaterial;
				}
				
			}
			else
			{
				isSeeingFirstTime = true;
				//change material to seen
				Transform temp = this.transform.FindChild("soldier/Drone (1)/Drone_Red_Mesh");
				//temp.renderer.material=SeenMaterial;
				if ( _isAlert )
				{
					ChangeAlertMeter( false, 0.1f ); // Decrease Meter Slowly
				}
				else
				{
					ChangeAlertMeter( false, 0.5f ); // Decrease Alert Meter Quickly
				}
				//AudioManager.Manager.ThiefLosesGuardByVision( true );
				BasicScoreSystem.Manager.TimesAvoided += 1;
			}
			
			
			// if Alert Meter is anything but 0 Display it on screen
			if ( m_level > 0 )
			{
				gameObject.GetComponent<AlertMeter>().SetAlertPosition();
				_visible = true;
				//Debug.Log ("Alert Meter Should be visible");
			}
			else
			{
				_visible = false;
				//change material to patroling
				Transform temp = this.transform.FindChild("soldier/Drone (1)/Drone_Red_Mesh");
				//temp.renderer.material=PatrolingMaterial;
					
				//Debug.Log("material:" +temp.renderer.material);
				
			}
		}
	}

	
	private void ChangeAlertMeter(bool i_increasing, float rate)
	{	guardHeatAmount = (Time.deltaTime * (rate + guardHeatMeterMultiplier * (guardHeatMeter/guardHeatMeterMaxLvl)))*100.0f;
		float amount = (Time.deltaTime * rate)*100.0f;
		
//		if( Input.GetKey( KeyCode.LeftShift ) )
//		{
//			guardHeatAmount = guardHeatAmount / 2;   Code to check whether the thief is crouching. Legacy code. Change asap.
//			amount = amount / 2;
//		}
		
		float diff = 0.0f;
		//Debug.Log ("i_increasing - "+ i_increasing + " _isAlert - " + _isAlert + " m_level - " + m_level + " rate - "  + rate);
		// Change Alert Level
		if (i_increasing )
		{
			if(m_level + guardHeatAmount > 100.0f)
			{
				m_level = 100.0f;
			}
			else
			{
			m_level = m_level + guardHeatAmount;
			}
			
			if (guardHeatMeter + amount > guardHeatMeterMaxLvl)
			{
				guardHeatMeter = guardHeatMeterMaxLvl;
			}
			else
			{
				guardHeatMeter+=amount;
			}
			
			diff = Time.time - m_timer;
		}

		else if ( !i_increasing && m_level > 0)
		{
			m_level -= amount;
			m_timer = Time.time;
			if( !_isAlert )
			{
				//AudioManager.Manager.ThiefLosesGuardByAlertMeter( true );	
				BasicScoreSystem.Manager.TimesAvoided += 1;
			}
		}
		
		if( i_increasing && diff >= 0.5f ) //Repeat at half a second.
		{
			m_timer = Time.time;
			//AudioManager.Manager.ThiefGuardisSeeingYou( true );	
		}
		
		// Check for state changes
		if ( !_isAlert && i_increasing && m_level >= 100.0f ) // Guard has reached full alert
		{
			//Debug.Log ("FULL ALERT!!!");
			_isAlert = true;
			m_level = 100.0f;
			//AudioManager.Manager.ThiefStartedGettingChasedByGuard( true );
			
			//This should stop the trolling. Threat increases when the Alert meter is full.
			NetworkManager.Manager.BoostAlertLevelForTime( 0.25f, ThiefManager.Manager.AlertDamage );
			
			// The guard is now alert [SOUND TAG] [Guard_Alerted_loop , Start]
			soundMan.soundMgr.playOnSource(this.audio,"Guard_Alerted_loop",true,GameManager.Manager.PlayerType);			
		}
		else if ( _isAlert && !i_increasing && m_level <= 0.0 ) // Guard is no longer alert
		{
			//Debug.Log ("Not Alert!!!");
			//AudioManager.Manager.ThiefLosesGuardByAlertMeter( true );
			BasicScoreSystem.Manager.TimesAvoided += 1;
			_isAlert = false;
			m_level = 0.0f;
		}
		
		
	}
	
	
	
	// Rate is represented in 100% fill per second
	// so a value of 1.0 will move the alert meter from 0 to 100% in 1 second.
	// a value of 0.5 will move the alert meter from 0 100% in 2 seconds.
	private float CalculateRate()
	{
		//Calculation of level
		float VisionStrength = _myVision.perceptionStrength; 	// Floating-point vision strength
	
		if ( VisionStrength > 0 && VisionStrength < 1 )
		{
			return VisionStrength;
		}
		else
		{
			VisionStrength = VisionStrength/100.0f;
		}
	
		return VisionStrength;
		
	}
	
	private bool AreAlmostEqual( float a, float b, float maxDiff )
	{
		float diff = Mathf.Abs( a - b );
		a = Mathf.Abs( a );
		b = Mathf.Abs( b );
		
		float largest = ( b > a ) ? b : a;
		
		if( diff <= largest * maxDiff )
			return true;
		return false;
	}
	
	
	private bool NormalizeVisionStrength( ref float o_visionStrength )
	{
		if( o_visionStrength > 1 && o_visionStrength < 100 ) 									//if Vision Strength is between 0 to 100; make it between 0 to 1.
		{
			o_visionStrength = o_visionStrength/100;	
			return true;
		}
		else if( o_visionStrength >= 0 && o_visionStrength <= 1 ) 								//if Vision Strength is between 0 to 1; use it as is.
		{
			return true;
		}
		return false;
	}
	
	
	private void DecreaseAwarenessLevel()
	{
		if( m_wasFull == false )
		{
			m_level = 0;	
		}
		else if( AreAlmostEqual( m_level, FULL_ALERT, 0.001f ) && (  m_level > 0.0f || AreAlmostEqual(m_level, NOT_ALERT, 0.001f ) ) )
		{
			m_level -= 1;
		}
		
		if( AreAlmostEqual( m_level, NOT_ALERT, 0.001f ) )
		{
			m_wasFull = false;	
		}
	}
	
	public bool GetVisibility()
	{
		return _visible;	
	}
}
