using UnityEngine;
using System.Collections;

public class HackerGUI : MonoBehaviour {
	
	private int sWidth;
	private int sHeight;
	private HackerActions myActionsScript;
	private float screenEdgeBuffer;
	private float alertWidth;
	private float alertHeight;
	
	private Vector3 PingButtonPosition;
	private Vector3 ZoomButtonPosition;
	
	public float GUIVirtualJoystickX;
	public float GUIVirtualJoystickY;
	public float GUIVirtualJoystickWidth;
	public float GUIVirtualJoystickHeight;


	public GUIStyle ThreatFrame;
	public GUIStyle ThreatBkg;
	//public GUIStyle PowerBkg;
	public GUIStyle TopButtonBar;
	public GUIStyle Button_Zoom;
	public GUIStyle Button_Ping;
	public GUIStyle Button_Warning;
	public GUIStyle Button_PingActive;
	public GUIStyle Button_ZoomActive;
	public GUIStyle Button_Esc;
	public GUIStyle Button_EscActive;
	public GUIStyle Button_VPAD;
	
	public GUIStyle hackerThreatAlertBackground;
	public GUIStyle hackerThreatAlertForeground;
	
	public Texture2D blue;
	public Texture2D red;
	private Texture2D standardPointer;
	private Texture2D fixedPointer;
	private Texture2D AlertFill_Texture;
	//private Texture2D powerBKg;
	private Texture2D powerFrame;
	private Texture2D powerFrame_Warning;
	private Texture2D powerBkg_Blue;
	private Texture2D powerBkg_Yellow;
	private Texture2D powerBkg_Red;
	private Texture2D powerSegment;
	private Texture2D threatFrameTex;
	private Texture2D escButtonTex;
	private Texture2D VPADTex;
	private Texture2D VPADOutlineTex;


	private Texture2D _goneDarkBKG;
	private Texture2D _goneDarkBTN_active;
	private Texture2D _goneDarkBTN_normal;

	private Texture2D _legend;

	private float Power_Blue_startX = 4.9f;
	private float Power_Yellow_startX = 2.7f;
	private float Power_Red_startX = 0.0f;
	private float Power_Blue_width = 9.0f;
	private float Power_Yellow_width = 2.3f;
	private float Power_Red_width = 2.7f;
	private float Power_Segment_width = 0.1f;
	public bool _showESC = false;
	private bool _slideInESC = true;
	private bool blink = false;
	private float nextBlink = 0.0f;
	private float blinkRate = 0.25f;
	private ESCMenu _escMenu = new ESCMenu();
	
	private bool _showRewardMenu =	false;
	private RewardMenu _rewardMenu = new RewardMenu(true);
	private GameObject hackerTopCamera;
	
	public GUIStyle Cube1;
	public GUIStyle AlertFill;
	// Use this for initialization
	void Start () 
	{
		hackerTopCamera = GameObject.Find("TopDownCamera");
		sWidth = Screen.width;
		sHeight = Screen.height;
		myActionsScript = gameObject.GetComponent<HackerActions>();

		alertWidth = sWidth/20;
		alertHeight = sWidth/20;
		//Initializing
		PingButtonPosition = new Vector3(0,0,0);
		ZoomButtonPosition = new Vector3(0,0,0);
		
		screenEdgeBuffer = 60.0f;
		
		standardPointer = Resources.Load("Textures/HackerGUI/alert_pointer", typeof(Texture2D)) as Texture2D;
		AlertFill_Texture = Resources.Load("Textures/HackerGUI/alert_button", typeof(Texture2D)) as Texture2D;
		powerFrame = Resources.Load("Textures/HackerGUI/powerMeter_frame", typeof(Texture2D)) as Texture2D;
		powerFrame_Warning = Resources.Load("Textures/HackerGUI/PowerFrame_Red", typeof(Texture2D)) as Texture2D;
		powerBkg_Blue = Resources.Load("Textures/HackerGUI/power_blue", typeof(Texture2D)) as Texture2D;
		powerBkg_Yellow = Resources.Load("Textures/HackerGUI/power_Yellow", typeof(Texture2D)) as Texture2D;
		powerBkg_Red = Resources.Load("Textures/HackerGUI/power_red", typeof(Texture2D)) as Texture2D;
		powerSegment = Resources.Load("Textures/HackerGUI/power_separator", typeof(Texture2D)) as Texture2D;
		threatFrameTex = Resources.Load ("Textures/HackerGUI/hackerThreatBackground", typeof(Texture2D)) as Texture2D;
		VPADTex = Resources.Load ("Textures/HackerGUI/JoystickThumb", typeof(Texture2D)) as Texture2D;
		VPADOutlineTex = Resources.Load ("Textures/HackerGUI/blueRing", typeof(Texture2D)) as Texture2D;
		_goneDarkBKG = Resources.Load ("Textures/GoneDark/GoneDark_01", typeof(Texture2D)) as Texture2D;
		_goneDarkBTN_active = Resources.Load ("Textures/GoneDark/Abort_Mission_btnActive", typeof(Texture2D)) as Texture2D;
		_goneDarkBTN_normal = Resources.Load ("Textures/GoneDark/Abort_Mission_btn", typeof(Texture2D)) as Texture2D;
		_legend = Resources.Load ("Textures/hackerLedgend", typeof(Texture2D)) as Texture2D;
		_escMenu.LoadESCMenuContent();
		_rewardMenu.LoadRewardMenuContent();
	}
	
