using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Steamworks;

public enum ConnectionStatus
{
	None,
	Connecting,
	Failed,
	RoomFull,
}

public struct SteamLobby
{
	public CSteamID m_LobbyID;
	public string   m_LobbyName;
	public CSteamID m_LobbyOwner;
	public string 	m_LobbyOwnerIPAddress;
	public string 	m_LobbyInfo;
}

public class GameLobbyInterface : MonoBehaviour {
	
	private bool _showGameLobby = false;
	private Rect windowRect = new Rect(Screen.width/2, Screen.height/2, 600, 400);
	private StartGameFlow _flow;
	private GameObject _camera;
	private GameObject _playerUtil;
	private string _localIP = "127.0.0.1";
	private string _serverName;
	private string _receivedLANInfo;
	private string _gameroomName = "";
	
	private Texture2D _gameLobbyScreenBackground;
	private Texture2D _createNewGameButton;
	private Texture2D _createNewGameButtonActive;
	private Texture2D _createLocalGameButton;
	private Texture2D _createLocalGameButtonActive;
	private Texture2D _joinButton;
	private Texture2D _joinButtonActive;
	private Texture2D _refreshButtonActive;
	private Texture2D _refreshButtonNormal;
	private Texture2D _genericMessageBox;
	private Texture2D _closeMessageBox;


	private Texture2D _mainWindow;
	private Texture2D _scrollUpArrow;
	private Texture2D _scrollUpDisable;
	private Texture2D _scrollDownArrow;
	private Texture2D _scroolDownDisable;
	private Texture2D _sideWindow;
	private Texture2D _internetNotice;
	private Texture2D _lanNotice;
	private Texture2D _mainMenuButton;
	private Texture2D _mainMenuButtonActive;

	// Steam Callbacks
	protected Callback<LobbyGameCreated_t> m_LobbyGameCreated;
	protected Callback<LobbyEnter_t> m_LobbyEnteredCallback;

	// Steam Call Results
	private CallResult<LobbyCreated_t> OnLobbyCreatedCallResult;
	private CallResult<LobbyMatchList_t> OnLobbyMatchListCallResult;

	private List<SteamLobby> _activeSteamLobbies = new List<SteamLobby>();

	private HostData[] 		_hostData;
	private List<HostData>  _filteredhostData = new List<HostData>();
	private List<LANData> 	_lanData = new List<LANData>();
	private List<LANData>   _filteredLANData = new List<LANData>();

	private ConnectionStatus _connStatus = ConnectionStatus.None;
	private string _failedMessage = "";

	#region GUISettings
	private const int MenuStartPosition_Y = 13;
	private const int ListStartPosition_Y = 6;
	private const int ListInterval_Y = 2;
	private const float JoinButtonOffset = 0.5f;
	private const int GameNameStartPosition_X = 24;
	private const int GameNameWidth = 12;
	private const int LocationStartPosition_X = 46;
	private const int LocationWidth = 14;
	private const int JoinButtonWidth = 8;
	private const int RoleStartPosition_X = 38;
	private const int RefreshButtonPosition_X = 48;
	private const int RoleWidth = 6;
	private const int JoinButtonStartPosition_X = 48;
	
	private const int GameCapacity = 10;
	private int _currentGameIndex = 0;
	private int _totalGames = 0;
	private int _serverGames = 0;
	private int _LANGames = 0;
	private float _RefreshInterval = 0.5f;
	private float _currentTimer = 0.0f;
	private float _constantTimer = 0.0f;
	private bool _showIPs = false;
	private bool _customJoin = false;
	private bool _customCreate = false;
	private string _customIP = "127.0.0.1";
	private string _customSPort = "25566";
	private int _customPort = 0;
	private bool _menuActive = true;
	
	#endregion

