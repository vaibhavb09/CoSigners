using UnityEngine;
using System.Collections;

public class NetworkManager : Photon.PunBehaviour {
	
	#region Singleton Declaration
	
	private static NetworkManager m_instance;
	private PivotManager _manager;
	private GameObject _playerUtil;
	private GameObject _chatInstance;
	private GameObject _playerThief;
	private HackerGUI _hackGUI;


	public static NetworkManager Manager
	{
		get
		{
			if(m_instance == null)
			{
				m_instance = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();			
			}
			return m_instance;
		}
	}

	static int _initTime = 0;
	int _initTimes = 0;
	
	void Awake()
	{
		Debug.Log("Connecting to photon Server");
		if(_initTime == 0 && _initTimes == 0)
		{
			DontDestroyOnLoad(transform.gameObject);
			_initTime++;
			_initTimes++;
			InitControl.PreGameInitialization();
		}
		else if(_initTime != 0 && _initTimes == 0)
		{
			DestroyImmediate(transform.gameObject);
		}
	}
	#endregion

	#region PlayerSync

	public void SetPlayer(GameObject i_player)
	{
		_playerThief = i_player;
	}

	public void SyncPlayerPosition(Vector3 i_position)
	{
		photonView.RPC("SyncPlayerPositionRPC", PhotonTargets.Others, i_position);
	}

	[PunRPC]
	public void SyncPlayerPositionRPC(Vector3 i_position)
	{
		if(_playerThief != null)
		{
			_playerThief.GetComponent<PlayerSync>().SetPosition(i_position);
		}
	}

	public void SyncPlayerRotation(Quaternion i_rotation)
	{
		photonView.RPC("SyncPlayerRotationnRPC", PhotonTargets.Others, i_rotation);
	}
	
	[PunRPC]
	public void SyncPlayerRotationnRPC(Quaternion i_rotation)
	{
		if(_playerThief != null)
		{
			_playerThief.GetComponent<PlayerSync>().SetRotation(i_rotation);
		}
	}
	#endregion

	#region GameInitStuff
	public void CheckIfGameIsReady()
	{
		if(GameManager.Manager.ThiefReady && GameManager.Manager.HackerReady && !GameManager.Manager.LevelStarted)
		{
			photonView.RPC("PostGameStartRPC", PhotonTargets.All);
		}
	}

	[PunRPC]
	public void PostGameStartRPC()
	{
		InitControl.PostGameStart();
	}

	public void ImReady(int i_playerType, bool i_inTransition)
	{
		switch(i_playerType)
		{
		case 1://thief
			photonView.RPC("ThiefReadyRPC", PhotonTargets.All, i_inTransition);
			break;

		case 2://hacker
			photonView.RPC("HackerReadyRPC", PhotonTargets.All, i_inTransition);
			break;
		}
	}

	[PunRPC]
	private void ThiefReadyRPC(bool i_inTransition)
	{
		if(!i_inTransition)
		{
			GameManager.Manager.ThiefReady = true;

			//if both are ready, Game should start
			if(GameManager.Manager.HackerReady == true && !Application.isLoadingLevel)
			{
				InitControl.PostGameStart();
			}
		}
		else
		{
			GameObject scaleformCamera =  GameObject.FindGameObjectWithTag("ScaleformCamera");
			scaleformCamera.GetComponent<ScaleFormCamera>().Init();
		}
	}

	[PunRPC]
	private void HackerReadyRPC(bool i_inTransition)
	{
		if(!i_inTransition)
		{
			GameManager.Manager.HackerReady = true;

			//if both are ready, Game should start
			if(GameManager.Manager.ThiefReady == true && !Application.isLoadingLevel)
			{
				InitControl.PostGameStart();
			}
		}
		else
		{
			GameObject scaleformCamera =  GameObject.FindGameObjectWithTag("ScaleformCamera");
			scaleformCamera.GetComponent<ScaleFormCamera>().Init();
		}
	}

	#endregion
	
	#region ScaleForm
	public void ChangeInStartMenuStatus(bool i_isInStartMenu)
	{
		photonView.RPC("ChangeInStartMenuStatusRPC", PhotonTargets.All, i_isInStartMenu);
	}


	[PunRPC]
	public void ChangeInStartMenuStatusRPC(bool i_isInStartMenu)
	{
		GameManager.Manager.InStartMenu = i_isInStartMenu;
	}

	public void SetupLevelInfoInternally(string i_levelName, string i_transmitterCount, string i_estimatedTime, string i_difficulty, string i_desc, Texture2D i_texture, int i_playerType = 0)
	{
		GameObject scaleformCamera =  GameObject.FindGameObjectWithTag("ScaleformCamera");
		scaleformCamera.GetComponent<ScaleFormCamera>().SetUpLevelInfo(i_levelName, i_transmitterCount, i_estimatedTime, i_difficulty, i_desc,i_texture);
	}

	public void InvokeMethodInternally(string i_swfName, string i_variable, int i_playerType = 0)
	{
		GameObject scaleformCamera =  GameObject.FindGameObjectWithTag("ScaleformCamera");
		scaleformCamera.GetComponent<ScaleFormCamera>().Play(i_swfName);
	}


	//1 - thief 2 - Hacker
	public void PlayMovie(string i_swfName, int i_playerType = 0)
	{
		photonView.RPC("PlayMovieRPC", PhotonTargets.All, i_swfName, i_playerType);
	}
	[PunRPC]
	public void PlayMovieRPC(string i_swfName, int i_playerType)
	{
		GameObject scaleformCamera =  GameObject.FindGameObjectWithTag("ScaleformCamera");
		scaleformCamera.GetComponent<ScaleFormCamera>().Play(i_swfName);
	}

	public void PlayAnimation(string i_swfName, string i_functionCall, int i_playerType = 0)
	{

		photonView.RPC("PlayAnimationRPC", PhotonTargets.All, i_swfName, i_functionCall, i_playerType);
	}

	[PunRPC]
	public void PlayAnimationRPC(string i_swfName, string i_functionCall, int i_playerType = 0)
	{
		GameObject scaleformCamera =  GameObject.FindGameObjectWithTag("ScaleformCamera");
		scaleformCamera.GetComponent<ScaleFormCamera>().PlayAnimation(i_swfName, i_functionCall);
	}
	
	public void StopMovie(string i_swfName, int i_playerType = 0)
	{
		photonView.RPC("StopMovieRPC", PhotonTargets.All, i_swfName, i_playerType);

	}


	[PunRPC]
	public void StopMovieRPC(string i_swfName, int i_playerType)
	{
		GameObject scaleformCamera =  GameObject.FindGameObjectWithTag("ScaleformCamera");
		scaleformCamera.GetComponent<ScaleFormCamera>().Stop(i_swfName);

	}

	#endregion
	
	#region Restart Level
	
	public void ChangeTestMode()
	{
		photonView.RPC("ChangeTestModeRPC", PhotonTargets.All);
	}
	
