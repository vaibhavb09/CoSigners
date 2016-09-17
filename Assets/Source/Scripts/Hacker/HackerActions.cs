using UnityEngine;
using System.Collections;

public class HackerActions : MonoBehaviour {
	
	// Universal Members
	private bool pingOn;
	public bool _disabled;
	private int lastPingHex;

	public bool VPAD_Left;
	public bool VPAD_Right;
	public bool VPAD_Up;
	public bool VPAD_Down;
	public bool VPAD_KeyMove;

	
	// PC Related Members
	bool leftMouseDown;
	bool rightMouseDown;
	bool middleMouseDown;
	bool simulateIPad;
	bool _pingDragging;
	public bool _vPadDragging;
	Vector2 lastMousePos;
	public float screenEdgeDelta;
	Vector2 GUIMousePosition = Vector2.zero;

	//iPad related members
	bool twoFingersDown;
	const float distance = 1.0f;
	const float speedFactor = 0.5f;
	Vector2 currentPoint = Vector2.zero;
	Vector2 dragVelocity = Vector2.zero;

	private int currentNodeIndexGettingClicked = -2;
	private Vector3 currentNodePositionGettingClicked = Vector3.zero;
	
	private bool potentialLinkCreationOnNode = false;
	private HackerGUI myGUIScript;
	#region Properties
	public bool PingActive
	{
		get{
			return pingOn;
		}
	}
	#endregion

	// Use this for initialization
	void Start () 
	{
		leftMouseDown = false;
		rightMouseDown = false;
		middleMouseDown = false;
		twoFingersDown = false;
		VPAD_KeyMove = false;
		_pingDragging = false;
		#if UNITY_IPHONE || UNITY_ANDROID
		simulateIPad = true;
		#else
		simulateIPad = false;
		#endif
		pingOn = false;
		lastPingHex = -1;
		_disabled = false;
		myGUIScript = gameObject.GetComponent<HackerGUI>();
	}

	void OnGUI()
	{
		GUIMousePosition = Event.current.mousePosition;
	}
	
	#region public API
	
	public void DisableHackerActions()
	{
		_disabled = true;
	}
	public void EnableHackerActions()
	{
		_disabled = false;
	}

	#endregion
	
	
	// Update is called once per frame
	void Update () 
	{
	  	if(!HackerManager.Manager.gameIsPaused)
      	{
			if(GameManager.Manager.PlayerType == 2 ) //only hacker can do this
			{
			
				//Clean up bools
				if( _disabled )
				{
					leftMouseDown = false;
					rightMouseDown = false;
					middleMouseDown = false;
				}
			
#if UNITY_IPHONE || UNITY_ANDROID
				CheckiOSInputs();
#else
				//Debug.Log("CHECKING FOR HACKER CONTROLS!!");
				if ( !_disabled && !CheckPCInputs() )// Check if there are any new inputs
				{
					if( !DetectHackerKeyMovement() )// if there are no camaera key inputs, check the other inputs
					{
						if ( leftMouseDown )// Left click and drag
						{
							if ( _pingDragging )
								HandleRightClickAndDrag( Camera.mainCamera.ScreenToWorldPoint(Input.mousePosition), Input.mousePosition );
							else
								HandleLeftClickAndDrag( Camera.mainCamera.ScreenToWorldPoint(Input.mousePosition), Input.mousePosition );
						}
						else if ( rightMouseDown )// Right click and drag
						{
							HandleRightClickAndDrag( Camera.mainCamera.ScreenToWorldPoint(Input.mousePosition), Input.mousePosition );
						}
						else if( middleMouseDown )//Middle click and drag
						{
							HandleMiddleClickAndDrag( Input.mousePosition );
						}
						else if (VirtualKeyboard.isWindowsTablet == false)// if there are no other inputs at all check for mouse camera movement.
						{
							DetectHackerMouseMovement();
						}
					}
				}
#endif
			}
		}
	}