	// Use this for initialization
	void Start () 
	{
		// Create Steam Callbacks
		m_LobbyGameCreated = Callback<LobbyGameCreated_t>.Create(OnLobbyGameCreated);
		m_LobbyEnteredCallback = Callback<LobbyEnter_t>.Create (OnLobbyEntered);

		// Create Steam Call Results
		OnLobbyCreatedCallResult = CallResult<LobbyCreated_t>.Create(OnLobbyCreated);
		OnLobbyMatchListCallResult = CallResult<LobbyMatchList_t>.Create (OnLobbyMatchListReceived);

		_playerUtil = GameObject.Find("PlayerUtil");
		_camera = GameObject.Find("TopDownCamera");
		_flow = gameObject.GetComponent<StartGameFlow>();
		_gameLobbyScreenBackground 	= Resources.Load("Textures/LevelSelect/Background", typeof(Texture2D)) as Texture2D;
		_createNewGameButton 		= Resources.Load("Textures/NewGameLobby/btn_internetGame_norm", typeof(Texture2D)) as Texture2D;
		_createNewGameButtonActive 	= Resources.Load("Textures/NewGameLobby/btn_internetGame_active", typeof(Texture2D)) as Texture2D;
		_createLocalGameButton		= Resources.Load("Textures/NewGameLobby/btn_LANGame_norm", typeof(Texture2D)) as Texture2D;
		_createLocalGameButtonActive= Resources.Load("Textures/NewGameLobby/btn_LANGame_active", typeof(Texture2D)) as Texture2D;
		_joinButton 				= Resources.Load("Textures/NewGameLobby/btn_LineItem_norm", typeof(Texture2D)) as Texture2D;
		_joinButtonActive 			= Resources.Load("Textures/NewGameLobby/btn_LineItem_active", typeof(Texture2D)) as Texture2D;
		_refreshButtonActive 		= Resources.Load("Textures/NewGameLobby/btn_Refresh_norm", typeof(Texture2D)) as Texture2D;
		_refreshButtonNormal		= Resources.Load("Textures/NewGameLobby/btn_Refresh_norm", typeof(Texture2D)) as Texture2D;
		_genericMessageBox			= Resources.Load("Textures/StartScreenUI/GenericMessageBox", typeof(Texture2D)) as Texture2D;
		_closeMessageBox			= Resources.Load("Textures/StartScreenUI/RemoveMessageBtn", typeof(Texture2D)) as Texture2D;

		_mainWindow					= Resources.Load("Textures/NewGameLobby/GamesListWindow", typeof(Texture2D)) as Texture2D;
		_scrollUpArrow				= Resources.Load("Textures/NewGameLobby/btn_ScrollUp_norm", typeof(Texture2D)) as Texture2D;
		_scrollUpDisable			= Resources.Load("Textures/NewGameLobby/btn_ScrollUp_disable", typeof(Texture2D)) as Texture2D;
		_scrollDownArrow			= Resources.Load("Textures/NewGameLobby/btn_ScrollDown_norm", typeof(Texture2D)) as Texture2D;
		_scroolDownDisable			= Resources.Load("Textures/NewGameLobby/btn_ScrollDown_disable", typeof(Texture2D)) as Texture2D;
		_sideWindow					= Resources.Load("Textures/NewGameLobby/SideWindow", typeof(Texture2D)) as Texture2D;
		_internetNotice				= Resources.Load("Textures/NewGameLobby/Internet_Notice", typeof(Texture2D)) as Texture2D;
		_lanNotice					= Resources.Load("Textures/NewGameLobby/LAN_Notice", typeof(Texture2D)) as Texture2D;
		_mainMenuButton				= Resources.Load("Textures/NewGameLobby/btn_MainMenu_norm", typeof(Texture2D)) as Texture2D;
		_mainMenuButtonActive		= Resources.Load("Textures/NewGameLobby/btn_MainMenu_active", typeof(Texture2D)) as Texture2D;
	}
	
	// Update is called once per frame
	void Update () {

		if(_showGameLobby)
		{
			if(Input.GetKeyDown(KeyCode.F1))
			{	
				_showIPs = !_showIPs;
			}

			if(Input.GetKeyDown(KeyCode.F2))
			{	
				_customJoin = true;
			}

			if(Input.GetKeyDown(KeyCode.F4))
			{
				_customCreate = true;
			}
		}

		if(_currentTimer > _RefreshInterval)
		{
			if(_flow.IsInGameLobby)
			{
				_receivedLANInfo = _flow.LANReceiveRoomInfo();
				if(_receivedLANInfo != " ")
					ProcessLANInfo(_receivedLANInfo);
				FilterHostList();
			}
			_currentTimer = 0.0f;
		}
		else
		{
			_currentTimer += Time.deltaTime;
		}

		_constantTimer += Time.deltaTime;

		if(_totalGames > GameCapacity)
		{
			if ( (_totalGames- _currentGameIndex > GameCapacity) && Input.GetAxis("Mouse ScrollWheel") > 0 )
			{
				_currentGameIndex++;
				if(_currentGameIndex > _totalGames -1)
				{
					_currentGameIndex = _totalGames - 1;
				}
			}
			else if ( Input.GetAxis("Mouse ScrollWheel") < 0 )
			{
				_currentGameIndex--;
				if(_currentGameIndex < 0)
				{
					_currentGameIndex = 0;
				}
			}
		}
	}
	
