using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Vectrosity;

public class Intersect
{
	public bool	dead;
	public bool active; // is this intersect accesible to the hacker
	public bool pivot; // Is there a pivot here
	public bool node; // Is there a node here
	
	public Intersect (bool i_active)
	{
		active = i_active;
		pivot = false;
		node = false;
		dead = false;
	}
	
	public void SetActive( bool i_active) {
		active = i_active;}
	
	public void SetPivot( bool i_pivot) {
		pivot = i_pivot;}
	
	public void SetNode( bool i_node) {
		node = i_node;}

	public void SetDead( bool i_dead) {
		dead = i_dead;}
	
}

public class GridLine
{
	public bool active;
	public bool powered;
	public int lineIndex;
	public VectorLine vLine;

	public GridLine (bool i_active, bool i_powered, int i_index, VectorLine i_line)
	{
		active = i_active;
		powered = i_powered;
		lineIndex = i_index;
		vLine = i_line;
	}

}


public class HexGrid : MonoBehaviour {

	public enum LineState
	{
		UNAVAILABLE,
		AVAILABLE,
		CONNECTED,
		POWERED,
		JAMMED
	};

	private static HexGrid _instance;
	public static float GRID_W = 0.5f;			// Conceptually the Hex graph is built on a rectangular grid where each rectangle has a width of (GRID_W*hexSize)
	public static float GRID_H = 0.866f;		// and a height of (GRID_H*hexSize).  These are static constants that should not be changed.  They are mathematically 
										// pre-set to create perfect hexagons.  Changing these values will distort the aspect ration of the hexagon grid.
	string 			_levelName;
	public float 	hexSize;			// The length of one side of the hexagons in world units	
	
	public int 		gridSize;			// These number of intersections or index points on the grid. In order to have a nice looking
										// grid this number must be set to: (n * ( rowSize * 2 )

	public int 		rowSize;			// The number of intersections or index points in a single row.  Needs to be an odd number.
	
	public float 	gridZ;				// The Z depth of the base grid in world space.
			
	public float 	offsetX;			// The Grid begins in the upper left corner at 0,0, and is drawn down and to the right.
	public float 	offsetY;			// The offset re-positions the whole graph, along with its intersections and connection lines.
	
	public float 	lineWidth;			// Sets the width of the lines that make up the graph base.
	
	public float 	horzClickRange;		// Click ranges refer to the distance limit that will qualify for a click on a grid line.
	public float 	vertClickRange;		// acceptable ranges are from 0.0 - 0.5.  0.5 means that any click that is closer than halfway
										// between the suggested gridline and the next one over will be acceptable. Basically setting
										// these to 0.5 means that any click will result in finding an acceptable closest gridline match.
										// reducing these to 0.0 will result in only a perfectly exact intersection click being accepted.
	
	public float 	transmitterResetTime;	// The number of second it takes for a transmitter to reset.
	//enum 			intersectType		{inactive, intersect, pivot, node};
	
	public int radiusBoostForTransmitter; //increase in radius on transmitter boost
	
	Dictionary<int, Intersect> intersects;	// An indexed dicitonary of all the intersects on the grid.
	//List<Transform> intersectionPrefabs;

	Dictionary<int, GridLine> gridLines; // An indexed dictionary of all the lines connectin intersects on the hex grid.
	
	ArrayList 		baseLines;			// This holds all of the VectorLine objects that make up the base Hex Grid.
	ArrayList		basePoints;			// This holds all of the VectorPoints objects that display the active or inactive intercest points.
	Dictionary<int, Transmitter>	transmitters; // All of the transmitter that have been placed in the game.
	Dictionary<int, Transmitter>	jammers; // All of the jammers in the level.
	
	public Dictionary<int, Transform>      scramblers;
	
	PivotManager	pivotManager;		// The pivot manager instance
	
	Vector3 		mouseClickOrigin;
	bool 			adjustEngage;
	int 			currentSector;
	bool			creatingNewPivot;
	public GameObject		transmitter;			// The newly created transmitter.
	private bool 	gridChanged;
	
	// Materials
	public Material testMaterial;
	public Material accesibleMaterial;
	public Material inaccesibleMaterial;
	
	//Prefab
	private Transform pingMarker;
	private Transform gridIntersection;
	private List<Transform> PingMarkers;
	private HackerActions myActionsScript;
	
	//------------------------------------------------------------------------------------------------------------------------------------------
	
