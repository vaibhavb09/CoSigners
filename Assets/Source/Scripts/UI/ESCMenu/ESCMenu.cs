using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;

public class ESCMenu {
	
	private List<ESCContent> _menuContents  = new List<ESCContent>();
	private GameObject _playerUtil;
	private HackerGUI _hackerGUI;
	private int _currentContentIndex = 0;
	private int _currentPlayerType = 1;
	private float _constantTicker = 0.0f;
	private bool _showLevelImage = true;
	private bool _isSlidingIn = true;

	#region ForGUI
	//private Texture2D ESCBackGround;
	public  Texture2D DisplayBackGround;
	private Texture2D TextBackGround;


	private Texture2D ControlButtonActive;
	private Texture2D ControlButtonNormal;

	private Texture2D ExitButtonActive;
	private Texture2D ExitButtonNormal;

	private Texture2D LevelSelectButtonActive;
	private Texture2D LevelSelectButtonNormal;
	
	private Texture2D RestartButtonActive;
	private Texture2D RestartButtonNormal;
	private Texture2D RestartButtonDisabled;
	
	private Texture2D ReturnButtonActive;
	private Texture2D ReturnButtonNormal;
	
	private Texture2D HackerControl;
	private Texture2D ThiefControl;

	#endregion
	
	public void LoadESCMenuContent()
	{
		_playerUtil = GameObject.Find("PlayerUtil");
		_hackerGUI = GameObject.Find("TopDownCamera").GetComponent<HackerGUI>();
		//ESCBackGround = Resources.Load("Textures/ESCMenu/PauseMenu_PopUp_Frame", typeof(Texture2D)) as Texture2D;
		DisplayBackGround 	= Resources.Load("Textures/NewESCMenu/DarkenBakcground", typeof(Texture2D)) as Texture2D;
		TextBackGround 		= Resources.Load("Textures/NewESCMenu/Menu_MainWindow", typeof(Texture2D)) as Texture2D;

		ControlButtonActive 			= Resources.Load("Textures/NewESCMenu/MenuBtn_Controls_active", typeof(Texture2D)) as Texture2D;
		ControlButtonNormal 			= Resources.Load("Textures/NewESCMenu/MenuBtn_Controls_norm", typeof(Texture2D)) as Texture2D;
		ExitButtonActive 			= Resources.Load("Textures/NewESCMenu/MenuBtn_Exit_active", typeof(Texture2D)) as Texture2D;
		ExitButtonNormal 			= Resources.Load("Textures/NewESCMenu/MenuBtn_Exit_norm", typeof(Texture2D)) as Texture2D;
		LevelSelectButtonActive 	= Resources.Load("Textures/NewESCMenu/MenuBtn_LevelSelect_active", typeof(Texture2D)) as Texture2D;
		LevelSelectButtonNormal 	= Resources.Load("Textures/NewESCMenu/MenuBtn_LevelSelect_norm", typeof(Texture2D)) as Texture2D;
		RestartButtonActive = Resources.Load("Textures/NewESCMenu/MenuBtn_Restart_active", typeof(Texture2D)) as Texture2D;
		RestartButtonDisabled = Resources.Load("Textures/NewESCMenu/MenuBtn_Restart_norm", typeof(Texture2D)) as Texture2D; 
		RestartButtonNormal = Resources.Load("Textures/NewESCMenu/MenuBtn_Restart_norm", typeof(Texture2D)) as Texture2D;
		ReturnButtonActive 	= Resources.Load("Textures/NewESCMenu/MenuBtn_Resume_active", typeof(Texture2D)) as Texture2D;
		ReturnButtonNormal 	= Resources.Load("Textures/NewESCMenu/MenuBtn_Resume_norm", typeof(Texture2D)) as Texture2D;

		HackerControl 	= Resources.Load("Textures/NewESCMenu/hackerControls", typeof(Texture2D)) as Texture2D;
		ThiefControl 	= Resources.Load("Textures/NewESCMenu/thiefControls", typeof(Texture2D)) as Texture2D;

			
		//ParseESCMenuFile();
		_currentPlayerType = GameManager.Manager.PlayerType;
		_currentContentIndex = _currentPlayerType == 2? 0: 1;
	}
	
