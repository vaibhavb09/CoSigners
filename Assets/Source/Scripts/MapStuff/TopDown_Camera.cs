using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[RequireComponent(typeof(AudioSource))]
public class TopDown_Camera : MonoBehaviour  
{	
	public float tabletCameraSize;
	private static TopDown_Camera m_instance = null;
	private float originX = 0;
	private float originZ = 0;
	public GameObject BackPlane;
	public Vector3 BackPlaneOrigin;	
	public bool zoomedIn;
	private bool zoomBlock;
	private bool zooming;
	private bool panning;
	private Vector3 StartPos;
	private float StartSize;
	private Vector3 EndPos;
	private float EndSize;
	private GenericTimer myZoomTimer;
	private GenericTimer myPanTimer;
	private float _standardCameraSize;	
	private float zoomDurration;
	private float cameraMoveScaleFactor;
	private Vector2 currentSpeed;
	public float decelerationFactor, keyAccelerationFactor, mouseAccelerationFactor, keyTopSpeed, mouseTopSpeed;

	#region Properties
	public static TopDown_Camera Manager
	{
		get
		{
			if(m_instance == null)
			{
				m_instance = GameObject.Find("TopDownCamera").GetComponent<TopDown_Camera>();			
			}
			return m_instance;
		}
	}
	
	public bool Zooming
	{
		get{
			return zooming;
		}
	}
	
	public bool ZoomBlocked
	{
		get{
			return zoomBlock;
		}
	}
	
	public bool InputLocked
	{
		get
		{
			//Debug.Log("Panning:" + panning );
			//Debug.Log("ZoomedIn: " + zoomedIn );
			return ( (!zoomedIn) || panning || zooming );	
		}
	}
	#endregion
	
	public void InitializeSounds()
	{
		if(GameManager.Manager.PlayerType == 2)
		{			
			if( Application.loadedLevelName.CompareTo("JM_53") == 0 )
				soundMan.soundMgr.playOnSource(null,"FinalLevelPart1",true,GameManager.Manager.PlayerType,2,0.6f);
			else
				soundMan.soundMgr.playOnSource(null,"Hacker_BGM_loop",true,GameManager.Manager.PlayerType,2,0.6f);
		}
	}
	
	// Use this for initialization
	void Start () 
	{
		zooming = false;
		zoomedIn = false;
		zoomDurration = 0.4f;

#if UNITY_IPHONE || UNITY_ANDROID
		_standardCameraSize = 7.0f;
#else
		if (VirtualKeyboard.isWindowsTablet)
			_standardCameraSize = tabletCameraSize;
		else
			_standardCameraSize = 11.0f;
#endif

		cameraMoveScaleFactor = 30.0f;
		zoomBlock = false;
		panning = false;
		gameObject.camera.orthographicSize = _standardCameraSize;
		currentSpeed = new Vector2( 0, 0);

		BackPlane = GameObject.Find("Back_plane");
		if ( BackPlane != null)
		{
			BackPlaneOrigin = BackPlane.transform.position;
		}
		
		if(GameManager.Manager.PlayerType == 2)
		{
			gameObject.AddComponent("AudioListener");
			ZoomIn( GameObject.Find("player2DPrefab(Clone)").transform.position );
		}

		// Set camera movement values if not set already
		if ( decelerationFactor == 0 )
		{
			decelerationFactor = 0.9f;
			keyAccelerationFactor = 0.25f;
			mouseAccelerationFactor = 0.25f;
			keyTopSpeed = 6.0f;
			mouseTopSpeed = 6.0f;
		}

		InitializeBackPlane();

	}
	
	
	
	
	// Update is called once per frame
	void Update () {
		if ( zooming )
		{
			ZoomAnimation();
		}
		else if ( panning )
		{
			//PanAnimation();
		}
	}
	
	
	public void ZoomOut()
	{
		zoomedIn = false;
		StartPos = gameObject.camera.transform.position;
		StartSize = gameObject.camera.orthographicSize;
		float width = ((float)((HexGrid.Manager.rowSize-1)/2) * 3 + 0.5f);
		//Debug.Log ("ROw Size=" + HexGrid.Manager.rowSize + "  gridSize=" + HexGrid.Manager.gridSize + "  height=" + (HexGrid.Manager.gridSize/HexGrid.Manager.rowSize));
		float height = (((float)(HexGrid.Manager.gridSize/HexGrid.Manager.rowSize)/2.0f+0.5f)*1.732f);
	
		float endX = -(width / 2.0f);
		float endZ = (height / 2.0f)-(height/12.0f);
		EndPos = new Vector3( endX, 120.0f, endZ);
		
		if ( width/4.0f > (height*0.85f)/3.0f )
		{
			// base size on the width
			//Debug.Log ("WIDTH ZOOM");
			EndSize = width*0.42f;
		}
		else
		{
			// Base size on the height
			//Debug.Log ("HEIGHT ZOOM");
			EndSize = height*0.55f;
		}
		
		StartAnimation(false);
	}
	
	
	public void ZoomIn( Vector3 i_zoomPoint )
	{
		zoomedIn = true;
		PivotManager.Manager.CancelNewLink();
		StartPos = gameObject.camera.transform.position;
		StartSize = gameObject.camera.orthographicSize;
		
		float width = ((float)((HexGrid.Manager.rowSize-1)/2) * 3 + 0.5f);
		float height = (((float)(HexGrid.Manager.gridSize/HexGrid.Manager.rowSize)/2.0f+0.5f)*1.732f);
		
		if( i_zoomPoint.x > (-width * 0.25f) )
			i_zoomPoint.x = -width * 0.25f;
		
		if( i_zoomPoint.x < (-width * 0.75f) )
			i_zoomPoint.x = -width * 0.75f;
		
		if( i_zoomPoint.z < (height * 0.25f) )
			i_zoomPoint.z = height * 0.25f;
		
		if( i_zoomPoint.z > (height * 0.75f) )
			i_zoomPoint.z = height * 0.75f;
		
		float endX = i_zoomPoint.x;
		float endZ = i_zoomPoint.z;
		EndPos = new Vector3( endX, 120.0f, endZ);
		EndSize = _standardCameraSize;
		
		StartAnimation(true);
	}
	
