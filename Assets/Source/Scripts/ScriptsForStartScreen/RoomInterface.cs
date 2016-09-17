using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.IO;

public class RoomInterface : MonoBehaviour {
	
	public Transform playerPrefab;
	public GUISkin customSkin;
	public string[] ChapterNames;
	private string[] _chapterPrefixs = {"TRAINING", "AREA I", "AREA II","AREA III","MAIN SERVER","BONUS"}; 
	
	private Dictionary<int, int> _playerRoles = new Dictionary<int, int>(); //0 - none, 1 - prisoner, 2 - hacker, 3- observer
	private Dictionary<int, List<LevelDescription>> _levelDict = new Dictionary<int, List<LevelDescription>>();
	private List<int> _playerIDs = new List<int>();
	private List<LevelDescription> _levelDetails  = new List<LevelDescription>(); 
	private ArrayList playerScripts = new ArrayList();
	
	private string[] _roleContent = new string[4];
	private int _playerRole = 0;				//0 - none, 1 - prisoner, 2 - hacker, 3- observer
	private int _selectedLevel = 0;
	private int _playerID;
	private bool _showRoomInterface = false;
	private bool _requireUpdate = false;
	private bool _justStarted = true;
	
	private int _currentChapter = 0;
	private int _currentSelectionIndex = 0;
	
	private Rect _roomInterfaceWindow;
	
	private StartGameFlow _flow;
	private GameObject _camera;
	private GameObject _playerUtil;
	
	private float _RefreshInterval = 0.5f;
	private float _currentTimer = 0.0f;
	private float _currentTicker = 0.0f;
	private bool _gameStarted = false;

	#region GUIStuff
	private Texture2D _RoomInterfaceBackground;
	private Texture2D _startButtonActive;
	private Texture2D _startButtonNormal;
	private Texture2D _startButtonDisable;
	private Texture2D _mainMenuButtonActive;
	private Texture2D _mainMenuButtonNormal;
	private Texture2D _hackerButtonActive;
	private Texture2D _hackerButtonNormal;
	private Texture2D _thiefButtonActive;
	private Texture2D _thiefButtonNormal;
	private Texture2D[] _Areas = new Texture2D[6]; 
	private Texture2D _AreaHighLight;
	private Texture2D _AreaMouseOver;
	private Texture2D _LevelHighLight;
	private Texture2D _LevelMouseOver;
	private Texture2D _leftWindowBkg;
	private Texture2D _rightWindowBkg;
	#endregion
		
	void Update () {

		if(_requireUpdate)
		{
			networkView.RPC("SyncChapter", RPCMode.Others, _currentChapter);
			networkView.RPC("SyncLevelSelection", RPCMode.Others, _currentSelectionIndex, _selectedLevel);
			_requireUpdate = false;
		}

		if(_currentTimer > _RefreshInterval)
		{
			if(_showRoomInterface && _flow.IsLANGame)
			{
				string roleNeeded = null;

				roleNeeded = _roleContent[3- _playerRole];
				
				if(_playerIDs.Count < 2)
				{
					_flow.LANBroadcastRoomMessage(PlayerProfile.PlayerName + "'s Game#" + roleNeeded + "#" + _chapterPrefixs[_currentChapter] +": " + _levelDetails[_selectedLevel].LevelName + "#true#false");
				}
				else if(_playerIDs.Count >= 2)
				{
					_flow.LANBroadcastRoomMessage(PlayerProfile.PlayerName + "'s Game#" + roleNeeded + "#" + _chapterPrefixs[_currentChapter] +": " + _levelDetails[_selectedLevel].LevelName + "#true#true");
				}
				else if(_gameStarted)
				{
					_flow.LANBroadcastRoomMessage(PlayerProfile.PlayerName + "'s Game#" + roleNeeded + "#" + _chapterPrefixs[_currentChapter] +": " + _levelDetails[_selectedLevel].LevelName + "#false#true");
				}
			}
			_currentTimer = 0.0f;
			_justStarted = false;
		}
		else
		{
			_currentTimer += Time.deltaTime;
		}
		_currentTicker += Time.deltaTime;
	}
	