	public static HexGrid Manager
	{
		get
		{
			if(_instance == null)
			{
				_instance = GameObject.Find ("HexGrid").GetComponent<HexGrid>();			
			}
			return _instance;
		}
	}

	
	// Use this for initialization
	void Start () {
	}
	
	
	public void LoadGraphFromGameData( GraphData i_graphData)
	{
		//Debug.Log ("Loading Hex Grid");
		GlobalData[] globalData = i_graphData.Globals;
		_levelName = globalData[0].LevelName;
		rowSize = globalData[0].LevelWidth;
		gridSize = ((globalData[0].LevelHeight+1)*2) * rowSize;
		gridChanged = true;		
		
		//Debug.Log ( "rowSize = " + rowSize + " : GridSize = " + gridSize);
		adjustEngage = false;
		currentSector = 0;
		creatingNewPivot = false;
		baseLines = new ArrayList();
		gridLines = new Dictionary<int, GridLine>();
		basePoints = new ArrayList();
		transmitters = new Dictionary<int, Transmitter>();
		jammers = new Dictionary<int, Transmitter>();
		intersects = new Dictionary<int, Intersect>();
		scramblers=new Dictionary<int, Transform>();
		
		HackerManager.Manager.HackerClearance = 1;
		InitializeIntersects(i_graphData);
		FillBaseLines();	 
		CreateIntersectionPrefabs();
		SetJammers(i_graphData);
		SetBonuses(i_graphData);
		SetTransmitters(i_graphData);
		//AddHotSpot(22);
		pivotManager = (PivotManager) gameObject.GetComponent("PivotManager");
		transmitterResetTime = 2.0f;
		radiusBoostForTransmitter=3;
		
		CreatePingMarkers();
		//Debug.Log ( "*** LOADED HEX GRID DATA ***");
	}
	
	
	// Update is called once per frame
	void Update () {
		if ( gridChanged )
		{
			//RefreshLines();
			//DrawBaseGrid();
		}
	}
	
	
	/// -----------------------------------------------------------------------------
	/// FILL BASE LINES
	/// <summary>Fills the array of Hex Grid Base Lines</summary>
	/// (This shoud only run once on start.  Existing lines can be moved and
	/// redrawn in update but should not be re-created.)
	/// Params : none
	/// Return : none
	/// -----------------------------------------------------------------------------
	void FillBaseLines()
	{
		//var points = new VectorPoints("Points", linePoints, lineMaterial, 2.0);
		for ( int i = 0 ; i < gridSize ; i++ )
		{
			// Search through every other index point, Only evens
			if ( i%2 == 0 )
			{
				if ( IsInBounds(i, i-rowSize) )
				{
					Vector3[] tempPoints1 = { GetCoord(i), GetCoord(i-rowSize)};
					VectorLine nLine = new VectorLine(("line_" + (i*10+1)),tempPoints1, testMaterial, lineWidth, LineType.Continuous);
					//baseLines.Add( new VectorLine(("line_" + (i*10+1)),tempPoints1, testMaterial, lineWidth, LineType.Continuous) );
					gridLines.Add( (i*10+1), new GridLine(false, false, (i*10+1), nLine) );
				}
				if ( IsInBounds(i, i+rowSize) )
				{
					Vector3[] tempPoints2 = { GetCoord(i), GetCoord(i+rowSize)};
					VectorLine nLine = new VectorLine(("line_" + (i*10+2)),tempPoints2, testMaterial, lineWidth, LineType.Continuous);
					//baseLines.Add( new VectorLine(("line_" + (i*10+2)),tempPoints2, testMaterial, lineWidth, LineType.Continuous) );
					gridLines.Add( (i*10+2), new GridLine(false, false, (i*10+2), nLine) );
				}
				if ( IsInBounds(i, i-1) )
				{
					Vector3[] tempPoints0 = { GetCoord(i), GetCoord(i-1)};
					VectorLine nLine = new VectorLine(("line_" + (i*10)),tempPoints0, testMaterial, lineWidth, LineType.Continuous);
					//baseLines.Add( new VectorLine(("line_" + (i*10)),tempPoints0, testMaterial, lineWidth, LineType.Continuous) );
					gridLines.Add( (i*10), new GridLine(false, false, (i*10), nLine) );
				}
			}

		
			/*
			if ( i > rowSize ) // Dont draw first row
			{
				Vector3[] tempPoints = { GetCoord(i), GetCoord(i-rowSize), GetCoord(i-rowSize+1), GetCoord(i+1)};
				if ( (i%rowSize) != rowSize-1)
				{
					baseLines.Add( new VectorLine(("line_" + i),tempPoints, testMaterial, lineWidth, LineType.Continuous) );
				}
				else // special case for last intersection in a row. ( omit horz line)
				{
					baseLines.Add( new VectorLine(("line_" + i),tempPoints, testMaterial, lineWidth, LineType.Discrete) );
				}

			}
			
			if ( ((i+2)/rowSize) == (gridSize/rowSize-1) ) // Special case for last row to finish off bottom
			{
				Vector3[] tempPoints = { GetCoord(i+1), GetCoord(i+2)};
				baseLines.Add( new VectorLine(("line_" + i),tempPoints, testMaterial, lineWidth) );
			}
			*/
		}
		
		// Create Points
		for ( int i = 0 ; i < gridSize ; i++ )
		{
			Vector3[] tempPoint = { GetCoord(i)};
			basePoints.Add( new VectorPoints(("Point_" + i), tempPoint, inaccesibleMaterial, 3.0f) );
		}
		DrawBaseGrid();
		RefreshLines();
		gridChanged = true;
	}
	
	public void RefreshLines()
	{
		// Set the active state for each grid line.
		foreach ( KeyValuePair<int, GridLine> line in gridLines )
		{
			int basePoint = line.Key/10;
			int otherPoint = GetIndex( (line.Key/10), (byte)(line.Key%10) );
			if ( intersects[basePoint].active && intersects[otherPoint].active )
			{
				line.Value.active = true;
				line.Value.vLine.vectorObject.renderer.enabled = true;
			}
			else
			{
				line.Value.active = false;
				line.Value.vLine.vectorObject.renderer.enabled = false;
			}
		}
	}

	void CreateIntersectionPrefabs()
	{
		/*
		gridIntersection = Resources.Load("Prefabs/Hacker/GridIntersection", typeof(Transform)) as Transform;
		intersectionPrefabs =  new List<Transform>();
		for( int i = 0 ; i < gridSize ; i++ )
		{
			Transform prefab =  (Transform) Instantiate( gridIntersection, GetCoord( i, 60.0f), Quaternion.identity ) as Transform;
			prefab.renderer.enabled = false;
			intersectionPrefabs.Add( prefab );

		}*/
	}
	
	/// -----------------------------------------------------------------------------
	/// INITIALIZE INTERSECTS
	/// <summary>Adds appropriate number of default intersects to the intersects Dictionary</summary>
	/// Params : none
	/// Return : none
	/// -----------------------------------------------------------------------------
	void InitializeIntersects(GraphData i_graphData)
	{
		for( int i = 0 ; i < gridSize ; i++ )
		{
			intersects.Add( i, new Intersect( false ));
		}

		// Tag Deadzone Indecies
		DeadZoneData[] deadZoneData = i_graphData.DeadZones;
		foreach(DeadZoneData deadZones in deadZoneData)
		{
			intersects[deadZones.deadIndex].SetDead(true);
		}
		RefreshLines();
		gridChanged = true;
	}
	
