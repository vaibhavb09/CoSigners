using UnityEngine;
using System.Collections;

public class DoorController : IDoorController 
{
	//Door state
	public bool 			isOffline;
	public bool 			isOpen;
	public bool 			isLocked;
	public bool 			isDeadlocked;

	//Player interaction
	public bool 			playerCloseEnoughToOpen;
	public bool 			animating;
	private bool 			blastDoorAnimating;

	//Guard interaction
	private bool 			guardWaiting;
	private GameObject 		guard;
	public bool 			guardCloseEnoughToOpen;

	//Raycast to check for objects in door
	public Vector3			rayCastStartPoint;
	public float			rayLength;
	private LayerMask 		rayCastMask;
	private bool			isBlocked;

	//Resources
	public Material 		locked;
	public Material 		unlocked;
	public Material			aboveClearance;
	private Animation 		_doorAnimation;
	private Transform		_glowMeshTop;
	//private Transform		_glowMeshLock1;
	//private Transform 		_glowMeshLock2;
	private Transform 		_doorCollision;

	//Animation flags
	private bool 			_openFlag = false;
	private bool 			_closeFlag = false;
	private bool 			_deadLockOpenFlag = false;
	private bool 			_deadLockCloseFlag = false;


	// Use this for initialization
	void Start () 
	{ 
		if( !isOffline )
			_type = DoorType.NormalDoor;
		else
			_type = DoorType.OfflineNormalDoor;

		LoadDefaultState();
		LoadResources();
	}

	#region Load methods
	public override void Load( int i_number, bool i_locked, bool i_open )
	{
		DoorNumber = i_number;
		_doorNode = GraphManager.Manager.GetNode(DoorNumber) as DoorNode;
		if ( _doorNode == null )
		{
			//Debug.LogError("Door " + DoorNumber + " is unable to load");
		}
		else
		{
			//Debug.Log ("Loaded Door at: " + DoorNumber);
		}

		isLocked = i_locked;
		if( !isLocked )
			NetworkManager.Manager.UnlockDoor( DoorNumber );
		if( i_open )
			NetworkManager.Manager.OpenDoor( DoorNumber );
	}


	private void LoadDefaultState()
	{
		playerCloseEnoughToOpen = false;
		guardCloseEnoughToOpen = false;
		guardWaiting  = false;
		animating = false;
		blastDoorAnimating = false;
		isDeadlocked = false;
		isBlocked =	false;

		if( rayLength == 0 )
			rayLength = 2; 
		// Clear the Raycast Mask
		rayCastMask = 0;
		// Add the Ignore raycast layer to the mask (Layer to name doesnt work for some reason !)
		rayCastMask = 1 << 2;
		// Add the ignore guards layer to the mask (Layer to name doesnt work for some reason !)
		rayCastMask = 1 << 10;
		// Ad dthe see through ping layer to the ignore for guards
		rayCastMask = 1 << 11;
		// invert the mask (the mask now collides with everything that is not on the "ignoreGuards" and "Ignore Raycast" layer
		rayCastMask = ~rayCastMask;

		//Offset the raycast start point using the door's position.
		rayCastStartPoint += transform.position;

		if( isOffline ) 
		{
			if( !isLocked )
				UnlockDoor();
			else
				LockDoor();
			
			if( isOpen )
				OpenDoor();
		}

		gameObject.GetComponent<NavMeshObstacle>().enabled = false;
	}

	private void LoadResources()
	{
		_doorAnimation = transform.FindChild("DOR_2M_BB_mesh").GetComponent<Animation>();
		_glowMeshTop = transform.Find("DOR_2M_BB_mesh/GlowMesh_Door_2M");
		//_glowMeshLock1 = transform.Find("DOR_2M_BB_mesh/Door01/LockBase_01/Lock_Top01/GlowMesh02");
		//_glowMeshLock2 = transform.Find("DOR_2M_BB_mesh/Door02/LockBase_03/Lock_Top01 1/GlowMesh01");
		_doorCollision = transform.FindChild("movingCollisionBox");
	}
	#endregion

