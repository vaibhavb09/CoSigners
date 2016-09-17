using UnityEngine;
using System.Collections;

public class ThiefGUI : MonoBehaviour {
	
	public GUIStyle Cube1;
	public GUIStyle AlertFill;
	private float sWidth;
	private float sHeight;
	private float alertWidth;
	private float alertHeight;
	private float screenEdgeBuffer;
	private float radiusA; 
	private float radiusB;
	private bool alertMeterOnScreen;
	private float lerpynessFactor;
	private float radiusAForPing;
	private float radiusBForPing;

	private Texture2D standardPointer;
	private Texture2D fixedPointer;
	private Texture2D AlertFill_1;
	private Texture2D AlertFill_2;
	private Texture2D AlertFill_3;
	private Texture2D AlertFill_4;
	private Texture2D AlertFill_5;
	private Texture2D Ping_Button;
	private Texture2D Ping_Pointer;
	private Texture2D TransmitterCounter;
	
	private Texture CrossHair;
	private Texture Cursor_Door;
	private Texture Cursor_Override;
	private Texture Cursor_InfoNode;
	private Texture Cursor_SAP;
	private Texture Cursor_Activate;
	private Texture Cursor_Transmitter;
	private Texture CurrentCursor;
	private Texture CurveBarTopTex, CurveBarTex, CompassBaseTex, CompassFrontTex, PowerPodTex;
	
	private Rect crosshairRect;
	private Rect meterFill;

	private Rect curveBarTop, curveBar3, curveBar4;
	private Rect compassBase, compassFront, powerPodsFrame;

	public bool transmitterPlaced;
	public float transmitterPlacedTime;
	public Vector3 transmitterPosScreenSpace;

