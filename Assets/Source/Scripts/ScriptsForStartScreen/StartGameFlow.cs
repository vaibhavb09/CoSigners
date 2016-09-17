using UnityEngine;
using System.Collections;

public class StartGameFlow : MonoBehaviour {
	
	#region Field
	private StartGameInterface 		_startGameInterface;
	private GameLobbyInterface 		_gameLobbyInterface;
	private RoomInterface 	   		_roomInterface;
	private UDPBroadCaster     		_caster;
	private UDPBroadcastReceiver 	_receiver;
		
	private Texture2D _buttonAccentLeft;
	private Texture2D _buttonAccentRight;
	private Texture2D _cancelButtonActive;
	private Texture2D _cancelButtonNormal;
	private Texture2D _nextAreaActive;
	private Texture2D _nextAreaNormal;
	private Texture2D _prevAreaActive;
	private Texture2D _prevAreaNormal;
	
	public string GameTypeName = "CeilBlock1";
	private bool _isLANGame = false;
	private bool _isInGameLobby  = false;
	private bool _isInGameRoom = false;
	#endregion

	#region IPPortConfig
	private string MasterServerIP;
	private int MasterServerPort;
	private string NATFacilitatorIP;
	private int NATFacilitatorPort;
	private string ConnectionTesterIP;
	private int ConnectionTesterPort;
	#endregion
	
	#region Properties
	
	public bool LoginComplete = false;

	public bool IsLANGame
	{
		get
		{
			return _isLANGame;
		}
		set
		{
			_isLANGame = value;
		}
	}

	public bool IsInGameLobby
	{
		get
		{
			return _isInGameLobby;
		}
	}

	public UDPBroadCaster Caster
	{
		get
		{
			return _caster;
		}
	}

	public UDPBroadcastReceiver Receiver
	{
		get
		{
			return _receiver;
		}
	}

	public Texture2D NextAreaActive
	{
		get
		{
			return _nextAreaActive;
		}
	}
	
	public Texture2D NextAreaNormal
	{
		get
		{
			return _nextAreaNormal;
		}
	}
	
	public Texture2D PrevAreaActive
	{
		get
		{
			return _prevAreaActive;
		}
	}
	
	public Texture2D PrevAreaNormal
	{
		get
		{
			return _prevAreaNormal;
		}
	}
	
	public Texture2D AccentLeft
	{
		get
		{
			return _buttonAccentLeft;
		}
	}
	
	public Texture2D AccentRight
	{
		get
		{
			return _buttonAccentRight;
		}
	}
	
	public Texture2D CancelButtonActive
	{
		get
		{
			return _cancelButtonActive;
		}
	}
	
	public Texture2D CancelButtonNormal
	{
		get
		{
			return _cancelButtonNormal;
		}
	}
	#endregion

	void ReadIPPortConfigValues()
	{
		string[] lines = System.IO.File.ReadAllLines(@"Config_IP_Ports.ini");
		foreach (string line in lines) 
		{
			string[] substrings = line.Split('=');
			if(substrings.Length == 2)
			{
				if(string.Equals(substrings[0],"MasterServerIP"))
					MasterServerIP = substrings[1];
				if(string.Equals(substrings[0],"MasterServerPort"))
					MasterServerPort = int.Parse(substrings[1]);
				if(string.Equals(substrings[0],"NATFacilitatorIP"))
					NATFacilitatorIP = substrings[1];
				if(string.Equals(substrings[0],"NATFacilitatorPort"))
					NATFacilitatorPort = int.Parse(substrings[1]);
				if(string.Equals(substrings[0],"ConnectionTesterIP"))
					ConnectionTesterIP = substrings[1];
				if(string.Equals(substrings[0],"ConnectionTesterPort"))
					ConnectionTesterPort = int.Parse(substrings[1]);
			}
		}
	}
	