	public void Show()
	{
		if (VirtualKeyboard.enabled == true)
			VirtualKeyboard.text = "";
		_showRoomInterface = true;
		_justStarted = true;
		_currentTimer = 0.0f;
		_currentTicker = 0.0f;
		_selectedLevel = 0;
		networkView.RPC("RequestServerUpdate",RPCMode.Server, true);
//		if(GameManager.Manager.FirstLogin)
//		{
//			ReRegisterRoom();
//			GameManager.Manager.FirstLogin = false;
//		}
	}
	
	public void SetupStuffForRoomInterface()
	{
		//_playerID = Convert.ToInt32(Network.player.ToString());
		if(Network.isServer)
		{
			_playerID = 0;
		}
		else
		{
			_playerID = 1;
		}

		_playerRole = GameManager.Manager.PlayerType;
		_playerIDs.Clear();
		_playerIDs.Add(0);
		_playerIDs.Add(1);
		_playerRoles.Clear();
		_playerRoles.Add(_playerID, _playerRole);
		_playerRoles.Add(1-_playerID, 3- _playerRole);
	}
	
	public void ShowFromClient()
	{
		_showRoomInterface = true;
	}
	
	public void ShutDown()
	{
		if(Network.isServer)
		{
			//Debug.Log(" #Max: unreg");
			MasterServer.UnregisterHost();
			networkView.RPC("ForceShutDown",RPCMode.Others);		
			_showRoomInterface = false;
			_flow.ReturnToLobbyFromRoom();
			ExitCleanUp();
			_flow.LANBroadcastRoomMessage(PlayerProfile.PlayerName + "'s Game#Undecided#" + _chapterPrefixs[_currentChapter] +": " + _levelDetails[_selectedLevel].LevelName + "#false#true");
		}
		else
		{
			ForceShutDown();
		}
		Network.Disconnect();
	}

