using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IRNode : Node {
	
	public float _radius;
	public GameObject guardIconPrefab;
	public bool IRboost=false;
	private float boost;
	private GameObject _myWave;
	private List<GameObject> guardIcons;
	private List<Transform> guardTransforms;
	
	
	public IRNode()
	{
	}
	
	public void Set( IRNodeData i_data )
	{
		Index = i_data.Index;
		Connected = false;
		SecurityLevel = i_data.SecurityLevel;
		PowerConsumption = 10;
		_radius = i_data.Radius;
		Type = GameManager.NodeType.InfraRed;
		float myScale = GraphManager.Manager.DefaultScale;
		this.gameObject.transform.localScale = new Vector3(myScale, myScale, myScale);
	}
	
	// Use this for initialization
	void Start () {
	}
	
	
	
	// Update is called once per frame
	void Update ()
	{
		/*
		if ( Connected )
		{
			DisplayVisibleGuards();
		}*/
		/*
		if( HackerManager.Manager.CheckHackerClearance( this.SecurityLevel ) == false )
		{
			this.gameObject.transform.renderer.material = GraphManager.Manager.IRUnavailable;
		}
		if( !Connected && HackerManager.Manager.CheckHackerClearance( this.SecurityLevel ) == true )
			this.gameObject.transform.renderer.material = GraphManager.Manager.IRAvailable;
			*/
	}
	
	
	public override void SetConnected( bool i_connected )
	{
		Connected = i_connected;
		if ( i_connected && HackerManager.Manager.CheckHackerClearance( this.SecurityLevel ) )
		{
			//AudioManager.Manager.HackerConnectsToIRNode( true );
			
			BasicScoreSystem.Manager.IRNodesCaptured += 1;
			StartIRWave();
			this.gameObject.transform.renderer.material = GraphManager.Manager.IRConnected;
			
			// [ SOUND TAG ] IR Node powered [Node_Connect]
			soundMan.soundMgr.playOneShotOnSource(null,"Node_Connect",GameManager.Manager.PlayerType,2);
		}
		else
		{
			// End Wave if no longer connected
			if( !i_connected && HackerManager.Manager.CheckHackerClearance( this.SecurityLevel ) ) 
			{
				//AudioManager.Manager.HackerDisconnectsFromIRNode( true );
				
				// [ SOUND TAG ] IR Node powered down [Node_Disconnect]
				soundMan.soundMgr.playOneShotOnSource(null,"Node_Disconnect",GameManager.Manager.PlayerType,2);
				
				BasicScoreSystem.Manager.IRNodesCaptured -= 1;
				EndIRWave();	
			}
			
			// Set Materials
			if( HackerManager.Manager.CheckHackerClearance( this.SecurityLevel ) == false )
			{
				this.gameObject.transform.renderer.material = GraphManager.Manager.IRUnavailable;
			}
			else
				this.gameObject.transform.renderer.material = GraphManager.Manager.IRAvailable;
		}
	}
	
	public override void SetClearance ( )
	{
		if( !HackerManager.Manager.CheckHackerClearance( SecurityLevel ) )
		{
			this.gameObject.transform.renderer.material = GraphManager.Manager.IRUnavailable;
		}
		else
		{
			this.gameObject.transform.renderer.material = GraphManager.Manager.IRAvailable;
		}
	}
	
	public override void HandleClickEvent()
	{
		//Debug.Log ("You clicked on an IR Node");
	}
	
	private void InstanstiateGuardIcons()
	{
		if( guardIcons == null )
		{
			guardIcons = new List<GameObject>();
			guardTransforms = new List<Transform>();
			GameObject[] guards = GameObject.FindGameObjectsWithTag("Guard");
			foreach ( GameObject guard in guards )
			{
				Vector3 temp = guard.transform.position;
				temp.y = 60.0f;
				guardIcons.Add( (GameObject)Instantiate( guardIconPrefab, temp, guard.transform.rotation ) );
				guardTransforms.Add( guard.transform );
			}
		}
	}
	
	
	private void StartIRWave()
	{
		_myWave = (GameObject)Instantiate( GraphManager.Manager.IRWave, HexGrid.Manager.GetCoord(Index, 60.0f), Quaternion.identity);
		SetWaveRadius(_radius);
	}
	
	
	private void EndIRWave()
	{
		Destroy(_myWave);
	}
	
	
	private void SetWaveRadius(float i_radius)
	{
		float tempRadius;
		
		if(IRboost==true)
		   tempRadius = i_radius+boost;
		else 
		   tempRadius = i_radius;
		
		float myScale = (tempRadius/5.0f);
		_myWave.transform.localScale = new Vector3(myScale, myScale, myScale);
	}
	
	public void AddIRBoost()
	{
		
		IRboost=true;
		
		if(Connected)
		{
			float tempRadius=_radius+boost;
			float myScale = (tempRadius/5.0f);
			_myWave.transform.localScale = new Vector3(myScale, myScale, myScale);
		}
	}
	
	public void RemoveIRBoost()
	{
		
		IRboost=false;
		if(Connected)
		{    
			float tempRadius=_radius-boost;
			float myScale = (tempRadius/5.0f);
			_myWave.transform.localScale = new Vector3(myScale, myScale, myScale);
		}
	}
	
	
	
	public bool GuardIsWithinRange( Vector3 i_pos )
	{
		Vector3 nodePos = HexGrid.Manager.GetCoord( this.Index );
		Vector2 guardPos2 = new Vector2( i_pos.x, i_pos.z );
		Vector2 nodePos2 = new Vector2( nodePos.x, nodePos.z);
		float dist = Vector2.Distance( nodePos2, guardPos2 );
		
		if ( dist < (_radius-0.2) )
			return true;
		else
			return false;
	}
	
			
}
