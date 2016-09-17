using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using Steamworks;

public class PlayerProfile {
	
	#region properties
	public static string PlayerName
	{
		get	{	return _playerName;		}
		set	{	_playerName = value;	}
	}
	#endregion

	static string _playerName;
	
	#region Public Interface
	static public string LoadPlayerProfile()
	{
		if(string.IsNullOrEmpty(_playerName))
			_playerName = SteamFriends.GetPersonaName ();
		return _playerName;
	}
	#endregion
}
