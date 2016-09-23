using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Vectrosity;

public class ThiefGrid : MonoBehaviour {
	
	public class FloorLine
	{
		public Transform _transform;
		public LinkState _state;

	
		public FloorLine (LinkState i_state, Transform i_line)
		{
			_state = i_state;
			_transform = i_line;
		}
	}
	
	
	public enum LinkState
	{
		UNAVAILABLE,
		AVAILABLE,
		CONNECTED,
		POWERED,
		JAMMED
	};
	
	private static ThiefGrid _instance;
	HexGrid _hexGrid;
	Dictionary<int, LineEffect> GridLines;
	Dictionary<int, Transform> ActiveTransmitters;
	Dictionary<int, Transform> InActiveTransmitters;
	
	public Dictionary<int, Transform> ActiveScramblers;
	
	public Material UnavailableMaterial;  
	public Material AvailableMaterial;
	public Material ConnectedMaterial;
	public Material PoweredMaterial;
	public Material JammedMaterial;
	public Material ConnectedNoise;
	public Material PoweredNoise;
	public Material ActiveState;
	public Material ResetState;
	public Material DeadState;
	
	public Transform line;
	public Transform activeTransmitter;
	public Transform ghostTransmitter;
	public Transform ghostScrambler;
	public Transform activeScrambler;
	public Transform resetTransmitter;
	public Transform jammer;
	public Transform pingRecievedIndicator;
	public Transform pingRecievedOutline;
	//public Transform pingMadeIndicator;
	public Transform pingCreatedIndicator;
	public Transform probablePingIndicator;
	
	private int prevPingHexIndex = -1;
	
	
	// Use this for initialization
	void Start () 
	{}
	
	
	public static ThiefGrid Manager
	{
		get
		{
			if(_instance == null)
			{
				_instance = new ThiefGrid();			
			}
			return _instance;
		}
	}
	
	
	#region Constructor
	public ThiefGrid () 
    { 
        _instance = this;
    }
	#endregion
	
	
	public void Load()
	{
		ActiveTransmitters = new Dictionary<int, Transform>();
		InActiveTransmitters = new Dictionary<int, Transform>();
		
		ActiveScramblers=new Dictionary<int, Transform>();
			
		_hexGrid = HexGrid.Manager;
		LoadResources();
		LoadGridLines();
	}
	
	
	// Update is called once per frame
	void Update () {
	}
	
	
	public void LoadGridLines()
	{
		Debug.Log ("LoadGridLines - _hexGrid.gridSize = " + _hexGrid.gridSize);
		GridLines = new Dictionary<int, LineEffect>();
		for ( int i = 0 ; i < _hexGrid.gridSize ; i+=2 )
		{
			if ( i%2 == 0 )
			{
				Vector3 linePos = _hexGrid.GetCoord(i, 0.01f);
				if ( i%_hexGrid.rowSize != 0 )
				{
					Transform horzLine = (Transform) Instantiate(line, linePos, Quaternion.identity);
					horzLine.Rotate( new Vector3(0.0f, -90.0f, 0.0f) );
					
					if ( !HexGrid.Manager.IsLineJammed( i*10 ) )
					{
						//horzLine.renderer.material = UnavailableMaterial;
						LineEffect lineScript = horzLine.GetComponent<LineEffect>();
						lineScript.SetState(LinkState.UNAVAILABLE);
						FloorLine newLine = new FloorLine( LinkState.UNAVAILABLE, horzLine);
						GridLines.Add(i*10, lineScript);
					}
					else
					{
						//horzLine.renderer.material = JammedMaterial;
						LineEffect lineScript = horzLine.GetComponent<LineEffect>();
						lineScript.SetState(LinkState.JAMMED);
						FloorLine newLine = new FloorLine( LinkState.JAMMED, horzLine);
						GridLines.Add(i*10, lineScript);
					}

				}
				if ( i > _hexGrid.rowSize )
				{
					Transform upLine = (Transform) Instantiate(line, linePos, Quaternion.identity);
					upLine.Rotate( new Vector3(0.0f, 30.0f, 0.0f) );
					
					if ( !HexGrid.Manager.IsLineJammed( i*10+1 ) )
					{
						//upLine.renderer.material = UnavailableMaterial;
						LineEffect lineScript = upLine.GetComponent<LineEffect>();
						lineScript.SetState(LinkState.UNAVAILABLE);
						FloorLine newLine = new FloorLine( LinkState.UNAVAILABLE, upLine);
						GridLines.Add(i*10+1, lineScript);
					}
					else{
						//upLine.renderer.material = JammedMaterial;
						LineEffect lineScript = upLine.GetComponent<LineEffect>();
						lineScript.SetState(LinkState.JAMMED);
						FloorLine newLine = new FloorLine( LinkState.JAMMED, upLine);
						GridLines.Add(i*10+1, lineScript);
					}
				}
				if ( i < _hexGrid.gridSize - _hexGrid.rowSize )
				{
					Transform downLine = (Transform) Instantiate(line, linePos, Quaternion.identity);
					downLine.Rotate( new Vector3(0.0f, 150.0f, 0.0f) );
					if ( !HexGrid.Manager.IsLineJammed( i*10+2 ) )
					{
						//downLine.renderer.material =  UnavailableMaterial;
						LineEffect lineScript = downLine.GetComponent<LineEffect>();
						lineScript.SetState(LinkState.UNAVAILABLE);
						FloorLine newLine = new FloorLine( LinkState.UNAVAILABLE, downLine);
						GridLines.Add(i*10+2, lineScript);
					}
					else{
						//downLine.renderer.material =  JammedMaterial;
						LineEffect lineScript = downLine.GetComponent<LineEffect>();
						lineScript.SetState(LinkState.JAMMED);
						FloorLine newLine = new FloorLine( LinkState.JAMMED, downLine);
						GridLines.Add(i*10+2, lineScript);
					}
				}
			}
		}
	}

