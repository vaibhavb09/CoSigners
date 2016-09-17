#if UNITY_IPHONE
//Do iphone stuff
using UnityEngine;
using System.Collections;

public class ThiefActions : MonoBehaviour
{
	private void Start(){}
	private void Update(){}
	public void playerCaught(){}
	public void DisableInput(){}
	public void EnableInput(){}
	public void PauseGame(){}
	public void unPauseGame(){}
}

#else

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Net.Mail;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

public class ThiefActions : MonoBehaviour {
	
	public float force;
	public Vector3 offset;
	public float EMP_Influence_Radius;
	public float PingDistance = 20.0f;
	public float pushPower = 0.0f;
	public Transform ProbablePingObject;
	public Transform PingObject;
	
	private float _transmitterPlacementDistance;
	private float _scramblerPlacementDistance;
	private float _scramblerActiveTime;
	private float _pingDistance;
	private float _peekSnapDistanceCheck;
	private bool _placingTransmitter;
	private bool _resettingTransmitter;
	private bool _placingScrambler;
	private bool _cancelTransPlacement;
	public bool _peeking;
	public bool _peekingRight;
	public bool _peekSnapping;
	public float _peekSnapRotation;
	GameObject throwable;
	GameObject _camera;
	private Transform _tempScrambler;
	private Transform _tempTransmitter;
	private Vector3 _startMousePosition;
	private int _transmitterToReset;
	Transform _currentTransmitter;
	
	private bool Player_Caught = false;
	private GameObject Player;
	float 	caughtTime;
	public 	int caughtTimeDisplayDuration = 3;
	LayerMask maskForPing;
	private float cursorRayTimer = 0.0f;
	private float cursorRayInterval = 0.1f;
	
	Vector3 lastObjPingPosition = Vector3.zero;
	
	// Mask for ignoring raycast for peeking on open doors
	private LayerMask mask;
	private bool enableKeyInputs;
	
	//For Cues
	float showTime = 0.0f;
	bool isShowingCue = false;

	public void playerCaught()
	{
		Player_Caught = true;
		caughtTime = Time.time;
	}
	
	public bool IsPeeking
	{
		get{
			return _peeking;
		}
	}
	
	public bool IsCaught
	{
		get{
			return Player_Caught;
		}
		
		
	}
	public bool PeekingRight
	{
		get{
			return _peekingRight;
		}
	}
	
	// Use this for initialization
	void Start ()
	{
		enableKeyInputs = true;
		Player = GameObject.Find("Playertheif(Clone)");
		_placingTransmitter = false;
		_cancelTransPlacement = false;
		_placingScrambler=false;
		_transmitterPlacementDistance = 3.0f;
		_scramblerPlacementDistance=3.0f;
		_transmitterPlacementDistance = 3.0f;
		_pingDistance = 7.0f;
		_peekSnapDistanceCheck = 1.5f;
		_peeking = false;
		_peekingRight = false;
		_peekSnapping = false;
		
		_camera = (GameObject)GameObject.Find ("FPSCamera");

		if(GameManager.Manager.PlayerType==1)
		{
			GameObject startMarker=GameObject.Find("StartMarker");
			if(startMarker!=null)
				DestroyObject(startMarker);
			
			GameObject endMarker=GameObject.Find("EndMarker");
			if(startMarker!=null)
				DestroyObject(endMarker);
		}

		/*** FOR PINGS ***/
		maskForPing = 1 << 2;
		maskForPing |= 1 << 11;
		maskForPing = ~maskForPing;
		/*****************/
		
		mask =0;
		
		// Add the Ignore raycast layer to the mask (Layer to name doesnt work for some reason !)
		mask = 1 << 2;
		
		// Add the ignore guards layer to the mask (Layer to name doesnt work for some reason !)
		mask = 1 << 10;
		
		// Ad dthe see through ping layer to the ignore for guards
		mask = 1 << 11;
		
		// invert the mask (the mask now collides with everything that is not on the "ignoreGuards" and "Ignore Raycast" layer
		mask = ~mask;
	}
	
	void EmailTest()
	{   
		MailMessage message = new MailMessage();
		message.To.Add("cyberheistgame@gmail.com, sagarmistry25@gmail.com");
		message.Subject = "OMG this works!!!";
		message.Body = "This email was sent from the game bitches.-Sagar Mistry.";
		message.From = new MailAddress("cyberheistest@yahoo.com");
		SmtpClient smtp = new SmtpClient("smtp.mail.yahoo.com", 587);
		smtp.EnableSsl = true;
		smtp.UseDefaultCredentials = false;
		smtp.Credentials = new NetworkCredential("cyberheistest@yahoo.com", "Hacknhide1");
		ServicePointManager.ServerCertificateValidationCallback = 
                delegate(object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) 
                    { return true; };
		smtp.Send(message);
		Debug.Log("Email sent");
	}