	#region Door public interface
	// <public interface>
	public override DoorType GetDoorType ()
	{
		return _type;
	}

	public override int GetDoorIndex()
	{
		return DoorNumber;
	}

	public override void SetClearance( bool i_clear )
	{
		if( !i_clear )
			UpdateDoorLightIndicator(aboveClearance);
		else
		{
			if( isLocked )
				UpdateDoorLightIndicator( locked );
			else
				UpdateDoorLightIndicator( unlocked );
		}
	}

	public override void OpenDoor()
	{
		isOpen = true;
		animating = true;
		_openFlag = false;
		_doorAnimation.PlayQueued("Door_Open");
		gameObject.layer = 11;
		_doorCollision.collider.enabled = false;
		// Door was open [SOUND TAG] [door_open]
		soundMan.soundMgr.playOneShotOnSource(this.audio,"door_open",GameManager.Manager.PlayerType);
		

	}
	
	public override void CloseDoor()
	{
		isOpen = false;
		animating = true;
		_doorAnimation.PlayQueued("Door_Close");
		gameObject.layer = 0;
		_doorCollision.collider.enabled = true;
		// Door was Closed [SOUND TAG] [door_close]
		soundMan.soundMgr.playOneShotOnSource(this.audio,"door_close",GameManager.Manager.PlayerType);
	}
	
	public override void LockDoor()
	{
		UpdateDoorLightIndicator( locked );
		//Debug.Log("Lock door "+ DoorNumber);
		isLocked = true;
		
		// Door got Locked [SOUND TAG] [Door_Change_State] 
		// soundMan.soundMgr.playOneShotOnSource(this.audio,"Door_Change_State",GameManager.Manager.PlayerType);
	}

	public override void UnlockDoor()
	{
		UpdateDoorLightIndicator( unlocked );
		isLocked = false;
		if(isDeadlocked)
			NetworkManager.Manager.UnSecureLockDoor( DoorNumber );
		
		// Door got Locked [SOUND TAG] [Door_Change_State]
		soundMan.soundMgr.playOneShotOnSource(this.audio,"Door_Change_State",GameManager.Manager.PlayerType);
	}
	
	public override void DeadlockDoor()
	{
		UpdateDoorLightIndicator(locked);
		isDeadlocked = true;
		blastDoorAnimating = true;
		_doorAnimation.PlayQueued("Secure_Lock_Close");
		_doorCollision.collider.enabled = true;
		gameObject.GetComponent<NavMeshObstacle>().enabled = true;
			
		// Door got deadlocked [SOUND TAG] [Blast_door_Lock_Unlock]
		soundMan.soundMgr.playOneShotOnSource(this.audio,"Blast_door_Lock_Unlock",GameManager.Manager.PlayerType);
	}
	
	public override void UnDeadlockDoor()
	{
		if( !isLocked )
			UpdateDoorLightIndicator(unlocked);

		isDeadlocked = false;
		blastDoorAnimating = true;
		_doorAnimation.PlayQueued("Secure_Lock_Open");
		//if( !isLocked )
		//_doorCollision.collider.enabled = false;
		gameObject.GetComponent<NavMeshObstacle>().enabled = false;
		
		// Door got opened [SOUND TAG] [Blast_door_Lock_Unlock]
		soundMan.soundMgr.playOneShotOnSource(this.audio,"Blast_door_Lock_Unlock",GameManager.Manager.PlayerType);
	}

	public override bool CanDeadlockDoor()
	{
		if( _doorAnimation.isPlaying /*|| guardCloseEnoughToOpen*/)
		{
			return false;
		}
		return true;
	}

	public override bool CanUndeadlockDoor()
	{
		if(_doorAnimation.isPlaying)
		{
			return false;
		}
		return true;
	}