	public void LoadGhostTransmitter()
	{
		activeTransmitter = Resources.Load("Prefabs/Theif/Transmitter_Prefab", typeof(Transform)) as Transform;
	}

	private void LoadResources()
	{
		line = Resources.Load ("Prefabs/Theif/FloorLineThick", typeof(Transform)) as Transform;
		//activeTransmitter = Resources.Load("Prefabs/Theif/Transmitter_Prefab", typeof(Transform)) as Transform;
		ghostTransmitter = Resources.Load("Prefabs/Theif/GhostTransmitter", typeof(Transform)) as Transform;
		jammer = Resources.Load("Prefabs/Theif/Jammer_Prefab", typeof(Transform)) as Transform;
		resetTransmitter = Resources.Load("Prefabs/Theif/TransmitterResetting_Prefab", typeof(Transform)) as Transform;
		ghostScrambler =Resources.Load("Prefabs/Theif/GhostTransmitter", typeof(Transform)) as Transform;
		activeScrambler = Resources.Load("Prefabs/Theif/Scrambler_Prefab", typeof(Transform)) as Transform;
		pingRecievedOutline = Resources.Load("Prefabs/Theif/ThiefPingRecievedOutline", typeof(Transform)) as Transform;
		
		JammedMaterial = Resources.Load("Materials/Thief/FloorLineJammed", typeof(Material)) as Material;
		UnavailableMaterial = Resources.Load("Materials/Thief/FloorLineUnavailable", typeof(Material)) as Material;
		AvailableMaterial = Resources.Load("Materials/Thief/FloorLIneAvailable", typeof(Material)) as Material;
		ConnectedMaterial = Resources.Load("Materials/Thief/GlowLineLinked", typeof(Material)) as Material;
		PoweredMaterial = Resources.Load("Materials/Thief/GlowLinePowered", typeof(Material)) as Material;
		ConnectedNoise = Resources.Load("Materials/Thief/GlowNoise2", typeof(Material)) as Material;
		PoweredNoise = Resources.Load("Materials/Thief/GlowNoise", typeof(Material)) as Material;
		
		ActiveState = Resources.Load("Materials/Thief/GlowGreen", typeof(Material)) as Material;
		ResetState = Resources.Load("Materials/Thief/GuardLightSeen", typeof(Material)) as Material;
		DeadState = Resources.Load("Materials/Thief/GuardLightAlert", typeof(Material)) as Material;
	}
	
	
	public void SetLinkState( int i_index, LinkState i_state )
	{
		Debug.Log ("in SetLinkState ");
		LineEffect lineScript = GridLines[i_index].gameObject.GetComponent<LineEffect>();
		if ( !HexGrid.Manager.IsLineJammed( i_index ) )
		{
			switch( i_state )
			{
				case LinkState.UNAVAILABLE : 
					lineScript.SetState(LinkState.UNAVAILABLE);
					//GridLines[i_index]._transform.renderer.material = UnavailableMaterial;
					GridLines[i_index]._state = LinkState.UNAVAILABLE;
					break;
				
				case LinkState.AVAILABLE : 
					lineScript.SetState(LinkState.AVAILABLE);
					//GridLines[i_index]._transform.renderer.material = AvailableMaterial;
					GridLines[i_index]._state = LinkState.AVAILABLE;
					break;
				
				case LinkState.CONNECTED : 
					lineScript.SetState(LinkState.CONNECTED);
					//GridLines[i_index]._transform.renderer.material = ConnectedMaterial;
					GridLines[i_index]._state = LinkState.CONNECTED;
					break;
				
				case LinkState.POWERED :
					lineScript.SetState(LinkState.POWERED);
					//GridLines[i_index]._transform.renderer.material = PoweredMaterial;
					GridLines[i_index]._state = LinkState.POWERED;
					break;
				case LinkState.JAMMED :
					lineScript.SetState(LinkState.JAMMED);
					//GridLines[i_index]._transform.renderer.material = JammedMaterial;
					GridLines[i_index]._state = LinkState.JAMMED;
					break;
			}
		}
		else
		{
			//GridLines[i_index]._transform.renderer.material = JammedMaterial;
			lineScript.SetState(LinkState.JAMMED);
			GridLines[i_index]._state = LinkState.JAMMED;
		}
	}

