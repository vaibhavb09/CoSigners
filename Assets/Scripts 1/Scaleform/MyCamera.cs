
/**********************************************************************

Filename    :   MyCamera.cs
Content     :   Inherits from SFCamera
Created     :   
Authors     :   Ankur Mohan

Copyright   :   Copyright 2012 Autodesk, Inc. All Rights reserved.

Use of this software is subject to the terms of the Autodesk license
agreement provided at the time of installation or download, or which
otherwise accompanies this software in either electronic or hard copy form.
 
***********************************************************************/
using UnityEngine;
using System.Runtime.InteropServices;
using System;
using System.IO;
using System.Collections;
using Scaleform;

/* The user should override SFCamera and add methods for creating movies whenever specific events take place in the game.
*/
public class MyCamera : SFCamera {
	
    public MainMenu demo1 = null;
	public RenderTextureDemo rttDemo = null;
	
	////////////////
	
	public GameObject stormShutters;
	public GameObject anchorStormShutters;
	public Vector3 originalStormShutterPos;
	public GameObject[] anchors;
	public GameObject currentTarget;
	public GameObject joystick;
	public GameObject joystickOrigin;
	public GameObject dynamicLight;
	
	public const int STATE_BEGIN = 1;
	public const int STATE_MAIN_MENU = 2;
	public const int STATE_RTT = 3;
	public const int STATE_INTERACTION = 4;
	public const int STATE_FONTS = 5;
	public const int STATE_CLIK = 6;
	
	public bool freeLook = true;
	public bool stageMouseDown = false;
	public int currentState;
	public float camSpeed;
	public GameObject textureInSWFCamera;
	public GameObject rttScreen;
	
	public Quaternion originalRotation;
	
	public float sensitivity = 1f;
	
	public float rotationX = 0F;
    public float rotationY = 0F;
	
	public float sensitivityX = .0002F;
    public float sensitivityY = .0001F;
 
    public float minimumX = -360F;
    public float maximumX = 360F;
 
    public float minimumY = -60F;
    public float maximumY = 60F;
	
	public bool controllingJoystick;
	public Vector3 originalMousePos;
	public Vector3 firstMousePos;
	public Vector3 originalJoystickPos;
	public Quaternion originalJoystickRot;
    
	//
	new public void Awake()
	{
		dynamicLight = GameObject.Find("light_interior_dyanmic");
		anchorStormShutters = GameObject.Find ("AnchorStormShutters");
		stormShutters = GameObject.Find ("storm_shutters");
		originalStormShutterPos = stormShutters.transform.position;
		joystick = GameObject.Find ("Joystick");
		originalJoystickPos = joystick.transform.position;
		originalJoystickRot = joystick.transform.rotation;
		joystickOrigin = GameObject.Find ("JoystickOrigin");
		textureInSWFCamera = GameObject.Find ("TextureInSWF Camera");
		rttScreen = GameObject.Find ("monitor_screen");
		anchors = new GameObject[100];
		anchors[1] = GameObject.Find ("AnchorBegin");
		anchors[2] = GameObject.Find ("AnchorMainMenu");
		anchors[3] = GameObject.Find ("AnchorRTT");
		anchors[4] = GameObject.Find ("AnchorInteraction");
		anchors[5] = GameObject.Find ("AnchorFonts");
		anchors[6] = GameObject.Find ("AnchorCLIK");
		anchors[7] = GameObject.Find ("AnchorRTTDemo");
		anchors[8] = GameObject.Find ("AnchorInteractionDemo");
		anchors[9] = GameObject.Find ("AnchorFontsDemo");
		anchors[10]= GameObject.Find ("AnchorCLIKDemo");
		
		currentState = STATE_BEGIN;	
		currentTarget = anchors[currentState];
		camSpeed = 2.5f;
		
		transform.position = anchors[currentState].transform.position;
		transform.rotation = anchors[currentState].transform.rotation;
		
		originalRotation = transform.localRotation;
		
	}
	