	// Use this for initialization
	void Start () {
		_isInGameLobby = false;
		_startGameInterface = gameObject.GetComponent<StartGameInterface>();
		_gameLobbyInterface = gameObject.GetComponent<GameLobbyInterface>();
		_roomInterface	= gameObject.GetComponent<RoomInterface>();

		ReadIPPortConfigValues();

		MasterServer.ipAddress = MasterServerIP;
		//MasterServer.ipAddress = "54.187.70.136";
		MasterServer.port = MasterServerPort;
		Network.natFacilitatorIP = NATFacilitatorIP;
		Network.natFacilitatorPort = NATFacilitatorPort;
		Network.connectionTesterIP = ConnectionTesterIP;
		Network.connectionTesterPort = ConnectionTesterPort;
		//_startGameInterface.Show();
		_buttonAccentLeft = Resources.Load("Textures/StartScreenUI/btnAccent_right", typeof(Texture2D)) as Texture2D;
		_buttonAccentRight = Resources.Load("Textures/StartScreenUI/btnAccent_left", typeof(Texture2D)) as Texture2D;
		_cancelButtonActive = Resources.Load("Textures/StartScreenUI/btn_cancel_active", typeof(Texture2D)) as Texture2D;
		_cancelButtonNormal = Resources.Load("Textures/StartScreenUI/btn_cancel_normal", typeof(Texture2D)) as Texture2D;
		_nextAreaActive = Resources.Load("Textures/StartScreenUI/btn_NextArea_active", typeof(Texture2D)) as Texture2D;
		_nextAreaNormal = Resources.Load("Textures/StartScreenUI/btn_NextArea_normal", typeof(Texture2D)) as Texture2D;
		_prevAreaActive = Resources.Load("Textures/StartScreenUI/btn_PrevArea_active", typeof(Texture2D)) as Texture2D;
	 	_prevAreaNormal = Resources.Load("Textures/StartScreenUI/btn_PrevArea_normal", typeof(Texture2D)) as Texture2D;
	}

	public void LANBroadcastRoomMessage(string i_message)
	{
		if(_receiver != null) 
		{
			_receiver.CloseReceiver();
			_receiver = null;
		}
		if(_caster == null) 
		{
			_caster = new UDPBroadCaster();
			_caster.Init();
			_caster.BroadcastMessage(i_message);
			//Debug.LogError("#MAx Trying to start broad");
		}
		else
		{
			//Debug.LogError("#MAx Trying to broad:" + i_message);
			_caster.BroadcastMessage(i_message);
		}
	}

	public string LANReceiveRoomInfo()
	{
		if(_caster != null)	
		{
			//Debug.Log("#MAx try to delete caster");
			_caster.CloseCaster();
			_caster = null;
		}
		
		if(_receiver == null) 
		{
			//Debug.Log("#MAx Try to start receive");
			_receiver =  new UDPBroadcastReceiver();
			_receiver.Init();
			_receiver.ReceiveMessage();
		}
		else
		{
			_receiver.ReceiveMessage();
			//Debug.Log("#MAx Try to receive: " + _receiver.GetMessage());
		}

		return _receiver.GetMessage();
	}

	public void ShowLoginInterface()
	{
		_isInGameLobby = false;
		_startGameInterface.Show();
		_gameLobbyInterface.ShutDown();
		_roomInterface.ShutDown();
	}
	
	public void UpdatePlayerName()
	{
		_startGameInterface.UpdateName();
	}
	
	public void ShowMenu()
	{
		LoginComplete = true;
		_startGameInterface.ShutDownStartGameInterface();
	}
	
	public void EnableStartMenu()
	{
	}
	
	public void DisableStartMenu()
	{
	}
	
	public void ShowGameRoom()
	{
		_gameLobbyInterface.ShutDown();
		_roomInterface.Show();
		_isInGameLobby = false;
		_isInGameRoom = true;
	}
	
	public void EnterRoomFromLobby()
	{
		_gameLobbyInterface.ShutDown();
		_roomInterface.ShowFromClient();
		_isInGameLobby = false;
		_isInGameRoom = true;
	}
	
	public void ReturnToLobbyFromRoom()
	{
		_gameLobbyInterface.ClearLANData();
		_gameLobbyInterface.Show();
		_isInGameLobby = true;
		_isInGameRoom = false;
	}
	
	public void ShowGameLobby()
	{
		_startGameInterface.ShutDownStartGameInterface();
		_gameLobbyInterface.ClearLANData();
		_gameLobbyInterface.Show();	
		_isInGameLobby = true;
		_isInGameRoom = false;
	}
	
	// Update is called once per frame
	void Update () {
		if(Network.connections.Length != 0 && (_isInGameRoom == false || _startGameInterface.IsStillActive()))
		{
			_startGameInterface.ShutDownStartGameInterface();
			_roomInterface.Show();
			_isInGameRoom= true;
			_roomInterface.SetupStuffForRoomInterface();
		}

		if(_isInGameLobby)
		{
			if(_receiver != null && !_receiver.IsBound())
			{
				_receiver.Init();
			}
		}
	}
}