	public void PlaceScrambler( Vector3 i_pos, Vector3 i_direction, int i_hexIndex )
	{
		Quaternion rotation = Quaternion.Euler(i_direction.x, i_direction.y, i_direction.z);
		//play animation of scrambler
		Transform newScrambler = (Transform) Instantiate(activeScrambler, i_pos, rotation);
		List <int>checkHexes=HexGrid.Manager.GetSurroundingHexes(i_hexIndex);
		newScrambler.GetComponent<Scrambler>().Set(i_hexIndex, checkHexes);
		ActiveScramblers.Add( i_hexIndex, newScrambler );
		
	}
	
	
	public void RemoveScrambler(int i_hexIndex)
	{
		//play the scrambler closing animation 
		ActiveScramblers.Remove( i_hexIndex );
	}
	
	
	public void PlaceTransmitter( Vector3 i_pos, Vector3 i_direction, int i_hexIndex )
	{
		if ( !BonusManager.Manager.ContainsBonus(i_hexIndex) )
		{
			Quaternion rotation = Quaternion.Euler(i_direction.x, i_direction.y, i_direction.z);
			Transform newTransmitter = (Transform) Instantiate(activeTransmitter, i_pos, rotation);
			ActiveTransmitters.Add( i_hexIndex, newTransmitter );
				
				ActivateTransmitter( i_hexIndex );			
		}
	}
	
