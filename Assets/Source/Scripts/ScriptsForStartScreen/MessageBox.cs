using UnityEngine;
using System.Collections;

public class MessageBox : Photon.MonoBehaviour {
	
	//int numberOfScenes = 6;
	
	public GUISkin MessagingGUISkin;
	public GUIStyle MessagingGUIStyle;
	public string  mystring = null;
	public Vector2 BoxPosition;
	public int M_BoxHeight;
	public int M_BoxWidth;
	public int GUItype;
	bool pause;
	bool willpause;
	private bool showGUI;
	bool TutorialToggle;
	//public GUIStyle myStyle;
	int m_delta_timeout;
	float _constantTicker = 0.0f;
	
	public bool replayHacker = false;
	public bool replayThief = false;
	public bool nxtLevelHacker = false;
	public bool nxtLevelThief = false;
	public bool mainMenuHacker = false;
	public bool mainMenuThief = false;
	public bool goActivated = false;
	
	
	public int levelGUIWidth;
	public int levelGUIHeight;

	public static Color yellow = new Color(1.0f, 0.745f, 0.353f, 1.0f);

	private GameObject _playerUtil;
	
	#region EndScreen
	private Texture2D EndScreenBackGround;

	private Texture2D ContinueButtonActive;
	private Texture2D ContinueButtonNormal;

	private Texture2D LevelSelectButtonActive;
	private Texture2D LevelSelectButtonNormal;

	private Texture2D ExitButtonActive;
	private Texture2D ExitButtonNormal;

	private Texture2D RestartButtonActive;
	private Texture2D RestartButtonNormal;

	#endregion
	
	#region FailScreen
	private Texture2D FailedBackGround;
	
	private Texture2D FailedRestartButtonActive;
	private Texture2D FailedRestartButtonNormal;
	
	private Texture2D FailedMenuButton;
	
	private Texture2D HackerTip1;
	private Texture2D HackerTip2;
	private Texture2D HackerTip3;
	private Texture2D ThiefTip1;
	private Texture2D ThiefTip2;
	private Texture2D ThiefTip3;

	private Texture2D GameStatBG;
	
	private RewardMenu _rewardMenu;
	private bool _showRewardMenu = false;
	#endregion
	
	
	// Use this for initialization
	void Start () {
		showGUI = false;
		m_delta_timeout = 10;
		_playerUtil = GameObject.Find("PlayerUtil");
		LoadTextures();
		_rewardMenu = new RewardMenu(true);
		_rewardMenu.LoadRewardMenuContent();
		if( GameManager.Manager.PlayerType == 1 )
			Screen.lockCursor = true;
		
	}
	
	void LoadTextures()
	{
		EndScreenBackGround = Resources.Load("Textures/NewVictoryScreen/victoryMainWindow") as Texture2D;

		ContinueButtonActive = Resources.Load("Textures/NewVictoryScreen/btn_Continue_active") as Texture2D;
		ContinueButtonNormal = Resources.Load("Textures/NewVictoryScreen/btn_Continue_norm") as Texture2D;

		RestartButtonActive 	= Resources.Load("Textures/NewEscMenu/MenuBtn_Restart_active") as Texture2D;
		RestartButtonNormal 	= Resources.Load("Textures/NewEscMenu/MenuBtn_Restart_norm") as Texture2D;

		FailedRestartButtonActive 	= Resources.Load("Textures/NewFailScreen/btn_Restart_active") as Texture2D;
		FailedRestartButtonNormal 	= Resources.Load("Textures/NewFailScreen/btn_Restart_norm") as Texture2D;

		LevelSelectButtonActive = Resources.Load("Textures/NewEscMenu/MenuBtn_LevelSelect_active") as Texture2D;
		LevelSelectButtonNormal = Resources.Load("Textures/NewEscMenu/MenuBtn_LevelSelect_norm") as Texture2D;

		ExitButtonActive = Resources.Load("Textures/NewEscMenu/MenuBtn_Exit_active") as Texture2D;
		ExitButtonNormal = Resources.Load("Textures/NewEscMenu/MenuBtn_Exit_norm") as Texture2D; 

		FailedBackGround 			= Resources.Load("Textures/NewFailScreen/FailWindowBackground") as Texture2D;
	}
	