	// Main entry point for interacting with doors as the Thief.
	public override void InteractWithDoor()
	{
		//Debug.Log ("THIS DOOR IS: " + _doorNode._doorState + " - Type: " + _doorNode._doorType + " - IsOpen: " + _doorNode.isOpen);
		if( !_doorNode.animating && _doorNode._doorState == DoorState.UNLOCKED )
		//if( !animating && !isLocked && !isDeadlocked  ) 
		{
			if( !_doorNode.isOpen )
			{
				if( _doorNode.Connected )
					NetworkManager.Manager.OpenDoor( DoorNumber );
				else
					OpenDoor ();
				
				// [Sound tag] [Door_Change_State]
				soundMan.soundMgr.playOneShotOnSource(this.audio,"Door_Change_State",GameManager.Manager.PlayerType);
			}
			else
			{
				if( _doorNode.Connected && !guardCloseEnoughToOpen && guard == null )
					NetworkManager.Manager.CloseDoor( DoorNumber );
				else if( !guardCloseEnoughToOpen && guard == null )
					CloseDoor();
				
				if( guard != null )
				{
					guard.GetComponent<NavMeshAgent>().Stop();
				}
				
				// [Sound tag] [Door_Change_State]
				soundMan.soundMgr.playOneShotOnSource(this.audio,"Door_Change_State",GameManager.Manager.PlayerType);
			}
		}
		
	}

	//</public interface >
	#endregion
	
	void UpdateDoorLightIndicator( Material mat )
	{
		_glowMeshTop.renderer.material = mat;
		//_glowMeshLock1.renderer.material = mat;
		//_glowMeshLock2.renderer.material = mat;
	}

	bool IsActorBlockingDoor()
	{
		RaycastHit hitInfo;
		//Do a rayvast downwards from raycaststartpoint to check is something is in the doorway.
		if( Physics.Raycast(rayCastStartPoint, -1 * Vector3.up,out hitInfo, rayLength, rayCastMask) )
		{
			if( hitInfo.collider.gameObject.CompareTag("Guard") || hitInfo.collider.gameObject.CompareTag("Player") )
				return true;
		}  
		return false;
	}