	// Update is called once per frame
	void Update () {
		if((Input.GetKeyDown(KeyCode.Escape)) && (!HackerManager.Manager.gameEnded))
		{
			NetworkManager.Manager.ToggleESCMenu();
			//ToggleESCMenu();
		}
		
		if(_showRewardMenu)
		{
			_rewardMenu.Display();
		}

		if(_showESC)
		{
			_escMenu.Update(Time.deltaTime);
		}
	}

	public void ShutDownESCMenu()
	{
		_showESC = false;
		if(GameManager.Manager.PlayerType == 1)
		{
			//GameObject.Find("Playertheif(Clone)").GetComponent<MouseLookAround>().enabled = true;
			ThiefManager.Manager.EnableThiefActions();
		}
		if(GameManager.Manager.PlayerType == 2)
		{
			//hackerTopCamera.GetComponent<HackerActions>().EnableHackerActions();
			HackerManager.Manager.EnableHackerActions();
		}
	}
	
	public void ShowEndGameRewardMenu()
	{
		_showRewardMenu = true;
	}
	
	public void ToggleESCMenu()
	{
		if( GameManager.Manager._currentInfoData.m_currentMovie == "" )
		{
			if(_showESC == false)
			{
				_showESC = true;
				_slideInESC = true;
			}
			else
			{
				_slideInESC = false;
			}

			if (_showESC)
			{
				_escMenu.ResetTicker(_slideInESC);
				if(GameManager.Manager.PlayerType == 1)
				{
					//GameObject.Find("Playertheif(Clone)").GetComponent<MouseLookAround>().enabled = false;
					ThiefManager.Manager.DisableThiefActions();
				}
				if(GameManager.Manager.PlayerType == 2)
				{
					//hackerTopCamera.GetComponent<HackerActions>().DisableHackerActions();
					HackerManager.Manager.DisableHackerActions();
				}
			}		
		}
	}
	
	void OnGUI() 
	{
		if ( GameManager.Manager.PlayerType == 2 ) // Only the hacker can see this
		{
			GUI.depth = 2;
			RenderLegend();

		 	RenderFrameBkg();
			//RenderPower();
			//RenderZoom();
			//RenderPing();
			
			//RenderAlert();
			//RenderFrameTop();
			//if(	gameObject.GetComponent<ThiefActions>().IsCaught == true)
			//{
			//	HackerMessage();
			//}
			//RenderTracerIndicators();
			RenderOverrideIndicator();

#if UNITY_IPHONE || UNITY_ANDROID
			RenderEscButton();
#endif

			if (VirtualKeyboard.isWindowsTablet)
			{
					RenderEscButton();
			}

		}
		GUI.depth = 1;
		RenderESCScreen();
		CheckConnectionStatus();
		//RenderHackerThreatLevel(); 
	}
	
	private void CheckConnectionStatus()
	{
		if(Network.connections.Length == 0)
		{
			GUI.depth = -1;
			ScreenHelper.DrawTexture(0, 0, 64, 36, _goneDarkBKG);
			if(ScreenHelper.DrawButton(24, 28, 16, 2, _goneDarkBTN_active, _goneDarkBTN_normal))
			{
				NetworkManager.Manager.GoBackToLoginScreenRPC();
			}
		}
	}
	private void RenderESCScreen()
	{
		if(_showESC)
		{
			_escMenu.show();
		}
	}