	public void SetIntersect(int i_index, bool i_active, bool i_pivot)
	{
		if ( !intersects[i_index].dead )
		{
			intersects[i_index].pivot = i_pivot;
			intersects[i_index].active = i_active;
		}
	}
	
	
	/// -----------------------------------------------------------------------------
	/// GET COORDS
	/// <summary>Calculates the x and y coordinates of an intersection index</summary>
	/// Params : (int) The index number of the hex grid intersection.
	/// Return : Vector3 representing the world coordinates where that index is located.
	/// -----------------------------------------------------------------------------
	public Vector3 GetCoord( int i)
	{
		int col = (int)(i%rowSize);
		int row = (int)(i/rowSize);
		
		// Calculate x coordinate
		float x = col*3;
		x += (row%2 == 0)? (( col%2 == 0 )? 0 : 1) : ( ( col%2 == 0 )? 1 : 0 );
		x *= -(hexSize * GRID_W);
		x += offsetX;
		
		// Calculate y coordinate
		float y = (row*hexSize*GRID_H);
		y += offsetY;
		
		return new Vector3 (x,gridZ,y);
	}
	
	
	public Vector3 GetCoord( int i, float Yval)
	{
		int col = (int)(i%rowSize);
		int row = (int)(i/rowSize);
		
		// Calculate x coordinate
		float x = col*3;
		x += (row%2 == 0)? (( col%2 == 0 )? 0 : 1) : ( ( col%2 == 0 )? 1 : 0 );
		x *= -(hexSize * GRID_W);
		x += offsetX;
		
		// Calculate y coordinate
		float y = (row*hexSize*GRID_H);
		y += offsetY;
		
		return new Vector3 (x,Yval,y);
	}
	
	// Returns the world coordinates of the center of a hex
	public Vector3 GetCoordHex( int i, float Yval=0)
	{
		int col = (int)(i%rowSize);
		int row = (int)(i/rowSize);
		
		// Calculate x coordinate
		float x = col*3;
		x += (row%2 == 0)? (( col%2 == 0 )? 0 : 1) : ( ( col%2 == 0 )? 1 : 0 );
		x *= -(hexSize * GRID_W);
		x += offsetX;
		x -= hexSize;
		
		// Calculate y coordinate
		float y = (row*hexSize*GRID_H);
		y += offsetY;
		
		return new Vector3 (x,Yval,y);
	}
	
	
	public List<int> GetHexIntersects(int i_hexIndex)
	{
		List<int> list = new List<int>();
		
		if ( i_hexIndex <= rowSize || i_hexIndex >= (gridSize-rowSize))
			return list;
		if ( i_hexIndex%2 == 1 )
			return list;
		
		list.Add(i_hexIndex);
		list.Add(i_hexIndex - rowSize);
		list.Add(i_hexIndex - rowSize + 1);
		list.Add(i_hexIndex + 1);
		list.Add(i_hexIndex + rowSize);
		list.Add( i_hexIndex + rowSize + 1);
		
		return list;
	}
	
	
	//Returns a list of the six hexes that surround the given hex
	public List<int> GetSurroundingHexes(int i_hexIndex)
	{
		List<int> list = new List<int>();
		
		if ( i_hexIndex <= rowSize || i_hexIndex >= (gridSize-rowSize))
			return list;
		if ( i_hexIndex%2 == 1 )
			return list;
		
		list.Add(i_hexIndex - rowSize*2);
		list.Add(i_hexIndex - rowSize + 1);
		list.Add( i_hexIndex + rowSize + 1);
		list.Add(i_hexIndex + rowSize*2);
		list.Add(i_hexIndex - 1 + rowSize);
		list.Add(i_hexIndex - 1 - rowSize);
		return list;
	}
	
	public void AddScrambler(int i_index)
	{
		Vector3 transCenter = GetCoord(i_index);
		transCenter.x = transCenter.x - hexSize;
		Transform newScrambler=(Transform) Instantiate(ThiefGrid.Manager.ghostScrambler, transCenter, Quaternion.identity);
	
		scramblers.Add(i_index,newScrambler);
		
	}
	
	public void RemoveScrambler(int i_index)
	{
	
		//remove scrambler from list
		Transform tempScrambler=null;
		scramblers.TryGetValue(i_index, out tempScrambler);
		if(tempScrambler)
		Destroy(tempScrambler.gameObject);
		scramblers.Remove(i_index);
			
	}
	