	[PunRPC]
	public void ChangeTestModeRPC()
	{
		GameObject.Find("TestingModeController").GetComponent<TestingMode>().ChangeTestMode();
		
	}	

	public void endLevelMenuResponse( int selection, bool value)
	{
		photonView.RPC("endLevelMenuResponseRPC", PhotonTargets.All, selection, value);
	}
	
	[PunRPC]
	public void endLevelMenuResponseRPC(int selection, bool value)
	{
		switch (selection)
		{
		case 1:
			GameObject.Find("MessageBox").GetComponent<MessageBox>().nxtLevelThief = value;
			GameObject.Find("MessageBox").GetComponent<MessageBox>().replayThief = false;
			GameObject.Find("MessageBox").GetComponent<MessageBox>().mainMenuThief = false;
			
			break;
		case 2:
			GameObject.Find("MessageBox").GetComponent<MessageBox>().nxtLevelHacker = value;
			GameObject.Find("MessageBox").GetComponent<MessageBox>().replayHacker = false;
			GameObject.Find("MessageBox").GetComponent<MessageBox>().mainMenuHacker = false;
			break;
		case 3:
			GameObject.Find("MessageBox").GetComponent<MessageBox>().nxtLevelThief = false;
			GameObject.Find("MessageBox").GetComponent<MessageBox>().replayThief = value;
			GameObject.Find("MessageBox").GetComponent<MessageBox>().mainMenuThief = false;
			break;			
		case 4:
			GameObject.Find("MessageBox").GetComponent<MessageBox>().nxtLevelHacker = false;
			GameObject.Find("MessageBox").GetComponent<MessageBox>().replayHacker = value;
			GameObject.Find("MessageBox").GetComponent<MessageBox>().mainMenuHacker = false;
			break;
		case 5:
			GameObject.Find("MessageBox").GetComponent<MessageBox>().nxtLevelThief = false;
			GameObject.Find("MessageBox").GetComponent<MessageBox>().replayThief = false;
			GameObject.Find("MessageBox").GetComponent<MessageBox>().mainMenuThief = value;
			break;
		case 6:
			GameObject.Find("MessageBox").GetComponent<MessageBox>().nxtLevelHacker = false;
			GameObject.Find("MessageBox").GetComponent<MessageBox>().replayHacker = false;
			GameObject.Find("MessageBox").GetComponent<MessageBox>().mainMenuHacker = value;
			break;
		}
	}
	
	public void RestartLevel()
	{
		photonView.RPC("RestartLevelRPC", PhotonTargets.All);
	}
	
	[PunRPC]
	public void RestartLevelRPC()
	{
		//Debug.Log("#Max:Restart");
		//Application.LoadLevel("Level_X");
		Application.LoadLevel(Application.loadedLevel);
	}

	public void GoBackToLoginScreen()
	{
		photonView.RPC("GoBackToLoginScreenRPC", PhotonTargets.All );
	}

	[PunRPC]
	public void GoBackToLoginScreenRPC()
	{	
		//Application.LoadLevel("Level_X");
		_playerUtil = GameObject.Find("PlayerUtil");
		Time.timeScale = 1;
		//Time.timeScale = 1;
//		if(GameManager.Manager.PlayerType == 1)
//		{
//			//GameObject.Find("Playertheif(Clone)").GetComponent<MouseLookAround>().enabled = true;
//			//Screen.lockCursor = true;
//			ThiefManager.Manager.EnableThiefActions();
//		}
//		else
//		{
//			//GameObject.Find("TopDownCamera").GetComponent<HackerActions>().EnableHackerActions();
//			HackerManager.Manager.EnableHackerActions();
//		}
		Screen.lockCursor = false;
		if(PhotonNetwork.isMasterClient)
		{
			MasterServer.UnregisterHost();
		}
		_playerUtil.GetComponent<AccountSystem>().ResetNamesAfterForceShutDown();
		//PhotonNetwork.Disconnect();
		Application.LoadLevel(0);
	}

	public void LoadLevel( int levelNumber)
	{
		photonView.RPC("LoadLevelRPC", PhotonTargets.All,levelNumber );
	}
	
	[PunRPC]
	public void LoadLevelRPC(int levelNumber)
	{
		//Debug.Log("Restart");
		//Application.LoadLevel("Level_X");
		Application.LoadLevel(levelNumber);
	}

	public void LoadLevel( string LevelName )
	{
		photonView.RPC("LoadLevelUsingStringRPC", PhotonTargets.All, LevelName );
	}
	
	[PunRPC]
	public void LoadLevelUsingStringRPC( string LevelName )
	{
		GameManager.Manager.CurrentLevelName = Application.loadedLevelName;
		//if(GameManager.Manager.AO != null)
		//	GameManager.Manager.AO.allowSceneActivation = true;
		//Application.LoadLevel("Level_X");
		Application.LoadLevel(LevelName);
	}

	public void ShowLevelEndScreen( int value)
	{
		photonView.RPC("ShowLevelEndScreenRPC", PhotonTargets.All,value );
	}
	
	[PunRPC]
	public void ShowLevelEndScreenRPC(int value)
	{
		//Debug.Log("Level End");
		//Application.LoadLevel("Level_X");
		GameObject.Find("MessageBox").GetComponent<MessageBox>().MessageBoxShow(value);	
	}
	#endregion

	#region ChatRelated
	public void SendChatMessage(string i_message)
	{
		photonView.RPC("SendChatMessageRPC", PhotonTargets.All, i_message);
	}

	[PunRPC]
	public void SendChatMessageRPC(string i_message)
	{
		_chatInstance = GameObject.Find("Chat");
		_chatInstance.GetComponent<InGameChatScript>().SendMessage(i_message);
	}

	#endregion

	#region public network wide functions
	#region Connection Related
	public void UploadConenctions(bool[] i_ConnectionArray)
	{
		// very ugly! Sending more than 8 bytes of data for every element
		for(int i = 0; i < i_ConnectionArray.Length; i++)
		{	
			int temp = i;
			int packedData = i_ConnectionArray[i]? 1 + (temp << 1): 0 + (temp << 1);
			photonView.RPC("SendConnectionDataRPC", PhotonTargets.All, packedData);
		}
	}
	

	public void RotatePivot( int i_index, bool i_clear)
	{
		photonView.RPC("RotatePivotRPC", PhotonTargets.All, i_index, i_clear);
	}
	
	[PunRPC]
	public void RotatePivotRPC( int i_index, bool i_clear )
	{
		PivotManager.Manager.RotatePivot(i_index, i_clear);
	}
	

	public void RefreshConnections()
	{
		photonView.RPC("RefreshConnectionsRPC", PhotonTargets.All);
	}
	
	[PunRPC]
	public void RefreshConnectionsRPC( )
	{
		PivotManager.Manager.RefreshConnections();
	}
	
	
	public void CreatePivot( int i_index, bool i_centered, int i_armA, int i_armB )
	{
		photonView.RPC("CreatePivotRPC", PhotonTargets.All, i_index, i_centered, i_armA, i_armB);
	}
	