	void throwObject( float i_force, Vector3 i_dir )
	{
		throwable.transform.position = transform.position + transform.forward + offset;
		throwable.transform.rotation = transform.rotation;
		throwable.rigidbody.AddForce( i_dir * i_force );
	}
	
	// Update is called once per frame
	void Update () 
	{
		if ( (GameManager.Manager.PlayerType == 1) && (enableKeyInputs == true)) // only Thief can do this
		{
			if ( _placingTransmitter )
			{
				ProcessTransmitterPlacement();
			}
			else if ( _peeking )
			{
				ProcessPeeking();
			}
			else if ( _placingScrambler )
			{
				ProcessScramblerPlacement();
			}
			else
			{
				ProcessMouseInputs();
				ProcessKeyInputs();
			}
			
			if(cursorRayTimer >= cursorRayInterval)
			{
				ProcessCursorChanges();
				cursorRayTimer = 0.0f;
			}
			else
			{
				cursorRayTimer += Time.deltaTime;
			}

			if(Input.GetKeyDown(KeyCode.K) )
			{
				/*** Kill Tracers ***/
				NetworkManager.Manager.KillTracers(); 

				/*** Expose Hex Grid To Hacker ***/
				NetworkManager.Manager.ExposeHexGrid();

				/*** Kill Guards ***/
				GameObject.Find("LevelPreset").transform.FindChild("GuardOverlord").GetComponent<GuardOverlord>().enabled = false;

				GameObject[] guards = GameObject.FindGameObjectsWithTag("Guard");
				
				foreach(GameObject guard in guards)
				{
					GameObject newGuard = Resources.Load("Prefabs/Theif/Guard_Inactive") as GameObject;
					newGuard = (GameObject)Instantiate(newGuard, guard.transform.position, guard.transform.rotation) as GameObject;
				}

				foreach(GameObject guard in guards)
				{
					Destroy(guard);
				}
				/********************/
			}
		}
		
	}
	
	private void OnTriggerEnter(Collider other)
	{
		if( other.gameObject.tag == "MailBox" )
		{
			//GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>().EMPPulses++;
		}
	}
	
	private void OnControllerColliderHit( ControllerColliderHit hit )
	{
		//CHECKING IF GUARD CAUGHT THE PLAYER
		if( hit.gameObject.CompareTag("Guard") )
		{
			Player_Caught = true;
			caughtTime = Time.time;
		}

		/*
		//Pushing other rigidbodies
		Rigidbody body = hit.collider.attachedRigidbody;
		if (body == null || body.isKinematic || !body.gameObject.tag.Equals( "Pushable" ) )
			return;
		
		if (hit.moveDirection.y < -0.3F)
			return;
		
		Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);
		body.velocity = pushDir * pushPower;
		*/
	}
	
	private void ProcessCursorChanges()
	{
		// Cast a ray in font of the player
		RaycastHit rayHit;
		Vector3 fwd = transform.TransformDirection(Vector3.forward);
 		if (Physics.Raycast(_camera.transform.position, fwd, out rayHit, 12.0f))
		{
			// If it hits somethign perform action based on what was hit.
			if ( rayHit.transform.gameObject.name.Equals("FloorPlane" ) && rayHit.distance < _transmitterPlacementDistance )
			{
				Player.GetComponent<ThiefGUI>().ChangeCursorToTransmitter();
			}
			else if ( rayHit.transform.gameObject.name.Equals("Transmitter_Prefab(Clone)") )
			{
				if ( rayHit.distance < _transmitterPlacementDistance )
				{
					Player.GetComponent<ThiefGUI>().ChangeCursorToActivate();
				}
				else
				{
					Player.GetComponent<ThiefGUI>().ChangeCursorToCrossHair();
				}
			}
			else if ( rayHit.transform.gameObject.name.Equals("Jammer_Prefab(Clone)") )
			{
				if ( rayHit.distance < 3.0f )
				{
					Player.GetComponent<ThiefGUI>().ChangeCursorToActivate();
				}
				else
				{
					Player.GetComponent<ThiefGUI>().ChangeCursorToCrossHair();
				}
			}
			else if ( rayHit.transform.gameObject.tag.Equals("Door")  )
			{	
				if ( rayHit.distance < 3.0f )
				{
					Player.GetComponent<ThiefGUI>().ChangeCursorToDoor();
				}
				else
				{
					Player.GetComponent<ThiefGUI>().ChangeCursorToCrossHair();
				}
			}
			else if( rayHit.transform.gameObject.tag.Equals("DoorRaycastEmpty") ) 
			{
				if ( rayHit.distance < 3.0f )
				{
					Player.GetComponent<ThiefGUI>().ChangeCursorToDoor();
				}
				else
				{
					Player.GetComponent<ThiefGUI>().ChangeCursorToCrossHair();
				}
			}
			else if( rayHit.transform.gameObject.name.Equals("SAP_Screen") ) 
			{
				if ( rayHit.distance < 3.0f )
				{
					Player.GetComponent<ThiefGUI>().ChangeCursorToSAP();
				}
				else
				{
					Player.GetComponent<ThiefGUI>().ChangeCursorToCrossHair();
				}
			}
			else if( rayHit.transform.gameObject.name.Equals("OverridePlatform(Clone)") ) //override platform
			{
				if ( rayHit.distance < 3.0f )
				{
					Player.GetComponent<ThiefGUI>().ChangeCursorToOverride();
				}
				else
				{
					Player.GetComponent<ThiefGUI>().ChangeCursorToCrossHair();
				}
			}
			else if( rayHit.transform.gameObject.name.Equals("IT_Stand_Parent(Clone)") ) //InfoNode platform. Assumed name
			{
				if( rayHit.distance < 3.0f )
				{
					Player.GetComponent<ThiefGUI>().ChangeCursorToInfoNode();
				}
				else
				{
					Player.GetComponent<ThiefGUI>().ChangeCursorToCrossHair();
				}
			}
			else
			{
				Player.GetComponent<ThiefGUI>().ChangeCursorToCrossHair();
			}
		}
		else
		{
			Player.GetComponent<ThiefGUI>().ChangeCursorToCrossHair();
		}
	}
	