	public void RemoveTransmitter( int i_hexIndex, List<int> affecteLines )
	{
		ActiveTransmitters.Remove( i_hexIndex );
		DeactivateTransmitter( affecteLines );
	}
	
	
	public void ResetTransmitter( int i_hexIndex, List<int> affecteLines )
	{
		if ( ActiveTransmitters.ContainsKey( i_hexIndex) )
		{
			Transform tempTransmitter = ActiveTransmitters[i_hexIndex];
			foreach (Transform child in tempTransmitter)
			{
				child.renderer.material = ResetState;
			}
			
			ActiveTransmitters.Remove( i_hexIndex );
			InActiveTransmitters.Add ( i_hexIndex, tempTransmitter);
			DeactivateTransmitter( affecteLines );
		}
		
		Action timerEndAction = delegate(){ActivateTransmitter(i_hexIndex);};
		GenericTimer myGenericTimer = gameObject.AddComponent<GenericTimer>();
		myGenericTimer.Set( 2.0f, false, timerEndAction );
		myGenericTimer.Run();
	}
	
	public void ResetTransmitter( int i_hexIndex )
	{
		List<int> affectedLines;
		if ( HexGrid.Manager.HasTransmitter( i_hexIndex ) )
			affectedLines = HexGrid.Manager.GetTransmitter( i_hexIndex ).GetAffectedLines();
		else 
		{
			//Debug.Log ("Transmitter " + i_hexIndex + " Not found");
			return;
		}
		
		Transform tempTransmitter = ActiveTransmitters[i_hexIndex];
		foreach (Transform child in tempTransmitter)
		{
			child.renderer.material = ResetState;
		}
		ActiveTransmitters.Remove( i_hexIndex );
		InActiveTransmitters.Add ( i_hexIndex, tempTransmitter);
		DeactivateTransmitter( affectedLines );
				
		Action timerEndAction = delegate(){ActivateTransmitter(i_hexIndex);};
		GenericTimer myGenericTimer = gameObject.AddComponent<GenericTimer>();
		myGenericTimer.Set( 2.0f, false, timerEndAction );
		myGenericTimer.Run();
	}
	
	
	public void KillTransmitter( int i_hexIndex, List<int> i_affectedLines)
	{
		for ( int i=0 ; i<i_affectedLines.Count ; i++ )
		{
			
			bool covered = false;
			foreach ( KeyValuePair<int, Transform> t in ActiveTransmitters )
			{
				Transmitter trans = _hexGrid.GetTransmitter( t.Key );
				if ( trans.ContainsLine( i_affectedLines[i] ) && trans.isActive )
				{
					covered = true;
					break;
				}
			}
			if ( !covered )
				SetLinkState( i_affectedLines[i], LinkState.UNAVAILABLE );
				
		}
		
		if ( InActiveTransmitters.ContainsKey( i_hexIndex ) )
		{
			Transform tempTransmitter = ActiveTransmitters[i_hexIndex];
			foreach (Transform child in tempTransmitter)
			{
				child.renderer.material = DeadState;
			}
		}
		
	}
	
	
	public void DeactivateTransmitter( List<int> i_affectedLines )
	{
		for ( int i=0 ; i<i_affectedLines.Count ; i++ )
		{
			// Evaluate each line.

			if ( HexGrid.Manager.IsLineJammed( i_affectedLines[i] ) )
			{
				SetLinkState(i_affectedLines[i], LinkState.JAMMED);
			}
			else
			{
				if ( ConnectionManager.Manager.IsLinePowered( i_affectedLines[i] ) )
				{
					//Debug.Log ("Setting Line: " + l.Key + " to " + " Powered");
					SetLinkState(i_affectedLines[i], LinkState.POWERED);
				}
				else if ( ConnectionManager.Manager.IsLineConnected( i_affectedLines[i] ) )
				{
					SetLinkState(i_affectedLines[i], LinkState.CONNECTED);
					
				}
				else if ( HexGrid.Manager.IsLineAvailable ( i_affectedLines[i] ) )
				{
					//Debug.Log ("Setting Line: " + l.Key + " to " + " Available");
					SetLinkState(i_affectedLines[i], LinkState.AVAILABLE);
				}
				else
				{
					SetLinkState(i_affectedLines[i], LinkState.UNAVAILABLE);
				}
			}
			/*
			bool covered = false;
			foreach ( KeyValuePair<int, Transform> t in ActiveTransmitters )
			{
				Transmitter trans = _hexGrid.GetTransmitter( t.Key );
				if ( trans.ContainsLine( i_affectedLines[i] ) )
				{
					covered = true;
					break;
				}
			}
			if ( !covered )
				SetLinkState( i_affectedLines[i], LinkState.UNAVAILABLE );
			*/
		}

	}
	
	
	public void ActivateTransmitter( int i_hexIndex )
	{
		Transmitter thisTransmitter = _hexGrid.GetTransmitter(i_hexIndex);
		if ( thisTransmitter != null )
		{
			// For each line in the affected list switch its state.
			List<int> lines = thisTransmitter.GetAffectedLines();
			for ( int i=0 ; i<lines.Count ; i++ )
			{
				if ( GridLines[lines[i]]._state == LinkState.UNAVAILABLE )
					SetLinkState( lines[i], LinkState.AVAILABLE );
			}
		}
		else
		{
			//Debug.LogError("COULD NOT GET TRANSMITTER TO ACTIVATE");
		}
		
		// If this was an inactive transmitter, move it to the activ list
		if ( InActiveTransmitters.ContainsKey( i_hexIndex ) )
		{
			Transform tempTransmitter = InActiveTransmitters[i_hexIndex];
			foreach (Transform child in tempTransmitter)
			{
				child.renderer.material = ActiveState;
			}
			InActiveTransmitters.Remove( i_hexIndex );
			ActiveTransmitters.Add ( i_hexIndex, tempTransmitter);
		}
	}
	