	[PunRPC]
	public void CreatePivotRPC(int i_index, bool i_centered, int i_armA, int i_armB )
	{
		PivotManager.Manager.AddPivot(i_index, i_centered, i_armA, i_armB);
		//ThiefGrid.Manager.AddNewLink(i_index, i_centered, i_armA, i_armB);
	}
	
	public void RemovePivot( int i_index, bool i_refresh )
	{
		photonView.RPC("RemovePivotRPC", PhotonTargets.All, i_index, i_refresh);
	}
	
	[PunRPC]
	public void RemovePivotRPC(int i_index, bool i_refresh )
	{
		PivotManager.Manager.RemovePivot( i_index, i_refresh );
	}
	
	
	public void UpdateConnectionManager( byte[] i_connections )
	{
		//Debug.Log ("Ran UpdateConnectiosn Manager");

		photonView.RPC("UpdateConnectionManagerRPC", PhotonTargets.Others, i_connections);
	}
	
	[PunRPC]
	public void UpdateConnectionManagerRPC( byte[] i_connections )
	{
		//Debug.Log ("Ran UpdateConnectionManager RPC");
		ConnectionManager.Manager.UpdateConnectionsThief( i_connections);
	}

	public void RaiseSecurityClearance()
	{
		photonView.RPC("RaiseSecurityClearanceRPC", PhotonTargets.All);
	}
	
	[PunRPC]
	public void RaiseSecurityClearanceRPC()
	{
		GraphManager.Manager.RaiseSecurityClearance();
	}



	public void AlterFloorLine(int i_line, bool i_addLine)
	{
		photonView.RPC("AlterFloorLineRPC", PhotonTargets.Others, i_line, i_addLine);
	}
	
	[PunRPC]
	public void AlterFloorLineRPC( int i_line, bool i_addLine )
	{
		ThiefGrid.Manager.AlterLine( i_line, i_addLine);
	}
	#endregion
	
	#region DoorRelated
	
	public void SyncDoorType( int i_doorIndex, int i_type )
	{
		photonView.RPC("SyncDoorTypeRPC", PhotonTargets.All, i_doorIndex, i_type);
	}
	
	[PunRPC]
	public void SyncDoorTypeRPC( int i_doorIndex, int i_type )
	{
		DoorNode thisDoor = (DoorNode)GraphManager.Manager.GetNode( i_doorIndex );
		if( thisDoor != null )
			thisDoor._doorType = (DoorType)i_type;
		//if( thisDoor._doorType == DoorType.EndDoor )
			//thisDoor.isEndDoor = true;
	}

	public void SetDoorClearance( int i_doorIndex, bool i_clear )
	{
		photonView.RPC("SetDoorClearanceRPC", PhotonTargets.All, i_doorIndex, i_clear);
	}
	
	[PunRPC]
	public void SetDoorClearanceRPC( int i_doorIndex, bool i_clear )
	{
		GameObject door = PointmanNetManager.Manager.GetDoorObject(i_doorIndex);
		if( door != null )
		{
			DoorController doorController = door.GetComponent<DoorController>();

			if( doorController != null )
				doorController.SetClearance(i_clear);
		}
	}

	public void SetGuardProximity( int i_doorIndex, bool closeEnough )
	{
		photonView.RPC("SetGuardProximityRPC", PhotonTargets.All, closeEnough);
	}
	
	[PunRPC]
	public void SetGuardProximityRPC( int i_doorIndex, bool closeEnough )
	{
		PointmanNetManager.Manager.GetDoorObject(i_doorIndex).GetComponent<DoorController>().guardCloseEnoughToOpen = closeEnough;
	}

	//passing door number
	public void OpenDoor(int i_doorIndex)
	{
		photonView.RPC("OpenDoorRPC", PhotonTargets.All, i_doorIndex);
	}
	
	[PunRPC]
	public void OpenDoorRPC(int i_doorIndex)
	{
		PointmanNetManager.Manager.OpenDoor(i_doorIndex);
		HackerNetManager.Manager.OpenDoor(i_doorIndex);
	}
	
	public void CloseDoor(int i_doorIndex)
	{
		photonView.RPC("CloseDoorRPC", PhotonTargets.All, i_doorIndex);
	}
	
	[PunRPC]
	public void CloseDoorRPC(int i_doorIndex)
	{
		PointmanNetManager.Manager.CloseDoor(i_doorIndex);		
		HackerNetManager.Manager.CloseDoor(i_doorIndex);
	}

	public void DisableDoorTimer(int i_doorIndex, float i_time)
	{
		photonView.RPC("DisableDoorTimerRPC", PhotonTargets.All, i_doorIndex, i_time);
	}
	
	[PunRPC]
	public void DisableDoorTimerRPC(int i_doorIndex, float i_time)
	{
		DoorNode thisDoor = (DoorNode)GraphManager.Manager.GetNode( i_doorIndex );
		thisDoor.SetDisableTimer( i_time );
	}

	public void BlockDoor(int i_doorIndex)
	{
		photonView.RPC("BlockDoorRPC", PhotonTargets.All, i_doorIndex);
	}
	
	[PunRPC]
	public void BlockDoorRPC(int i_doorIndex)
	{
		DoorNode thisDoor = (DoorNode)GraphManager.Manager.GetNode( i_doorIndex );
		DoorStates.OnActorEnter( thisDoor );
	}

	public void UnBlockDoor(int i_doorIndex)
	{
		photonView.RPC("UnBlockDoorRPC", PhotonTargets.All, i_doorIndex);
	}
	
	[PunRPC]
	public void UnBlockDoorRPC(int i_doorIndex)
	{
		DoorNode thisDoor = (DoorNode)GraphManager.Manager.GetNode( i_doorIndex );
		DoorStates.OnActorExit( thisDoor );
	}

	public void LockDoor(int i_doorIndex)
	{
		photonView.RPC("LockDoorRPC", PhotonTargets.All, i_doorIndex);
	}
	
	[PunRPC]
	public void LockDoorRPC(int i_doorIndex)
	{
		HackerNetManager.Manager.LockDoor( i_doorIndex );
		PointmanNetManager.Manager.LockDoor( i_doorIndex );
	}

	public void UnlockDoor(int i_doorIndex)
	{
		photonView.RPC("UnlockDoorRPC", PhotonTargets.All, i_doorIndex);
	}
	
	[PunRPC]
	public void UnlockDoorRPC(int i_doorIndex)
	{
		HackerNetManager.Manager.UnlockDoor( i_doorIndex );
		PointmanNetManager.Manager.UnlockDoor( i_doorIndex );
	}
	
	public void SecureLockDoor(int i_doorIndex)
	{
		photonView.RPC("SecureLockDoorRPC", PhotonTargets.All, i_doorIndex);
	}
	
	[PunRPC]
	public void SecureLockDoorRPC(int i_doorIndex)
	{
		PointmanNetManager.Manager.SecureLockDoor( i_doorIndex ); //Thief
		HackerNetManager.Manager.SecureLockDoor( i_doorIndex ); //Hacker	
	}	
	