	private void SetTransmitters(GraphData i_graphData)
	{
		TransmitterData[] transmitterDatas = i_graphData.Transmitters;
		foreach(TransmitterData trans in transmitterDatas)
		{
			AddHotSpot(trans.HexIndex, trans.Range, bool.Parse(trans.Visible), true);
		}
	}
	
	
	public bool AddHotSpot(int i_index, int i_range=5, bool i_isVisible = true, bool i_isInit = false)
	{
		// Make sure ther is not already a transmitter at i_index
		if ( transmitters.ContainsKey( i_index ) )
			return false;
		
		// Create and store reference to new hot spot
		//Debug.Log ("ADDING TRANSMITTER");
		Transmitter tempHotSpot = new Transmitter(i_index, this, false, i_range);
		transmitters.Add(i_index, tempHotSpot);
		Vector3 transCenter = GetCoord(i_index);
		transCenter.x = transCenter.x - hexSize;
		GameObject newTransmitter = (GameObject) Instantiate(transmitter, transCenter, Quaternion.identity);
		PowerPod_Animations script = newTransmitter.GetComponent<PowerPod_Animations>();
		script.Set();
		tempHotSpot.transmitter = newTransmitter;

		//if it's not visible, don't show the transmitter on hacker's side
		if(i_isVisible == false)
		{
			//newTransmitter.renderer.enabled = false;
			//PowerPod_Animations script = newTransmitter.GetComponent<PowerPod_Animations>();
			script.SetVisible( false );
		}
		else if(i_isVisible == true && i_isInit == true)
		{
			// if it's the preset transmitter and it's visible, initialize a transmitter prefab on the ground
			//Transform prefab = Resources.Load("Prefabs/Theif/Transmitter_Prefab", typeof(Transform)) as Transform;
			Instantiate(ThiefGrid.Manager.activeTransmitter, GetCoordHex( i_index, 0.0f ), Quaternion.identity);
		}
		
		// A Transmitter was placed [SOUND TAG] [Transmitter_Laid]
		if(GameManager.Manager.PlayerType == 1)
			soundMan.soundMgr.playOneShotOnSource(null,"Transmitter_Laid",GameManager.Manager.PlayerType,GameManager.Manager.PlayerType);
		
		// [ SOUND TAG ] [Transmitter_Laid_Hacker]
		if(GameManager.Manager.PlayerType == 2)
			soundMan.soundMgr.playOneShotOnSource(null,"Transmitter_Laid_Hacker",GameManager.Manager.PlayerType,GameManager.Manager.PlayerType);
		
		// Activate affected Hot Spot points
		ActivateHotSpot(tempHotSpot);
		RefreshLines();
		gridChanged = true;
		
		return true;
	}
	
	
	public bool RemoveHotSpot(int i_index)
	{
		// Check that there is a hotSpot located at the requested index. If not return false.
		if ( !transmitters.ContainsKey( i_index ) )
			return false;
			
		// Remove transmitter and deactivate all points on grid
		List<int> affectedLines = GetTransmitter( i_index ).GetAffectedLines();
		Destroy( transmitters[i_index].transmitter );
		transmitters.Remove(i_index);
		for ( int i=0; i<intersects.Count ; i++ )
		{
			intersects[i].SetActive(false);
			//intersectionPrefabs[i].renderer.enabled = false;
		}	
			
		// Reactivate points within range of all remaining indecies.
		foreach ( KeyValuePair<int, Transmitter> t in transmitters )
		{
			ActivateHotSpot ( t.Value );
		}
		

		//List<int> affectedLines = GetTransmitter( i_index ).GetAffectedLines();
		for ( int i=0 ; i<affectedLines.Count ; i++ )
		{
			if ( PivotManager.Manager.PivotExists(affectedLines[i]) )
			{
				Pivot thisPivot = PivotManager.Manager.GetPivot( affectedLines[i] );
				int pointA = thisPivot.Point;
				int pointB = HexGrid.Manager.GetIndex(pointA, thisPivot.Dir );
				if ( !intersects[pointA].active && !intersects[pointB].active )
				{
					PivotManager.Manager.RemovePivot( affectedLines[i], false );
				}
			}
		}
		
		// refresh Connections
		pivotManager.RefreshConnections();
		RefreshLines();
		gridChanged = true;
		
		return true;
	}
	
	
	public bool DeactivateHotSpot(int i_index)
	{
		// Check that there is a hotSpot located at the requested index. If not return false.
		if ( !transmitters.ContainsKey( i_index ) )
			return false;
			
		// Change Material

		
		// Deactivate transmitter
		transmitters[i_index].isActive = false;
		
		// Deactivate all links temporarily
		for ( int i=0; i<intersects.Count ; i++ )
		{
			intersects[i].SetActive(false);
			//intersectionPrefabs[i].renderer.enabled = false;
		}	
			
		// Reactivate points within range of all remaining indecies.
		foreach ( KeyValuePair<int, Transmitter> t in transmitters )
		{
			if ( t.Value.isActive )
			{
				ActivateHotSpot ( t.Value );
			}
		}
		
		// Remove affected pivots and links
		//Debug.Log ("Removing links for hotspot: " + i_index);
		for ( int i = 0; i<intersects.Count ; i++ )
		{
			if ( !intersects[i].active && intersects[i].pivot )
			{
				//Debug.Log ("intersect on: " + i + " passed not active && is pivot");
				if ( pivotManager.RemovePivot(i, false) )
				{
					//Debug.Log (" Passed Removal");
					intersects[i].SetPivot(false);
				}
			}
		}
		
		// refresh Connections
		pivotManager.RefreshConnections();
		RefreshLines();
		gridChanged = true;
		
		return true;
	}
	
	
	public void AddTransmitterBoost()
	{
		//Debug.Log("inside tT");
		// loop to store original radius
		Dictionary<int, int> boostTransmitters;	
		boostTransmitters = new Dictionary<int, int>();
		foreach (KeyValuePair<int, Transmitter> newT in transmitters)
		{
			
			
			   boostTransmitters.Add(newT.Value.index, newT.Value.radius);  // location, range
				
			
		}
				
		// remove transmitters that need boost
		foreach (KeyValuePair<int, int> oldT in boostTransmitters)
		{
		
		        Destroy( transmitters[oldT.Key].transmitter );
				transmitters.Remove(oldT.Key);
		}
		
		
		// Add new transmitters
		foreach (KeyValuePair<int, int> T in boostTransmitters)
		{
		    int newRange=T.Value+radiusBoostForTransmitter;
		    //Debug.Log("add more t");
		    Transmitter tempHotSpot = new Transmitter(T.Key, this, false, newRange );
			transmitters.Add(T.Key, tempHotSpot);
			Vector3 transCenter = GetCoord(T.Key);
			transCenter.x = transCenter.x - hexSize;
			GameObject newTransmitter = (GameObject) Instantiate(transmitter, transCenter, Quaternion.identity);
			PowerPod_Animations script = newTransmitter.GetComponent<PowerPod_Animations>();
			script.Set();	
			tempHotSpot.transmitter = newTransmitter;
				
			// Activate affected Hot Spot points
			ActivateHotSpot(tempHotSpot);
		}
		RefreshLines();
		gridChanged = true;
		
     }
		
	
			