	private void ExitCleanUp()
	{
		if(Network.connections.Length != 0)
			networkView.RPC("RemoveMyExistence", RPCMode.Others, _playerID);
		else
			RemoveMyExistence(_playerID);
		_playerIDs.Clear();
		_playerID = 0;
		_selectedLevel = 0;
		_currentSelectionIndex = 0;
		_playerUtil.GetComponent<AccountSystem>().ResetNamesAfterForceShutDown();
	}
	

	
	private void Start()
	{
		_roleContent[0] 			= "Undecided";
		_roleContent[1] 			= "Thief";
		_roleContent[2] 			= "Hacker";
		
		_flow 						= gameObject.GetComponent<StartGameFlow>();
		_camera 					= GameObject.Find("TopDownCamera");
		_playerUtil 				= GameObject.Find("PlayerUtil");
		
		_RoomInterfaceBackground 	= Resources.Load("Textures/LevelSelect/Background", typeof(Texture2D)) as Texture2D;
		_startButtonActive 			= Resources.Load("Textures/LevelSelect/Btn_Select_active", typeof(Texture2D)) as Texture2D;
		_startButtonNormal 			= Resources.Load("Textures/LevelSelect/Btn_Select_norm", typeof(Texture2D)) as Texture2D;
		_startButtonDisable 		= Resources.Load("Textures/LevelSelect/Btn_Select_disabled", typeof(Texture2D)) as Texture2D;
		_hackerButtonActive 		= Resources.Load("Textures/LevelSelect/Hacker_Btn_On", typeof(Texture2D)) as Texture2D;
		_hackerButtonNormal 		= Resources.Load("Textures/LevelSelect/Hacker_Btn_Off", typeof(Texture2D)) as Texture2D;
		_thiefButtonActive 			= Resources.Load("Textures/LevelSelect/Thief_Btn_On", typeof(Texture2D)) as Texture2D;
		_thiefButtonNormal 			= Resources.Load("Textures/LevelSelect/Thief_Btn_Off", typeof(Texture2D)) as Texture2D;
		_Areas[0] 					= Resources.Load("Textures/LevelSelect/Area_00", typeof(Texture2D)) as Texture2D;
		_Areas[1] 					= Resources.Load("Textures/LevelSelect/Area_01", typeof(Texture2D)) as Texture2D;
		_Areas[2]					= Resources.Load("Textures/LevelSelect/Area_02", typeof(Texture2D)) as Texture2D;
		_Areas[3]					= Resources.Load("Textures/LevelSelect/Area_03", typeof(Texture2D)) as Texture2D;
		_Areas[4]					= Resources.Load("Textures/LevelSelect/Area_04", typeof(Texture2D)) as Texture2D;
		_Areas[5]					= Resources.Load("Textures/LevelSelect/Area_05", typeof(Texture2D)) as Texture2D;

		_AreaHighLight 				= Resources.Load("Textures/LevelSelect/Area_highlight", typeof(Texture2D)) as Texture2D;
		_LevelHighLight				= Resources.Load("Textures/LevelSelect/Level_Highlight", typeof(Texture2D)) as Texture2D;

		_AreaMouseOver				= Resources.Load("Textures/LevelSelect/Area_MouseOver", typeof(Texture2D)) as Texture2D;
		_LevelMouseOver 			= Resources.Load("Textures/LevelSelect/Level_MouseOver", typeof(Texture2D)) as Texture2D;

		_leftWindowBkg 				= Resources.Load("Textures/LevelSelect/LeftWindow_bkg", typeof(Texture2D)) as Texture2D;
		_rightWindowBkg 			= Resources.Load("Textures/LevelSelect/RightWindow_bkg", typeof(Texture2D)) as Texture2D;
		_mainMenuButtonActive		= Resources.Load("Textures/LevelSelect/Btn_MainMenu_active", typeof(Texture2D)) as Texture2D;
		_mainMenuButtonNormal		= Resources.Load("Textures/LevelSelect/Btn_MainMenu_norm", typeof(Texture2D)) as Texture2D;
		// Adding a None level for the GUI
		//GameManager.Manager.LevelNames.Add("None");
		
		// reading the level descriptor as a text file
		TextAsset levelText = (TextAsset) Resources.Load("Levels/LevelConfig");
		GameManager.Manager.LevelNames.Clear();
		
		using (TextReader levelReader = new StringReader((string)levelText.text))
		{
			int index = 0;
			while(levelReader.Peek() >= 0)
			{
				// ---- Level Data - Max - 10/18/13
				// - 0.Chapter Number
				// - 1.Scene File Name
				// - 2.Level Name
				// - 3.Path for thumbnail image
				// - 4.Path for detail image
				// - 5.Level Description
				// - 6.Estimated Time
				// - 7.Start Transmitters
				// - 8.Difficulty
				
				string[] levelData = levelReader.ReadLine().Split("#".ToCharArray());
				LevelDescription thisLevel = new LevelDescription();
				thisLevel.Chapter = Convert.ToInt32(levelData[0]);
				thisLevel.SceneFile = levelData[1];
				thisLevel.LevelName = levelData[2];
				thisLevel.LevelThumbnail = Resources.Load(levelData[3], typeof(Texture2D)) as Texture2D;
				thisLevel.LevelDetail = Resources.Load(levelData[4], typeof(Texture2D)) as Texture2D;
				thisLevel.Description = levelData[5];
				thisLevel.EstimatedTime = levelData[6];
				thisLevel.TransmitterNumber = Convert.ToInt32(levelData[7]);
				thisLevel.Difficulty = levelData[8];
				thisLevel.Index = index;
				_levelDetails.Add(thisLevel);
				AddLevelToDictionary(thisLevel);
				index++;
				//do we need this or not, to be decided.
				GameManager.Manager.LevelNames.Add(thisLevel.SceneFile);
				
				//Debug.Log("Level name read : " + GameManager.Manager.LevelNames[GameManager.Manager.LevelNames.Count-1]);
			}
			
			levelReader.Close();			
		}
	}
	
	private void AddLevelToDictionary(LevelDescription i_desc)
	{
		if(_levelDict.ContainsKey(i_desc.Chapter))
		{
			_levelDict[i_desc.Chapter].Add(i_desc);
		}
		else
		{
			List<LevelDescription> tempList = new List<LevelDescription>();
			tempList.Add(i_desc);
			_levelDict.Add(i_desc.Chapter, tempList);
		}
	}