	public void MoveCamera(bool i_right, bool i_left, bool i_up, bool i_down, bool withKeys)
	{
		float topSpeed, acc; 
		// Set movement settings based on the type of input
		if ( withKeys )
		{
			topSpeed = keyTopSpeed;
			acc = keyAccelerationFactor;
		}
		else
		{
			topSpeed = mouseTopSpeed;
			acc = mouseAccelerationFactor;
		}

		// Set the new speed
		if ( i_right || i_left )
		{
			if ( i_right )
				currentSpeed.x -= acc;
			if ( i_left )
				currentSpeed.x += acc;
		}
		else
		{
			StopCamera(true, false, false);
		}

		if ( i_up || i_down )
		{
			if ( i_up )
				currentSpeed.y -= acc;
			if ( i_down )
				currentSpeed.y += acc;
		}
		else
		{
			StopCamera(false, true, false);
		}

		// Make sure speed does not exceed top speed
		if ( Mathf.Abs(currentSpeed.x) > topSpeed )
			currentSpeed.x = (currentSpeed.x >= 0)? topSpeed : -topSpeed;
		if ( Mathf.Abs(currentSpeed.y) > topSpeed )
			currentSpeed.y = (currentSpeed.y >= 0)? topSpeed : -topSpeed;

		// Move the camera
		panning = true;
		MoveCameraAmount ( currentSpeed );
	}
	

	public void StopCamera(bool x, bool y, bool allStop)
	{
		// decelerate x
		if ( x )
		{
			if ( Mathf.Abs(currentSpeed.x) > 0.1 ) 
				currentSpeed.x *= decelerationFactor;
			else
				currentSpeed.x = 0;
		}

		// decelerate y
		if ( y )
		{
			if ( Mathf.Abs(currentSpeed.y) > 0.1 ) 
				currentSpeed.y *= decelerationFactor;
			else
				currentSpeed.y = 0;
		}

		// Move camera or stop it entirely
		if ( currentSpeed.x == 0 && currentSpeed.y == 0 )
			panning = false;
		else if ( allStop ) // if no movement inputs slow to stop.
			MoveCameraAmount( currentSpeed );

	}

	public void MoveCameraAmount ( Vector2 i_amount)
	{
		float currentX = gameObject.camera.transform.position.x;
		float currentZ = gameObject.camera.transform.position.z;
		float width = ((float)((HexGrid.Manager.rowSize-1)/2) * 3 + 0.5f);
		float height = (((float)(HexGrid.Manager.gridSize/HexGrid.Manager.rowSize)/2.0f+0.5f)*1.732f);
		
		
		if ( (currentX > -5 && i_amount.x > 0) || ((currentX < -(width-5)) && i_amount.x < 0) )
			i_amount.x = 0;
		
		if ( (currentZ < 5 && i_amount.y < 0) || ((currentZ > (height-5)) && i_amount.y > 0) )
			i_amount.y = 0;
		
		gameObject.camera.transform.position = new Vector3( currentX+(i_amount.x/cameraMoveScaleFactor), 120, currentZ+(i_amount.y/cameraMoveScaleFactor) );

		currentX = BackPlane.transform.position.x;
		currentZ = BackPlane.transform.position.z;

		BackPlane.transform.position = new Vector3( currentX + i_amount.x/(cameraMoveScaleFactor * 3.0f), 50, currentZ + i_amount.y/(cameraMoveScaleFactor * 3.0f) );
	}
	