	public void UnSecureLockDoor(int i_doorIndex)
	{
		photonView.RPC("UnSecureLockDoorRPC", PhotonTargets.All, i_doorIndex);
	}
	
	[PunRPC]
	public void UnSecureLockDoorRPC(int i_doorIndex)
	{
		HackerNetManager.Manager.UnSecureLockDoor ( i_doorIndex );
		PointmanNetManager.Manager.UnSecureLockDoor( i_doorIndex ); //Thief
	}
	#endregion

	#region PauseGame
	
	public void PauseGame(bool pause)
	{
		photonView.RPC("PauseGameRPC", PhotonTargets.All, pause);
	}
	
	[PunRPC]
	public void PauseGameRPC(bool pause)
	{
		if(GameManager.Manager.PlayerType == 1)
		{
			if( pause )
				ThiefManager.Manager.PauseGame();
			else
				ThiefManager.Manager.UnPauseGame();
		}
		else if(GameManager.Manager.PlayerType == 2)
		{
			HackerNetManager.Manager.PauseGame(pause);
		}
	}
	#endregion

	#region Scrambler
	
	public void RemoveScrambler(int i_hexIndex)
	{
		photonView.RPC("RemoveScramblerRPC", PhotonTargets.All, i_hexIndex);
	}
	
	[PunRPC]
	public void RemoveScramblerRPC(int i_hexIndex)
	{
		HackerNetManager.Manager.RemoveScrambler(i_hexIndex);
	}
	
	#endregion

	public void SetCurrentInfoDataParams( string i_movieName, int playerType )
	{
		photonView.RPC ("SetCurrentInfoDataParamsRPC", PhotonTargets.All, i_movieName, playerType );
	}

	[PunRPC]
	public void SetCurrentInfoDataParamsRPC( string i_movieName, int playerType )
	{
		GameManager.Manager._currentInfoData.m_currentMovie = i_movieName;

		if( playerType == 1 )
		{
			GameManager.Manager._currentInfoData.m_thiefReady = true;
		}
		else
		{
			GameManager.Manager._currentInfoData.m_hackerReady = true;
		}
	}

	public void PlayInfoNodeAudio(string audioName, int playerType = -1)
	{
		photonView.RPC ("PlayInfoNodeAudioRPC", PhotonTargets.All, audioName, playerType);
	}

	[PunRPC]
	public void PlayInfoNodeAudioRPC(string audioName, int playerType = -1)
	{
		AudioSource i_audioSource = GameObject.Find("InfoNodeSource").audio;
		soundMan.soundMgr.playOneShotOnSource(i_audioSource, audioName, playerType);
	}

	public void SilenceInfoNodeSource()
	{
		photonView.RPC("SilenceInfoNodeSourceRPC", PhotonTargets.All);
	}
	
	[PunRPC]
	private void SilenceInfoNodeSourceRPC()
	{
		InfoNodeManager.Manager.SilenceInfoNodeSource();
	}


	#region LevelTransition
	public void PlayerReady(int i_ready)
	{
		photonView.RPC("PlayerReadyRPC", PhotonTargets.All, i_ready);
	}

	[PunRPC]
	private void PlayerReadyRPC(int i_ready)
	{
		GameManager.Manager.m_roleReady = i_ready;
	}
	#endregion

	#region Ping
	
	public void SendPing(Vector3 i_pos)
	{
		photonView.RPC("SendPingRPC", PhotonTargets.All, i_pos);
	}
	#endregion
	
	#region Position Updating Related
	
	public void UpdatePointmanPosition(Transform i_playerTransform)
	{
		
	}
	
	//considering the fact that I'll have to pass a extra int to sync the guard than do it directly, I won't do it for now.
	public void UpdateGuardPosition(Vector3 i_guardPosition, int i_guardID)
	{
		photonView.RPC("UpdateGuardPositionRPC", PhotonTargets.All, i_guardPosition, i_guardID);
	}

	public void UpdateGuardRotation(Quaternion i_guardQuaternion, int i_guardID)
	{
		photonView.RPC("UpdateGuardRotationRPC", PhotonTargets.All, i_guardQuaternion, i_guardID);
	}

	
	#region GuardRelated
	
	[PunRPC]
	public void UpdateGuardPositionRPC(Vector3 i_guardPosition, int i_guardID)
	{
		PointmanNetManager.Manager.UpdateGuardPos(i_guardPosition, i_guardID);
	}

	[PunRPC]
	public void UpdateGuardRotationRPC(Quaternion i_guardQuaternion, int i_guardID)
	{
		PointmanNetManager.Manager.UpdateGuardRot(i_guardQuaternion, i_guardID);
	}
	#endregion
	
	#endregion
	
	#region PowerNode
	
	//pass in the number of the control panel that's supposed to be activated. The number should be coherent with the node number
	public void PowerNode(int PowerNodeID, bool status)
	{
		photonView.RPC("PowerNodeRPC", PhotonTargets.All, PowerNodeID, status);
	}

	#endregion
	
	#region EMP
	
	public void EMPactivated(float i_EmpPos_x, float i_EmpPos_z, float EMP_Influence_Radius)
	{
		photonView.RPC("EMPActivated", PhotonTargets.All, i_EmpPos_x, i_EmpPos_z, EMP_Influence_Radius);
	}
	
	#endregion
	
	
	#region Scrambler
	
	public void ScramblerPlaced(int hexIndex)
	{
		photonView.RPC("ScramblerPlacedRPC",PhotonTargets.All, hexIndex);
	}
	
	[PunRPC]
	public void ScramblerPlacedRPC( int hexIndex )
	{
		HackerNetManager.Manager.ScramblerPlaced( hexIndex );
	}
	#endregion

	#region Pause

	public void ToggleESCMenu()
	{
		photonView.RPC("ToggleESCMenuRPC", PhotonTargets.All);
		PauseGame(true);
	}

	[PunRPC]
	public void ToggleESCMenuRPC()
	{
		if(_hackGUI == null)
			_hackGUI = GameObject.Find("TopDownCamera").GetComponent<HackerGUI>();
		_hackGUI.ToggleESCMenu();
	}

	public void ShutDownESCMenu()
	{
		photonView.RPC("ShutDownESCMenuRPC", PhotonTargets.All);
		PauseGame(false);
	}
	
	[PunRPC]
	public void ShutDownESCMenuRPC()
	{
		if(_hackGUI == null)
			_hackGUI = GameObject.Find("TopDownCamera").GetComponent<HackerGUI>();
		_hackGUI.ShutDownESCMenu();
	}

	public void DisableActions()
	{
		photonView.RPC("DisableActionsRPC", PhotonTargets.All);
	}

	[PunRPC]
	public void DisableActionsRPC()
	{
		if(GameManager.Manager.PlayerType == 1)
		{
			//GameObject.Find("Playertheif(Clone)").GetComponent<MouseLookAround>().enabled = true;
			//Screen.lockCursor = true;
			ThiefManager.Manager.DisableThiefActions();
		}
		else
		{
			//GameObject.Find("TopDownCamera").GetComponent<HackerActions>().EnableHackerActions();
			HackerManager.Manager.DisableHackerActions();
		}
	}

