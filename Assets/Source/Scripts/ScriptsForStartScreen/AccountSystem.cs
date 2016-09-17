using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

public class AccountSystem : MonoBehaviour {
	
	public static string ClientPlayerName = " ";
	public static string ServerPlayerName = " ";

	private string _playerName = "type your ID";
	private string _currentName = " ";
	private bool _nameExist = false;
	private bool _showCreateNameInterface = false;
	private bool _requireUpdate = false;
	private int _connections = 0;
	string _password1  = "";
	string _password2 = "";
	//private ArrayList _playerNames = new ArrayList();ruc
	Dictionary<int, string> _playerNames = new Dictionary<int, string>();
	
	private Rect _createAccountWindow = new Rect(Screen.width/2 - 320, Screen.height/2 + 90, 250, 240);
	private StartGameFlow _flow;
	struct AccountInfo
	{
		public string name;
		public string password;
	}
	
	List<AccountInfo> _players = new List<AccountInfo>();
	// Use this for initialization
	static int _initTime = 0;
	int _initTimes = 0;
	
	void Awake()
	{
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

	void Start () 
	{
		_flow = GameObject.Find("GameFlow").GetComponent<StartGameFlow>();
		LoadAccountInfo();
	}
	
	public void ResetNamesAfterForceShutDown()
	{
		_playerNames.Clear();
		_connections = 0;
	}
	
	void LoadAccountInfo()
	{
		_flow.UpdatePlayerName();
	}
	
	public bool ValidateAccount(string i_name, string i_pwd)
	{
		//Debug.Log(i_name);
		//Debug.Log(i_pwd);
		
		foreach(AccountInfo info in _players)
		{
			//Debug.Log("Account: " +info.name);
			//Debug.Log("pwd: " + info.password);
			if(info.name == i_name && info.password == i_pwd)
			{
				return true;
			}
		}
		return false;
	}
	
	void OnConnectedToServer()
	{
		if(_nameExist)
		{
			//Debug.Log("Hit this?");
			int playerID = Convert.ToInt32(Network.player.ToString());
			if(_playerNames.ContainsKey(playerID))
			{
				//_playerNames[playerID] = _playerName;
				networkView.RPC("UpdateNameByID", RPCMode.All, playerID, _playerName);
			}
			else
			{
				//Debug.Log("#Max:Yea, that happened");
				//_playerNames.Add(playerID, _playerName);
				++_connections;
				networkView.RPC("AddName", RPCMode.All, playerID, _playerName);
			}
		}
	}
	
	public void SetUpAnonymousAccount()
	{
		_nameExist = true;
		_playerName = "ANONYMOUS";	
	}
	
	void OnServerInitialized()
	{
		if(_nameExist)
		{
			//Debug.Log("Does this happened?");
			if(_playerNames.ContainsKey(0))
			{
				//_playerNames[playerID] = _playerName;
				networkView.RPC("UpdateNameByID", RPCMode.All, 0, _playerName);
				//_connections++;
			}
			else
			{
				//Debug.Log("Impossible");
				//_playerNames.Add(playerID, _playerName);
				_connections++;
				//Debug.Log(_connections);
				networkView.RPC("AddName", RPCMode.All, 0, _playerName);
				
			}
		}
	}
	
	void OnPlayerConnected(NetworkPlayer player)
	{
		_connections++;
		networkView.RPC("UpdateConnections", RPCMode.All, _connections);
		//Debug.Log("connects when logged in:" + Network.connections.Length);
	}
	
	void OnPlayerDisconnected(NetworkPlayer player)
	{
		_connections--;
		_playerNames.Remove(Convert.ToInt32(player.ToString()));
		//networkView.RPC("UpdateConnections", RPCMode.All, _connections);
	}
	
	void OnDisconnectedFromServer(NetworkDisconnection info)
	{
		_connections--;
		if(_connections < 0)
			_connections = 0;
		//networkView.RPC("UpdateConnections", RPCMode.All, _connections);
	}
	
	[RPC]
	void UpdateConnections(int i_length)
	{
		_connections = i_length;
	}
	
	public string GetName()
	{
		if(!_nameExist)
		{
			_playerName = PlayerProfile.LoadPlayerProfile();
			return _playerName;
		}
		else
		{
			return _playerName;
		}
	}
	
	
	public string GetNameFromID(int i_id)
	{
		if(_playerNames.ContainsKey(i_id))
		{
			return _playerNames[i_id];
		}
		else 
		{
			networkView.RPC("RequestServerUpdateNames", RPCMode.Server, true);
			return "";
		}
	}
	
	public int GetConnectionLength()
	{
		return _connections;//_playerNames.Count;
	}
	
	private void OnGUI()
	{
		//WriteAllText
		
		if(_playerNames.Count < _connections)
		{
			networkView.RPC("RequestServerUpdateNames", RPCMode.Server, true);
		}
		
		if(Network.isServer)
		{
			if(_requireUpdate)
			{
				//the server name is put at first
				networkView.RPC ("InitPlayerName", RPCMode.All, 0, _playerName);
				//if(_connections == 2 && _playerNames.Count == 2)
				//{
				//	for(int i = 1; i < _connections; i++)
				//		networkView.RPC ("InitPlayerName", RPCMode.All, i, _playerNames[i]);					
				//}
				//else
				//{
				//	Debug.Log("I'm updating what?");
				//}

				_requireUpdate = false;
			}
		}
	}
	
	[RPC]
	void InitPlayerName(int id, string i_name)	
	{
		if(!_playerNames.ContainsKey(id))
		{
			_playerNames.Add(id, i_name);
		}
		else
		{
			_playerNames[id] = i_name;
		}
		if(id == 0)
		{
			ServerPlayerName = i_name;
		}
		else
		{
			ClientPlayerName = i_name;
		}
	}
	
	[RPC]
	void RequestServerUpdateNames(bool i_bool)
	{
		_requireUpdate = i_bool;
	}
	
	
	public void CreateNewAccount()
	{
		_nameExist = false;
		_showCreateNameInterface = true;
		_currentName = _playerName;
		if (VirtualKeyboard.isWindowsTablet == true)
			VirtualKeyboard.RemoteEnable();
	}
	
	public void ChangeName()
	{
		_nameExist = false;
		_showCreateNameInterface = true;
	}

	[RPC]
	void UpdateNameByID(int i_id, string i_name)
	{
		_playerNames[i_id] = i_name;
		if(i_id== 0)
		{
			ServerPlayerName = i_name;
		}
		else
		{
			ClientPlayerName = i_name;
		}
	}

	[RPC]
	void AddName(int i_id, string i_name)
	{
		_playerNames.Add(i_id, i_name);
		if(i_id== 0)
		{
			ServerPlayerName = i_name;
		}
		else
		{
			ClientPlayerName = i_name;
		}
	}	
}