	private void RenderLegend()
	{
		if ( HackerManager.Manager.legendOpen )
			GUI.DrawTexture( new Rect(0,0,Screen.width/4,Screen.height), _legend);
		else
			GUI.DrawTexture( new Rect(-(Screen.width/4.6f),0,Screen.width/4,Screen.height), _legend);
	}


	private void RenderOverrideIndicator()
	{
		// Position on Screen
		if ( !OverrideManager.Manager.IsOverrideOnScreen )
		{
			Cube1.normal.background = standardPointer;
			// Determine Pos
			float currentAngle = OverrideManager.Manager.OverrideAngle;
			float sideA = Screen.width/2-screenEdgeBuffer;
			float sideB = Screen.height/2-screenEdgeBuffer;
			float angleRadians = currentAngle*Mathf.Deg2Rad;
			//float armLength = (sideA*sideB) / Mathf.Sqrt((Mathf.Pow((sideB * Mathf.Cos(angleRadians)), 2)) + (Mathf.Pow((sideA * Mathf.Sin(angleRadians)), 2))) ;
			float xPos = sideA * Mathf.Cos(angleRadians)+sideA + screenEdgeBuffer;
			float yPos = Screen.height-(sideB * Mathf.Sin(angleRadians)+sideB) - screenEdgeBuffer;			
			
			GUIUtility.RotateAroundPivot( -currentAngle, new Vector2( xPos, yPos) );
			
			Rect meter = new Rect ( xPos-alertWidth/2, yPos-alertHeight/2, alertWidth, alertHeight );
			
			GUI.Box ( meter, GUIContent.none, Cube1 );
			
			GUIUtility.RotateAroundPivot( currentAngle,  new Vector2( xPos, yPos) );
			Rect meterFill = new Rect ( xPos-alertWidth/2+0.5f, yPos-alertHeight/2+0.5f, alertWidth, alertHeight );	
			
			if(GUI.Button ( meterFill, GUIContent.none, AlertFill ))
			{
				TopDown_Camera.Manager.ZoomIn(OverrideManager.Manager.GetOverrideNode().transform.position);				
			}
		}		
	}
	
	private void RenderTracerIndicators()
	{
		if( SecurityManager.Manager.GetLiveTracers() != null )
		for(int i = 0; i < SecurityManager.Manager.GetLiveTracers().Count; i ++)
		{
			Tracer currentTracer = SecurityManager.Manager.GetLiveTracers()[i].GetComponent<Tracer>();
			if(currentTracer.OnToHacker)
			{
				ShowTracerIndicator(currentTracer);
			}
		}
	}
	
	private void ShowTracerIndicator(Tracer i_tracer)
	{
		//Time.deltaTime
		i_tracer.CalcPosition();
				// Calculate Fill material
		//float warningLevel = i_tracer.CalibratePercentage;
		//if ( warningLevel > 0.80f )
		AlertFill.normal.background = AlertFill_Texture;
		
		// Position on Screen
		if ( !i_tracer.OnScreen )
		{
			Cube1.normal.background = standardPointer;
			// Determine Pos
			float currentAngle = i_tracer.DisplayAngle;
			float sideA = Screen.width/2-screenEdgeBuffer;
			float sideB = Screen.height/2-screenEdgeBuffer;
			float angleRadians = currentAngle*Mathf.Deg2Rad;
			//float armLength = (sideA*sideB) / Mathf.Sqrt((Mathf.Pow((sideB * Mathf.Cos(angleRadians)), 2)) + (Mathf.Pow((sideA * Mathf.Sin(angleRadians)), 2))) ;
			float xPos = sideA * Mathf.Cos(angleRadians)+sideA + screenEdgeBuffer;
			float yPos = Screen.height-(sideB * Mathf.Sin(angleRadians)+sideB) - screenEdgeBuffer;			
			
			GUIUtility.RotateAroundPivot( -currentAngle, new Vector2( xPos, yPos) );
			
			Rect meter = new Rect ( xPos-alertWidth/2, yPos-alertHeight/2, alertWidth, alertHeight );
			GUI.Box ( meter, GUIContent.none, Cube1 );

			GUIUtility.RotateAroundPivot( currentAngle,  new Vector2( xPos, yPos) );
			Rect meterFill = new Rect ( xPos-alertWidth/2+0.5f, yPos-alertHeight/2+0.5f, alertWidth, alertHeight );
			if(GUI.Button ( meterFill, GUIContent.none, AlertFill ))
			{
				TopDown_Camera.Manager.ZoomIn(i_tracer.gameObject.transform.position);				
				
				// Tracer indicator button was pressed [SOUND TAG] [Tracer_Indicator_Press]
				soundMan.soundMgr.playOneShotOnSource(null,"Tracer_Indicator_Press",GameManager.Manager.PlayerType,2);
			}
		}
	}
	