	public void EnableActions()
	{
		photonView.RPC("EnableActionsRPC", PhotonTargets.All);
	}
	
	[PunRPC]
	public void EnableActionsRPC()
	{
		if(GameManager.Manager.PlayerType == 1)
		{
			//GameObject.Find("Playertheif(Clone)").GetComponent<MouseLookAround>().enabled = true;
			//Screen.lockCursor = true;
			ThiefManager.Manager.EnableThiefActions();
		}
		else
		{
			//GameObject.Find("TopDownCamera").GetComponent<HackerActions>().EnableHackerActions();
			HackerManager.Manager.EnableHackerActions();
		}
	}

	public void Silence()
	{
		photonView.RPC("EnableActionsRPC", PhotonTargets.All);
	}
	
	[PunRPC]
	public void SilenceRPC()
	{
		if(GameManager.Manager.PlayerType == 1)
		{
			//GameObject.Find("Playertheif(Clone)").GetComponent<MouseLookAround>().enabled = true;
			//Screen.lockCursor = true;
			ThiefManager.Manager.EnableThiefActions();
		}
		else
		{
			//GameObject.Find("TopDownCamera").GetComponent<HackerActions>().EnableHackerActions();
			HackerManager.Manager.EnableHackerActions();
		}
	}
	
	#endregion
	
	#region InfoNodeRelated
	
	public void InfoNodeHexCaptured( int i_ID, bool i_open )
	{
		photonView.RPC("InfoNodeHexCapturedRPC", PhotonTargets.Others, i_ID, i_open );
	}

	[PunRPC]
	private void InfoNodeHexCapturedRPC( int i_ID, bool i_open )
	{
		PointmanNetManager.Manager.InfoNodePlatformStateChange( i_ID, i_open );
	}

	public void PlayInformation( int i_ID )
	{
		photonView.RPC ( "PlayInformationRPC", PhotonTargets.All, i_ID );
	}

	[PunRPC]
	private void PlayInformationRPC( int i_ID )
	{
		ThiefManager.Manager.PlayInformation(i_ID);
		HackerNetManager.Manager.PlayInformation( i_ID );
	}
	
	#endregion
		
	#region Override
	
	public void InitializeLockDownSequence(int hexIndex)
	{
		photonView.RPC("InitializeLockDownSequenceRPC",PhotonTargets.All, hexIndex);
	}
	
	[PunRPC]
	public void InitializeLockDownSequenceRPC(int hexIndex)
	{
		BasicScoreSystem.Manager.LockdownsNeeded++;
		
		HackerNetManager.Manager.InitializeLockDownSequence(hexIndex);
		
		if( GameManager.Manager.PlayerType == 1 )
		{
			PointmanNetManager.Manager.LockDownSAPs();
			PointmanNetManager.Manager.LockdownEndDoor();
		}
	}
	
	public void SyncOverrideLosingState()
	{
		photonView.RPC("SyncOverrideLosingStateRPC",PhotonTargets.All);
	}
	
	[PunRPC]
	public void SyncOverrideLosingStateRPC()
	{
		OverrideManager.Manager.SyncLose();
	}	
	
	public void SyncOverrideTimer(float i_timer)
	{
		photonView.RPC("SyncOverrideTimerRPC",PhotonTargets.Others, i_timer);
	}
	
	[PunRPC]
	public void SyncOverrideTimerRPC(float i_timer)
	{
		OverrideManager.Manager.SyncTimer(i_timer);
	}
	
	public void OverrideSuccess()
	{
		photonView.RPC("OverrideSuccessRPC",PhotonTargets.All);
	}
	
	[PunRPC]
	public void OverrideSuccessRPC()
	{
		HackerNetManager.Manager.OverrideSuccess();
		
		if( GameManager.Manager.PlayerType == 1 )
		{
			PointmanNetManager.Manager.RestoreSAPs();
			PointmanNetManager.Manager.RestoreEndDoor();
		}
	}
	
	public void EnableOverride()
	{
		photonView.RPC("EnableOverrideRPC",PhotonTargets.All);
	}
	
	[PunRPC]
	public void EnableOverrideRPC()
	{
		HackerNetManager.Manager.EnableOverride();
	}
	
	
	public void DisableOverride()
	{
		photonView.RPC("DisableOverrideRPC",PhotonTargets.All);
	}
	
	[PunRPC]
	public void DisableOverrideRPC()
	{
		HackerNetManager.Manager.DisableOverride();
	}	
	
	#endregion
	#region Transmitter
	
	public void TransmitterPlaced( int hexIndex )
	{
		photonView.RPC("TransmitterPlacedRPC",PhotonTargets.All, hexIndex); // notify Hacker
		HexGrid.Manager.AddHotSpot( hexIndex ); // Add Hotspot for thief
	}
	
	[PunRPC]
	public void TransmitterPlacedRPC( int hexIndex )
	{
		HackerNetManager.Manager.TransmitterPlaced( hexIndex );
	}
	
	public void TransmitterReset( int hexIndex )
	{
		photonView.RPC("TransmitterResetRPC",PhotonTargets.All, hexIndex);
	}
	
	[PunRPC]
	public void TransmitterResetRPC( int hexIndex )
	{
		HackerNetManager.Manager.TransmitterReset( hexIndex );
		PointmanNetManager.Manager.TransmitterReset( hexIndex );
	}
	
	public void TransmitterPickUp( int hexIndex )
	{
		photonView.RPC("TransmitterPickUpRPC",PhotonTargets.All, hexIndex);
	}
	
	[PunRPC]
	public void TransmitterPickUpRPC( int hexIndex )
	{
		HackerNetManager.Manager.TransmitterPickUp( hexIndex );
	}
	
	public void DisableTransmitter( int hexIndex )
	{
		photonView.RPC("DisableTransmitterRPC",PhotonTargets.All, hexIndex);
	}
	
	[PunRPC]
	public void DisableTransmitterRPC( int hexIndex )
	{
		PointmanNetManager.Manager.DisableTransmitter( hexIndex );
	}
	
	#region Tracer Related Functions 
	public void HackerCaught( int hexIndex )
	{
		photonView.RPC("HackerCaughtRPC",PhotonTargets.All, hexIndex);
	}
	
	[PunRPC]
	public void HackerCaughtRPC( int hexIndex )
	{
		PointmanNetManager.Manager.HackerCaught( hexIndex );
	}
	
	
	public void CreateTracer( int i_index, float i_delay, float i_calibration, float i_active)
	{
		photonView.RPC("CreateTracerRPC",PhotonTargets.All, i_index, i_delay, i_calibration, i_active);
	}
	
