using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GraphManager : MonoBehaviour {
	
	
	#region Members
	private Dictionary<int, Node> _nodes;
	private static GraphManager m_instance; 
	private float _defaultNodeScale;
	private int _connectedFactor;
	public DoorStates _doorStateMachine;
	private List<IRNode> _radars;
	public float RadarDisplayTime;

	// Object Prefabs
	public GameObject IRNodePrefab;
	public GameObject LaserNodePrefab;
	public GameObject DoorNodePrefab;
	public GameObject CameraNodePrefab;
	public GameObject SecurityNodePrefab;
	public GameObject IRWave;
	public GameObject SourceNodePrefab;
	
	// Materials
	public Material IRUnavailable;
	public Material IRAvailable;
	public Material IRConnected;
	public Material DoorUnavailable;
	public Material DoorConnectedLocked;
	public Material DoorConnectedUnlocked;
	public Material DoorClosedLocked;
	public Material DoorClosedUnlocked;
	public Material DoorOpenUnlocked;
	public Material DoorAvailable;
	public Material CameraUnavailable;
	public Material CameraAvailable;
	public Material CameraConnected;
	public Material SecurityUnavailable;
	public Material SecurityAvailable;
	public Material SecurityAccepted;
	public Material SecurityConnected;
	public Material OverrideUnavailable;
	public Material OverrideAvailable;
	public Material OverrideConnected;
	
	#endregion
	
	#region Properties
	public Dictionary<int, Node> NodeList
	{
		get
		{
			return _nodes;
		}
	}
	
	public float DefaultScale
	{
		get{
			return _defaultNodeScale;
		}
	}
	
	
	public static GraphManager Manager
	{
		get
		{
			if(m_instance == null)
			{
				m_instance = GameObject.Find ("GraphManager").GetComponent<GraphManager>();			
			}
			return m_instance;
		}
	}
	
	public int PowerUsage
	{
		get{
			return _connectedFactor;
		}
	}
	#endregion
	
	#region Constructor
	public GraphManager () 
    { 
        m_instance = this;
		_defaultNodeScale = 0.12f;
    }
	#endregion
	
	void Start()
	{
	}
	
	void Update()
	{
	}

	#region Public methods
	//get the graph data from the XML file and initialize the nodes and its states
	public void LoadGraphFromGameData(GraphData i_graphData)
	{	
		//Debug.Log ("*** BEGIN LOAD GRAPH MANAGER DATA ***");
		LoadResources();
		_nodes = new Dictionary<int, Node>();
		
		// Instantiate Source Nodes
		int[] sourceNodes = new int[1];
		// Get index of Start Door
		int startDoorIndex = 0;
		DoorNodeData[] doorNodeData = i_graphData.DoorNodes;
		foreach(DoorNodeData doorNode in doorNodeData)
		{
			if ( doorNode.DoorType.Equals("START") )
				startDoorIndex = doorNode.Index;
		}
		sourceNodes[0] = startDoorIndex;
		Instantiate ( SourceNodePrefab, HexGrid.Manager.GetCoord( startDoorIndex, 60), Quaternion.identity);
		ConnectionManager.Manager.SetUpSourceNodes(sourceNodes);
		
		// Instantiate IR Nodes
		IRNodeData[] IRNodeDatas = i_graphData.IRNodes;
		_radars = new List<IRNode>();
		BasicScoreSystem.Manager.TotalIRs = IRNodeDatas.Length;
		foreach(IRNodeData IRData in IRNodeDatas)
		{
			GameObject thisObject = (GameObject)Instantiate( IRNodePrefab, HexGrid.Manager.GetCoord(IRData.Index, 60.0f), Quaternion.identity);
			IRNode thisIR = thisObject.GetComponent<IRNode>();
			thisIR.Set(IRData);
			_nodes.Add(IRData.Index, thisIR);
			_radars.Add(thisIR);
		}
				
		// Instantiate Door Nodes
		DoorNodeData[] doorNodeDatas = i_graphData.DoorNodes;
		foreach(DoorNodeData doorData in doorNodeDatas)
		{
			GameObject thisObject = (GameObject)Instantiate( DoorNodePrefab, HexGrid.Manager.GetCoord(doorData.Index, 60.0f), Quaternion.identity);
			DoorNode thisDoor = thisObject.GetComponent<DoorNode>();
			thisDoor.Set(doorData);
			_nodes.Add(doorData.Index, thisDoor);
			if ( doorData.DoorType.Equals("START") ) // if this is a start door dont do anything with it
			{
				thisObject.renderer.enabled = false;
			}
		}
		_doorStateMachine = new DoorStates();

		if(GameManager.Manager.PlayerType==1) //thief should not see 
		{
		
			//GameObject startM=GameObject.Find("StartMarker");
			//startM.renderer.enabled=false;
			if( Application.loadedLevel > 2 )
			{
				GameObject startText = GameObject.Find("StartText");
				startText.renderer.enabled=false;
				GameObject startRing1 = GameObject.Find("StartRing1");
				startRing1.renderer.enabled=false;
				GameObject startRing2 = GameObject.Find("StartRing2");
				startRing2.renderer.enabled=false;

				//GameObject endMarker=GameObject.Find("EndMarker");
				//endMarker.renderer.enabled=false;
				GameObject endText = GameObject.Find("EndText");
				endText.renderer.enabled=false;
				GameObject endRing1 = GameObject.Find("EndRing1");
				endRing1.renderer.enabled=false;
				GameObject endRing2 = GameObject.Find("EndRing2");
				endRing2.renderer.enabled=false;
			}
		}

		// Instantiate Security Nodes
		SecurityNodeData[] securityNodeDatas = i_graphData.SecurityNodes;
		BasicScoreSystem.Manager.TotalSecurityNodes = securityNodeDatas.Length;
		foreach(SecurityNodeData securityData in securityNodeDatas)
		{
			GameObject thisObject = (GameObject)Instantiate( SecurityNodePrefab, HexGrid.Manager.GetCoord(securityData.Index, 60.0f), Quaternion.identity);
			SecurityNode thisSecurity = thisObject.GetComponent<SecurityNode>();
			thisSecurity.Set(securityData);
			_nodes.Add(securityData.Index, thisSecurity);
		}
		
		foreach ( KeyValuePair<int, Node> node in _nodes)
		{
			HexGrid.Manager.SetNode(node.Key);
			node.Value.SetConnected ( false );
		}
		//Debug.Log ("*** END LOAD GRAPH MANAGER DATA ***");
		RadarDisplayTime = 0.5f;
		InvokeRepeating("DisplayVisibleGuards", RadarDisplayTime, RadarDisplayTime);
	}


	// Refresh the connection states of all of the nodes.
	public void RefreshGraph()
	{
		_connectedFactor = 0;
		
		foreach ( KeyValuePair<int, Node> n in _nodes)
		{
			//Hacker Power Update
			/*
			if( ConnectionManager.Manager.IsConnected( n.Key ) )
				_connectedFactor += n.Value.PowerConsumption;
			*/

			// If you just changed the conection state of a node...
			if ( HackerManager.Manager.CheckHackerClearance( n.Value.SecurityLevel ) && n.Value.Connected != ConnectionManager.Manager.IsConnected( n.Key ) )
			{
				n.Value.SetConnected ( ConnectionManager.Manager.IsConnected( n.Key ) );	
			}
		}
	}

	public DoorNode GetEndDoorNode()
	{
		foreach( KeyValuePair<int, Node> n in _nodes )
		{
			if( n.Value is DoorNode )
			{
				DoorNode thisDoor = n.Value as DoorNode;
				if ( thisDoor._doorType == DoorType.EndDoor )
				{
					return thisDoor;
				}
			}
		}
		return null;
	}


	public void RaiseSecurityClearance()
	{
		// Raise Security Clearance
		BasicScoreSystem.Manager.SecurityNodesCaptured += 1;
		HackerManager.Manager.HackerClearance += 1;
		BasicScoreSystem.Manager.PasswordsObtained += 1;
		GraphManager.Manager.RefreshSecurityClearance();
		GraphManager.Manager.RefreshGraph();
	}
	
	
	public void RefreshSecurityClearance()
	{
		foreach ( KeyValuePair<int, Node> n in _nodes)
		{	
			n.Value.SetClearance ( );	
		}
	}

	public void HackerClickedCancel()
	{
		foreach( KeyValuePair<int, Node> n in _nodes )
		{
			if( n.Value is SecurityNode )
			{
				SecurityNode otherAsSecNode = n.Value as SecurityNode;
				otherAsSecNode.HackerClickedCancel();
			}
		}
	}
	
	
	// Handler for when the player clicks on a node
	public void HandleNodeClick( int indexClicked )
	{
		//Debug.Log ("Graph Manager Handling Node Click");
		_nodes[indexClicked].HandleClickEvent();
	}
	
	
	// Loads all of the Game objects and Material Resources for the Nodes
	private void LoadResources()
	{
		// Load Prefabs & Materials
		IRNodePrefab = (GameObject) Resources.Load("Prefabs/Hacker/Graph/IRNode");
		DoorNodePrefab = (GameObject) Resources.Load("Prefabs/Hacker/Graph/DoorNode");
		CameraNodePrefab = (GameObject) Resources.Load("Prefabs/Hacker/Graph/CameraNode");
		SecurityNodePrefab = (GameObject) Resources.Load("Prefabs/Hacker/Graph/SecurityNode");
		SourceNodePrefab = (GameObject) Resources.Load("Prefabs/Hacker/Graph/SourceNode");
		LaserNodePrefab = ( GameObject ) Resources.Load("Prefabs/Hacker/Graph/LaserNode");
		
		IRUnavailable = (Material) Resources.Load ("Materials/Hacker/Nodes/IR_AboveClearance_Material");
		IRAvailable = (Material) Resources.Load ("Materials/Hacker/Nodes/IR_Neutral_Material");
		IRConnected = (Material) Resources.Load ("Materials/Hacker/Nodes/IR_Captured_Material");
		DoorUnavailable = (Material) Resources.Load ("Materials/Hacker/Nodes/Door_AboveClearance_Material");
		DoorAvailable = (Material) Resources.Load ("Materials/Hacker/Nodes/Door_Neutral");
		DoorConnectedLocked = (Material) Resources.Load ("Materials/Hacker/Nodes/Door_Closed_Locked");
		DoorConnectedUnlocked = (Material) Resources.Load ("Materials/Hacker/Nodes/Door_Capture_Unlocked");
		DoorClosedLocked = (Material) Resources.Load ("Materials/Hacker/Nodes/Door_Closed_Locked");
		DoorClosedUnlocked = (Material) Resources.Load ("Materials/Hacker/Nodes/Door_Closed_Unlocked");
		DoorOpenUnlocked = (Material) Resources.Load( "Materials/Hacker/Nodes/Door_Open_Unlocked" );
		CameraUnavailable = (Material) Resources.Load ("Materials/Hacker/Nodes/Camera_AboveClearance_Material");
		CameraAvailable = (Material) Resources.Load ("Materials/Hacker/Nodes/Camera_Neutral");
		CameraConnected = (Material) Resources.Load ("Materials/Hacker/Nodes/Camera_Captured");
		SecurityUnavailable = (Material) Resources.Load ("Materials/Hacker/Nodes/Powered_AboveClearance");
		SecurityAvailable = (Material) Resources.Load ("Materials/Hacker/Nodes/Node_Neutral");
		SecurityConnected = (Material) Resources.Load ("Materials/Hacker/Nodes/Node_Captured");
		SecurityAccepted = (Material) Resources.Load ("Materials/Hacker/Nodes/Node_Security_Accepted");
		
		IRWave = (GameObject) Resources.Load ("Prefabs/Hacker/Graph/IRWave");
		
	}
	
	// Helper Method 
	public bool NodeExistsOnIndex(int i_index)
	{
		return _nodes.ContainsKey( i_index);
	}
	
	
	public Node GetNode(int i_index)
	{
		if ( _nodes.ContainsKey( i_index ) )
			return _nodes[i_index];
		else
			return null;
	}
	
	public List<Node> GetNodes(GameManager.NodeType i_type)
	{
		List<Node> nodes = new List<Node>();
		foreach ( KeyValuePair<int, Node> thisNode in _nodes)
		{
			if ( thisNode.Value.Type == i_type)
				nodes.Add( thisNode.Value );
		}
		return nodes;
	}	
	
	private void DisplayVisibleGuards()
	{	
		GameObject[] guard2D = GameObject.FindGameObjectsWithTag("Guard2D");

		foreach( GameObject g in guard2D)
		{
			bool visible = false;

			foreach ( IRNode radar in _radars )
			{
				if ( radar.Connected && radar.GuardIsWithinRange ( g.transform.position ) && HackerManager.Manager.CheckHackerClearance( radar.SecurityLevel ) )
				{
					visible = true;
				}
			}
			
			if ( visible )
				g.renderer.enabled = true;
			else
				g.renderer.enabled = false;
			
		}
	}
	#endregion
	
	#region Old Code -2013/07/12
	/*
	public Node SecDestNode;
	private List<Node> SourceNodeList = new List<Node>();
	private List<Node> ActiveIRNodeList = new List<Node>();
	public List<Node> EntireList = new List<Node>();
	
	private static GraphManager m_instance;
	
	public bool UpdateNotice = false;
	
	public static securityOverlord overlord;
	
	public float JumpTime;
	
	int jumpCounter;
	
	// Use this for initialization	
	void Start () 
	{
		//add all the node to the main node
		GameObject[] objList = GameObject.FindGameObjectsWithTag("Node");
		Debug.Log("List Length: " + objList.Length);
		GetSourceList();
		// adding nodes in order
		//int j = 0;
		if(EntireList.Count == 0)
		{
			for(int i = 0; i < objList.Length; i++)
			{
				EntireList.Add(((GameObject)objList[i]).GetComponent<Node>());
				//Debug.Log("length right now: "+EntireList.Count);
			}
		}
		
		jumpCounter = 0;
		
		//Debug.Log("Addlist: " + EntireList.Count);
		//if(SourceNode == null)
		//	GetSource();
		
		
		foreach(Node n in EntireList)
		{
			if(n.NodeList.Count == 0)
			{
				Debug.Log(n.name + " HAS NO NEIGHBORS");
			}
		}
		UpdateNodeSystem();
		GameObject securityOverlord = GameObject.Find("securityOverlord");
		overlord = (securityOverlord)securityOverlord.GetComponent<securityOverlord>();
		JumpTime = overlord.jumpTime;
		reset();
	}
	
	public void TriggerStart()
	{
		//add all the node to the main node
		GameObject[] objList = GameObject.FindGameObjectsWithTag("Node");
		Debug.Log(objList.Length);
		
		// adding nodes in order
		for(int i = 0; i < objList.Length; i++)
		{
			EntireList.Add(((GameObject)objList[i]).GetComponent<Node>());
		}
		GetSourceList();
		//Debug.Log("Addlist: " + EntireList.Count);		
	}
	
	void Update()
	{
		if(UpdateNotice)
		{
			Debug.Log("UpdateFromSource");
			UpdateFromSource();
			UpdateNotice = false;
		}
	}
	
	public void GetSourceList()
	{
		SourceNodeList.Clear();
		int j = 0;
		foreach(Node node in EntireList)
		{
			if(node.IsSource == true)
			{
				j++;
				Debug.Log("Found SourceNode" + j);
				SourceNodeList.Add(node);
			}
		}
	}
	
	public void reset()
	{
		//Debug.Log(EntireList.Count);
		if(EntireList.Count == 0)
		{
			Debug.Log("List Is Empty");
			TriggerStart();
		}
		foreach(Node node in EntireList)
		{
			node.Reset();
		}
	}
	
	public void UpdateNodeSystem()
	{
		UpdateFromSource();
		EnableUpdate();
	}
	
	
	//so we do this when the releasing starts
	public void UpdateFromSource()
	{
		reset();
		if(SourceNodeList.Count == 0)
			GetSourceList();
		
		//mark the source node as searched
		foreach(Node src in SourceNodeList)
		{
			src.Mark();
		}
		//start tracking algorithm
		foreach(Node src in SourceNodeList)
		{
			if(src.GetState() == GameManager.NodeState.Controlled ||
				src.GetState() == GameManager.NodeState.Encrypted ||
				src.GetState() == GameManager.NodeState.Encrypting)
			{
				src.TrackFromSource();
			}
		}
		//SourceNode.TrackFromSource();
	}
	
	//seperate this enhance the performance
	public void EnableUpdate()
	{
		//so every node that is not linked to the source are disabled
		foreach(Node node in EntireList)
		{
			if(!node.isMarked())
			{
				if((node._state == GameManager.NodeState.Capturing) 
				 ||(node._state == GameManager.NodeState.Encrypting)
				 ||(node._state == GameManager.NodeState.Releasing))
				{
					node.EnableInterrupted();
				}
				
				if((node.GetState() == GameManager.NodeState.Controlled)
				||(node.GetState() == GameManager.NodeState.Encrypted)
				||(node.GetState() == GameManager.NodeState.Encrypting))
				{
					node.State_MakeDeactivatied();
				}
				else
				{
					node.state_MakeUnreachable();
				}
				
			}
		}
		//update the node system to see what nodes are neutral
		foreach(Node node in EntireList)
		{
			if(node.isMarked())
			{
				if((node.GetState() != GameManager.NodeState.Capturing)
					&&(node.GetState() != GameManager.NodeState.Neutral)
					&&(node.GetState() != GameManager.NodeState.Unreachable)
					&&(node.GetState() != GameManager.NodeState.aboveSecurityClearance)
					&&(node.GetState() != GameManager.NodeState.PoweredOff)
					&&(node.GetState() != GameManager.NodeState.Deactivated))
					node.UpdateState();
			}
		}
		
		//keep the source node alive
		foreach(Node src in SourceNodeList)
		{
			if(src.GetState() == GameManager.NodeState.Unreachable)
				src.state_MakeNeutral();
		}
	}
	
	public void clearAllNode()
	{
		foreach(Node node in EntireList)
		{
			if((node._state == GameManager.NodeState.Capturing) 
			 ||(node._state == GameManager.NodeState.Encrypting)
			 ||(node._state == GameManager.NodeState.Releasing))
			{
				node.EnableInterrupted();
			}
			node.state_MakeUnreachable();
				
		}
		if(SourceNodeList.Count == 0)
		{
			GetSourceList();
		}
		foreach(Node src in SourceNodeList)
		{
			src.state_MakeNeutral();
		}
	}
	
	public void UpdateSourceNodeList()
	{
		SourceNodeList.Clear();
		int j = 0;
		foreach(Node node in EntireList)
		{
			if(node.IsSource == true)
			{
				j++;
				Debug.Log("Found SourceNode" + j);
				SourceNodeList.Add(node);
			}
		}		
	}
	
	public GraphManager () 
    { 
        m_instance = this;
    }
	
	public Node getNodeByNumber(int i_num)
	{
		if(EntireList.Count == 0)
		{
			//Debug.Log("OutofRange");
			TriggerStart ();
			if(SourceNodeList.Count == 0)
			{
				GetSourceList();
			}
			return EntireList[i_num];
		}
		else
			return EntireList[i_num];
	}
	
	public void EMPActivated(Vector2 i_EmpPos, float EMP_Influence_Radius)
	{
		Debug.Log ("Last EMP function called. Logic works here.");
		
		Vector2 currNodePos;
		
		foreach(Node node in EntireList)
		{
			currNodePos = new Vector2(node.gameObject.transform.position.x, node.gameObject.transform.position.z);
			
			if(Vector2.Distance(i_EmpPos, currNodePos) < EMP_Influence_Radius)
			{
				node.State_ChangeToEMPReleased();
			}
		}
		
		//UpdateNodeSystem();
	}
	
	public void RecoverEMP()
	{		
		UpdateNodeSystem();	
	}
	
	public GameObject getGameObjectByNumber(int i_num)
	{
		if(EntireList.Count == 0)
		{
			//Debug.Log("OutofRange");
			TriggerStart ();
			if(SourceNodeList.Count == 0)
			{
				GetSourceList();
			}
			
			foreach(Node obj in EntireList)
			{
				if(obj.NodeNumber == i_num)
					return obj.gameObject;
			}
			return EntireList[i_num].gameObject;
			
		}
		else
		{
			foreach(Node obj in EntireList)
			{
				if(obj.NodeNumber == i_num)
					return obj.gameObject;
			}
			return null;
		}
	}
	
	public static GraphManager Manager
	{
		get
		{
			if(m_instance == null)
			{
				m_instance = new GraphManager();			
			}
			return m_instance;
		}
	}
	*/
	#endregion
}
