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
	
	// Use this for initialization
	void Start () {
		_isInGameLobby = false;
		_startGameInterface = gameObject.GetComponent<StartGameInterface>();
		_gameLobbyInterface = gameObject.GetComponent<GameLobbyInterface>();
		_roomInterface	= gameObject.GetComponent<RoomInterface>();

		MasterServer.ipAddress = "hacknhide.eaemgs.utah.edu";
		//MasterServer.ipAddress = "54.187.70.136";
		MasterServer.port = 23466;
		Network.natFacilitatorIP = "hacknhide.eaemgs.utah.edu";
		Network.natFacilitatorPort = 50005;
		Network.connectionTesterIP = "hacknhide.eaemgs.utah.edu";
		Network.connectionTesterPort = 23467;
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
