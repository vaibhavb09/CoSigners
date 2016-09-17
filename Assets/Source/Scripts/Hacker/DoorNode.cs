using UnityEngine;
using System.Collections;
using System;

public class DoorNode : Node {

	//Indicates what door type this node represents.
	public DoorType 		_doorType;
	public DoorState		_doorState;
	public bool 			isOpen;
	public bool 			_disabled;
	
	//End door inner and outer ring animation.
	public float 			rotationSpeed;
	public float 			startAngle;
	public float 			endAngle;
	public bool 			animating;
	private float 			currentAngle;
	private Transform 		ring1;
	private Transform 		ring2;
	GenericTimer 			_ringTimer;
	GenericTimer 			_disableTimer;
	public GameObject 		DisablePrefab;
	public GameObject		DisableIndicator;
	public GameObject 		TimerPrefab;
	public GameObject		DisableTimer;
	public GameObject 		ShinePrefab;
	public GameObject		DoorShine;

	private Texture2D		DisableBaseTex, Shine_01, Shine_02, Shine_03, Shine_04, Shine_05;

	public DoorNode(){}
	
	public void Set( DoorNodeData i_data )
	{
		Index = i_data.Index;
		SecurityLevel = i_data.SecurityLevel;
		PowerConsumption = 5;
		_doorState = DoorState.UNPOWERED;
		Type = GameManager.NodeType.Door;
		float myScale = GraphManager.Manager.DefaultScale;
		this.gameObject.transform.localScale = new Vector3(myScale, myScale, myScale);
		isOpen = !i_data.Closed;
		currentAngle = startAngle;
		animating = false;
		_doorType = CalcDoorType(i_data.DoorType);
		SyncDoorType(_doorType);
		Connected = (_doorType == DoorType.StartDoor)? true:false;
		_ringTimer = gameObject.AddComponent<GenericTimer>();

		DisablePrefab = (GameObject) Resources.Load("Prefabs/Hacker/Graph/DoorDisableBase");
		DisableIndicator = (GameObject)Instantiate( DisablePrefab, HexGrid.Manager.GetCoord(Index, 61.0f), Quaternion.identity);
		DisableIndicator.transform.localScale = new Vector3(0.12f, 0.12f, 0.12f);
		DisableIndicator.transform.renderer.enabled = false;

		TimerPrefab = (GameObject) Resources.Load("Prefabs/Hacker/Graph/DoorDisableTimer");
		DisableTimer = (GameObject)Instantiate( TimerPrefab, HexGrid.Manager.GetCoord(Index, 61.1f), Quaternion.identity);
		DisableTimer.transform.localScale = new Vector3(0.12f, 0.12f, 0.12f);
		DisableTimer.transform.renderer.enabled = false;

		DisableBaseTex = (Texture2D) Resources.Load ("Textures/HackerNodes/Disable_Timer_02");
		Shine_01 = (Texture2D) Resources.Load ("Textures/HackerNodes/DoorShine_01");
		Shine_02 = (Texture2D) Resources.Load ("Textures/HackerNodes/DoorShine_02");
		Shine_03 = (Texture2D) Resources.Load ("Textures/HackerNodes/DoorShine_03");
		Shine_04 = (Texture2D) Resources.Load ("Textures/HackerNodes/DoorShine_04");
		Shine_05 = (Texture2D) Resources.Load ("Textures/HackerNodes/DoorShine_05");
		
		_disabled = false;
	}

	void Start () {}

	// Update is called once per frame
	void Update ()
	{
		if(animating) 
		{
			PlayAnimation();
		}
	}

	public DoorType CalcDoorType( string i_type )
	{
		if ( i_type.Equals("NORMAL") )
			return DoorType.NormalDoor;
		else if ( i_type.Equals("END") )
			return DoorType.EndDoor;
		else if ( i_type.Equals("START") )
			return DoorType.StartDoor;
		else
			return DoorType.OfflineNormalDoor;
	}

	public void SyncDoorType( DoorType i_type )
	{
		//Debug.Log ("** Synching door type: " + i_type );
		if ( _doorType == null )
			_doorType = i_type;

		if( _doorType == DoorType.EndDoor )
		{
			ring1 = GameObject.Find("EndRing1").transform;
			ring2 = GameObject.Find("EndRing2").transform;
			//isEndDoor = true;
		}
		else if( _doorType == DoorType.StartDoor )
		{
			ring1 = GameObject.Find("StartRing1").transform;
			ring2 = GameObject.Find("StartRing2").transform;
		}

	}

