using UnityEngine;
using System.Collections;

public class HackerNetManager {
	
	#region singleton declearation
	private static HackerNetManager m_instance;
	private HexGrid _hexGrid;
	
	public HackerNetManager () 
    {
        if (m_instance != null)
        {
            return;
        }
 
        m_instance = this;
    }
	
	public static HackerNetManager Manager
	{
		get
		{
			if(m_instance == null)
			{
				m_instance = new HackerNetManager();			
			}
			return m_instance;
		}
	}
	#endregion
	
	#region Door stuff
	
	public void OpenDoor( int i_doorIndex )
	{ 
		DoorNode thisDoor = (DoorNode)GraphManager.Manager.GetNode( i_doorIndex );
		//thisDoor.OpenDoor();
		DoorStates.OnPlayerOpen( thisDoor );
	}
	
	public void CloseDoor( int i_doorIndex )
	{
		DoorNode thisDoor = (DoorNode)GraphManager.Manager.GetNode( i_doorIndex );
		//thisDoor.CloseDoor();
		DoorStates.OnPlayerClose( thisDoor );
	}
	
	// find the node, update the UI for the node 	
	public void LockDoor( int i_doorIndex )
	{
		DoorNode thisDoor = (DoorNode)GraphManager.Manager.GetNode( i_doorIndex );
		thisDoor.NormalLock();
	}
	
	// find the node, update the UI for the node 
	public void UnlockDoor( int i_doorIndex )
	{
		DoorNode thisDoor = (DoorNode)GraphManager.Manager.GetNode( i_doorIndex );
		thisDoor.Unlock();
	}

	public void SecureLockDoor( int i_doorIndex )
	{
		DoorNode thisDoor = (DoorNode)GraphManager.Manager.GetNode( i_doorIndex );
		thisDoor.SecureLock();
	}

	public void UnSecureLockDoor( int i_doorIndex )
	{
		DoorNode thisDoor = (DoorNode)GraphManager.Manager.GetNode( i_doorIndex );
		thisDoor.Unlock();
	}
	
	#endregion
	
	#region PowerNode
	
	public void PowerNode(int PowerNodeID, bool status)
	{
		
		//GameObject node = GraphManager.Manager.getGameObjectByNumber(PowerNodeID);
		//Node powerNode = node.GetComponent<Node>();
		//powerNode.PowerNode(status);
		
	}
	
	#endregion
	
	#region EMP
	
	public void EMPActivated(float i_EmpPos_x, float i_EmpPos_z, float EMP_Influence_Radius)
	{
		Vector2 empPosition = new Vector2(i_EmpPos_x,i_EmpPos_z);
		
		//GraphManager.Manager.EMPActivated(empPosition, EMP_Influence_Radius);
		
		// Indicate to the security overlord that an emp happened at indicated location
		//GraphManager.overlord.EMPat(empPosition,EMP_Influence_Radius);
		
		UIManager.Manager.CreateEMPShockWaveAtPlayerPosition(EMP_Influence_Radius);
		//Debug.Log ("Hacker Net Manager Received the emp call");
	}
	
	#endregion
	
	
	#region IR
	
	public void EnableIR(int i_nodeNumber)
	{
		//Debug.Log ("Hacker Side, Enable IR " + i_nodeNumber);
		//Node nodeScript = GraphManager.Manager.getNodeByIndex(i_nodeNumber);
		//IRSystem.ISystem.AddIRNode(nodeScript);
		//UIManager.Manager.CreateIRShockWaveForNode(nodeScript, IRSystem.ISystem.irRadius);
	}
	
	public void DisableIR(int i_nodeNumber)
	{
		//Node nodeScript = GraphManager.Manager.getNodeByIndex(i_nodeNumber);
		//IRSystem.ISystem.RemoveIRNode(nodeScript);
		//nodeScript.DeleteIRWave();
	}
	#endregion
	
	
	#region Override
	
	public void InitializeLockDownSequence(int hexIndex)
	{
		OverrideManager.Manager.CreateOverrideBasedOnIndex(hexIndex);
		//SecurityManager.Manager.DisableTracerCreators();
		OverrideManager.Manager.Refresh();
	}
	
	public void OverrideSuccess()
	{
		OverrideManager.Manager.OverrideSuccess();
		SecurityManager.Manager.EnableTracerCreators();
	}
	
	public void EnableOverride()
	{
		OverrideManager.Manager.EnableOverride();
	}
	
	public void DisableOverride()
	{
		OverrideManager.Manager.DisableOverride();
	}
	
	#endregion

	#region InfoNode

	public void PlayInformation( int i_ID )
	{
		InfoNodeManager.Manager._currentInfoNodeID = i_ID;
		InfoNodeManager.Manager.PlayInformation( i_ID );
	}

	#endregion

	#region Transmitter
	
	public void TransmitterPlaced( int i_hexIndex )
	{
		if(_hexGrid == null)
		{
			_hexGrid = GameObject.Find("HexGrid").GetComponent<HexGrid>();
		}
		_hexGrid.AddHotSpot( i_hexIndex );
	}
	
	public void TransmitterReset( int i_hexIndex )
	{
		if(_hexGrid == null)
		{
			_hexGrid = GameObject.Find("HexGrid").GetComponent<HexGrid>();
		}
		_hexGrid.ResetTransmitter( i_hexIndex );
	}
	
	public void ScramblerPlaced(int i_hexIndex)
	{
		if(_hexGrid == null)
		{
			_hexGrid = GameObject.Find("HexGrid").GetComponent<HexGrid>();
		}
		_hexGrid.AddScrambler( i_hexIndex );
	}
	
	
	public void RemoveScrambler(int i_hexIndex)
	{
		if(_hexGrid == null)
		{
			_hexGrid = GameObject.Find("HexGrid").GetComponent<HexGrid>();
		}
		_hexGrid.RemoveScrambler( i_hexIndex );
	}
	
	public void TransmitterPickUp( int i_hexIndex )
	{
		if(_hexGrid == null)
		{
			_hexGrid = GameObject.Find("HexGrid").GetComponent<HexGrid>();
		}
		_hexGrid.RemoveHotSpot( i_hexIndex );
	}
	
	#endregion

	#region PauseGame
	
	public void PauseGame(bool status)
	{
		HackerManager.Manager.PauseGame(status);
	}
	
	#endregion

	#region Pings
	public void ThiefPing( int i_hexIndex )
	{
		HackerManager.Manager.RecievePing( i_hexIndex );
	}
	
	public void CreateThiefObjectPing( float rotation, float xScale, float zScale, float xPosition, float zPosition )
	{
		HackerManager.Manager.CreateThiefObjectPing( rotation, xScale, zScale, xPosition, zPosition );
	}
	#endregion
	
	#region Override
	public void CreateOverride()
	{
		OverrideManager.Manager.CreateOverrideBasedOnPlayerPosition();
	}
	#endregion
	
	public void SetPasswordHacker( string i_password )
	{
		PasswordGenerator.Manager.SetPassword( i_password );
	}

	public void AddPassword( string i_password )
	{
		PasswordGenerator.Manager.AcceptablePasswords.Add( i_password );
	}

	#region HackerThreat/SecurityAccessPanel
	
	public void ActivateSecurityPanel() //Drops hacker threat level
	{
		HackerThreat.Manager.DecreaseThreatAmount(10);
	}

	#endregion
}