	private void OnGUI()
	{
		if(Player != null)
		{
			if( Player_Caught && ( Time.time - caughtTime ) <= caughtTimeDisplayDuration && GameManager.Manager.PlayerType == 1
				&& BasicScoreSystem.Manager.ThiefCaughtByGuard == false)
			{
				//OnGUI is an update loop, the following functions are called a hundred times after the player is caught!
				//It might be better if we put it somewhere else where it is called only once. - Max
				
				// [NEED SOUND]
				// AudioManager.Manager.ThiefCaughtByGuard( true );Input.GetMouseButtonDown( 0 )
				
				BasicScoreSystem.Manager.ThiefCaughtByGuard = true;
				
				NetworkManager.Manager.ShowLevelEndScreen(1);		
				NetworkManager.Manager.PauseGame(true);
				//GUI.Label( new Rect( Screen.width /2, Screen.height/2 - 200, 400, 400 ), "BUSTED!!!" );	
				//GUI.Label( new Rect( Screen.width /2, Screen.height/2, Screen.width/2, Screen.height/2 ), "Press F2 to restart the Game" );
			}
			else
			{
				if(Player_Caught)
				Player_Caught = false;
			}
		}
	}
	
	private void ProcessMouseInputs()
	{
		// ACTION BUTTON
		// The action buton can perform different functions depending on what the thief is looking at.
		if( Input.GetMouseButtonDown( 0 ) || Input.GetKey( "joystick button 3" ) )
		{


			LayerMask maskIgnoreRaycast = 1 << 2;
			maskIgnoreRaycast = ~maskIgnoreRaycast;
			// Cast a ray in font of the player
			RaycastHit rayHit;
			Vector3 fwd = transform.TransformDirection(Vector3.forward);
 			if (Physics.Raycast(_camera.transform.position, fwd, out rayHit, 12.0f, maskIgnoreRaycast))
			{
				//Debug.Log("Rayhit on " + rayHit.transform.gameObject.name);
				// If it hits somethign perform action based on what was hit.
				if ( rayHit.transform.gameObject.name.Equals("FloorPlane" ) && rayHit.distance < _transmitterPlacementDistance )
				{
					if( ThiefManager.Manager.IsTransmitterCountZero() )
					{
						//Out of transmitters Inactive
						//If we decide to add a cue later
						return;
					}
					int thisHex = HexGrid.Manager.GetHex( rayHit.point );
					if ( HexGrid.Manager.GetTransmitter( thisHex ) == null )
					{
						// Begin placement of a new transmitter
						Quaternion rotation = Quaternion.Euler(fwd.x, fwd.y, fwd.z);
						_tempTransmitter = (Transform) Instantiate(ThiefGrid.Manager.ghostTransmitter, rayHit.point, rotation);
						_placingTransmitter = true;
					}
					else
					{
						//Debug.Log ("------------------------\nA Transmitter Already Exists Here!\n-----------------------------");
					}
				}
				else if ( rayHit.transform.gameObject.name.Equals("Transmitter_Prefab(Clone)") )
				{
					
					_transmitterToReset = HexGrid.Manager.GetHex( rayHit.transform.position );
					// First make sure transmitter is not in middle of a reset.  If so, dont do anything
					if ( !ThiefGrid.Manager.IsInactive( _transmitterToReset ) && !rayHit.transform.GetComponent<TransmitterController>().initialTransmitter )
					{
						//Debug.Log ("Pick up");
						
						/*
						if ( _resettingTransmitter == false)
						{
							//AudioManager.Manager.ThiefRestartsTransmitter( true );
							BasicScoreSystem.Manager.TransmittersRestarted += 1;
							_resettingTransmitter = true;
							_startMousePosition = Input.mousePosition;
							_currentTransmitter = rayHit.transform;
							
							Action timerEndAction = delegate(){ResetTransmitter( _transmitterToReset );};
							GenericTimer myGenericTimer = gameObject.AddComponent<GenericTimer>();
							myGenericTimer.Set( 0.3f, false, timerEndAction );
							myGenericTimer.Run ();
						}
						else
						*/
						if( rayHit.distance < _transmitterPlacementDistance )
						{
							// [NEED SOUND]
							// AudioManager.Manager.ThiefPicksUpTransmitter( true );
							BasicScoreSystem.Manager.TransmittersPickedUp += 1;
							_currentTransmitter = rayHit.transform;
							PickUpTransmitter( _transmitterToReset );
							ThiefManager.Manager.IncrementTransmitterCount();
							_resettingTransmitter = false;
						}
					}
				}
				else if ( rayHit.transform.gameObject.name.Equals("Jammer_Prefab(Clone)") )
				{
					// Debug.Log ("YOU CLICKED ON THE JAMMER");
					// AudioManager.Manager.ThiefDisablesJammer( true );
					if ( rayHit.distance < 3.0f )
					{
						BasicScoreSystem.Manager.JammersDisabled += 1;
						int thisHex = HexGrid.Manager.GetHex( rayHit.point );
						NetworkManager.Manager.DisableJammer( thisHex );
						
						rayHit.transform.GetComponent<JammerController>().DisableJammer();
					}
				}
				else if ( rayHit.transform.gameObject.tag.Equals("Door")  )
				{
					if ( rayHit.distance < 3.0f )
					{
						IDoorController thisDoor = rayHit.transform.gameObject.GetComponent<IDoorController>();
						thisDoor.InteractWithDoor();
					}
				}
				else if( rayHit.transform.gameObject.name.Equals("DoorRaycastEmpty") ) 
				{
					if ( rayHit.distance < 3.0f )
					{
						GameObject emptyBox = rayHit.transform.gameObject;
						IDoorController thisDoor = emptyBox.transform.parent.GetComponent<IDoorController>();
						thisDoor.InteractWithDoor();
					}
				}
				else if( rayHit.transform.gameObject.name.Equals("SAP_Screen") ) 
				{
					if ( rayHit.distance < 3.0f )
					{
						GameObject panel = rayHit.transform.gameObject;
						panel.GetComponent<SecurityAccessPanel>().ActivatePanel();
					}
				}
				else if( rayHit.transform.gameObject.name.Equals("OverridePlatform(Clone)") ) //override platform
				{
					//Debug.Log("Hit Platform");
					if ( rayHit.distance < 3.0f )
					{
						OverridePlatform platform = rayHit.transform.gameObject.GetComponentInChildren<OverridePlatform>();
						//Debug.Log("Hit it?");
						if(platform.Activated == true)
						{
							platform.StartOverride();
						}
						else
						{
							//Override Inactive
							//If we decide to add a cue later
						}
					}
				}
				else if( rayHit.transform.gameObject.name.Equals("IT_Stand_Parent(Clone)") && !OverrideManager.Manager.IsInOverride() ) //Info Node platform. Assumed name
				{
					//Debug.Log("Info Node Platform hit");
					 
					if( rayHit.distance < 3.0f )
					{
						InfoNodePlatform infoPlatform = rayHit.transform.gameObject.GetComponent<InfoNodePlatform>();

						//Debug.Log("infoPlatform.Activated: " + infoPlatform.Activated);

						if( infoPlatform.Activated == true )
						{
							infoPlatform.StartInfoNodeAnimation();
						}
						else
						{
							//Info Node Inactive
							//If we decide to add a cue later
						}
					}
				}

			}
		}
		
		if ( Input.GetMouseButton( 1 ) || Input.GetKeyDown( "joystick button 2" ) ) // On Right Mouse Pressed //DETECTING PROBABLE PINGS
		{
			int hexId;
			
			RaycastHit rayHit;
			
			//Vector3 fwd = transform.TransformDirection(Vector3.forward);
			Vector3 fwd = _camera.transform.forward;
			
			if(Physics.Raycast(_camera.transform.position, fwd, out rayHit, PingDistance, maskForPing)) // Casts a ray
			{
				//Debug.Log("Rayhit on " + rayHit.transform.gameObject.name);
				
				if(lastObjPingPosition != rayHit.transform.position ) // If mouse is moved to other object then delete the previously drawn object
				{
					DestroyProbablePings();
				}
				
				if ( rayHit.transform.gameObject.name.Equals("FloorPlane" ) ) // If Floor
				{
					hexId = HexGrid.Manager.GetHex( rayHit.point );
					
					if( hexId > 0 )
					{
						ThiefGrid.Manager.ShowProbablePing( hexId );
					}
				}
				else if( !(rayHit.transform.gameObject.CompareTag("Guard")) && !(rayHit.transform.gameObject.CompareTag("Wall")) 
					&& !(rayHit.transform.gameObject.name.Equals("Roof")) && !(rayHit.transform.gameObject.CompareTag("Player")) 
				    && !(rayHit.transform.gameObject.CompareTag("SAP")) )// If not the Floor
				{					
					bool modifiedRotation = false;

					Quaternion rotation = rayHit.transform.rotation;
					
					if( /*rotation % 90 != 0 &&*/ lastObjPingPosition != rayHit.transform.position )
					{	
						rayHit.transform.rotation = new Quaternion( 0.0f, 0.0f, 0.0f, 1.0f );	
						modifiedRotation = true;
					}
					
					if( GameObject.Find("ProbableFloorPing") != null )
					{	
						Destroy( GameObject.Find("ProbableFloorPing") );
					}
					CreateObjectPing( rayHit.transform.gameObject, false, rotation );

					if( modifiedRotation )
					{	
						rayHit.transform.rotation = rotation;
					}
				}

				lastObjPingPosition = rayHit.transform.position;
			}
		}


		if ( ( Input.GetMouseButtonUp( 1 ) || ( Input.GetKeyUp( "joystick button 2" ) ) ) )// && ( gameObject.GetComponent<MovementScript>().CurrentMoveState == MovementState.Stopped ) ) // On Right Mouse Up //CREATING PINGS
		{
			RaycastHit rayHit;
			
			//Vector3 fwd = transform.TransformDirection(Vector3.forward);
			Vector3 fwd = _camera.transform.forward;
			
			if(Physics.Raycast(_camera.transform.position, fwd, out rayHit, PingDistance, maskForPing))
			{
				//Debug.Log("Rayhit on " + rayHit.transform.gameObject.name);
				
				DestroyProbablePings();	
				
				if ( rayHit.transform.gameObject.name.Equals("FloorPlane" ) ) // If Floor
				{
					int hexId = HexGrid.Manager.GetHex( rayHit.point );
					
					if( hexId > 0 )
					{
						NetworkManager.Manager.ThiefPing( hexId );	
					}
				}
				else if( !(rayHit.transform.gameObject.CompareTag("Guard")) && !(rayHit.transform.gameObject.CompareTag("Wall")) 
					&& !(rayHit.transform.gameObject.name.Equals("Roof")) && !(rayHit.transform.gameObject.CompareTag("Player"))
				        && !(rayHit.transform.gameObject.CompareTag("SAP")) )// If not the Floor
				{	
					//Debug.Log("Raycast Tag: " +rayHit.transform.gameObject.tag );
					GameObject ObjectPinged = rayHit.transform.gameObject;
						
					bool modifiedRotation = false;
					Quaternion rotation = ObjectPinged.transform.rotation;;
					
					//if( rotation % 90 != 0 )
					{
						ObjectPinged.transform.rotation = new Quaternion( 0.0f, 0.0f, 0.0f, 1.0f );	
						modifiedRotation = true;
					}

					NetworkManager.Manager.CreateThiefObjectPing(	rotation.eulerAngles.y,
																	ObjectPinged.collider.bounds.extents.x * 2,
																  	ObjectPinged.collider.bounds.extents.z * 2,
																	ObjectPinged.transform.position.x,
																  	ObjectPinged.transform.position.z ) ;
					
					CreateObjectPing ( ObjectPinged, true, rotation );
					
					if( modifiedRotation )
					{
						ObjectPinged.transform.rotation = rotation;
					}
				}
			}
		}
	}
	
