using UnityEngine;
using System.Collections;

public class EnergyNode : Node
{
	public GameObject m_EnergyNodePrefab;
	
	public void Set( EnergyNodeData i_data )
	{
		Index = i_data.Index;
		Connected = false;
		SecurityLevel = i_data.SecurityLevel;
		PowerConsumption = 0;
		Type = GameManager.NodeType.Energy;
		float myScale = GraphManager.Manager.DefaultScale;
		this.gameObject.transform.localScale = new Vector3(myScale, myScale, myScale);
	}
	
	void Update()
	{
		
	}
	
	public override void SetConnected( bool i_connected )
	{
		
		if((Connected) && (!i_connected))
		{
			// [ SOUND TAG ] Energy Node powered [Node_Disconnect]
			if(GameManager.Manager.PlayerType == 2)
				soundMan.soundMgr.playOneShotOnSource(null,"Node_Disconnect",GameManager.Manager.PlayerType,GameManager.Manager.PlayerType);
		}
		
		Connected = i_connected;
		if ( i_connected && HackerManager.Manager.CheckHackerClearance( this.SecurityLevel ) )
		{
			// [ SOUND TAG ] Energy Node powered [Node_Connect]
			if(GameManager.Manager.PlayerType == 2)
				soundMan.soundMgr.playOneShotOnSource(null,"Node_Connect",GameManager.Manager.PlayerType,GameManager.Manager.PlayerType);
			
			//this.gameObject.transform.renderer.material = GraphManager.Manager.IRConnected;
			HackerManager.Manager.powerCapacity++;
		}
		else
		{			
			if( !i_connected && HackerManager.Manager.CheckHackerClearance( this.SecurityLevel ) ) 
			{
				HackerManager.Manager.powerCapacity--;
			}
			if( HackerManager.Manager.CheckHackerClearance( this.SecurityLevel ) == false )
			{
				//this.gameObject.transform.renderer.material = GraphManager.Manager.IRUnavailable;
			}
			//else
				//this.gameObject.transform.renderer.material = GraphManager.Manager.IRAvailable;
		}
	}
}