	// ----------------------------------------------------------------------------------------------
	//                    PC SPECIFIC FUNCTIONS
	// ----------------------------------------------------------------------------------------------
	#region PC ACTIONS
	private bool CheckPCInputs()
	{
		if ( Input.GetMouseButtonDown(0) )//Left click in this frame
		{
			leftMouseDown = true;
			HandleLeftClick( Camera.mainCamera.ScreenToWorldPoint(Input.mousePosition), Input.mousePosition );
			return true;
		}
		else if ( Input.GetMouseButtonDown(1) )// Right click in this frame
		{
			rightMouseDown = true;
			HandleRightClick( Camera.mainCamera.ScreenToWorldPoint(Input.mousePosition), Input.mousePosition );
			return true;
		}
		else if ( Input.GetMouseButtonUp(0) )// Left click release
		{
			leftMouseDown = false;
			_pingDragging = false;
			VPAD_KeyMove = false;
			lastPingHex = -1;
			HandleLeftRelease(Camera.mainCamera.ScreenToWorldPoint(Input.mousePosition));
			return true;
		}
		else if ( Input.GetMouseButtonUp(1) )// Right click Relase
		{
			rightMouseDown = false;
			lastPingHex = -1;
			HandleRightRelease();
			return true;
		}
		else if( Input.GetMouseButtonDown(2) )//Middle mouse button down
		{
			middleMouseDown = true;
			HandleMiddleClick( Input.mousePosition );
			return true;
		}
		else if( Input.GetMouseButtonUp(2) )//Middle mouse button down
		{
			middleMouseDown = false;
			//HandleRightRelease();
			return true;
		}
		
		if ( Input.GetKeyDown(KeyCode.Space) )
		{
			//Debug.Log ("DROP THREAT BY 10");
			//HackerThreat.Manager.DecreaseThreatAmount( 10 );
			HandleZoomOut();
		}

		return false;
	}

	private float PosW(float i_increment)
	{
		return (Screen.width/64)*i_increment;
	}
	
	private float PosH(float i_increment)
	{
		return (Screen.height/36)*i_increment;
	}

