using UnityEngine;
using System.Collections;
using Vectrosity;

public class Pivot{
	
	public int 				lineIndex;			
	
	//public int 				index;//X
	//public bool				right;//X				// If the pivot is on a right facing intersection
	//public bool				centered;//X			// If the pivot Control is center oriented
	//public byte				armA;//X
	//public byte				armB;//X
	
	public VectorLine 		myConnections;		// The VectorLine Object that represents the connections for this pivot.
	//public GameObject		pivotControl;//X		// The Pivot Control Objects
	
	//-------------------------------------------------------------------------------------------------------------------
	
	
	public int Point // returns the base point of the line
	{
		get{
			return lineIndex/10;
		}
	}
	
	public byte Dir // returns the secondary point of the line
	{
		get{
			return ((byte)(lineIndex%10));
		}
	}
	
	/// -----------------------------------------------------------------------------
	/// PIVOT CONSTRUCTOR
	/// <summary>Creates a new Pivot</summary>
	/// Params : (int) The direction of the pivot.  >- = Right Pointing : -< = Left Pointing
	/// -----------------------------------------------------------------------------
	public Pivot ( bool i_rightPointing )
	{
		/*
		centered = true;
		if ( i_rightPointing )
			right = true;
		else
			right = false;
		*/
		myConnections = null;
	}
	
	public Pivot ( int i_index,  bool i_rightPointing, bool centered )
	{
		/*
		centered = false;
		index = i_index;
		right = i_rightPointing;
		*/
		myConnections = null;
	}
	
	
	public Pivot ( int i_index, bool i_centered, byte i_armA, byte i_armB, GameObject tempPivotControl, VectorLine tempLine )
	{
		/*
		index = i_index;
		right = (i_index%2 != 0);
		centered = i_centered;
		armA = i_armA;
		armB = i_armB;
		*/
		myConnections = tempLine;
		//pivotControl = tempPivotControl;
	}
	
	public Pivot ( int i_lineIndex, VectorLine i_line  )
	{
		lineIndex = i_lineIndex;
		myConnections = i_line;
		/*
		index = 0;
		right = false;
		centered = false;
		armA = 0;
		armB = 0;
		myConnections = i_line;
		pivotControl = null;
		*/
	}
	
	
	
	
	
	// Sets the inital arm positions once the pivot has been locked
	public void SetArms(bool i_upper, int i_sector)
	{
		/*
		if ( centered )
		{
			if ( i_sector == 1)
			{
				armA = 1;
				armB = 2;
			}
			else
			{
				armA = 0;
				if ( i_upper )
					armB = 1;
				else
					armB = 2;
			}

		}
		else
		{
			if ( i_upper )
			{
				if ( i_sector == 0 )
				{
					armA = 1;
					armB = 1;
				}
				else if ( i_sector == 1 )
				{
					armA = 1;
					armB = 0;
				}
				else
				{
					armA = 0;
					armB = 1;
				}
			}
			else
			{
				if ( i_sector == 0 )
				{
					armA = 2;
					armB = 2;
				}
				else if ( i_sector == 1 )
				{
					armA = 2;
					armB = 0;
				}
				else
				{
					armA = 0;
					armB = 2;
				}
			}
		}
		//Debug.Log ( "ArmA: " + armA + " - ArmB: " + armB);
		*/
	}
	
	
	// Rotates the pivot arms.
	public void RotateArms()
	{
		/*
		if ( right )
		{
			armA = (byte) ((armA ==0)?2:armA-1);
			if ( centered )
				armB = (byte) ((armB ==0)?2:armB-1);
			else
				armB = (byte) ((armB ==2)?0:armB+1);
		}
		else
		{		
			armA = (byte) ((armA ==2)?0:armA+1);
			if ( centered )
				armB = (byte) ((armB ==2)?0:armB+1);
			else
				armB = (byte) ((armB ==0)?2:armB-1);
		}
		
		//Debug.Log ( "ArmA: " + armA + " - ArmB: " + armB);
		*/
	}
	
	public void RotateLine(int i_centerIndex)
	{
		if ( i_centerIndex%2 == 0)
		{
			if ( lineIndex%10<2 )
				lineIndex ++;
			else
				lineIndex -= 2;
		}
		else
		{
			if ( lineIndex%10==0)
				lineIndex = (((lineIndex/10)-1+HexGrid.Manager.rowSize)*10)+1;
			else if ( lineIndex%10==1)
				lineIndex = (((lineIndex/10)-(HexGrid.Manager.rowSize*2))*10)+2;
			else if ( lineIndex%10==2)
				lineIndex = (((lineIndex/10)+1+HexGrid.Manager.rowSize)*10);
		}
	}
	
	/*
	public void SetConnections(int i_gridSize, int i_rowSize)
	{
		
		int indexCenter = 0;
		int indexA = 0;
		int indexB = 0;
		
		if ( centered )
		{
			indexCenter = index;
			indexA = GetIndex (indexCenter, armA, i_gridSize, i_rowSize, right);
			indexB = GetIndex (indexCenter, armB, i_gridSize, i_rowSize, right);
		}
		else
		{
			indexA = index;
			indexCenter = GetIndex (indexA, armA, i_gridSize, i_rowSize, right);
			indexB = GetIndex (indexCenter, armB, i_gridSize, i_rowSize, !right);
		}
		
		// connections are: center->A, center->B, A->Center, B->Center
		
	}*/
	
	/*
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
			
	}*/
	
	
	/*
	/// -----------------------------------------------------------------------------
	/// Has Index
	/// <summary>Check if the current index belongs to this pivot</summary>
	/// Params : (int) the index that we are checking
	/// returns: true if it is.
	/// -----------------------------------------------------------------------------	
	public bool HasIndex(int i_index)
	{
		// a function that I needed - Max 6/30/2013
		//ToDo: check if the index is valid based on the booleans above.
		
		return true;
	}
	
	/// -----------------------------------------------------------------------------
	/// Get Another End
	/// <summary>Get the index of another end of the pivot based on the current end</summary>
	/// Params : (int) the index of the current end
	/// returns: index of another end  if the input is right, otherwise -1.
	/// -----------------------------------------------------------------------------	
	public int GetAnotherEnd(int i_index)
	{
		if(HasIndex(i_index))
		{
			//ToDo: this should be another end of the pivot. Calculate it based on the booleans.
			return 100; 
		}
		return -1; 
	}
	
	
	/// -----------------------------------------------------------------------------
	/// Get Another End
	/// <summary>Get the index of another end of the pivot based on the current end</summary>
	/// Params : (int) the index of the current end
	/// returns: index of another end  if the input is right, otherwise -1.
	/// -----------------------------------------------------------------------------	
	public int GetMiddlePoint(int i_index)
	{
		if(HasIndex(i_index))
		{
			//ToDo: this should be the middle of the pivot(not necessarily the pivot). Calculate it based on the booleans.
			return 100; 
		}
		return -1; 
	}*/
}