	public void ClearLANData()
	{
		_lanData.Clear();
		_filteredLANData.Clear();
	}
	
	private void ProcessLANInfo(string i_LANInfo)
	{
		string[] IPRoleAndScore = i_LANInfo.Split("#".ToCharArray());
		LANData data = new LANData();
		
		data.ip = 			IPRoleAndScore[0];
		data.gameName = 	IPRoleAndScore[1];
		data.roleNeeded = 	IPRoleAndScore[2];
		data.mapName = 		IPRoleAndScore[3];
		data.flag = 		IPRoleAndScore[4];
		data.roomFull = 	IPRoleAndScore[5];
		
		
		if(data.flag == "true")
		{
			bool hasThisData = false;
			for(int i = 0; i < _lanData.Count; i++)
			{
				if(_lanData[i].ip == data.ip)
				{
					hasThisData = true;
					_lanData[i] = data;
					break;
				}
			}
			//foreach(LANData idata in _lanData)
			//{
			//	if(idata.ip == data.ip)
			//	{
			//		hasThisData = true;
			//		idata = data;
			//	}
			//}
			
			if(!hasThisData)
				_lanData.Add(data);
		}
		else if(data.flag == "false")
		{
			foreach(LANData idata in _lanData)
			{
				if(idata.ip == data.ip)
				{
					_lanData.Remove(idata);
					break;
				}
			}
		}
		else
		{
			//Debug.Log("#Max the data has an invalid flag");
		}
	}
	
	public void ShutDown()
	{
		_showGameLobby = false;
		_connStatus = ConnectionStatus.None;
		ClearLANData();
		_flow.EnableStartMenu();
	}
	
	public void Show()
	{
		_showGameLobby = true;
		_menuActive = true;
		if (VirtualKeyboard.enabled == true)
			VirtualKeyboard.text = "";
		_constantTimer =0.0f;
		//_flow.DisableStartMenu();
	}
	
	private void OnGUI()
	{
		if(_showGameLobby)
		{
			GameLobbyInterfaceLoop();
			ScrollStuff();
		}
		if(_customJoin)
		{
			GUI.Window(2, new Rect(Screen.width/2 - 100, Screen.height/2 - 100, 200, 200), JoinGameLoop, "Custom Join Game");
		}

		if(_customCreate)
		{
			GUI.Window(2, new Rect(Screen.width/2 - 100, Screen.height/2 - 100, 200, 200), CreateGameLoop, "Custom Create Game");
		}
	}

	private void ScrollStuff()
	{
		if(_totalGames > GameCapacity)
		{
			if(_currentGameIndex == 0)
			{
				ScreenHelper.DrawTexture(61, 5, 2, 4, _scrollUpDisable);
			}
			else
			{
				if(ScreenHelper.DrawButton(61, 5, 2, 4, _scrollUpArrow))
				{
					_currentGameIndex--;
					if(_currentGameIndex < 0)
					{
						_currentGameIndex = 0;
					}
				}
			}
			if(_currentGameIndex == _totalGames - 1)
			{
				ScreenHelper.DrawTexture(61, 29, 2, 4, _scroolDownDisable);
			}
			else
			{
				if(ScreenHelper.DrawButton(61, 29, 2, 4, _scrollDownArrow))
				{
					_currentGameIndex++;
					if(_currentGameIndex > _totalGames -1)
					{
						_currentGameIndex = _totalGames - 1;
					}
				}
			}
		}
	}

	private void JoinGameLoop(int i_id)
	{
		GUILayout.BeginVertical();
		GUILayout.BeginHorizontal();
		GUILayout.Label("IP ADDRESS");
		_customIP = GUILayout.TextField(_customIP);
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUILayout.Label("PORT");
		_customSPort = GUILayout.TextField(_customSPort);
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		if(GUILayout.Button("Cancel"))
		{
			_customJoin =false;
		}
		if(GUILayout.Button("Connect"))
		{
			_connStatus = ConnectionStatus.Connecting;
			_customPort = int.Parse(_customSPort);
			Network.Connect(_customIP, _customPort);
			_customJoin =false;
		}
		GUILayout.EndHorizontal();
		GUILayout.EndVertical();
	}

