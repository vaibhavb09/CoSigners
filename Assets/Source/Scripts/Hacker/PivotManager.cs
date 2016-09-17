using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Vectrosity;

public class PivotManager : MonoBehaviour 
{
	
	private static PivotManager		m_instance;
	HexGrid 				hexGrid;				// A reference to the main Hex Grid object for this level
	ConnectionManager		connectionManager;
	Dictionary<int, Pivot> 	myPivots;				// A collection of all of the current pivots on the grid
	Queue					animationQueue;			// A queue of all of the pivot animations.
	Dictionary<int, Queue>  animationQueues;		

	// Variables relating to the creation of a New Pivot
	Pivot					newPivot;				// A pivot that is currently being created ( only one can exist at a time)
	int						newPivotIndex;			// The hex index number of the new pivot
	Vector3					newPivotPos;			// A Vector 3 representing the world coordinates of the new pivot
	int						newPivotSector;			// sector the the pivot is currently positioned in.
	bool					newPivotUpper;
	int[]					newPivotTargets;
	byte					newPivotArmA;
	byte 					newPivotArmB;
	int						consecutiveNewLinks;
	Transform[]				newPivotLinkTargets;
	Transform				newLinkTargetBar;
	bool					newPivotInitialCorrect;	// If the new pivot has been adjusted to hacker initial pointer position
	private int 			pivotActive;			// Represents if there is currently a pivotable index on the map.		
	private float 			lineWidth;				// Public variable representing the width of basic connection lines
	private float			connectedLineWidth;
	public float			baseRotationDurration;
	float 					innerSize;				// InnerSize is in HexSize Units
	float 					cancelSize;				// CancelSize is in hexSize Units.
	private float 			temp;
	public	bool			justRotated;
	private int				currentRotation;
	
	//State bools
	bool					creatingNewPivot;		// A state bool: a new pivot is currently being created.
	
	public Material 		Line_New_Material;
	public Material 		Line_Basic_Material;
	public Material			Line_Connected_Material;
	private GenericTimer	doubleClickTimer;
	//----------------------------------------------------------------------------------------------------------------------
	
	#region Properties
	public Dictionary<int, Pivot> PivotList
	{
		get
		{
			return myPivots;
		}
	}
	
	public static PivotManager Manager
	{
		get
		{
			if(m_instance == null)
			{
				m_instance = GameObject.Find ("HexGrid").GetComponent<PivotManager>();			
			}
			return m_instance;
		}
	}
	
	public int ActivePivot
	{
		get{
			return pivotActive;
		}
	}
	
	public int ConsecutiveLinks
	{
		get{
			return consecutiveNewLinks;
		}
	}
	#endregion
	
	#region Constructor
	public PivotManager () 
    { 
        m_instance = this;
    }
	#endregion
	
	
	// Use this for initialization
	void Start () {
	}
	
	public void Load()
	{
		temp =0.1f;
		creatingNewPivot = false;
		newPivotSector = -1;
		newPivotUpper = false;
		innerSize = 1.5f;
		cancelSize = 5.0f;
		consecutiveNewLinks = 0;
		pivotActive = -1;
		justRotated = false;
		currentRotation = 0;
		hexGrid = (HexGrid) gameObject.GetComponent("HexGrid");
		connectionManager = (ConnectionManager) gameObject.GetComponent("ConnectionManager");
		connectionManager._pivotManager = this;
		myPivots = new Dictionary<int, Pivot>();
		animationQueues = new Dictionary<int, Queue>();
		animationQueue = new Queue();

		lineWidth = Mathf.Round(Screen.width/480);
		connectedLineWidth = Mathf.Round(Screen.width/240);

		Transform LinkTarget = Resources.Load("Prefabs/Hacker/Graph/LinkTargetPrefab", typeof(Transform)) as Transform;
		Transform LinkTargetBarPrefab = Resources.Load("Prefabs/Hacker/Graph/LinkTargetBarPrefab", typeof(Transform)) as Transform;
		newPivotLinkTargets = new Transform[12];
		Vector3 startPos = new Vector3(0.0f, 0.0f, 0.0f);
		newLinkTargetBar = (Transform) Instantiate(LinkTargetBarPrefab, startPos, Quaternion.identity);
		
		for ( int i=0 ; i<12 ; i++ )
		{
			Transform tempTarget = (Transform) Instantiate(LinkTarget, startPos, Quaternion.identity);
			newPivotLinkTargets[i] = tempTarget;
			newPivotLinkTargets[i].renderer.enabled = false;
		}

	}
	
