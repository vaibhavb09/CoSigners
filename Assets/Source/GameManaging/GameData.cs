using UnityEngine;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;

[XmlRootAttribute("Level")]
//Original
/*
public class GraphData
{
	[XmlArrayItem("Index", IsNullable = true)]
	public int[] SourceNode;
	
	[XmlArrayItem("Node")]
	public NodeData[] Nodes;
		
	[XmlArrayItem("BonusCreator")]
	public BonusCreatorData[] BonusCreators;	
	
}

public class NodeData
{
	[XmlElementAttribute("Index")]
	public int Index;
	[XmlElementAttribute("Type")]
	public string Type;
}

public class BonusCreatorData
{
	[XmlElementAttribute("BCID")]
	public int BCID;
	[XmlElementAttribute("BCType")]
	public string BCType;
	[XmlArrayItem("Bonus")]
	public BonusData[] Bonuses;
}

public class BonusData
{
	[XmlElementAttribute("HexIndex")]
	public int HexIndex;
	[XmlElementAttribute("HexType")]
	public bool Type;
}*/



public class GraphData
{
	[XmlArrayItem("SourceNode", IsNullable = true)]
	public SourceNodeData[] SourceNodes;
	
	[XmlArrayItem("DoorNode")]
	public DoorNodeData[] DoorNodes;
	/*	
	[XmlArrayItem("CameraNode")]
	public CameraNodeData[] CameraNodes;
	*/
	[XmlArrayItem("IRNode")]
	public IRNodeData[] IRNodes;
	
	[XmlArrayItem("SecurityNode")]
	public SecurityNodeData[] SecurityNodes;	
	
	[XmlArrayItem("Transmitter")]
	public TransmitterData[] Transmitters;
	
	[XmlArrayItem("Jammer")]
	public JammerData[] Jammers;
	
	[XmlArrayItem("Override")]
	public OverrideData[] Overrides;
	
	[XmlArrayItem("SearchPoint")]
	public SearchPointData[] SearchPoints;
	
	[XmlArrayItem("DeadZone")]
	public DeadZoneData[] DeadZones;
	
	[XmlArrayItem("Laser")]
	public LaserData[] Lasers;
	
	[XmlArrayItem("TracerCreator")]
	public TracerCreatorData[] TracerCreators;
	
	[XmlArrayItem("Password")]
	public BonusPasswordData[] BonusPasswords;

	[XmlArrayItem("Info")]
	public InfoData[] Infos;
	
	//[XmlArrayItem("BonusCreator")]
	//public BonusCreatorData[] BonusCreators;
	
	[XmlArrayItem("GuardPath")]
	public GuardPathData[] GuardPaths;
	/*
	[XmlArrayItem("Laser")]
	public LaserData[] Lasers;
	*/
	[XmlArrayItem("Global")]
	public GlobalData[] Globals;
}

public class SourceNodeData
{
	[XmlElementAttribute("SourceIndex")]
	public int Index;
}

public class DoorNodeData
{
	[XmlElementAttribute("DoorIndex")]
	public int Index;
	[XmlElementAttribute("Locked")]
	public bool Locked;
	[XmlElementAttribute("Closed")]
	public bool Closed;
	[XmlElementAttribute("DoorSecurity")]
	public int SecurityLevel;
	[XmlElementAttribute("DoorType")]
	public string DoorType;
}

/*
public class CameraNodeData
{
	[XmlElementAttribute("CameraIndex")]
	public int Index;
	[XmlElementAttribute("Rotate")]
	public bool Rotate;
	[XmlElementAttribute("VarianceAngle")]
	public float Angle;
	[XmlElementAttribute("CameraSpeed")]
	public float Speed;
}
*/

public class IRNodeData
{
	[XmlElementAttribute("IRIndex")]
	public int Index;
	[XmlElementAttribute("Radius")]
	public float Radius;
	[XmlElementAttribute("IRSecurity")]
	public int SecurityLevel;
}

public class EnergyNodeData
{
	[XmlElementAttribute("EnergyNode")]
	public int Index;
	[XmlElementAttribute("EnergySecurity")]
	public int SecurityLevel;
}

public class SecurityNodeData
{
	[XmlElementAttribute("SecurityIndex")]
	public int Index;
	[XmlElementAttribute("SecurityLevel")]
	public int Level;
	[XmlElementAttribute("SecuritySecurity")]
	public int SecurityLevel;
}

public class TransmitterData
{
	[XmlElementAttribute("TransmitterHexIndex")]
	public int HexIndex;
	[XmlElementAttribute("TransmitterRange")]
	public int Range;
	[XmlElementAttribute("TransmitterVis")]
	public string Visible;
}

public class JammerData
{
	[XmlElementAttribute("JammerHexIndex")]
	public int HexIndex;
	[XmlElementAttribute("JammerRange")]
	public int Range;
	[XmlElementAttribute("JammerFacing")]
	public string Facing;
}

public class OverrideData
{
	[XmlElementAttribute("OverrideIndex")]
	public int HexIndex;
}

public class SearchPointData
{
	
	[XmlElementAttribute("SearchPointX")]
	public float xPos;
	[XmlElementAttribute("SearchPointZ")]
	public float zPos;
	[XmlElementAttribute("StartAngle")]
	public int startAngle;
	[XmlElementAttribute("SweepAngle")]
	public int sweepAngle;
	
}

public class DeadZoneData
{	
	[XmlElementAttribute("dIndex")]
	public int deadIndex;
}