	void Pausethegame()		
	{
		if(pause)
		{
			Time.timeScale = 0;
			GameObject.Find("Playertheif(Clone)").GetComponent<MouseLookAround>().enabled = false;
		}
	}
	
	void DisableHackerAndThiefInput()
	{
		//Time.timeScale = 0;
		if(GameManager.Manager.PlayerType == 1)
		{
			//GameObject.Find("Playertheif(Clone)").GetComponent<MouseLookAround>().enabled = false;
			//Screen.lockCursor = false;
			ThiefManager.Manager.DisableThiefActions();
		}
		else
		{
			//GameObject.Find("TopDownCamera").GetComponent<HackerActions>().DisableHackerActions();
			HackerManager.Manager.DisableHackerActions();
		}
		
	}
	
	void EnableHackerAndThiefInput()
	{
		//Time.timeScale = 1;
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
	
	public void MessageBoxShow (int M_type)
	{
		GUItype = M_type;
		_constantTicker = 0.0f;
		// END menu; Reward Menu;
		/*
		if(Application.loadedLevel == 12 && M_type == 2) //hacky, to be removed
		{
			DisableHackerAndThiefInput();
			//_rewardMenu.LoadRewardMenuContent();
			HackerManager.Manager.gameEnded = true;
			HackerManager.Manager.DisableESCMenu();
			_showRewardMenu = true;
		}
		*/
		if(Application.loadedLevelName.CompareTo("JM_53") == 0  && M_type == 2 )
		{
			NetworkManager.Manager.LoadLevel("Credits");
		}
		else
		{
			showGUI = true;
		}
		
		HackerThreat.Manager.active = false;
		
		
		// if M_type = 1 then level fail, if M_Type = 2 level success
		// [SOUND_TAG] [Level_End]
		if(M_type == 1)
		{
			NetworkManager.Manager.PauseGame(true);

			soundMan.soundMgr.silenceSource(null,GameManager.Manager.PlayerType);
			
			soundMan.soundMgr.playOneShotOnSource(null,"Level_End_Fail",GameManager.Manager.PlayerType,GameManager.Manager.PlayerType);
		}
		else if(M_type == 2)
		{
			NetworkManager.Manager.PauseGame(true);

			soundMan.soundMgr.silenceSource(null,GameManager.Manager.PlayerType);
			
			soundMan.soundMgr.playOneShotOnSource(null,"Level_End_Success",GameManager.Manager.PlayerType,GameManager.Manager.PlayerType);
		}
		
		if(OverrideManager.Manager.IsActive)
		{
			OverrideManager.Manager.OverrideSuccess();
			HackerThreat.Manager.Active = false;
		}
		
	}
	
	public void ShowEndScreen()
	{
		if(_showRewardMenu)
		{
			_rewardMenu.Display();
		}
		if(showGUI)
		{
			Screen.lockCursor = false;
			DisableHackerAndThiefInput();
			HackerManager.Manager.gameEnded = true;
			HackerManager.Manager.DisableESCMenu();
			
			if(GUItype == 2)
			{
				DrawEndScreenBackground();
				DrawEndScreenSelection();
				DrawStats();
			}
			
			if(GUItype == 1)
			{
				DrawFailScreenBackground();
				DrawFailReplayButton();
			}
		}
	}
	
	void DrawFailReplayButton()
	{
		if(PhotonNetwork.playerList.Length == 0)
		{
			ScreenHelper.SlideInTexture(-26, 32, 0, 32, 26, 3, LevelSelectButtonNormal, _constantTicker, 0.5f, 0.4f, 0.0f);
			ScreenHelper.SlideInTexture(64, 31, 40, 31, 28, 3, RestartButtonNormal, _constantTicker, 0.5f, 0.2f, 0.0f);
		}
		else
		{
			if(ScreenHelper.SlideInButton(-26, 32, 0, 32, 26, 3, LevelSelectButtonActive, LevelSelectButtonNormal, _constantTicker, 0.5f, 0.2f, 0.0f))
			{
				Time.timeScale = 1;
				EnableHackerAndThiefInput();
				Screen.lockCursor = false;	
				NetworkManager.Manager.LoadLevel(0);
				GUItype = 0;
				showGUI = false;
			}
			
			if(ScreenHelper.SlideInButton(64, 31, 40, 31, 28, 3, FailedRestartButtonActive, FailedRestartButtonNormal, _constantTicker, 0.5f, 0.2f, 0.0f))
			{
				EnableHackerAndThiefInput();
				NetworkManager.Manager.PauseGame(false);
				NetworkManager.Manager.RestartLevel();
				GUItype = 0;
				showGUI = false;
			}
		}

	}

	void DrawStats()
	{	
		if ((BasicScoreSystem.Manager.LevelCompleteTime == 0) && (GameManager.Manager.PlayerType == 1))
		{
			NetworkManager.Manager.SetLevelCompletionTime(Time.timeSinceLevelLoad);
		}

		ScreenHelper.SlideInText(28, 47, 28, 13, 8, 2, ((int)(BasicScoreSystem.Manager.LevelCompleteTime/60)).ToString() + ":"
		                         + (BasicScoreSystem.Manager.LevelCompleteTime%60).ToString(), yellow, _constantTicker, 0.5f, 0.75f, 0.0f, TextAnchor.MiddleCenter, 45);
		ScreenHelper.SlideInText(28, 51, 28, 17, 3, 2, BasicScoreSystem.Manager.TimesSeen.ToString(), yellow, _constantTicker, 0.5f, 0.75f, 0.0f, TextAnchor.LowerLeft, 30);
		ScreenHelper.SlideInText(28, 53, 28, 19, 3, 2, BasicScoreSystem.Manager.HackerCaughtByTracer.ToString(), yellow, _constantTicker, 0.5f, 0.75f, 0.0f, TextAnchor.LowerLeft, 30);
		ScreenHelper.SlideInText(28, 55, 28, 21, 3, 2, BasicScoreSystem.Manager.LockdownsNeeded.ToString(), yellow, _constantTicker, 0.5f, 0.75f, 0.0f, TextAnchor.LowerLeft, 30);
		ScreenHelper.SlideInText(45, 51, 45, 17, 3, 2, BasicScoreSystem.Manager.TransmittersPlaced.ToString(), yellow, _constantTicker, 0.5f, 0.75f, 0.0f, TextAnchor.LowerLeft, 30);
		ScreenHelper.SlideInText(45, 53, 45, 19, 3, 2, BasicScoreSystem.Manager.LinksPlaced.ToString(), yellow, _constantTicker, 0.5f, 0.75f, 0.0f, TextAnchor.LowerLeft, 30);
		ScreenHelper.SlideInText(45, 55, 45, 21, 3, 2, BasicScoreSystem.Manager.NoOfSAPsActivated.ToString(), yellow, _constantTicker, 0.5f, 0.75f, 0.0f, TextAnchor.LowerLeft, 30);
	}

	void DrawFailScreenBackground()
	{
		ScreenHelper.SlideInTexture(12, 36, 12, 2, 40, 34, FailedBackGround, _constantTicker, 0.5f, 0.75f, 0.0f);
		if(ScreenHelper.SlideInButton(-16, 29, 0, 29, 16, 3, ExitButtonActive, ExitButtonNormal, _constantTicker, 0.5f, 0.0f, 0.0f))
		{
			showGUI = false;
			EnableHackerAndThiefInput();
			Screen.lockCursor = false;
			if(PhotonNetwork.isMasterClient)
			{
				MasterServer.UnregisterHost();
			}
			_playerUtil.GetComponent<AccountSystem>().ResetNamesAfterForceShutDown();
			//PlayerProfile.SavePlayerProfile();
			//PhotonNetwork.Disconnect();	
			//BasicScoreSystem.Manager.ResetData();
			Application.LoadLevel(0);
		}
	}
	
	void DrawEndScreenBackground()
	{
		ScreenHelper.SlideInTexture(12, 36, 12, 2, 40, 34, EndScreenBackGround, _constantTicker, 0.5f, 0.75f, 0.0f);
	}
	
	void DrawEndScreenSelection()
	{
		//exit button
		if(ScreenHelper.SlideInButton(-16, 29, 0, 29, 16, 3, ExitButtonActive, ExitButtonNormal, _constantTicker, 0.5f, 0.0f, 0.0f))
		{
			showGUI = false;
			Time.timeScale = 1;
			EnableHackerAndThiefInput();
			Screen.lockCursor = false;
			if(PhotonNetwork.isMasterClient)
			{
				MasterServer.UnregisterHost();
			}
			_playerUtil.GetComponent<AccountSystem>().ResetNamesAfterForceShutDown();
			//PhotonNetwork.Disconnect();									
			Application.LoadLevel(0);
		}

		if(PhotonNetwork.playerList.Length == 0)
		{
			ScreenHelper.SlideInTexture(-26, 32, 0, 32, 26, 3, LevelSelectButtonNormal, _constantTicker, 0.5f, 0.4f, 0.0f);
			ScreenHelper.SlideInTexture(64, 27, 36, 27, 28, 3, RestartButtonNormal, _constantTicker, 0.5f, 0.2f, 0.0f);
		}
		else
		{
			if(ScreenHelper.SlideInButton(-26, 32, 0, 32, 26, 3, LevelSelectButtonActive, LevelSelectButtonNormal, _constantTicker, 0.5f, 0.2f, 0.0f))
			{
				Time.timeScale = 1;	
				EnableHackerAndThiefInput();
				Screen.lockCursor = false;
				NetworkManager.Manager.LoadLevel(0);
				GUItype = 0;
				showGUI = false;
			}
			
			if(ScreenHelper.SlideInButton(64, 27, 36, 27, 28, 3, RestartButtonActive, RestartButtonNormal, _constantTicker, 0.5f, 0.2f, 0.0f))
			{
				EnableHackerAndThiefInput();
				NetworkManager.Manager.PauseGame(false);
				NetworkManager.Manager.RestartLevel();
				GUItype = 0;
				showGUI = false;
			}
		}

		if(ScreenHelper.SlideInButton(64, 31, 40, 31, 24, 3, ContinueButtonActive, ContinueButtonNormal, _constantTicker, 0.5f, 0.4f, 0.0f))
		{
			showGUI = false;
			Time.timeScale = 1;
			GameManager.Manager.CurrentLevelName = Application.loadedLevelName;
			NetworkManager.Manager.LoadLevel("LevelTransition");
		}
	}

	public void RenderMessageBox()
	{
		/*
			if( showGUI )
			{	

				Screen.lockCursor = false;
				Pausethegame();
					//TextureForBackground =Resources.Load("Textures/background_caught 1") as Texture2D;
					//TextureForMenuButton = Resources.Load("Textures/restart_button") as Texture2D;
					//TextureForRestartButton = Resources.Load ("Textures/restart_button") as Texture2D;
					//TextureForMessage = Resources.Load("Textures/message_caught") as Texture2D;
					//GUI.BeginGroup(new Rect((Screen.width/2-TextureForBackground.width/2),(Screen.height/2-TextureForBackground.height/2),TextureForBackground.width,TextureForBackground.height),TextureForBackground);
				//
				GUI.Box(new Rect(0,0,Screen.width,Screen.height), "Menu ");
				if (GUItype == 1)
				{
					GUI.Box(new Rect(Screen.width/2 - TextureForDrone.width/2 ,y3,TextureForDrone.width,TextureForDrone.height), TextureForDrone);
					GUI.Box(new Rect(Screen.width/2 - TextureForLoss.width/2 ,y4,TextureForLoss.width-w1,TextureForLoss.height-h1), TextureForLoss);
				}
				else if (GUItype == 2)
				{
					GUI.Box(new Rect(Screen.width/2 - TextureForLoss.width/2 ,y4,TextureForLoss.width-w1,TextureForLoss.height-h1), TextureForWin);
				}
       	GUI.BeginGroup(new Rect(Screen.width / 2 - levelEndMenuGroupWidth/2, Screen.height / 2 - levelEndMenuGroupHeight/2, levelEndMenuGroupWidth, levelEndMenuGroupHeight));
        GUI.Box(new Rect(0, 0, levelEndMenuGroupWidth, levelEndMenuGroupHeight), "Actions", MessagingGUIStyle);
			
				//GUI.Box(new Rect(Screen.width - x6,y6,x7,y7), "Actions");
			
			if(GameManager.Manager.PlayerType == 1)
			{
				////GUI.Label (new Rect (Screen.width - x6,y6 + 15,x7/2,20), "Thief (You)");
				////GUI.Label (new Rect (Screen.width - x6/2,y6 + 15,x7/2,20), "Hacker");
				GUI.Label (new Rect (x6 + ((levelEndMenuGroupWidth/x7)/2),y6 + (levelEndMenuGroupHeight/levelEndMenuYOffsetFactor) ,levelEndMenuGroupWidth/x7,levelEndMenuGroupHeight/y7), "Thief (You)");
				GUI.Label (new Rect (x6 + levelEndMenuGroupWidth/2 + ((levelEndMenuGroupWidth/x7)/2) ,y6 + (levelEndMenuGroupHeight/levelEndMenuYOffsetFactor),levelEndMenuGroupWidth/x7,levelEndMenuGroupHeight/y7), "Hacker");
			}
			else if (GameManager.Manager.PlayerType == 2)
			{
				GUI.Label (new Rect (x6 + ((levelEndMenuGroupWidth/x7)/2),y6 + (levelEndMenuGroupHeight/levelEndMenuYOffsetFactor) ,levelEndMenuGroupWidth/x7,levelEndMenuGroupHeight/y7), "Thief");
				GUI.Label (new Rect (x6 + levelEndMenuGroupWidth/2 + ((levelEndMenuGroupWidth/x7)/2) ,y6 + (levelEndMenuGroupHeight/levelEndMenuYOffsetFactor),levelEndMenuGroupWidth/x7,levelEndMenuGroupHeight/y7), "Hacker (You)");				
			}
				if (GUItype == 2)
				{
				setGUIColor(nxtLevelThief);

					if(GUI.Button(new Rect(x6,y6 + (levelEndMenuGroupHeight/levelEndMenuYOffsetFactor * 2),levelEndMenuGroupWidth/x7,levelEndMenuGroupHeight/y7),"Next Level"))
						{
							if (GameManager.Manager.PlayerType == 1)
							{
									nxtLevelThief = !nxtLevelThief;
		
									NetworkManager.Manager.endLevelMenuResponse(1,nxtLevelThief);
							}
						
						}
				
				setGUIColor(nxtLevelHacker);
						if(GUI.Button(new Rect(x6 + levelEndMenuGroupWidth/2,y6 + (levelEndMenuGroupHeight/levelEndMenuYOffsetFactor * 2),levelEndMenuGroupWidth/x7,levelEndMenuGroupHeight/y7),"Next Level"))
						{
							if (GameManager.Manager.PlayerType == 2)
							{
									nxtLevelHacker = !nxtLevelHacker;
									NetworkManager.Manager.endLevelMenuResponse(2,nxtLevelHacker);
							}
						
						}
				}
				
				setGUIColor(replayThief);
					if(GUI.Button(new Rect(x6,y6 + (levelEndMenuGroupHeight/levelEndMenuYOffsetFactor * 3),levelEndMenuGroupWidth/x7,levelEndMenuGroupHeight/y7),"Replay"))
						{
							if (GameManager.Manager.PlayerType == 1)
							{
									replayThief = !replayThief;
									NetworkManager.Manager.endLevelMenuResponse(3,replayThief);
							}
						}
				setGUIColor(replayHacker);

					if(GUI.Button(new Rect(x6 + levelEndMenuGroupWidth/2,y6 + (levelEndMenuGroupHeight/levelEndMenuYOffsetFactor * 3),levelEndMenuGroupWidth/x7,levelEndMenuGroupHeight/y7),"Replay"))
						{
							if (GameManager.Manager.PlayerType == 2)
							{
									replayHacker = !replayHacker;
									NetworkManager.Manager.endLevelMenuResponse(4,replayHacker);
							}
						}
				
				setGUIColor(mainMenuThief);
		
					if(GUI.Button(new Rect(x6,y6 + (levelEndMenuGroupHeight/levelEndMenuYOffsetFactor * 4),levelEndMenuGroupWidth/x7,levelEndMenuGroupHeight/y7),"Main Menu"))
						{
							if (GameManager.Manager.PlayerType == 1)
							{
								mainMenuThief = !mainMenuThief;
								if(PhotonNetwork.playerList.Length <=1)
								{
									NetworkManager.Manager.endLevelMenuResponseRPC(5,mainMenuThief);
								}
								else
								{
									NetworkManager.Manager.endLevelMenuResponse(5,mainMenuThief);
								}
							}

						}
				setGUIColor(mainMenuHacker);
		
					if(GUI.Button(new Rect(x6 + levelEndMenuGroupWidth/2,y6 + (levelEndMenuGroupHeight/levelEndMenuYOffsetFactor * 4),levelEndMenuGroupWidth/x7,levelEndMenuGroupHeight/y7),"Main Menu"))
						{
							if (GameManager.Manager.PlayerType == 2)
							{
								mainMenuHacker = !mainMenuHacker;
								if(PhotonNetwork.playerList.Length <=1)
								{
									NetworkManager.Manager.endLevelMenuResponseRPC(6,mainMenuHacker);
								}
								else
								{
									NetworkManager.Manager.endLevelMenuResponse(6,mainMenuHacker);
								}
								
							}

						}
				
				if ( (nxtLevelThief && nxtLevelHacker ) || (replayThief && replayHacker) || (GameManager.Manager.PlayerType == 1 && mainMenuThief) || (GameManager.Manager.PlayerType == 2 && mainMenuHacker) )
				{
					goActivated = true;	
				}
				else
				{
					goActivated = false;
				}
				
				setGUIColor(goActivated);
		
					if(GUI.Button(new Rect(x6 + levelEndMenuGroupWidth/2 - ((levelEndMenuGroupWidth/x7)/2),y6 + (levelEndMenuGroupHeight/levelEndMenuYOffsetFactor * 5),levelEndMenuGroupWidth/x7,levelEndMenuGroupHeight/y7),"GO !!"))
						{
							if ( nxtLevelThief && nxtLevelHacker )
							{
								showGUI = false;
								Time.timeScale = 1;
								Screen.lockCursor = false;
				
						        if (Application.loadedLevel == (numberOfScenes - 1))
								{		
										if(PhotonNetwork.isMasterClient)
										{
											MasterServer.UnregisterHost();
										}
										_playerUtil.GetComponent<AccountSystem>().ResetNamesAfterForceShutDown();
										//PhotonNetwork.Disconnect();
										Application.LoadLevel(0);
								}
								else
								{
										NetworkManager.Manager.LoadLevel(Application.loadedLevel + 1);
								}
								GameObject.Find("Playertheif(Clone)").GetComponent<MouseLookAround>().enabled = true;
							}
							else if ( replayThief && replayHacker )
							{
									showGUI = false;
									Time.timeScale = 1;
									Screen.lockCursor = false;
									NetworkManager.Manager.RestartLevel();
									GameObject.Find("Playertheif(Clone)").GetComponent<MouseLookAround>().enabled = true;	
								
							}
							else if ( ( GameManager.Manager.PlayerType == 1 && mainMenuThief ) || (GameManager.Manager.PlayerType == 2 && mainMenuHacker) )
							{
									showGUI = false;
									Time.timeScale = 1;
									Screen.lockCursor = false;
									if(PhotonNetwork.isMasterClient)
									{
										MasterServer.UnregisterHost();
									}
									_playerUtil.GetComponent<AccountSystem>().ResetNamesAfterForceShutDown();
									//PhotonNetwork.Disconnect();									
									Application.LoadLevel(0);
									GameObject.Find("Playertheif(Clone)").GetComponent<MouseLookAround>().enabled = true;
							}
					
						}  // if(GUI.Button(new Rect(Screen.width - (x6 - x6/3 ) ,y6 + 100,x7/2,40),"GO !!"))
				GUI.EndGroup();
			} // if( showGUI )
			*/
	}
	
	void OnGUI()
	{
		ShowEndScreen();
	}
	
	
	// Update is called once per frame
	void Update () {
		_constantTicker += Time.deltaTime;
	}

	void setGUIColor( bool value)
	{
		if (value == true)
		{
			GUI.backgroundColor = Color.green;
		}
		else
		{
			GUI.backgroundColor = Color.red;
		}	
	}
}


