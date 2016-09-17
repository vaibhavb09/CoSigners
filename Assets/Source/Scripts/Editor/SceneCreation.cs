using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System;


// LEGACY CODE
/*
public enum WallType 
{TeeMMU, TeeUUM, CornerLot, CornerRot,  CornerXol, CornerXor, CornerObtM, CornerObtU, WhyU, WhyM, Door_M, Door_U, Door_End, Door_Start, Wall_1M, Wall_2M, Wall_4M, Wall_2U, Wall_4U, End_Cap}

public enum DoorPcType 
{ StandardM, StandardU, End, Start}


public class WallPiece
{
	public WallPiece( float i_x, float i_z, float i_rot, bool i_M, bool i_U, bool i_X, WallType i_type)
	{
		x = i_x;
		z = i_z;
		rot = i_rot;
		long_M = i_M; // In case of Tee this is the right M;
		long_U = i_U; // In case of Tee this is the top U;
		extLong = i_X; // In case of Tee this is the extention of extra arm.
		type = i_type;
	}

	public float x, z, rot;
	public bool long_M, long_U, extLong; 
	public WallType type;
}


public class DoorPiece
{
	public DoorPiece(float i_x, float i_z, float i_rot, DoorPcType i_type)
	{
		x = i_x;
		z = i_z;
		rot = i_rot;
		type = i_type;
	}

	public float x,z,rot;
	public DoorPcType type;
}


public class WallCollider
{
	public WallCollider(float i_x, float i_z, float i_w, float i_r)
	{
		centerX = i_x;
		centerZ = i_z;
		width = i_w;
		rotation = i_r;
	}

	public float centerX, centerZ, width, rotation;

}


[ExecuteInEditMode]
public class SceneCreation : MonoBehaviour {

	#region Public Variables
	public bool BuildScene;
	public string SceneFile;
	public GameObject CONTAINER;
	public GameObject SIDE;
	public GameObject COR_LIN;
	public GameObject COR_LOT;
	public GameObject COR_RIN;
	public GameObject COR_ROT;
	public GameObject EXT_ML;
	public GameObject EXT_MR;
	public GameObject EXT_UL;
	public GameObject EXT_UR;
	public GameObject OBT_MIN;
	public GameObject OBT_MOT;
	public GameObject OBT_UIN;
	public GameObject OBT_UOT;
	public GameObject XOB_LIN;
	public GameObject XOB_LOT;
	public GameObject XOB_RIN;
	public GameObject XOB_ROT;
	public GameObject VAL_1M;
	public GameObject VAL_2M;
	public GameObject VAL_4M;
	public GameObject VAL_2U;
	public GameObject VAL_4U;
	public GameObject DOR_2M;
	public GameObject DOR_2U;
	public GameObject DOR_END;
	public GameObject END_CAP;
	public GameObject EXT_CUL;
	public GameObject EXT_CUR;
	public GameObject WAL_COL;
	#endregion

	#region Private Variables
	private int TeeMMUCount;
	private int TeeUUMCount;
	private int CorLotCount;
	private int CorRotCount;
	private int CorXolCount;
	private int CorXorCount;
	private int CorObtMCount;
	private int CorObtUCount;
	private int WhyUCount;
	private int WhyMCount;
	private int DoorPcCount;
	private int Wall1MCount;
	private int Wall2MCount;
	private int Wall4MCount;
	private int Wall2UCount;
	private int Wall4UCount;
	private int EndCapCount;
	private int ColliderCount;
	
	private List<DoorPiece> _doors;
	private List<WallPiece> _pieces;
	private List<WallPiece> _caps;
	private List<WallCollider> _colliders;
	private GameObject _levelWalls;
	private GameObject _levelDoors;
	private GameObject _levelColliders;
	#endregion

	// Use this for initialization
	void Start () {
	
	}


	// Update is called once per frame
	void Update () {
		if ( BuildScene )
		{
			// Re-Build the Scene
			Debug.Log ("REBUILDING SCENE");
			LoadPieces();

			RemoveOldWalls();

			BuildWalls();
		}
	}


	void LoadPieces ()
	{
		_pieces = new List<WallPiece>();
		_doors = new List<DoorPiece>();
		_caps = new List<WallPiece>();
		_colliders = new List<WallCollider>();

		// Load Intersection Pieces.
		TextAsset txt = (TextAsset) Resources.Load("Levels/SceneBuilders/SceneFile");		
		int pointer = 0;
		string[] lines = txt.text.Split("\n"[0]);

		int IntersectCount = Convert.ToInt32(lines[0]);
		pointer ++;
		int endPointer = IntersectCount + pointer;

		for ( int i=pointer ; i<endPointer ; i++ )
		{
			string[] parameters = lines[i].Split(","[0]);

			float x = Convert.ToSingle(parameters[0]);
			float z = Convert.ToSingle(parameters[1]);
			float rot = Convert.ToSingle(parameters[2]);
			bool longM = Convert.ToBoolean(parameters[3]);
			bool longU = Convert.ToBoolean(parameters[4]);
			bool longX = Convert.ToBoolean(parameters[5]);
			WallType type = ParseType( parameters[6] );

			WallPiece tempPiece = new WallPiece( x, z, rot, longM, longU, longX, type);
			_pieces.Add( tempPiece );
			Debug.Log (lines[i]);
			pointer ++;
		}

		// Load Wall Pieces.
		int WallCount = Convert.ToInt32(lines[pointer]);
		pointer ++;
		endPointer = WallCount + pointer;

		for ( int i=pointer; i<endPointer ; i++ )
		{
			string[] parameters = lines[i].Split(","[0]);

			float x = Convert.ToSingle(parameters[0]);
			float z = Convert.ToSingle(parameters[1]);
			float rot = Convert.ToSingle(parameters[2]);
			bool longM = Convert.ToBoolean(parameters[3]);
			bool longU = Convert.ToBoolean(parameters[4]);
			bool longX = Convert.ToBoolean(parameters[5]);
			WallType type = ParseType( parameters[6] );

			if ( type == WallType.Door_M || type == WallType.Door_U ) // This is a door
			{
				DoorPcType thisType;
				if ( type == WallType.Door_M )
					thisType = DoorPcType.StandardM;
				else if ( type == WallType.Door_U )
					thisType = DoorPcType.StandardU;
				else if ( type == WallType.Door_End )
					thisType = DoorPcType.End;
				else 
					thisType = DoorPcType.Start;

				DoorPiece doorPiece1 = new DoorPiece(x, z, rot, thisType);
				_doors.Add( doorPiece1 );
			}
			else // This is a wall.
			{
				WallPiece tempPiece = new WallPiece( x, z, rot, longM, longU, longX, type);
				_pieces.Add( tempPiece );
			}

			Debug.Log (lines[i]);
			pointer ++;
		}

		// Load End Point Pieces 
		int EndCapCount = Convert.ToInt32(lines[pointer]);
		pointer ++;
		endPointer = EndCapCount + pointer;
		
		for ( int i=pointer ; i<endPointer ; i++ )
		{
			string[] parameters = lines[i].Split(","[0]);
			
			float x = Convert.ToSingle(parameters[0]);
			float z = Convert.ToSingle(parameters[1]);
			float rot = Convert.ToSingle(parameters[2]);
			bool longM = Convert.ToBoolean(parameters[3]);
			bool longU = Convert.ToBoolean(parameters[4]);
			bool longX = Convert.ToBoolean(parameters[5]);
			WallType type = ParseType( parameters[6] );
			
			WallPiece tempPiece = new WallPiece( x, z, rot, longM, longU, longX, type);
			_caps.Add( tempPiece );
			Debug.Log (lines[i]);
			pointer ++;
		}

		// Load Wall Collision Lines
		int ColliderCount = Convert.ToInt32(lines[pointer]);
		pointer ++;
		endPointer = ColliderCount + pointer;
		
		for ( int i=pointer ; i<endPointer ; i++ )
		{
			string[] parameters = lines[i].Split(","[0]);
			
			float x = Convert.ToSingle(parameters[0]);
			float z = Convert.ToSingle(parameters[1]);
			float width = Convert.ToSingle(parameters[2]);
			float rot = Convert.ToSingle(parameters[3]);
			
			WallCollider tempPiece = new WallCollider( x, z, width, rot);
			_colliders.Add( tempPiece );
			Debug.Log (lines[i]);
			pointer ++;
		}

		//WallCollider tempCollider = new WallCollider( 0, 10, 10, 0);
		//_colliders.Add( tempCollider);

	}


	void BuildWalls()
	{

		// Reset Counts
		TeeMMUCount = 0;
		TeeUUMCount = 0;
		CorLotCount = 0;
		CorRotCount = 0;
		CorXolCount = 0;
		CorXorCount = 0;
		CorObtMCount = 0;
		CorObtUCount = 0;
		WhyUCount = 0;
		WhyMCount = 0;
		DoorPcCount = 0;
		Wall1MCount = 0;
		Wall2MCount = 0;
		Wall4MCount = 0;
		Wall2UCount = 0;
		Wall4UCount = 0;
		EndCapCount = 0;
		ColliderCount = 0;

		///--------------------
		// **** DOORS ***
		///--------------------
		_levelDoors = (GameObject)Instantiate( CONTAINER, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
		_levelDoors.tag = "LevelDoors";
		_levelDoors.name = "LEVEL_DOORS";
		
		// Instantiate each door in the list
		for ( int i=0 ; i<_doors.Count ; i++ )
		{
			BuildDoor ( _doors[i] );
		}

		///--------------------
		// **** WALLS ***
		///--------------------
		_levelWalls = (GameObject)Instantiate( CONTAINER, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
		_levelWalls.tag = "LevelWalls";
		_levelWalls.name = "LEVEL_WALLS";

		// Instantiate each piece in the list
		for ( int i=0 ; i<_pieces.Count ; i++ )
		{
			switch ( _pieces[i].type )
			{
				case WallType.TeeMMU:
					BuildTeeMMU ( _pieces[i] );
					break;
				case WallType.TeeUUM:
					BuildTeeUUM ( _pieces[i] );
					break;
				case WallType.CornerRot:
					BuildCornerRot ( _pieces[i] );
					break;
				case WallType.CornerLot:
					BuildCornerLot ( _pieces[i] );
					break;
				case WallType.CornerXol:
					BuildCornerXol ( _pieces[i] );
					break;
				case WallType.CornerXor:
					BuildCornerXor ( _pieces[i] );
					break;
				case WallType.CornerObtM:
					BuildCornerObtM ( _pieces[i] );
					break;
				case WallType.CornerObtU:
					BuildCornerObtU ( _pieces[i] );
					break;
				case WallType.WhyU:
					BuildWhyU ( _pieces[i] );
					break;
				case WallType.WhyM:
					BuildWhyM ( _pieces[i] );
					break;
				case WallType.Wall_1M:
					BuildWall1M ( _pieces[i] );
					break;
				case WallType.Wall_2M:
					BuildWall2M ( _pieces[i] );
					break;
				case WallType.Wall_4M:
					BuildWall4M ( _pieces[i] );
					break;
				case WallType.Wall_2U:
					BuildWall2U ( _pieces[i] );
					break;
				case WallType.Wall_4U:
					BuildWall4U ( _pieces[i] );
					break;
				case WallType.End_Cap:
					BuildEndCap ( _pieces[i] );
					break;
			}
		}

		///--------------------
		// **** END CAPS ***
		///--------------------
		// Instantiate each cap in the list
		for ( int i=0 ; i<_caps.Count ; i++ )
		{
			BuildEndCap ( _caps[i] );
		}

		///--------------------
		// **** COLLIDERS ***
		///--------------------
		// Create All Wall Colliders
		_levelColliders = (GameObject)Instantiate( CONTAINER, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
		_levelColliders.tag = "Wall";
		_levelColliders.name = "WALL_COLLIDERS";

		for ( int i=0 ; i<_colliders.Count ; i++ )
		{
			BuildCollider ( _colliders[i] );
		}
		
	}


	WallType ParseType(string i_type)
	{
		if ( i_type.Equals ( "TEE_MMU" ) )
		    return WallType.TeeMMU;
		else if ( i_type.Equals ( "TEE_UUM" ) )
			return WallType.TeeUUM;
		else if ( i_type.Equals ( "COR_90L" ) )
			return WallType.CornerLot;
		else if ( i_type.Equals ( "COR_90R" ) )
			return WallType.CornerRot;
		else if ( i_type.Equals ( "COR_XOL" ) )
			return WallType.CornerXol;
		else if ( i_type.Equals ( "COR_XOR" ) )
			return WallType.CornerXor;
		else if ( i_type.Equals ( "COR_OBM" ) )
			return WallType.CornerObtM;
		else if ( i_type.Equals ( "COR_OBU" ) )
			return WallType.CornerObtU;
		else if ( i_type.Equals ( "WHY_U" ) )
			return WallType.WhyU;
		else if ( i_type.Equals ( "WHY_M" ) )
			return WallType.WhyM;
		else if ( i_type.Equals ( "DOR_M" ) )
			return WallType.Door_M;
		else if ( i_type.Equals ( "DOR_U" ) )
			return WallType.Door_U;
		else if ( i_type.Equals ( "DOR_END" ) )
			return WallType.Door_End;
		else if ( i_type.Equals ( "DOR_STR" ) )
			return WallType.Door_Start;
		else if ( i_type.Equals ( "WAL_1M" ) )
			return WallType.Wall_1M;
		else if ( i_type.Equals ( "WAL_2M" ) )
			return WallType.Wall_2M;
		else if ( i_type.Equals ( "WAL_4M" ) )
			return WallType.Wall_4M;
		else if ( i_type.Equals ( "WAL_2U" ) )
			return WallType.Wall_2U;
		else if ( i_type.Equals ( "WAL_4U" ) )
			return WallType.Wall_4U;
		else if ( i_type.Equals ( "END_CAP" ) )
			return WallType.End_Cap;
		else
			return WallType.Wall_1M;
	}

	DoorPcType ParseDoorType(string i_type)
	{
		if ( i_type.Equals ( "StandardM" ) )
			return DoorPcType.StandardM;
		else if ( i_type.Equals ( "StandardU" ) )
			return DoorPcType.StandardU;
		else if ( i_type.Equals ( "End" ) )
			return DoorPcType.End;
		else if ( i_type.Equals ( "Start" ) )
			return DoorPcType.Start;
		else
			return DoorPcType.StandardM;
	}


	void RemoveOldWalls()
	{
		// Remove all Walls
		GameObject[] wallList = GameObject.FindGameObjectsWithTag("Wall");
		foreach ( GameObject wall in wallList )
		{
			DestroyImmediate(wall);
		}

		// Remove all Doors
		GameObject[] doorList = GameObject.FindGameObjectsWithTag("Door");
		foreach ( GameObject door in doorList )
		{
			DestroyImmediate(door);
		}

		// Remove Walls Containers
		GameObject Walls = GameObject.Find("LEVEL_WALLS");
		DestroyImmediate(Walls);

		// Remove Walls Containers
		GameObject Doors = GameObject.Find("LEVEL_DOORS");
		DestroyImmediate(Doors);

		// Remove Walls Containers
		GameObject Colliders = GameObject.Find("WALL_COLLIDERS");
		DestroyImmediate(Colliders);


		// Might also need to remove end doors and obstacles.
	}


	// Bulder helper methods

	#region BUILD CORNER 90 LEFT
	void BuildCornerLot( WallPiece i_piece)
	{
		// Create Wall Container Object
		List<GameObject> myPieces = new List<GameObject>();
		GameObject thisPARENT = (GameObject)Instantiate( CONTAINER, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
		thisPARENT.tag = "WallContainer";
		thisPARENT.name = "COR_LOT_" + CorLotCount;

		// Create SideA
		GameObject SideA = (GameObject)Instantiate( SIDE, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
		SideA.transform.Rotate(0,-45,0);
		SideA.name = "SideA";
		SideA.transform.parent = thisPARENT.transform;
		// Create SideB
		GameObject SideB = (GameObject)Instantiate( SIDE, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
		SideB.transform.Rotate(0,135,0);
		SideB.name = "SideB";
		SideB.transform.parent = thisPARENT.transform;


		// Build Base Pieces
		Debug.Log ("Build Corner");
		GameObject iCOR_LIN = (GameObject)Instantiate( COR_LIN, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
		iCOR_LIN.transform.parent = SideB.transform;
		myPieces.Add(iCOR_LIN);

		GameObject iCOR_LOT = (GameObject)Instantiate( COR_LOT, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
		iCOR_LOT.transform.parent = SideA.transform;
		myPieces.Add (iCOR_LOT);

		// Build Extension Pieces if Needed.
		if ( i_piece.long_M )
		{
			GameObject iEXT_ML = (GameObject)Instantiate( EXT_ML, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity );
			iEXT_ML.transform.Rotate(0,180,0);
			iEXT_ML.transform.parent = SideB.transform;
			myPieces.Add (iEXT_ML);
			GameObject iEXT_MR = (GameObject)Instantiate( EXT_MR, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity );
			iEXT_MR.transform.parent = SideA.transform;
			myPieces.Add (iEXT_MR);
		}

		if ( i_piece.long_U )
		{
			GameObject iEXT_UL = (GameObject)Instantiate( EXT_UL, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
			iEXT_UL.transform.parent = SideA.transform;
			myPieces.Add (iEXT_UL);
			GameObject iEXT_UR = (GameObject)Instantiate( EXT_UR, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
			iEXT_UR.transform.Rotate(0,180,0);
			iEXT_UR.transform.parent = SideB.transform;
			myPieces.Add (iEXT_UR);
		}
		 
		// Tag all pieces and add them to parent
		for ( int i = 0 ; i<myPieces.Count ; i++ )
		{
			myPieces[i].tag = "Wall";
		}

		// Translate and Rotate All Pieces
		thisPARENT.transform.Translate( i_piece.x, 0.0f, i_piece.z );
		thisPARENT.transform.Rotate(0.0f, i_piece.rot, 0.0f);

		thisPARENT.transform.parent = _levelWalls.transform;
		CorLotCount ++;
	}
	#endregion

	#region BUILD CORNER 90 RIGHT
	void BuildCornerRot( WallPiece i_piece)
	{
		// Create Wall Container Object
		List<GameObject> myPieces = new List<GameObject>();
		GameObject thisPARENT = (GameObject)Instantiate( CONTAINER, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
		thisPARENT.tag = "WallContainer";
		thisPARENT.name = "COR_ROT_" + CorRotCount;
		
		// Create SideA
		GameObject SideA = (GameObject)Instantiate( SIDE, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
		SideA.transform.Rotate(0,225,0);
		SideA.name = "SideA";
		SideA.transform.parent = thisPARENT.transform;
		// Create SideB
		GameObject SideB = (GameObject)Instantiate( SIDE, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
		SideB.transform.Rotate(0,45,0);
		SideB.name = "SideB";
		SideB.transform.parent = thisPARENT.transform;
		
		
		// Build Base Pieces
		Debug.Log ("Build Corner");
		GameObject iCOR_RIN = (GameObject)Instantiate( COR_RIN, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
		iCOR_RIN.transform.parent = SideB.transform;
		myPieces.Add(iCOR_RIN);
		
		GameObject iCOR_ROT = (GameObject)Instantiate( COR_ROT, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
		iCOR_ROT.transform.parent = SideA.transform;
		myPieces.Add (iCOR_ROT);
		
		// Build Extension Pieces if Needed.
		if ( i_piece.long_M )
		{
			GameObject iEXT_ML = (GameObject)Instantiate( EXT_ML, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity );
			iEXT_ML.transform.parent = SideA.transform;
			myPieces.Add (iEXT_ML);
			GameObject iEXT_MR = (GameObject)Instantiate( EXT_MR, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity );
			iEXT_MR.transform.Rotate(0,180,0);
			iEXT_MR.transform.parent = SideB.transform;
			myPieces.Add (iEXT_MR);
		}
		
		if ( i_piece.long_U )
		{
			GameObject iEXT_UL = (GameObject)Instantiate( EXT_UL, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
			iEXT_UL.transform.parent = SideB.transform;
			myPieces.Add (iEXT_UL);
			GameObject iEXT_UR = (GameObject)Instantiate( EXT_UR, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
			iEXT_UR.transform.Rotate(0,180,0);
			iEXT_UR.transform.parent = SideA.transform;
			myPieces.Add (iEXT_UR);
		}
		
		// Tag all pieces and add them to parent
		for ( int i = 0 ; i<myPieces.Count ; i++ )
		{
			myPieces[i].tag = "Wall";
		}
		
		// Translate and Rotate All Pieces
		thisPARENT.transform.Translate( i_piece.x, 0.0f, i_piece.z );
		thisPARENT.transform.Rotate(0.0f, i_piece.rot, 0.0f);
		
		thisPARENT.transform.parent = _levelWalls.transform;
		CorRotCount ++;
	}
	#endregion

	#region BUILD CORNER OBTUSE M
	void BuildCornerObtM( WallPiece i_piece)
	{
		// Create Wall Container Object
		List<GameObject> myPieces = new List<GameObject>();
		GameObject thisPARENT = (GameObject)Instantiate( CONTAINER, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
		thisPARENT.tag = "WallContainer";
		thisPARENT.name = "COR_OBT_" + CorObtMCount;
		
		// Create SideA
		GameObject SideA = (GameObject)Instantiate( SIDE, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
		SideA.transform.Rotate(0,-60,0);
		SideA.name = "SideA";
		SideA.transform.parent = thisPARENT.transform;
		// Create SideB
		GameObject SideB = (GameObject)Instantiate( SIDE, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
		SideB.transform.Rotate(0,120,0);
		SideB.name = "SideB";
		SideB.transform.parent = thisPARENT.transform;
		
		
		// Build Base Pieces
		Debug.Log ("Build Corner");
		GameObject iOBT_MIN = (GameObject)Instantiate( OBT_MIN, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
		iOBT_MIN.transform.parent = SideB.transform;
		myPieces.Add(iOBT_MIN);
		
		GameObject iOBT_MOT = (GameObject)Instantiate( OBT_MOT, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
		iOBT_MOT.transform.parent = SideA.transform;
		myPieces.Add (iOBT_MOT);

		if ( i_piece.long_M )// This refers to the dominant M or guide arm
		{
			GameObject iEXT_ML = (GameObject)Instantiate( EXT_ML, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity );
			iEXT_ML.transform.parent = SideB.transform;
			iEXT_ML.transform.Rotate(0,180,0);
			myPieces.Add (iEXT_ML);
			GameObject iEXT_MR = (GameObject)Instantiate( EXT_MR, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity );
			iEXT_MR.transform.parent = SideA.transform;
			myPieces.Add (iEXT_MR);
		}

		if ( i_piece.long_U )// This refers to the secondary M arm
		{
			GameObject iEXT_ML = (GameObject)Instantiate( EXT_ML, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity );
			iEXT_ML.transform.parent = SideA.transform;
			iEXT_ML.transform.Rotate(0,60,0);
			myPieces.Add (iEXT_ML);
			GameObject iEXT_MR = (GameObject)Instantiate( EXT_MR, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity );
			iEXT_MR.transform.parent = SideB.transform;
			iEXT_MR.transform.Rotate(0,240,0);
			myPieces.Add (iEXT_MR);
		}


		// Tag all pieces and add them to parent
		for ( int i = 0 ; i<myPieces.Count ; i++ )
		{
			myPieces[i].tag = "Wall";
		}
		
		// Translate and Rotate All Pieces
		thisPARENT.transform.Translate( i_piece.x, 0.0f, i_piece.z );
		thisPARENT.transform.Rotate(0.0f, i_piece.rot, 0.0f);
		
		thisPARENT.transform.parent = _levelWalls.transform;
		CorObtMCount ++;
	}
	#endregion

	#region BUILD CORNER OBTUSE U
	void BuildCornerObtU( WallPiece i_piece)
	{
		// Create Wall Container Object
		List<GameObject> myPieces = new List<GameObject>();
		GameObject thisPARENT = (GameObject)Instantiate( CONTAINER, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
		thisPARENT.tag = "WallContainer";
		thisPARENT.name = "COR_OBTU_" + CorObtUCount;
		
		// Create SideA
		GameObject SideA = (GameObject)Instantiate( SIDE, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
		SideA.transform.Rotate(0,210,0);
		SideA.name = "SideA";
		SideA.transform.parent = thisPARENT.transform;
		// Create SideB
		GameObject SideB = (GameObject)Instantiate( SIDE, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
		SideB.transform.Rotate(0,30,0);
		SideB.name = "SideB";
		SideB.transform.parent = thisPARENT.transform;
		
		
		// Build Base Pieces
		Debug.Log ("Build Corner");
		GameObject iOBT_UIN = (GameObject)Instantiate( OBT_UIN, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
		iOBT_UIN.transform.parent = SideB.transform;
		myPieces.Add(iOBT_UIN);
		
		GameObject iOBT_UOT = (GameObject)Instantiate( OBT_UOT, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
		iOBT_UOT.transform.parent = SideA.transform;
		myPieces.Add (iOBT_UOT);

		if ( i_piece.long_U ) // This is the dominant U or guide arm
		{
			GameObject iEXT_UL = (GameObject)Instantiate( EXT_UL, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
			iEXT_UL.transform.parent = SideB.transform;
			myPieces.Add (iEXT_UL);
			GameObject iEXT_UR = (GameObject)Instantiate( EXT_UR, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
			iEXT_UR.transform.Rotate(0,180,0);
			iEXT_UR.transform.parent = SideA.transform;
			myPieces.Add (iEXT_UR);
		}

		if ( i_piece.long_M ) // This is the secondary arm
		{
			GameObject iEXT_UL = (GameObject)Instantiate( EXT_UL, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
			iEXT_UL.transform.parent = SideA.transform;
			iEXT_UL.transform.Rotate(0,240,0);
			myPieces.Add (iEXT_UL);
			GameObject iEXT_UR = (GameObject)Instantiate( EXT_UR, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
			iEXT_UR.transform.Rotate(0,60,0);
			iEXT_UR.transform.parent = SideB.transform;
			myPieces.Add (iEXT_UR);
		}

		// Tag all pieces and add them to parent
		for ( int i = 0 ; i<myPieces.Count ; i++ )
		{
			myPieces[i].tag = "Wall";
		}
		
		// Translate and Rotate All Pieces
		thisPARENT.transform.Translate( i_piece.x, 0.0f, i_piece.z );
		thisPARENT.transform.Rotate(0.0f, i_piece.rot, 0.0f);
		
		thisPARENT.transform.parent = _levelWalls.transform;
		CorObtUCount ++;
	}
	#endregion

	#region BUILD CORNER EXTRA OBTUSE LEFT
	void BuildCornerXol( WallPiece i_piece)
	{
		// Create Wall Container Object
		List<GameObject> myPieces = new List<GameObject>();
		GameObject thisPARENT = (GameObject)Instantiate( CONTAINER, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
		thisPARENT.tag = "WallContainer";
		thisPARENT.name = "COR_XOL_" + CorXolCount;
		
		// Create SideA
		GameObject SideA = (GameObject)Instantiate( SIDE, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
		SideA.transform.Rotate(0,-30,0);
		SideA.name = "SideA";
		SideA.transform.parent = thisPARENT.transform;
		// Create SideB
		GameObject SideB = (GameObject)Instantiate( SIDE, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
		SideB.transform.Rotate(0,150,0);
		SideB.name = "SideB";
		SideB.transform.parent = thisPARENT.transform;
		
		
		// Build Base Pieces
		Debug.Log ("Build Corner");
		GameObject iXOB_LIN = (GameObject)Instantiate( XOB_LIN, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
		iXOB_LIN.transform.parent = SideB.transform;
		myPieces.Add(iXOB_LIN);
		
		GameObject iXOB_LOT = (GameObject)Instantiate( XOB_LOT, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
		iXOB_LOT.transform.parent = SideA.transform;
		myPieces.Add (iXOB_LOT);
		
		// Build Extension Pieces if Needed.
		if ( i_piece.long_M )
		{
			GameObject iEXT_ML = (GameObject)Instantiate( EXT_ML, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity );
			iEXT_ML.transform.Rotate(0,240,0);
			iEXT_ML.transform.parent = SideB.transform;
			myPieces.Add (iEXT_ML);
			GameObject iEXT_MR = (GameObject)Instantiate( EXT_MR, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity );
			iEXT_MR.transform.Rotate(0,60,0);
			iEXT_MR.transform.parent = SideA.transform;
			myPieces.Add (iEXT_MR);
		}
		
		if ( i_piece.long_U )
		{
			GameObject iEXT_UL = (GameObject)Instantiate( EXT_UL, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
			iEXT_UL.transform.parent = SideA.transform;
			myPieces.Add (iEXT_UL);
			GameObject iEXT_UR = (GameObject)Instantiate( EXT_UR, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
			iEXT_UR.transform.Rotate(0,180,0);
			iEXT_UR.transform.parent = SideB.transform;
			myPieces.Add (iEXT_UR);
		}
		
		// Tag all pieces and add them to parent
		for ( int i = 0 ; i<myPieces.Count ; i++ )
		{
			myPieces[i].tag = "Wall";
		}
		
		// Translate and Rotate All Pieces
		thisPARENT.transform.Translate( i_piece.x, 0.0f, i_piece.z );
		thisPARENT.transform.Rotate(0.0f, i_piece.rot, 0.0f);
		
		thisPARENT.transform.parent = _levelWalls.transform;
		CorXolCount ++;
	}
	#endregion

	#region BUILD CORNER EXTRA OBTUSE RIGHT
	void BuildCornerXor( WallPiece i_piece)
	{
		// Create Wall Container Object
		List<GameObject> myPieces = new List<GameObject>();
		GameObject thisPARENT = (GameObject)Instantiate( CONTAINER, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
		thisPARENT.tag = "WallContainer";
		thisPARENT.name = "COR_XOL_" + CorXolCount;
		
		// Create SideA
		GameObject SideA = (GameObject)Instantiate( SIDE, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
		SideA.transform.Rotate(0,30,0);
		SideA.name = "SideA";
		SideA.transform.parent = thisPARENT.transform;
		// Create SideB
		GameObject SideB = (GameObject)Instantiate( SIDE, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
		SideB.transform.Rotate(0,210,0);
		SideB.name = "SideB";
		SideB.transform.parent = thisPARENT.transform;
		
		
		// Build Base Pieces
		Debug.Log ("Build Corner");
		GameObject iXOB_RIN = (GameObject)Instantiate( XOB_RIN, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
		iXOB_RIN.transform.parent = SideB.transform;
		myPieces.Add(iXOB_RIN);
		
		GameObject iXOB_ROT = (GameObject)Instantiate( XOB_ROT, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
		iXOB_ROT.transform.parent = SideA.transform;
		myPieces.Add (iXOB_ROT);
		
		// Build Extension Pieces if Needed.
		if ( i_piece.long_M )
		{
			GameObject iEXT_ML = (GameObject)Instantiate( EXT_ML, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity );
			iEXT_ML.transform.Rotate(0,300,0);
			iEXT_ML.transform.parent = SideA.transform;
			myPieces.Add (iEXT_ML);
			GameObject iEXT_MR = (GameObject)Instantiate( EXT_MR, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity );
			iEXT_MR.transform.Rotate(0,120,0);
			iEXT_MR.transform.parent = SideB.transform;
			myPieces.Add (iEXT_MR);
		}
		
		if ( i_piece.long_U )
		{
			GameObject iEXT_UL = (GameObject)Instantiate( EXT_UL, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
			iEXT_UL.transform.parent = SideB.transform;
			myPieces.Add (iEXT_UL);
			GameObject iEXT_UR = (GameObject)Instantiate( EXT_UR, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
			iEXT_UR.transform.Rotate(0,180,0);
			iEXT_UR.transform.parent = SideA.transform;
			myPieces.Add (iEXT_UR);
		}
		
		// Tag all pieces and add them to parent
		for ( int i = 0 ; i<myPieces.Count ; i++ )
		{
			myPieces[i].tag = "Wall";
		}
		
		// Translate and Rotate All Pieces
		thisPARENT.transform.Translate( i_piece.x, 0.0f, i_piece.z );
		thisPARENT.transform.Rotate(0.0f, i_piece.rot, 0.0f);
		
		thisPARENT.transform.parent = _levelWalls.transform;
		CorXolCount ++;
	}
	#endregion

	#region BUILD TEE PIECE MMU
	void BuildTeeMMU( WallPiece i_piece)
	{
		// Create Wall Container Object
		List<GameObject> myPieces = new List<GameObject>();
		GameObject thisPARENT = (GameObject)Instantiate( CONTAINER, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
		thisPARENT.tag = "WallContainer";
		thisPARENT.name = "TEE_MMU_" + TeeMMUCount;
		
		// Create SideA
		GameObject SideA = (GameObject)Instantiate( SIDE, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
		SideA.transform.Rotate(0,150,0);
		SideA.name = "SideA";
		SideA.transform.parent = thisPARENT.transform;
		// Create SideB
		GameObject SideB = (GameObject)Instantiate( SIDE, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
		SideB.transform.Rotate(0,30,0);
		SideB.name = "SideB";
		SideB.transform.parent = thisPARENT.transform;
		// Create SideC
		GameObject SideC = (GameObject)Instantiate( SIDE, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
		SideC.transform.Rotate(0,-90,0);
		SideC.name = "SideC";
		SideC.transform.parent = thisPARENT.transform;
		
		
		// Build Base Pieces
		Debug.Log ("Build Corner");
		GameObject iCOR_RIN = (GameObject)Instantiate( COR_RIN, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
		iCOR_RIN.transform.parent = SideB.transform;
		myPieces.Add(iCOR_RIN); // right inner corner
		
		GameObject iCOR_LIN = (GameObject)Instantiate( COR_LIN, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
		iCOR_LIN.transform.parent = SideA.transform;
		myPieces.Add (iCOR_LIN); // Left inner corner

		GameObject iVAL_1M = (GameObject)Instantiate( VAL_1M, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
		iVAL_1M.transform.parent = SideC.transform;
		myPieces.Add (iVAL_1M); // back piece
	
		
		// Build Extension Pieces if Needed.
		if ( i_piece.long_M )
		{
			GameObject iEXT_ML = (GameObject)Instantiate( EXT_ML, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity );
			iEXT_ML.transform.parent = SideC.transform;
			myPieces.Add (iEXT_ML);

			GameObject iEXT_MR = (GameObject)Instantiate( EXT_MR, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity );
			iEXT_MR.transform.Rotate(0,180,0);
			iEXT_MR.transform.parent = SideB.transform;
			myPieces.Add (iEXT_MR);
		}

		if ( i_piece.extLong )
		{
			GameObject iEXT_ML = (GameObject)Instantiate( EXT_ML, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity );
			iEXT_ML.transform.Rotate(0,180,0);
			iEXT_ML.transform.parent = SideA.transform;
			myPieces.Add (iEXT_ML);
			
			GameObject iEXT_MR = (GameObject)Instantiate( EXT_MR, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity );
			iEXT_MR.transform.parent = SideC.transform;
			myPieces.Add (iEXT_MR);
		}

		if ( i_piece.long_U )
		{
			GameObject iEXT_UL = (GameObject)Instantiate( EXT_UL, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
			iEXT_UL.transform.parent = SideB.transform;
			myPieces.Add (iEXT_UL);

			GameObject iEXT_UR = (GameObject)Instantiate( EXT_UR, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
			iEXT_UR.transform.Rotate(0,180,0);
			iEXT_UR.transform.parent = SideA.transform;
			myPieces.Add (iEXT_UR);
		}
		
		// Tag all pieces and add them to parent
		for ( int i = 0 ; i<myPieces.Count ; i++ )
		{
			myPieces[i].tag = "Wall";
		}
		
		// Translate and Rotate All Pieces
		thisPARENT.transform.Translate( i_piece.x, 0.0f, i_piece.z );
		thisPARENT.transform.Rotate(0.0f, i_piece.rot, 0.0f);
		
		thisPARENT.transform.parent = _levelWalls.transform;
		TeeMMUCount ++;
	}
	#endregion

	#region BUILD TEE PIECE UUM
	void BuildTeeUUM(WallPiece i_piece)
	{
		// Create Wall Container Object
		List<GameObject> myPieces = new List<GameObject>();
		GameObject thisPARENT = (GameObject)Instantiate( CONTAINER, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
		thisPARENT.tag = "WallContainer";
		thisPARENT.name = "TEE_UUM_" + TeeUUMCount;
		
		// Create SideA
		GameObject SideA = (GameObject)Instantiate( SIDE, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
		SideA.transform.Rotate(0,60,0);
		SideA.name = "SideA";
		SideA.transform.parent = thisPARENT.transform;
		// Create SideB
		GameObject SideB = (GameObject)Instantiate( SIDE, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
		SideB.transform.Rotate(0,-60,0);
		SideB.name = "SideB";
		SideB.transform.parent = thisPARENT.transform;
		// Create SideC
		GameObject SideC = (GameObject)Instantiate( SIDE, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
		SideC.transform.Rotate(0,180,0);
		SideC.name = "SideC";
		SideC.transform.parent = thisPARENT.transform;
		
		
		// Build Base Pieces
		GameObject iCOR_RIN = (GameObject)Instantiate( COR_RIN, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
		iCOR_RIN.transform.parent = SideA.transform;
		myPieces.Add(iCOR_RIN);
		
		GameObject iCOR_LIN = (GameObject)Instantiate( COR_LIN, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
		iCOR_LIN.transform.Rotate(0,180,0);
		iCOR_LIN.transform.parent = SideB.transform;
		myPieces.Add (iCOR_LIN);

		GameObject iVAL_2U = (GameObject)Instantiate( VAL_2U, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
		iVAL_2U.transform.parent = SideC.transform;
		myPieces.Add (iVAL_2U); // back piece
		
		
		// Build Extension Pieces if Needed.
		if ( i_piece.long_M )
		{	
			GameObject iEXT_ML = (GameObject)Instantiate( EXT_ML, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity );
			//iEXT_ML.transform.Rotate(0,180,0);
			iEXT_ML.transform.parent = SideB.transform;
			myPieces.Add (iEXT_ML);
			
			GameObject iEXT_MR = (GameObject)Instantiate( EXT_MR, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity );
			iEXT_MR.transform.Rotate(0,180,0);
			iEXT_MR.transform.parent = SideA.transform;
			myPieces.Add (iEXT_MR);
		}
		
		if ( i_piece.long_U )
		{
			GameObject iEXT_UL = (GameObject)Instantiate( EXT_UL, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
			iEXT_UL.transform.Rotate(0,180,0);
			iEXT_UL.transform.parent = SideC.transform;
			myPieces.Add (iEXT_UL);

			GameObject iEXT_UR = (GameObject)Instantiate( EXT_UR, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
			iEXT_UR.transform.parent = SideB.transform;
			myPieces.Add (iEXT_UR);
		}


		if ( i_piece.extLong )
		{
			GameObject iEXT_UL = (GameObject)Instantiate( EXT_UL, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
			iEXT_UL.transform.parent = SideA.transform;
			myPieces.Add (iEXT_UL);
			
			GameObject iEXT_UR = (GameObject)Instantiate( EXT_UR, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
			iEXT_UR.transform.Rotate(0,180,0);
			iEXT_UR.transform.parent = SideC.transform;
			myPieces.Add (iEXT_UR);
		}

		// Tag all pieces and add them to parent
		for ( int i = 0 ; i<myPieces.Count ; i++ )
		{
			myPieces[i].tag = "Wall";
		}
		
		// Translate and Rotate All Pieces
		thisPARENT.transform.Translate( i_piece.x, 0.0f, i_piece.z );
		thisPARENT.transform.Rotate(0.0f, i_piece.rot, 0.0f);
		
		thisPARENT.transform.parent = _levelWalls.transform;
		TeeUUMCount ++;
	}
	#endregion

	#region BUILD WHY U
	void BuildWhyU( WallPiece i_piece)
	{
		// Create Wall Container Object
		List<GameObject> myPieces = new List<GameObject>();
		GameObject thisPARENT = (GameObject)Instantiate( CONTAINER, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
		thisPARENT.tag = "WallContainer";
		thisPARENT.name = "WHY_M_" + WhyMCount;
		
		// Create SideA
		GameObject SideA = (GameObject)Instantiate( SIDE, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
		SideA.transform.Rotate(0,30,0);
		SideA.name = "SideA";
		SideA.transform.parent = thisPARENT.transform;
		// Create SideB
		GameObject SideB = (GameObject)Instantiate( SIDE, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
		SideB.transform.Rotate(0,150,0);
		SideB.name = "SideB";
		SideB.transform.parent = thisPARENT.transform;
		// Create SideC
		GameObject SideC = (GameObject)Instantiate( SIDE, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
		SideC.transform.Rotate(0,270,0);
		SideC.name = "SideC";
		SideC.transform.parent = thisPARENT.transform;
		
		
		// Build Base Pieces
		Debug.Log ("Build Corner");
		GameObject iOBT_UIN = (GameObject)Instantiate( OBT_UIN, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
		iOBT_UIN.transform.parent = SideA.transform;
		myPieces.Add(iOBT_UIN);
		
		GameObject iOBT_UIN2 = (GameObject)Instantiate( OBT_UIN, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
		iOBT_UIN2.transform.Rotate(0,120,0);
		iOBT_UIN2.transform.parent = SideB.transform;
		myPieces.Add (iOBT_UIN2);
		
		GameObject iOBT_UIN3 = (GameObject)Instantiate( OBT_UIN, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
		iOBT_UIN3.transform.Rotate(0,240,0);
		iOBT_UIN3.transform.parent = SideC.transform;
		myPieces.Add (iOBT_UIN3);
		
		if ( i_piece.long_U )
		{	
			// *** First Pair ( Upper Left Branch)
			GameObject iEXT_UL = (GameObject)Instantiate( EXT_UL, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity );
			iEXT_UL.transform.Rotate(0,120,0);
			iEXT_UL.transform.parent = SideB.transform;
			myPieces.Add (iEXT_UL);
			
			GameObject iEXT_UR = (GameObject)Instantiate( EXT_UR, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity );
			iEXT_UR.transform.Rotate(0,300,0);
			iEXT_UR.transform.parent = SideC.transform;
			myPieces.Add (iEXT_UR);

			// ***Second Pair ( Upper Right Branch )
			GameObject iEXT_UL2 = (GameObject)Instantiate( EXT_UL, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity );
			iEXT_UL2.transform.Rotate(0,240,0);
			iEXT_UL2.transform.parent = SideC.transform;
			myPieces.Add (iEXT_UL2);
			
			GameObject iEXT_UR2 = (GameObject)Instantiate( EXT_UR, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity );
			iEXT_UR2.transform.Rotate(0,60,0);
			iEXT_UR2.transform.parent = SideA.transform;
			myPieces.Add (iEXT_UR2);

			// ***Third Pair ( Bottom Branch )
			GameObject iEXT_UL3 = (GameObject)Instantiate( EXT_UL, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity );
			iEXT_UL3.transform.Rotate(0,0,0);
			iEXT_UL3.transform.parent = SideA.transform;
			myPieces.Add (iEXT_UL3);
			
			GameObject iEXT_UR3 = (GameObject)Instantiate( EXT_UR, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity );
			iEXT_UR3.transform.Rotate(0,180,0);
			iEXT_UR3.transform.parent = SideB.transform;
			myPieces.Add (iEXT_UR3);
		}
		
		
		// Tag all pieces and add them to parent
		for ( int i = 0 ; i<myPieces.Count ; i++ )
		{
			myPieces[i].tag = "Wall";
		}
		
		// Translate and Rotate All Pieces
		thisPARENT.transform.Translate( i_piece.x, 0.0f, i_piece.z );
		thisPARENT.transform.Rotate(0.0f, i_piece.rot, 0.0f);
		
		thisPARENT.transform.parent = _levelWalls.transform;
		WhyMCount ++;
	}
	#endregion

	#region BUILD WHY M
	void BuildWhyM ( WallPiece i_piece)
	{
		// Create Wall Container Object
		List<GameObject> myPieces = new List<GameObject>();
		GameObject thisPARENT = (GameObject)Instantiate( CONTAINER, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
		thisPARENT.tag = "WallContainer";
		thisPARENT.name = "WHY_M_" + WhyMCount;
		
		// Create SideA
		GameObject SideA = (GameObject)Instantiate( SIDE, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
		SideA.transform.Rotate(0,0,0);
		SideA.name = "SideA";
		SideA.transform.parent = thisPARENT.transform;
		// Create SideB
		GameObject SideB = (GameObject)Instantiate( SIDE, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
		SideB.transform.Rotate(0,120,0);
		SideB.name = "SideB";
		SideB.transform.parent = thisPARENT.transform;
		// Create SideC
		GameObject SideC = (GameObject)Instantiate( SIDE, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
		SideC.transform.Rotate(0,240,0);
		SideC.name = "SideC";
		SideC.transform.parent = thisPARENT.transform;
		
		
		// Build Base Pieces
		Debug.Log ("Build Corner");
		GameObject iOBT_MIN = (GameObject)Instantiate( OBT_MIN, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
		iOBT_MIN.transform.parent = SideB.transform;
		myPieces.Add(iOBT_MIN);
		
		GameObject iOBT_MIN2 = (GameObject)Instantiate( OBT_MIN, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
		iOBT_MIN2.transform.Rotate(0,120,0);
		iOBT_MIN2.transform.parent = SideC.transform;
		myPieces.Add (iOBT_MIN2);

		GameObject iOBT_MIN3 = (GameObject)Instantiate( OBT_MIN, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
		iOBT_MIN3.transform.Rotate(0,240,0);
		iOBT_MIN3.transform.parent = SideA.transform;
		myPieces.Add (iOBT_MIN3);
		
		if ( i_piece.long_M )
		{	
			// *** First Pair
			GameObject iEXT_ML = (GameObject)Instantiate( EXT_ML, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity );
			iEXT_ML.transform.Rotate(0,180,0);
			iEXT_ML.transform.parent = SideB.transform;
			myPieces.Add (iEXT_ML);
			
			GameObject iEXT_MR = (GameObject)Instantiate( EXT_MR, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity );
			//iEXT_MR.transform.Rotate(0,180,0);
			iEXT_MR.transform.parent = SideC.transform;
			myPieces.Add (iEXT_MR);

			// ***Second Pair
			GameObject iEXT_ML2 = (GameObject)Instantiate( EXT_ML, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity );
			iEXT_ML2.transform.Rotate(0,300,0);
			iEXT_ML2.transform.parent = SideC.transform;
			myPieces.Add (iEXT_ML2);
			
			GameObject iEXT_MR2 = (GameObject)Instantiate( EXT_MR, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity );
			iEXT_MR2.transform.Rotate(0,120,0);
			iEXT_MR2.transform.parent = SideA.transform;
			myPieces.Add (iEXT_MR2);

			// ***Third Pair
			GameObject iEXT_ML3 = (GameObject)Instantiate( EXT_ML, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity );
			iEXT_ML3.transform.Rotate(0,60,0);
			iEXT_ML3.transform.parent = SideA.transform;
			myPieces.Add (iEXT_ML2);
			
			GameObject iEXT_MR3 = (GameObject)Instantiate( EXT_MR, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity );
			iEXT_MR3.transform.Rotate(0,240,0);
			iEXT_MR3.transform.parent = SideB.transform;
			myPieces.Add (iEXT_MR2);
		}


		// Tag all pieces and add them to parent
		for ( int i = 0 ; i<myPieces.Count ; i++ )
		{
			myPieces[i].tag = "Wall";
		}
		
		// Translate and Rotate All Pieces
		thisPARENT.transform.Translate( i_piece.x, 0.0f, i_piece.z );
		thisPARENT.transform.Rotate(0.0f, i_piece.rot, 0.0f);
		
		thisPARENT.transform.parent = _levelWalls.transform;
		WhyMCount ++;
	}
	#endregion	

	#region BUILD WALL 1M
	void BuildWall1M( WallPiece i_piece)
	{
		// Create Wall Container Object
		List<GameObject> myPieces = new List<GameObject>();
		GameObject thisPARENT = (GameObject)Instantiate( CONTAINER, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
		thisPARENT.tag = "WallContainer";
		thisPARENT.name = "VAL_1M_" + Wall1MCount;
		
		// Create SideA
		GameObject SideA = (GameObject)Instantiate( SIDE, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
		SideA.transform.Rotate(0,90,0);
		SideA.name = "SideA";
		SideA.transform.parent = thisPARENT.transform;
		// Create SideB
		GameObject SideB = (GameObject)Instantiate( SIDE, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
		SideB.transform.Rotate(0,270,0);
		SideB.name = "SideB";
		SideB.transform.parent = thisPARENT.transform;
		
		
		// Build Base Pieces
		GameObject iVAL_1M_1 = (GameObject)Instantiate( VAL_1M, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
		iVAL_1M_1.transform.parent = SideB.transform;
		myPieces.Add(iVAL_1M_1);
		
		GameObject iVAL_1M_2 = (GameObject)Instantiate( VAL_1M, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
		iVAL_1M_2.transform.Rotate(0,180,0);
		iVAL_1M_2.transform.parent = SideA.transform;
		myPieces.Add (iVAL_1M_2);

		// Tag all pieces and add them to parent
		for ( int i = 0 ; i<myPieces.Count ; i++ )
		{
			myPieces[i].tag = "Wall";
		}
		
		// Translate and Rotate All Pieces
		thisPARENT.transform.Translate( i_piece.x, 0.0f, i_piece.z );
		thisPARENT.transform.Rotate(0.0f, i_piece.rot, 0.0f);
		
		thisPARENT.transform.parent = _levelWalls.transform;
		Wall1MCount ++;
	}
	#endregion

	#region BUILD WALL 2M
	void BuildWall2M( WallPiece i_piece)
	{
		// Create Wall Container Object
		List<GameObject> myPieces = new List<GameObject>();
		GameObject thisPARENT = (GameObject)Instantiate( CONTAINER, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
		thisPARENT.tag = "WallContainer";
		thisPARENT.name = "VAL_2M_" + Wall2MCount;
		
		// Create SideA
		GameObject SideA = (GameObject)Instantiate( SIDE, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
		SideA.transform.Rotate(0,90,0);
		SideA.name = "SideA";
		SideA.transform.parent = thisPARENT.transform;
		// Create SideB
		GameObject SideB = (GameObject)Instantiate( SIDE, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
		SideB.transform.Rotate(0,270,0);
		SideB.name = "SideB";
		SideB.transform.parent = thisPARENT.transform;
		
		
		// Build Base Pieces
		GameObject iVAL_2M_1 = (GameObject)Instantiate( VAL_2M, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
		iVAL_2M_1.transform.parent = SideB.transform;
		myPieces.Add(iVAL_2M_1);
		
		GameObject iVAL_2M_2 = (GameObject)Instantiate( VAL_2M, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
		iVAL_2M_2.transform.Rotate(0,180,0);
		iVAL_2M_2.transform.parent = SideA.transform;
		myPieces.Add (iVAL_2M_2);
		
		// Tag all pieces and add them to parent
		for ( int i = 0 ; i<myPieces.Count ; i++ )
		{
			myPieces[i].tag = "Wall";
		}
		
		// Translate and Rotate All Pieces
		thisPARENT.transform.Translate( i_piece.x, 0.0f, i_piece.z );
		thisPARENT.transform.Rotate(0.0f, i_piece.rot, 0.0f);
		
		thisPARENT.transform.parent = _levelWalls.transform;
		Wall2MCount ++;
	}
	#endregion

	#region BUILD WALL 4M
	void BuildWall4M( WallPiece i_piece)
	{
		// Create Wall Container Object
		List<GameObject> myPieces = new List<GameObject>();
		GameObject thisPARENT = (GameObject)Instantiate( CONTAINER, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
		thisPARENT.tag = "WallContainer";
		thisPARENT.name = "VAL_4M_" + Wall4MCount;
		
		// Create SideA
		GameObject SideA = (GameObject)Instantiate( SIDE, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
		SideA.transform.Rotate(0,90,0);
		SideA.name = "SideA";
		SideA.transform.parent = thisPARENT.transform;
		// Create SideB
		GameObject SideB = (GameObject)Instantiate( SIDE, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
		SideB.transform.Rotate(0,270,0);
		SideB.name = "SideB";
		SideB.transform.parent = thisPARENT.transform;
		
		
		// Build Base Pieces
		GameObject iVAL_4M_1 = (GameObject)Instantiate( VAL_4M, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
		iVAL_4M_1.transform.parent = SideB.transform;
		myPieces.Add(iVAL_4M_1);
		
		GameObject iVAL_4M_2 = (GameObject)Instantiate( VAL_4M, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
		iVAL_4M_2.transform.Rotate(0,180,0);
		iVAL_4M_2.transform.parent = SideA.transform;
		myPieces.Add (iVAL_4M_2);
		
		// Tag all pieces and add them to parent
		for ( int i = 0 ; i<myPieces.Count ; i++ )
		{
			myPieces[i].tag = "Wall";
		}
		
		// Translate and Rotate All Pieces
		thisPARENT.transform.Translate( i_piece.x, 0.0f, i_piece.z );
		thisPARENT.transform.Rotate(0.0f, i_piece.rot, 0.0f);
		
		thisPARENT.transform.parent = _levelWalls.transform;
		Wall4MCount ++;
	}
	#endregion

	#region BUILD WALL 2U
	void BuildWall2U( WallPiece i_piece)
	{
		// Create Wall Container Object
		List<GameObject> myPieces = new List<GameObject>();
		GameObject thisPARENT = (GameObject)Instantiate( CONTAINER, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
		thisPARENT.tag = "WallContainer";
		thisPARENT.name = "VAL_2U_" + Wall2UCount;
		
		// Create SideA
		GameObject SideA = (GameObject)Instantiate( SIDE, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
		SideA.transform.Rotate(0,0,0);
		SideA.name = "SideA";
		SideA.transform.parent = thisPARENT.transform;
		// Create SideB
		GameObject SideB = (GameObject)Instantiate( SIDE, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
		SideB.transform.Rotate(0,180,0);
		SideB.name = "SideB";
		SideB.transform.parent = thisPARENT.transform;
		
		
		// Build Base Pieces
		GameObject iVAL_2U_1 = (GameObject)Instantiate( VAL_2U, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
		iVAL_2U_1.transform.parent = SideB.transform;
		myPieces.Add(iVAL_2U_1);
		
		GameObject iVAL_2U_2 = (GameObject)Instantiate( VAL_2U, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
		iVAL_2U_2.transform.Rotate(0,180,0);
		iVAL_2U_2.transform.parent = SideA.transform;
		myPieces.Add (iVAL_2U_2);
		
		// Tag all pieces and add them to parent
		for ( int i = 0 ; i<myPieces.Count ; i++ )
		{
			myPieces[i].tag = "Wall";
		}
		
		// Translate and Rotate All Pieces
		thisPARENT.transform.Translate( i_piece.x, 0.0f, i_piece.z );
		thisPARENT.transform.Rotate(0.0f, i_piece.rot, 0.0f);
		
		thisPARENT.transform.parent = _levelWalls.transform;
		Wall2UCount ++;
	}
	#endregion

	#region BUILD WALL 4U
	void BuildWall4U( WallPiece i_piece)
	{
		// Create Wall Container Object
		List<GameObject> myPieces = new List<GameObject>();
		GameObject thisPARENT = (GameObject)Instantiate( CONTAINER, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
		thisPARENT.tag = "WallContainer";
		thisPARENT.name = "VAL_4U_" + Wall4UCount;
		
		// Create SideA
		GameObject SideA = (GameObject)Instantiate( SIDE, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
		SideA.transform.Rotate(0,0,0);
		SideA.name = "SideA";
		SideA.transform.parent = thisPARENT.transform;
		// Create SideB
		GameObject SideB = (GameObject)Instantiate( SIDE, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
		SideB.transform.Rotate(0,180,0);
		SideB.name = "SideB";
		SideB.transform.parent = thisPARENT.transform;
		
		
		// Build Base Pieces
		GameObject iVAL_4U_1 = (GameObject)Instantiate( VAL_4U, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
		iVAL_4U_1.transform.parent = SideB.transform;
		myPieces.Add(iVAL_4U_1);
		
		GameObject iVAL_4U_2 = (GameObject)Instantiate( VAL_4U, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
		iVAL_4U_2.transform.Rotate(0,180,0);
		iVAL_4U_2.transform.parent = SideA.transform;
		myPieces.Add (iVAL_4U_2);
		
		// Tag all pieces and add them to parent
		for ( int i = 0 ; i<myPieces.Count ; i++ )
		{
			myPieces[i].tag = "Wall";
		}
		
		// Translate and Rotate All Pieces
		thisPARENT.transform.Translate( i_piece.x, 0.0f, i_piece.z );
		thisPARENT.transform.Rotate(0.0f, i_piece.rot, 0.0f);
		
		thisPARENT.transform.parent = _levelWalls.transform;
		Wall4UCount ++;
	}
	#endregion


	#region BUILD END CAP
	void BuildEndCap( WallPiece i_piece)
	{
		// Create Wall Container Object
		List<GameObject> myPieces = new List<GameObject>();
		GameObject thisPARENT = (GameObject)Instantiate( CONTAINER, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
		thisPARENT.tag = "WallContainer";
		thisPARENT.name = "END_CAP_" + EndCapCount;
		
		// Create SideA
		GameObject SideA = (GameObject)Instantiate( SIDE, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity );
		SideA.transform.Rotate(0,0,0);
		SideA.name = "SideA";
		SideA.transform.parent = thisPARENT.transform;
		// Create SideB
		GameObject SideB = (GameObject)Instantiate( SIDE, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity );
		SideB.transform.Rotate(0,180,0);
		SideB.name = "SideB";
		SideB.transform.parent = thisPARENT.transform;	
		
		// Build Base Pieces
		GameObject iEND_CAP = (GameObject)Instantiate( END_CAP, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity );
		iEND_CAP.transform.Rotate(0,180,0);
		iEND_CAP.transform.parent = SideA.transform;
		myPieces.Add (iEND_CAP);

		if ( i_piece.long_U )
		{
			GameObject iEXT_CUL = (GameObject)Instantiate( EXT_CUL, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
			iEXT_CUL.transform.parent = SideB.transform;
			iEXT_CUL.transform.Rotate(0,180,0);
			myPieces.Add (iEXT_CUL);
			
			GameObject iEXT_CUR = (GameObject)Instantiate( EXT_CUR, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
			iEXT_CUR.transform.parent = SideA.transform;
			myPieces.Add (iEXT_CUR);
		}

		// Tag all pieces and add them to parent
		for ( int i = 0 ; i<myPieces.Count ; i++ )
		{
			myPieces[i].tag = "Wall";
		}
		
		// Translate and Rotate All Pieces
		thisPARENT.transform.Translate( i_piece.x, 0.0f, i_piece.z );
		thisPARENT.transform.Rotate(0.0f, i_piece.rot, 0.0f);
		
		thisPARENT.transform.parent = _levelWalls.transform;
		EndCapCount ++;

	}
	#endregion


	void BuildDoor( DoorPiece i_door)
	{
		if ( i_door.type == DoorPcType.StandardM )
		{
			// Create Door GameObject
			GameObject iDOR_2M = PrefabUtility.InstantiatePrefab( DOR_2M as GameObject ) as GameObject;
			iDOR_2M.transform.parent = _levelDoors.transform;
			iDOR_2M.name = "DOR_2M_" + DoorPcCount;

			// Rotate and translate piece into place.
			iDOR_2M.transform.Translate( i_door.x, 0.0f, i_door.z );
			iDOR_2M.transform.Rotate(0.0f, i_door.rot, 0.0f);

			DoorPcCount ++;
		}
		else if ( i_door.type == DoorPcType.StandardU )
		{
			// Create Door GameObject
			GameObject iDOR_2U = PrefabUtility.InstantiatePrefab( DOR_2U as GameObject ) as GameObject;
			iDOR_2U.transform.parent = _levelDoors.transform;
			iDOR_2U.name = "DOR_2U_" + DoorPcCount;
			
			// Rotate and translate piece into place.
			iDOR_2U.transform.Translate( i_door.x, 0.0f, i_door.z );
			iDOR_2U.transform.Rotate(0.0f, i_door.rot, 0.0f);
			
			DoorPcCount ++;

		}
		else if ( i_door.type == DoorPcType.End )
		{
			// Create Door GameObject
			//GameObject iDOR_END = (GameObject)Instantiate( DOR_END, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
			GameObject iDOR_END = PrefabUtility.InstantiatePrefab( DOR_END as GameObject ) as GameObject;
			iDOR_END.transform.parent = _levelDoors.transform;
			iDOR_END.name = "DOR_END";
			
			// Rotate and translate piece into place.
			iDOR_END.transform.Translate( i_door.x, 0.0f, i_door.z );
			iDOR_END.transform.Rotate(0.0f, i_door.rot, 0.0f);

		}
		else if ( i_door.type == DoorPcType.Start )
		{
			// Create Door GameObject
			//GameObject iDOR_START = (GameObject)Instantiate( DOR_END, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
			GameObject iDOR_START = PrefabUtility.InstantiatePrefab( DOR_END as GameObject ) as GameObject;
			iDOR_START.transform.parent = _levelDoors.transform;
			iDOR_START.name = "DOR_START";
			
			// Rotate and translate piece into place.
			iDOR_START.transform.Translate( i_door.x, 0.0f, i_door.z );
			iDOR_START.transform.Rotate(0.0f, i_door.rot, 0.0f);

		}
	}

	#region BUILD WALL COLLIDERS
	void BuildCollider( WallCollider i_piece)
	{
		// Build Base Pieces
		GameObject iWAL_COL = PrefabUtility.InstantiatePrefab( WAL_COL as GameObject ) as GameObject;
		iWAL_COL.name = "WAL_COL_" + ColliderCount;
		iWAL_COL.transform.parent = _levelColliders.transform;
		iWAL_COL.tag = "Wall";

		// Translate and Rotate All Pieces
		iWAL_COL.transform.localScale = new Vector3(1.0f, 1.0f, i_piece.width);
		iWAL_COL.transform.Translate( i_piece.centerX, 0.0f, i_piece.centerZ );
		iWAL_COL.transform.Rotate(0.0f, i_piece.rotation, 0.0f);

		ColliderCount ++;

		
	}
	#endregion

}*/