	private void ParseESCMenuFile()
	{
		TextAsset menuText = (TextAsset) Resources.Load("Levels/ESCMenu");
		
		using (TextReader reader = new StringReader((string)menuText.text))
		{
			int index = 0;
			while(reader.Peek() >= 0)
			{
				// ---- Menu Data - Max - 10/22/13
				// - 0.Name of the Item
				// - 1.Path for the item image
				// - 2.Item Description
				// - 3.Role
				// - 4.Thumbnail Image
				// - 5.Reticle Image
				
				string[] menuData = reader.ReadLine().Split("#".ToCharArray());
				ESCContent thisMenu = new ESCContent();
				thisMenu.ItemName 		= menuData[0];
				//thisMenu.MenuImage = Resources.Load(menuData[3], typeof(Texture2D)) as Texture2D;
				thisMenu.ItemThumbnail 	= Resources.Load(menuData[1], typeof(Texture2D)) as Texture2D;
				thisMenu.ItemDesciption = menuData[2];
				thisMenu.Role 			= Convert.ToInt32(menuData[3]);
				//thisMenu.ThumbnaiImage 	= Resources.Load(menuData[4], typeof(Texture2D)) as Texture2D;
				//thisMenu.ReticleImage = Resources.Load(menuData[5], typeof(Texture2D)) as Texture2D;
				_menuContents.Add(thisMenu);
			}
			
			reader.Close();
		}
	}
	
	public void show()
	{
		GUI.depth = 1;
		DrawBackGroundStuff();
		DrawMenuControlButtons();
		//DrawItemMenus();
	}

	public void Update(float i_deltaTime)
	{
		if(_isSlidingIn)
			_constantTicker += i_deltaTime;
		else
		{
			_constantTicker -= i_deltaTime;
			if(_constantTicker < 0.0f)
			{
				NetworkManager.Manager.ShutDownESCMenu();
				//_hackerGUI.ShutDownESCMenu();
			}
		}
	}

	public void ResetTicker(bool i_slideIn)
	{
		_isSlidingIn = i_slideIn;
		if(_isSlidingIn)
			_constantTicker = 0.0f;
		else
			_constantTicker = _constantTicker>1.5f? 1.5f : _constantTicker;
	}

	private void DrawBackGroundStuff()
	{
		ScreenHelper.DrawTexture(0, 0, 64, 36, DisplayBackGround);
		ScreenHelper.SlideInTexture(12, 36, 12, 2, 40, 34, TextBackGround, _constantTicker, 0.5f, 0.75f, 0.0f);
		if(_showLevelImage)
			ScreenHelper.SlideInTexture(20, 38, 20, 4, 24, 24, GameManager.Manager.CurrentLevelTexture, _constantTicker, 0.5f, 0.75f, 0.0f);
		else
		{
			if(_currentPlayerType == 1)
			{
				ScreenHelper.SlideInTexture(16, 38, 16, 4, 32, 22, ThiefControl, _constantTicker, 0.5f, 0.75f, 0.0f);
			}
			else
			{
				ScreenHelper.SlideInTexture(16, 38, 16, 4, 32, 22, HackerControl, _constantTicker, 0.5f, 0.75f, 0.0f);
			}
		}
		//ScreenHelper.DrawTexture(0, 2, 32, 8, SelectFrame);
		
//		if(_currentPlayerType == 1)
//		{
//			ScreenHelper.DrawTexture(14, 4, 8, 2, ThiefButtonActive);
//			if(ScreenHelper.DrawButton(5, 4, 8, 2, HackerButtonActive, HackerButtonInactive))
//			{
//				_currentPlayerType = 2;
//			}
//		}
//		
//		if(_currentPlayerType == 2)
//		{
//			if(ScreenHelper.DrawButton(14, 4, 8, 2, ThiefButtonActive,ThiefButtonInactive))
//			{
//				_currentPlayerType = 1;
//			}
//			ScreenHelper.DrawTexture(5, 4, 8, 2, HackerButtonActive);			
//		}
	}
	
