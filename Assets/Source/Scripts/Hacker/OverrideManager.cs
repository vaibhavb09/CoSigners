using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class OverrideManager : MonoBehaviour {
	
	public float MinRange;
	public float MaxRange;
	public float CountDownTime;
	public List<int> PossibleIndices;
	
	public GameObject _overridePlatform;
	private GameObject _overrideNode;
	private GameObject _overrideNodePrefab;
	private GameObject _overrideNodeFramePrefab;
	private GameObject _overrideNodeFrame;
	private GameObject _overridePlatformPrefab;
	private Material   _overrideActiveMat;
	private Material   _overrideInActiveMat;
	
	//private Texture2D LockdownTimer;
	private Texture2D WarningFrameBox;
	private Texture2D LockdowntimerFrame;
	private int startX;
	private int endX;
	private int currentX;
	private int slideRate;
	
	private static OverrideManager _instance;
	private float _timer;
	private float _syncInterval = 1.0f;
	private float _currentSyncTimer = 0.0f;
	private bool _startCounting = false;
	private int _overrideIndex;
	
	private bool _overrideOnScreen = true;
	private float _overrideAngle = 0.0f;
	private int _lastFuckedUpIndex = -1;
	
	#region Properties
	
	public bool IsOverrideOnScreen
	{
		get
		{
			return _overrideOnScreen;
		}
	}
	
	public float OverrideAngle
	{
		get{
			return _overrideAngle;
		}
	}
	#endregion
	
	// Use this for initialization
	void Start () {
	
	}
	
	public static OverrideManager Manager
	{
		get
		{
			if(_instance == null)
			{
				_instance = new OverrideManager();			
			}
			return _instance;
		}
	}

	public OverrideManager () 
    { 
        _instance = this;
    }
	
	public bool IsActive
	{
		get
		{
			return _startCounting;
		}
	}
	
	// Update is called once per frame
	void Update () {
		
		if(_startCounting && GameManager.Manager.PlayerType == 1)
		{
			if(_timer > CountDownTime)
			{
				NetworkManager.Manager.SyncOverrideLosingState();	
				NetworkManager.Manager.PauseGame(true);

			}
			if(!_overridePlatform.GetComponentInChildren<OverridePlatform>().Activated)
			{
				//Debug.Log("Trying To Refresh");
				Refresh();
			}
			
			_timer += Time.deltaTime;
			
			if(_currentSyncTimer > _syncInterval)
			{
				NetworkManager.Manager.SyncOverrideTimer(_timer);
				_currentSyncTimer = 0.0f;
			}
			_currentSyncTimer += Time.deltaTime;
		}
		
		if(_overrideNode != null)
		{
			CalcPosition();
		}
		
		if(_overrideIndex != -1 && _startCounting)
		{
			SlideInAnimation();
		}
		else
		{
			SlideOutAnimation();
		}

	}
	
	public void SyncTimer(float i_timer)
	{
		_timer = i_timer;
	}
	
	public void SyncLose()
	{
		Lose();
	}
	
	public void CalcPosition()
	{
		GameObject playerCamera = (GameObject)GameObject.Find ("TopDownCamera");
		Vector3 playerForward = playerCamera.transform.up;
		Vector3 playerRight = playerCamera.transform.right;
		Vector3 targetDir =  _overrideNode.transform.position - playerCamera.transform.position;
		targetDir.y = 0.0f;
		
		float angleFwd = Vector3.Angle(targetDir, playerForward);
		//Vector3 tracerPosition = new Vector3(transform.position.x, transform.position.y+1.5f, transform.position.z);
		Vector3 screenPos = playerCamera.camera.WorldToScreenPoint( _overrideNode.transform.position );
		_overrideOnScreen = (screenPos.x<Screen.width && screenPos.x>0 && screenPos.y<Screen.height && screenPos.y>0);
			
		// Determine if Guard is to right or left
		float angleRt = Vector3.Angle(targetDir, playerRight);
		bool right = (angleRt<90.0f);
		
		// Adjust displayed angle Based on camera angle ( right now assumes Level)
		//float camAngle = 90; // Looking up decreases angle, looking down increases.
		//float displayAngle = (((angleFwd-20)/170.0f)*110)+70;
		float displayAngle = angleFwd;
		
		// Convert Angle to GUI Coordinates
		if ( right )
		{
			if ( displayAngle <= 90 )
			{
				_overrideAngle = Mathf.Abs(displayAngle-90);
			}
			else
			{
				_overrideAngle = Mathf.Abs(displayAngle-450);
			}
		}
		else
		{
			_overrideAngle = displayAngle + 90;
		}
	}
	
	private void SlideInAnimation()
	{
		if( currentX <= endX )
			currentX += 8;
	}
	
	private void SlideOutAnimation()
	{
		if( currentX >= startX )
			currentX -= 8;
	}
	
	private void OnGUI()
	{
		if(_overrideIndex != -1 && _startCounting)
		{
			//Lockdown meter red 
			ScreenHelper.DrawTexture( 0, 1, 32, 2, WarningFrameBox ); 
			
			//ScreenHelper.DrawTexture( 0, 3, 12, 4, LockdownTimer );
			//GUI.DrawTexture( new Rect( ScreenHelper.startX + (float)currentX , ScreenHelper.startY + 3.0f * ScreenHelper.GetUnitLength(), 
				//12.0f * ScreenHelper.GetUnitLength(), 4.0f * ScreenHelper.GetUnitLength() ), LockdownTimer );
			
			string formattedTime = "";
			int time = (int)(CountDownTime - _timer);
			int mins = time / 60;
			int seconds = time % 60;
			if( seconds < 10 )
				formattedTime = mins.ToString() + ":0" + seconds.ToString();
			else
				formattedTime = mins.ToString() + ":" + seconds.ToString();
			
			if( currentX >= endX )
			{
				//ScreenHelper.DrawText( 2, 4, 5, 2, formattedTime, 50, new Color( 1.0f, 0.79f, 0.71f, 1.0f ) );
				if( GameManager.Manager.PlayerType == 1 )
				{
					ScreenHelper.DrawTexture(26.0f, 4.5f, 12.0f, 3.0f, LockdowntimerFrame);
					ScreenHelper.DrawText( 29.5f, 5.2f, 5.0f, 2.0f, formattedTime, 50, new Color( 1.0f, 0.79f, 0.71f, 1.0f ) );
				}
				if( GameManager.Manager.PlayerType == 2 )
				{
					ScreenHelper.DrawTexture(26.0f, 3.7f, 12.0f, 3.0f, LockdowntimerFrame);
					ScreenHelper.DrawText( 29.5f, 4.4f, 5.0f, 2.0f, formattedTime, 50, new Color( 1.0f, 0.79f, 0.71f, 1.0f ) );
				}
				currentX = 0;
			}
			//GUI.Label(new Rect(96,56,160,48), formattedTime );
		}
		else
		{
			//if( LockdownTimer != null )
			//GUI.DrawTexture( new Rect(  ScreenHelper.startX + (float)currentX,  ScreenHelper.startY + 3.0f * ScreenHelper.GetUnitLength(), 
				//12.0f * ScreenHelper.GetUnitLength(), 4.0f * ScreenHelper.GetUnitLength() ), LockdownTimer );
			if( currentX <= startX )
			{
				currentX = (int)(-12 * ScreenHelper.GetUnitLength());
			}
		}
	}
	
	void LoadResources()
	{
		_overrideIndex = -1;
		_overrideNodePrefab = (GameObject)Resources.Load("Prefabs/Hacker/Graph/Override_Prefab");
		_overridePlatformPrefab = (GameObject)Resources.Load("Prefabs/Theif/OverridePlatform");
		_overrideNodeFramePrefab = (GameObject)Resources.Load("Prefabs/Hacker/Graph/OverrideTarget_Prefab");

		LockdowntimerFrame = Resources.Load("Textures/Lockdown_Count_bkg", typeof(Texture2D)) as Texture2D;
			 
		if(_overridePlatformPrefab == null)
		{
			//Debug.Log("platform load fail");
		}
		_overrideActiveMat = Resources.Load("Materials/Override_active", typeof(Material)) as Material;
		_overrideInActiveMat = Resources.Load("Materials/Override_inactive", typeof(Material)) as Material;
		//LockdownTimer = Resources.Load("Textures/Timer_background") as Texture2D;
		WarningFrameBox = Resources.Load("Textures/HackerGUI/LockdownFrame_Red_New") as Texture2D;
		startX = (int)(-12 * ScreenHelper.GetUnitLength());
		endX = 0;
		currentX = startX;
		slideRate = 8;
	}
	
	public bool IsInOverride()
	{
		return _startCounting;
	}
	
	public void Refresh()
	{
		if(_overrideIndex != -1)
		{
			//if(GameManager.Manager.PlayerType == 2) //hacker
			{
				//Debug.Log("Hex Index that we are checking: " + _overrideIndex);
				if(HexGrid.Manager.IsHexCaptured ( _overrideIndex ))
				{
					//Debug.Log("Hex Captured");
					NetworkManager.Manager.EnableOverride();
				}
				else
				{
					//Debug.Log("Hex Uncaptured");
					HackerNetManager.Manager.DisableOverride();
				}
			}
		}
	}
	
	public GameObject GetOverrideNode()
	{
		return _overrideNode;
	}
	public GameObject GetOverridePlatform()
	{
		return _overridePlatform;
	}

	public GameObject GetOverrideFrame()
	{
		return _overrideNodeFrame;
	}
	
	public void SetOverridePlatform(GameObject i_platform)
	{
		_overridePlatform = i_platform;
	}
	public void EnableOverride()
	{
		//Debug.Log("Change To Active");
		if(_overrideNode != null)
		{
			_overrideNode.renderer.material = _overrideActiveMat;
		}
		//Debug.Log("My Player Type: " + GameManager.Manager.PlayerType);
		if(GameManager.Manager.PlayerType == 1) //thief
		{
			EnableTheifOverride();
			
			// [ SOUND TAG ]  [OS_Button_Reveal] [if _overridePlatform not null then use it for sound source]
			if(_overridePlatform != null)
				soundMan.soundMgr.playOneShotOnSource(_overridePlatform.audio,"OS_Button_Reveal",GameManager.Manager.PlayerType);
				
		}
	}

	public void CloseOverridePlatform()
	{
		//Debug.Log("#Max: deleting override platform");
		if(_overridePlatform != null)
		{
			Destroy(_overridePlatform);
		}
		_overridePlatform = null;
		if( _overrideNode != null )
		{
			Destroy(_overrideNode);
		}
		_overrideNode = null;

	}

	public void DisableOverride()
	{
		//Debug.Log("Change To Inactive");
		_overrideNode.renderer.material = _overrideInActiveMat;
		
		//Debug.Log("My Player Type: " + GameManager.Manager.PlayerType);
		if(GameManager.Manager.PlayerType == 1)
		{
			DisableTheifOverride();
		}
	}
	
	public void LoadOverrideDataFromConfig(GraphData i_gData)
	{
		LoadResources();
		PossibleIndices.Clear();
		OverrideData[] overrideData = i_gData.Overrides;
		foreach(OverrideData overrides in overrideData)
		{
			PossibleIndices.Add(overrides.HexIndex);
		}
		MinRange = i_gData.Globals[0].OverrideMinDist;
		MaxRange = i_gData.Globals[0].OverrideMaxDist;
		CountDownTime = i_gData.Globals[0].LockdownTime;
	}
	
	public void CreateOverrideBasedOnPlayerPosition()
	{
		
		//if(_overrideIndex == -1)
		//{
		//	//Debug.Log("WTF?");
		//	return;
		//}
		
		if(GameManager.Manager.PlayerType == 2) //only hacker should do this
		{
			//Debug.Log("Create override stuff");
			GameObject Player = GameObject.Find("Playertheif(Clone)");
			//Debug.Log("#Max: New Override Index Assigned: " + _overrideIndex);
			_overrideIndex =  PickRandomHexForOverride(Player.transform.position);
			NetworkManager.Manager.InitializeLockDownSequence(_overrideIndex);
			//GA.API.Design.NewEvent("Game:LockDownSequence", Time.timeSinceLevelLoad);
		}
	}
	
	public void CreateOverrideBasedOnIndex(int i_index)
	{ 	
		//Debug.Log("StartLockDown");
		if(!_startCounting)
		{
			_overrideIndex = i_index;
			//Debug.Log("Hex index that we are creating: " + i_index);
		}
		if ( GameManager.Manager.PlayerType == 1)
		{
			_overrideNode = (GameObject)Instantiate( _overrideNodePrefab, HexGrid.Manager.GetCoordHex(_overrideIndex, 0.01f), Quaternion.identity);
			_overridePlatform = (GameObject)Instantiate( _overridePlatformPrefab, HexGrid.Manager.GetCoordHex(_overrideIndex, 0.01f), Quaternion.identity);
			//OverrideManager.Manager.SetOverridePlatform(_overridePlatform);
			if(_overridePlatform == null)
			{
				//Debug.Log("_overridePlatform fail");
			}
			//else
			//{
				//Debug.Log("Loaded");
			//}
		}
		else
		{
			_overrideNode = (GameObject)Instantiate( _overrideNodePrefab, HexGrid.Manager.GetCoordHex(_overrideIndex, 60.0f), Quaternion.identity);
			_overrideNodeFrame = (GameObject)Instantiate( _overrideNodeFramePrefab, HexGrid.Manager.GetCoordHex(_overrideIndex, 60.0f), Quaternion.identity);
		}
		
		_startCounting = true;


		GraphManager.Manager.GetEndDoorNode().ProtectEndDoor();

		LockDownManager.Manager.TriggerLockDownAnimation();
		//OverrideManager.Manager.Refresh();
		_timer = 0.0f;
		
	}

	public void EnableTheifOverride()
	{
		_overridePlatform.GetComponentInChildren<OverridePlatform>().Enable();
	}
	
	public void DisableTheifOverride()
	{
		_overridePlatform.GetComponentInChildren<OverridePlatform>().Disable();
	}
	
	public void OverrideSuccess()
	{
		//Debug.Log("Override Successs");
		HackerThreat.Manager.ResetThreatLevel();
		GameManager.Manager.HackerCaught = false;
		LockDownManager.Manager.DisableLockDownAnimation();
		_startCounting = false;
		_timer = 0.0f;
		
		// [SOUND TAG] [OS_Button_Press]
		if(_overridePlatform != null)
			soundMan.soundMgr.playOneShotOnSource(_overridePlatform.audio,"OS_Button_Press",GameManager.Manager.PlayerType);
		
		
		Destroy(_overrideNode);
		_overrideNode = null;

		if(_overrideNodeFrame != null)
		{
			Destroy(_overrideNodeFrame);
		}
		_overrideNodeFrame = null;
		
		// Reset End Door State if connected.
		DoorNode endDoorNode = GraphManager.Manager.GetEndDoorNode();
		GraphManager.Manager.GetEndDoorNode().SetConnected( endDoorNode.Connected );

		_overrideIndex = -1;
	}
	
	private void Lose()
	{
		soundMan.soundMgr.silenceSource(null,GameManager.Manager.PlayerType);
		
		GameObject.Find("MessageBox").GetComponent<MessageBox>().MessageBoxShow(1);
		//Debug.Log("Lost By LockDown");
		GameManager.Manager.HackerCaught = false;
		_startCounting = false;
		_timer = 0.0f;
	}
	
	private int PickRandomHexForOverride(Vector3 i_playerPos)
	{
		System.Random random = new System.Random();
		List<int> potentialList = new List<int>();
		foreach(int index in PossibleIndices)
		{
			Vector3 hexPos = HexGrid.Manager.GetCoord(index);
			float distance = GetDistanceFromCoord(i_playerPos, hexPos);
			if(distance > MinRange && distance < MaxRange)
			{
				potentialList.Add(index);
			}
		}
		if(potentialList.Count != 0)
		{
			int i = random.Next(potentialList.Count);
			return potentialList[i];
		}
		else
		{
			//if no proper hex is found, spawn a random one
			int i = random.Next(PossibleIndices.Count);
			if(_lastFuckedUpIndex == -1)
			{
				_lastFuckedUpIndex = i;
			}
			else
			{
				if(PossibleIndices.Count == 1)
				{
					_lastFuckedUpIndex = 0;
					return PossibleIndices[0];
				}
				else if(PossibleIndices.Count == 0)
				{
					return -1;
				}
				else
				{
					while(_lastFuckedUpIndex == i)
					{
						i = random.Next(PossibleIndices.Count);
					}
					_lastFuckedUpIndex = i;
					return PossibleIndices[i];
				}
				
			}
			return PossibleIndices[i];
		}
		
		return -1;
	}
	
	private float GetDistanceFromCoord(Vector3 i_playerPos, Vector3 i_hexPos)
	{
		return Vector2.Distance(new Vector2(i_playerPos.x, i_playerPos.z),new Vector2(i_hexPos.x, i_hexPos.z));
	}
}