	public void DisableJammer( int i_hexIndex )
	{
		//Debug.Log ("ThiefGrid Disable Jammer");
		// Remove jammer lines
		List<int> lines = HexGrid.Manager.GetJammer( i_hexIndex ).GetAffectedLines();
		foreach ( int i in lines )
		{
			//Debug.Log ("Set Link to Unavailable");
			SetLinkState(i, LinkState.UNAVAILABLE);
		}
		
		// Add available lines where necessary
		foreach ( KeyValuePair<int, Transform> t in ActiveTransmitters )
		{
			ActivateTransmitter( t.Key );
		}
	}
	
	
	public bool IsInactive( int i_hexIndex )
	{
		if ( InActiveTransmitters.ContainsKey( i_hexIndex ) )
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	
	public void RecievePing( int i_hexIndex )
	{
		// AudioManager.Manager.ThiefReceivesPings( true );
		
		// Point man received a ping [SOUND TAG] [Ping]
		// If this is the Point man client
		if(GameManager.Manager.PlayerType == 1)
			soundMan.soundMgr.playOneShotOnSource(null,"Ping",GameManager.Manager.PlayerType,1);
		
		//Debug.Log ("I RECIEVED A PING ON HEX:" + i_hexIndex);
		Vector3 pingHexPos = HexGrid.Manager.GetCoordHex( i_hexIndex, 0.02f );
		Transform pingRecieved = (Transform) Instantiate(pingRecievedIndicator, pingHexPos, Quaternion.identity);
		Transform pingRecievedBorder = (Transform) Instantiate( pingRecievedOutline, pingHexPos, Quaternion.identity);
	}
	
	public void CreatePing( int i_hexIndex )
	{
		Debug.Log ("Creating Ping");
		Vector3 pingPos = HexGrid.Manager.GetCoordHex( i_hexIndex, 0.02f);
		Transform pingAnimation = (Transform) Instantiate(pingCreatedIndicator, pingPos, Quaternion.identity);
	}
	
	public void ShowProbablePing( int i_hexIndex )
	{
		if( prevPingHexIndex != i_hexIndex)
		{
			if( GameObject.Find("ProbableFloorPing") != null )
			{
				Destroy( GameObject.Find("ProbableFloorPing") );
			}
			Vector3 pingHexPos = HexGrid.Manager.GetCoordHex( i_hexIndex, 0.02f);
			Transform probablePing = (Transform) Instantiate(probablePingIndicator, pingHexPos, Quaternion.identity);
			probablePing.name = "ProbableFloorPing";
			prevPingHexIndex = i_hexIndex;
		}
	}
	
	/*
	public void AddNewLink(int i_index, bool i_centered, int i_armA, int i_armB)
	{
		//Debug.Log ("Adding new Link to Floor " + i_index);
		int lineA=GetLineIndex( i_index, (byte) i_armA);
		int lineB;
		
		if ( i_centered )
		{
			lineB=GetLineIndex( i_index, (byte) i_armB);
		}
		else
		{
			int indexA = HexGrid.Manager.GetIndex( i_index, (byte)i_armA );
			lineB=GetLineIndex( indexA, (byte) i_armB);
		}
		
		SetLinkState( lineA, LinkState.CONNECTED );
		SetLinkState( lineB, LinkState.CONNECTED );
		UpdateFloorConnections();
	}*/
	
	
	public int GetLineIndex ( int i_index, byte direction)
	{
		int lineIndex=0;
		if ( i_index%2==0)
		{
			lineIndex = i_index*10 + direction;
		}
		else
		{
			int newIndex = HexGrid.Manager.GetIndex( i_index, direction);
			if ( direction == 0)
				lineIndex = newIndex*10;
			else if ( direction == 1)
				lineIndex = newIndex*10 + 2;
			else 
				lineIndex = newIndex*10 + 1;
		}
		
		// TODO Check to make sure this is a valid line (not out of bounds)
		
		return lineIndex;
	}
	
	
	public void UpdateFloorConnections()
	{
		Debug.Log ("Updating Floor Connections");
		// Use connection manager data to update floor.
		foreach ( KeyValuePair<int, LineEffect> l in GridLines)
		{
			if ( HexGrid.Manager.IsLineJammed( l.Key ) )
			{
				SetLinkState(l.Key, LinkState.JAMMED);
			}
			else
			{
				if ( ConnectionManager.Manager.IsLinePowered( l.Key ) )
				{
					Debug.Log ("Setting Line: " + l.Key + " to " + " Powered");
					SetLinkState(l.Key, LinkState.POWERED);
				}
				else if ( ConnectionManager.Manager.IsLineConnected( l.Key ) )
				{
					SetLinkState(l.Key, LinkState.CONNECTED);
					
				}
				//else if ( HexGrid.Manager.IsAvailable ( l.Key/10 ) )
				//else if ( HexGrid.Manager.IsAvailable ( l.Key ) )
				else if ( HexGrid.Manager.IsLineAvailable ( l.Key ) )
				{
					Debug.Log ("Setting Line: " + l.Key + " to " + " Available");
					SetLinkState(l.Key, LinkState.AVAILABLE);
				}
				else
				{
					SetLinkState(l.Key, LinkState.UNAVAILABLE);
				}
			}

			
			/*
			if ( ConnectionManager.Manager.IsConnected((l.Key/10)) && l.Value._state == LinkState.CONNECTED)
				SetLinkState(l.Key, LinkState.POWERED);
			else if ( !ConnectionManager.Manager.IsConnected((l.Key/10)) && l.Value._state == LinkState.POWERED)
				SetLinkState(l.Key, LinkState.CONNECTED);
				*/
		}
		
	}
	
	
	public void AlterLine( int i_line, bool i_addLine)
	{
		//Debug.Log ("Altered Line:" + i_line);
		if ( i_addLine )
		{
			SetLinkState( i_line, LinkState.CONNECTED);
		}
		else
		{
			SetLinkState( i_line, LinkState.AVAILABLE);
		}
		UpdateFloorConnections();
	}
}