	#region Network Events
	void OnServerInitialized()
	{
		_playerID = 0;
		_playerRole = 1;
		_playerRoles.Clear();
		_playerRoles.Add(0, 1);
		_playerRoles.Add(1, 2);
		_playerIDs.Clear();
		_playerIDs.Add(0);
		_showRoomInterface = true;
	}
	
	void OnPlayerConnected(NetworkPlayer player)
	{		
		//this is server's doing
		networkView.RPC("SyncPlayerRole", RPCMode.Others, 0, _playerRole);
		networkView.RPC("SyncLevelSelection", RPCMode.Others, _currentSelectionIndex, _selectedLevel);
	}

	void OnConnectedToServer()
	{
		_flow.EnterRoomFromLobby();
		_showRoomInterface = true;
		//_playerID = Convert.ToInt32(Network.player.ToString());
		_playerID = 1;
		networkView.RPC("AddPlayerID", RPCMode.All, _playerID);
		networkView.RPC("RequestServerUpdate",RPCMode.Server, true);
	}
	
	void OnPlayerDisconnected(NetworkPlayer player)
	{
		//int ID = Convert.ToInt32(player.ToString());
		int ID = 1;
		networkView.RPC("RemovePlayerID", RPCMode.All, ID);

		Network.RemoveRPCs(player);
		Network.DestroyPlayerObjects(player);
	}
	#endregion

	#region RPCs
	
	[RPC]
	void InitPlayerID(int i_id)
	{
		if(_playerIDs.Count == 0)
		{
			_playerIDs.Add(i_id);
		}
		else if(!_playerIDs.Contains(0))
		{
			_playerIDs.Add(_playerIDs[0]);
			_playerIDs[0] = 0;
		}
	}
	
	[RPC]
	void RemovePlayerID(int i_id)
	{
		_playerIDs.Remove(i_id);	
	}
		
	[RPC]
	void AddPlayerID(int i_id)
	{
		if(_playerIDs.Count != 0)
		{
			_playerIDs.Add(i_id);
		}
		else
		{
			_playerIDs.Add(0);
			_playerIDs.Add(i_id);
		}
	}
	
	[RPC]
	void SyncPlayerRole(int i_id, int i_role)
	{
		_playerRoles[i_id] = i_role;
		_playerRoles[1-i_id] = 3- i_role;
		_playerRole = (i_id == _playerID? i_role: (3 - i_role));
	}
	
	[RPC]
	void SyncLevelSelection(int i_index, int i_level)
	{	
		_currentSelectionIndex = i_index;
		_selectedLevel = i_level;
		//Debug.Log("#Max:The level I'm getting: " + i_level);
	}

	[RPC]
	void SyncChapter(int i_chapter)
	{	
		_currentChapter = i_chapter;
		//Debug.Log("#Max:The Chapter I'm getting: " + i_chapter);
	}
	
	[RPC]
	void BackToLevelSelection()
	{
		_selectedLevel = 0;
		_currentChapter = 0;
	}

	[RPC]
	public void ForceShutDown()
	{
		_showRoomInterface = false;
		_flow.ReturnToLobbyFromRoom();
		ExitCleanUp();
	}
	
	[RPC]
	private void RemoveMyExistence(int i_playerID)
	{
		_playerIDs.Remove(i_playerID);
	}

	
	[RPC]
	void RequestServerUpdate(bool i_bool)
	{
		_requireUpdate = i_bool;
	}
	#endregion

	void OnGUI()
	{
		if(_showRoomInterface)
		{
			int connections = _playerUtil.GetComponent<AccountSystem>().GetConnectionLength();

			RoomInterfaceLoop(connections);
			
		}
	}
	
	private void RoomInterfaceLoop(int id)
	{
		//show player list
		ShowBackGroundAndRole();
		ShowLevelSelectionInterface();

//		if(!_levelSelected)
//		{
//			ShowLevelSelectionInterface();
//		}                                                                                                                                                                                                                                   
//		else
//		{
//			ShowLevelDescriptionInterface();
//		}
	}
	