	// Update is called once per frame
	void Update () {
		if ( myPivots == null )
			Load ();

		if ( GameManager.Manager.PlayerType == 2 )
			DrawConnections();
	}
	
	
	// Begins the new pivot process.  Displays traget points and 
	// runs detection for finalizing pivot creation.
	public void StartCreateNewLink(int i_index)
	{
		// Link creation was started [SOUND TAG] [Link_Press]
		soundMan.soundMgr.playOneShotOnSource(null,"Link_Press",GameManager.Manager.PlayerType,2);
		
		creatingNewPivot = true;
		newPivotIndex = i_index;
		newPivotPos = hexGrid.GetCoord(newPivotIndex);
		DisplayLinkTargets();
	}
	
	
	public void EnableRotation(int i_index)
	{
		if ( AreLinesTouchingIndex(i_index) )
		{
			HexGrid.Manager.SetIntersectPivot( i_index, true );
			pivotActive = i_index;

			// Start a timer that will expire the active pivot if not actually double clicked.
			doubleClickTimer = gameObject.AddComponent<GenericTimer>();
			Action timerEndAction = delegate(){TimeOutActivePivot();};
			doubleClickTimer.Set( 0.2f, false, timerEndAction );
			doubleClickTimer.Run();
		}
	}
	
	public void TimeOutActivePivot()
	{
		Destroy( doubleClickTimer );
		DisableActivePivot();
	}