	// Use this for initialization
	void Start () {
		sWidth = Screen.width;
		sHeight = Screen.height;
		alertWidth = sWidth/20;
		alertHeight = sWidth/20;
		radiusA= 200.0f;
		radiusB= 250.0f;
		alertMeterOnScreen=true;
		lerpynessFactor=1.0f;

		radiusAForPing=300.0f;
		radiusBForPing=350.0f;

		//alertWidth = sWidth/40;
		//alertHeight = sWidth/10;
		screenEdgeBuffer = 60.0f;

		standardPointer = Resources.Load("Textures/Alert_Indicator_01", typeof(Texture2D)) as Texture2D;
		fixedPointer = Resources.Load("Textures/Alert_Indicator_02", typeof(Texture2D)) as Texture2D;
		AlertFill_1 = Resources.Load("Textures/AlertMeter/Alert_1", typeof(Texture2D)) as Texture2D;
		AlertFill_2 = Resources.Load("Textures/AlertMeter/Alert_2", typeof(Texture2D)) as Texture2D;
		AlertFill_3 = Resources.Load("Textures/AlertMeter/Alert_3", typeof(Texture2D)) as Texture2D;
		AlertFill_4 = Resources.Load("Textures/AlertMeter/Alert_4", typeof(Texture2D)) as Texture2D;
		AlertFill_5 = Resources.Load("Textures/AlertMeter/Alert_5", typeof(Texture2D)) as Texture2D;
		CrossHair 			= Resources.Load("Textures/Cursors/Crosshair", typeof(Texture)) as Texture;
		Cursor_Door 		= Resources.Load("Textures/Cursors/Cursor_Door", typeof(Texture)) as Texture;
		Cursor_Override 	= Resources.Load("Textures/Cursors/Cursor_Override", typeof(Texture)) as Texture;
		Cursor_InfoNode 	= Resources.Load("Textures/Cursors/Cursor_Override", typeof(Texture)) as Texture; //Change to InfoNode when texture is available.
		Cursor_SAP 			= Resources.Load("Textures/Cursors/Cursor_SAP", typeof(Texture)) as Texture;
		Cursor_Activate 	= Resources.Load("Textures/Cursors/Cursor_Password", typeof(Texture)) as Texture;
		Cursor_Transmitter 	= Resources.Load("Textures/Cursors/Cursor_Transmitter", typeof(Texture)) as Texture;
		
		Ping_Button = Resources.Load("Textures/HackerGUI/thiefPing_offScreen", typeof(Texture2D)) as Texture2D;
		Ping_Pointer = Resources.Load("Textures/HackerGUI/thiefPing_pointer", typeof(Texture2D)) as Texture2D;
		TransmitterCounter = Resources.Load("Textures/TransmitterCounter", typeof(Texture2D)) as Texture2D;

		CurveBarTopTex = Resources.Load("Textures/ThiefHUD/Top_Curve_Bar", typeof(Texture2D)) as Texture2D;
		CurveBarTex = Resources.Load("Textures/ThiefHUD/CurveBar_02", typeof(Texture2D)) as Texture2D;
		CompassBaseTex = Resources.Load("Textures/ThiefHUD/CompassBase_01", typeof(Texture2D)) as Texture2D;
		CompassFrontTex = Resources.Load("Textures/ThiefHUD/CompassInner", typeof(Texture2D)) as Texture2D;
		PowerPodTex = Resources.Load("Textures/ThiefHUD/PowerPods", typeof(Texture2D)) as Texture2D;

		CurrentCursor = CrossHair;
		float crossHairSize = Screen.width * 0.05f;
		crosshairRect = new Rect( Screen.width/2 - crossHairSize/2.0f, Screen.height/2 - crossHairSize/2, crossHairSize, crossHairSize );
		curveBarTop = new Rect( 0, 			Screen.height/40, 		Screen.width, 	Screen.height/7.97f);
		curveBar3 = new Rect( 0, 			Screen.height/20*19, 	Screen.width/2, 	-Screen.height/13.5f );
		curveBar4 = new Rect( Screen.width, Screen.height/20*19, 	-Screen.width/2, 	-Screen.height/13.5f );
		compassBase = new Rect( 0, Screen.height/20*15, 	Screen.width/7.5f, 	Screen.height/4.2f );
		powerPodsFrame = new Rect( Screen.width/15*13.2f, Screen.height/24*17.3f, 	Screen.width/15*2, 	Screen.height/4.2f );

		transmitterPlaced = false;
		transmitterPlacedTime = 0.0f;
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
	
	void OnGUI()
	{
		GUI.depth = 2;
		if ( GameManager.Manager.PlayerType == 1 ) // Only the thief can see this
		{
			for ( int i=0 ; i<GuardOverlord.Manager.m_Guards.Count ; i++ )
			{
				//AlertSystem alertScript = GuardOverlord.Manager.m_Guards[i].GetComponent<AlertSystem>();
				if ( GuardOverlord.Manager.m_Guards[i].GetComponent<Perception>().isGuardAlert )
				{
					ShowAlertMeter(i);
				}
			}
			
			OffScreenPingIndicator.Manager.CalculatePingPosition();
			if(!OffScreenPingIndicator.Manager.InScreen)
			{
				ShowOffScreenCursor();
			}
			
			//Crosshair
			GUI.DrawTexture( crosshairRect, CurrentCursor );
			GUI.backgroundColor = Color.blue;
			//GUI.Button(new Rect(Screen.width / 10, Screen.height/48 ,150, 20),("Transmitters Left: " +ThiefManager.Manager.TransmitterCount));
			//GUI.Button(new Rect(meterFill.x + 60, meterFill.y + 30,150, 20),("Transmitters Left: " +ThiefManager.Manager.TransmitterCount));
			
			//Transmitter count
			//ScreenHelper.DrawTexture( 48, 31, 16, 4, TransmitterCounter );
			//ScreenHelper.DrawText( 58, 32, 5, 20, ""+ ThiefManager.Manager.GetTransmitterCount()+ "/"+ ThiefManager.Manager.maxTransmitterCount, 60, Color.white );
			
			//GUI.Label(new Rect(10, 120, 150, 20), "Transmitters Left: " + ThiefManager.Manager.GetTransmitterCount() );
			DrawHUD();

			//Transmitter Placement
			//DrawTransmitterText();
		}
	}

	/*
	public void DrawTransmitterText()
	{
		if( Time.time - transmitterPlacedTime < 1.0f )
		{
			transmitterPosScreenSpace = new Vector3( transmitterPosScreenSpace.x, transmitterPosScreenSpace.y - 0.5f, transmitterPosScreenSpace.z );

			ScreenHelper.StartScreenStyle.normal.background = null;
			ScreenHelper.StartScreenStyle.normal.textColor = Color.white;
			ScreenHelper.StartScreenStyle.fontSize = (int)(Screen.width * 0.018f);
			ScreenHelper.StartScreenStyle.font = Resources.Load("Fonts/Aller_Bd",typeof(Font)) as Font;
			ScreenHelper.StartScreenStyle.alignment = TextAnchor.MiddleCenter;

			GUI.Label( new Rect( transmitterPosScreenSpace.x,
			                    transmitterPosScreenSpace.y,
			                    Screen.width * 0.01f,
			                    Screen.height * 0.06f),
			          			ThiefManager.Manager.GetTransmitterCount().ToString(),
								ScreenHelper.StartScreenStyle );
		}
	}
	*/
	
	public void ChangeCursorToCrossHair()
	{
		CurrentCursor = CrossHair;
	}
	
	public void ChangeCursorToDoor()
	{
		CurrentCursor = Cursor_Door;
	}
	
	public void ChangeCursorToOverride()
	{
		CurrentCursor = Cursor_Override;
	}

	public void ChangeCursorToInfoNode()
	{
		CurrentCursor = Cursor_InfoNode;
	}
	
	public void ChangeCursorToActivate()
	{
		CurrentCursor = Cursor_Activate;
	}
	
	public void ChangeCursorToTransmitter()
	{
		CurrentCursor = Cursor_Transmitter;
	}
	
	public void ChangeCursorToSAP()
	{
		CurrentCursor = Cursor_SAP;
	}
	
	private void ShowOffScreenCursor()
	{
		// Determine Pos
		Cube1.normal.background = Ping_Pointer;
		AlertFill.normal.background = Ping_Button;
		
		float currentAngle = OffScreenPingIndicator.Manager.GetAngle();
		float sideA = Screen.width/2-screenEdgeBuffer;
		float sideB = Screen.height/2-screenEdgeBuffer;
		float angleRadians = currentAngle*Mathf.Deg2Rad;
		//float armLength = (sideA*sideB) / Mathf.Sqrt((Mathf.Pow((sideB * Mathf.Cos(angleRadians)), 2)) + (Mathf.Pow((sideA * Mathf.Sin(angleRadians)), 2))) ;
		//float xPos = sideA * Mathf.Cos(angleRadians)+sideA + screenEdgeBuffer;
		//float yPos = Screen.height-(sideB * Mathf.Sin(angleRadians)+sideB) - screenEdgeBuffer;			

		//alert meter in elliptical form
		float xPos = radiusAForPing* Mathf.Cos(-angleRadians) +(Screen.width/2);
		float yPos = radiusBForPing* Mathf.Sin(-angleRadians) +(Screen.height/2);

		//GUIUtility.RotateAroundPivot( -currentAngle, new Vector2( xPos, yPos) );
				
		//Rect meter = new Rect ( xPos-alertWidth/2, yPos-alertHeight/2, alertWidth, alertHeight );
		//GUI.Box ( meter, GUIContent.none, Cube1 );
		//GUIUtility.RotateAroundPivot( currentAngle, new Vector2( xPos, yPos) );
				
		GUIUtility.RotateAroundPivot( -currentAngle, new Vector2( xPos, yPos) );
			
		Rect meter = new Rect ( xPos-alertWidth/2, yPos-alertHeight/2, alertWidth, alertHeight );
		GUI.Box ( meter, GUIContent.none, Cube1 );

		GUIUtility.RotateAroundPivot( currentAngle,  new Vector2( xPos, yPos) );
		Rect meterFill = new Rect ( xPos-alertWidth/2+0.5f, yPos-alertHeight/2+0.5f, alertWidth, alertHeight );
		GUI.Box ( meterFill, GUIContent.none, AlertFill );


	}
	
	private void ShowAlertMeter(int i)
	{
		// Calculate Fill material
		float alertLevel = GuardOverlord.Manager.m_Guards[i].GetComponent<Perception>().AlertLevel;
		if ( alertLevel > 98.0f )
			AlertFill.normal.background = AlertFill_5;
		else if ( alertLevel > 70.0f )
			AlertFill.normal.background = AlertFill_4;
		else if ( alertLevel > 50.0f )
			AlertFill.normal.background = AlertFill_3;
		else if ( alertLevel > 25.0f )
			AlertFill.normal.background = AlertFill_2;
		else
			AlertFill.normal.background = AlertFill_1;
	

		// Position on Screen
		if ( GuardOverlord.Manager.m_Guards[i].GetComponent<AlertMeter>().onScreen )
		{
			if(lerpynessFactor<20.0f)
				lerpynessFactor+=0.5f;

			Cube1.normal.background = fixedPointer;
			GameObject playerCamera = (GameObject)GameObject.Find ("FPSCamera");
			Vector3 GuardPos = GuardOverlord.Manager.m_Guards[i].transform.position;
			Vector3 AboveGuard = new Vector3 ( GuardPos.x, GuardPos.y+1.3f, GuardPos.z);
			Vector3 screenPos = playerCamera.camera.WorldToScreenPoint( AboveGuard );

			if(!GuardOverlord.Manager.m_Guards[i].GetComponent<AlertMeter>().firstSpotted)
			{
				GuardOverlord.Manager.m_Guards[i].GetComponent<AlertMeter>().alertMeterPosX= screenPos.x;
				GuardOverlord.Manager.m_Guards[i].GetComponent<AlertMeter>().alertMeterPosY= Screen.height-screenPos.y;
				GuardOverlord.Manager.m_Guards[i].GetComponent<AlertMeter>().firstSpotted=true;
			}

			float lerpedX=screenPos.x;
			float lerpedY=Screen.height-screenPos.y;
		
			{
				lerpedX=Mathf.Lerp(GuardOverlord.Manager.m_Guards[i].GetComponent<AlertMeter>().alertMeterPosX,screenPos.x, lerpynessFactor*0.05f);
				lerpedY=Mathf.Lerp(GuardOverlord.Manager.m_Guards[i].GetComponent<AlertMeter>().alertMeterPosY, Screen.height-screenPos.y, lerpynessFactor*0.05f);
				
				GuardOverlord.Manager.m_Guards[i].GetComponent<AlertMeter>().alertMeterPosX=lerpedX;
				GuardOverlord.Manager.m_Guards[i].GetComponent<AlertMeter>().alertMeterPosY=lerpedY;
			}

			Rect meter = new Rect ( lerpedX-alertWidth/2 , lerpedY-alertHeight/2, alertWidth, alertHeight );
			GUI.Box ( meter, GUIContent.none, Cube1 );
			GUI.Box ( meter, GUIContent.none, AlertFill );
		}
		else
		{
			lerpynessFactor=1.0f;

			Cube1.normal.background = standardPointer;
			// Determine Pos
			float currentAngle = GuardOverlord.Manager.m_Guards[i].GetComponent<AlertMeter>().PlayerToGuardAngle;
			//float sideA = Screen.width/2-screenEdgeBuffer;
			//float sideB = Screen.height/2-screenEdgeBuffer;
			float angleRadians = currentAngle*Mathf.Deg2Rad;
			//float armLength = (sideA*sideB) / Mathf.Sqrt((Mathf.Pow((sideB * Mathf.Cos(angleRadians)), 2)) + (Mathf.Pow((sideA * Mathf.Sin(angleRadians)), 2))) ;

			//alert meter in elliptical form
			float xPos = radiusA* Mathf.Cos(-angleRadians) +(Screen.width/2);
			float yPos = radiusB* Mathf.Sin(-angleRadians) +(Screen.height/2);

			if(!GuardOverlord.Manager.m_Guards[i].GetComponent<AlertMeter>().firstSpotted)
			{
				GuardOverlord.Manager.m_Guards[i].GetComponent<AlertMeter>().alertMeterPosX= xPos;
				GuardOverlord.Manager.m_Guards[i].GetComponent<AlertMeter>().alertMeterPosY= yPos;
				GuardOverlord.Manager.m_Guards[i].GetComponent<AlertMeter>().firstSpotted=true;
			}
			//lerp and prevent snapping
			//if(alertMeterOnScreen)
			{
				float lerpedX=Mathf.Lerp(GuardOverlord.Manager.m_Guards[i].GetComponent<AlertMeter>().alertMeterPosX,xPos, lerpynessFactor* 0.05f);
				float lerpedY=Mathf.Lerp(GuardOverlord.Manager.m_Guards[i].GetComponent<AlertMeter>().alertMeterPosY,yPos, lerpynessFactor* 0.05f);

				GuardOverlord.Manager.m_Guards[i].GetComponent<AlertMeter>().alertMeterPosX=lerpedX;
				GuardOverlord.Manager.m_Guards[i].GetComponent<AlertMeter>().alertMeterPosY=lerpedY;

				xPos=lerpedX;
				yPos=lerpedY;
			}

			//float xPos = sideA * Mathf.Cos(angleRadians)+sideA + screenEdgeBuffer;
			//float yPos = Screen.height-(sideB * Mathf.Sin(angleRadians)+sideB) - screenEdgeBuffer;			
			
			GUIUtility.RotateAroundPivot( -currentAngle, new Vector2( xPos, yPos) );
			Rect meter = new Rect ( xPos-alertWidth/2, yPos-alertHeight/2, alertWidth, alertHeight );
			GUI.Box ( meter, GUIContent.none, Cube1 );

			GUIUtility.RotateAroundPivot( currentAngle,  new Vector2( xPos, yPos) );
			meterFill = new Rect ( xPos-alertWidth/2+0.5f, yPos-alertHeight/2+0.5f, alertWidth, alertHeight );
			GUI.Box ( meterFill, GUIContent.none, AlertFill );		
		}
	}

	public void DrawHUD()
	{
		GUI.DrawTexture( curveBarTop, CurveBarTopTex);
		GUI.DrawTexture( curveBar3, CurveBarTex);
		GUI.DrawTexture( curveBar4, CurveBarTex);

		// Draw Power Pods Inventory
		GUI.DrawTexture( powerPodsFrame, PowerPodTex);
		//ScreenHelper.DrawTexture( 48, 31, 16, 4, TransmitterCounter );
		ScreenHelper.DrawText( 58, 28, 5, 20, ""+ ThiefManager.Manager.GetTransmitterCount(), 80, Color.white );
	
		// Draw Compass Stuff
		GUI.DrawTexture( compassBase, CompassBaseTex);

		// get player angle
		float angleFwd = transform.localRotation.eulerAngles.y;
		//ScreenHelper.DrawText( 61, 29, 5, 20, ""+ angleFwd + "/"+ ThiefManager.Manager.maxTransmitterCount, 80, Color.white );

		//compassBase = new Rect( 0, Screen.height/20*15, 	Screen.width/7.5f, 	Screen.height/4.2f );

		float compassSide = Screen.width/15;
		float xPos = Screen.width/15;
		float yPos = Screen.height/20*15 + Screen.height/8.4f;
		GUIUtility.RotateAroundPivot( -angleFwd, new Vector2( xPos, yPos) );
		Rect compassFront = new Rect ( xPos-compassSide/2, yPos-compassSide/2, compassSide, compassSide );
		GUI.DrawTexture( compassFront, CompassFrontTex);
		GUIUtility.RotateAroundPivot( angleFwd, new Vector2( xPos, yPos) );

		
	}
	
}