	void ShowLevelSelectionInterface()
	{
		ScreenHelper.DrawText(22, 6, 24, 1, _chapterPrefixs[_currentChapter], 30, Color.green);

		for(int i = 0; i < _Areas.Length; i++)
		{
			if(i == _currentChapter)
			{
				ScreenHelper.DrawTexture(19 + i*5, 1, 4, 4, _Areas[i]);
				ScreenHelper.DrawTexture(18 + i*5, 0, 6, 4, _AreaHighLight);
			}
			else
			{
				if(Network.isServer)
				{
					if(ScreenHelper.DrawButton(19 + i*5, 1, 4, 4, _Areas[i], OnHoverAreaButton))
					{
						_currentChapter = i;
						_currentSelectionIndex = 0;
						networkView.RPC("SyncChapter", RPCMode.All, _currentChapter);
						networkView.RPC("SyncLevelSelection", RPCMode.All, _currentSelectionIndex, _levelDict[_currentChapter][0].Index);
						ReRegisterRoom();
					}
				}
				else
				{
					//ScreenHelper.DrawTexture(19 + i*5, 1, 4, 4, _Areas[i]);
				}
			}
		}

		//Debug.Log ("Current Chapter is: " + _currentChapter + " _chapterPrefix Length is: " + (_chapterPrefixs.Length-1) );

		DrawLevels();
	}

	void OnHoverAreaButton(int i_x, int i_y, int i_width, int i_height)
	{
		ScreenHelper.DrawTexture(i_x-1, i_y-1, i_width+2, i_height+1, _AreaMouseOver);
	}

	void OnHoverLevelButton(int i_x, int i_y, int i_width, int i_height)
	{
		ScreenHelper.DrawTexture(i_x, i_y, i_width, i_height, _LevelMouseOver);
	}

