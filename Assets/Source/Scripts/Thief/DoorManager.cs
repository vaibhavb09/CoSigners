using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DoorManager {
	
	
	private static 			DoorManager _instance; 
	public 					GameObject[] doors;
	
	// Use this for initialization
	void Start (){}
	
	public static DoorManager Manager
	{
		get
		{
			if(_instance == null)
			{
				_instance = new DoorManager();			
			}
			return _instance;
		}
	}

	public DoorManager () 
    { 
        _instance = this;
    }

//#if UNITY_IPHONE
//	
//	public void LoadDoors(GraphData i_gData){}
//	public void UnlockAllDoors() {}
//	void Update (){}
//	
//#else
	
	public void LoadDoors(GraphData i_gData)
	{
		doors = GameObject.FindGameObjectsWithTag("Door");
		DoorNodeData[] doorNodeData = i_gData.DoorNodes;
		BasicScoreSystem.Manager.TotalDoors = doorNodeData.Length;
		foreach(DoorNodeData doorNode in doorNodeData)
		{
			foreach(GameObject door in doors)
			{	
				if( door.GetComponent<IDoorController>().GetDoorType() ==  DoorType.NormalDoor ) //Normal doors
				{
					if( !door.GetComponent<DoorController>().isOffline ) 
					{
						int currentIndex = HexGrid.Manager.GetIndex( door.transform.position );
						//Debug.Log ("DoorNode:" + doorNode.Index + " Actual Door:" + currentIndex);
						if( doorNode.Index == currentIndex )
							door.GetComponent<DoorController>().Load( doorNode.Index, doorNode.Locked, !doorNode.Closed );
					}
					else
					{
						int offlineIndex = -1;
						door.GetComponent<DoorController>().Load( offlineIndex, true, false );
					}
				}
				else if( door.GetComponent<IDoorController>().GetDoorType() ==  DoorType.EndDoor )//End door
				{
					int currentIndex = HexGrid.Manager.GetIndex( door.transform.position );
					if( doorNode.Index == currentIndex )
						door.GetComponent<EndDoorController>().Load( doorNode.Index, doorNode.Locked, !doorNode.Closed );
				}
				else if( door.GetComponent<IDoorController>().GetDoorType() ==  DoorType.StartDoor )//End door
				{
					int currentIndex = HexGrid.Manager.GetIndex( door.transform.position );
					if( doorNode.Index == currentIndex )
						door.GetComponent<EndDoorController>().Load( doorNode.Index, !doorNode.Locked, !doorNode.Closed );
				}
			}
		}
	}
	
	//DEBUG
	public void UnlockAllDoors() 
	{
		doors = GameObject.FindGameObjectsWithTag("Door");
		foreach(GameObject door in doors)
		{	
			NetworkManager.Manager.UnlockDoor( door.GetComponent<IDoorController>().DoorNumber);	
		}
	}
		
//#endif
	
}

