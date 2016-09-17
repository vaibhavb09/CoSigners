
using UnityEngine;
using System.Collections;

public class LaserNode : Node 
{
	
	private bool 		isActivated;
	private int 		laserGroupID;
	
	
//	public void Set( LaserNodeData i_data )
//	{
//	}
	
	
	// Use this for initialization
	void Start () 
	{
		isActivated = false;
		laserGroupID = 0;
	}
		
	public LaserNode()
	{
		
	}
	
	public override void SetConnected( bool i_connected )
	{
		Connected = i_connected;
		if ( i_connected && HackerManager.Manager.CheckHackerClearance( SecurityLevel ) )
		{
			//Connected
			
		}
		else
		{			
			//Dont have clearance or disconnected.
		}
	}
	
	public override void SetClearance ( )
	{
		//temp
		SecurityLevel = 1;
	}
	
	public override void HandleClickEvent()
	{
		if ( Connected && HackerManager.Manager.CheckHackerClearance( SecurityLevel ) )
		{
			isActivated = isActivated ? false : true;
			if( isActivated )
			{
				NetworkManager.Manager.DeactivateGroup( laserGroupID ); 
				//Debug.Log( "\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\ Laser group " + laserGroupID + " deactivated" );
			}
			else
			{
				NetworkManager.Manager.ActivateGroup( laserGroupID );
				//Debug.Log( "\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\ Laser group " + laserGroupID + " activated" );
			}
		}
	}
	
	// Update is called once per frame
	void Update ()
	{		
	}
	
	
}
