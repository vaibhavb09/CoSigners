using UnityEngine;
using System.Collections;

public enum MovementState
{
	Stopped = 1,
	Crouching,
	Walking,
	Running
}

public class MovementScript : MonoBehaviour 
{
	public bool 			CrouchMode;						//The bool which switches state from crouch to run and back when using left control key.
	public float 			WalkSpeed; 						//This will not be used; but still kept if in future we need an actual running speed for theif.
	public float 			CrouchSpeed;					//The current Crouching Speed.
	public float			RunSpeed; 						//The current Running ( Default ) speed.
	public float 			CrouchHeight; 					//Desired Crouch Height
	public float			RunningHeight;					//Desired Running Height
	public float 			SpeedToCrouch; 					//Speed that would decide how fast you crouch

	private GuardOverlord 	m_gOverlord; 					//Guard Overlord reference
	private CharacterMotor 	m_chMotor;						//Character Motor reference
	private GameObject		m_thiefCamera;					//FPS Camera reference

	private MovementState	m_currentState;					//Private Thief Movement State
	public MovementState	CurrentMoveState				//Move state of theif for access from other scripts
	{
		get
		{
			return m_currentState;
		}
	}

	private AudioSource 	m_thiefSoundSource;				//Thief's Sound Source

	public void InitializeSounds()
	{
		m_thiefSoundSource = (AudioSource)gameObject.AddComponent("AudioSource");
		m_thiefSoundSource.playOnAwake = false;
		m_thiefSoundSource.maxDistance = 1.0f;
		m_thiefSoundSource.minDistance = 5.0f;
		m_thiefSoundSource.volume = 1.0f;
	}

	void Start()
	{
		if( GameManager.Manager.PlayerType == 1 )
		{
			m_currentState = MovementState.Stopped; //Starting Move state
			m_chMotor = GetComponent<CharacterMotor>(); 
			CrouchMode = false;
			SpeedToCrouch = 2f;
			WalkSpeed = RunSpeed = 4f;
			CrouchSpeed = 2f;
			CrouchHeight = 0.6f;
			RunningHeight = 1.08f;
			m_thiefCamera = GameObject.Find("FPSCamera");
			m_thiefCamera.transform.Translate( new Vector3( 0,0.7f, 0 ) );
			m_gOverlord = GuardOverlord.Manager; //Setting up the guard overlord

			//This plays the thief loop based on whether it's final level or not
			if( Application.loadedLevelName.CompareTo("JM_53") == 0 )
			{
				soundMan.soundMgr.playOnSource(null,"FinalLevelPart1",true,GameManager.Manager.PlayerType,GameManager.Manager.PlayerType,0.6f);
			}
			else
			{
				soundMan.soundMgr.playOnSource(null,"Theif_BGM_loop",true,GameManager.Manager.PlayerType,GameManager.Manager.PlayerType,0.6f);
			}
		}
		else
		{
			gameObject.GetComponent<FPSInputController>().enabled = false;
		}
	}

	void Update()
	{
		if( GameManager.Manager.PlayerType == 1 ) //Should happen only on thief
		{
			float _speed = 0.0f;
			//Debug.Log( m_chMotor.jumping.speedUpOnJump );
			if( Input.GetKeyDown( KeyCode.LeftShift ) )
			{
				CrouchMode = true;
				m_currentState = MovementState.Crouching;
				//GA.API.Design.NewEvent("Thief:CrouchModeActivated",Time.timeSinceLevelLoad, tr.position.x, tr.position.y, tr.position.z); //Analytics for crouching
			}
			if( Input.GetKey( KeyCode.LeftShift ) )
			{
				CrouchMode = true;
				m_currentState = MovementState.Crouching;
				//GA.API.Design.NewEvent("Thief:CrouchModeActivated",Time.timeSinceLevelLoad, tr.position.x, tr.position.y, tr.position.z); //Analytics for crouching
			}
			if( Input.GetKeyUp( KeyCode.LeftShift ) )
			{
				CrouchMode = false;
				m_currentState = MovementState.Running;
				//GA.API.Design.NewEvent("Thief:CrouchModeActivated",Time.timeSinceLevelLoad, tr.position.x, tr.position.y, tr.position.z); //Analytics for crouching
			}
			if( Input.GetKeyUp(KeyCode.Space) )
			{
				CrouchMode = false;
				m_currentState = MovementState.Running;
				//GA.API.Design.NewEvent("Thief:CrouchModeDeactivated",Time.timeSinceLevelLoad, tr.position.x, tr.position.y, tr.position.z);
			}
			if( Input.GetKeyDown( KeyCode.LeftControl ) ) // || Input.GetKeyDown( "joystick button 4" ) ) //To be activated if we get joysticks in
		   	{
				CrouchMode = !CrouchMode;

				if( CrouchMode )
				{
					m_currentState = MovementState.Crouching;
				}
				else
				{
					m_currentState = MovementState.Running;
				}
			}


			if( Input.GetKeyUp( KeyCode.A ) && Input.GetKeyUp( KeyCode.S ) &&
			    Input.GetKeyUp( KeyCode.W ) && Input.GetKeyUp( KeyCode.D ) && Input.GetKey( KeyCode.LeftShift ) )
			{
				m_currentState = MovementState.Stopped;
			}
			else
			{
				if( CrouchMode )
				{
					m_currentState = MovementState.Crouching;
				}
				else
				{
					m_currentState = MovementState.Running;
				}
			}

			//Move State management
			if( m_currentState == MovementState.Running ) //This is the default move state
			{
				_speed = RunSpeed;
				soundMan.soundMgr.playOnSource(m_thiefSoundSource,"Thief_Run_loop",true,GameManager.Manager.PlayerType);
				if( m_gOverlord != null )
				{
					m_gOverlord.mThiefIsRunning(m_thiefCamera.transform);
				}
			}
			else if( m_currentState == MovementState.Crouching )
			{
				_speed = CrouchSpeed;
				soundMan.soundMgr.playOnSource(m_thiefSoundSource,"Thief_Crawl_loop",true,GameManager.Manager.PlayerType);
			}
			else if( m_currentState == MovementState.Walking )
			{
				Debug.Log("THIS SHOULDN'T BE CALLED! CHECK MOVEMENT SCRIPT IMMEDIATELY");
				_speed = WalkSpeed;
				soundMan.soundMgr.playOnSource(m_thiefSoundSource, "Thief_Crawl_loop", true, GameManager.Manager.PlayerType);
			}
			else if( m_currentState == MovementState.Stopped )
			{
				_speed = 0;
				soundMan.soundMgr.silenceSource(m_thiefSoundSource);
			}
			m_chMotor.movement.maxForwardSpeed = _speed; // set max speed
			m_chMotor.movement.maxSidewaysSpeed = _speed;
			m_chMotor.movement.maxBackwardsSpeed = _speed;
			SwitchCrouchingAndRunning( CrouchMode );
		}
	}