	public void RemoveTransmitterBoost()
	{
		// loop to store original radius
		Dictionary<int, int> boostTransmitters;	
		boostTransmitters = new Dictionary<int, int>();
		foreach (KeyValuePair<int, Transmitter> newT in transmitters)
		{
			
			
		       boostTransmitters.Add(newT.Value.index, newT.Value.radius);  // location, range
				
			
		}
				
		// remove transmitters
		for ( int i=0; i<intersects.Count ; i++ )
		{
			intersects[i].SetActive(false);
			//intersectionPrefabs[i].renderer.enabled = false;
		}	
		foreach (KeyValuePair<int, int> oldT in boostTransmitters)
		{
		
		        Destroy( transmitters[oldT.Key].transmitter );
				transmitters.Remove(oldT.Key);
		}
		
		// Add new transmitters
		foreach (KeyValuePair<int, int> T in boostTransmitters)
		{
		    int newRange=T.Value-radiusBoostForTransmitter;
		    Transmitter tempHotSpot = new Transmitter(T.Key, this, false, newRange );
			transmitters.Add(T.Key, tempHotSpot);
			Vector3 transCenter = GetCoord(T.Key);
			transCenter.x = transCenter.x - hexSize;
			GameObject newTransmitter = (GameObject) Instantiate(transmitter, transCenter, Quaternion.identity);
			PowerPod_Animations script = newTransmitter.GetComponent<PowerPod_Animations>();
			script.Set();	
			tempHotSpot.transmitter = newTransmitter;
				
			// Activate affected Hot Spot points
			ActivateHotSpot(tempHotSpot);
		}
		RefreshLines();
		gridChanged = true;
		
	}
	
	
	public void RemovePivotsInRange( int i_hexIndex )
	{
		//Debug.Log (" Removing Pivots...");
		List<int> pivotsToRemove = new List<int>();
		List<int> intersectsToCheck = new List<int>();
		
		intersectsToCheck = GetHexIntersects(i_hexIndex);
		foreach(int index in intersectsToCheck)
		{		
			List<Pivot> tempList = PivotManager.Manager.GetLinesTouchingIndex(index);
			for ( int i=0 ; i<tempList.Count ; i++ )
			{
				if ( !pivotsToRemove.Contains( tempList[i].lineIndex ))
				{
					pivotsToRemove.Add ( tempList[i].lineIndex );
				}
			}
		}
		
		
		for ( int i=0 ; i<pivotsToRemove.Count ; i++ )
		{
			NetworkManager.Manager.RemovePivot(pivotsToRemove[i], false);
		}
		
		// refresh Connections
		NetworkManager.Manager.RefreshConnections();
		RefreshLines();
		gridChanged = true;
	}
	

	
	public void DeactivateHotSpotsInRange( int hexIndex )
	{
		List<Transmitter> toBeDeactivated = new List<Transmitter>();
		//Determine which hot spots are in range.
		foreach( KeyValuePair<int, Transmitter> t in transmitters )
		{
			if ( t.Value.IsInRange( hexIndex ) )
			{
				toBeDeactivated.Add(t.Value);
			}
		}
			
		
		for ( int i=0; i<toBeDeactivated.Count ; i++ )
		{
			//Debug.Log ("Deactivating HexGrid Hotspot: " + toBeDeactivated[i].index);
			DeactivateHotSpot(toBeDeactivated[i].index);
		}
		RefreshLines();
		gridChanged = true;
	}
	
	public List<Transmitter> GetTransmittersInRange( int i_hexIndex )
	{
		List<Transmitter> inRange = new List<Transmitter>();
		//Determine which hot spots are in range.
		foreach( KeyValuePair<int, Transmitter> t in transmitters )
		{
			if ( t.Value.IsInRange( i_hexIndex ) )
			{
				inRange.Add(t.Value);
			}
		}
		
		return inRange;
	}
	// For TestingMode 
	public void AddRange()
	{
		foreach ( KeyValuePair<int, Transmitter> t in transmitters)
		{
			t.Value.TransmitterAddRadius();

		}		
	}
	
	private void RefreshTransmitters()
	{
		foreach ( KeyValuePair<int, Transmitter> t in transmitters)
		{
			List<int> affected = t.Value.GetAffected();
			//Debug.Log ("Refersh Transmitter at: " + t.Value.index);
			for ( int i = 0 ; i<affected.Count ; i++)
			{
				if ( !IsPointJammed(affected[i]) )
					ActivatePoint( affected[i] );
			}
		}

	}
	
	
	private void ActivateHotSpot(Transmitter i_hotSpot)
	{
		List<int> affected = i_hotSpot.GetAffected();
		for ( int i = 0 ; i<affected.Count ; i++)
		{
			if ( !IsPointJammed(affected[i]) )
				ActivatePoint( affected[i] );
		}
	}
	
	
	private void ActivatePoint(int i_index)
	{
		if ( i_index > 0 && i_index < gridSize )
		{
			if ( !intersects[i_index].active )
			{
				intersects[i_index].SetActive(true);
				//if( !intersects[i_index].node && !intersects[i_index].pivot && !intersects[i_index].dead)
					//intersectionPrefabs[i_index].renderer.enabled = true;
			}
		}
	}
	
	
	public bool IsPointJammed( int i_index )
	{
		foreach( KeyValuePair<int, Transmitter> j in jammers)
		{
			if ( j.Value.isActive && j.Value.ContainsPoint( i_index ) )
				return true;
		}
		return false;
	}
	
	public bool IsLineJammed( int i_lineIndex )
	{
		foreach( KeyValuePair<int, Transmitter> j in jammers)
		{
			if ( j.Value.isActive )
			{
				if ( j.Value.ContainsLine( i_lineIndex ) )
					return true;
			}
		}
		return false;
	}
	
	public bool IsLineAvailable( int i_lineIndex )
	{
		if (gridLines.ContainsKey( i_lineIndex ))
		{
			if ( gridLines[i_lineIndex].active )
				return true;
		}

		return false;
	}


	public Transmitter GetTransmitter( int i_index )
	{
		if ( transmitters.ContainsKey( i_index ) )
		{
			return transmitters[ i_index ];
		}
		else
		{
			return null;
		}
	}
	
	
	public Transmitter GetJammer( int i_index )
	{
		if ( jammers.ContainsKey( i_index ) )
		{
			
			
			return jammers[ i_index ];
		}
		else
		{
			return null;
		}
	}
	
	
	public bool OffEdge( int questionIndex, int baseIndex )
	{
		if ( questionIndex<0 || questionIndex> gridSize)
		{
			return true;
		}
		else
		{
			int questionRow = questionIndex%rowSize;
			int baseRow = baseIndex%rowSize;
			return ((Math.Abs(questionRow-baseRow))>1);
		}
	}
	