	private void CreateGameLoop(int i_id)
	{
		GUILayout.BeginVertical();
		GUILayout.BeginHorizontal();
		GUILayout.Label("PORT");
		_customSPort = GUILayout.TextField(_customSPort);
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		if(GUILayout.Button("Cancel"))
		{
			_customCreate =false;
		}
		if(GUILayout.Button("Create"))
		{
			_flow.IsLANGame = false;
			_customCreate =false;
			//{
			_customPort = int.Parse(_customSPort);
			Network.InitializeSecurity();
			Network.InitializeServer(1, _customPort, !Network.HavePublicAddress());
			
			//Debug.Log("#Max: " + _playerUtil.GetComponent<AccountSystem>().GetName() + "is trying to reg host");
			_serverName = _playerUtil.GetComponent<AccountSystem>().GetName() + "'s Game";		
			MasterServer.RegisterHost("CH", _serverName, "Undecided#Undecided");
			//}
			ShutDown();
			_flow.ShowGameRoom();
		}
		GUILayout.EndHorizontal();
		GUILayout.EndVertical();
	}
	
	private void GameLobbyInterfaceLoop()
	{
		ScreenHelper.DrawTexture(0, 0, 64, 36, _gameLobbyScreenBackground);
		//ScreenHelper.SlideInTexture(-64, 0, 0, 0, 64, 36, _gameLobbyScreenBackground, _constantTimer, 1, 0, 0);
		//ScreenHelper.DrawTexture(0, 0, 64, 36, _gameLobbyScreenBackground);
		//ScreenHelper.DrawTexture(23, 7, 4, 2, _flow.AccentLeft);
		//ScreenHelper.DrawTexture(37, 7, 4, 2, _flow.AccentRight);
		ScreenHelper.SlideInTexture(-16, 13, 0, 13, 16, 8, _internetNotice, _constantTimer, 0.5f, 0, 0);
		ScreenHelper.SlideInTexture(-16, 23, 0, 23, 16, 8, _lanNotice, _constantTimer, 0.5f, 0, 0);
		ScreenHelper.SlideInTexture(2, -13, 2, 0, 16, 12, _sideWindow, _constantTimer, 0.5f, 0, 0);
		ScreenHelper.SlideInTexture(64, 0, 22, 0, 42, 36, _mainWindow, _constantTimer, 0.5f, 0, 0);

		if(ScreenHelper.SlideInButton(-22, 27, 0, 27, 22, 3, _createLocalGameButton, _constantTimer, 0.5f, 0, 0) && Network.peerType == NetworkPeerType.Disconnected)
		{
			_flow.IsLANGame = true;
			Network.InitializeServer(1, 25566, false);
			_serverName = _playerUtil.GetComponent<AccountSystem>().GetName() + "'s Game";
			_flow.LANBroadcastRoomMessage(_serverName + "#Hacker#Welcome to CERTA!#true#false");
			_flow.ShowGameRoom();
		}
		
		if(ScreenHelper.SlideInButton(-22, 17, 0, 17, 22, 3, _createNewGameButton, _constantTimer, 0.5f, 0, 0) && Network.peerType == NetworkPeerType.Disconnected)
		{
			// Initialize the Server
			//if(_flow.IsALocalGame())
			//{
			//	Network.InitializeServer(2, 25566, false);
			//}
			_flow.IsLANGame = false;
			//{
			
			Network.InitializeSecurity();
			Network.InitializeServer(1, 25566, !Network.HavePublicAddress());

			//Debug.Log("#Max: " + _playerUtil.GetComponent<AccountSystem>().GetName() + "is trying to reg host");
			_serverName = _playerUtil.GetComponent<AccountSystem>().GetName() + "'s Game";		
			//MasterServer.RegisterHost("CH", _serverName, "Undecided#Undecided");
			//}

			if(SteamManager.Initialized)
			{
				SteamAPICall_t handle = SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic, 2);
				OnLobbyCreatedCallResult.Set(handle);
			}

			ShutDown();
			_flow.ShowGameRoom();
		}
		
		//ScreenHelper.DrawTexture(23, 32, 4, 2, _flow.AccentLeft);
		//ScreenHelper.DrawTexture(37, 32, 4, 2, _flow.AccentRight);
		if(ScreenHelper.SlideInButton(-18, 32, 0, 32, 18, 3, _mainMenuButtonActive, _mainMenuButton, _constantTimer, 0.5f, 0.0f, 0.0f))
		{
			_flow.ShowLoginInterface();
		}