	public void ReleaseCameraMove()
	{
		float width = ((float)((HexGrid.Manager.rowSize-1)/2) * 3 + 0.5f);
		float height = (((float)(HexGrid.Manager.gridSize/HexGrid.Manager.rowSize)/2.0f+0.5f)*1.732f);
		
		bool xAxis=false, xPositive=false, zAxis=false, zPositive=false;
		
		if ( gameObject.camera.transform.position.x > -5 )
		{
			xAxis = true;
			xPositive = false;
		}
		else if ( gameObject.camera.transform.position.x < -(width-5) )
		{
			xAxis = true;
			xPositive = true;
		}
		
		if ( gameObject.camera.transform.position.z < 5 )
		{
			zAxis = true;
			zPositive = true;
		}
		else if ( gameObject.camera.transform.position.z > height-5 )
		{
			zAxis = true;
			zPositive = false;
		}

		StartPanAnimation( xAxis, xPositive, zAxis, zPositive, width, height);
	}
	
	
	private void StartPanAnimation( bool xAxis, bool xPositive, bool zAxis, bool zPositive, float i_width, float i_height )
	{
		float targetX = (xAxis)? ((xPositive)? -(i_width-5.0f) : -5.0f) : gameObject.camera.transform.position.x;
		float targetZ = (zAxis)? ((zPositive)? 5.0f : (i_height-5.0f)) : gameObject.camera.transform.position.z;
		
		StartPos = gameObject.camera.transform.position;
		EndPos = new Vector3( targetX, 120, targetZ);
		
		panning = true;
		Action timerEndAction = delegate(){EndPanAnimation();};
		myPanTimer = gameObject.AddComponent<GenericTimer>();
		myPanTimer.Set( zoomDurration, false, timerEndAction );
		myPanTimer.Run();
	}
	
	
	private void PanAnimation()
	{
		float x = Mathf.Lerp( StartPos.x, EndPos.x, myPanTimer.PercentCompleteSmooth() );
		float z = Mathf.Lerp( StartPos.z, EndPos.z, myPanTimer.PercentCompleteSmooth() );
		gameObject.camera.transform.position = new Vector3(x, 120, z);
	}
	
	
	public void EndPanAnimation()
	{
		panning = false;
		gameObject.camera.transform.position = EndPos;
	}
	
	
	private void StartAnimation(bool i_in)
	{
		zooming = true;
		Action timerEndAction = delegate(){EndAnimation(i_in);};
		myZoomTimer = gameObject.AddComponent<GenericTimer>();
		myZoomTimer.Set( zoomDurration, false, timerEndAction );
		myZoomTimer.Run();
	}
	
	
	private void ZoomAnimation()
	{
		float x = Mathf.Lerp( StartPos.x, EndPos.x, myZoomTimer.PercentCompleteSmooth() );
		float z = Mathf.Lerp( StartPos.z, EndPos.z, myZoomTimer.PercentCompleteSmooth() );
		gameObject.camera.transform.position = new Vector3(x, 120, z);
		gameObject.camera.orthographicSize = Mathf.Lerp(StartSize, EndSize, myZoomTimer.PercentCompleteSmooth());
	}
	
	
	public void EndAnimation(bool i_in)
	{
		zooming = false;
		
		if ( i_in )
		{
			gameObject.camera.orthographicSize = _standardCameraSize;
			gameObject.camera.transform.position = EndPos;
			zoomBlock = false;
		}
		else
		{
			gameObject.camera.orthographicSize = EndSize;
			gameObject.camera.transform.position = EndPos;
			zoomBlock = true;
		}
	}

	public void InitializeBackPlane()
	{
		float minWidth = 4.7f, minHeight = 2.6f;
		float width, height;

		if( minWidth > HexGrid.Manager.GetLevelWidth() * 2.0f/10.0f )
			width = minWidth;
		else
			width = HexGrid.Manager.GetLevelWidth() * 2.0f/10.0f;

		if( minHeight > HexGrid.Manager.GetLevelHeight() * 2.0f/10.0f )
			height = minHeight;
		else
			height = HexGrid.Manager.GetLevelHeight() * 2.0f/10.0f;

		BackPlane.transform.position = HexGrid.Manager.GetMapCenter();
		BackPlane.transform.localScale = new Vector3(width, 1, height);
	}
}