	//added -7/12/2013 max
	public void SetNode(int i_index)
	{
		intersects[i_index].SetNode(true);
	}
	
	
	public void SetIntersectPivot(int i_index, bool i_pivot)
	{
		intersects[i_index].SetPivot(i_pivot);
	}
	
	
	private void RefreshIntersectPoints()
	{
		for ( int i = 0 ; i < basePoints.Count ; i++ )
		{
			if ( !intersects[i].active && ((VectorPoints) basePoints[i]).material != inaccesibleMaterial)
			{
				//intersectionPrefabs[i].renderer.enabled = false;
			}
			else if ( intersects[i].active && ((VectorPoints) basePoints[i]).material != accesibleMaterial)
			{
				//if( !intersects[i].node && !intersects[i].pivot )
				//intersectionPrefabs[i].renderer.enabled = true;
			}
		}
	}
	
	
	/// -----------------------------------------------------------------------------
	/// DRAW BASE GRID
	/// <summary>Draws all of the lines that make up the base grid in 3D space</summary>
	/// Params : none
	/// Return : none
	/// -----------------------------------------------------------------------------
	void DrawBaseGrid()
	{
		if ( GameManager.Manager.PlayerType == 2 )// Only draw the grid for the hacker
		{
			foreach ( KeyValuePair<int, GridLine> l in gridLines)
			{	
				l.Value.vLine.Draw3D();
			}
		}

		gridChanged = false;
	}
	
	public int CheckIfIndexIsOnTracer(int i_pivotIndex)
	{
		if(OverrideManager.Manager.IsInOverride() && (i_pivotIndex != -1))
		{
			foreach(GameObject tracer in SecurityManager.Manager.GetLiveTracers())
			{
				Tracer tracerScript = tracer.GetComponent<Tracer>();
				foreach(int index in GetHexIntersects(tracerScript.getHexIndex()))
				{
					if(index == i_pivotIndex)
					{
						return -1;
					}
				}
			}
		}
		
		return i_pivotIndex;
	}
	
	/// -----------------------------------------------------------------------------
	/// GET INDEX
	/// <summary>Given a position in WORLD COORDINATES, calculates and returns the corresponding grid index</summary>
	/// Params : (Vector3) representing coordinates in world space.  Only the x and z values are relevant.
	/// Return : an int representing the hex grid index number that is within some range of the parameter
	/// 		coordinates.  If there is no grid index close enough, the function returns -1.
	/// -----------------------------------------------------------------------------
	public int GetIndex(Vector3 i_pos)
	{
		#region Legacy code Chris 1/13/14
		/*
		// Calculate the horz and vertical position of the click in grid coordinates.
		float posX = i_pos.x-offsetX;
		float posY = i_pos.z-offsetY;		
		posX = -(posX/(hexSize * GRID_W));
		posY = posY/(hexSize * GRID_H);
		
		// Get closest row and collumn of click, verify that click is within range.
		int row = (int) Mathf.Round(posY);
		int col = (int) Mathf.Round (posX);
		
		
		// Determine if click row/col corresponds to a hex index
		int index = 0;
		if ( row%2 == 0 )
		{
			if ( col%6 == 0) {
				index = (col/6) * 2 + (row * rowSize);
			}
			else if ( (col-4)%6 == 0 ) {
				index = (col/6) * 2 + 1 + (row * rowSize);
			}
			else { // Miss Click
				index = -1;
			}
		}
		else 
		{
			if ( (col-1)%6 == 0) {
				index = (col/6) * 2 + (row * rowSize);
			}
			else if ( (col-3)%6 == 0 ) {
				index = (col/6) * 2 + 1 + (row * rowSize);
			}
			else { // Miss Click
				index = -1;
			}
		}
		*/
		#endregion

		int index = 0;
		//Shift Coordinates to 0,0 space.
		float posX2 = i_pos.x-offsetX;
		float posY2 = i_pos.z-offsetY;	

		//calculate which Box row and column you clicked in
		// Boxes are .5 wide and .866 tall.
		int clickCol = (int) Mathf.Ceil(-posX2/(GRID_W));
		int clickRow = (int) Mathf.Round(posY2/(GRID_H));

		//Debug.Log ("Row: " + clickRow + "  - Col: " + clickCol);

		if ( clickRow%2 == 0 ) // even row
		{
			if ((clickCol)%6==0 || (clickCol-1)%6==0) // Inside Left Facing index
			{
				index = (clickCol/6) * 2 + (clickRow * rowSize);
				//Debug.Log ("LEFT INDEX");
			}
			else if ((clickCol-4)%6==0 || (clickCol-5)%6==0) // Inside Right facing index
			{
				index = (clickCol/6) * 2 + 1 + (clickRow * rowSize);
				//Debug.Log ("RIGHT INDEX");
			}
			else // Hex Center
			{
				index = -1;
				//Debug.Log ("PING");
			}
		}
		else
		{
			if ((clickCol-1)%6==0 || (clickCol-2)%6==0) // Inside Right Facing index
			{
				index = (clickCol/6) * 2 + (clickRow * rowSize);
			}
			else if ((clickCol-3)%6==0 || (clickCol-4)%6==0) // Inside Left Facing index
			{
				index = (clickCol/6) * 2 + 1 + (clickRow * rowSize);
			}
			else// Hex Center
			{
				index = -1;
			}
		}

		return index;
	}
	
