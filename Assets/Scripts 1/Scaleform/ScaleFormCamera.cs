
/**********************************************************************

Filename    :   MyCamera.cs
Content     :   Inherits from SFCamera
Created     :   
Authors     :   Ankur Mohan

Copyright   :   Copyright 2012 Autodesk, Inc. All Rights reserved.

Use of this software is subject to the terms of the Autodesk license
agreement provided at the time of installation or download, or which
otherwise accompanies this software in either electronic or hard copy form.
 
***********************************************************************/
using UnityEngine;
using System.Runtime.InteropServices;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Scaleform;

/* The user should override SFCamera and add methods for creating movies whenever specific events take place in the game.
*/
public class ScaleFormCamera : SFCamera {
	
	public Dictionary<string,SwfLayout> m_swfDic = new Dictionary<string, SwfLayout>();
	private string m_currentSWFName = null;
	private string m_nextSWFName = null;
	private bool   m_InitCalled = false;

	void Awake ()
	{
		DontDestroyOnLoad (this.gameObject);
	}

	// Hides the Start function in the base SFCamera. Will be called every time the ScaleformCamera (Main Camera game object)
	// is created. Use new and not override, since return type is different from that of base::Start()
	new public  IEnumerator Start()
	{
		// The eval key must be set before any Scaleform related classes are loaded, other Scaleform Initialization will not 
		// take place.
		#if (UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_EDITOR) && !UNITY_WP8
		SF_SetKey("4WVKGE7E3BZ6VQVQ5WTSR9XWX72U3Y6ZMR6TW8XP9NNW37C0XXJAT5WE2PZWGVL");
		#elif UNITY_IPHONE
		SF_SetKey("06WVTRTALWDYTQSJ95Q7Y2ESSI5M5LUXIHF0Y15BJ612PIDIDUJ2OOL6XFJQQ3F");
		#elif UNITY_ANDROID
		SF_SetKey("SASU4AKY1M46IAXWE7I0JJTQG6PW6CARAQA8UNYM0UO6WAS2A2SKCB3H8J57J5M");
		#elif UNITY_WP8
		sf_setKey("");
		#endif
		//For GL based platforms - Sets a number to use for Unity specific texture management.  Adjust this number if
		//you start to experience black and/or mssing textures.
		#if UNITY_WP8
		sf_setTextureCount(500);
		#else
		SF_SetTextureCount(500);
		#endif

		return base.Start();
	}
	

	public void Init()
	{
		//Play ("VideoFrame.swf");
		m_InitCalled = true;

		if( Application.loadedLevel == 2 || Application.loadedLevel == 3 || Application.loadedLevel == 4 )
		{
			InfoNodeManager.Manager.LoadSwfInformation();
			NetworkManager.Manager.SetCurrentInfoDataParams(InfoNodeManager.Manager._MovieList["LevelIntroVideo_" +(Application.loadedLevel - 1).ToString()], GameManager.Manager.PlayerType);
		}

		if( Application.loadedLevelName == "LevelTransition")
		{
			Play ("LevelLoadMenu5_New.swf");
		}
	}

	public void PlayLevelIntroMovie()
	{
		InfoNodeManager.Manager._currentInfoNodeID = 0;

		NetworkManager.Manager.PlayMovieRPC(InfoNodeManager.Manager._MovieList["LevelIntroVideo_" + (Application.loadedLevel - 1).ToString()], GameManager.Manager.PlayerType);
		NetworkManager.Manager.PlayInfoNodeAudioRPC("LevelIntroAudio_"+(Application.loadedLevel-1).ToString());
		
		GameManager.Manager._currentInfoData.m_hackerReady = false;
		GameManager.Manager._currentInfoData.m_thiefReady = false;
		
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
	
	// Application specific code goes here
	new public void Update()
	{
		//CreateGameHud();
		//Play ("VideoFrame.swf");
		//if( !m_InitCalled )
		//	Init();

		if( InfoNodeManager.Manager._currentInfoNodeID == -1 && ( Application.loadedLevel == 2 || Application.loadedLevel == 3 || Application.loadedLevel == 4 ) ) //Intro Video
		{
			if( GameManager.Manager._currentInfoData.m_hackerReady && GameManager.Manager._currentInfoData.m_thiefReady )
			{
				//Video
				PlayLevelIntroMovie();
			}
		}

		base.Update ();
	}


	public void Play(string i_swfName)
	{
		if(!m_swfDic.ContainsKey(i_swfName))
		{
			//Debug.Log("#Max:Trying to add " +i_swfName+ " to list");
			//Debug.Log("#Max TRYING TO PLAY SCALEFORM STUFF!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!" + m_nextSWFName);
			SFMovieCreationParams creationParams = CreateMovieCreationParams(i_swfName);
			//     creationParams.TheScaleModeType  = ScaleModeType.SM_ShowAll;
			creationParams.IsInitFirstFrame = false;
			SwfLayout demo1 = new SwfLayout(this, SFMgr, creationParams);
			demo1.SwapDepths(SFMgr.GetTopMovie()[0]);
			m_swfDic.Add(i_swfName, demo1);
			//Debug.Log("#Max:Added " +i_swfName+ " to list");
		}
	}

	public void SetUpLevelInfo(string i_levelName, string i_transmitterCount, string i_estimatedTime, string i_difficulty, string i_desc, Texture2D i_texture)
	{
		m_swfDic["LevelLoadMenu5_New.swf"].SetUpLevelInfo(i_levelName, i_transmitterCount, i_estimatedTime, i_difficulty, i_desc, i_texture);
	}

	public void PlayAnimation(string i_swfName, string i_call)
	{
		//Debug.Log("#Max:Playing " + i_call +" in " +i_swfName );
		//Debug.Log("#Max:Movie Exists? : " + m_swfDic[i_swfName] );
		if( m_swfDic.ContainsKey(i_swfName) )
		{
			m_swfDic[i_swfName].InvokeAnimation(i_call);
		}
	}
	
	public void Stop(string i_swfName)
	{
		if( m_swfDic.ContainsKey(i_swfName) )
		{
			//Debug.Log("#Max:Removing " + i_swfName);
			m_swfDic[i_swfName].Destroy();
			m_swfDic.Remove(i_swfName);
		}
		else
		{
			//Debug.Log("#Max:Movie does not exists " + i_swfName);
		}
	}

	public void StopAllMovies()
	{
		foreach(var movie in m_swfDic)
		{
			movie.Value.Destroy();
			//Debug.Log("Clearing:.." + movie.Key);
		}

		m_swfDic.Clear();
	}
//	private void CreateGameHud()
//	{
//		if (demo1 == null)
//		{
//			SFMovieCreationParams creationParams = CreateMovieCreationParams("MainMenu.swf");
//			//     creationParams.TheScaleModeType  = ScaleModeType.SM_ShowAll;
//			creationParams.IsInitFirstFrame = false;
//			demo1 = new InfoNodeLayout(this, SFMgr, creationParams);
//		}
//	}
}