	private float PosW(float i_increment)
	{
		return (Screen.width/64)*i_increment;
	}
	
	private float PosH(float i_increment)
	{
		return (Screen.height/36)*i_increment;
	}
	
	
	private void RenderFrameBkg()
	{
		//GUI.Box(new Rect(PosW(0),PosH(1),PosW(32),PosH(2)), GUIContent.none, ThreatBkg);
		//GUI.Box(new Rect(PosW(50),PosH(1),PosW(8),PosH(2)), GUIContent.none, TopButtonBar);
		GUI.DrawTexture( new Rect(Screen.width/6,0,Screen.width/1.5f,Screen.height/7), threatFrameTex);
	}
	
	private void RenderFrameTop()
	{
		GUI.Box(new Rect(PosW(0),PosH(1),PosW(32),PosH(2)), GUIContent.none, ThreatFrame);
	}
	
	public void RenderZoom()
	{
		Rect zoomPos = new Rect ( PosW(54), PosH(1), PosW(8), PosH (4) );
		TopDown_Camera script = (TopDown_Camera) gameObject.GetComponent<TopDown_Camera>();
		
		if ( script.zoomedIn )
		{
			if (GUI.Button (zoomPos, GUIContent.none, Button_Zoom) )
			{ 
				if ( !script.Zooming )
				{
					script.ZoomOut();
				}
			}
		}
		else
		{ 
			if (GUI.Button (zoomPos, GUIContent.none, Button_ZoomActive) )
			{
				if ( !script.Zooming )
				{
					script.ZoomIn( new Vector3(0.0f, 0.0f, 0.0f));
				}
			}

		}
	}

	
	private void RenderPing()
	{
		Rect pingPos = new Rect ( PosW(46), PosH(1), PosW(8), PosH (2) );
		
		if ( myActionsScript.PingActive )
		{
			if (GUI.Button (pingPos, GUIContent.none, Button_PingActive) )
			{
				myActionsScript.PingToggleClicked();
			}
		}
		else
		{
			if (GUI.Button (pingPos, GUIContent.none, Button_Ping) )
			{
				myActionsScript.PingToggleClicked();
			}
		}
	}
	private void RenderEscButton()
	{
		Rect escButtonPos = new Rect ( PosW(1), PosH(1), PosW(6), PosH (6) );
		
		if ( _showESC )
		{
			if (GUI.Button (escButtonPos, GUIContent.none, Button_EscActive) )
			{
				NetworkManager.Manager.ToggleESCMenu();
			}
		}
		else
		{	
			if (VirtualKeyboard.isWindowsTablet)
			{
			//RenderGUIMoveButtons();
			RenderGUIVirtualJoystick();

			}
			if (GUI.Button (escButtonPos, GUIContent.none, Button_Esc) )
			{
				NetworkManager.Manager.ToggleESCMenu();
			}
		}

	}

	private void RenderGUIMoveButtons()
	{
		if (GUI.RepeatButton (new Rect(PosW(7), PosH(28), PosW(4), PosH (2)), "u"))
		{
			myActionsScript.VPAD_Up =true;
		}
		if (GUI.RepeatButton (new Rect(PosW(7), PosH(34), PosW(4), PosH (2)), "d"))
		{
			myActionsScript.VPAD_Down =true;
		}
		if (GUI.RepeatButton (new Rect(PosW(5), PosH(30), PosW(2), PosH (4)), "l"))
		{
			myActionsScript.VPAD_Left =true;
		}
		if (GUI.RepeatButton (new Rect(PosW(11), PosH(30), PosW(2), PosH (4)), "r"))
		{
			myActionsScript.VPAD_Right =true;
		}
		//Debug.Log("GUI VPAD_Up - " + myActionsScript.VPAD_Up);
	}