	void DrawLevels()
	{
		if ( _levelDict.ContainsKey( _currentChapter ) )
		{
			//if it is server, let him select
			if(Network.isServer)
			{
				for(int i = 0; i < _levelDict[_currentChapter].Count; i++)
				{
					if(i != _currentSelectionIndex)
					{
						if(ScreenHelper.DrawButton(18 + (i%3)*12, 8 + ((int)(i/3))*10, 8, 8, _levelDict[_currentChapter][i].LevelThumbnail, OnHoverLevelButton))
						{
							networkView.RPC("SyncLevelSelection", RPCMode.All, i, _levelDict[_currentChapter][i].Index);
							ReRegisterRoom();
						}
						ScreenHelper.DrawText(18 + (i%3)*12, 16 + ((int)(i/3))*10, 8, 1, _levelDict[_currentChapter][i].LevelName, 20, Color.white);
					}
					else
					{
						ScreenHelper.DrawTexture(18 + (i%3)*12, 8 + ((int)(i/3))*10, 8, 8, _levelDict[_currentChapter][i].LevelThumbnail);
						ScreenHelper.DrawTexture(18 + (i%3)*12, 8 + ((int)(i/3))*10, 8, 8, _LevelHighLight);
						ScreenHelper.DrawText(18 + (i%3)*12, 16 + ((int)(i/3))*10, 8, 1, _levelDict[_currentChapter][i].LevelName, 20, Color.white);
					}
				}
			}
			else //otherwise let him watch
			{
				for(int i = 0; i < _levelDict[_currentChapter].Count; i++)
				{
					if(i != _currentSelectionIndex)
					{
						//ScreenHelper.DrawTexture(18 + (i%3)*12, 8 + ((int)(i/3))*10, 8, 8, _levelDict[_currentChapter][i].LevelThumbnail);

						//ScreenHelper.DrawText(18 + (i%3)*12, 16 + ((int)(i/3))*10, 8, 1, _levelDict[_currentChapter][i].LevelName, 20, Color.white);
					}
					else
					{
						//ScreenHelper.DrawTexture(18 + (i%3)*12, 8 + ((int)(i/3))*10, 8, 8, _levelDict[_currentChapter][i].LevelThumbnail);
						//ScreenHelper.DrawTexture(18 + (i%3)*12, 8 + ((int)(i/3))*10, 8, 8, _LevelHighLight);
						//ScreenHelper.DrawText(18 + (i%3)*12, 16 + ((int)(i/3))*10, 8, 1, _levelDict[_currentChapter][i].LevelName, 20, Color.white);
						ScreenHelper.DrawTexture(26, 8, 16, 16, _levelDict[_currentChapter][i].LevelDetail);
						ScreenHelper.DrawTexture(26, 8, 16, 16, _LevelHighLight);
						ScreenHelper.DrawText(26, 24, 16, 4, _levelDict[_currentChapter][i].LevelName, 20, Color.white);
					}
				}
			}
		}
		else
		{
			//Debug.LogError ("THERE ARE NO LEVELS FOR THIS CHAPTER!!!");
		}
	}
	
	
//	void ShowLevelDescriptionInterface()
//	{	
//		ScreenHelper.DrawBlueText(Detail_LevelName_X, Detail_LevelName_Y, Detail_LevelName_Width, Detail_LevelName_Height, _levelDetails[_selectedLevel].LevelName, Detail_LevelName_FontSize);
//		ScreenHelper.DrawGrayText(Detail_LevelDesc_X, Detail_LevelDesc_Y, Detail_LevelDesc_Width, Detail_LevelDesc_Height, _levelDetails[_selectedLevel].Description, Detail_LevelDesc_FontSize);
//		ScreenHelper.DrawTexture(Detail_LevelImage_X, Detail_LevelImage_Y, Detail_LevelImage_Width, Detail_LevelImage_Height, _levelDetails[_selectedLevel].LevelDetail);
//		ScreenHelper.DrawBlueText(Detail_Par_X, Detail_ETime_Par_Y, Detail_Par_Width, Detail_Par_Height, "Estimated Completion Time:", Detail_Par_FontSize);
//		ScreenHelper.DrawBlueText(Detail_Par_X, Detail_Transmitter_Par_Y, Detail_Par_Width, Detail_Par_Height, "Start Transmitters:", Detail_Par_FontSize);
//		ScreenHelper.DrawBlueText(Detail_Par_X, Detail_Difficulty_Par_Y, Detail_Par_Width, Detail_Par_Height, "Security Difficulty:", Detail_Par_FontSize);
//		ScreenHelper.DrawGrayText(Detail_Val_X, Detail_ETime_Val_Y, Detail_Par_Width, Detail_Par_Height, _levelDetails[_selectedLevel].EstimatedTime, Detail_Val_FontSize);
//		ScreenHelper.DrawGrayText(Detail_Val_X, Detail_Transmitter_Val_Y, Detail_Val_Width, Detail_Val_Height, _levelDetails[_selectedLevel].TransmitterNumber.ToString(), Detail_Val_FontSize);
//		ScreenHelper.DrawGrayText(Detail_Val_X, Detail_Difficulty_Val_Y, Detail_Val_Width, Detail_Val_Height, _levelDetails[_selectedLevel].Difficulty, Detail_Val_FontSize);
//		
//		if(Network.isServer && ScreenHelper.DrawButton(BackButton_X, BackButton_Y, BackButton_Width, BackButton_Height, _backButton))
//		{
//			networkView.RPC("BackToLevelSelection", RPCMode.All);
//			ReRegisterRoom();
//		}
//	}
	