	[PunRPC]
	public void CreateTracerRPC( int i_index, float i_delay, float i_calibration, float i_active)
	{
		SecurityManager.Manager.CreateTracer(i_index, i_delay, i_calibration, i_active);
	}
	#endregion
	
	
	// Functions for TestingMode
	public void PausingStateChange(bool i_pause)
	{
		photonView.RPC("PausingStateChangeRPC",PhotonTargets.All,i_pause);	
	}
	[PunRPC]
	public void PausingStateChangeRPC(bool i_pause)
	{
		GameObject.Find("TestingModeController").GetComponent<TestingMode>().ChangeToPause(i_pause);
		
	}
	
	public void PausingGame(bool i_pause)
	{
		photonView.RPC("PausingGameRPC",PhotonTargets.All,i_pause);
		
	}
	[PunRPC]
	public void PausingGameRPC(bool i_pause)
	{
		if(i_pause==false) 
		{
			Time.timeScale = 0;
			GameObject.Find("Playertheif(Clone)").GetComponent<MouseLookAround>().enabled = false;
		}
		if(i_pause==true)
		{
			Time.timeScale = 1;
			GameObject.Find("Playertheif(Clone)").GetComponent<MouseLookAround>().enabled = true;
			
		}
		
	}
	public void AllPointsAccessable()
	{
		photonView.RPC("AllPointsAccessableRPC",PhotonTargets.All);
		
	}
	[PunRPC]
	public void AllPointsAccessableRPC()
	{
		HexGrid.Manager.DebugMakeAllAvailable();
	}	
	
	public void DisableDrones()
	{
		photonView.RPC("DisableDronesRPC",PhotonTargets.All);
		
	}
	[PunRPC]
	public void DisableDronesRPC()
	{
		var AllGuard2= GameObject.FindWithTag("Guard");
		AllGuard2.GetComponent<GuardActions>().enabled = false;
		AllGuard2.GetComponent<Perception>().enabled = false;
		AllGuard2.GetComponent<AlertMeter>().enabled = false;
		AllGuard2.GetComponent<Perception>().enabled = false;
		AllGuard2.GetComponent<GuardBehaviourScript>().enabled = false;
		AllGuard2.GetComponent<NavMeshAgent>().enabled = false;
		
		//AllGuard2.GetComponent<GuardSync>.enabled =false;
		
	}
	public void KillAllGuards()
	{
		photonView.RPC("KillAllGuardsRPC",PhotonTargets.All);	
	}
	[PunRPC]
	public void KillAllGuardsRPC()
	{
		var AllGuard = GameObject.FindWithTag("Guard");
		Destroy(AllGuard);	
	}
	public void UnlockAllDoors()
	{
		photonView.RPC("UnlockAllDoorsRPC",PhotonTargets.All);	
	}
	[PunRPC]
	public void UnlockAllDoorsRPC()
	{
		DoorManager.Manager.UnlockAllDoors();
	}
	public void ResetHackerThreatMeter()
	{
		photonView.RPC("ResetHackerThreatMeterRPC",PhotonTargets.All);	
	}
	[PunRPC]
	public void ResetHackerThreatMeterRPC()
	{
		HackerThreat.Manager.ResetThreatLevel();
	}
	public void SetSecurityClearance(int level)
	{
		
		photonView.RPC("DisableAllJammersRPC",PhotonTargets.All,level);
	}
	[PunRPC]
	public void SetSecurityClearanceRPC(int level)
	{
		HackerManager.Manager.HackerClearance = level;
	}
	public void DisableAllJammers()
	{
		
		photonView.RPC("DisableAllJammersRPC",PhotonTargets.All);
	}
	[PunRPC]
	public void DisableAllJammersRPC()
	{	
		//Debug.Log ("Disabling All Jammers");
		HexGrid.Manager.DisableAllJammersTestingMode();
	}
	public void DisableAllTracers()
	{
		
		photonView.RPC("DisableAllTracersRPC",PhotonTargets.All);
	}
	[PunRPC]
	public void DisableAllTracersRPC()
	{
		SecurityManager.Manager.DisableTracer();
		SecurityManager.Manager.DisableTracerCreators();
		//Debug.Log("Disable All the Tracers.");
		
	}
	public void DisableJammer( int i_hexIndex )
	{
		photonView.RPC("DisableJammerRPC",PhotonTargets.All, i_hexIndex);
	}
	
	[PunRPC]
	public void DisableJammerRPC( int i_hexIndex )
	{
		//Debug.Log ("Disabling Jammer on index " + i_hexIndex);
		HexGrid.Manager.DisableJammer( i_hexIndex );
		ThiefGrid.Manager.DisableJammer( i_hexIndex );
	}
	#endregion
	
	#region Pings
	public void HackerPing( int hexIndex )
	{
		photonView.RPC("HackerPingRPC",PhotonTargets.All, hexIndex);
	}
	
	[PunRPC]
	public void HackerPingRPC( int hexIndex )
	{
		PointmanNetManager.Manager.HackerPing( hexIndex );
	}
	
	public void ThiefPing( int hexIndex )
	{
		photonView.RPC("ThiefPingRPC",PhotonTargets.All, hexIndex);
	}
	
	[PunRPC]
	public void ThiefPingRPC( int hexIndex )
	{
		HackerNetManager.Manager.ThiefPing( hexIndex );
		BasicScoreSystem.Manager.PingsSentByTheif += 1;
		PointmanNetManager.Manager.ThiefPing( hexIndex );
	}
	
	public void CreateThiefObjectPing( float rotation, float xScale, float zScale, float xPosition, float zPosition )
	{
		photonView.RPC("CreateThiefObjectPingRPC",PhotonTargets.All, rotation, xScale, zScale, xPosition, zPosition );
	}
	
	[PunRPC]
	public void CreateThiefObjectPingRPC( float rotation, float xScale, float zScale, float xPosition, float zPosition )
	{
		BasicScoreSystem.Manager.ObjectPingsSentByThief += 1;
		HackerNetManager.Manager.CreateThiefObjectPing( rotation, xScale, zScale, xPosition, zPosition );
	}
	#endregion
	
	#region IR
	public void EnableIR(int i_nodeNumber)
	{
		photonView.RPC("EnableIRRPC", PhotonTargets.All, i_nodeNumber);
	}
	
	public void DisableIR(int i_nodeNumber)
	{
		photonView.RPC("DisableIRRPC", PhotonTargets.All, i_nodeNumber);
	}	
	#endregion
	
	#region FloorGrid
	
	public void SetLinkState( int i_index, ThiefGrid.LinkState i_state )
	{
		photonView.RPC("SetLinkStateRPC",PhotonTargets.Others, i_index, (int)i_state);
	}
	
	#endregion
	
	#endregion
	
	
	#region RPC functions related to the public functions
	
	[PunRPC]
	public void PowerNodeRPC(int PowerNodeID, bool status)
	{
		//PointmanNetManager.Manager.PowerNode(PowerNodeID, status);
	}
	
    [PunRPC]
	public void EMPActivated(float i_EmpPos_x, float i_EmpPos_z, float EMP_Influence_Radius)
	{
		//Empty
	}
	
	
	[PunRPC]
	public void SendPingRPC(Vector3 i_pos)
	{
		//PointmanNetManager.Manager.GetPingPosition(i_pos);
	}
	