	private void RenderGUIVirtualJoystick()
	{	
		if (GUI.RepeatButton (new Rect(PosW(GUIVirtualJoystickX), PosH(GUIVirtualJoystickY), PosW(GUIVirtualJoystickWidth), PosH (GUIVirtualJoystickHeight)),GUIContent.none,Button_VPAD))
		{	
			myActionsScript.VPAD_KeyMove = true;
			//Debug.Log("myActionsScript.VPAD_KeyMove = true;");
			/*
			Vector2 directionVector = Event.current.mousePosition - new Vector2 ((PosW(9)),PosH(24));
			directionVector.Normalize();
			directionVector = new Vector2(- directionVector.x, directionVector.y);
			//Debug.Log ("Event.current.mousePosition - " + Event.current.mousePosition.x + " ' " + Event.current.mousePosition.y + " (PosW(9) - " + PosW(9) + "PosH(24) - " + PosH(24) + "directionVector - " + directionVector);
			gameObject.GetComponent<TopDown_Camera>().MoveCameraAmount(directionVector * Time.deltaTime * 400);
			*/
		}
		if (myActionsScript.VPAD_KeyMove == false)
		{
			//GUI.DrawTexture(new Rect(PosW (8) - PosW (2), PosH (32) - PosH (2),PosW (4), PosH (4)),VPADTex);
			GUI.DrawTexture(new Rect(PosW (GUIVirtualJoystickX + (GUIVirtualJoystickWidth/2)) - PosW (GUIVirtualJoystickWidth/4), 
			                         PosH (GUIVirtualJoystickY + (GUIVirtualJoystickHeight/2)) - PosH (GUIVirtualJoystickHeight/4),
			                         PosW (GUIVirtualJoystickWidth/2),
			                         PosH (GUIVirtualJoystickHeight/2)),
			                		 VPADTex);
		}
		else
		{
			//if (Vector2.Distance(Event.current.mousePosition,new Vector2 ((PosW(8)),PosH(32))) > PosW (2))
			if (Vector2.Distance(Event.current.mousePosition,new Vector2 ( PosW (GUIVirtualJoystickX + (GUIVirtualJoystickWidth/2) ),
			                                                              PosH (GUIVirtualJoystickY + (GUIVirtualJoystickHeight/2)) ))
			                     > PosW (GUIVirtualJoystickWidth/4))
			    
		    {
			//Vector2 directionVector = Event.current.mousePosition - new Vector2 ((PosW(8)),PosH(32));
				Vector2 directionVector = Event.current.mousePosition - new Vector2 (PosW (GUIVirtualJoystickX + (GUIVirtualJoystickWidth/2)),
				                                                                     PosH (GUIVirtualJoystickY + (GUIVirtualJoystickHeight/2)));

				directionVector.Normalize();
				//Vector2 VPADLocation = new Vector2 ((PosW(8)),PosH(32)) + ( directionVector * PosW (2));
				Vector2 VPADLocation = new Vector2 (PosW (GUIVirtualJoystickX + (GUIVirtualJoystickWidth/2)),
				                                    PosH (GUIVirtualJoystickY + (GUIVirtualJoystickHeight/2))) 
												+ ( directionVector * PosW (GUIVirtualJoystickWidth/4));

				//GUI.DrawTexture(new Rect(VPADLocation.x - PosW (2) ,VPADLocation.y - PosH (2),PosW (4), PosH (4)) ,VPADTex);
				GUI.DrawTexture(new Rect(VPADLocation.x - PosW (GUIVirtualJoystickWidth/4) ,
				                         VPADLocation.y - PosH (GUIVirtualJoystickHeight/4),
				                         PosW (GUIVirtualJoystickWidth/2), 
				                         PosH (GUIVirtualJoystickHeight/2)) ,
				                         VPADTex);
			}
			else
			{
				//GUI.DrawTexture(new Rect(Event.current.mousePosition.x - PosW (2), Event.current.mousePosition.y - PosH (2),PosW (4), PosH (4)),VPADTex);
				GUI.DrawTexture(new Rect(Event.current.mousePosition.x - PosW (GUIVirtualJoystickWidth/4),
				                         Event.current.mousePosition.y - PosH (GUIVirtualJoystickHeight/4),
				                         PosW (GUIVirtualJoystickWidth/2),
				                         PosH (GUIVirtualJoystickHeight/2)),
				                         VPADTex);
			}
		}
	}
	
	private void RenderAlert()
	{
		// Warning Button
		if (GUI.Button (new Rect (((sWidth/64.0f)*55.0f),0.0f, (sWidth/8.0f), (Screen.height/6.0f)), GUIContent.none, Button_Warning) )
		{
			// Go to alert if active
		}
	}
	