	public void DisableActivePivot()
	{
		if ( pivotActive != -1 )
		{
			HexGrid.Manager.SetIntersectPivot( pivotActive, false );
			pivotActive = -1;
		}
	}
	
	
	private void DisplayLinkTargets()
	{
		newPivotTargets = HexGrid.Manager.GetIndexTargets1( newPivotIndex );
		for ( int i=0 ; i<newPivotTargets.Length ; i++ )
		{
			if ( newPivotTargets[i] != -1 && HexGrid.Manager.GetIntersect(newPivotTargets[i]).active ) // Make sure that this is a valid location and that it is an active index
			{
				//Debug.Log ("Link Target at:" + newPivotTargets[i]);
				newPivotLinkTargets[i].transform.position = HexGrid.Manager.GetCoord( newPivotTargets[i], 60);
				newPivotLinkTargets[i].renderer.enabled = true;
			}
		}
		newLinkTargetBar.GetChild(0).renderer.enabled = true;
		newLinkTargetBar.transform.position = HexGrid.Manager.GetCoord(newPivotIndex, 60.0f);
	}
	
	
	private void HideLinkTargets()
	{
		for ( int i=0 ; i<newPivotLinkTargets.Length ; i++ )
		{
			newPivotLinkTargets[i].renderer.enabled = false;
		}
		newLinkTargetBar.GetChild(0).renderer.enabled = false;
	}
	
	
	// This removes the new links pivot and target points and 
	// resets the pivot manager to non creation state.
	public void CancelNewLink()
	{
		consecutiveNewLinks = 0;
		ResetNewPivot();		
	}
	
	
	// Resets all of the new pivot creation parameters.
	private void ResetNewPivot()
	{
		HideLinkTargets();
		creatingNewPivot = false;
		newPivotIndex = 0;
		newPivotTargets = new int[0];
		newPivotArmA = 0;
		newPivotArmB = 0;
		currentRotation = 0;
	}
	
	
	// completes the placement of the link, draws the link lines
	// and resets all of the creation states. This only runs on Hacker side.
	public void FinishCreateNewLink()
	{	
		// A new link was created [SOUND TAG] [Link_Placed]
		soundMan.soundMgr.playOneShotOnSource(null,"Link_Placed",GameManager.Manager.PlayerType,2);
		
		BasicScoreSystem.Manager.LinksPlaced += 1;
		NetworkManager.Manager.CreatePivot(newPivotIndex, false, newPivotArmA, newPivotArmB);
	}
	
	
	// Runs on both Hacker and Thief Side
	public void AddPivot( int i_index, bool i_centered, int i_armA, int i_armB)
	{
		//Debug.Log ("CHRIS CREATE NEW PIVOT!!!");
		byte armA = (byte)i_armA;
		int lineIndex = HexGrid.Manager.GetLineIndex(i_index, armA);
		
		// Check to see if this line already exists.
		bool alreadyExists = false;
		if ( myPivots.ContainsKey( lineIndex ))
		{
			alreadyExists = true;
		}
		else
		{
			// Create and add the VectorLines for this link
			int p1 = i_index;
			int p2 = HexGrid.Manager.GetIndex(i_index, armA);
			Vector3[] tempPoints = { hexGrid.GetCoord(p1), hexGrid.GetCoord(p2)};
			VectorLine line =  new VectorLine(("connect_" + i_index),tempPoints, Line_Basic_Material, lineWidth, LineType.Continuous);		
			Pivot newPivot = new Pivot( lineIndex, line );
			myPivots.Add( lineIndex, newPivot);
			
			connectionManager.SetConnections(newPivot, false);
			connectionManager.RefreshConnected();
			RefreshConnectionMaterials();
			GraphManager.Manager.RefreshGraph();
			BonusManager.Manager.RefreshBonuses();
			OverrideManager.Manager.Refresh();
			InfoNodeManager.Manager.Refresh();
			HackerManager.Manager.RefreshPower();
			HexGrid.Manager.RefreshLines();
		}
		
		int endIndex = HexGrid.Manager.GetIndex(i_index, armA);
		if ( GameManager.Manager.PlayerType == 2 )
		{
			ResetNewPivot();
			// Determine if a new pivot shoudl auto create
			if ( DetectAutoCreateNewLink(endIndex) )//|| alreadyExists)
			{
				consecutiveNewLinks ++;
				StartCreateNewLink(endIndex);
			}
			else
			{
				consecutiveNewLinks = 0;
			}
		}
		
	}
	
	
	// Tests the input index to verify that mouse drag is in one of the link targets.
	// This runs contiually while the hacker is creating new links.
	public void TestLinkTargetDrag( Vector3 i_worldPos )
	{
		// Calculate Direction of touch and indicator sector
		float touchAngle = Vector2.Angle( new Vector2(-1.0f, 0.0f), new Vector2( i_worldPos.x-newPivotPos.x, i_worldPos.z-newPivotPos.z) );
		bool above = (i_worldPos.z < newPivotPos.z);
		bool right = newPivotIndex%2==0;
		int sector = 0;
		if (right)
		 	sector = (touchAngle>120.0f)? 0 : ((above)? 1 : 2);
		else
			sector = (touchAngle<60.0f)? 0 : ((above)? 1 : 2);

		// Determine if direction is valid, must be in bounds and active target index.
		// For displaying the new link target direction bar
		bool valid = false;
		if ( newPivotTargets[sector] == -1 || !HexGrid.Manager.GetIntersect(newPivotTargets[sector]).active)
		{
			newLinkTargetBar.GetChild(0).renderer.enabled = false;
		}
		else
		{
			valid = true;
			newLinkTargetBar.GetChild(0).renderer.enabled = true;
		
			// Rotate Target indicator
			if ( right )
			{
				if ( touchAngle>120.0f )
					newLinkTargetBar.transform.rotation =  Quaternion.Euler(0, 225, 0);
				else if ( above )
					newLinkTargetBar.transform.rotation =  Quaternion.Euler(0, 345, 0);
				else
					newLinkTargetBar.transform.rotation =  Quaternion.Euler(0, 105, 0);
			}
			else
			{
				if ( touchAngle < 60 )
					newLinkTargetBar.transform.rotation =  Quaternion.Euler(0, 45, 0); 
				else if ( above )
					newLinkTargetBar.transform.rotation =  Quaternion.Euler(0, 285, 0); 
				else
					newLinkTargetBar.transform.rotation =  Quaternion.Euler(0, 165, 0); 
			}
		}
		
		// Calculate world distance from new pivot
		float touchDist = Vector2.Distance( new Vector2( i_worldPos.x, i_worldPos.z), new Vector2(newPivotPos.x, newPivotPos.z) );
		
		if ( touchDist >0.8f )
		{
			if ( valid )
			{
				newPivotArmA = (byte) (sector);
				FinishCreateNewLink();
			}
			else
			{
				CancelNewLink();
			}
		}
	}
	
	
	// returns the index of the new link target that is hit. -1 if no target is hit.
	// Targets are labeled as follows: 
	private int LinkTargetHit ( Vector3 i_worldPos )
	{
		int[] targets = HexGrid.Manager.GetIndexTargets2( newPivotIndex );
		for ( int i=0 ; i< targets.Length ; i++ )
		{
			if ( targets[i] == HexGrid.Manager.GetIndex( i_worldPos, 0.6f ) )
			{
				newPivotArmA = (byte) (i/2);
				newPivotArmB = (byte) ((i%3==2)? 0 : (i%3+1));
				return targets[i];
			}
		}
		return -1;
	}
	
	
	// This determines upon finishing the creation of a link, if a new
	// link should auto create branching out from the end of the end of the previous link.
	private bool DetectAutoCreateNewLink(int i_index)
	{
		// Check that you are on a valid index
		if ( i_index == -1 )
			return false;
		
		// Check to see if you are on an avalable index
		if ( !HexGrid.Manager.IsAvailable( i_index ) )
			return false;
			
		return true;
	}
	
	
	// Returns true if the pivot manager is in the process of creatinga new link
	public bool IsCreatingNewLink()
	{
		return creatingNewPivot;
	}
	
	
	// Returns true if 
	public bool PivotExists(int i_lineIndex)
	{
		return myPivots.ContainsKey( i_lineIndex );
		
	}
	
	
	// Removes the pivot and related links at the specified index.
	public bool RemovePivot(int i_index, bool i_refresh)
	{
		// Get the requested Pivot return false if there is none
		Pivot thisPivot = null;
		if ( myPivots.ContainsKey(i_index))
			thisPivot = myPivots[i_index];
		else
			return false;
		
		// Destroy the Vector Line object as well as the Control Point
		VectorLine.Destroy ( ref thisPivot.myConnections );
		myPivots.Remove(thisPivot.lineIndex);
		connectionManager.SetConnections(thisPivot, true);
		
		// If there are animations remaining, destroy them.		
		if ( animationQueues.ContainsKey(thisPivot.lineIndex) )
		{	
			Queue myQueue = animationQueues[thisPivot.lineIndex];
			while ( myQueue.Count > 0)
			{
				PivotAnimation finished = (PivotAnimation) myQueue.Dequeue();
				Destroy( finished );
			}
			animationQueues.Remove(thisPivot.lineIndex);
		}	
		thisPivot = null;
		
		// If requested, Refresh the Connections
		if ( i_refresh )
		{
			connectionManager.RefreshConnected();
			RefreshConnectionMaterials();
			HackerManager.Manager.RefreshPower();
		}
		return true;
	}
	
	
	private bool InBounds( List<Pivot> i_Pivots )
	{		
		for ( int i=0 ; i<i_Pivots.Count ; i++ )
		{
			if ( !HexGrid.Manager.IsLineInBounds(i_Pivots[i].lineIndex) )
				return false;
		}
		
		return true;
	}
		
	
	public void RefreshConnections()
	{
		//Debug.Log ("**** Refreshing Connecitons TEST ****");
		connectionManager.RefreshConnected();
		RefreshConnectionMaterials();
		BonusManager.Manager.RefreshBonuses();
		InfoNodeManager.Manager.Refresh();
		OverrideManager.Manager.Refresh();
		HackerManager.Manager.RefreshPower();
		//HexGrid.Manager.RefreshLines();
	}
	
	
	public Pivot GetPivot ( int i_lineIndex )
	{
		return myPivots[i_lineIndex];
	}
	
	
	/// -----------------------------------------------------------------------------
	/// Rotate Pivot
	/// <summary>Rotates a normal pivot</summary>
	/// Params : (int) The index of teh pivot to be rotated
	/// Return : none
	/// -----------------------------------------------------------------------------
	public void RotatePivot( int i_index, bool i_clear, List<Pivot> i_list=null )
	{
		// This is a temp bandaid for the top row rotation bug.
		if ( i_index < HexGrid.Manager.rowSize )
			return;
		
		// Get lines affected by this rotation
		List<Pivot> lines = new List<Pivot>();
		if ( i_clear )
		{
			lines = GetLinesTouchingIndex(i_index);
			for ( int i=0 ; i<lines.Count ; i++ )
			{
					myPivots.Remove(lines[i].lineIndex);
					connectionManager.SetConnections(lines[i], true);
			}
		}
		else
		{
			lines = i_list;
		}
		
		// Increment the itteration of this rotation (if rotation goes out of bounds then it will keep rotating)
		currentRotation += 1;
		
		// Rotate the lines and add new lines to dictionary	
		for ( int i=0 ; i<lines.Count ; i++ )
		{
			lines[i].RotateLine( i_index );
		}
			
		// Set up and perform rotation animation
		if ( InBounds(lines) || currentRotation == 3)
		{
			if(GameManager.Manager.PlayerType == 2)
				soundMan.soundMgr.playOneShotOnSource(null,"Link_Rotate",GameManager.Manager.PlayerType,GameManager.Manager.PlayerType);
			
			Vector3 center = hexGrid.GetCoord(i_index);
			PivotAnimation newPivotAnimation = gameObject.AddComponent<PivotAnimation>();
			Action timerAction = delegate(){DequeueAnimation(i_index);};		
			newPivotAnimation.Set( lines, center, 120*currentRotation, (0.2f*(currentRotation)), timerAction, 120*currentRotation );
			EnqueueAnimation( newPivotAnimation, i_index );
			
			for ( int i=0 ; i<lines.Count ; i++ )
			{
				if ( !myPivots.ContainsKey(lines[i].lineIndex) )
					myPivots.Add(lines[i].lineIndex, lines[i]);
			}
			
			for ( int i=0 ; i<lines.Count ; i++ )
			{
				connectionManager.SetConnections(lines[i], false);
			}
			
			connectionManager.RefreshConnected();
			RefreshConnectionMaterials();
			GraphManager.Manager.RefreshGraph();
			BonusManager.Manager.RefreshBonuses();
			OverrideManager.Manager.Refresh();
			InfoNodeManager.Manager.Refresh();
			HackerManager.Manager.RefreshPower();
			//HexGrid.Manager.RefreshLines();
			
			justRotated = true;
			currentRotation = 0;
			DisableActivePivot();
		}
		else
		{
			//Rotation is out of bounds, try one more rotation.
			RotatePivot ( i_index, false, lines );
		}
	}
	
	
	public List<Pivot> GetLinesTouchingIndex(int i_index)
	{
		List<Pivot> returnLines = new List<Pivot>();
		
		if ( i_index%2 == 0 )
		{
			int horz = i_index*10;
			if ( myPivots.ContainsKey( horz ))
				returnLines.Add( myPivots[horz] );
			
			int up = i_index*10 + 1;
			if ( myPivots.ContainsKey( up ))
				returnLines.Add( myPivots[up] );
			
			int down = i_index*10 + 2;
			if ( myPivots.ContainsKey( down ))
				returnLines.Add( myPivots[down] );			
		}
		else
		{
			int horz = (i_index+1)*10;
			if ( myPivots.ContainsKey( horz ))
				returnLines.Add( myPivots[horz] );
			
			int up = (i_index-HexGrid.Manager.rowSize)*10 + 2;
			if ( myPivots.ContainsKey( up ))
				returnLines.Add( myPivots[up] );
			
			int down = (i_index+HexGrid.Manager.rowSize)*10 + 1;
			if ( myPivots.ContainsKey( down ))
				returnLines.Add( myPivots[down] );
		}
		
		return returnLines;
	}
	
	
	public bool AreLinesTouchingIndex(int i_index)
	{		
		if ( i_index%2 == 0 )
		{
			int horz = i_index*10;
			if ( myPivots.ContainsKey( horz ))
				return true;
			
			int up = i_index*10 + 1;
			if ( myPivots.ContainsKey( up ))
				return true;
			
			int down = i_index*10 + 2;
			if ( myPivots.ContainsKey( down ))
				return true;			
		}
		else
		{
			int horz = (i_index+1)*10;
			if ( myPivots.ContainsKey( horz ))
				return true;	
			
			int up = (i_index-HexGrid.Manager.rowSize)*10 + 2;
			if ( myPivots.ContainsKey( up ))
				return true;	
			
			int down = (i_index+HexGrid.Manager.rowSize)*10 + 1;
			if ( myPivots.ContainsKey( down ))
				return true;	
		}
		
		return false;
	}
	