	private void ProcessKeyInputs()
	{
		if( Input.GetKeyDown( KeyCode.P ) )
		{
			
		}
		
		MouseLookAround lookScript = gameObject.GetComponent<MouseLookAround>();
		if ( Input.GetKeyDown(KeyCode.Q) )
		{

			// Indicates the peek direction vector
			Vector3 _directionVector =_camera.transform.TransformDirection(Vector3.left);
			
			// If the Raycast hit something, then just reurn and set to not peeking
			if(Physics.Raycast(_camera.transform.position,_directionVector,1.0f,mask))
				return;

			
			_peeking = true;
			_peekingRight = false;
			
			lookScript.StartPeek(false);
			
			if ( PeekSnapCheck(false) )
			{
				_peekSnapRotation = transform.localEulerAngles.y;
				_peekSnapping = true;
			}
			
			// If Q is pressed stop the ping
			
			DestroyProbablePings();
			
		}
		else if ( Input.GetKeyDown(KeyCode.E) )
		{
			
			// Indicates the peek direction vector
			Vector3 _directionVector =_camera.transform.TransformDirection(Vector3.right);
			
			// If the Raycast hit something, then just reurn and set to not peeking
			if(Physics.Raycast(_camera.transform.position,_directionVector,1.0f,mask))
				return;
			
			_peeking = true;
			_peekingRight = true;
			
			lookScript.StartPeek(true);
			
			if ( PeekSnapCheck(true) )
			{
				_peekSnapRotation = transform.localEulerAngles.y;
				_peekSnapping = true;
			}
			
			// If E is pressed stop the ping
			
			DestroyProbablePings();
		}
		else if(Input.GetKeyDown(KeyCode.F) )
		{
			// Cast a ray in font of the player
			RaycastHit rayHit;
			Vector3 fwd = transform.TransformDirection(Vector3.forward);
 			if (Physics.Raycast(_camera.transform.position, fwd, out rayHit, 12.0f))
			{
				if ( rayHit.transform.gameObject.name.Equals("FloorPlane" ) && rayHit.distance < _transmitterPlacementDistance )
				{
					int thisHex = HexGrid.Manager.GetHex( rayHit.point );
					if ( HexGrid.Manager.GetTransmitter( thisHex ) == null )
					{
						// Begin placement of a new transmitter
						Quaternion rotation = Quaternion.Euler(fwd.x, fwd.y, fwd.z);
						
						// [NEED SOUND]
						// AudioManager.Manager.ThiefUsesScrambler( true );
						
						BasicScoreSystem.Manager.ScramblerUsed += 1;

						_tempScrambler = (Transform) Instantiate(ThiefGrid.Manager.ghostScrambler, rayHit.point, rotation);
						_placingScrambler=true;
					}
					else
					{
						//Debug.Log ("------------------------\nA Transmitter Already Exists Here!\n-----------------------------");
					}
				}
				else if ( rayHit.transform.gameObject.name.Equals("Transmitter_Prefab(Clone)") )
				{
					//Debug.Log ("------------------------\nA Transmitter Already Exists Here!\n-----------------------------");
					
				}

			}
		}
	}
	
	
	private void ProcessScramblerPlacement()
	{
		if(Input.GetKey(KeyCode.F))
		{
			// Cast a ray in front of the player
			RaycastHit rayHit;
			Vector3 fwd = transform.TransformDirection(Vector3.forward);
 			if (Physics.Raycast(_camera.transform.position, fwd, out rayHit, 12.0f))
			{
				//radius check
				if ( rayHit.transform.gameObject.name.Equals("FloorPlane" ) && rayHit.distance < _scramblerPlacementDistance )
				{
					int hexIndex = HexGrid.Manager.GetHex( rayHit.point );
					
					//Transmitter exist
					if ( HexGrid.Manager.GetTransmitter( hexIndex ) == null )
					{
								// Begin placement of a new Scrambler
							Quaternion rotation = Quaternion.Euler(fwd.x, fwd.y, fwd.z);
							float diffX = rayHit.point.x -_tempScrambler.position.x;
							float diffZ = rayHit.point.z -_tempScrambler.position.z;
							_tempScrambler.Translate(new Vector3( diffX, 0.0f, diffZ) );		
						    
					}
				}
					
			}
		}
		
		if(Input.GetKeyUp(KeyCode.F))
		{
			// Cast a ray in front of the player
			RaycastHit rayHit;
			Vector3 fwd = transform.TransformDirection(Vector3.forward);
 			if (Physics.Raycast(_camera.transform.position, fwd, out rayHit, 12.0f))
			{
				//radius check
				if ( rayHit.transform.gameObject.name.Equals("FloorPlane" ) && rayHit.distance < _scramblerPlacementDistance )
				{
					int hexIndex = HexGrid.Manager.GetHex( rayHit.point );
					
					//Transmitter exist
					if ( HexGrid.Manager.GetTransmitter( hexIndex ) == null )
					{

						     _placingScrambler=false;
							PlaceScrambler(_tempScrambler.position , fwd );
						    Destroy(_tempScrambler.gameObject);
						    
					}
					else
					{
						//Debug.Log ("------------------------\nA Transmitter Exists Here!\n-----------------------------");
						_placingScrambler=false;
						Destroy(_tempScrambler.gameObject);
					}
				}
				else if ( rayHit.transform.gameObject.name.Equals("Transmitter_Prefab(Clone)") )
				{
						//Debug.Log ("------------------------\nA Transmitter Exists Here!\n-----------------------------");
					    _placingScrambler=false;
					    Destroy(_tempScrambler.gameObject);
				}
				else
				{
						//Debug.Log ("------------------------\nScrambler cannot be placed this far !\n-----------------------------");
					    _placingScrambler=false;
					    Destroy(_tempScrambler.gameObject);
				}
			}
		}
	}
	
	
	void PlaceScrambler( Vector3 i_pos, Vector3 i_direction )
	{		
		//Calculate Hex Index
		int hexIndex = HexGrid.Manager.GetHex( i_pos );
		
		// Notify Hacker
		NetworkManager.Manager.ScramblerPlaced( hexIndex );
		
		// Place the Scrambler object on the floor
		ThiefGrid.Manager.PlaceScrambler( i_pos, i_direction, hexIndex );
		
		
	}
		