		GameListInterface();
		ShowConnectionStatus();
		//windowRect = GUI.Window(0, windowRect, gameListFunc, "Game Lobby");
	}

	private void ShowConnectionStatus()
	{
		if(_connStatus == ConnectionStatus.Connecting)
		{
			ScreenHelper.DrawTexture(16, 12, 32, 8, _genericMessageBox);
			ScreenHelper.DrawText(16, 12, 32, 8, "Connecting...", 60, Color.cyan, TextAnchor.MiddleCenter, true);
		}
		else if(_connStatus == ConnectionStatus.Failed)
		{
			ScreenHelper.DrawTexture(16, 12, 32, 8, _genericMessageBox);
			ScreenHelper.DrawText(16, 12, 32, 8, "Connection Failed: " + _failedMessage, 60, Color.cyan, TextAnchor.MiddleCenter, true);
			if(ScreenHelper.DrawButton(46, 12, 2, 2, _closeMessageBox))
			{
				_failedMessage = "";
				_connStatus = ConnectionStatus.None;
				_menuActive = true;
			}
		}
		else if(_connStatus == ConnectionStatus.RoomFull)
		{
			ScreenHelper.DrawTexture(16, 12, 32, 8, _genericMessageBox);
			ScreenHelper.DrawText(16, 12, 32, 8, "Room Full", 60, Color.cyan, TextAnchor.MiddleCenter, true);
			if(ScreenHelper.DrawButton(46, 12, 2, 2, _closeMessageBox))
			{
				_failedMessage = "";
				_connStatus = ConnectionStatus.None;
				_menuActive = true;
			}
		}
	}

	void OnFailedToConnect(NetworkConnectionError error) 
	{
		_connStatus = ConnectionStatus.Failed;
		//Debug.Log("Could not connect to server: " + error);
		_failedMessage = error.ToString();
	}
	
	private void FilterHostList()
	{
		//MasterServer.RequestHostList("CH");
		//_filteredLANData.Clear();
		//if(_lanData.Count != 0)
		//{
		//	foreach(LANData ldata in _lanData)
		//	{
		//		if(CheckingNames(ldata.gameName))
		//		{
		//			_filteredLANData.Add(ldata);
		//		}
		//	}
		//}

		if (SteamManager.Initialized) 
		{
			SteamAPICall_t handle = SteamMatchmaking.RequestLobbyList ();
			OnLobbyMatchListCallResult.Set(handle);
		}

		//_hostData = MasterServer.PollHostList();
		//_filteredhostData.Clear();
		//if(_hostData.Length != 0)
		//{
		//	foreach(HostData hdata in _hostData)
		//	{
		//		if(CheckingNames(hdata.gameName))
		//		{
		//			_filteredhostData.Add(hdata);
		//		}
		//	}
		//}                                          
	}
	
	private bool CheckingNames(string i_name)
	{
		if(i_name == "") return true;
		string temp = i_name.ToLower();
		if(temp.Contains(_gameroomName.ToLower())) return true;
		return false;
	}
	
	private void GameListInterface()
	{
		//ScreenHelper.DrawGreenTitleText(GameNameStartPosition_X, MenuStartPosition_Y, GameNameWidth, 2, "Game Name");
		//ScreenHelper.DrawGreenTitleText(LocationStartPosition_X, MenuStartPosition_Y, LocationWidth, 2, "Location");
		//ScreenHelper.DrawGreenTitleText(RoleStartPosition_X, MenuStartPosition_Y, RoleWidth, 2, "Role Needed");

		if (VirtualKeyboard.enabled == true)
			_gameroomName = VirtualKeyboard.text;
		_gameroomName = ScreenHelper.DrawTextField(47, 1, 13, 1, _gameroomName, 30);
		
		if(ScreenHelper.DrawButton(48, 33, 8, 2, _refreshButtonActive, _refreshButtonNormal))
		{
			MasterServer.RequestHostList("CH");
		}
		//Debug.Log("I'm polling host list... please don't show up while playing games");
		
		//_LANGames = _lanData.Count;
		//_serverGames =  _hostData.Length;
		
		
		//_LANGames = _filteredLANData.Count;
		_LANGames = 0;
		_serverGames = _activeSteamLobbies.Count;
		
		_totalGames = _LANGames + _serverGames;
		
		if(_totalGames != 0)
		{
			if(_totalGames <= GameCapacity)
			{
				ShowGameList(_totalGames, _LANGames, _serverGames, 0);
			}
			else
			{
				ShowGameList(GameCapacity, _LANGames, _serverGames, _currentGameIndex);
			}
		}
	}
	
	private void ShowGameList(int i_numberOfGamesToShow, int i_lanGameNumber, int i_serverGameNumber, int i_startIndex)
	{
		for(int i = 0; i < i_numberOfGamesToShow; i++)
		{
			if(i_startIndex + i < i_lanGameNumber)
			{
				//_lanData
				//Debug.Log("Local: " + (i + i_startIndex).ToString());
				if(_menuActive)
				{
					if(ScreenHelper.SlideInButton(65, ListStartPosition_Y + i * ListInterval_Y, 23, ListStartPosition_Y + i * ListInterval_Y, 39, 2, _joinButtonActive, _joinButton, _constantTimer, 0.5f, 0, 0))
					{
						//if(_filteredLANData[i + i_startIndex].roomFull == "false")
						//{
						//	_connStatus = ConnectionStatus.Connecting;
						//	Network.Connect(_lanData[i + i_startIndex].ip, 25566);
						//}
						//else
						//{
						//	_connStatus = ConnectionStatus.RoomFull;
						//}

						if(i + i_startIndex < _activeSteamLobbies.Count)
						{
							_connStatus = ConnectionStatus.Connecting;
							SteamAPICall_t handle = SteamMatchmaking.JoinLobby(_activeSteamLobbies[i + i_startIndex].m_LobbyID);
							Network.Connect(_activeSteamLobbies[i + i_startIndex].m_LobbyOwnerIPAddress, 25566);
						}
						else
						{
							_connStatus = ConnectionStatus.Failed;
						}

						_menuActive = false;
					}
				}
				else
				{
					ScreenHelper.SlideInTexture(65, ListStartPosition_Y + i * ListInterval_Y, 23, ListStartPosition_Y + i * ListInterval_Y, 39, 2, _joinButton, _constantTimer, 0.5f, 0, 0);
				}

				ScreenHelper.SlideInText(66, ListStartPosition_Y + i * ListInterval_Y, GameNameStartPosition_X, ListStartPosition_Y + i * ListInterval_Y, GameNameWidth, 2,
				                         _filteredLANData[i + i_startIndex].gameName,
				                         _constantTimer, 0.5f, 0, 0);
				ScreenHelper.SlideInText(150, ListStartPosition_Y + i * ListInterval_Y, LocationStartPosition_X, ListStartPosition_Y + i * ListInterval_Y, LocationWidth, 2,
				                         _filteredLANData[i + i_startIndex].mapName,
				                         _constantTimer, 0.5f, 0, 0);
				ScreenHelper.SlideInText(88, ListStartPosition_Y + i * ListInterval_Y, RoleStartPosition_X, ListStartPosition_Y + i * ListInterval_Y, RoleWidth, 2,
				                         _filteredLANData[i + i_startIndex].roleNeeded,
				                         _constantTimer, 0.5f, 0, 0);
//				ScreenHelper.DrawGameNameText(GameNameStartPosition_X, ListStartPosition_Y + i * ListInterval_Y, GameNameWidth, 2, 
//				                              _filteredLANData[i + i_startIndex].gameName);
//				ScreenHelper.DrawGameListText(LocationStartPosition_X, ListStartPosition_Y + i * ListInterval_Y, LocationWidth, 2,
//				                              _filteredLANData[i + i_startIndex].mapName);
//				ScreenHelper.DrawGameListText(RoleStartPosition_X, ListStartPosition_Y + i * ListInterval_Y, LocationWidth, 2,
//				                              _filteredLANData[i + i_startIndex].roleNeeded);

				//if(ScreenHelper.D)
//
//				if(_filteredLANData[i + i_startIndex].roomFull == "false")
//				{
//					if(ScreenHelper.DrawButton(JoinButtonStartPosition_X, (float)ListStartPosition_Y + (float)(i * ListInterval_Y) - JoinButtonOffset, JoinButtonWidth,
//					                           2, _joinButton))
//					{
//						_connStatus = ConnectionStatus.Connecting;
//						Network.Connect(_lanData[i + i_startIndex].ip, 25566);
//					}
//				}
//				else if(_filteredLANData[i + i_startIndex].roomFull == "true")
//				{
//					ScreenHelper.DrawGameListText(JoinButtonStartPosition_X, ListStartPosition_Y + i * ListInterval_Y, JoinButtonWidth, 1,
//					                              "RoomFull");
//				}
			}
			else
			{
				if(_menuActive)
				{
					if(ScreenHelper.SlideInButton(65, ListStartPosition_Y + i * ListInterval_Y, 23, ListStartPosition_Y + i * ListInterval_Y, 39, 2, _joinButtonActive, _joinButton, _constantTimer, 0.5f, 0, 0))
					{
						//if(_filteredhostData[i + i_startIndex - i_lanGameNumber].connectedPlayers <= 1)
						//{
						//	_connStatus = ConnectionStatus.Connecting;
						//	Network.Connect(_filteredhostData[i + i_startIndex - i_lanGameNumber]);
						//}
						//else
						//{
						//	_connStatus = ConnectionStatus.RoomFull;
						//}

						if(i + i_startIndex < _activeSteamLobbies.Count)
						{
							_connStatus = ConnectionStatus.Connecting;
							SteamAPICall_t handle = SteamMatchmaking.JoinLobby(_activeSteamLobbies[i + i_startIndex - i_lanGameNumber].m_LobbyID);
							Network.Connect(_activeSteamLobbies[i + i_startIndex - i_lanGameNumber].m_LobbyOwnerIPAddress, 25566);
						}

						_menuActive = false;
					}
				}
				else
				{
					ScreenHelper.SlideInTexture(65, ListStartPosition_Y + i * ListInterval_Y, 23, ListStartPosition_Y + i * ListInterval_Y, 39, 2, _joinButton, _constantTimer, 0.5f, 0, 0);
				}

				//Debug.Log("Host: " + (i + i_startIndex - i_lanGameNumber).ToString());
				string lobby_info = _activeSteamLobbies[i + i_startIndex - i_lanGameNumber].m_LobbyInfo;
				string game_name = _activeSteamLobbies[i + i_startIndex - i_lanGameNumber].m_LobbyName;

				string[] roleAndScore = lobby_info.Split("#".ToCharArray());
				if(!_showIPs)
				{
					ScreenHelper.SlideInText(66, ListStartPosition_Y + i * ListInterval_Y, GameNameStartPosition_X, ListStartPosition_Y + i * ListInterval_Y, GameNameWidth, 2,
					                         game_name,
					                         _constantTimer, 0.5f, 0, 0);
					if(roleAndScore.Length == 2)
					{
						ScreenHelper.SlideInText(150, ListStartPosition_Y + i * ListInterval_Y, LocationStartPosition_X, ListStartPosition_Y + i * ListInterval_Y, LocationWidth, 2,
					                   	      roleAndScore[1],
					                         _constantTimer, 0.5f, 0, 0);
						ScreenHelper.SlideInText(88, ListStartPosition_Y + i * ListInterval_Y, RoleStartPosition_X, ListStartPosition_Y + i * ListInterval_Y, RoleWidth, 2,
					                         roleAndScore[0],
					                        	 _constantTimer, 0.5f, 0, 0);
					}
				}
				else
				{
					ScreenHelper.SlideInText(66, ListStartPosition_Y + i * ListInterval_Y, GameNameStartPosition_X, ListStartPosition_Y + i * ListInterval_Y, GameNameWidth, 2,
					                         _filteredhostData[i + i_startIndex - i_lanGameNumber].ip[0] + ": "+ _filteredhostData[i + i_startIndex - i_lanGameNumber].port,
					                         _constantTimer, 0.5f, 0, 0);
				}


//				if(_filteredhostData[i + i_startIndex - i_lanGameNumber].connectedPlayers <= 1)
//				{
//					if(ScreenHelper.DrawButton(JoinButtonStartPosition_X, (float)ListStartPosition_Y + (float)(i * ListInterval_Y) - JoinButtonOffset, JoinButtonWidth,
//					                           2, _joinButton))
//					{
//						_connStatus = ConnectionStatus.Connecting;
//						Network.Connect(_filteredhostData[i + i_startIndex - i_lanGameNumber]);
//						//Network.Connect(_filteredhostData[i + i_startIndex - i_lanGameNumber].ip, _filteredhostData[i + i_startIndex - i_lanGameNumber].port);
//					}
//				}
//				else
//				{
//					ScreenHelper.DrawGameListText(JoinButtonStartPosition_X, ListStartPosition_Y + i * ListInterval_Y, JoinButtonWidth, 1,
//					                              "RoomFull");
//				}
			}
		}
	}
	// UI Details for Connecting to an existing Server.
	private void gameListFunc(int id)
	{
		if(GUILayout.Button("Refresh"))
		{
			MasterServer.RequestHostList("CH");
		}
		
		GUILayout.BeginHorizontal();
		
		GUILayout.Box ("Room Name");
		GUILayout.Box ("Role");
		GUILayout.Box ("Level");
		GUILayout.Box("Map Name");
		GUILayout.Box("Availability");
		GUILayout.EndHorizontal();
		
		//if(_flow.IsALocalGame())
		{
			GUILayout.BeginHorizontal();
			GUILayout.Box("LocalGameConnection");
			_localIP = GUILayout.TextField(_localIP);
			if(GUILayout.Button("Connect"))
			{
				Network.Connect(_localIP, 25566);
			}
			GUILayout.EndHorizontal();
		}
		
		if(MasterServer.PollHostList().Length != 0)
		{
			HostData[] data = MasterServer.PollHostList();
			GUILayout.BeginVertical();
			
			foreach(HostData i_data in data)
			{
				GUILayout.BeginHorizontal();
				GUILayout.Box(i_data.gameName);
				string[] roleAndScore = i_data.comment.Split("#".ToCharArray());
				GUILayout.Box(roleAndScore[0]);
				GUILayout.Box(roleAndScore[1]);
				GUILayout.Box(roleAndScore[2]);
				if(i_data.connectedPlayers <= 1)
				{
					if(GUILayout.Button("Connect"))
					{
						Network.Connect(i_data.ip, i_data.port);
					}
				}
				else
				{
					GUILayout.Label("RoomFull");
				}
				GUILayout.EndHorizontal();
			}
			GUILayout.EndVertical();
		}
		if(GUI.Button(new Rect(250, 350, 100, 20), "Cancel"))
		{
			ShutDown();
		}
		//GUI.DragWindow(new Rect(0, 0, Screen.width, Screen.height));
	}

	public string GetLocalIPAddress()
	{
		string ip = ""; 
		ip = new System.Net.WebClient().DownloadString("http://ipv4bot.whatismyipaddress.com/");
		if (ip == "") 
		{
			ip = new System.Net.WebClient().DownloadString("http://ipinfo.io/ip");
			if (ip == "") 
			{
				ip = GetIPAddressFromDynDns();
			}
		}

		return ip;
	}

	public string GetIPAddressFromDynDns()
	{
		string response = new System.Net.WebClient().DownloadString("http://checkip.dyndns.org");
		string[] a = response.Split(':');
		string a2 = a[1].Substring(1);
		string[] a3 = a2.Split('<');
		string a4 = a3[0];
		return a4;		
	}

	// Steam Callbacks
	void OnLobbyGameCreated(LobbyGameCreated_t pCallback) 
	{
		Debug.LogError("[" + LobbyGameCreated_t.k_iCallback + " - LobbyGameCreated] - " + pCallback.m_ulSteamIDLobby + " -- " + pCallback.m_ulSteamIDGameServer + " -- " + pCallback.m_unIP + " -- " + pCallback.m_usPort);
	}
	
	void OnLobbyCreated(LobbyCreated_t pCallback, bool bIOFailure) 
	{
		Debug.LogError("[" + LobbyCreated_t.k_iCallback + " - LobbyCreated] - " + pCallback.m_eResult + " -- " + pCallback.m_ulSteamIDLobby);
		if (SteamManager.Initialized) 
		{
			CSteamID lobbyID = new CSteamID(pCallback.m_ulSteamIDLobby);
			SteamMatchmaking.SetLobbyData (lobbyID, "name", _serverName);
			SteamMatchmaking.SetLobbyData (lobbyID, "host_ip", GetLocalIPAddress()); 
			SteamMatchmaking.SetLobbyData (lobbyID, "lobby_info", "Undecided#Undecided");
			Debug.LogError("Local Ip Address is:" + GetLocalIPAddress());
		}
	}

	void OnLobbyEntered(LobbyEnter_t pCallback)
	{
		Debug.LogError ("Joined Lobby:" + pCallback.m_ulSteamIDLobby);
	}
	
	void OnLobbyMatchListReceived(LobbyMatchList_t pCallback, bool bIOFailure) 
	{
		Debug.Log("[" + LobbyMatchList_t.k_iCallback + " - LobbyMatchList] - " + pCallback.m_nLobbiesMatching);
		uint NumServerGames = pCallback.m_nLobbiesMatching;

		_activeSteamLobbies.Clear();

		for (int i = 0; i < NumServerGames; ++i) 
		{
			if (SteamManager.Initialized) 
			{
				CSteamID currentLobbyID = SteamMatchmaking.GetLobbyByIndex (i);
				CSteamID lobbyOwner = SteamMatchmaking.GetLobbyMemberByIndex(currentLobbyID, 0);
			
				SteamLobby newLobby;
				newLobby.m_LobbyID = currentLobbyID;
				newLobby.m_LobbyOwner = lobbyOwner;
				newLobby.m_LobbyName = SteamMatchmaking.GetLobbyData(currentLobbyID, "name");
				newLobby.m_LobbyOwnerIPAddress = SteamMatchmaking.GetLobbyData(currentLobbyID, "host_ip");
				newLobby.m_LobbyInfo = SteamMatchmaking.GetLobbyData(currentLobbyID, "lobby_info");

				_activeSteamLobbies.Add(newLobby);
			}
		}
	}
}