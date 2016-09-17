using UnityEngine;
using System.Collections;

public class AlertMeter : MonoBehaviour 
{
	private GameObject m_player;
	public float m_posX, m_posY, m_diffX, m_diffZ, m_angle;
	GUIStyle style1;
	GUIStyle style2;
	public Texture2D AlertBackground;
	public Texture2D AlertForeground;
	private const float STRAIGHT_IN_FRONT = 0.0f;
	private const float DIRECTLY_BEHIND = 180.0f;
	private const float FULL_ALERT = 100.0f;
	private const float NOT_ALERT = 0.0f;
	private Transform meterInstance;
	//public AlertPositioning myMeter;
	public float PlayerToGuardAngle;
	public bool onScreen;
	public float alertMeterPosX;
	public float alertMeterPosY;
	public bool firstSpotted;
	
	void Start () 
	{
		AlertBackground = Resources.Load ("Textures/RedAlertBkg", typeof(Texture2D)) as Texture2D;
		AlertForeground = Resources.Load ("Textures/RedAlert", typeof(Texture2D)) as Texture2D;
		
		style1 = new GUIStyle();
		style1.normal.background = AlertBackground;
		style2 = new GUIStyle();
		style2.normal.background = AlertForeground;
		firstSpotted=false;
		//m_player = GameObject.FindGameObjectWithTag("Player");
	}
	
	
	void Update ()
	{
		if ( GetComponent<Perception>().AlertLevel > 0 )
			SetAlertPosition();
	}
	
	void OnGUI()
	{
		/*
		if(GameManager.Manager.PlayerType == 1 && gameObject.GetComponent<AlertSystem>().GetVisibility()) // is thief
		{
			GUI.Box(new Rect(m_posX, m_posZ, 40, 100), new GUIContent(""), style1);
			GUI.Box(new Rect(m_posX, (m_posZ + 100- gameObject.GetComponent<AlertSystem>().m_level), 40, gameObject.GetComponent<AlertSystem>().m_level), new GUIContent(""), style2);
		}*/
	}
	
	public void SetAlertPosition()
	{
		
		// Determine if Guard is on Screen
		GameObject playerCamera = (GameObject)GameObject.Find ("FPSCamera");
		Vector3 playerForward = playerCamera.transform.forward;
		Vector3 playerRight = playerCamera.transform.right;
		Vector3 targetDir = transform.position - playerCamera.transform.position;
		
		float angleFwd = Vector3.Angle(targetDir, playerForward);
		Vector3 droneTopPos = new Vector3(transform.position.x, transform.position.y+1.5f, transform.position.z);
		Vector3 screenPos = playerCamera.camera.WorldToScreenPoint( droneTopPos );
		onScreen = (angleFwd<90.0f && screenPos.x<Screen.width && screenPos.x>0 && screenPos.y<Screen.height && screenPos.y>0);
			
		// Determine if Guard is to right or left
		float angleRt = Vector3.Angle(targetDir, playerRight);
		bool right = (angleRt<90.0f);
		
		// Adjust displayed angle Based on camera angle ( right now assumes Level)
		float camAngle = 90; // Looking up decreases angle, looking down increases.
		float displayAngle = (((angleFwd-20)/170.0f)*110)+70;
		//float displayAngle = angleFwd;
		
		// Convert Angle to GUI Coordinates
		if ( right )
		{
			if ( displayAngle <= 90 )
			{
				PlayerToGuardAngle = Mathf.Abs(displayAngle-90);
			}
			else
			{
				PlayerToGuardAngle = Mathf.Abs(displayAngle-450);
			}
		}
		else
		{
			PlayerToGuardAngle = displayAngle + 90;
		}
			
			//Debug.Log ("Angle: " + angleFwd + " - Right: " + right);
	}
	
	public float DetermineAngleSign( GameObject i_player, float i_angle, Vector2 i_vecPlayerToGuard )
	{
		//Determine whether guard is to left or right of player
		Vector2 vecRightPlayer = new Vector2( m_player.transform.right.x, m_player.transform.right.z );
		float cos_angleRightToPlayer = Vector2.Dot( i_vecPlayerToGuard.normalized, vecRightPlayer.normalized );
		float angleRightToPlayer = Mathf.Rad2Deg * Mathf.Acos( cos_angleRightToPlayer );
		if( angleRightToPlayer > 90.0f )
		{
			return -i_angle;	
		}
		return i_angle;
	}
}
