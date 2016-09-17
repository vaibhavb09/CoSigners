using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

public class NameSystem : MonoBehaviour {
	
	private string _playerName = "type your ID";
	private bool _nameExist = false;
	private bool _showCreateNameInterface = false;
	private bool _requireUpdate = false;
	private int _connections = 0;
	//private ArrayList _playerNames = new ArrayList();
	Dictionary<int, string> _playerNames = new Dictionary<int, string>();
	// Use this for initialization
	
	void Start () 
	{
		if(! Directory.Exists(Application.dataPath + "UserProfile"))
		{
			Directory.CreateDirectory(Application.dataPath + "UserProfile");
		}
		else
		{
			StreamReader reader = new StreamReader(Application.dataPath + "UserProfile/name.txt");
			_playerName = reader.ReadLine();
			
			if(_playerName != "")
			{
				_nameExist = true;
			}
		}
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
				//_playerNames.Add(playerID, _playerName);
				networkView.RPC("AddName", RPCMode.All, playerID, _playerName);
			}
		}
	}
	
	void OnServerInitialized()
	{
		if(_nameExist)
		{
			if(_playerNames.ContainsKey(0))
			{
				//_playerNames[playerID] = _playerName;
				networkView.RPC("UpdateNameByID", RPCMode.All, 0, _playerName);
				_connections++;
			}
			else
			{
				//_playerNames.Add(playerID, _playerName);
				networkView.RPC("AddName", RPCMode.All, 0, _playerName);
				_connections++;
			}
		}
	}
	
	void OnPlayerConnected(NetworkPlayer player)
	{
		_connections++;
		networkView.RPC("UpdateConnections", RPCMode.All, _connections);
		//Debug.Log("connects when logged in:" + Network.connections.Length);
	}
	
	[RPC]
	void UpdateConnections(int i_length)
	{
		if(i_length > _connections)
		{
			_connections = i_length;
		}
	}
	
	public string GetName()
	{
		if(!_nameExist)
		{
			_showCreateNameInterface = true;
			return null;
		}
		else
			return _playerName;
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
			
				for(int i = 1; i < _connections; i++)
					networkView.RPC ("InitPlayerName", RPCMode.All, i, _playerNames[i]);
				_requireUpdate = false;
			}
		}
		
		if(_showCreateNameInterface)
		{
			CreateNameInterface();
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
	}
	
	[RPC]
	void RequestServerUpdateNames(bool i_bool)
	{
		_requireUpdate = i_bool;
	}
	
	
	
	
	public void ChangeName()
	{
		_nameExist = false;
		_showCreateNameInterface = true;
	}
	
	private void CreateNameInterface()
	{
		//GUILayout.Label("Username",
		GUI.Box(new Rect(Screen.width/2 - 120, Screen.height/2 - 40, 240, 140), "Create Account");
		//GUI.Label(new Rect(Screen.width/2 - 100, Screen.height/2 - 20, 200, 20), "Create Account");
		GUI.Label(new Rect(Screen.width/2 - 100, Screen.height/2, 80, 20), "Username");
		_playerName = GUI.TextField(new Rect(Screen.width/2, Screen.height/2, 100, 20), _playerName);
		if(GUI.Button(new Rect(Screen.width/2- 60, Screen.height/2 + 40, 100, 20), "Create"))
		{
			_showCreateNameInterface = false;
			_nameExist = true;
			File.WriteAllText(Application.dataPath + "UserProfile/name.txt", _playerName);
			
			if(Network.isClient)
			{
				int playerID = Convert.ToInt32(Network.player.ToString());
				if(_playerNames.ContainsKey(playerID))
				{
					//_playerNames[playerID] = _playerName;
					networkView.RPC("UpdateNameByID", RPCMode.All, playerID, _playerName);
				}
				else
				{
					//_playerNames.Add(playerID, _playerName);
					networkView.RPC("AddName", RPCMode.All, playerID, _playerName);
				}
			}
			else if(Network.isServer)
			{
				if(_playerNames.ContainsKey(0))
				{
					//_playerNames[playerID] = _playerName;
					networkView.RPC("UpdateNameByID", RPCMode.All, 0, _playerName);
				}
				else
				{
					//_playerNames.Add(playerID, _playerName);
					networkView.RPC("AddName", RPCMode.All, 0, _playerName);
				}				
			}
		}
	}
	
	[RPC]
	void UpdateNameByID(int i_id, string i_name)
	{
		_playerNames[i_id] = i_name;
	}
	
	
	[RPC]
	void AddName(int i_id, string i_name)
	{
		_playerNames.Add(i_id, i_name);
	}	
}