	private byte[] GetRotationLines(int i_index)
	{
		byte[] links = new byte[9];
		int point0 = (i_index%2==0)? i_index-1 : i_index+1;
		int point1 = i_index-HexGrid.Manager.rowSize;
		int point2 = i_index+HexGrid.Manager.rowSize;
		
		links[0] = (byte)((( ConnectionManager.Manager.connections[point0][0])>0)? 1: 0);
		links[1] = (byte)((( ConnectionManager.Manager.connections[point0][1])>0)? 1: 0);
		links[2] = (byte)((( ConnectionManager.Manager.connections[point0][2])>0)? 1: 0);
		links[3] = (byte)((( ConnectionManager.Manager.connections[point1][0])>0)? 1: 0);
		links[4] = (byte)((( ConnectionManager.Manager.connections[point1][1])>0)? 1: 0);
		links[5] = (byte)((( ConnectionManager.Manager.connections[point1][2])>0)? 1: 0);
		links[6] = (byte)((( ConnectionManager.Manager.connections[point2][0])>0)? 1: 0);
		links[7] = (byte)((( ConnectionManager.Manager.connections[point2][1])>0)? 1: 0);
		links[8] = (byte)((( ConnectionManager.Manager.connections[point2][2])>0)? 1: 0);
		
		return links;
	}
	
	
	/// -----------------------------------------------------------------------------
	/// ENQUEUE ANIMATION
	/// <summary>Enqueues a new Pivot Animation. All pivot animations are initiated this way.</summary>
	/// 			If this is the only pivot animation in the queue, it will automatically begin running.
	/// Params : (PivotAnimation) The animation to be run.  Animation parameters should already be set when Enqueueing.
	/// Return : none
	/// -----------------------------------------------------------------------------
	void EnqueueAnimation( PivotAnimation i_animation, int i_index )
	{
		Queue myQueue;
		if ( animationQueues.ContainsKey( i_index ) )
		{
			animationQueues.TryGetValue(i_index, out myQueue);
		}
		else
		{
			myQueue = new Queue();
			animationQueues.Add( i_index, myQueue );
		}
		
		myQueue.Enqueue( i_animation );
		if ( myQueue.Count == 1 )
		{
			i_animation.Run();
		}
	}
	 
	
	/// -----------------------------------------------------------------------------
	/// DEQUEUE ANIMATION
	/// <summary>Removes the completed animation from the queue and destroys the animation object.</summary>
	/// 			This also runs the next available animation in the queue.
	/// Params : none
	/// Return : none
	/// -----------------------------------------------------------------------------
	void DequeueAnimation(int i_index)
	{		
		Queue myQueue = null; 
		animationQueues.TryGetValue(i_index, out myQueue);
		
		if ( myQueue != null )
		{
			PivotAnimation finished = (PivotAnimation) myQueue.Dequeue();
			
			// Remove and destroy the completed animation
			Destroy( finished );
			
			// If there are animations remaining, begin a new one.
			if ( myQueue.Count > 0)
			{
				PivotAnimation next = (PivotAnimation) myQueue.Peek ();
				next.Run();
			}
			else
			{
				animationQueues.Remove(i_index);
			}
			
			if ( !creatingNewPivot )
				RefreshConnectionMaterials();
				GraphManager.Manager.RefreshGraph();
		}
	}
	
	
	public void RefreshConnectionMaterials()
	{
		MeshRenderer mr;
		foreach ( KeyValuePair<int, Pivot> pivot in myPivots )
		{
			if ( connectionManager.IsLinePowered(pivot.Value.lineIndex) )
			{
				pivot.Value.myConnections.material = Line_Connected_Material;
				pivot.Value.myConnections.lineWidth = connectedLineWidth;
			}
			else
			{
				pivot.Value.myConnections.material = Line_Basic_Material;
				pivot.Value.myConnections.lineWidth = lineWidth;
			}	
		}		
	}
	
	
	public int GetPivotCount()
	{
		return myPivots.Count;
	}
	
	
	/// -----------------------------------------------------------------------------
	/// DRAW CONNECTIONS
	/// <summary>Draws all of the pivots and connetions</summary>
	/// Params : none
	/// Return : none
	/// -----------------------------------------------------------------------------
	void DrawConnections()
	{
		foreach ( KeyValuePair<int, Pivot> pivot in myPivots )
		{
			pivot.Value.myConnections.Draw3D();
		}		
	}
	
}