    // Hides the Start function in the base SFCamera. Will be called every time the ScaleformCamera (Main Camera game object)
    // is created. Use new and not override, since return type is different from that of base::Start()
    new public  IEnumerator Start()
    {
        // The eval key must be set before any Scaleform related classes are loaded, other Scaleform Initialization will not 
        // take place.
#if (UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_EDITOR) && !UNITY_WP8
        SF_SetKey("");
#elif UNITY_IPHONE
		SF_SetKey("");
#elif UNITY_ANDROID
		SF_SetKey("");
#elif UNITY_WP8
		sf_setKey("");
#endif
		
		//For GL based platforms - Sets a number to use for Unity specific texture management.  Adjust this number if
		//you start to experience black and/or mssing textures.
#if UNITY_WP8
		sf_setTextureCount(500);
#else
		SF_SetTextureCount(500);
#endif
		return base.Start();
    }

    // Application specific code goes here
    new public void Update()
    {
        CreateGameHud();
			
		
		if(Input.GetMouseButtonDown(0))
		{
			rotationX=0;
			rotationY=0;
			originalRotation = transform.localRotation;
		}	
		
		if(currentTarget!=null)
		{
			transform.position = Vector3.Lerp(transform.position, currentTarget.transform.position, Time.deltaTime*camSpeed);
			if(stageMouseDown && freeLook)
			{
	            rotationX += Input.GetAxis("Mouse X") * sensitivityX * sensitivity;
	            rotationY += Input.GetAxis("Mouse Y") * sensitivityY * sensitivity;
	 
	            rotationX = ClampAngle (rotationX, minimumX, maximumX);
	            rotationY = ClampAngle (rotationY, minimumY, maximumY);
	 
	            Quaternion xQuaternion = Quaternion.AngleAxis (rotationX, Vector3.up);
	            Quaternion yQuaternion = Quaternion.AngleAxis (rotationY, -Vector3.right);
	 
	            transform.localRotation = originalRotation * xQuaternion * yQuaternion;
			}
			else
			{
				transform.rotation = Quaternion.Slerp(transform.rotation, currentTarget.transform.rotation, Time.deltaTime*camSpeed);
	            transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, 0);
			}
		}
		
		if (Input.GetMouseButtonDown(0))
	    {
			RaycastHit hit;
			Ray ray;
	        ray = camera.ScreenPointToRay(Input.mousePosition);
	
	        if (Physics.Raycast(ray, out hit, 100.0f, 1)) 
	        {
				if(hit.collider.name=="Joystick")
				{
	            	Debug.Log (hit.collider.name);
					controllingJoystick = true;
					originalMousePos = Input.mousePosition;
					firstMousePos = originalMousePos;
				}
	        }
	    }
		
		if(Input.GetMouseButtonUp (0))
		{
			controllingJoystick = false;
		}
		
		if(controllingJoystick)
		{
			Vector3 deltaMouse = Input.mousePosition - originalMousePos;
			joystick.transform.RotateAround(joystickOrigin.transform.position, joystickOrigin.transform.forward, deltaMouse.x*.1f);
			joystick.transform.RotateAround(joystickOrigin.transform.position, joystickOrigin.transform.right, -deltaMouse.y*.1f);
			originalMousePos = Input.mousePosition;
						
			Vector3 directionMouse = (Input.mousePosition - firstMousePos).normalized*2;
			demo1.SetCrosshairPos(directionMouse.x, -directionMouse.y);
		}
		else
		{
			joystick.transform.position = Vector3.Lerp(joystick.transform.position, originalJoystickPos, 8*Time.deltaTime);
			joystick.transform.rotation = Quaternion.Slerp(joystick.transform.rotation, originalJoystickRot, 8*Time.deltaTime);
		}
		
		if(currentState>=7 && currentState<=10)
		{
			stormShutters.transform.position = Vector3.Lerp(stormShutters.transform.position, anchorStormShutters.transform.position, Time.deltaTime);
			
		}
		else
		{
			stormShutters.transform.position = Vector3.Lerp(stormShutters.transform.position, originalStormShutterPos, Time.deltaTime);
		}
		
        base.Update ();
    }
	
    private void CreateGameHud()
    {
        if (demo1 == null)
        {
            SFMovieCreationParams creationParams = CreateMovieCreationParams("MainMenu.swf");
       //     creationParams.TheScaleModeType  = ScaleModeType.SM_ShowAll;
            creationParams.IsInitFirstFrame = false;
            demo1 = new MainMenu(this, SFMgr, creationParams);
        }
    }
	
	public static float ClampAngle (float angle, float min, float max)
    {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
        return Mathf.Clamp (angle, min, max);
    }
}