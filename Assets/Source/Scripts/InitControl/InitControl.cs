using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Timers;

public class InitControl {

	private static System.Timers.Timer _timer;

	public static void PreGameInitialization()
	{
		LoadAccessPoint();
	}

	static void LoadAccessPoint()
	{
		TextAsset menuText = (TextAsset) Resources.Load("Levels/AccessPoint");
		
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
				AccessPoint point = new AccessPoint();
				point.ID 		= Convert.ToInt32(menuData[0]);
				point.path		= menuData[1];

				GameManager.Manager.MasterAccessPoints.Add(point);
			}
			
			reader.Close();
		}
	}

	public static void StartInitialization()
	{
		GameManager.Manager.LevelStarted = false;
		GraphData gData = LevelLoader.LoadLevel(Application.loadedLevelName);
		GameObject.Find("Initialize").GetComponent<PlayerInitialize>().InitPlayer();

		soundMan.soundMgr.Initialize();
		
		//////////////////////////////////////////////////
		//////////////// HACKER STUFF ////////////////////
		//////////////////////////////////////////////////

		ThiefGrid.Manager.LoadGhostTransmitter();
		HexGrid.Manager.LoadGraphFromGameData(gData);
		PivotManager.Manager.Load();
		SecurityManager.Manager.LoadTracers(gData);
		ConnectionManager.Manager.Load();
		GraphManager.Manager.LoadGraphFromGameData(gData);
		ThiefGrid.Manager.Load ();
		
		//Loading alert damage ( for when guards see you ) for thief side. Will be done through the XML file later. 
		switch( Application.loadedLevelName )
		{

			case "JM_53":		ThiefManager.Manager.AlertDamage = 0.0f;
								break;
			
			default:			ThiefManager.Manager.AlertDamage = 10.0f;
								break;
		}

		//////////////////////////////////////////////////
		//////////////// THIEF STUFF /////////////////////
		//////////////////////////////////////////////////

		GlobalData[] globalData = gData.Globals;
		//Load thief data eg. no of transmitters
		ThiefManager.Manager.Load( int.Parse(globalData[0].TransInventory) );
		
		//Load door state
		DoorManager.Manager.LoadDoors(gData);

		//GuardOverlord.Manager.CreatePaths(gData);
		GuardOverlord.Manager.OverLordInit(gData);

		SearchPoint.initializeSearchPointList(ref gData.SearchPoints);

		//////////////////////////////////////////////////
		//////////////// COMMON STUFF ////////////////////
		//////////////////////////////////////////////////
		BonusManager.Manager.LoadBonuses(gData);
		
		OverrideManager.Manager.LoadOverrideDataFromConfig(gData);

		//For Info Node
		InfoNodeManager.Manager.LoadInfoNodeDataFromConfig(gData);
		
		if ( GameManager.Manager.PlayerType == 1 )
		{
			ThiefGrid.Manager.UpdateFloorConnections();
		}
		else
		{
			GameObject TDC = GameObject.Find("TopDownCamera");
			TopDown_Camera TDCScript = TDC.GetComponent<TopDown_Camera>();
			TDCScript.ZoomOut();
		}

		if( Application.loadedLevelName.Equals("Level_01") || Application.loadedLevelName.Equals("Level_02") ) //Tutorial specific stuff
		{
			HackerThreat.Manager.active = false;
		}
		else //Rest of the levels
		{
			HackerThreat.Manager.active = true;
		}
		BasicScoreSystem.Manager.ResetData();


		//keep this at the end of the level
		NetworkManager.Manager.ImReady(GameManager.Manager.PlayerType, false);

		// Create a timer with a 1 second interval.
		//_timer = new System.Timers.Timer(1000);
		
		// Hook up the Elapsed event for the timer.
		//_timer.Elapsed += new ElapsedEventHandler(PostGameStartCheck);

		// Set the Interval to 2 seconds (2000 milliseconds).
		//_timer.Interval = 2000;
		//_timer.Enabled = true;
		//GC.KeepAlive(_timer);
	}

//
//	private static void PostGameStartCheck(object source, ElapsedEventArgs e)
//	{
//		//Debug.LogError("#Max The Elapsed event was raised at" +  e.SignalTime);
//
//		if(GameManager.Manager.LevelStarted)
//		{
//			_timer.Close();
//			return;
//		}
//		NetworkManager.Manager.CheckIfGameIsReady();
//	}
//	

	public static void PostGameStart()
	{
		GameManager.Manager.LevelStarted = true;

		//Calls in here should be synced

		//Debug.LogError("#MAx Being Called?");

		GameObject scaleformCamera = GameObject.FindGameObjectWithTag("ScaleformCamera");
		scaleformCamera.GetComponent<ScaleFormCamera>().Init();

		GameManager.Manager.HackerReady = false;
		GameManager.Manager.ThiefReady = false;
	}
}
