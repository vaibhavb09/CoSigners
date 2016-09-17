using UnityEngine;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

public class LevelLoader {
	
	
	#region Members
	
	//TextAsset levelXMLfile;
	//public int levelID;
	//public string levelName;
	
	#endregion 
	// Use this for initialization
	//void Start () {
	
	//put it here for now for testing.
	//LoadLevel(1);
	//LoadLevel (Application.loadedLevelName);
	
	//Analytics
	//if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
	//{
	//	try
	//	{
	//		//NetworkManager.Manager.SendGameInfoAsEmail("cyberheistgame@gmail.com", " OUR GAME IS BEING PLAYED RIGHT NOW ");
	//	}
	//	catch
	//	{
	//	}
	//}
	
	//if(GameManager.Manager.PlayerType == 1 && GameManager.ID == 0)
	//{
	//	NetworkManager.Manager.RestartLevel();
	//	GameManager.ID++;
	//}
	
	//}
	
	static public GraphData LoadLevel(string i_level)
	{		
		
		//Debug.Log("#Max:Level Name is "+i_level);
		
		//levelName =  "Levels/Level_X";
		/*
		if(! Directory.Exists(Application.dataPath + "UserProfile"))
		{
			Directory.CreateDirectory(Application.dataPath + "UserProfile");
			Debug.Log("No gameplay data found");
			return;
		}*/
		
		// Set up the file name
		//string filename = Application.dataPath + "UserProfile/Level" + i_level.ToString() + ".xml";
		//string filename = Application.dataPath + "UserProfile/Level" + "_X" + ".xml";
		
		// Creates an instance of the XmlSerializer class;
		// specifies the type of object to be deserialized.
		XmlSerializer serializer = new XmlSerializer(typeof(GraphData));
		
		// If the XML document has been altered with unknown 
		// nodes or attributes, handles them with the 
		// UnknownNode and UnknownAttribute events.
		serializer.UnknownNode+= new 
			XmlNodeEventHandler(serializer_UnknownNode);
		serializer.UnknownAttribute+= new 
			XmlAttributeEventHandler(serializer_UnknownAttribute);
		
		//TextAsset txt = (TextAsset) Resources.Load("Levels/Level_X");
		//Debug.LogError("loaded level index" + i_level +" "+ GameManager.Manager.LevelNames[(int)i_level]);
		//TextAsset txt = (TextAsset) Resources.Load("Levels/" + i_level);	
		TextAsset txt = (TextAsset) Resources.Load("Levels/" + i_level + "/" + i_level + "_CONFIG");	
		string str = (string) txt.text;
		//Debug.Log("LEVEL LOADER DATA:\n" + str);
		TextReader tr = new StringReader(str);
		// A FileStream is needed to read the XML document.
		//FileStream fs = new FileStream( Application.dataPath + "/Resources/Levels/Level_X", FileMode.Open);
		//FileStream fs = new FileStream(filename, FileMode.Open);
		// Declares an object variable of the type to be deserialized.
		GraphData gData;
		// Uses the Deserialize method to restore the object's state 
		// with data from the XML document. */
		gData = (GraphData) serializer.Deserialize(tr);
		//WriteDebugData(gData);
		return gData;
	}
	
	// error callbacks
	static protected void serializer_UnknownNode(object sender, XmlNodeEventArgs e)
	{
		//Debug.Log("Unknown Node:" +   e.Name + "\t" + e.Text);
	}
	
	static protected void serializer_UnknownAttribute(object sender, XmlAttributeEventArgs e)
	{
		System.Xml.XmlAttribute attr = e.Attr;
		//Debug.Log("Unknown attribute " + attr.Name + "='" + attr.Value + "'");
	}
	