	private void DrawMenuControlButtons()
	{
		//ScreenHelper.DrawTexture(0, 30, 64, 4, ButtonAccent);
		if(ScreenHelper.SlideInButton(-16, 29, 0, 29, 16, 3, ExitButtonActive, ExitButtonNormal, _constantTicker, 0.5f, 0.0f, 0.0f))
		{
			Time.timeScale = 1;
			Screen.lockCursor = false;
			if(Network.isServer)
			{
				MasterServer.UnregisterHost();
			}
			_playerUtil.GetComponent<AccountSystem>().ResetNamesAfterForceShutDown();
			Network.Disconnect();									
			Application.LoadLevel(0);
		}

		//ScreenHelper.DrawTexture(0, 30, 64, 4, ButtonAccent);

		if(ScreenHelper.SlideInButton(-22, 26, 0, 26, 22, 3, ControlButtonActive, ControlButtonNormal, _constantTicker, 0.5f, 0.4f, 0.0f))
		{
			_showLevelImage = !_showLevelImage;
		}

		if(Network.connections.Length == 0)
		{
			ScreenHelper.SlideInTexture(-26, 32, 0, 32, 26, 3, LevelSelectButtonNormal, _constantTicker, 0.5f, 0.4f, 0.0f);
			ScreenHelper.SlideInTexture(64, 27, 36, 27, 28, 3, RestartButtonNormal, _constantTicker, 0.5f, 0.2f, 0.0f);
		}
		else
		{
			if(ScreenHelper.SlideInButton(-26, 32, 0, 32, 26, 3, LevelSelectButtonActive, LevelSelectButtonNormal, _constantTicker, 0.5f, 0.2f, 0.0f))
			{
				Time.timeScale = 1;
				Screen.lockCursor = false;	
				NetworkManager.Manager.LoadLevel(0);
				//Application.LoadLevel(0);
			}

			if(ScreenHelper.SlideInButton(64, 27, 36, 27, 28, 3, RestartButtonActive, RestartButtonNormal, _constantTicker, 0.5f, 0.2f, 0.0f))
			{
				if( GameManager.Manager.PlayerType == 1 )
				{
					Screen.lockCursor = true;	
				}
				NetworkManager.Manager.RestartLevel();
			}
		}
		
		if(ScreenHelper.SlideInButton(64, 31, 40, 31, 24, 3, ReturnButtonActive, ReturnButtonNormal, _constantTicker, 0.5f, 0.6f, 0.0f))
		{
			NetworkManager.Manager.ToggleESCMenu();
		}
	}
	
//	private void DrawItemMenus()
//	{
//		int index = 0;
//		for(int i = 0; i < _menuContents.Count; i ++)
//		{
//			if(_menuContents[i].Role == _currentPlayerType)
//			{
//				if(_currentContentIndex != i)
//				{
//					if(ScreenHelper.DrawButton(8, 9 + 2 * index, 15, 2, ItemMenuBackgroundActive, ItemMenuBackgroundNormal))
//					{
//						_currentContentIndex = i;
//					}
//					ScreenHelper.DrawText(10, 9 + 2 * index, 15, 2, _menuContents[i].ItemName, 20, new Color(0.0f,0.0589f,0.157f), TextAnchor.MiddleLeft, false);
//				}
//				else
//				{
//					ScreenHelper.DrawTexture(8, 9 + 2 * index, 15, 2, ItemMenuBackgroundPressed);
//					ScreenHelper.DrawText(10, 9 + 2 * index, 15, 2, _menuContents[i].ItemName, 20, new Color(0.0f,0.0589f,0.157f), TextAnchor.MiddleLeft, false);
//				}
//				//ScreenHelper.DrawTexture(5, 9 + 2 * index, 2, 2, _menuContents[i].ThumbnaiImage);
//				index++;
//			}
//		}
//		
//		//ScreenHelper.DrawText(17, 4, 20, 2, _menuContents[_currentContentIndex].ItemName, 50, Color.gray, TextAnchor.MiddleLeft, false);
//		ScreenHelper.DrawTexture(24, 4, 30, 16, _menuContents[_currentContentIndex].ItemThumbnail);
//		ScreenHelper.DrawText(24, 22, 16, 8, _menuContents[_currentContentIndex].ItemDesciption, 20, new Color(0.0f,0.0589f,0.157f), TextAnchor.UpperLeft, true);
//
//	}
}