	private bool PeekSnapCheck(bool peekRight)
	{
		RaycastHit rayHit;
		Vector3 fwd = transform.TransformDirection(Vector3.forward);
		if (Physics.Raycast(_camera.transform.position, fwd, out rayHit, _peekSnapDistanceCheck)) // See if we are in front of a wall
		{
			RaycastHit rayHit2;
			_camera.transform.Translate( new Vector3(((peekRight)?2.0f:-2.0f), 0.0f, 0.0f), Space.Self);
			Vector3 gapCheck = _camera.transform.position;
			
			_camera.transform.Translate( new Vector3( ((peekRight)?-2.0f:2.0f), 0.0f, 0.0f), Space.Self);// move camera back
			
			if ( !(Physics.Raycast(gapCheck, fwd, out rayHit, _peekSnapDistanceCheck+1.0f)) ) // See if we are next to a gap
			{
				return true;
			}
		}
		return false;
	}
	
	
	private void ProcessPeeking()
	{
		// Check to see if the player has stopped peeking
		if ( Input.GetKeyUp(KeyCode.Q) || Input.GetKeyUp(KeyCode.E) )
		{
			//Debug.Log ("STOP PEEKING!!!");
			MouseLookAround lookScript = gameObject.GetComponent<MouseLookAround>();
			lookScript.EndPeek(_peekingRight);
			_peeking = false;
			_peekSnapping = false;
		}
		
		// Disengage peekSnapping if the player tries to move
		if ( ( gameObject.GetComponent<MovementScript>().CurrentMoveState == MovementState.Stopped ) )
		{
			_peekSnapping = false;
		}
		
		if ( _peekSnapping )
		{
			//Debug.Log ("PEEK SNAPPING");
			RaycastHit rayHit;
			Vector3 fwd = _camera.transform.TransformDirection(Vector3.forward);
			Transform peekPosition = _camera.transform;
			if ( Physics.Raycast(peekPosition.position, fwd, out rayHit, _peekSnapDistanceCheck)) // See if we are in front of a wall
			{
				
				// Translate the players position
				gameObject.transform.Translate( (new Vector3( ((_peekingRight)?0.03f:-0.03f), 0.02f , -0.01f)), Space.Self);// move camera back
				float totalRotation = Mathf.Abs( transform.localEulerAngles.y - _peekSnapRotation); // Turn off snapping if your rotated 90 degrees
//				Debug.Log ("Total Rotation: " + totalRotation);
				if ( totalRotation > 100.0f )
					_peekSnapping = false;
			}
			else 
			{
				//Debug.Log ("STOP PEEK SNAPPING");
				//_peekSnapping = false; // Turn this back on if you dont want continuous peek adjusting.
			}	
		}
	}
	
	
	private void ProcessTransmitterPlacement()
	{
		LayerMask maskIgnoreRaycast = 1 << 2;
		maskIgnoreRaycast = ~maskIgnoreRaycast;
		// Cast a ray in font of the player
		RaycastHit rayHit;
		Vector3 fwd = transform.TransformDirection(Vector3.forward);
		if (Physics.Raycast(_camera.transform.position, fwd, out rayHit, 12.0f, maskIgnoreRaycast))
		{
			// If it hits somethign perform action based on what was hit.
			if ( rayHit.transform.gameObject.name.Equals("FloorPlane" ) && rayHit.distance < _transmitterPlacementDistance )
			{
				// Make sure you are not trying to place on existing hex
				int thisHex = HexGrid.Manager.GetHex( rayHit.point );
				if ( HexGrid.Manager.GetTransmitter( thisHex ) == null )
				{
					// Continue placement verification
					float diffX = rayHit.point.x -_tempTransmitter.position.x;
					float diffZ = rayHit.point.z -_tempTransmitter.position.z;
					_tempTransmitter.Translate(new Vector3( diffX, 0.0f, diffZ) );		
				}
				
				// determine if placement is final
				if ( Input.GetMouseButtonUp( 0 ) || Input.GetKeyUp( "joystick button 3" )  )
				{
					_placingTransmitter = false;
					// Commit transmitter placement	
					//AudioManager.Manager.ThiefPlacesTransmitter( true );
					NetworkManager.Manager.SetTransmittersPlaced(1);
					//GA.API.Design.NewEvent("Thief:TransmitterPlaced", Time.timeSinceLevelLoad, _tempTransmitter.position.x,_tempTransmitter.position.y,_tempTransmitter.position.z);
					PlaceTransmitter( _tempTransmitter.position, fwd );
					ThiefManager.Manager.DecrementTransmitterCount();
					Destroy( _tempTransmitter.gameObject);
				}
			}
			else	
			{
				Destroy( _tempTransmitter.gameObject);
				_placingTransmitter = false;
			}
		}
		else
		{
			Destroy( _tempTransmitter.gameObject);
			_placingTransmitter = false;
		}
	}
	
	
	void PlaceTransmitter( Vector3 i_pos, Vector3 i_direction )
	{		
		//Calculate Hex Index
		int hexIndex = HexGrid.Manager.GetHex( i_pos );

		// Notify Hacker
		NetworkManager.Manager.TransmitterPlaced( hexIndex );
		
		// Place the Transmitter object on the floor
		ThiefGrid.Manager.PlaceTransmitter( i_pos, i_direction, hexIndex );

		/*
		gameObject.GetComponent<ThiefGUI>().transmitterPlaced = true;
		gameObject.GetComponent<ThiefGUI>().transmitterPlacedTime = Time.time;
		gameObject.GetComponent<ThiefGUI>().transmitterPosScreenSpace = new Vector3( gameObject.GetComponentInChildren<Camera>().WorldToScreenPoint(i_pos).x,
		                                                                             gameObject.GetComponentInChildren<Camera>().WorldToScreenPoint(i_pos).y - 100,
		                                                                             0 );
		*/
	}
	
	
	public void ResetTransmitter( int i_hexIndex )
	{	
		if ( _resettingTransmitter )
		{
			NetworkManager.Manager.TransmitterReset( i_hexIndex );

			_resettingTransmitter = false;
		}
	}
	
	
	public void PickUpTransmitter( int i_hexIndex )
	{
		if ( HexGrid.Manager.GetTransmitter( i_hexIndex ) != null ) // Need a way areound this for when transmitter dies.
		{

			List<int> affectedLines = HexGrid.Manager.GetTransmitter( i_hexIndex ).GetAffectedLines();
		
			// Reset the floor Grid
			NetworkManager.Manager.TransmitterPickUp( i_hexIndex );
			ThiefGrid.Manager.RemoveTransmitter( i_hexIndex, affectedLines );
			Destroy ( _currentTransmitter.gameObject );
		}
		//NetworkManager.Manager.TransmitterPickUp( i_hexIndex );
		//Destroy ( _currentTransmitter.gameObject );

	}
	