	static private void WriteDebugData(GraphData gData)
	{
		//                    
		// DEBUGGING STUFF
		//
		Debug.Log("******  WRITING GRAPH DATA  *******");
		Debug.Log ("-------SOURCE NODES-------");
		SourceNodeData[] sourceNodeData = gData.SourceNodes;
		foreach(SourceNodeData sourceNode in sourceNodeData)
			Debug.Log("Source Node -- Index: " + sourceNode.Index);
		Debug.Log ("\n");
		
		Debug.Log ("-------DOOR NODES-------");
		DoorNodeData[] doorNodeData = gData.DoorNodes;
		foreach(DoorNodeData doorNode in doorNodeData)
			Debug.Log("Door Node -- Index:" + doorNode.Index + " Locked:" + doorNode.Locked + " Closed:" + doorNode.Closed );
		Debug.Log ("\n");
		
		/*
		Debug.Log ("-------CAMERA NODES-------");
		CameraNodeData[] cameraNodeData = gData.CameraNodes;
		foreach(CameraNodeData cameraNode in cameraNodeData)
			Debug.Log("Camera Node -- Index:" + cameraNode.Index + " Rotate:" + cameraNode.Rotate + " Angle:" + cameraNode.Angle + " Speed:" + cameraNode.Speed);
		Debug.Log ("\n");
		*/
		
		Debug.Log ("-------IR NODES-------");
		IRNodeData[] IRNodeData = gData.IRNodes;
		foreach(IRNodeData IRNode in IRNodeData)
			Debug.Log("IR Node -- Index:" + IRNode.Index + " Radius:" + IRNode.Radius );
		Debug.Log ("\n");
		
		Debug.Log ("-------SECURITY NODES-------");
		SecurityNodeData[] securityNodeData = gData.SecurityNodes;
		foreach(SecurityNodeData securityNode in securityNodeData)
			Debug.Log("Security Node -- Index:" + securityNode.Index + " SecurityLevel:" +securityNode.Level );
		Debug.Log ("\n");
		
		Debug.Log ("-------TRANSMITTERS-------");
		TransmitterData[] transmitterData = gData.Transmitters;
		foreach(TransmitterData transmitters in transmitterData)
			Debug.Log("Transmitter -- Index:" + transmitters.HexIndex + " Range:" + transmitters.Range + " Visible:" + transmitters.Visible );
		Debug.Log ("\n");
		
		Debug.Log ("-------JAMMERS-------");
		JammerData[] jammerData = gData.Jammers;
		foreach(JammerData jammers in jammerData)
			Debug.Log("Jammer -- Index:" + jammers.HexIndex + " Range:" + jammers.Range + " Facing:" + jammers.Facing );
		Debug.Log ("\n");
		
		Debug.Log ("-------OVERRIDES-------");
		OverrideData[] overrideData = gData.Overrides;
		foreach(OverrideData overrides in overrideData)
			Debug.Log("Override -- Index:" + overrides.HexIndex );
		Debug.Log ("\n");
		
		Debug.Log ("-------INFO TERMINAL-------");
		InfoData[] infoData = gData.Infos;
		foreach(InfoData info in infoData)
			Debug.Log("Info Terminals -- Index:" + info.InfoHexIndex + "  ID;" + info.InfoID + "  type:" + info.InfoType);
		Debug.Log ("\n");

		Debug.Log ("-------SEARCH POINTS-------");
		SearchPointData[] searchPointData = gData.SearchPoints;
		foreach(SearchPointData searchPoints in searchPointData)
			Debug.Log("SearchPoint --xPos:" + searchPoints.xPos + " zPos:" + searchPoints.zPos + " startAngle:" + searchPoints.startAngle + " sweepAngle:" + searchPoints.sweepAngle );
		Debug.Log ("\n");
		
		Debug.Log ("-------DEAD ZONES-------");
		DeadZoneData[] deadZoneData = gData.DeadZones;
		foreach(DeadZoneData deadZones in deadZoneData)
			Debug.Log("Dead Index --:" + deadZones.deadIndex);
		Debug.Log ("\n");
		
		Debug.Log ("-------LASERS-------");
		LaserData[] laserData = gData.Lasers;
		foreach(LaserData lasers in laserData)
			Debug.Log("SearchPoint --Group:" + lasers.groupID + " AX:" + lasers.pointAX + " AY:" + lasers.pointAY  + " AZ:" + lasers.pointAZ
			          + " BX:" + lasers.pointBX + " BY:" + lasers.pointBY + " BZ:" + lasers.pointBZ);
		Debug.Log ("\n");
		
		Debug.Log ("-------TRACERS-------");
		TracerCreatorData[] tracerCreatorData = gData.TracerCreators;
		foreach(TracerCreatorData tracerCreators in tracerCreatorData)
		{
			Debug.Log("TracerCreator -- Type:" + tracerCreators.Type + " Frequency:" + tracerCreators.Frequency );
			TracerData[] tracers = tracerCreators.Tracers;
			foreach(TracerData tracer in tracers)
			{
				Debug.Log("      Tracer -- HexIndex: " + tracer.HexIndex + " Delay: " + tracer.Delay + " Calibration: " + tracer.Calibration + " Active: " + tracer.Active);
			}
		}
		Debug.Log ("\n");
		
		Debug.Log ("-------PASSWORD BONUSES-------");
		BonusPasswordData[] bonusPasswordData = gData.BonusPasswords;
		foreach(BonusPasswordData bonusPassword in bonusPasswordData)
		{
			Debug.Log("      BonusPassword -- HexIndex: " + bonusPassword.HexIndex + " Facing: " + bonusPassword.Facing );
		}
		Debug.Log ("\n");
		
		Debug.Log ("-------GUARD PATHS-------");
		GuardPathData[] guardPathData = gData.GuardPaths;
		foreach(GuardPathData guardPaths in guardPathData)
		{
			Debug.Log("Guard Path -- Type:" + guardPaths.Type + " Speed:" + guardPaths.Speed + " Duplicate:" + guardPaths.Duplicate  + " Frequency:" + guardPaths.Frequency );
			WayPointData[] wayPoints = guardPaths.WayPoints;
			foreach(WayPointData wayPoint in wayPoints)
			{
				Debug.Log("      WayPoint -- XPos: " + wayPoint.XPos + " ZPos: " + wayPoint.ZPos + " Order: " + wayPoint.OrderID + " Pause1: " + wayPoint.Pause1
				          + " Look: " + wayPoint.Look + " Pause2: " + wayPoint.Pause2 + " Turn: " + wayPoint.Turn);
			}
		}
		Debug.Log ("\n");
		
		Debug.Log ("-------GLOBALS-------");
		{
			GlobalData[] globalData = gData.Globals;
			Debug.Log("      LevelName = " + globalData[0].LevelName);
			Debug.Log("      LevelWidth = " + globalData[0].LevelWidth);
			Debug.Log("      LevelHeight = " + globalData[0].LevelHeight);
			Debug.Log("      TimeMaxScore = " + globalData[0].TimeMaxScore);
			Debug.Log("      TimeMinScore = " + globalData[0].TimeMinScore);
			Debug.Log("      TimeDead = " + globalData[0].TimeDead);
			Debug.Log("      TransMaxScore = " + globalData[0].TransMaxScore);
			Debug.Log("      TransMinScore = " + globalData[0].TransMinScore);
			Debug.Log("      TransInventory = " + globalData[0].TransInventory);
			Debug.Log("      TransRefresh = " + globalData[0].TransRefresh);
			Debug.Log("      TransCooldown = " + globalData[0].TransCooldown);
			Debug.Log("      EMPMaxScore = " + globalData[0].EMPMaxScore);
			Debug.Log("      EMPMinScore = " + globalData[0].EMPMinScore);
			Debug.Log("      EMPInventory = " + globalData[0].EMPInventory);
			Debug.Log("      PowerCapacity = " + globalData[0].PowerCapacity);
			Debug.Log("      EMPCooldown = " + globalData[0].EMPCooldown);
			Debug.Log("      PivotsMaxScore = " + globalData[0].PivotsMaxScore);
			Debug.Log("      PivotsMinScore = " + globalData[0].PivotsMinScore);
			Debug.Log("      PivotsInventory = " + globalData[0].PivotsInventory);
			Debug.Log("      LockdownTime = " + globalData[0].LockdownTime);
			Debug.Log("      OverrideMinDist = " + globalData[0].OverrideMinDist);
			Debug.Log("      OverrideMaxDist = " + globalData[0].OverrideMaxDist);
		}
		
		Debug.Log ("\n");
		Debug.Log("******  FINISHED WRITING GRAPH DATA  *******");
	}
	
	
}