	#region IR
	[PunRPC]
	public void EnableIRRPC(int i_nodeNumber)
	{
		HackerNetManager.Manager.EnableIR(i_nodeNumber);
	}
	
	[PunRPC]
	public void DisableIRRPC(int i_nodeNumber)
	{
		HackerNetManager.Manager.DisableIR(i_nodeNumber);
	}	
	#endregion
	

	
	#region ConnectionRelated
	[PunRPC]
	public void SendConnectionDataRPC(int i_packedData)
	{
		bool isConnected = (i_packedData & 1) == 1? true: false;
		int index = i_packedData >> 1;
		ConnectionManager.Manager.SetUpConnections(isConnected, index);
	}
	
	#endregion

	public void AddPassword( string i_password )
	{
		photonView.RPC("AddPassowordRPC", PhotonTargets.Others, i_password );
	}

	[PunRPC]
	private void AddPassowordRPC( string i_password )
	{
		HackerNetManager.Manager.AddPassword( i_password );
	}

	public void PasswordPlatformStateChange( int i_hexID, bool i_open )
	{
		photonView.RPC("PasswordPlatformStateChangeRPC", PhotonTargets.Others, i_hexID, i_open );
	}
	
	[PunRPC]
	private void PasswordPlatformStateChangeRPC( int i_hexID, bool i_open )
	{
		PointmanNetManager.Manager.PasswordPlatformStateChange( i_hexID, i_open );
	}

	#endregion
	
	#region FloorGrid

	[PunRPC]
	public void SetLinkStateRPC( int i_index, int i_state )
	{
		PointmanNetManager.Manager.SetLinkState( i_index, (ThiefGrid.LinkState)i_state );
	}

	#endregion
	
	#region Laser
	
	public void ActivateGroup( int i_groupID )
	{
		photonView.RPC("ActivateGroupRPC", PhotonTargets.All, i_groupID);
	}
	
	[PunRPC]
	public void ActivateGroupRPC( int i_groupID )
	{
		PointmanNetManager.Manager.ActivateGroup(i_groupID);
	}
	
	public void DeactivateGroup( int i_groupID )
	{
		photonView.RPC("DeactivateGroupRPC", PhotonTargets.All, i_groupID);
	}
	
	[PunRPC]
	public void DeactivateGroupRPC( int i_groupID )
	{
		PointmanNetManager.Manager.DeactivateGroup(i_groupID);
	}
	
	#endregion
	
	#region HackerThreat/SecurityAccessPanel
	
	public void SetPowerMultiplierForThreatLevel( float i_multiplier )
	{
		photonView.RPC("SetPowerMultiplierForThreatLevelRPC", PhotonTargets.All, i_multiplier);
	}
	
	[PunRPC]
	public void SetPowerMultiplierForThreatLevelRPC( float i_multiplier )
	{
		HackerThreat.Manager.SetPowerMultiplier( i_multiplier );
	}
	
	public void ResetThreatLevel()
	{
		photonView.RPC("ResetThreatLevelRPC", PhotonTargets.All);
	}
	
	[PunRPC]
	public void ResetThreatLevelRPC()
	{
		HackerThreat.Manager.ResetThreatLevel();
	}
	
	public void SetConnectedCount( int i_count )
	{
		photonView.RPC("SetConnectedCountRPC", PhotonTargets.All, i_count);
	}
	
	[PunRPC]
	public void SetConnectedCountRPC( int i_count )
	{
		HackerThreat.Manager.SetConnectedCount(i_count);
	}
	
	public void BoostAlertLevelForTime( float time, float amount )
	{
		photonView.RPC("BoostAlertLevelForTimeRPC", PhotonTargets.All, time, amount);
	}
	
	[PunRPC]
	public void BoostAlertLevelForTimeRPC( float time, float amount )
	{
		HackerThreat.Manager.BoostAlertLevelForTime(time,amount);
	}
	
	public void ModifyThreatRate(float i_rate)
	{
		photonView.RPC("ModifyThreatRateRPC", PhotonTargets.All, i_rate);
	}
	
	[PunRPC]
	public void ModifyThreatRateRPC(float i_rate)
	{
		HackerThreat.Manager.ModifyAlertRate( i_rate );
	}
	
	public void ActivateSecurityPanel() //Drops hacker threat level
	{
		photonView.RPC("DropAlertLevelRPC", PhotonTargets.All);
	}
	
	[PunRPC]
	public void DropAlertLevelRPC()
	{
		HackerThreat.Manager.DecreaseThreatAmount(10);
		BasicScoreSystem.Manager.NoOfSAPsActivated++;
	}

	public void SetLevelCompletionTime(float i_time) //Set level completion time on both player's BasicScorindSystem
	{
		photonView.RPC("SetLevelCompletionTimeRPC", PhotonTargets.All, i_time);
	}
	
	[PunRPC]
	public void SetLevelCompletionTimeRPC(float i_time)
	{
		BasicScoreSystem.Manager.LevelCompleteTime = (int) i_time;
	}

	public void SetTransmittersPlaced(int i_number) //Set transmitters placed on both player's BasicScorindSystem
	{
		photonView.RPC("SetTransmittersPlacedRPC", PhotonTargets.All, i_number);
	}
	
	[PunRPC]
	public void SetTransmittersPlacedRPC(int i_number)
	{
		BasicScoreSystem.Manager.TransmittersPlaced += i_number;
	}

	public void SetTimesSeenByGuard(int i_number) //Set times seen by guard on both player's BasicScorindSystem
	{
		photonView.RPC("SetTimesSeenByGuardRPC", PhotonTargets.All, i_number);
	}
	
	[PunRPC]
	public void SetTimesSeenByGuardRPC(int i_number)
	{
		BasicScoreSystem.Manager.TimesSeen += i_number;
	}

	public void SetTimesChasedByGuard(int i_number) //Set times seen by guard on both player's BasicScorindSystem
	{
		photonView.RPC("SetTimesChasedByGuardRPC", PhotonTargets.All, i_number);
	}
	
	[PunRPC]
	public void SetTimesChasedByGuardRPC(int i_number)
	{
		BasicScoreSystem.Manager.TimesChased += i_number;
	}

	public void SetTimesCaughtByTracer(int i_number) //SetTimesCaughtByTracer on both player's BasicScorindSystem
	{
		photonView.RPC("SetTimesCaughtByTracerRPC", PhotonTargets.All, i_number);
	}
	
	[PunRPC]
	public void SetTimesCaughtByTracerRPC(int i_number)
	{
		BasicScoreSystem.Manager.HackerCaughtByTracer += i_number;
	}

	public void DecreaseThreatAmount( int i_amount ) //Drops hacker threat level
	{
		photonView.RPC("DecreaseThreatAmountRPC", PhotonTargets.All, i_amount);
	}
	
	[PunRPC]
	public void DecreaseThreatAmountRPC( int i_amount )
	{
		HackerThreat.Manager.DecreaseThreatAmount( i_amount );
	}
	
