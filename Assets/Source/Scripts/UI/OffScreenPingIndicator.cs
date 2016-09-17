using UnityEngine;
using System.Collections;

public class OffScreenPingIndicator : MonoBehaviour {
	
	private GameObject m_player;
	private float PlayerToGuardAngle;
	public GUIStyle Cube;
	public bool InScreen  =true;
	
	private float screenEdgeBuffer = 60.0f;
	private float alertWidth;
	private float alertHeight;
	private int _hexIndex = 0;
	//private float _currentTick = 0.0f;
	private float PingDuration = 5.0f;
	private bool _pingActivated = false;
	#region singleton declearation
	private static OffScreenPingIndicator m_instance;
	private float _startTime = 0.01f;
	private bool _startPing = false;
	
	public OffScreenPingIndicator () 
    {
        //if (m_instance != null)
        //{
        //    return;
        //}
 
        m_instance = this;
    }
	
	public static OffScreenPingIndicator Manager
	{
		get
		{
			if(m_instance == null)
			{
				m_instance = new OffScreenPingIndicator();			
			}
			return m_instance;
		}
	}
	#endregion
	
	// Use this for initialization
	void Start () {
		alertWidth = Screen.width/20;
		alertHeight = Screen.width/20;	
	}
	
	// Update is called once per frame
	void Update () {

	}
	
	public float GetAngle()
	{
		return PlayerToGuardAngle;
	}
	
	public void CalculatePingPosition()
	{
		if((Time.time - _startTime) < PingDuration && _startPing)
		{
			GameObject playerCamera = (GameObject)GameObject.Find ("FPSCamera");
			Vector3 pingPos = HexGrid.Manager.GetCoordHex( _hexIndex, 0.02f);
			Vector3 playerForward = playerCamera.transform.forward;
			Vector3 playerRight = playerCamera.transform.right;
			Vector3 targetDir = pingPos - playerCamera.transform.position;
			
			float angleFwd = Vector3.Angle(targetDir, playerForward);
			Vector3 screenPos = playerCamera.camera.WorldToScreenPoint( pingPos );
			InScreen = (angleFwd<90.0f && screenPos.x<Screen.width && screenPos.x>0 && screenPos.y<Screen.height && screenPos.y>0);
				
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
		}
		else
		{
			InScreen = true;
		}
		
	}
	
	public void SetOffScreenPingPosition(int i_hexIndex)
	{
		_startPing = true;
		_startTime = Time.time;
		_hexIndex = i_hexIndex;
	}
}