	// detects whether a map-moving key has been pressed, and sets the appropriate bools.
	public bool DetectHackerKeyMovement()
	{
		//Debug.Log("Hacker Actions VPAD_Up - " + VPAD_Up);
		bool keyMovement;
			keyMovement = false;

		bool right=false, left=false, up=false, down=false;

		if (VPAD_KeyMove)
		{
			//Debug.Log ("Calling HandleVPADMovement");
			HandleVPADMovement();
			keyMovement = true;
		}
		else
		{
			if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)
			   || VPAD_Up )
			{
				up = true;
				keyMovement = true;
			}
			if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)
			   || VPAD_Left )
			{
				left = true;
				keyMovement = true;
			}
			if(Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)
			   || VPAD_Down )
			{
				down = true;
				keyMovement = true;
			}
			if(   Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)
			   || VPAD_Right )
			{
				right = true;
				keyMovement = true;
			}

			if ( keyMovement )
				TopDown_Camera.Manager.MoveCamera(right, left, up, down, true);
			else
				TopDown_Camera.Manager.StopCamera(true, true, true);
		}

		 VPAD_Up=VPAD_Down=VPAD_Left=VPAD_Right=false;
		return keyMovement;
	}


	//detects whether the mouse is on the edge of the screen, and sets the appropriate movement bools.
	private bool DetectHackerMouseMovement()
	{
		bool mouseAtEdge = false;
		bool right=false, left=false, up=false, down=false;

		if (Input.mousePosition.x >= Screen.width - screenEdgeDelta)
		{
			right = true;
			mouseAtEdge = true;
		}
		if (Input.mousePosition.x <= screenEdgeDelta)
		{
			left = true;
			mouseAtEdge = true;
		}
		if (Input.mousePosition.y >= Screen.height - screenEdgeDelta)
		{
			up = true;
			mouseAtEdge = true;
		}
		if (Input.mousePosition.y <= screenEdgeDelta)
		{
			down = true;
			mouseAtEdge = true;
		}

		if ( mouseAtEdge )
			TopDown_Camera.Manager.MoveCamera(right, left, up, down, false);
		else
			TopDown_Camera.Manager.StopCamera(true, true, true);

		return mouseAtEdge;
	}


	/*private bool IsWSADorArrowKeysPressed()
	{
		if(
			Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow) || 
			Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow) ||
			Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow) ||
			Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow) )
		return true;
		   else
		return false;

	}*/

	/*private void MoveScreenInDirection(Vector3 dir)
	{
		if (screenSpeed < screenMoveSpeed && !screenMovedLastFrame)
		{
			screenSpeed +=  Time.deltaTime * screenAccelerateSpeed;
			gameObject.GetComponent<TopDown_Camera>().MoveCameraAmount(dir * Time.deltaTime * screenSpeed);
		}
		else
		{
			gameObject.GetComponent<TopDown_Camera>().MoveCameraAmount(dir * Time.deltaTime * screenMoveSpeed);
		}

	}*/

	//private vo

	private void HandleLeftClick(Vector3 i_pos, Vector2 i_mousePos)
	{
		if ( !CheckHitGUI(i_mousePos) )// if you did not hit a GUI button
		{

			if( !TopDown_Camera.Manager.zoomedIn )
				HandleZoomIn( Camera.mainCamera.ScreenToWorldPoint(i_mousePos) );
			
			if ( !TopDown_Camera.Manager.InputLocked )// Make sure your not zoomed out to far.
			{
				if ( simulateIPad && pingOn ) // Try to ping
				{
					HandlePingClick(i_pos);
				}
				else // Standard click on map
				{
					//Debug.Log("TopDown_Camera.Manager.InputLocked :" +TopDown_Camera.Manager.InputLocked );
					HandleLinkClick(i_pos);
				}
			}
			//else
			//{
			//	HandleZoomIn( Camera.mainCamera.ScreenToWorldPoint(Input.mousePosition) );
			//}
		}
	}
	
	private void HandleMiddleClick( Vector2 i_mousePos )
	{
		if ( !CheckHitGUI(i_mousePos) && !TopDown_Camera.Manager.InputLocked )
		{
			lastMousePos = i_mousePos;
		}
	}
	
	private void HandleRightClick(Vector3 i_pos, Vector2 i_mousePos)
	{
		if ( !CheckHitGUI(i_mousePos) )// if you did not hit a GUI button
		{
			if ( simulateIPad )
			{
				// Start Camera Move
				lastMousePos = i_mousePos;
				
				//int tempIndex = HexGrid.Manager.GetIndex( i_pos );
				//Debug.Log ("Getting Pivots touching index: " + tempIndex);
				//PivotManager.Manager.GetPivotsTouchingIndex( tempIndex );
			}
			else
			{
				HandlePingClick(i_pos);
			}	
		}
	}
	
	private void HandleRightRelease()
	{
		HandleReleaseCameraMove();
	}
	
	
	private void HandleLeftRelease(Vector3 i_pos)
	{
		HandleRelease(i_pos);
	}
	
	
	private void HandleLeftClickAndDrag(Vector3 i_pos, Vector2 i_mousePos)
	{
		if ( !TopDown_Camera.Manager.InputLocked && !CheckHitGUI(i_mousePos) ) // Fist make sure your not zoom blocked
		{
			if ( simulateIPad && pingOn ) // Try to ping
			{
				HandlePingDrag(i_pos);
			}
			else //Standard drag on map
			{
				//Debug.Log("TopDown_Camera.Manager.InputLocked: " +TopDown_Camera.Manager.InputLocked);
				HandleLinkDrag(i_pos);
			}
		}
	}
	
	private void HandleMiddleClickAndDrag( Vector2 i_mousePos )
	{
		if ( !CheckHitGUI(i_mousePos) && !TopDown_Camera.Manager.InputLocked )
		{
			HandleCameraDrag( i_mousePos );
		}
	}
	
	private void HandleRightClickAndDrag(Vector3 i_pos, Vector2 i_mousePos)
	{
		if ( !CheckHitGUI(i_mousePos) /*&& !TopDown_Camera.Manager.InputLocked */)
		{
			if ( simulateIPad )
			{
				HandleCameraDrag(i_mousePos);
			}
			else
			{
				HandlePingDrag(i_pos);
			}
		}
	}
	private void HandleVPADMovement()
	{
		//Vector2 directionVector = GUIMousePosition - new Vector2 ((PosW(8)),PosH(32));
		Vector2 directionVector = GUIMousePosition - new Vector2 (PosW (myGUIScript.GUIVirtualJoystickX + (myGUIScript.GUIVirtualJoystickWidth/2)),
		                                                          PosH (myGUIScript.GUIVirtualJoystickY + (myGUIScript.GUIVirtualJoystickHeight/2)));
		directionVector.Normalize();
		directionVector = new Vector2(- directionVector.x, directionVector.y);
		//Debug.Log ("Event.current.mousePosition - " + Event.current.mousePosition.x + " ' " + Event.current.mousePosition.y + " (PosW(9) - " + PosW(9) + "PosH(24) - " + PosH(24) + "directionVector - " + directionVector);
		if (Vector2.Distance(GUIMousePosition,new Vector2 (PosW (myGUIScript.GUIVirtualJoystickX + (myGUIScript.GUIVirtualJoystickWidth/2)),
		                                                   PosH (myGUIScript.GUIVirtualJoystickY + (myGUIScript.GUIVirtualJoystickHeight/2)))) 
		    												< PosW (myGUIScript.GUIVirtualJoystickWidth/4))
		{
			gameObject.GetComponent<TopDown_Camera>().MoveCameraAmount(directionVector * Time.deltaTime * 
			                             (Vector2.Distance(GUIMousePosition,new Vector2 (PosW (myGUIScript.GUIVirtualJoystickX + (myGUIScript.GUIVirtualJoystickWidth/2)),
			                                  PosH (myGUIScript.GUIVirtualJoystickY + (myGUIScript.GUIVirtualJoystickHeight/2)))) 
			 									/ PosW (myGUIScript.GUIVirtualJoystickWidth/4)) * 400);
		}
		else
		{
			gameObject.GetComponent<TopDown_Camera>().MoveCameraAmount(directionVector * Time.deltaTime * 400);
		}

	}
	
	#endregion
	
	
	// ----------------------------------------------------------------------------------------------
	//                    IPAD SPECIFIC FUNCTIONS
	// ----------------------------------------------------------------------------------------------
	private void CheckiOSInputs()
	{
		if( Input.touchCount == 1  ) //Link mode
		{
			Touch touch = Input.GetTouch(0);

			if( CheckHitGUI(touch.position) )
				return;

			if( !pingOn )
			{
				if( touch.phase == TouchPhase.Began ) 
				{
					leftMouseDown = true;
					HandleLinkClick( Camera.mainCamera.ScreenToWorldPoint(touch.position) );
				}
				if( touch.phase == TouchPhase.Moved )
				{
					if( _pingDragging )
						HandlePingDrag( Camera.mainCamera.ScreenToWorldPoint(touch.position) );
					else
						HandleLinkDrag( Camera.mainCamera.ScreenToWorldPoint(touch.position) );
				}
				if( touch.phase == TouchPhase.Ended )
				{
					leftMouseDown = false;
					_pingDragging = false;
					lastPingHex = -1;
					HandleRelease( Camera.mainCamera.ScreenToWorldPoint(touch.position) );
				}
			}
			else //Ping mode
			{
				if( touch.phase == TouchPhase.Began ) 
				{
					HandlePingClick( Camera.mainCamera.ScreenToWorldPoint(touch.position) );
				}
				if( touch.phase == TouchPhase.Moved )
				{
					HandlePingDrag( Camera.mainCamera.ScreenToWorldPoint(touch.position) );
				}
			}
			
			if( touch.tapCount == 1 ) //Zoom in camera 
			{
				if( !TopDown_Camera.Manager.zoomedIn && !CheckHitGUI( touch.position ))
				HandleZoomIn( Camera.mainCamera.ScreenToWorldPoint(touch.position) );
			}
		}
		
		if( Input.touchCount == 2 ) //Panning camera
		{
			if( CheckHitGUI(Input.GetTouch(0).position) || CheckHitGUI(Input.GetTouch(1).position) )
				return;

			if( Input.GetTouch(0).phase == TouchPhase.Began || Input.GetTouch(1).phase == TouchPhase.Began )
			{ 
				currentPoint = GetMidpoint( Input.GetTouch(0), Input.GetTouch(1) );
			}
			
			if( Input.GetTouch(0).phase == TouchPhase.Moved && Input.GetTouch(1).phase == TouchPhase.Moved )
			{
				HandleCameraDrag(GetDeltaDrag());
			}
			else
			{
				dragVelocity = Vector2.zero;
			}
		}
	}
	
	private Vector2 GetMidpoint(Touch touch1, Touch touch2)
    {
       return Vector2.Lerp (touch1.position, touch2.position, 0.5f);
    }
	
	private Vector2 GetDeltaDrag()
	{	
		Vector2 prevPoint = currentPoint;
	  	currentPoint = GetMidpoint( Input.GetTouch(0), Input.GetTouch(1) );
		
        Vector2 panMove = currentPoint - prevPoint;
		
		float deltaY = panMove.y * distance * speedFactor;
        float deltaX = panMove.x * distance * speedFactor;
	
		Vector2 delta = new Vector2( deltaX, deltaY );		
		dragVelocity += delta;
		//dragVelocity.y = Mathf.Clamp( dragVelocity.y, -10.0f, 10.0f );
		//dragVelocity.x = Mathf.Clamp( dragVelocity.x, -10.0f, 10.0f );
		
       	//return dragVelocity;
		return delta;
	}
	
	// ----------------------------------------------------------------------------------------------
	//                    UNIVERSAL FUNCTIONS
	// ----------------------------------------------------------------------------------------------
	#region UNIVERSAL FUNCTIONS
	// Checks for and responds to GUI element clicks
	private bool CheckHitGUI(Vector2 i_pos)
	{
		bool GUIhit = false;

		//if (VirtualKeyboard.isWindowsTablet == true)
		if (VirtualKeyboard.isWindowsTablet == true)
		{
			if (VPAD_KeyMove)
			{
				//Debug.Log ("GUI HIT TRUE");
				GUIhit = true;
			}

			float VPADRightVerticalBorder = PosW (myGUIScript.GUIVirtualJoystickX + myGUIScript.GUIVirtualJoystickWidth);
			float VPADTopHorizontalBoder = PosH(36) - PosH (myGUIScript.GUIVirtualJoystickHeight - 2);
			//Debug.Log ("MouseX=" + i_pos.x + "MouseY=" + i_pos.y + "VPADRightVerticalBorder=" + VPADRightVerticalBorder + "VPADTopHorizontalBoder=" + VPADTopHorizontalBoder);
			if (i_pos.x < VPADRightVerticalBorder &&  i_pos.y < VPADTopHorizontalBoder )
				GUIhit = true;
		
			Rect VKeyboardToggleRectInMouseCoordinates = new Rect(VirtualKeyboard.toggleRect.xMin, 
			                                                      Screen.height - VirtualKeyboard.toggleRect.yMax,
			                                                      VirtualKeyboard.toggleRect.width,
			                                                      VirtualKeyboard.toggleRect.height);
			if (VKeyboardToggleRectInMouseCoordinates.Contains(i_pos))
				GUIhit = true;

			if (VirtualKeyboard.enabled == true )
			{
				//Debug.Log ("i_pos = " + i_pos);
				//Debug.Log ("VirtualKeyboard.windowRect = " + VirtualKeyboard.windowRect.x + " " + VirtualKeyboard.windowRect.y);
				Rect VKeyboardRectInMouseCoordinates = new Rect(VirtualKeyboard.windowRect.xMin, 
				                                                Screen.height - VirtualKeyboard.windowRect.yMax,
				                                                VirtualKeyboard.windowRect.width,
				                                                VirtualKeyboard.windowRect.height);
				//Debug.Log ("VKeyboardRectInMouseCoordinates = " + VKeyboardRectInMouseCoordinates.x + " " + VKeyboardRectInMouseCoordinates.y);
				if (VKeyboardRectInMouseCoordinates.Contains(i_pos))
					GUIhit = true;
			}
		}

		if ( HackerManager.Manager.legendOpen )
		{
			if ( i_pos.x < Screen.width/4.0f )
			{
				GUIhit = true;
				// Check for close button hit
				if ( i_pos.x > Screen.width/5.25f && i_pos.y > Screen.height/2.4f && i_pos.y < Screen.height/1.92f)
				{
					//Debug.Log ("HIT LEGEND BUTTON: OPEN");
					HackerManager.Manager.CloseLegend();
				}
			}
		}
		// Check for open legend hit
		else if ( i_pos.x < Screen.width/24.0f && i_pos.y > Screen.height/2.4f && i_pos.y < Screen.height/1.92f)
		{
			//Debug.Log ("HIT LEGEND BUTTON: CLOSED");
			HackerManager.Manager.OpenLegend();
			GUIhit = true;
		}

		//Debug.Log ("GUI HIT!!! - " + GUIhit);
		//Debug.Log ("Position - " + i_pos.x + "," + i_pos.y);
		return GUIhit;
/*
		Debug.Log ("CHECK GUI HIT");
		float topLeftLimit = (float) Screen.height - (Screen.height * 0.055f);
		float topRightLimit = (float) Screen.height - (Screen.height * 0.11f);
		float topHorzDivide = (float) (Screen.width * 0.6f);
		
		Debug.Log ("MouseX=" + i_pos.x + "MouseY=" + i_pos.y + "leftLimit=" + topLeftLimit + "rightLimit=" + topRightLimit);
		if ( i_pos.x < topHorzDivide )
		{
			if ( i_pos.y > topLeftLimit )
				return true;
		}
		else
		{
			if ( i_pos.y > topRightLimit )
				return true;
		}

		//Debug.Log ("NO GUI HIT");
*/
	}
	
	
	// Handles an event for creating a ping
	private void HandlePingClick(Vector3 i_pos)
	{
		//if( TopDown_Camera.Manager.InputLocked /*|| TutorialCues.Manager.IsActive*/)
		//	return;
		if (VPAD_KeyMove)
			return;
		//Debug.Log ("PING CLICK");
		int pingHex = HexGrid.Manager.GetHex( i_pos );
		
		if ( pingHex >= 0 ) // Make sure a valid hex was clicked
		{
			lastPingHex = pingHex;
			// Create the ping on hackers side
			HackerManager.Manager.CreatePing( i_pos, pingHex );
			
			// Notify Thief of ping
			NetworkManager.Manager.HackerPing( pingHex );
		}	
	}
	
	
	// Handles an event for interacting with links and pivots
	private void HandleLinkClick(Vector3 i_pos)
	{
		// If inputs are locked return
		if( TopDown_Camera.Manager.InputLocked )
			return;
		
		int indexClicked = HexGrid.Manager.GetIndex( i_pos );

		// TEST: Handle Ping click on center of hex 
		if ( indexClicked == -1 ) // This must be a ping click
		{
			_pingDragging = true;
			HandlePingClick( i_pos );
		}

		// If there is no power here just ping
		if ( !ConnectionManager.Manager.IsConnected(indexClicked) && !PivotManager.Manager.AreLinesTouchingIndex(indexClicked) )
		{
			_pingDragging = true;
			HandlePingClick( i_pos );
		}

		// If you clicked on an active pivot rotate the links
		if ( PivotManager.Manager.ActivePivot != -1 && indexClicked == PivotManager.Manager.ActivePivot 
			&& !GraphManager.Manager.NodeExistsOnIndex( indexClicked ))
		{
			//Debug.Log ("Called Rotate Pivot");
			NetworkManager.Manager.RotatePivot( indexClicked, true );
		}
		else // Otherwies check for other types of inputs
		{
			PivotManager.Manager.DisableActivePivot();
			PivotManager.Manager.justRotated = false;
			if ( GraphManager.Manager.NodeExistsOnIndex( indexClicked ) ) // Clicked on a node
			{
				currentNodeIndexGettingClicked = indexClicked;
				currentNodePositionGettingClicked = i_pos;
				potentialLinkCreationOnNode = true;
				//this should be called when mouse is up
				//GraphManager.Manager.HandleNodeClick( indexClicked );
			}
			else if ( !PivotManager.Manager.PivotExists(indexClicked) && HexGrid.Manager.IsAvailable(indexClicked) && ConnectionManager.Manager.IsConnected(indexClicked) ) // Clicked on an empty index
			{
				PivotManager.Manager.StartCreateNewLink( indexClicked );	
			}
		}

	}
	
	// Handles releasing or lifting the finger.
	private void HandleRelease(Vector3 i_pos)
	{
		int indexClicked = HexGrid.Manager.GetIndex( i_pos );
		if ( indexClicked != -1 && HexGrid.Manager.IsAvailable(indexClicked) && !PivotManager.Manager.justRotated)
		{
			if ( PivotManager.Manager.ConsecutiveLinks == 0 )
				PivotManager.Manager.EnableRotation(indexClicked);
		}
		
		if(currentNodeIndexGettingClicked == indexClicked)
		{
			GraphManager.Manager.HandleNodeClick( indexClicked );
		}
		
		if ( PivotManager.Manager.IsCreatingNewLink() ) // If you were creating a link
		{
			PivotManager.Manager.CancelNewLink();
		}
	}
	
	
	// Handles an event for dragging the ping across the map
	private void HandlePingDrag(Vector3 i_pos)
	{
		//if( TopDown_Camera.Manager.InputLocked /*|| TutorialCues.Manager.IsActive*/)
		//	return;
		
		//Debug.Log ("PING DRAG");
		if (VPAD_KeyMove)
			return;
		int thisHex = HexGrid.Manager.GetHex(i_pos);
		if ( (thisHex != lastPingHex) && thisHex >= 0 )
		{
			lastPingHex = thisHex;
			// Create the ping on hackers side
			HackerManager.Manager.CreatePing( i_pos, thisHex );
			
			// Notify Thief of ping
			NetworkManager.Manager.HackerPing( thisHex );
		}
	}
	
	
	// Handles an event for draging link interactions across the map
	private void HandleLinkDrag(Vector3 i_pos)
	{	
		if( TopDown_Camera.Manager.InputLocked /*|| TutorialCues.Manager.IsActive*/)
			return;
		
		//Debug.Log ("LINK DRAG");
		if ( PivotManager.Manager.IsCreatingNewLink() )// check if you are in the process of making a new link
		{
			PivotManager.Manager.TestLinkTargetDrag( i_pos );
		}
		if ( potentialLinkCreationOnNode == true)
		{
			int indexClicked = HexGrid.Manager.GetIndex( i_pos );
			//Debug.Log("Index Clicked:" + indexClicked);
			if(i_pos != currentNodePositionGettingClicked && indexClicked != -1 && HexGrid.Manager.IsAvailable(indexClicked) && ConnectionManager.Manager.IsConnected(indexClicked) )
			{
				//Debug.Log ("Link drag Creation Test!!!");
				PivotManager.Manager.StartCreateNewLink(indexClicked);
				potentialLinkCreationOnNode = false;
			}
		}
	}
	
	
	
	