	public void IncreaseThreatAmount( int i_amount ) //Drops hacker threat level
	{
		photonView.RPC("IncreaseThreatAmountRPC", PhotonTargets.All, i_amount);
	}
	
	[PunRPC]
	public void IncreaseThreatAmountRPC( int i_amount )
	{
		HackerThreat.Manager.IncreaseThreatAmount( i_amount );
	}
	
	#endregion
	
	public void SetPasswordHacker( string i_password )
	{
		photonView.RPC ( "SetPasswordHackerRPC", PhotonTargets.Others, i_password );
	}
	
	[PunRPC]
	private void SetPasswordHackerRPC( string i_password )
	{
		HackerNetManager.Manager.SetPasswordHacker( i_password );
	}
	 
	#region EMAIL stuff
	
	public void SendGameInfoAsEmail( string i_receipnts, string i_subject )
	{
		photonView.RPC ( "SendGameInfoAsEmailRPC", PhotonTargets.All, i_receipnts, i_subject );
	}

	private string GetAnalytics()
	{
		string body =  "The game is being played right now. \nSystem info: \n" + SystemInfo.deviceName + "\n" + SystemInfo.deviceType + "\n"+ SystemInfo.operatingSystem +"\nNetwork data: \n";
			
			string playerType;
			if( GameManager.Manager.PlayerType == 1 )
				playerType = " Thief.";
			else
				playerType = " Hacker.";
			
			if( PhotonNetwork.isMasterClient )
				body += "This is the server\n";
			else
				body += "This is the client\n";
			
			foreach ( PhotonPlayer conn in PhotonNetwork.playerList )
			{
				//body += "IP: " + conn. + " Port: " + conn.port + "\n";
			}
			
			body += "Game Info: \nPlayer Type:" + playerType + "\nLevel:" + Application.loadedLevelName ;
		
		return body;
	}
	
	[PunRPC]
	private void SendGameInfoAsEmailRPC( string i_receipnts, string i_subject )
	{	
		EmailClient.Manager.SendEmail(i_receipnts, i_subject, GetAnalytics() );
	}

	public void KillTracers()
	{
		photonView.RPC ( "KillTracersRPC", PhotonTargets.Others );
	}

	[PunRPC]
	public void KillTracersRPC()
	{
		SecurityManager.Manager.DisableTracerCreators();
		SecurityManager.Manager.GetLiveTracers().Clear();

		GameObject[] Tracers = GameObject.FindGameObjectsWithTag("Tracer");
		
		foreach(GameObject tracer in Tracers)
			Destroy( tracer );
	}

	public void ExposeHexGrid()
	{
		photonView.RPC ( "ExposeHexGridRPC", PhotonTargets.Others );
	}

	[PunRPC]
	public void ExposeHexGridRPC()
	{
		HexGrid.Manager.ExposeHexGrid();
	}

	public void DeleteGuards()
	{
		photonView.RPC ( "DeleteGuardsRPC", PhotonTargets.All );
	}

	[PunRPC]
	public void DeleteGuardsRPC()
	{
		GuardOverlord.Manager.m_Guards.Clear();
		
		GameObject[] guards = GameObject.FindGameObjectsWithTag("Guard");
		
		foreach(GameObject guard in guards)
		{
			GameObject newGuard = Resources.Load("Prefabs/Theif/Guard_Inactive") as GameObject;
			newGuard = (GameObject)GameObject.Instantiate(newGuard, guard.transform.position, guard.transform.rotation) as GameObject;
		}
		
		foreach(GameObject guard in guards)
		{
			GameObject.Destroy(guard);
		}

		if( GameManager.Manager.PlayerType == 2 )
		{
			GameObject[] guards2D = GameObject.FindGameObjectsWithTag("Guard2D");

			foreach(GameObject guard in guards2D)
			{
				GameObject.Destroy(guard);
			}
		}
	}

	public void ShowEndScreen(int m_type)
	{
		photonView.RPC ( "ShowEndScreenRPC", PhotonTargets.All, m_type );
	}
	
	[PunRPC]
	public void ShowEndScreenRPC(int m_type)
	{
		GameObject.Find("MessageBox").GetComponent<MessageBox>().MessageBoxShow(m_type);
	}
	
	public void CloseInfoVideoAudio()
	{
		photonView.RPC ( "CloseInfoVideoAudioRPC", PhotonTargets.All );
	}
	
	[PunRPC]
	public void CloseInfoVideoAudioRPC()
	{
		InfoNodeManager.Manager.CloseInfoVideoAudio();
	}

	public void DisableThreat()
	{
		photonView.RPC ( "DisableThreatRPC", PhotonTargets.All );
	}

	[PunRPC]
	public void DisableThreatRPC()
	{
		HackerThreat.Manager.ResetThreatLevel();
		HackerThreat.Manager._threatDisabled = true;
	}

	public void PlayBackgroundMusic()
	{
		photonView.RPC ( "PlayBackgroundMusicRPC", PhotonTargets.All );
	}

	[PunRPC]
	public void PlayBackgroundMusicRPC()
	{
		soundMan.soundMgr.silenceSource(null,GameManager.Manager.PlayerType);

		if( GameManager.Manager.PlayerType == 1 )
		{
			soundMan.soundMgr.playOnSource(null,"Theif_BGM_loop",true,GameManager.Manager.PlayerType,2,0.6f);
		}
		else if( GameManager.Manager.PlayerType == 2 )
		{
			soundMan.soundMgr.playOnSource(null,"Hacker_BGM_loop",true,GameManager.Manager.PlayerType,2,0.6f);
		}
	}

	public void LastMenuAction(bool playClicked, bool levelClicked, bool exitClicked)
	{
		photonView.RPC ( "LastMenuActionRPC", PhotonTargets.All, playClicked, levelClicked, exitClicked );
	}

	[PunRPC]
	public void LastMenuActionRPC(bool playClicked, bool levelClicked, bool exitClicked)
	{
		GameManager.Manager.playClicked = playClicked;
		GameManager.Manager.levelClicked = levelClicked;
		GameManager.Manager.exitClicked = exitClicked;
	}

	public void OnFinalVideoEnd()
	{
		photonView.RPC ( "OnFinalVideoEndRPC", PhotonTargets.All );
	}

	[PunRPC]
	public void OnFinalVideoEndRPC()
	{
		/*** Kill Tracers ***/
		NetworkManager.Manager.KillTracersRPC(); 
		
		/*** Expose Hex Grid To Hacker ***/
		NetworkManager.Manager.ExposeHexGridRPC();
		
		/*** Kill Guards ***/
		NetworkManager.Manager.DeleteGuardsRPC();
		
		/*** Disable Threat ***/
		NetworkManager.Manager.DisableThreatRPC();
		
		/*** Play Normal Background Music ***/
		NetworkManager.Manager.PlayBackgroundMusicRPC();
		//}
	}

	#endregion

	void Update()
	{
		if( Input.GetKeyDown(KeyCode.F3) )
		{
			LoadLevel(Application.loadedLevel);
		}
	}
	
}