	void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.tag == "Player")
		{
			playerCloseEnoughToOpen = true;
		}
		if( GameManager.Manager.PlayerType == 1 )
		{
			
			if(other.gameObject.tag == "Guard")
			{
				if( !isOpen )
				{
					//NetworkManager.Manager.SetGuardProximity(DoorNumber, true);
					guardCloseEnoughToOpen = true;
					guard = other.gameObject;
					if( !isDeadlocked )
					{
						other.gameObject.GetComponent<NavMeshAgent>().Stop();
					}
					else
					{
						Vector3 ogDestination = guard.GetComponent<NavMeshAgent>().destination;
						//Vector3 ogDestination = new Vector3( -29.5f, 0.1f, 22.0f  );
						guard.GetComponent<NavMeshAgent>().ResetPath();
						NavMeshPath path = new NavMeshPath();
						guard.GetComponent<NavMeshAgent>().CalculatePath( ogDestination, path );
						if( path.status == NavMeshPathStatus.PathInvalid )
						{
							//Debug.Log("Path is invalid");
						}
						guard.GetComponent<NavMeshAgent>().SetDestination( ogDestination );
					}
				}
			}
		}
	}
	
	void OnTriggerStay( Collider other )
	{
		//Something is blocking the door.
		if(	!isBlocked && IsActorBlockingDoor() )
		{
			NetworkManager.Manager.BlockDoor( DoorNumber );
			isBlocked = true;
		}
		if( isBlocked && !IsActorBlockingDoor() )
		{
			NetworkManager.Manager.UnBlockDoor( DoorNumber );
			isBlocked = false;
		}

		if( GameManager.Manager.PlayerType == 1 )
		{
			if(other.gameObject.tag == "Guard")
			{
				guard = other.gameObject;
				if(guard != null && !isOpen )
				{
					if( !isDeadlocked )
					guard.GetComponent<NavMeshAgent>().Stop();
					else
					{
						Vector3 ogDestination = guard.GetComponent<NavMeshAgent>().destination;
						//Vector3 ogDestination = new Vector3( -29.5f, 0.1f, 22.0f );
						guard.GetComponent<NavMeshAgent>().ResetPath();
						NavMeshPath path = new NavMeshPath();
						guard.GetComponent<NavMeshAgent>().CalculatePath( ogDestination, path );
						if( path.status == NavMeshPathStatus.PathInvalid )
						{
							//Debug.Log("Path is invalid");
						}
						guard.GetComponent<NavMeshAgent>().SetDestination( ogDestination );				
					}
				}
			}
		}
	}
	
	void OnTriggerExit(Collider other)
	{
		if(other.gameObject.tag == "Player")
		{
			playerCloseEnoughToOpen = false;
		}
		if( GameManager.Manager.PlayerType == 1 )
		{
			if(other.gameObject.tag == "Guard")
			{
				//NetworkManager.Manager.SetGuardProximity(DoorNumber, false);
				guardCloseEnoughToOpen = false;
				guardWaiting = false;
				if(guard !=  null)
				guard.GetComponent<NavMeshAgent>().Resume();
				if( !isDeadlocked ) //This ensures that the door does not reclose, and change from the deadlocked to the unlocked-closed state
					NetworkManager.Manager.CloseDoor( DoorNumber );
				guard = null;
			}
		}
	}
	
	void OpenAnimation()
	{		
		if(animating == true && _doorAnimation.IsPlaying("Door_Open"))
		{
			_openFlag = true;
		}
		
		if(_openFlag && !_doorAnimation.IsPlaying("Door_Open"))
		{
			animating = false;
			isOpen = true;
			_doorCollision.collider.enabled = false;
			if( guard != null )
			{
				guard.GetComponent<NavMeshAgent>().Resume();
			}
		}
	}
	
	void CloseAnimation()
	{	
		if(animating == true && _doorAnimation.IsPlaying("Door_Close"))
		{
			_closeFlag = true;
		}
		
		if(_closeFlag && !_doorAnimation.IsPlaying("Door_Close"))
		{
			animating = false;
			isOpen = false;
			_doorCollision.collider.enabled = true;
			if( guard != null )
			{
				guard.GetComponent<NavMeshAgent>().Stop();
			}
		}
	}  
	
	void DeadlockAnimation()
	{
		if(blastDoorAnimating == true && _doorAnimation.IsPlaying("Secure_Lock_Close"))
		{
			_deadLockCloseFlag = true;
		}
		
		if(_deadLockCloseFlag && !_doorAnimation.IsPlaying("Secure_Lock_Close"))
		{
			blastDoorAnimating = false;
			isDeadlocked = true;
		}
	}
	
	void UnDeadlockAnimation()
	{		
		if(blastDoorAnimating == true && _doorAnimation.IsPlaying("Secure_Lock_Open"))
		{
			_deadLockOpenFlag = true;
		}
		
		if(_deadLockOpenFlag && !_doorAnimation.IsPlaying("Secure_Lock_Open"))
		{
			blastDoorAnimating = false;
			isDeadlocked = false;
		}
	}
	
	// Update is called once per frame
	void Update () 
	{	
		if( GameManager.Manager.PlayerType == 1 )
		{
			//Guard AI stuff
			if( guardCloseEnoughToOpen && !isDeadlocked && !isOpen && !animating && !_doorNode.animating )
			{
				NetworkManager.Manager.OpenDoor( DoorNumber );
			}
		}
		
		if(animating) //Door panel animations
		{
			if( isOpen)
				OpenAnimation();
			else
				CloseAnimation();
			
		}
		
		if(blastDoorAnimating)
		{
			if( isDeadlocked )
				DeadlockAnimation();
			else
				UnDeadlockAnimation();
		}	
	}
}