using UnityEngine;
using System.Collections;

public class Networkconnection : Photon.MonoBehaviour {
	
	//private int _playerType = 0; //1 for prisoner, 2 for hacker, 3 for Observer 
	private bool _createGame = false;
	private bool _joinGame = false;
	
	private string _serverName = "", _maxPlayers = "0", _port = "25566";
	private const string _gameTitle = "CeilBlock";
	private Rect windowRect = new Rect(0, 0, 400, 400);
	
	private GameObject _playerUtil;
	private StartGameFlow _flow;
	
	void Start()
	{
		_playerUtil = GameObject.Find("PlayerUtil");
		_flow = GameObject.Find("GameFlow").GetComponent<StartGameFlow>();
	}
	
	
	private void OnGUI()
	{
		//if(_flow.LoginComplete)
		{
			GUI.Label(new Rect(Screen.width - 180, 10, 160, 25), "Logged in as: " + _playerUtil.GetComponent<NameSystem>().GetName());
			//disable this if the game starts!
			if(GUI.Button(new Rect(Screen.width -160, 30, 100, 20), "ChangeUser"))
			{
				_playerUtil.GetComponent<AccountSystem>().ChangeName();
			}
		}
		

		if(PhotonNetwork.connectionState == ConnectionState.Disconnected)
		{
			if((_createGame == false)&&(_joinGame == false))
			{
				if(GUI.Button(new Rect(10, 50, 120, 20), "Create Game"))
				{
					_createGame = true;
				}
				
				if(GUI.Button(new Rect(10, 30, 120, 20), "Join Game"))
				{
					_joinGame = true;
				}
			}
			
			if(_createGame)
			{
				CreateGameIntrerface();
			}
			else if(_joinGame)
			{
				JoinGameInterface();
			}
		}

		//else if(PhotonNetwork.connectionState == PhotonNetwork.isMasterClient)
		else if(PhotonNetwork.connectionState == ConnectionState.Connected)
		{
			GUI.Label(new Rect(10, 10, 300, 20), "Status: Connected as Server");
			if(GUI.Button(new Rect(10, 30, 120, 20), "Disconnect"))
			{
				//PhotonNetwork.Disconnect();
				_createGame = false;
				_joinGame = false;
			}
		}
		//else if(PhotonNetwork.connectionState == PhotonNetwork.isNonMasterClientInRoom)
		else if(PhotonNetwork.connectionState == ConnectionState.Connecting)
		{
			GUI.Label(new Rect(10, 10, 300, 20), "Status: Connected as Client");
			if(GUI.Button(new Rect(10, 30, 120, 20), "Disconnect"))
			{
				//PhotonNetwork.Disconnect();
				_createGame = false;
				_joinGame = false;
			}		
		}
	}
	
	// Interface for connecting to an existing game server
	private void JoinGameInterface()
	{
		windowRect = GUI.Window(0, windowRect, gameListFunc, "Game Lobby");
	}
	
	
	// Interface for Creating a new Game Server
	private void CreateGameIntrerface()
	{
		GUILayout.Label ("Room Name");
		_serverName = GUILayout.TextField(_serverName);
				
		GUILayout.Label ("Port");
		_port = GUILayout.TextField(_port);
				
		GUILayout.Label ("Player Number");
		_maxPlayers = GUILayout.TextField(_maxPlayers);
				
		if(GUILayout.Button("Create"))
		{
			// Initialize the Server
			//PhotonNetwork.InitializeSecurity();
			//PhotonNetwork.CreateRoom(int.Parse(_maxPlayers), int.Parse(_port), !PhotonNetwork.HavePublicAddress());
			PhotonNetwork.CreateRoom(_serverName);
			// NIK REGISTER HOST [CHANGE THIS]
			//MasterServer.RegisterHost("CeilBlock", _serverName);
		}
	}
	
	
	// UI Details for Connecting to an existing Server.
	private void gameListFunc(int id)
	{
		if(GUILayout.Button("Refresh"))
		{
			MasterServer.RequestHostList(_gameTitle);
		}
		
		GUILayout.BeginHorizontal();
		
		GUILayout.Box ("Game List");
		GUILayout.EndHorizontal();
		
		if(MasterServer.PollHostList().Length != 0)
		{
			HostData[] data = MasterServer.PollHostList();
			GUILayout.BeginHorizontal();
			foreach(HostData i_data in data)
			{
				GUILayout.Box(i_data.gameName);
				if(GUILayout.Button("Connect"))
				{
					PhotonNetwork.JoinRoom(i_data.gameName);
				}
			}
			GUILayout.EndHorizontal();
		}
		
		GUI.DragWindow(new Rect(0, 0, Screen.width, Screen.height));
	}
}