	void ShowBackGroundAndRole()
	{
		int connections = _playerUtil.GetComponent<AccountSystem>().GetConnectionLength();
		
		#region BackGround
		ScreenHelper.DrawTexture(0, 0, 64, 36, _RoomInterfaceBackground);
		ScreenHelper.SlideInTexture(0, 36, 0, 0, 14, 36, _leftWindowBkg, _currentTicker, 0.5f, 0.0f, 0.0f);
		ScreenHelper.SlideInTexture(15, -29, 15, 0, 48, 29, _rightWindowBkg, _currentTicker, 0.5f, 0.0f, 0.0f);
		#endregion
		
		#region RoleSelection
		//ScreenHelper.DrawBlueText(RoleText_X, RoleAndNameText_Y, RoleText_Width, 2, "Select Your Role:", RoleAndNameText_FontSize);
		//ScreenHelper.DrawBlueText(AccompliceText_X, RoleAndNameText_Y, AccompliceText_Width, 2, "Your Accomplice:", RoleAndNameText_FontSize);

		if((connections == 2 && _playerIDs.Count == 2) || connections == 1 || connections == 0)
		{
			if(_justStarted)
			{
				//do nothing
			}
			else
			{
				if(Network.connections.Length == 0)
				{
					ScreenHelper.DrawText(2, 11.5f, 10, 2, "Waiting...", 30, Color.green);
				}
				else
				{
					if(Network.isServer)
					{
						ScreenHelper.DrawText(2, 11.5f, 10, 2, AccountSystem.ClientPlayerName, 30, Color.green);
					}
					else
					{
						ScreenHelper.DrawText(2, 11.5f, 10, 2, AccountSystem.ServerPlayerName, 30, Color.green);
					}
				}

				if(Network.isServer)
				{
					ShowRoleSelection();
				}
				else
				{
					//Debug.Log("Max#: PlayerID:" + _playerID);
					if(_playerRoles[_playerID] == 1)
					{
						ScreenHelper.DrawTexture(2, 4, 11, 3, _thiefButtonActive);
					}
					else
					{
						ScreenHelper.DrawTexture(2, 4, 11, 3, _hackerButtonActive);
					}
				}
			}
		} // the first if
		#endregion
		
		#region Start and Cancel Button
		//if it is server, show startgame option
		if(Network.isServer)
		{				
			if(Network.connections.Length == 1)
			{
				if(ScreenHelper.SlideInButton(64, 30, 34, 30, 30, 4, _startButtonActive, _startButtonNormal, _currentTicker, 0.5f, 0.0f, 0.0f))
				{
					networkView.RPC("StartGame", RPCMode.All, _selectedLevel);
				}				
			}
			else
			{
				ScreenHelper.SlideInTexture(64, 30, 34, 30, 30, 4, _startButtonDisable, _currentTicker, 0.5f, 0.0f, 0.0f);
			}
			
			if(ScreenHelper.SlideInButton(-20, 32, 0, 32, 20, 3, _mainMenuButtonActive, _mainMenuButtonNormal, _currentTicker, 0.5f, 0.0f, 0.0f))
			{
				ShutDown();
			}
		}
		else
		{
			if(ScreenHelper.SlideInButton(-20, 32, 0, 32, 20, 3, _mainMenuButtonActive, _mainMenuButtonNormal, _currentTicker, 0.5f, 0.0f, 0.0f))
			{
				ShutDown();
			}
		}
		#endregion
	}
	
	void ShowRoleSelection()
	{
		if(_playerRole == 1)
		{
			if(ScreenHelper.DrawButton(2, 6, 11, 3, _hackerButtonActive, _hackerButtonNormal))
			{
				_playerRole = 2;
				networkView.RPC("SyncPlayerRole", RPCMode.All, _playerID, _playerRole);
				
				if(Network.isServer)
				{
					ReRegisterRoom();
				}
			}
			ScreenHelper.DrawTexture(2, 3, 11, 3, _thiefButtonActive);
		}
		else
		{
			if(ScreenHelper.DrawButton(2, 3, 11, 3, _thiefButtonActive, _thiefButtonNormal))
			{
				_playerRole = 1;
				networkView.RPC("SyncPlayerRole", RPCMode.All, _playerID, _playerRole);
				
				if(Network.isServer)
				{
					ReRegisterRoom();
				}
			}
			ScreenHelper.DrawTexture(2, 6, 11, 3, _hackerButtonActive);
		}
	}
	
	void ShowLevelDesc()
	{
		/*
		GUI.skin = customSkin;
		GUI.Label(new Rect(235, 40, 100, 20), "Map:");
		//GUI.Label(new Rect(Screen.width/2 + 140, Screen.height/2 - 200 + 25, 150, 20), _levelContent[_selectedLevel]);
		//guiTexture.texture = _levelDetails.LevelInfos[_selectedLevel].LevelTexture.texture;
		//guiTexture.pixelInset = new Rect(Screen.width/2 + 120, Screen.height/2 - 200 + 80, 100, 100);
		GUI.DrawTexture(new Rect(235,40 + 25, 200, 150), _levelDetails.LevelInfos[_selectedLevel].LevelTexture.texture);
		GUI.Label(new Rect(235, 40 + 180, 100, 30), "Description:");
		GUI.Label(new Rect(235, 40 + 195, 200, 30), _levelDetails.LevelInfos[_selectedLevel].Description);
		
		if(_selectedLevel != 0)
		{
			if(_playerRole == 1)
			{
				int score = _playerUtil.GetComponent<PlayerProfile>().PointmanScorePerLevel[_selectedLevel - 1];
				GUI.Label(new Rect(235, 40 + 215, 200, 30), "Best Pointman Score: " 
					+ (score==0? "None":score.ToString()));
			}
			else if(_playerRole == 2)
			{
				int score = _playerUtil.GetComponent<PlayerProfile>().HackerScorePerLevel[_selectedLevel - 1] ;
				GUI.Label(new Rect(235, 40 + 215, 200, 30), "Best Hacker Score: " 
					+ (score==0? "None":score.ToString()));			
			}
		}
		*/
	}
	
