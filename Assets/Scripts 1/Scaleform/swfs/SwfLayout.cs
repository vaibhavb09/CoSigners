/**********************************************************************

Filename    :	UI_Scene_Demo1.cs
Content     :  
Created     :   
Authors     :   Ankur Mohan

Copyright   :   Copyright 2012 Autodesk, Inc. All Rights reserved.

Use of this software is subject to the terms of the Autodesk license
agreement provided at the time of installation or download, or which
otherwise accompanies this software in either electronic or hard copy form.
 
***********************************************************************/

using System;
using System.Collections;
using UnityEngine;
using Scaleform;
using Scaleform.GFx;

public class SwfLayout : Movie
{
	public Value	theMovie = null;
	private ScaleFormCamera parent = null;
	GameObject _playerUtil;

	bool levelClicked = false, exitClicked = false, playClicked = false;

	public SwfLayout(ScaleFormCamera parent, SFManager sfmgr, SFMovieCreationParams cp) :
		base(sfmgr, cp)
	{
		this.parent = parent;
		SFMgr = sfmgr;
		this.SetFocus(true);
	}
	
	
	public void OnRegisterSWFCallback(Value movieRef)
	{
		//Debug.Log("#Max:Register!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!" + movieRef.ToString());

		if( movieRef == null )
		{
			Debug.LogError("#Max:Movie is null");
		}

		theMovie = movieRef;
		/*
		int a = theMovie.Invoke("test2Ints", 1, 2, "param", true, theMovie);
		
		theMovie.Invoke("test2Strings", "a", "b");
		theMovie.Invoke("test2Booleans", true, false);
		theMovie.Invoke("test2Anything", theMovie, theMovie);
		theMovie.Invoke("testIntString", 1, "b");
		*/
	}

	public void SetUpLevelInfo(string i_levelName, string i_transmitterCount, string i_estimatedTime, string i_difficulty, string i_desc, Texture2D i_texture)
	{

		//m_levelName-level_name
		//m_TransmitterCount - txt_power_pods
		//m_EstTime - txt_time
		//m_difficulty - txt_difficulty
		//m_desc - txt_description
		//m_levelImage - m_image
		//Debug.Log("Set up level info: " + i_levelName + " " + i_transmitterCount + " " + i_estimatedTime + " " + i_difficulty+ " " + i_desc);
			 
		theMovie.Invoke("SetUpLevelInfo", i_levelName, i_transmitterCount, i_estimatedTime, i_difficulty, i_desc);
		//Debug.Log("change Texture");
		SFMgr.ReplaceTexture(theMovie.GetMovieId(), "texture1", i_texture);
	}

	public void InvokeAnimation(string i_call)
	{
		theMovie.Invoke(i_call);
	}

	public void OnSkipClickedUnity()
	{
		NetworkManager.Manager.PlayAnimation(GameManager.Manager._currentInfoData.m_currentMovie, "PlayCloseAnimation");

		NetworkManager.Manager.CloseInfoVideoAudio();

		if( Application.loadedLevel == 2 )
		{
			NetworkManager.Manager.ShowEndScreen(2);
		}
	}

	public void OnAnimationEndUnity()
	{
		NetworkManager.Manager.StopMovie(GameManager.Manager._currentInfoData.m_currentMovie);

		NetworkManager.Manager.SetCurrentInfoDataParams("", 0);

		/*
		if( InfoNodeManager.Manager._currentInfoNodeID == -1 )
		{
			NetworkManager.Manager.StopMovie(InfoNodeManager.Manager._MovieList["LevelIntroVideo"+Application.loadedLevel], GameManager.Manager.PlayerType);
		}
		else
			NetworkManager.Manager.StopMovie(InfoNodeManager.Manager._MovieList["InfoNodeVideo_"+InfoNodeManager.Manager._currentInfoNodeID], GameManager.Manager.PlayerType);
		*/
	}

	public void OnPlayClickedUnity()
	{
		NetworkManager.Manager.LastMenuAction(true, false, false);
		
		if(GameManager.Manager.m_roleReady == 0)
		{
			NetworkManager.Manager.PlayerReady(GameManager.Manager.PlayerType);
			NetworkManager.Manager.PlayAnimationRPC("LevelLoadMenu5_New.swf", "PlayReadyEnter");
		}
		/*
		else if(GameManager.Manager.m_roleReady == GameManager.Manager.PlayerType)
		{
			NetworkManager.Manager.PlayerReady(0);
			NetworkManager.Manager.PlayAnimation("LevelLoadMenu5_New.swf", "PlayReadyExit");
		}
		*/
		else if(GameManager.Manager.m_roleReady != GameManager.Manager.PlayerType)
		{
			NetworkManager.Manager.PlayAnimation("LevelLoadMenu5_New.swf", "PlayButtonExit");
			//StartNextLevelUnity();
		}
		
		//NetworkManager.Manager.PlayerReady(GameManager.Manager.PlayerType);
	}

	public void OnLevelClickedUnity()
	{
		//_playerUtil = GameObject.Find("PlayerUtil");
		//_playerUtil.GetComponent<AccountSystem>().ResetNamesAfterForceShutDown();
		NetworkManager.Manager.LastMenuAction(false, true, false);
		NetworkManager.Manager.PlayAnimation("LevelLoadMenu5_New.swf", "PlayButtonExit");
		//Application.LoadLevel(0);
		
	}

