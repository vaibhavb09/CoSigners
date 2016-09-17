using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PointmanNetManager {
	
	#region singleton declearation
	private static PointmanNetManager m_instance;
	
	public PointmanNetManager () 
    {
        if (m_instance != null)
        {
            return;
        }
 
        m_instance = this;
    }
	
	public static PointmanNetManager Manager
	{
		get
		{
			if(m_instance == null)
			{
				m_instance = new PointmanNetManager();			
			}
			return m_instance;
		}
	}
	#endregion
	
#if UNITY_IPHONE
	
	public GameObject GetDoorObject( int i_doorNumber )
	{
		GameObject[] doors = GameObject.FindGameObjectsWithTag("Door");
		foreach(GameObject door in doors)
		{
			if(door.GetComponent<DoorController>().DoorNumber == i_doorNumber)
			{			
				return door;
			}			
		}
		return null;
	}
	
	public void OpenDoor(int i_doorNumber){}
	public void CloseDoor(int i_doorNumber){}
	public void LockDoor(int i_doorNumber){}
	public void UnlockDoor(int i_doorNumber){}
	public void SecureLockDoor(int i_doorNumber){}
	public void UnSecureLockDoor(int i_doorNumber){}
	public void  GetPingPosition(Vector3 i_pos){}
	public void PowerNode(int PowerNodeID, bool status){}
	public void SetLinkState( int i_index, ThiefGrid.LinkState i_state ){}
	public void ActivateGroup( int i_groupID ){}
	public void DeactivateGroup( int i_groupID ){}
	public void HackerPing( int i_hexIndex ){}
	public void ThiefPing( int i_hexIndex ){}
	public void DisableTransmitter( int i_hexIndex ){}
	public void TransmitterReset( int hexIndex ){}
	public void HackerCaught( int i_hexIndex ){}
	public void PasswordPlatformStateChange( int i_hexID, bool i_open ){}
	public void InfoNodePlatformStateChange( int i_ID, bool i_open ){}
	public void LockdownEndDoor(){}
	public void RestoreEndDoor(){}
	public void LockDownSAPs(){}
	public void RestoreSAPs(){}
	public GameObject[] GetDoorsOfType( DoorType i_type ){ return null; }

#else
	#region Guard

	public void UpdateGuardPos(Vector3 i_guardPosition, int i_guardID)
	{
		GuardOverlord.Manager.m_Guards[i_guardID].GetComponent<GuardSync>().SetPosition(i_guardPosition);
	}

	public void UpdateGuardRot(Quaternion i_guardQuaternion, int i_guardID)
	{
		GuardOverlord.Manager.m_Guards[i_guardID].GetComponent<GuardSync>().SetRotation(i_guardQuaternion);
	}

	#endregion
	#region Door
	public GameObject GetDoorObject( int i_doorNumber )
	{
		GameObject[] doors = GameObject.FindGameObjectsWithTag("Door");
		foreach(GameObject door in doors)
		{
			if(door.GetComponent<IDoorController>().GetDoorIndex() == i_doorNumber)
			{			
				return door;
			}			
		}
		return null;
	}

	public GameObject[] GetDoorsOfType( DoorType i_type )
	{
		GameObject[] doors = GameObject.FindGameObjectsWithTag("Door");
		List<GameObject> doorsOfType = new List<GameObject>();
		foreach(GameObject door in doors)
		{
			if(door.GetComponent<IDoorController>().GetDoorType() == i_type)
			{			
				doorsOfType.Add( door );
			}			
		}
		return doorsOfType.ToArray();
	}

	//acutally open the freaking door, and change the relative booleans.
	public void OpenDoor(int i_doorNumber)
	{
		GetDoorObject(i_doorNumber).GetComponent<IDoorController>().OpenDoor();
	}
	
	public void CloseDoor(int i_doorNumber)
	{
		GetDoorObject(i_doorNumber).GetComponent<IDoorController>().CloseDoor();
	}
	
	//acutally lock the freaking door, update the door indicator and change the relative booleans.
	public void LockDoor(int i_doorNumber)
	{
		GameObject door = GetDoorObject(i_doorNumber);
		if( door != null )
			GetDoorObject(i_doorNumber).GetComponent<IDoorController>().LockDoor();
	}
	
	//acutally unlock the freaking door, update the door indicator and change the relative booleans.
	public void UnlockDoor(int i_doorNumber)
	{
		GetDoorObject(i_doorNumber).GetComponent<IDoorController>().UnlockDoor();
	}
	
	public void SecureLockDoor(int i_doorNumber)
	{
		GetDoorObject(i_doorNumber).GetComponent<IDoorController>().DeadlockDoor();
	}
	
	public void UnSecureLockDoor(int i_doorNumber) 
	{
		GameObject door = GetDoorObject(i_doorNumber);
		//if( !OverrideManager.Manager.IsActive )
		door.GetComponent<IDoorController>().UnDeadlockDoor();
		//else if( !door.GetComponent<DoorController>().isEndDoor )
			//door.GetComponent<DoorController>().UnDeadlockDoor();
	}

	public void  GetPingPosition(Vector3 i_pos)
	{

	}
	#endregion
	
	#region PowerNode
	
	public void PowerNode(int PowerNodeID, bool status)
	{
		GameObject powerNode = GameObject.Find ("PowerNode" + PowerNodeID);
		powerNode.GetComponent<PowerOnNode>().ChangePowerNodeState(status);
	}
	
	#endregion
	
	#region FloorGrid

	public void SetLinkState( int i_index, ThiefGrid.LinkState i_state )
	{
		GameObject tGM = GameObject.FindGameObjectWithTag("ThiefGridManager");
		tGM.GetComponent<ThiefGrid>().SetLinkState( i_index, i_state );
	}
	
	#endregion
	
	#region Laser
	
	public void ActivateGroup( int i_groupID )
	{
		LaserManager.Manager.ActivateGroup(i_groupID);
	}
	
	public void DeactivateGroup( int i_groupID )
	{
		LaserManager.Manager.DeactivateGroup(i_groupID);
	}
	
	#endregion
	
	
	public void HackerPing( int i_hexIndex )
	{
		ThiefGrid.Manager.RecievePing( i_hexIndex );
		//Debug.Log("I created a thief Ping");
		OffScreenPingIndicator.Manager.SetOffScreenPingPosition(i_hexIndex);
	}
	
	public void ThiefPing( int i_hexIndex )
	{
		ThiefGrid.Manager.CreatePing( i_hexIndex );
	}
	
	public void DisableTransmitter( int i_hexIndex )
	{
			List<int> affectedLines = HexGrid.Manager.GetTransmitter( i_hexIndex ).GetAffectedLines();
			//DeactivateHotSpot( i_hexIndex );
			ThiefGrid.Manager.KillTransmitter( i_hexIndex, affectedLines );
	}
	
	
	public void TransmitterReset( int hexIndex )
	{
		ThiefGrid.Manager.ResetTransmitter( hexIndex );
	}
	
	public void HackerCaught( int i_hexIndex )
	{
		List<Transmitter> transmittersToKill = HexGrid.Manager.GetTransmittersInRange( i_hexIndex );
		HexGrid.Manager.DeactivateHotSpotsInRange( i_hexIndex );
		
		for ( int i=0 ; i<transmittersToKill.Count ; i++ )
		{
			//Debug.Log ("Removing floor grid for transmitter at: " + transmittersToKill[i].index);
			List<int> affectedLines = HexGrid.Manager.GetTransmitter( transmittersToKill[i].index ).GetAffectedLines();
			ThiefGrid.Manager.KillTransmitter( transmittersToKill[i].index, affectedLines );
		}
	}
	
	public void PasswordPlatformStateChange( int i_hexID, bool i_open )
	{
		BonusManager.Manager.TriggerPasswordPlatformAnimation( i_hexID, i_open );
	}

	public void InfoNodePlatformStateChange( int i_ID, bool i_open )
	{
		InfoNodeManager.Manager.InfoHexCaptured( i_ID, i_open );
	}

	#region Lockdown stuff
	
	public void LockdownEndDoor()
	{
		GameObject[] doors = GameObject.FindGameObjectsWithTag("Door");
		foreach( GameObject d in doors )
		{
			IDoorController dc = d.GetComponent<IDoorController>();
			if( dc == null )
				return;
			if( dc._type == DoorType.EndDoor )
				NetworkManager.Manager.SecureLockDoor( dc.DoorNumber );
		}
	}
	
	public void RestoreEndDoor()
	{
		GameObject[] doors = GameObject.FindGameObjectsWithTag("Door");
		foreach( GameObject d in doors )
		{
			IDoorController dc = d.GetComponent<IDoorController>();
			if( dc == null )
				return;
			if( dc._type == DoorType.EndDoor )
				NetworkManager.Manager.UnSecureLockDoor( dc.DoorNumber );
		}
	}
	
	public void LockDownSAPs()
	{
		GameObject[] saps = GameObject.FindGameObjectsWithTag("SAP");
		foreach( GameObject sap in saps )
		{
			sap.GetComponent<SecurityAccessPanel>().ActivateLockdownState();
		}
	}
	
	public void RestoreSAPs()
	{
		GameObject[] saps = GameObject.FindGameObjectsWithTag("SAP");
		foreach( GameObject sap in saps )
		{
			sap.GetComponent<SecurityAccessPanel>().ReturnToOriginalState();
		}
	}
	
	#endregion
	
	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
	
	#endif
}


