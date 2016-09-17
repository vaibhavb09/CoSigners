using UnityEngine;
using System.Collections;

public class EndDoorController : IDoorController 
{
	//Door state
	public bool 			isLocked;
	public bool 			isOpen;

	//Open/Close Animation 
	public float 			rotationSpeed;
	public float 			startAngle;
	public float 			endAngle;
	private bool 			animating;
	private float 			currentAngle;

	//Lock animation
	public float			scaleSpeed;
	public float			translateSpeed;
	public float 			startScale; //Scale is for animating the floor glow plane.
	public float 			endScale;
	public float 			startY; // Y is for animating the vertical glow planes.
	public float 			endY;
	private bool			lockAnimating;
	private float			currentScale;
	private float			currentY;
	
	//Resources
	public Material			lockedMat;
	public Material			unlockedMat;
	public Material			innerCircleDefaultMat;
	public Transform		innerCircle;
	private Transform		verticalGlowPlanes;
	private Transform		floorGlowPlane;
	private Transform		endGameTrigger;

	// Use this for initialization
	void Start () 
	{
		//_type = DoorType.EndDoor;
	}

	#region Public interface

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
		isOpen = i_open;
		if( !isLocked )
			NetworkManager.Manager.UnlockDoor( DoorNumber );
		if( isOpen )
			NetworkManager.Manager.OpenDoor( DoorNumber );

		NetworkManager.Manager.SyncDoorType( DoorNumber, (int)_type ); //This syncs the door type on the hacker side.

		animating = false;
		currentAngle = endAngle;

		endGameTrigger = transform.FindChild("EndLevelTriggerBox");
		verticalGlowPlanes = transform.FindChild("VerticalGlowPlanes");
		floorGlowPlane = transform.FindChild("EndDoor_FloorGlowPlane");

		if(_type == DoorType.StartDoor)
		{
			endGameTrigger.collider.enabled = false;
			//Start door should have glow planes up
			currentY = 0.86f;
			currentScale = 0.01f;
			NetworkManager.Manager.UnlockDoor( DoorNumber );
			NetworkManager.Manager.OpenDoor( DoorNumber );
		}
		else //is EndDoor
		{
			endGameTrigger.collider.enabled = true;
			currentY = -3.58f;
			currentScale = 0.0f;
		}
	}
	
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
	}

	public override void OpenDoor()
	{
		isOpen = true;
		animating = true;
		animation.CrossFade("Enddoor_Open");
	}
	
	public override void CloseDoor()
	{
		isOpen = false;
		animating = true;
		animation.CrossFade("Enddoor_Close");
	}
	
	public override void LockDoor()
	{
		isLocked = true;
		lockAnimating = true;
		Material[] mats = new Material[]{lockedMat, innerCircleDefaultMat};
		innerCircle.renderer.materials = mats;
	}
	
	public override void UnlockDoor()
	{
		isLocked = false;
		lockAnimating = true;
		Material[] mats = new Material[]{unlockedMat, innerCircleDefaultMat};
		innerCircle.renderer.materials = mats;
	}
	
	public override void DeadlockDoor()
	{
		LockDoor();
	}
	
	public override void UnDeadlockDoor()
	{
		UnlockDoor();
	}
	
	public override void InteractWithDoor()
	{
		Debug.Log ( "End DOOR STATE IS: " + _doorNode._doorState + " -- " + _doorNode.animating + "And End Door number is: " + _doorNode.Index);
		//if( !animating && !isLocked )
		if( !_doorNode.animating && _doorNode._doorState == DoorState.UNLOCKED )
		{
			if( !isOpen )
			{
				NetworkManager.Manager.OpenDoor( DoorNumber );

			}
			else
			{
				NetworkManager.Manager.CloseDoor( DoorNumber );
			}
		}
		
	}

	public override bool CanDeadlockDoor()
	{
		return true;
	}

	public override bool CanUndeadlockDoor()
	{
		return true;
	}
	#endregion

	#region Door Animation

	void OpenAnimation()
	{
		if( !animation.IsPlaying("Enddoor_Open") )
		{
			animating = false;
			isOpen = true;
		}
	}
	
	void CloseAnimation()
	{	
		if( !animation.IsPlaying("Enddoor_Close") )
		{
			animating = false;
			isOpen = false;
		}
	}  

	void LockAnimation()
	{

		if( currentY >= startY ) // Translate vertical glow planes downwards.
		{
			currentY -=  translateSpeed;
			verticalGlowPlanes.position = new Vector3( verticalGlowPlanes.position.x, currentY, verticalGlowPlanes.position.z );
		}
		else
		{
			currentY = startY;
			verticalGlowPlanes.position = new Vector3( verticalGlowPlanes.position.x, currentY, verticalGlowPlanes.position.z );

			if( currentScale >= startScale ) //Scale down floor glow plane
			{
				currentScale -= scaleSpeed;
				floorGlowPlane.localScale = new Vector3( currentScale, floorGlowPlane.localScale.y, floorGlowPlane.localScale.z);
			}
			else
			{
				lockAnimating = false;
				currentScale = startScale;
				floorGlowPlane.localScale = new Vector3( currentScale, floorGlowPlane.localScale.y, floorGlowPlane.localScale.z);
			}
		}


	}

	void UnlockAnimation()
	{
		if( currentScale <= endScale ) //Scale up floor glow plane
		{
			currentScale += scaleSpeed;
			floorGlowPlane.localScale = new Vector3( currentScale, floorGlowPlane.localScale.y, floorGlowPlane.localScale.z);
		}
		else
		{
			currentScale = endScale;
			floorGlowPlane.localScale = new Vector3( currentScale, floorGlowPlane.localScale.y, floorGlowPlane.localScale.z);
			
			if( currentY <= endY ) // Translate vertical glow planes upwards.
			{
				currentY +=  translateSpeed;
				verticalGlowPlanes.position = new Vector3( verticalGlowPlanes.position.x, currentY, verticalGlowPlanes.position.z );
			}
			else
			{
				lockAnimating = false;
				currentY = endY;
				verticalGlowPlanes.position = new Vector3( verticalGlowPlanes.position.x, currentY, verticalGlowPlanes.position.z );
			}
		}
	}

	#endregion
	// Update is called once per frame
	void Update () 
	{
		if(animating) 
		{
			if( isOpen)
				OpenAnimation();
			else
				CloseAnimation();
		}
		if( lockAnimating )
		{
			if( isLocked )
				LockAnimation();
			else
				UnlockAnimation();
		}
	}


}