#if UNITY_IPHONE || UNITY_ANDROID
	private void HandleCameraDrag( Vector2 delta )
	{
		if( !gameObject.GetComponent<TopDown_Camera>().zoomedIn )
			return;
		
		gameObject.GetComponent<TopDown_Camera>().MoveCameraAmount(delta);
	}
#else
	// Handles an event for draging camera interactions on the map.
	private void HandleCameraDrag(Vector2 i_mousePos)
	{
		if( TopDown_Camera.Manager.InputLocked )
			return;
		
		Vector2 diff = new Vector2(i_mousePos.x - lastMousePos.x, i_mousePos.y - lastMousePos.y);
		//Debug.Log ("difference= x:" + diff.x + " y:" + diff.y);
		gameObject.GetComponent<TopDown_Camera>().MoveCameraAmount(diff);
		lastMousePos = i_mousePos;

	}
#endif
	
	private void HandleReleaseCameraMove()
	{
		TopDown_Camera.Manager.ReleaseCameraMove();
	}
	
	
	public void HandleZoomIn( Vector3 i_position)
	{
		//if ( !TutorialCues.Manager.IsActive )
		{
			TopDown_Camera.Manager.ZoomIn( Camera.mainCamera.ScreenToWorldPoint(Input.mousePosition) );

		
		// [SOUND TAG] [Zoom_Button]
			
		}
	}
	
	public void HandleZoomOut()
	{
		//if ( !TutorialCues.Manager.IsActive )
			TopDown_Camera.Manager.ZoomOut( );
	}
	#endregion
	
	
	#region GUI Actions
	public void PingToggleClicked()
	{
		if ( pingOn )
		{
			HexGrid.Manager.DisablePingMarkers();
			pingOn = false;
		}
		else
		{
			HexGrid.Manager.EnablePingMarkers();
			pingOn = true;
		}
	}
	#endregion

	public void PauseGame()
	{
		if(GameManager.Manager.PlayerType == 2 ) //only hacker can do this
		{
		   Time.timeScale=0;
		}
	}
	
	public void unPauseGame()
	{
		if(GameManager.Manager.PlayerType == 2 ) //only hacker can do this
		{
		    Time.timeScale=1;
		}
	}
}