	public void DestroyProbablePings()
	{
		if( GameObject.Find("ProbableObjectPing") != null ) //Delete Probable pings if it exists
		{
			Destroy( GameObject.Find("ProbableObjectPing") );
		}
		else if( GameObject.Find("ProbableFloorPing") != null )
		{
			Destroy( GameObject.Find("ProbableFloorPing") );
		}
	}
	
	public void CreateObjectPing( GameObject i_GameObject, bool isActualPing, Quaternion rotation ) // Probable Object Ping Thief Side
	{
		MeshRenderer renderer = null; 
		Transform objPing 	  = null;
		Bounds meshBounds 	  = new Bounds( Vector3.zero, Vector3.zero );
		
		if(lastObjPingPosition != i_GameObject.transform.position || isActualPing)
		{
			{
				renderer = i_GameObject.GetComponentInChildren<MeshRenderer>(); 

				if( renderer != null )
				{	
					meshBounds = renderer.bounds;

					if( !isActualPing )
					{
						objPing = (Transform) Instantiate ( ProbablePingObject, meshBounds.center, Quaternion.identity );
						objPing.name = "ProbableObjectPing";
					}
					else
					{
						objPing = (Transform) Instantiate ( PingObject, meshBounds.center, Quaternion.identity );
					}
					if( objPing != null )
					{	
						objPing.localScale = new Vector3( meshBounds.max.x - meshBounds.min.x + 0.1f, meshBounds.max.y - meshBounds.min.y + 0.1f, meshBounds.max.z - meshBounds.min.z + 0.1f  );
					}
				}
			}

			
			if( objPing != null)
			{
				objPing.transform.rotation = rotation;
			}
		}
	}
	
	public void DisableInput()
	{
		enableKeyInputs = false;
	}
	public void EnableInput()
	{
		enableKeyInputs = true;
	}

	public void PauseGame()
	{
		if(GameManager.Manager.PlayerType == 1 ) //only thief can do this
		{
			DisableInput();
		}
	}
	
	public void unPauseGame()
	{
		if(GameManager.Manager.PlayerType == 1 ) //only thief can do this
		{
			EnableInput();
		}
	}
}

#endif