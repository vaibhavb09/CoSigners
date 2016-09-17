using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class ConnectionManager : MonoBehaviour {
	
	#region members
	private HexGrid 					hexGrid;
	public PivotManager					_pivotManager;
	private List<int> 					_sourceNodeIndices;			//source node index
	//private List<Pivot> 				_connectedPivots;			//The pivots connected to the source node
	private bool[]						connectedIndecies;			// The indecies that are connected to a source node.
	private bool 						_isInitialized = false;
	private static ConnectionManager 	_instance;
	private int 						_connectedCount;			// Represents the number of indecies that are currently connected to Power.
	
	public List<byte[]>	connections;  // Describes the number of link arms conecting from any index (List) in any direction (byte[])
	#endregion
	
	
	#region Init
	// Use this for initialization
	void Start () {
	}
	
	#endregion
	
	public static ConnectionManager Manager
	{
		get
		{
			if(_instance == null)
			{
				_instance = GameObject.Find ("HexGrid").GetComponent<ConnectionManager>();			
			}
			return _instance;
		}
	}
	
		#region Constructor
	public ConnectionManager () 
    { 
        _instance = this;
    }
	#endregion
	
	public int ConectedCount
	{
		get{
			return _connectedCount;
		}
	}
	
	
	public void Load()
	{
		//Debug.Log ("Loading Connection Manager");
		hexGrid = (HexGrid) gameObject.GetComponent("HexGrid");
		
		// Set up the List of Connection Counts.
		connections = new List<byte[]>(hexGrid.gridSize);
		for ( int i=0 ; i<connections.Capacity ; i++)
			connections.Add(new byte[3]{0,0,0});
		
		connectedIndecies = new bool[HexGrid.Manager.gridSize];
		_isInitialized = true;
	}
	
	
	public void RefreshConnected()
	{
		if ( connectedIndecies != null)
			Array.Clear(connectedIndecies, 0, connectedIndecies.Length );
		
		if(_sourceNodeIndices == null)
		{
			_sourceNodeIndices = new List<int>();
		}
		
		_connectedCount = 0;
		foreach(int sourceIndex in _sourceNodeIndices)
		{
			CheckConnections(sourceIndex);
		}
		
		GraphManager.Manager.RefreshGraph();

		//printConnections();
		//Debug.Log ("Player Type is:" + GameManager.Manager.PlayerType);
		if ( GameManager.Manager.PlayerType == 1) // Thief needs to update the floor.
			ThiefGrid.Manager.UpdateFloorConnections();
	}
	
	
	public void SetUpSourceNodes(int[] i_sourceNodes)
	{
		if(_sourceNodeIndices == null)
		{
			_sourceNodeIndices = new List<int>();
		}
			
		if(i_sourceNodes != null)
		{
			_sourceNodeIndices.Clear();
			for(int i = 0; i < i_sourceNodes.Length; i++)
			{
				//Debug.Log("Source Node" + (i + 1) + ": " + i_sourceNodes[i]);
				_sourceNodeIndices.Add(i_sourceNodes[i]);
			}
		}		
	}
	
	
	private bool ConnectionsChanged(bool[] old)
	{
		for( int i=0 ; i<HexGrid.Manager.gridSize ; i++ )
		{
			if ( old[i] != connectedIndecies[i] )
				return true;
		}
		return false;
	}
	
	
	private void CheckConnections( int i_index )
	{
		//Debug.Log ("Checking Index: " + i_index);
		connectedIndecies[i_index] = true;
		_connectedCount ++;
		
		for ( int i = 0 ; i < 3 ; i++ )
		{
			int nextIndex;
			if ( connections[i_index][i] != 0 )
			{
				nextIndex = HexGrid.Manager.GetIndex(i_index, (byte)i);
				if ( nextIndex != -1 && !connectedIndecies[nextIndex] )
					CheckConnections( nextIndex );
				
				//nextIndex = GetIndex (i_index, i);
				//if ( !connectedIndecies[nextIndex] )
				//	CheckConnections( nextIndex );
			}
		}
	}
	
	
	private int GetIndex(int i_startIndex, int i_direction)
	{
		int destIndex = -1;
		bool right = (i_startIndex%2 != 0);
		
		if ( i_direction == 0 )
			destIndex = i_startIndex + ((right)?1:-1);
		else if ( i_direction == 1 )
			destIndex = i_startIndex - hexGrid.rowSize;
		else
			destIndex = i_startIndex + hexGrid.rowSize;
		
		return destIndex;
	}
	
	public int GetSourceIndex()
	{
		return _sourceNodeIndices[0];
	}
	
	
	public void SetConnections( Pivot i_pivot, bool clear )
	{	
		/*
		int indexP = i_pivot.index;
		int indexA = HexGrid.Manager.GetIndex(i_pivot.index, i_pivot.armA);
		int indexB = HexGrid.Manager.GetIndex(indexA, i_pivot.armB);
		byte PtoA = (byte)(i_pivot.armA);
		byte AtoP = (byte)((PtoA == 0)?0:((PtoA ==1)?2:1));
		byte XtoB = (byte)(i_pivot.armB);
		byte BtoX = (byte)((XtoB == 0)?0:((XtoB ==1)?2:1));
		
		if ( clear )
		{
			connections[indexP][PtoA] --;
			connections[indexA][AtoP] --;
			connections[indexB][BtoX] --;
			if ( i_pivot.centered )
				connections[indexP][XtoB] --;
			else
				connections[indexA][XtoB] --;
		}
		else
		{
			connections[indexP][PtoA] ++;
			connections[indexA][AtoP] ++;
			connections[indexB][BtoX] ++;
			if ( i_pivot.centered )
				connections[indexP][XtoB] ++;
			else
				connections[indexA][XtoB] ++;
		}
		
		//printConnections();
		*/
		
		int indexA = i_pivot.Point;
		byte dirA = i_pivot.Dir;
		int indexB = HexGrid.Manager.GetIndex(indexA, dirA);
		byte dirB = (byte)((dirA == 0)?0:((dirA ==1)?2:1));
		
		if ( clear )
		{
			connections[indexA][dirA] --;
			connections[indexB][dirB] --;
		}
		else
		{
			connections[indexA][dirA] ++;
			connections[indexB][dirB] ++;
		}
		
	}
	
	public void SetBranchConnections( int i_index, int i_0, int i_1, int i_2 )
	{
		connections[i_index][0] ++;
		connections[i_index][1] ++;
		connections[i_index][2] ++;
		
		if ( i_0 >= 0 )
			connections[i_0][0] ++;
		if ( i_1 >= 0 )
			connections[i_1][2] ++;
		if ( i_2 >= 0 )
			connections[i_2][1] ++;
	}
	
	
	int GetIndex( int index, byte direction, int gridSize, int rowSize, bool i_right)
	{
		if ( direction == 0 )
		{
			return (i_right)?index+1:index-1;
		}
		else if ( direction == 1)
			return index-rowSize;
		else
			return index+rowSize;
	}
	
	
	public bool IsConnected (int i_index)
	{
		if(i_index > connectedIndecies.Length)
		{
			Debug.LogError("Index is larger than the connectIndices length, Chris, something is fucked up. - Max");
			return false;
		}
		if ( _sourceNodeIndices.Contains( i_index ) )
			return true;
		if ( i_index < HexGrid.Manager.gridSize && connectedIndecies[i_index] )
			return true;
		else
			return false;

		//return connectedIndecies[i_index];
	}
	
	
	public bool IsLinePowered(int i_line)
	{
		int index = (int)(i_line/10);
		int direction = i_line%10;
		if ( connectedIndecies[index] )
		{
			if ( connections[index][direction] != 0 )
				return true;
		}
		return false;
	}
	
	public bool IsLineConnected(int i_line)
	{
		int index = (int)(i_line/10);
		int direction = i_line%10;
		if ( connections[index][direction] != 0 )
		{
			return true;
		}
		return false;
	}
	
	
	public void SetUpConnections(bool i_IsConnected, int i_index)
	{
		connectedIndecies[i_index] = i_IsConnected;
	}
	
	
	public void UpdateConnectionsThief( byte[] i_connections )
	{
		//Debug.Log ("UPDATE CONNECTION THIEF!!!");
		for( int i=0 ; i<HexGrid.Manager.gridSize ; i++)
		{
			int bucket = i/8;
			int value = (i_connections[bucket] >> i%8)&1;
			connectedIndecies[i] = (value==0)? false : true; 
			//Debug.Log ("Index:" + i + " = " + connectedIndecies[i]);
		}
	
		_pivotManager.RefreshConnectionMaterials();
		ThiefGrid.Manager.UpdateFloorConnections();
		
	}
	
	private int[] PackConnectedIndecies()
	{
		int arraySize = HexGrid.Manager.gridSize/32;
		int[] tempArray = new int[arraySize];
		
		for( int i=0 ; i<HexGrid.Manager.gridSize ; i++)
		{
			int bucket = i/32;
			if ( connectedIndecies[i] )
				tempArray[bucket] += (int)Mathf.Pow(2, i%32); 
		}
		
		return tempArray;
	}
	
	private byte[] PackByteConnectedIndecies()
	{
		int arraySize = HexGrid.Manager.gridSize/8;
		byte[] tempArray = new byte[arraySize];
		
		for( int i=0 ; i<HexGrid.Manager.gridSize ; i++)
		{
			byte bucket = (byte)(i/8);
			if ( connectedIndecies[i] )
				tempArray[bucket] += (byte)Mathf.Pow(2, i%8); 
		}
		
		return tempArray;
	}
	
	
	public bool IsHexTouchingSource( int i_hexIndex )
	{
		for ( int i=0 ; i<_sourceNodeIndices.Count ; i++ )
		{
			if ( _sourceNodeIndices[i]%2 == 0 ) // node is even
			{
				if ( (i_hexIndex == _sourceNodeIndices[i]) ||
						((_sourceNodeIndices[i]-1)- HexGrid.Manager.rowSize == i_hexIndex) ||
						((_sourceNodeIndices[i]-1)+ HexGrid.Manager.rowSize == i_hexIndex) )
					return true;		
			}
			else
			{
				if ( (_sourceNodeIndices[i]-1 == i_hexIndex) ||
						((_sourceNodeIndices[i])- HexGrid.Manager.rowSize == i_hexIndex) ||
						((_sourceNodeIndices[i])+ HexGrid.Manager.rowSize == i_hexIndex) )
					return true;
			}
		}
		return false;
	}
				
	
	public void printConnections()
	{
		//Debug.Log("connections Size: " + connections.Capacity);
//		for ( int i = 0 ; i < HexGrid.Manager.gridSize ; i++ )
//		{
//			if ( connectedIndecies[i] == true )
//			{
//				//Debug.Log ("index: " + i );
//			}
//			//Debug.Log ("index: " + i + " - " + (connections[i][0]) + (connections[i][1]) + (connections[i][2]) );
//		}
	}
	
}