	private void RenderPower()
	{
		float pUsage = (float) HackerManager.Manager.powerUsage;
		float pCapacity = (float) HackerManager.Manager.powerCapacity*25;
		float BlueThreadhold = pCapacity - 50f;
		float YellowThreadhold = pCapacity - 25f;
		int pSegmentgs = HackerManager.Manager.powerCapacity - 2;
		
		if(pUsage <= BlueThreadhold)
		{
			ScreenHelper.DrawPowerMeterTexture(14, Power_Blue_startX + Power_Blue_width * (1 - pUsage/BlueThreadhold), 1, Power_Blue_width * pUsage/BlueThreadhold, 1, powerBkg_Blue);
			NetworkManager.Manager.SetPowerMultiplierForThreatLevel( 1.0f );
		}
		else if(pUsage > BlueThreadhold && pUsage <= YellowThreadhold)
		{
			ScreenHelper.DrawPowerMeterTexture(14, Power_Blue_startX, 1, Power_Blue_width, 1, powerBkg_Blue);
			float ratio = (pUsage - BlueThreadhold)/25f;
			ScreenHelper.DrawPowerMeterTexture(14, Power_Yellow_startX + Power_Yellow_width * (1 - ratio), 1, Power_Yellow_width *  ratio, 1, powerBkg_Yellow);
			//Yellow rate
			NetworkManager.Manager.SetPowerMultiplierForThreatLevel( 2.0f );
		}
		
		else if(pUsage > YellowThreadhold && pUsage <= pCapacity)
		{
			ScreenHelper.DrawPowerMeterTexture(14, Power_Blue_startX, 1, Power_Blue_width, 1, powerBkg_Blue);
			ScreenHelper.DrawPowerMeterTexture(14, Power_Yellow_startX, 1, Power_Yellow_width, 1, powerBkg_Yellow);
			float ratio = (pUsage - YellowThreadhold)/25f;
			ScreenHelper.DrawPowerMeterTexture(14, Power_Red_startX + Power_Red_width * (1 - ratio), 1, Power_Red_width * ratio, 1, powerBkg_Red);
			//Red rate
			NetworkManager.Manager.SetPowerMultiplierForThreatLevel( 4.0f );
		}
		else if(pUsage > pCapacity)
		{
			ScreenHelper.DrawPowerMeterTexture(14, Power_Blue_startX, 1, Power_Blue_width, 1, powerBkg_Blue);
			ScreenHelper.DrawPowerMeterTexture(14, Power_Yellow_startX, 1, Power_Yellow_width, 1, powerBkg_Yellow);
			ScreenHelper.DrawPowerMeterTexture(14, Power_Red_startX, 1, Power_Red_width, 1, powerBkg_Red);	
		}

		for(int i = 1; i < pSegmentgs; i++)
		{
			ScreenHelper.DrawPowerMeterTexture(14, Power_Blue_startX + i * Power_Blue_width/pSegmentgs, 1, Power_Segment_width, 1, powerSegment);
		}
		ScreenHelper.DrawPowerMeterTexture(14, 0, 1, 14, 2, powerFrame);
		//GUI.Box(new Rect(PosW(50),PosH(1),PosW(16),PosH(2)), GUIContent.none, PowerBkg);
		
		//Blink mode hacky
		if( pUsage > BlueThreadhold )
		{
			if( nextBlink == 0.0f )
				nextBlink = Time.time + blinkRate;
			
			if( Time.time > nextBlink )
			{
				nextBlink = Time.time + blinkRate;
				blink = blink ? false : true;
			} 
		}
		else
		{
			nextBlink = 0.0f;
			blink = false;
		}
		
		if( blink )  
		{
			ScreenHelper.DrawPowerMeterTexture(14, 0, 1, 14, 2, powerFrame_Warning);
		}
		
		//GUI.Box ( new Rect ( PosW (55.5f), PosH (1), fillWidth, PosH (1) ), GUIContent.none, hackerThreatAlertForeground );
		
		//GUI.Box(new Rect(PosW(50),PosH(1),PosW(16),PosH(2)), GUIContent.none, PowerBkg);

		
	}
	
	private void RenderThreat()
	{
		//ThreatAnimation script = (ThreatAnimation) gameObject.GetComponent("ThreatAnimation");
		//script.Draw();
	}
	
	private void HackerMessage()
		
	{
		//gameObject.GetComponent<MessageBox>().showGUI =true;
		//gameObject.GetComponent<MessageBox>().GUItype=1;
		//gameObject.GetComponent<MessageBox>().RenderMessageBox();
	}
	

}