public class LaserData
{
	[XmlElementAttribute("LaserGroup")]
	public int groupID;
	[XmlElementAttribute("LaserAX")]
	public float pointAX;
	[XmlElementAttribute("LaserAY")]
	public float pointAY;
	[XmlElementAttribute("LaserAZ")]
	public float pointAZ;
	[XmlElementAttribute("LaserBX")]
	public float pointBX;
	[XmlElementAttribute("LaserBY")]
	public float pointBY;
	[XmlElementAttribute("LaserBZ")]
	public float pointBZ;
	
}

public class TracerCreatorData
{
	[XmlElementAttribute("TC_Type")]
	public string Type;
	[XmlElementAttribute("TC_Frequency")]
	public float Frequency;
	[XmlArrayItem("Tracer")]
	public TracerData[] Tracers;
}

public class TracerData
{
	[XmlElementAttribute("TracerHexIndex")]
	public int HexIndex;
	[XmlElementAttribute("TracerDelay")]
	public float Delay;
	[XmlElementAttribute("Calibration")]
	public float Calibration;
	[XmlElementAttribute("Active")]
	public int Active;
}

public class BonusPasswordData
{
	[XmlElementAttribute("PasswordIndex")]
	public int HexIndex;
	[XmlElementAttribute("PasswordFacing")]
	public int Facing;
}

public class InfoData
{
	[XmlElementAttribute("InfoHexIndex")]
	public int InfoHexIndex;
	[XmlElementAttribute("InfoID")]
	public int InfoID;
	[XmlElementAttribute("InfoType")]
	public string InfoType;
}

/*
public class BonusCreatorData
{
	[XmlElementAttribute("BC_Type")]
	public string Type;
	[XmlElementAttribute("BC_PlaceType")]
	public string PlaceType;
	[XmlElementAttribute("BC_Frequency")]
	public float Frequency;
	[XmlElementAttribute("BonusGroupID")]
	public int GroupID;
	[XmlArrayItem("Bonus")]
	public BonusData[] Bonuses;
}

public class BonusData
{
	[XmlElementAttribute("BonusHexIndex")]
	public int HexIndex;
	[XmlElementAttribute("Weight")]
	public int Weight;
	[XmlElementAttribute("BonusDelay")]
	public float Delay;
	[XmlElementAttribute("Lifespan")]
	public float Lifespan;
	[XmlElementAttribute("BonusParam1")]
	public string BonusParam1;
	[XmlElementAttribute("BonusParam2")]
	public string BonusParam2;
}*/

public class GuardPathData
{
	[XmlElementAttribute("GuardType")]
	public string Type;
	[XmlElementAttribute("GuardSpeed")]
	public float Speed;
	[XmlElementAttribute("Duplicate")]
	public bool Duplicate;
	[XmlElementAttribute("GuardFrequency")]
	public float Frequency;
	[XmlArrayItem("WayPoint")]
	public WayPointData[] WayPoints;
}

public class WayPointData
{
	[XmlElementAttribute("XPos")]
	public float XPos;
	[XmlElementAttribute("ZPos")]
	public float ZPos;
	[XmlElementAttribute("OrderID")]
	public int OrderID;
	[XmlElementAttribute("Pause1")]
	public float Pause1;
	[XmlElementAttribute("Look")]
	public string Look;
	[XmlElementAttribute("Pause2")]
	public float Pause2;
	[XmlElementAttribute("Turn")]
	public string Turn;
}
/*
public class LaserData
{
	[XmlElementAttribute("LaserGroupID")]
	public int LaserGroup;
	[XmlElementAttribute("StartXPos")]
	public float StartXPos;
	[XmlElementAttribute("StartYPos")]
	public float StartYPos;
	[XmlElementAttribute("StartZPos")]
	public float StartZPos;
	[XmlElementAttribute("EndXPos")]
	public float EndXPos;
	[XmlElementAttribute("EndYPos")]
	public float EndYPos;
	[XmlElementAttribute("EndZPos")]
	public float EndZPos;
	[XmlElementAttribute("Infinite")]
	public bool Infinite;
}
*/
public class GlobalData
{
	[XmlElementAttribute("LevelName")]
	public string LevelName;
	[XmlElementAttribute("LevelWidth")]
	public int LevelWidth;
	[XmlElementAttribute("LevelHeight")]
	public int LevelHeight;
	[XmlElementAttribute("TimeMaxScore")]
	public string TimeMaxScore;
	[XmlElementAttribute("TimeMinScore")]
	public string TimeMinScore;
	[XmlElementAttribute("TimeDead")]
	public string TimeDead;
	[XmlElementAttribute("TransMaxScore")]
	public string TransMaxScore;
	[XmlElementAttribute("TransMinScore")]
	public string TransMinScore;
	[XmlElementAttribute("TransInventory")]
	public string TransInventory;
	[XmlElementAttribute("TransRefresh")]
	public string TransRefresh;
	[XmlElementAttribute("TransCooldown")]
	public string TransCooldown;
	[XmlElementAttribute("EMPMaxScore")]
	public string EMPMaxScore;
	[XmlElementAttribute("EMPMinScore")]
	public string EMPMinScore;
	[XmlElementAttribute("EMPInventory")]
	public string EMPInventory;
	[XmlElementAttribute("EMPRefresh")]
	public int PowerCapacity;
	[XmlElementAttribute("EMPCooldown")]
	public string EMPCooldown;
	[XmlElementAttribute("PivotsMaxScore")]
	public string PivotsMaxScore;
	[XmlElementAttribute("PivotsMinScore")]
	public string PivotsMinScore;
	[XmlElementAttribute("PivotsInventory")]
	public string PivotsInventory;
	[XmlElementAttribute("LockdownTime")]
	public float LockdownTime;
	[XmlElementAttribute("OverrideMinDist")]
	public float OverrideMinDist;
	[XmlElementAttribute("OverrideMaxDist")]
	public float OverrideMaxDist;
	
}