	/// -----------------------------------------------------------------------------
	/// GET INDEX
	/// <summary>Given a current index and a direction to move, calculates the new index</summary>
	/// Params : (int) starting index, (byte) direction to move - 0=horz, ,1= up, 2=down
	/// Return : an int representing the new index. or -1 if the move was illeagal.
	/// -----------------------------------------------------------------------------
	public int GetIndex( int i_startIndex, byte i_direction)
	{
		int newIndex = -1;
		if ( i_direction == 0 )
		{
			newIndex = (i_startIndex%2==0)? i_startIndex-1 : i_startIndex+1;
		}
		else if ( i_direction == 1 )
		{
			newIndex = i_startIndex - rowSize;
		}
		else if ( i_direction == 2 )
		{
			newIndex = i_startIndex + rowSize;
		}

		if ( newIndex == -1 )
			return -1;
		else
			return ( IsInBounds(i_startIndex, newIndex) )? newIndex : -1 ;	
	}
	
	
	public int GetIndex( Vector3 i_pos, float i_radius)
	{
		List<int> closeIndecies = new List<int>();
		for ( int i=0 ; i<intersects.Count ; i++ )
		{
			float tempDist = Vector2.Distance( new Vector2( GetCoord(i).x, GetCoord(i).z), new Vector2( i_pos.x, i_pos.z) );
			if ( tempDist < i_radius )
			{
				//Debug.Log ("Intersection Clicked: " + i);
				return i;
			}
		}
		return -1;
	}
	
	
	private bool IsInBounds( int i_fromIndex, int i_toIndex )
	{
		int indexA = i_fromIndex;
		int indexB = i_toIndex;			
		
		int indexA_Row = indexA%rowSize;
		int indexB_Row = indexB%rowSize;
			
		if ( !intersects.ContainsKey( indexB ) ) // False if to index is not in the intersects list
		    return false;
		if ( indexB < 0 || indexB > gridSize || (Math.Abs(indexA_Row - indexB_Row)) > 1 )// Must be in valid row or column
			return false;
		else if ( intersects[indexB].dead || intersects[indexA].dead )// Must not be in a dead zone
			return false;
		else
			return true;
	}


	public bool IsLineInBounds(int i_lineIndex )
	{
		int baseIndex = i_lineIndex/10;
		byte direction = (byte) (i_lineIndex%10);
		
		if ( baseIndex < 0 || baseIndex > gridSize )
			return false;
			
		int endPoint = GetIndex(baseIndex, direction);
		if ( endPoint == -1 )
			return false;
		
		if ( baseIndex<HexGrid.Manager.rowSize && direction==1 )
			return false;
		
		return true;
	}
	
	
	public int GetLineIndex( int i_index, byte i_direction)
	{
		if ( i_index%2==0 )
		{
			return (i_index*10+i_direction);
		}
		else
		{
			if ( i_direction == 0 )
			{
				return ((i_index+1)*10);
			}
			else if ( i_direction == 1 )
			{
				return ((i_index-rowSize)*10)+2;
			}
			else if ( i_direction == 2 )
			{
				return ((i_index+rowSize)*10)+1;
			}
		}
		return -1;
	}
	
	public int GetHex(Vector3 i_pos)
	{
		// Calculate the horz and vertical position of the click in grid coordinates.
		float posX = i_pos.x-offsetX;
		float posY = i_pos.z-offsetY;		
		posX = -(posX/(hexSize * GRID_W));
		posY = posY/(hexSize * GRID_H);
		
		// Get closest row and colum of click.
		int row = (int) Mathf.Floor(posY);
		int col = (int) Mathf.Floor(posX/3);
		
		if ( col%2!=0 && row%2==0 )
			row++;
		if ( col%2==0 && row%2!=0 )
			row++;
			
		int hexNumber = (row*rowSize)+col;
		return hexNumber;
		
	}
	
	
	public Intersect GetIntersect( int i_index )
	{
		if ( intersects.ContainsKey( i_index) )
		{
			return intersects[i_index];
		}
		else return null;
	}
	
	
	public bool HasTransmitter( int i_index )
	{
		return transmitters.ContainsKey( i_index );
	}
	

	
	public bool IsAvailable(int i_index)
	{
		Intersect thisIntersect;
		if(intersects.ContainsKey(i_index))
		{
			thisIntersect=intersects[i_index];
		}
		else		
			return false;
		
		if ( thisIntersect.active )
			return true;
		else
			return false;
	}
	
	public bool IsIndexAPivot( int i_index )
	{
		Intersect thisIntersect;
		if(intersects.ContainsKey(i_index))
		{
			thisIntersect=intersects[i_index];
		}
		else
			return false;
		
		if ( thisIntersect.pivot )
			return true;
		else
			return false;
	}
	
	
	public void ResetTransmitter ( int i_hexIndex )
	{
		//Debug.Log ("RESET TRANSMITTER ON HEX GRID"); 
		DeactivateHotSpot( i_hexIndex);
		
		// Start REset Timer.
		Action timerEndAction = delegate(){ActivateHotSpot( transmitters[i_hexIndex] );};
		GenericTimer myGenericTimer = gameObject.AddComponent<GenericTimer>();
		myGenericTimer.Set( transmitterResetTime, false, timerEndAction );
		myGenericTimer.Run();
		
	}
	
	private void SetJammers( GraphData i_graphData)
	{
		JammerData[] jammerDatas = i_graphData.Jammers;
		BasicScoreSystem.Manager.TotalJammers = jammerDatas.Length;
		foreach(JammerData jammer in jammerDatas)
		{
			//Debug.Log ("ADDING JAMMER ON HEX " + jammer.HexIndex + "With Radius: " + jammer.Range);
			Transmitter tempJammer = new Transmitter(jammer.HexIndex, this, true, jammer.Range); // hard coded Temp range for now, take out for final build
			jammers.Add(jammer.HexIndex, tempJammer);
			
			// Create Physical Transmitter
			Transform jammerPrefab = Resources.Load("Prefabs/Theif/Jammer_Prefab", typeof(Transform)) as Transform;
			Vector3 jammerPos = HexGrid.Manager.GetCoordHex( jammer.HexIndex, 0.0f);
			Transform newJammer = (Transform) Instantiate(jammerPrefab, jammerPos, Quaternion.identity);

		}
	}
	
	
	public void DisableJammer( int i_hexIndex)
	{
		//Debug.Log ("HexGrid Disable Jammer");
		if ( !jammers.ContainsKey( i_hexIndex ) )
			return;
		
		//Debug.Log ("HexGrid Disable Jammer fo real");
		Transmitter thisJammer = jammers[i_hexIndex];
		thisJammer.isActive = false;
		
		RefreshTransmitters();
		RefreshLines();
	}


	public void DisableAllJammersTestingMode()
	{
		
		foreach(KeyValuePair<int,Transmitter> j in jammers)
		{
			
			j.Value.isActive= false;

		}
		
	}
	