	void SwitchCrouchingAndRunning( bool i_value )
	{
//		if( i_value )
//		{
//			float _desiredHeight = CrouchHeight;
//
//			Vector3 _tempScale = transform.localScale;
//			Vector3 _tempPosition = transform.position;
//
//			if( _tempScale.y > _desiredHeight )
//			{
//				_tempScale.y -= ( SpeedToCrouch * _tempScale.y / 10f );
//			}
//
//			transform.localScale = _tempScale;
//
//			if( ( _tempPosition.y- ( SpeedToCrouch * _tempScale.y / 10f ) > _desiredHeight ) && ( _tempPosition.y - ( SpeedToCrouch * _tempScale.y / 10f ) > 0.5f ) )
//			{
//				_tempPosition.y -= ( SpeedToCrouch * _tempScale.y / 10f );
//			}
//			else
//			{
//				Debug.Log("I'll fall now. WEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEE" );
//			}
//			transform.position = _tempPosition;
//			_tempScale.y = Mathf.Lerp( transform.localScale.y, CrouchHeight, 5 * Time.deltaTime );
//			transform.localScale = _tempScale;
//
//			_tempPosition.y += transform.localScale.y - _desiredScale;
//			if( _tempPosition.y < 0.5f )
//			{
//				_tempPosition.y += 0.2f;
//			}
//			transform.position = _tempPosition;
//		}
//		else
//		{
//			float _desiredHeight = RunningHeight;
//			
//			Vector3 _tempScale = transform.localScale;
//			Vector3 _tempPosition = transform.position;
//			
//			if( _tempScale.y < _desiredHeight )
//			{
//				_tempScale.y += ( SpeedToCrouch * _tempScale.y / 10f );
//			}
//
//			transform.localScale = _tempScale;
//			
//			if( _tempPosition.y < _desiredHeight )
//			{
//				_tempPosition.y += ( SpeedToCrouch * _tempScale.y / 10f );
//			}
//
//			transform.position = _tempPosition;
//			float _desiredScale = RunningHeight;
//			Vector3 _tempScale = transform.localScale;
//			Vector3 _tempPosition = transform.position;
//			
//			_tempScale.y = Mathf.Lerp( transform.localScale.y, CrouchHeight, 5 * Time.deltaTime );
//			transform.localScale = _tempScale;
//			
//			_tempPosition.y += _desiredScale - transform.localScale.y;
//			if( _tempPosition.y < 0.5f )
//			{
//				_tempPosition.y += 0.2f;
//			}
//			transform.position = _tempPosition;		}
		float ultScale = transform.localScale.y;
		float vScale = 0, dist = 1;
		if( i_value )
		{
			vScale = CrouchHeight;
		}
		else
		{
			vScale = RunningHeight;
		}
		Vector3 tmpScale = transform.localScale;
		Vector3 tmpPosition = transform.position;
		
		tmpScale.y = Mathf.Lerp(transform.localScale.y, vScale, 5 * Time.deltaTime);
		transform.localScale = tmpScale;
		
		//Debug.Log ("Ult Scale:"+ultScale);
		//Debug.Log ("Local Scale:"+tr.localScale);
		
		tmpPosition.y += dist * (transform.localScale.y - ultScale); // fix vertical position       
		if(tmpPosition.y < .5)
		{
			tmpPosition.y += .2f;	
		}
		transform.position = tmpPosition;
	}
}