	public void ProtectEndDoor()
	{
		_doorState = DoorState.PROTECTED;
		this.gameObject.transform.renderer.material = GraphManager.Manager.DoorUnavailable;
	}

	public override void SetConnected( bool i_connected )
	{
		Connected = (_doorType == DoorType.StartDoor)? true:i_connected;
		if ( i_connected && HackerManager.Manager.CheckHackerClearance( SecurityLevel ) )
		{
			NetworkManager.Manager.SetDoorClearance( Index, true );
			DoorStates.OnConnect( this );
		}
		else if( !i_connected ) 
		{
			DoorStates.OnDisconnect( this );

			if( !HackerManager.Manager.CheckHackerClearance( SecurityLevel ) )
			{	
				NetworkManager.Manager.SetDoorClearance( Index, false );
				_doorState = DoorState.PROTECTED;
				this.gameObject.transform.renderer.material = GraphManager.Manager.DoorUnavailable;
			}
		}

	}


	public override void SetClearance ( )
	{
		if( !HackerManager.Manager.CheckHackerClearance( SecurityLevel ) )
		{
			NetworkManager.Manager.SetDoorClearance( Index, false );
			this.gameObject.transform.renderer.material = GraphManager.Manager.DoorUnavailable;
		}
		else
		{
			if (  SecurityLevel == HackerManager.Manager.HackerClearance )
			{
				NetworkManager.Manager.SetDoorClearance( Index, true );
				if ( Connected )
				{
					this.gameObject.transform.renderer.material = GraphManager.Manager.DoorClosedUnlocked;
					_doorState = DoorState.UNLOCKED;
				}
				else
				{
					this.gameObject.transform.renderer.material = GraphManager.Manager.DoorAvailable;
					_doorState = DoorState.UNPOWERED;
				}
			}
		}
	}


	public override void HandleClickEvent()
	{
		// You cannot interact with the door if it is a start door
		// You also cannot interact with an end door when in lockdown
		if( _doorType == DoorType.StartDoor )
			return;

		if ( !_disabled )
		{
			//Debug.Log ("Interact !!!!!");
			DoorStates.OnInteract( this );
		}
		else
		{
			// Play Disabled Sound
		}
	}


	// This method helps play the animaitons for the start and end door rings
	// and also the disable timers for all nornal doors.
	private void PlayAnimation()
	{
		// If it is a start or end door than play thier animations
		if ( _doorType == DoorType.EndDoor || _doorType == DoorType.StartDoor )
		{
			if( isOpen)
				OpenRingAnimation();
			else
				CloseRingAnimation();
		}
		else if ( _doorType == DoorType.NormalDoor )
		{
			float percent = _disableTimer.PercentComplete();

			if ( percent < 0.9f )
			{
				Quaternion myRotation = DisableTimer.transform.rotation;
				myRotation = Quaternion.Euler(0, percent*360, 0);
				DisableTimer.transform.rotation = myRotation;
			}
			else
			{
				Quaternion myRotation = DisableTimer.transform.rotation;
				myRotation = Quaternion.Euler(0, 0, 0);
				DisableTimer.transform.rotation = myRotation;
				if ( percent > .98f )
					DisableTimer.transform.renderer.material.SetTexture("_MainTex", Shine_05);
				else if ( percent > .96f )
					DisableTimer.transform.renderer.material.SetTexture("_MainTex", Shine_04);
				else if ( percent > .94f )
					DisableTimer.transform.renderer.material.SetTexture("_MainTex", Shine_03);
				else if ( percent > .92f )
					DisableTimer.transform.renderer.material.SetTexture("_MainTex", Shine_02);
				else
					DisableTimer.transform.renderer.material.SetTexture("_MainTex", Shine_01);
			}
		}
	}


