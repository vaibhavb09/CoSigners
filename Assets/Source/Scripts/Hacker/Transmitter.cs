using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Transmitter {
	
	public int index;					// The hex index of the transmitter
	public int radius;					// The range in number of jumps of the transmitter

	public GameObject transmitter; 		// The transmitter prefab for the Hacker
	private List<int> affected;			// The index numbers of the intersections icluded in the transmitter range
	private List<int> affectedLines;	// The index numbers of all the lines included in the transmitter range
	int rowSize;				
	HexGrid _hexGrid;
	public bool isActive;
	public bool isJammer;
	

	public Transform myWorldTransform;
	public Transform solidTransmitter;
	public Transform ghostTransmitter;
	
	public Transmitter(int i_index, HexGrid i_grid, bool i_isJammer, int i_range)
	{
		solidTransmitter = Resources.Load("Prefabs/Theif/Transmitter_Prefab", typeof(Transform)) as Transform;
		ghostTransmitter = Resources.Load("Prefabs/Theif/GhostTransmitter", typeof(Transform)) as Transform;
		
		isActive = true;
		isJammer = i_isJammer;
		index = i_index;
		radius = i_range;
		_hexGrid = i_grid;
		rowSize = i_grid.rowSize;
		SetAffected();

	}
	
	// ---------------------------------------------------------------------
	// calculates al of the intersections and lines that are affeted by 
	// this transmitter and stores then in two lists.
	// ---------------------------------------------------------------------
	private void SetAffected()
	{
		affected = new List<int>();
		affectedLines = new List<int>();
		
		// Include Initial 6 points areound the hex
		affected.Add(index);
		if ( !_hexGrid.OffEdge(index+1, index ) )
		{
			affected.Add(index+1);
		}
		if ( !_hexGrid.OffEdge(index+rowSize, index ) )
		{
			affected.Add(index+rowSize);

			affectedLines.Add (index*10+2);//from index going down
		}
		if ( !_hexGrid.OffEdge(index-rowSize, index ) )
		{
			affected.Add(index-rowSize);
			affectedLines.Add(index*10+1); //from index going up

		}
		if ( !_hexGrid.OffEdge(index+rowSize+1, index ) )
		{
			affected.Add(index+rowSize+1);
			affectedLines.Add((index+rowSize+1)*10); //from index+row+1 going horz back

			if ( !_hexGrid.OffEdge(index+1, index ) )
			{
				affectedLines.Add((index+rowSize+1)*10+1); //from index-row+1 going up

			}
		}
		if ( !_hexGrid.OffEdge(index-rowSize+1, index ) )
		{
			affected.Add(index-rowSize+1);
			affectedLines.Add((index-rowSize+1)*10); //from index-row+1 going horz back

			if ( !_hexGrid.OffEdge(index+1, index ) )
			{
				affectedLines.Add((index-rowSize+1)*10+2); //from index-row+1 going down

			}
		}

		// Loop through Affected Indecies and activate points.
		List<int> previousAffected = new List<int>();
		previousAffected.AddRange( affected );
		List<int> tempNewAffected = new List<int>();
		//radius = (isJammer)? radius+1 : radius;
		for( int j=0 ; j<radius ; j++)
		{
			for ( int i = 0 ; i<previousAffected.Count ; i++ )
			{
				int thisIndex = (int)previousAffected[i];
				//Check Horz
				if ( !affected.Contains((thisIndex%2==0)?(thisIndex-1):(thisIndex+1)) && !_hexGrid.OffEdge((thisIndex%2==0)?(thisIndex-1):(thisIndex+1), thisIndex ))
				{
					if ( j<radius )
						affected.Add( (thisIndex%2==0)?(thisIndex-1):(thisIndex+1) );
					
					tempNewAffected.Add ((thisIndex%2==0)?(thisIndex-1):(thisIndex+1));
					
					int lineIndex = (thisIndex%2==0)? thisIndex*10 : (thisIndex+1)*10;

					
					if ( (isJammer)? j<radius+2 : j<radius+2)
					{
							affectedLines.Add( lineIndex );
						
						//Check for existing point up and down. ( if those points already exist make a line between them )
						if ( thisIndex%2 ==0 ) // for only even indecies
						{
							if ( affected.Contains(thisIndex+rowSize ) )
								affectedLines.Add ( thisIndex*10+2 );

							
							if ( affected.Contains(thisIndex-rowSize ) )
								affectedLines.Add ( thisIndex*10+1 );

						}
					}
				}
				//Check Down
				if ( !affected.Contains(thisIndex+rowSize) && !_hexGrid.OffEdge((thisIndex+rowSize), thisIndex ))
				{
					if ( j<radius )
						affected.Add( thisIndex+rowSize );
					
					tempNewAffected.Add( thisIndex+rowSize );
					
					int lineIndex = (thisIndex%2==0)? thisIndex*10+2 : (thisIndex+rowSize)*10+1;
					
					if ( (isJammer)? j<radius+2 : j<radius+2)
					{
						affectedLines.Add( lineIndex );
						
						//Check for existing point up and horz. ( if those points already exist make a line between them )
						if ( thisIndex%2 ==0 ) // for only even indecies
						{
							if ( affected.Contains(thisIndex-rowSize ) )
								affectedLines.Add ( thisIndex*10+1 );

							
							if ( affected.Contains( thisIndex-1 ) )
								affectedLines.Add ( thisIndex*10 );

						}
					}
				}
				//Check Up
				if ( !affected.Contains(thisIndex-rowSize) && !_hexGrid.OffEdge((thisIndex-rowSize), thisIndex ) ) 
				{
					if ( j<radius )
						affected.Add( thisIndex-rowSize );
					
					tempNewAffected.Add( thisIndex-rowSize );



					int lineIndex = (thisIndex%2==0)? thisIndex*10+1 : (thisIndex-rowSize)*10+2;
					
					if ( (isJammer)? j<radius+2 : j<radius+2)
					{
						affectedLines.Add( lineIndex );
						
						//Check for existing point down and horz. ( if those points already exist make a line between them )
						if ( thisIndex%2 ==0 ) // for only even indecies
						{
							if ( affected.Contains(thisIndex+rowSize ) )
								affectedLines.Add ( thisIndex*10+2 );

							
							if ( affected.Contains( thisIndex-1 ) )
								affectedLines.Add ( thisIndex*10 );

						}
					}
				}
			}
			previousAffected.Clear();
			previousAffected.AddRange(tempNewAffected);
			tempNewAffected.Clear();
		}
		
		//PrintAffectedLines();
	}
	
	public List<int> GetAffected()
	{
		return affected;
	}
	
	public List<int> GetAffectedLines()
	{
		return affectedLines;
	}
	
	public bool IsInRange( int i_index)
	{
		if (affected.Contains( i_index ))
			return true;
		else
			return false;
	}
	
	public bool ContainsLine( int i_lineIndex )
	{
		if (affectedLines.Contains( i_lineIndex ))
			return true;
		else
			return false;
	}
	
	
	public bool ContainsPoint( int i_point )
	{
		if (affected.Contains( i_point ))
			return true;
		else
			return false;
	}
		
		
		
	// DEBUG HELPER FUNCITON
	public void PrintAffectedLines()
	{
		for( int i = 0 ; i<affectedLines.Count ; i++ )
		{
			//Debug.Log ("Line:" + i + " = " + affectedLines[i]);
		}
	}
	
	public void TransmitterAddRadius()
	{
		radius = radius+1000;

	}
}