	private void SetBonuses ( GraphData i_graphData)
	{
	}
	
	
	public void PrintTransmitterList()
	{
		foreach ( KeyValuePair<int, Transmitter> t in transmitters)
		{
			Debug.Log ("Transmitter:" + t.Key);
		}
	}
	
	public int[] GetIndexTargets2(int i_index)
	{
		int[] targets = new int[6];
		// Test first set
		if ( GetIndex(i_index, 0) != -1 )
		{
			targets[0] = GetIndex( (GetIndex(i_index, 0)), 1);
			targets[1] = GetIndex( (GetIndex(i_index, 0)), 2);
		}
		else
		{
			targets[0] = -1;
			targets[1] = -1;
		}
		// Test second set
		if ( GetIndex(i_index, 1) != -1 )
		{
			targets[2] = GetIndex( (GetIndex(i_index, 1)), 0);
			targets[3] = GetIndex( (GetIndex(i_index, 1)), 1);
		}
		else
		{
			targets[2] = -1;
			targets[3] = -1;
		}
		// Test third set
		if ( GetIndex(i_index, 2) != -1 )
		{
			targets[4] = GetIndex( (GetIndex(i_index, 2)), 2);
			targets[5] = GetIndex( (GetIndex(i_index, 2)), 0);
		}
		else
		{
			targets[4] = -1;
			targets[5] = -1;
		}
		
		return targets;
	}
	
	public int[] GetIndexTargets1(int i_index)
	{
		int[] targets = new int[3];
		targets[0] = GetIndex( i_index, 0);
		targets[1] = GetIndex( i_index, 1);
		targets[2] = GetIndex( i_index, 2);

		return targets;
	}
	
	
	// returns an array of possible targets 2 jumps from the given index.
	// If a target is out of bounds of the grid it will return a -1 at that index in the array. 
	public int[] GetIndexTargetsOnTracer(int i_index)
	{
		int[] targets = new int[6];
		targets[0] = CheckIfIndexIsOnTracer(GetIndex( (GetIndex(i_index, 0)), 1));
		targets[1] = CheckIfIndexIsOnTracer(GetIndex( (GetIndex(i_index, 0)), 2));
		targets[2] = CheckIfIndexIsOnTracer(GetIndex( (GetIndex(i_index, 1)), 0));
		targets[3] = CheckIfIndexIsOnTracer(GetIndex( (GetIndex(i_index, 1)), 1));
		targets[4] = CheckIfIndexIsOnTracer(GetIndex( (GetIndex(i_index, 2)), 2));
		targets[5] = CheckIfIndexIsOnTracer(GetIndex( (GetIndex(i_index, 2)), 0));
		
		return targets;
		//TODO Still need to check in bounds
	}
	
	
	public List<int> GetHexLines( int i_hexIndex )
	{
		List<int> lines = new List<int>();
		lines.Add( i_hexIndex*10+1 );
		lines.Add( i_hexIndex*10+2 );
		lines.Add( (i_hexIndex-rowSize+1)*10 );
		lines.Add( (i_hexIndex-rowSize+1)*10 + 2);
		lines.Add( (i_hexIndex+rowSize+1)*10 );
		lines.Add( (i_hexIndex+rowSize+1)*10 + 1);
		
		return lines;
	}
	
	
	public bool IsHexCaptured(int i_hexIndex)
	{
		List<int> myLines = GetHexLines( i_hexIndex );
		for ( int i=0 ; i<myLines.Count ; i++ )
		{
			if ( !ConnectionManager.Manager.IsLinePowered(myLines[i]) )
				return false;
		}
		return true;
	}
	
	public bool IsHexInBounds( int i_hexIndex )
	{
		if ( i_hexIndex%2 != 0 ) // Verify it is an even index
			return false;
		
		if ( i_hexIndex < rowSize) // Verify it is not on the top row
			return false;
		
		if ( i_hexIndex > gridSize - rowSize ) // Verify it is not on the bottom row
			return false;
		
		if ( (i_hexIndex+1)%rowSize == 0 ) // Verify that it is not on right edge.
			return false;
		
		return true;
	}
	
	
	public void DebugMakeAllAvailable()
	{
		for ( int i=0 ; i<intersects.Count ; i++ )
		{
			intersects[i].active = true;
		}
		RefreshIntersectPoints();
	}
	
	private void CreatePingMarkers()
	{
		myActionsScript = gameObject.GetComponent<HackerActions>();
		pingMarker = Resources.Load("Prefabs/Hacker/PingMarker", typeof(Transform)) as Transform;
		
		PingMarkers = new List<Transform>();
		
		for( int i = rowSize + 1; i < gridSize - rowSize ; i+= 2 )
		{
			if( (i+1) % rowSize != 0 )
			{
				Transform tempPingMarker = (Transform) Instantiate( pingMarker, GetCoordHex(i, 55), Quaternion.identity );
				tempPingMarker.renderer.enabled = false;
				PingMarkers.Add(tempPingMarker);
			}
		}
	}
	
	public void EnablePingMarkers()
	{
		foreach(Transform pingMarkerInList in PingMarkers)
		{
			pingMarkerInList.renderer.enabled = true;	
		}
	}
	
	public void DisablePingMarkers()
	{
		foreach(Transform pingMarkerInList in PingMarkers)
		{
			pingMarkerInList.renderer.enabled = false;	
		}
	}

	public void ExposeHexGrid()
	{
		AddHotSpot(1104, 100, false, false);
	}

	public float GetLevelWidth()
	{
		return ((float)((HexGrid.Manager.rowSize-1)/2) * 3 + 0.5f);
	}

	public float GetLevelHeight()
	{
		return (((float)(HexGrid.Manager.gridSize/HexGrid.Manager.rowSize)/2.0f+0.5f)*1.732f);
	}

	public Vector3 GetMapCenter()
	{
		return new Vector3(-(GetLevelWidth() / 2.0f), 54.0f, (GetLevelHeight() / 2.0f)-(GetLevelHeight()/12.0f));
	}
}