	void OpenRingAnimation() 
	{
		if ( GameManager.Manager.PlayerType == 2 )
		{
			float endAngle = _ringTimer.PercentComplete() * 180;
			if(!float.IsNaN(endAngle))
			{
				ring1.localRotation = Quaternion.Euler( new Vector3( 90.0f, currentAngle + endAngle, 0.0f ) );
				ring2.localRotation = Quaternion.Euler( new Vector3( 90.0f, currentAngle + endAngle, 0.0f ) );
			}
		}
	}
	
	void CloseRingAnimation()
	{	
		if ( GameManager.Manager.PlayerType == 2 )
		{
			float endAngle = (_ringTimer.PercentComplete() * 180)-180;
			if(!float.IsNaN( currentAngle + endAngle))
			{
				ring1.localRotation = Quaternion.Euler( new Vector3( 90.0f, currentAngle + endAngle, 0.0f ) );
				ring2.localRotation = Quaternion.Euler( new Vector3( 90.0f, currentAngle + endAngle, 0.0f ) );
			}
		}
	}  

	
	public void OpenDoor()
	{		
		if ( GameManager.Manager.PlayerType == 2 && Connected )
		{
			transform.renderer.material = GraphManager.Manager.DoorOpenUnlocked;
		}
		isOpen = true;
		animating = true;

	}
	
	public void CloseDoor()
	{		
		// [ SOUND TAG ] [Door_Change_State_Hacker]
		if ( GameManager.Manager.PlayerType == 2 && Connected )
		{
			transform.renderer.material = GraphManager.Manager.DoorClosedUnlocked;
		}
		isOpen = false;
		animating = true;
	}
	
	public void SecureLock()
	{
		// [ SOUND TAG ] [Door_Change_State_Hacker] 
		_doorState = DoorState.LOCKED;
		isOpen = false;
		if ( Connected )
			transform.renderer.material = GraphManager.Manager.DoorConnectedLocked;
	}
	
	public void Disconnect()
	{
		this.gameObject.transform.renderer.material = GraphManager.Manager.DoorAvailable;
	}


	public void Unlock()
	{	
		_doorState = DoorState.UNLOCKED;
		// [ SOUND TAG ] [Door_Change_State_Hacker] 
		if (Connected) {	
			if (isOpen)
				this.gameObject.transform.renderer.material = GraphManager.Manager.DoorOpenUnlocked;
			else 
				this.gameObject.transform.renderer.material = GraphManager.Manager.DoorClosedUnlocked;
		} else {
			_doorState = DoorState.UNPOWERED;
		}
	}
	
	
	public void NormalLock()
	{
		isOpen = false;
		// [ SOUND TAG ] [Door_Change_State_Hacker]
		if ( Connected )
			this.gameObject.transform.renderer.material = GraphManager.Manager.DoorClosedUnlocked;
	}


	public void Block()
	{
		_disabled = true;
		DisableIndicator.transform.renderer.enabled = true;
	}

	public void Unblock()
	{
		_disabled = false;
		DisableIndicator.transform.renderer.enabled = false;
	}

	public void SetDisableTimer(float i_time)
	{
		//Debug.Log ("******* BEGIN DISABLE");
		animating = true;
		_disabled = true;
		DisableIndicator.transform.renderer.enabled = true;
		DisableTimer.transform.renderer.material.SetTexture("_MainTex", DisableBaseTex);
		DisableTimer.transform.renderer.enabled = true;
		Action timerEndAction = delegate(){EndDisableTimer();};
		_disableTimer = gameObject.AddComponent<GenericTimer>();
		_disableTimer.Set( i_time, false, timerEndAction );
		_disableTimer.Run();
	}

	public void EndDisableTimer()
	{
		//Debug.Log ("******* END DISABLE");
		animating = false;
		_disabled = false;
		Destroy (_disableTimer);
		DisableIndicator.transform.renderer.enabled = false;
		DisableTimer.transform.renderer.enabled = false;
	}

	public void StartRingTimer()
	{
		//Debug.Log ("******* BEGIN Ring Timer");
		animating = true;

		Action timerEndAction = delegate(){EndRingTimer();};
		_ringTimer = gameObject.AddComponent<GenericTimer>();
		_ringTimer.Set( 1.5f, false, timerEndAction );
		_ringTimer.Run();
	}

	public void EndRingTimer()
	{
		//Debug.Log ("******* END Ring Timer");
		animating = false;
		Destroy (_ringTimer);
	}
}