	void ReRegisterRoom()
	{
		string roleNeeded = null;
		
		if(_playerRole == 0)
		{
			roleNeeded = _roleContent[_playerRole];
		}
		else
		{
			roleNeeded = _roleContent[3- _playerRole];
		}
		
		if(_flow.IsLANGame)
		{
			if(_playerIDs.Count < 2)
			{
				_flow.LANBroadcastRoomMessage(PlayerProfile.PlayerName + "'s Game#" + roleNeeded + "#" + _chapterPrefixs[_currentChapter] +": " + _levelDetails[_selectedLevel].LevelName + "#true#false");
			}
			else
			{
				_flow.LANBroadcastRoomMessage(PlayerProfile.PlayerName + "'s Game#" + roleNeeded + "#" + _chapterPrefixs[_currentChapter] +": " + _levelDetails[_selectedLevel].LevelName + "#true#true");
			}
		}
		else
		{
			MasterServer.UnregisterHost();
			//temporary disable, don't Remove!!!!!!!!!!!!!!!!!!!!!!
			
			//int i_awesomeness = _playerRole == 1? _playerUtil.GetComponent<PlayerProfile>().PointmanOverallScore: _playerUtil.GetComponent<PlayerProfile>().HackerOverallScore;
			//string s_awesomeness = _playerRole == 0? "------" : CheckSkillLevel(i_awesomeness);
			
			//Debug.Log("#Max: I'm trying to reg here: " + PlayerProfile.PlayerName);
			//Debug.Log("#Max: " + PlayerProfile.PlayerName + "'s Game");
			//Debug.Log("GameTypeGame:" + _flow.GameTypeName);
			//CH16 is to be used for Steam release of CH
			MasterServer.RegisterHost("CH16", PlayerProfile.PlayerName + "'s Game", roleNeeded + "#" + _chapterPrefixs[_currentChapter]+": " + _levelDetails[_selectedLevel].LevelName);
		}
	}
	
	string CheckPlayerRole(int i_role)
	{
		switch(i_role)
		{
		case 0:
			return "Undecided";
		case 1:
			return "PointMan ";
		case 2:
		default:
			return "Hacker   ";
		}
	}
	string CheckSkillLevel(int i_skill)
	{
		switch(i_skill)
		{
		case 0:
			return "Novice  ";
		case 1:
		default:
			return "Mediocre";
		}
	}
	
	
	[RPC]
	void StartGame(int i_selectedLevel)
	{
		GameManager.Manager.PlayerType = _playerRole;
		_gameStarted = true;
		if(_flow.IsLANGame)
		{
			_flow.LANBroadcastRoomMessage(PlayerProfile.PlayerName + "'s Game#Undecided#" + _chapterPrefixs[_currentChapter] +": " + _levelDetails[_selectedLevel].LevelName + "#false#true");
		}
		
		// omitted code
		//Network.SetSendingEnabled(0, false);
		//Network.isMessageQueueRunning = false;
		GameManager.Manager.CurrentLevelTexture =  _levelDetails[_selectedLevel].LevelDetail;
		GameManager.Manager.CurrentLevelName = GameManager.Manager.LevelNames[i_selectedLevel];
		GameManager.Manager.InStartMenu = true;
		Application.LoadLevel("LevelTransition");
			
		// Allow receiving data again
		//Network.isMessageQueueRunning = true;
		// Now the level has been loaded and we can start sending out data
		//Network.SetSendingEnabled(0, true);
		//GameManager.Manager.LevelName = "Levels/" + _levelContent[i_selectedLevel];
		
	}
	

	
}