	public void OnExitClickedUnity()
	{
		NetworkManager.Manager.LastMenuAction(false, false, true);
		NetworkManager.Manager.PlayAnimation("LevelLoadMenu5_New.swf", "PlayButtonExit");
		//BasicScoreSystem.Manager.ResetData();
		//Application.LoadLevel(0);
	}

	public void OnExitAnimation()
	{
		NetworkManager.Manager.PlayerReady(0);

		if( GameManager.Manager.levelClicked )
		{
			NetworkManager.Manager.LastMenuActionRPC(false, false, false);
			NetworkManager.Manager.LoadLevelRPC(0);
		}
		else if( GameManager.Manager.exitClicked )
		{
			NetworkManager.Manager.LastMenuActionRPC(false, false, false);
			NetworkManager.Manager.GoBackToLoginScreenRPC();
		}
		else if( GameManager.Manager.playClicked )
		{
			NetworkManager.Manager.LastMenuActionRPC(false, false, false);
			StartNextLevelUnity();
		}
		NetworkManager.Manager.StopMovieRPC("LevelLoadMenu5_New.swf", 0);
	}

	//since function may called only on one side, so to sync, use Network manager.
	public void StartNextLevelUnity()
	{
		_playerUtil = GameObject.Find("PlayerUtil");
		NetworkManager.Manager.ChangeInStartMenuStatus(false);
		//GameManager.Manager.InStartMenu = false;
		//Time.timeScale = 1;
		if(Application.loadedLevelName != "LevelTransition")
		{
			Debug.Log("Start Next Level");
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
		
		if (Application.loadedLevelName == "JM_53")
		{		
			Debug.Log("Final Level Started");
			if(Network.isServer)
			{
				MasterServer.UnregisterHost();
			}
			_playerUtil.GetComponent<AccountSystem>().ResetNamesAfterForceShutDown();
			Network.Disconnect();
			//BasicScoreSystem.Manager.ResetData();
			Screen.lockCursor = false;
			Application.LoadLevel(0);
		}
		else
		{
			//BasicScoreSystem.Manager.ResetData();
			NetworkManager.Manager.LoadLevelUsingStringRPC(GameManager.Manager.NextLevelName);
		}
	}

	public void OnLevelContentShowsUp()
	{
		//LevelTransition.LoadLevelConfig();

	}

	public void OnMuteClicked()
	{
		AudioSource _infoNodeSource = GameObject.Find("InfoNodeSource").GetComponent<AudioSource>();
		
		GameManager.Manager.m_isInfoNodeSourceMuted = !GameManager.Manager.m_isInfoNodeSourceMuted;
		
		if( GameManager.Manager.m_isInfoNodeSourceMuted )
		{
			_infoNodeSource.mute = true;
		}
		else
		{
			_infoNodeSource.mute = false;
		}
	}	
		
	public void PlayInfoNodeAnimation()
	{
		if( GameManager.Manager._currentInfoData.m_currentMovie == "" )
		{
			if( InfoNodeManager.Manager._currentInfoNodeID == -1 && ( Application.loadedLevel == 2 || Application.loadedLevel == 3 || Application.loadedLevel == 4 ) ) //Intro Video
			{

				GameManager.Manager._currentInfoData.m_currentMovie = InfoNodeManager.Manager._MovieList["LevelIntroVideo_" + (Application.loadedLevel -1).ToString()];
				NetworkManager.Manager.SetCurrentInfoDataParams(InfoNodeManager.Manager._MovieList["LevelIntroVideo_" + (Application.loadedLevel - 1).ToString()], GameManager.Manager.PlayerType);
			}
			else
			{
				//Video
				NetworkManager.Manager.PlayMovie(InfoNodeManager.Manager._MovieList["InfoNodeVideo_" +InfoNodeManager.Manager._currentInfoNodeID], GameManager.Manager.PlayerType);
				//Audio
				AudioSource _infoNodeSource = GameObject.Find("InfoNodeSource").GetComponent<AudioSource>();
				soundMan.soundMgr.playOneShotOnSource(_infoNodeSource, "InfoNodeAudio_"+InfoNodeManager.Manager._currentInfoNodeID.ToString(),-1);
				NetworkManager.Manager.SetCurrentInfoDataParams(InfoNodeManager.Manager._MovieList["InfoNodeVideo_" +InfoNodeManager.Manager._currentInfoNodeID.ToString()], GameManager.Manager.PlayerType);
			}

			if(GameManager.Manager.PlayerType == 1) // only Thief can do this
			{
				NetworkManager.Manager.PauseGame(true);
			}
			
			if(GameManager.Manager.PlayerType == 1)
			{
				ThiefManager.Manager.DisableThiefActions();
			}
			else
			{
				HackerManager.Manager.DisableHackerActions();
			}
		}
		///NetworkManager.Manager.StopMovie("MenusTest_5.0_new.swf", 1);
	}

	public void SelfDestruct()
	{
		//NetworkManager.Manager.StopMovie("Video_01_B.swf", GameManager.Manager.PlayerType);
		NetworkManager.Manager.StopMovie("VideoFrame.swf", GameManager.Manager.PlayerType);
	}
	
}

