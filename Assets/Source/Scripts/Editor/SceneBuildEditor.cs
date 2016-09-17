
using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

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


[CustomEditor(typeof(SceneBuild))]
[CanEditMultipleObjects]
public class SceneBuildEditor : Editor {
	
	#region MEMBER VARIABLES ------------------
	private SerializedProperty stringProperty, boolProperty;
	private bool value;
	private string SceneFile;
	private List<DoorPiece> _doors;
	private List<WallPiece> _pieces;
	private List<WallPiece> _caps;
	private List<WallCollider> _colliders;
	private GameObject _levelWalls;
	private GameObject _levelDoors;
	private GameObject _levelColliders;
	
	// Basic Scene Construction Pieces
	public GameObject CONTAINER, SIDE, COR_LIN, COR_LOT, COR_RIN, COR_ROT, EXT_ML, EXT_MR, EXT_UL, EXT_UR, OBT_MIN,
	OBT_MOT, OBT_UIN, OBT_UOT, XOB_LIN, XOB_LOT, XOB_RIN, XOB_ROT, VAL_1M, VAL_2M, VAL_4M, 
	VAL_2U, VAL_4U, DOR_2M, DOR_2U, DOR_END, END_CAP, EXT_CUL, EXT_CUR, WAL_COL, LevelPreset, StartPrefab, EndPrefab;
	
	// Construction piece counts.
	private int TeeMMUCount,TeeUUMCount,CorLotCount,CorRotCount,CorXolCount,CorXorCount,CorObtMCount,
	CorObtUCount,WhyUCount,WhyMCount,DoorPcCount,Wall1MCount,Wall2MCount,Wall4MCount,
	Wall2UCount,Wall4UCount,EndCapCount,ColliderCount;

	private LevelDescriptionForEditor myDescription;
	private List<LevelDescriptionForEditor> _levelDetails  = new List<LevelDescriptionForEditor>();
	private float LevelWidth;
	private bool DataLoaded;

	#endregion --------------------------------
	
	
	void OnEnable() {
		DataLoaded = false;
		// Setup serialized property
		stringProperty = serializedObject.FindProperty("SceneFile");
	}
	
	void OnDisable()
	{
		SceneFile = SceneFile;
	}

	public override void OnInspectorGUI() 
	{
		// Display Editor Fields
		serializedObject.Update();
		SceneFile = stringProperty.stringValue;
		SceneFile = EditorGUILayout.TextField(SceneFile, new GUILayoutOption[0] );

		
		// On change
		if (GUI.changed )
		{
			stringProperty.stringValue = SceneFile;
			serializedObject.ApplyModifiedProperties();
		}

		GUI.skin.button.normal.textColor = new Color(1, 1, 1);

		if(GUILayout.Button("Load Scene Data"))
		{
			if ( SceneFile.Length > 0 )
			{
				LoadPieces();
				DataLoaded = true;
			}
		}

		if ( DataLoaded )
			GUI.skin.button.normal.textColor = new Color(0, 1, 0);
		else
			GUI.skin.button.normal.textColor = new Color(0.15f, 0.15f, 0.15f);

		if(GUILayout.Button("Build Scene Objects"))
		{
			if ( SceneFile.Length > 0 )
			{
				Debug.Log ("RAN: " + SceneFile);
				LoadResources();
				UpdateScene();
				NavMeshBuilder.BuildNavMesh();
			}
		}

		if(GUILayout.Button("Add Name To Level File"))
		{
			_levelDetails.Clear();
			LoadLevelDetails();

			if(!DoesLevelExist())
			{
				ShowMyDescriptionDebug();
				_levelDetails.Add(myDescription);
				WriteBack();
			}
		}

		if(GUILayout.Button("Remove Name From Level File"))
		{
			if(_levelDetails.Count == 0)
			{
				LoadLevelDetails();
			}
			if(DoesLevelExist())
			{
				Debug.Log("REmoving: "+ myDescription.LevelName);
				RemoveDescription();
				WriteBack();
			}
		}

		if(GUILayout.Button("Add Nav Mesh Carvings"))
		{
			if ( SceneFile.Length > 0 )
			{
				NavMeshCarving.InstantiateNavMeshCarvings(SceneFile);
				NavMeshBuilder.BuildNavMesh();
			}
		}
		GUI.skin.button.normal.textColor = new Color(1, 1, 1);
	}

	private void RemoveDescription()
	{
		int index = 0;
		bool found = false;
		for(int i = 0; i < _levelDetails.Count; i++)
		{
			if(_levelDetails[i].LevelName.CompareTo(myDescription.LevelName) == 0)
			{
				index = i;
				found = true;
				Debug.Log("Fount it! Name: " + _levelDetails[i].LevelName);
			}
		}
		if(found)
			_levelDetails.RemoveAt(index);
	}

	private void ShowMyDescriptionDebug()
	{
		Debug.Log("LevelName: " + myDescription.LevelName);
		Debug.Log("LevelThumbnail: " + myDescription.LevelThumbnail);
	}
	
	private void WriteBack()
	{

		//resource path: 		
		using (StreamWriter writer = new StreamWriter(Application.dataPath + "/Resources/Levels/LevelConfig.txt", false))
		{
			foreach(LevelDescriptionForEditor desc in _levelDetails)
			{
				//Debug.Log("LevelName: " + desc.LevelName);
				//Debug.Log("LevelThumbnail: " + desc.LevelThumbnail);
				writer.WriteLine(String.Format("{0}#{1}#{2}#{3}#{4}#{5}#{6}#{7}#{8}", 
				                               desc.Chapter, 
				                               desc.SceneFile,
				                               desc.LevelName,
				                               desc.LevelThumbnail,
				                               desc.LevelDetail,
				                               desc.Description,
				                               desc.EstimatedTime,
				                               desc.TransmitterNumber,
				                               desc.Difficulty));
			}
		}
	}

	private bool DoesLevelExist()
	{
		Debug.Log("Checking level, Name: " + myDescription.LevelName);
		foreach(LevelDescriptionForEditor desc in _levelDetails)
		{
			//only need to check against the name
			Debug.Log("Comparing .. List: " + desc.LevelName + " My: " + myDescription.LevelName);
			if(desc.LevelName.CompareTo(myDescription.LevelName) == 0)
			{
				return true;
			}
		}
		return false;
	}

	private void LoadLevelDetails()
	{
		TextAsset levelText = (TextAsset) Resources.Load("Levels/LevelConfig");
		Debug.Log("Log");
		using (TextReader levelReader = new StringReader((string)levelText.text))
		{
			int index = 0;
			while(levelReader.Peek() >= 0)
			{
				// ---- Level Data - Max - 10/18/13
				// - 0.Chapter Number
				// - 1.Scene File Name
				// - 2.Level Name
				// - 3.Path for thumbnail image
				// - 4.Path for detail image
				// - 5.Level Description
				// - 6.Estimated Time
				// - 7.Start Transmitters
				// - 8.Difficulty
				
				string[] levelData = levelReader.ReadLine().Split("#".ToCharArray());
				LevelDescriptionForEditor thisLevel = new LevelDescriptionForEditor();
				thisLevel.Chapter = Convert.ToInt32(levelData[0]);
				thisLevel.SceneFile = levelData[1];
				thisLevel.LevelName = levelData[2];
				thisLevel.LevelThumbnail = levelData[3];
				thisLevel.LevelDetail = levelData[4];
				thisLevel.Description = levelData[5];
				thisLevel.EstimatedTime = levelData[6];
				thisLevel.TransmitterNumber = Convert.ToInt32(levelData[7]);
				thisLevel.Difficulty = levelData[8];
				thisLevel.Index = index;
				_levelDetails.Add(thisLevel);
				index++;
			}
			
			levelReader.Close();			
		}
	}
	
	void LoadResources()
	{
		CONTAINER = (GameObject) Resources.Load("Prefabs/Theif/Wall Sections/WallContainer", typeof(GameObject) );
		SIDE = (GameObject) Resources.Load("Prefabs/Theif/Wall Sections/WallHandle", typeof(GameObject) );
		COR_LIN = (GameObject) Resources.Load("Prefabs/Theif/Wall Sections/COR_LIN", typeof(GameObject) );
		COR_LOT = (GameObject) Resources.Load("Prefabs/Theif/Wall Sections/COR_LOT", typeof(GameObject) );
		COR_RIN = (GameObject) Resources.Load("Prefabs/Theif/Wall Sections/COR_RIN", typeof(GameObject) );
		COR_ROT = (GameObject) Resources.Load("Prefabs/Theif/Wall Sections/COR_ROT", typeof(GameObject) );
		EXT_ML = (GameObject) Resources.Load("Prefabs/Theif/Wall Sections/EXT_ML", typeof(GameObject) );
		EXT_MR = (GameObject) Resources.Load("Prefabs/Theif/Wall Sections/EXT_MR", typeof(GameObject) );
		EXT_UL = (GameObject) Resources.Load("Prefabs/Theif/Wall Sections/EXT_UL", typeof(GameObject) );
		EXT_UR = (GameObject) Resources.Load("Prefabs/Theif/Wall Sections/EXT_UR", typeof(GameObject) );
		OBT_MIN = (GameObject) Resources.Load("Prefabs/Theif/Wall Sections/OBT_MIN", typeof(GameObject) );
		OBT_MOT = (GameObject) Resources.Load("Prefabs/Theif/Wall Sections/OBT_MOT", typeof(GameObject) );
		OBT_UIN = (GameObject) Resources.Load("Prefabs/Theif/Wall Sections/OBT_UIN", typeof(GameObject) );
		OBT_UOT = (GameObject) Resources.Load("Prefabs/Theif/Wall Sections/OBT_UOT", typeof(GameObject) );
		XOB_LIN = (GameObject) Resources.Load("Prefabs/Theif/Wall Sections/XOB_LIN", typeof(GameObject) );
		XOB_LOT = (GameObject) Resources.Load("Prefabs/Theif/Wall Sections/XOB_LOT", typeof(GameObject) );
		XOB_RIN = (GameObject) Resources.Load("Prefabs/Theif/Wall Sections/XOB_RIN", typeof(GameObject) );
		XOB_ROT = (GameObject) Resources.Load("Prefabs/Theif/Wall Sections/XOB_ROT", typeof(GameObject) );
		VAL_1M = (GameObject) Resources.Load("Prefabs/Theif/Wall Sections/VAL_1M", typeof(GameObject) );
		VAL_2M = (GameObject) Resources.Load("Prefabs/Theif/Wall Sections/VAL_2M", typeof(GameObject) );
		VAL_4M = (GameObject) Resources.Load("Prefabs/Theif/Wall Sections/VAL_4M", typeof(GameObject) );
		VAL_2U = (GameObject) Resources.Load("Prefabs/Theif/Wall Sections/VAL_2U", typeof(GameObject) );
		VAL_4U = (GameObject) Resources.Load("Prefabs/Theif/Wall Sections/VAL_4U", typeof(GameObject) );
		DOR_2M = (GameObject) Resources.Load("Prefabs/Theif/Doors/DOR_2M_BB", typeof(GameObject) );
		DOR_2U = (GameObject) Resources.Load("Prefabs/Theif/Doors/DOR_2U", typeof(GameObject) );
		DOR_END = (GameObject) Resources.Load("Prefabs/Theif/Doors/EndDoor", typeof(GameObject) );
		END_CAP = (GameObject) Resources.Load("Prefabs/Theif/Wall Sections/END_CAP", typeof(GameObject) );
		EXT_CUL = (GameObject) Resources.Load("Prefabs/Theif/Wall Sections/EXT_CUL", typeof(GameObject) );
		EXT_CUR = (GameObject) Resources.Load("Prefabs/Theif/Wall Sections/EXT_CUR", typeof(GameObject) );
		WAL_COL = (GameObject) Resources.Load("Prefabs/Theif/Wall Sections/WAL_COL", typeof(GameObject) );
		LevelPreset = (GameObject) Resources.Load("Prefabs/LevelPreset", typeof(GameObject) );
		StartPrefab = (GameObject) Resources.Load("Prefabs/Hacker/Graph/StartMarker", typeof(GameObject) );
		EndPrefab = (GameObject) Resources.Load("Prefabs/Hacker/Graph/EndMarker", typeof(GameObject) );
	}
	
	
	void UpdateScene()
	{
		//LoadPieces();
		
		RemoveOldWalls();

		BuildScenePreset();
		BuildWalls();
	}
	
	#region LOAD PIECES FROM FILE -------------------------------
	void LoadPieces ()
	{

		_pieces = new List<WallPiece>();
		_doors = new List<DoorPiece>();
		_caps = new List<WallPiece>();
		_colliders = new List<WallCollider>();
		
		// Load Intersection Pieces.
		TextAsset txt = (TextAsset) Resources.Load("Levels/" + SceneFile + "/" + SceneFile + "_SCENE");		
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
			//Debug.Log (lines[i]);
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
			
			if ( type == WallType.Door_M || type == WallType.Door_U || type == WallType.Door_End || type == WallType.Door_Start) // This is a door
			{
				DoorPcType thisType;
				if ( type == WallType.Door_M )
					thisType = DoorPcType.StandardM;
				else if ( type == WallType.Door_U )
					thisType = DoorPcType.StandardU;
				else if ( type == WallType.Door_End )
					thisType = DoorPcType.End;
				else if ( type == WallType.Door_Start )
					thisType = DoorPcType.Start;
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
			
			//Debug.Log (lines[i]);
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
			WallType type = ParseType( parameters[6]);
			
			WallPiece tempPiece = new WallPiece( x, z, rot, longM, longU, longX, type);
			_caps.Add( tempPiece );
			//Debug.Log (lines[i]);
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
			//Debug.Log (lines[i]);
			pointer ++;
		}
		
		// Load Level Meta Data
		int MetaCount = Convert.ToInt32(lines[pointer]);
		pointer++;
		myDescription.Chapter = Convert.ToInt32(lines[pointer]);
		myDescription.SceneFile = lines[pointer+1];
		myDescription.LevelName = lines[pointer+2];
		//myDescription.LevelThumbnail = lines[pointer+3];
		//myDescription.LevelDetail = lines[pointer+4];
		myDescription.LevelThumbnail = "Levels/" + lines[pointer+1] + "/" + lines[pointer+1] + "_THUMB";
		myDescription.LevelDetail = "Levels/" + lines[pointer+1] + "/" + lines[pointer+1] + "_DETAIL";
		myDescription.Description = lines[pointer+5];
		myDescription.EstimatedTime = lines[pointer+6];
		myDescription.TransmitterNumber = Convert.ToInt32 (lines[pointer+7]);
		myDescription.Difficulty = lines[pointer+8];
		LevelWidth = Convert.ToSingle(lines[pointer+9]);

		Debug.Log( "LOCATION OF THUMBNAIL = " + myDescription.LevelThumbnail);
		Debug.Log ( "Finished Loading Scene data: " + SceneFile);
		
	}
	#endregion -----------------------------------------------
	
	
	#region REMOVE OLD WALLS -------------------------------
	void RemoveOldWalls()
	{
		GameObject Preset = GameObject.Find("LevelPreset");
		DestroyImmediate(Preset);

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

		// If there is still an initial Main Camera delete it.
		GameObject MainCam = GameObject.Find("Main Camera");
		DestroyImmediate(MainCam);

		GameObject Start = GameObject.Find("StartMarker");
		DestroyImmediate(Start);
		GameObject End = GameObject.Find("EndMarker");
		DestroyImmediate(End);
	}
	#endregion ------------------------------------------------
	
	void BuildScenePreset()
	{
		GameObject scene = PrefabUtility.InstantiatePrefab( LevelPreset as GameObject ) as GameObject;

		GameObject MapPlane = GameObject.Find("MapPlane");
		float mapX, mapZ, mapScale;

		Material mapMaterial = (Material) Resources.Load("Materials/MapGuideTextures/" + SceneFile, typeof(Material) );
		if ( mapMaterial == null )
		{
			mapMaterial = new Material ( Shader.Find("Transparent/Diffuse"));
			AssetDatabase.CreateAsset(mapMaterial, "Assets/Resources/Materials/MapGuideTextures/" + SceneFile + ".mat");
		}
		Texture2D mapTexture = ( Texture2D ) Resources.Load ( ("Levels/" + SceneFile + "/" + SceneFile + "_MAP") );
		if ( mapTexture == null )
			Debug.LogError ("CANT FIND TEXTURE: Put the texture in the Textures/Map_Guides/ folder and name it: <SceneID>_MAP");

		mapMaterial.mainTexture = mapTexture;
		MapPlane.renderer.material = mapMaterial;

		mapScale = LevelWidth/10;
		MapPlane.transform.localScale = new Vector3(mapScale, 1.0f, mapScale);

		mapX = -(LevelWidth-2.0f)/2.0f;
		mapZ = (LevelWidth-1.732f)/2.0f;
		MapPlane.transform.position = new Vector3(mapX, 54.0f, mapZ);

	}


	#region BUILD WALLS --------------------------------
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
			Build_DOOR( _doors[i] );
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
				Build_TEE_MMU ( _pieces[i] );
				break;
			case WallType.TeeUUM:
				Build_TEE_UUM ( _pieces[i] );
				break;
			case WallType.CornerRot:
				Build_COR_90R ( _pieces[i] );
				break;
			case WallType.CornerLot:
				Build_COR_90L ( _pieces[i] );
				break;
			case WallType.CornerXol:
				Build_COR_XOL ( _pieces[i] );
				break;
			case WallType.CornerXor:
				Build_COR_XOR ( _pieces[i] );
				break;
			case WallType.CornerObtM:
				Build_COR_OBM ( _pieces[i] );
				break;
			case WallType.CornerObtU:
				Build_COR_OBU ( _pieces[i] );
				break;
			case WallType.WhyU:
				Build_WHY_U ( _pieces[i] );
				break;
			case WallType.WhyM:
				Build_WHY_M ( _pieces[i] );
				break;
			case WallType.Wall_1M:
				Build_WAL_1M ( _pieces[i] );
				break;
			case WallType.Wall_2M:
				Build_WAL_2M ( _pieces[i] );
				break;
			case WallType.Wall_4M:
				Build_WAL_4M ( _pieces[i] );
				break;
			case WallType.Wall_2U:
				Build_WAL_2U ( _pieces[i] );
				break;
			case WallType.Wall_4U:
				Build_WAL_4U ( _pieces[i] );
				break;
			case WallType.End_Cap:
				Build_END_CAP ( _pieces[i] );
				break;
			}
		}
		
		///--------------------
		// **** END CAPS ***
		///--------------------
		// Instantiate each cap in the list
		for ( int i=0 ; i<_caps.Count ; i++ )
		{
			Build_END_CAP( _caps[i] );
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
			Build_COLLIDER( _colliders[i] );
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
	#endregion -------------------------------------------
	
	
	#region INDIVIDUAL BUILDER SCRIPTS ----------------------
	void Build_COR_90L( WallPiece i_piece)
	{
		GameObject thisPARENT = CreateParent ( "COR_90L_" , CorLotCount);
		
		// Create Side Handles
		GameObject SideA = CreateSide("SideA", -45.0f, thisPARENT);
		GameObject SideB = CreateSide("SideB", 135.0f, thisPARENT);
		
		// Create Base Pieces
		GameObject iCOR_LIN = CreateBasePiece(COR_LIN, 0.0f, SideB );
		GameObject iCOR_LOT = CreateBasePiece(COR_LOT, 0.0f, SideA );
		
		// Build Extension Pieces if Needed.
		if ( i_piece.long_M )
		{
			GameObject iEXT_ML = CreateBasePiece(EXT_ML, 180.0f, SideB );
			GameObject iEXT_MR = CreateBasePiece(EXT_MR, 0.0f, SideA );
		}
		
		if ( i_piece.long_U )
		{
			GameObject iEXT_UL = CreateBasePiece(EXT_UL, 0.0f, SideA );
			GameObject iEXT_UR = CreateBasePiece(EXT_UR, 180.0f, SideB );
		}
		
		// Translate and Rotate All Pieces
		thisPARENT.transform.Translate( i_piece.x, 0.0f, i_piece.z );
		thisPARENT.transform.Rotate(0.0f, i_piece.rot, 0.0f);
		
		thisPARENT.transform.parent = _levelWalls.transform;
		CorLotCount ++;
	}
	
	
	void Build_COR_90R( WallPiece i_piece)
	{
		GameObject thisPARENT = CreateParent ( "COR_90R_" , CorRotCount);
		
		// Create Side Handles
		GameObject SideA = CreateSide("SideA", 225.0f, thisPARENT);
		GameObject SideB = CreateSide("SideB", 45.0f, thisPARENT);
		
		// Create Base Pieces
		GameObject iCOR_LIN = CreateBasePiece(COR_RIN, 0.0f, SideB );
		GameObject iCOR_LOT = CreateBasePiece(COR_ROT, 0.0f, SideA );
		
		// Build Extension Pieces if Needed.
		if ( i_piece.long_M )
		{
			GameObject iEXT_ML = CreateBasePiece(EXT_ML, 0.0f, SideA );
			GameObject iEXT_MR = CreateBasePiece(EXT_MR, 180.0f, SideB );
		}
		
		if ( i_piece.long_U )
		{
			GameObject iEXT_UL = CreateBasePiece(EXT_UL, 0.0f, SideB );
			GameObject iEXT_UR = CreateBasePiece(EXT_UR, 180.0f, SideA );
		}
		
		// Translate and Rotate All Pieces
		thisPARENT.transform.Translate( i_piece.x, 0.0f, i_piece.z );
		thisPARENT.transform.Rotate(0.0f, i_piece.rot, 0.0f);
		
		thisPARENT.transform.parent = _levelWalls.transform;
		CorRotCount ++;
	}
	
	
	void Build_COR_OBU( WallPiece i_piece)
	{
		GameObject thisPARENT = CreateParent ( "COR_OBU_" , CorObtUCount);
		
		// Create Side Handles
		GameObject SideA = CreateSide("SideA", 210.0f, thisPARENT);
		GameObject SideB = CreateSide("SideB", 30.0f, thisPARENT);
		
		// Create Base Pieces
		GameObject iCOR_LIN = CreateBasePiece(OBT_UIN, 0.0f, SideB );
		GameObject iCOR_LOT = CreateBasePiece(OBT_UOT, 0.0f, SideA );
		
		// Build Extension Pieces if Needed.
		if ( i_piece.long_M )
		{
			GameObject iEXT_UL = CreateBasePiece(EXT_UL, 240.0f, SideA );
			GameObject iEXT_UR = CreateBasePiece(EXT_UR, 60.0f, SideB );
		}
		
		if ( i_piece.long_U )
		{
			GameObject iEXT_UL = CreateBasePiece(EXT_UL, 0.0f, SideB );
			GameObject iEXT_UR = CreateBasePiece(EXT_UR, 180.0f, SideA );
		}
		
		// Translate and Rotate All Pieces
		thisPARENT.transform.Translate( i_piece.x, 0.0f, i_piece.z );
		thisPARENT.transform.Rotate(0.0f, i_piece.rot, 0.0f);
		
		thisPARENT.transform.parent = _levelWalls.transform;
		CorObtUCount ++;
	}
	
	void Build_COR_OBM( WallPiece i_piece)
	{
		GameObject thisPARENT = CreateParent ( "COR_OBM_" , CorObtMCount);
		
		// Create Side Handles
		GameObject SideA = CreateSide("SideA", -60.0f, thisPARENT);
		GameObject SideB = CreateSide("SideB", 120.0f, thisPARENT);
		
		// Create Base Pieces
		GameObject iCOR_LIN = CreateBasePiece(OBT_MIN, 0.0f, SideB );
		GameObject iCOR_LOT = CreateBasePiece(OBT_MOT, 0.0f, SideA );
		
		// Build Extension Pieces if Needed.
		if ( i_piece.long_M )
		{
			GameObject iEXT_ML = CreateBasePiece(EXT_ML, 180.0f, SideB );
			GameObject iEXT_MR = CreateBasePiece(EXT_MR, 0.0f, SideA );
		}
		
		if ( i_piece.long_U )
		{
			GameObject iEXT_UL = CreateBasePiece(EXT_ML, 60.0f, SideA );
			GameObject iEXT_UR = CreateBasePiece(EXT_MR, 240.0f, SideB );
		}
		
		// Translate and Rotate All Pieces
		thisPARENT.transform.Translate( i_piece.x, 0.0f, i_piece.z );
		thisPARENT.transform.Rotate(0.0f, i_piece.rot, 0.0f);
		
		thisPARENT.transform.parent = _levelWalls.transform;
		CorObtMCount ++;
	}
	
	void Build_COR_XOL( WallPiece i_piece)
	{
		GameObject thisPARENT = CreateParent ( "COR_XOL_" , CorXolCount);
		
		// Create Side Handles
		GameObject SideA = CreateSide("SideA", -30.0f, thisPARENT);
		GameObject SideB = CreateSide("SideB", 150.0f, thisPARENT);
		
		// Create Base Pieces
		GameObject iCOR_LIN = CreateBasePiece(XOB_LIN, 0.0f, SideB );
		GameObject iCOR_LOT = CreateBasePiece(XOB_LOT, 0.0f, SideA );
		
		// Build Extension Pieces if Needed.
		if ( i_piece.long_M )
		{
			GameObject iEXT_ML = CreateBasePiece(EXT_ML, 240.0f, SideB );
			GameObject iEXT_MR = CreateBasePiece(EXT_MR, 60.0f, SideA );
		}
		
		if ( i_piece.long_U )
		{
			GameObject iEXT_UL = CreateBasePiece(EXT_UL, 0.0f, SideA );
			GameObject iEXT_UR = CreateBasePiece(EXT_UR, 180.0f, SideB );
		}
		
		// Translate and Rotate All Pieces
		thisPARENT.transform.Translate( i_piece.x, 0.0f, i_piece.z );
		thisPARENT.transform.Rotate(0.0f, i_piece.rot, 0.0f);
		
		thisPARENT.transform.parent = _levelWalls.transform;
		CorXolCount ++;
	}
	
	void Build_COR_XOR( WallPiece i_piece)
	{
		GameObject thisPARENT = CreateParent ( "COR_XOR_" , CorXorCount);
		
		// Create Side Handles
		GameObject SideA = CreateSide("SideA", 30.0f, thisPARENT);
		GameObject SideB = CreateSide("SideB", 210.0f, thisPARENT);
		
		// Create Base Pieces
		GameObject iXOB_RIN = CreateBasePiece(XOB_RIN, 0.0f, SideA );
		GameObject iXOB_ROT = CreateBasePiece(XOB_ROT, 0.0f, SideB );
		
		// Build Extension Pieces if Needed.
		if ( i_piece.long_M )
		{
			GameObject iEXT_ML = CreateBasePiece(EXT_ML, 300.0f, SideB );
			GameObject iEXT_MR = CreateBasePiece(EXT_MR, 120.0f, SideA );
		}
		
		if ( i_piece.long_U )
		{
			GameObject iEXT_UL = CreateBasePiece(EXT_UL, 0.0f, SideA );
			GameObject iEXT_UR = CreateBasePiece(EXT_UR, 180.0f, SideB );
		}
		
		// Translate and Rotate All Pieces
		thisPARENT.transform.Translate( i_piece.x, 0.0f, i_piece.z );
		thisPARENT.transform.Rotate(0.0f, i_piece.rot, 0.0f);
		
		thisPARENT.transform.parent = _levelWalls.transform;
		CorXorCount ++;
	}
	
	
	void Build_TEE_MMU( WallPiece i_piece)
	{
		GameObject thisPARENT = CreateParent ( "TEE_MMU_" , TeeMMUCount);
		
		// Create Side Handles
		GameObject SideA = CreateSide("SideA", 150.0f, thisPARENT);
		GameObject SideB = CreateSide("SideB", 30.0f, thisPARENT);
		GameObject SideC = CreateSide("SideC", -90.0f, thisPARENT);
		
		// Create Base Pieces
		GameObject iCOR_RIN = CreateBasePiece(COR_RIN, 0.0f, SideB );
		GameObject iCOR_LIN = CreateBasePiece(COR_LIN, 0.0f, SideA );
		GameObject iVAL_1M = CreateBasePiece(VAL_1M, 0.0f, SideC );
		
		// Build Extension Pieces if Needed.
		if ( i_piece.long_M )
		{
			GameObject iEXT_ML = CreateBasePiece(EXT_ML, 180.0f, SideA );
			GameObject iEXT_MR = CreateBasePiece(EXT_MR, 0.0f, SideC );
		}
		
		if ( i_piece.long_U )
		{
			GameObject iEXT_UL = CreateBasePiece(EXT_UL, 0.0f, SideB );
			GameObject iEXT_UR = CreateBasePiece(EXT_UR, 180.0f, SideA );
		}
		
		if ( i_piece.extLong )
		{
			GameObject iEXT_ML = CreateBasePiece(EXT_ML, 0.0f, SideC );
			GameObject iEXT_MR = CreateBasePiece(EXT_MR, 180.0f, SideB );
		}
		
		// Translate and Rotate All Pieces
		thisPARENT.transform.Translate( i_piece.x, 0.0f, i_piece.z );
		thisPARENT.transform.Rotate(0.0f, i_piece.rot, 0.0f);
		
		thisPARENT.transform.parent = _levelWalls.transform;
		TeeMMUCount ++;
	}
	
	
	void Build_TEE_UUM( WallPiece i_piece)
	{
		GameObject thisPARENT = CreateParent ( "TEE_UUM_" , TeeUUMCount);
		
		// Create Side Handles
		GameObject SideA = CreateSide("SideA", 60.0f, thisPARENT);
		GameObject SideB = CreateSide("SideB", -60.0f, thisPARENT);
		GameObject SideC = CreateSide("SideC", 180.0f, thisPARENT);
		
		// Create Base Pieces
		GameObject iCOR_RIN = CreateBasePiece(COR_RIN, 0.0f, SideA );
		GameObject iCOR_LIN = CreateBasePiece(COR_LIN, 180.0f, SideB );
		GameObject iVAL_2U = CreateBasePiece(VAL_2U, 0.0f, SideC );
		
		// Build Extension Pieces if Needed.
		if ( i_piece.long_M )
		{
			GameObject iEXT_ML = CreateBasePiece(EXT_ML, 0.0f, SideB );
			GameObject iEXT_MR = CreateBasePiece(EXT_MR, 180.0f, SideA );
		}
		
		if ( i_piece.long_U )
		{
			GameObject iEXT_UL = CreateBasePiece(EXT_UL, 180.0f, SideC );
			GameObject iEXT_UR = CreateBasePiece(EXT_UR, 0.0f, SideB );
		}
		
		if ( i_piece.extLong )
		{
			GameObject iEXT_UL = CreateBasePiece(EXT_UL, 0.0f, SideA );
			GameObject iEXT_UR = CreateBasePiece(EXT_UR, 180.0f, SideC );
		}
		
		// Translate and Rotate All Pieces
		thisPARENT.transform.Translate( i_piece.x, 0.0f, i_piece.z );
		thisPARENT.transform.Rotate(0.0f, i_piece.rot, 0.0f);
		
		thisPARENT.transform.parent = _levelWalls.transform;
		TeeUUMCount ++;
	}
	
	
	void Build_WHY_U( WallPiece i_piece)
	{
		GameObject thisPARENT = CreateParent ( "WHY_U_" , WhyUCount);
		
		// Create Side Handles
		GameObject SideA = CreateSide("SideA", 30.0f, thisPARENT);
		GameObject SideB = CreateSide("SideB", 150.0f, thisPARENT);
		GameObject SideC = CreateSide("SideC", 270.0f, thisPARENT);
		
		// Create Base Pieces
		GameObject iOBT_UIN1 = CreateBasePiece(OBT_UIN, 0.0f, SideA );
		GameObject iOBT_UIN2 = CreateBasePiece(OBT_UIN, 120.0f, SideB );
		GameObject iOBT_UIN3 = CreateBasePiece(OBT_UIN, 240.0f, SideC );
		/*
		// Build Extension Pieces if Needed.
		if ( i_piece.long_U )
		{
			GameObject iEXT_UL = CreateBasePiece(EXT_UL, 120.0f, SideB );
			GameObject iEXT_UR = CreateBasePiece(EXT_UR, 300.0f, SideC );
		}
		if ( i_piece.extLong )
		{
			GameObject iEXT_UL2 = CreateBasePiece(EXT_UL, 240.0f, SideC );
			GameObject iEXT_UR2 = CreateBasePiece(EXT_UR, 60.0f, SideA );
		}
		if (i_piece.long_M )
		{
			GameObject iEXT_UL3 = CreateBasePiece(EXT_UL, 0.0f, SideA );
			GameObject iEXT_UR3 = CreateBasePiece(EXT_UR, 180.0f, SideB );
		}
*/
		if ( i_piece.long_U )
		{
			GameObject iEXT_UL3 = CreateBasePiece(EXT_UL, 0.0f, SideA );
			GameObject iEXT_UR3 = CreateBasePiece(EXT_UR, 180.0f, SideB );
		}
		if ( i_piece.extLong )
		{
			GameObject iEXT_UL2 = CreateBasePiece(EXT_UL, 240.0f, SideC );
			GameObject iEXT_UR2 = CreateBasePiece(EXT_UR, 60.0f, SideA );
		}
		if (i_piece.long_M )
		{
			GameObject iEXT_UL = CreateBasePiece(EXT_UL, 120.0f, SideB );
			GameObject iEXT_UR = CreateBasePiece(EXT_UR, 300.0f, SideC );
		}

		// Translate and Rotate All Pieces
		thisPARENT.transform.Translate( i_piece.x, 0.0f, i_piece.z );
		thisPARENT.transform.Rotate(0.0f, i_piece.rot, 0.0f);
		
		thisPARENT.transform.parent = _levelWalls.transform;
		WhyUCount ++;
	}
	
	
	void Build_WHY_M( WallPiece i_piece)
	{
		GameObject thisPARENT = CreateParent ( "WHY_M_" , WhyMCount);
		
		// Create Side Handles
		GameObject SideA = CreateSide("SideA", 15.0f, thisPARENT);
		GameObject SideB = CreateSide("SideB", 135.0f, thisPARENT);
		GameObject SideC = CreateSide("SideC", 245.0f, thisPARENT);
		
		// Create Base Pieces
		GameObject iOBT_MIN1 = CreateBasePiece(OBT_MIN, 0.0f, SideB );
		GameObject iOBT_MIN2 = CreateBasePiece(OBT_MIN, 120.0f, SideC );
		GameObject iOBT_MIN3 = CreateBasePiece(OBT_MIN, 240.0f, SideA );
		
		// Build Extension Pieces if Needed.
		if ( i_piece.long_M )
		{
			GameObject iEXT_ML = CreateBasePiece(EXT_ML, 180.0f, SideB );
			GameObject iEXT_MR = CreateBasePiece(EXT_MR, 0.0f, SideC );
		}
		if ( i_piece.long_U )
		{
			GameObject iEXT_ML2 = CreateBasePiece(EXT_ML, 300.0f, SideC );
			GameObject iEXT_MR2 = CreateBasePiece(EXT_MR, 120.0f, SideA );
		}
		if  ( i_piece.extLong )
		{
			GameObject iEXT_ML3 = CreateBasePiece(EXT_ML, 60.0f, SideA );
			GameObject iEXT_MR3 = CreateBasePiece(EXT_MR, 240.0f, SideB );
		}
		
		// Translate and Rotate All Pieces
		thisPARENT.transform.Translate( i_piece.x, 0.0f, i_piece.z );
		thisPARENT.transform.Rotate(0.0f, i_piece.rot, 0.0f);
		
		thisPARENT.transform.parent = _levelWalls.transform;
		WhyMCount ++;
	}
	
	void Build_WAL_1M( WallPiece i_piece)
	{
		GameObject thisPARENT = CreateParent ( "WAL_1M_" , Wall1MCount);
		
		// Create Side Handles
		GameObject SideA = CreateSide("SideA", 90.0f, thisPARENT);
		GameObject SideB = CreateSide("SideB", 270.0f, thisPARENT);
		
		// Create Base Pieces
		GameObject iVAL_1M1 = CreateBasePiece(VAL_1M, 0.0f, SideB );
		GameObject iVAL_1M2 = CreateBasePiece(VAL_1M, 180.0f, SideA );
		
		// Translate and Rotate All Pieces
		thisPARENT.transform.Translate( i_piece.x, 0.0f, i_piece.z );
		thisPARENT.transform.Rotate(0.0f, i_piece.rot, 0.0f);
		
		thisPARENT.transform.parent = _levelWalls.transform;
		Wall1MCount ++;
	}
	
	void Build_WAL_2M( WallPiece i_piece)
	{
		GameObject thisPARENT = CreateParent ( "WAL_2M_" , Wall2MCount);
		
		// Create Side Handles
		GameObject SideA = CreateSide("SideA", 90.0f, thisPARENT);
		GameObject SideB = CreateSide("SideB", 270.0f, thisPARENT);
		
		// Create Base Pieces
		GameObject iVAL_1M1 = CreateBasePiece(VAL_2M, 0.0f, SideB );
		GameObject iVAL_1M2 = CreateBasePiece(VAL_2M, 180.0f, SideA );
		
		// Translate and Rotate All Pieces
		thisPARENT.transform.Translate( i_piece.x, 0.0f, i_piece.z );
		thisPARENT.transform.Rotate(0.0f, i_piece.rot, 0.0f);
		
		thisPARENT.transform.parent = _levelWalls.transform;
		Wall2MCount ++;
	}
	
	void Build_WAL_4M( WallPiece i_piece)
	{
		GameObject thisPARENT = CreateParent ( "WAL_4M_" , Wall4MCount);
		
		// Create Side Handles
		GameObject SideA = CreateSide("SideA", 90.0f, thisPARENT);
		GameObject SideB = CreateSide("SideB", 270.0f, thisPARENT);
		
		// Create Base Pieces
		GameObject iVAL_4M1 = CreateBasePiece(VAL_4M, 0.0f, SideB );
		GameObject iVAL_4M2 = CreateBasePiece(VAL_4M, 180.0f, SideA );
		
		// Translate and Rotate All Pieces
		thisPARENT.transform.Translate( i_piece.x, 0.0f, i_piece.z );
		thisPARENT.transform.Rotate(0.0f, i_piece.rot, 0.0f);
		
		thisPARENT.transform.parent = _levelWalls.transform;
		Wall4MCount ++;
	}
	
	void Build_WAL_2U( WallPiece i_piece)
	{
		GameObject thisPARENT = CreateParent ( "WAL_2U_" , Wall2UCount);
		
		// Create Side Handles
		GameObject SideA = CreateSide("SideA", 0.0f, thisPARENT);
		GameObject SideB = CreateSide("SideB", 180.0f, thisPARENT);
		
		// Create Base Pieces
		GameObject iVAL_2U1 = CreateBasePiece(VAL_2U, 0.0f, SideB );
		GameObject iVAL_2U2 = CreateBasePiece(VAL_2U, 180.0f, SideA );
		
		// Translate and Rotate All Pieces
		thisPARENT.transform.Translate( i_piece.x, 0.0f, i_piece.z );
		thisPARENT.transform.Rotate(0.0f, i_piece.rot, 0.0f);
		
		thisPARENT.transform.parent = _levelWalls.transform;
		Wall2UCount ++;
	}
	
	
	void Build_WAL_4U( WallPiece i_piece)
	{
		GameObject thisPARENT = CreateParent ( "WAL_4U_" , Wall4UCount);
		
		// Create Side Handles
		GameObject SideA = CreateSide("SideA", 0.0f, thisPARENT);
		GameObject SideB = CreateSide("SideB", 180.0f, thisPARENT);
		
		// Create Base Pieces
		GameObject iVAL_2U1 = CreateBasePiece(VAL_4U, 0.0f, SideB );
		GameObject iVAL_2U2 = CreateBasePiece(VAL_4U, 180.0f, SideA );
		
		// Translate and Rotate All Pieces
		thisPARENT.transform.Translate( i_piece.x, 0.0f, i_piece.z );
		thisPARENT.transform.Rotate(0.0f, i_piece.rot, 0.0f);
		
		thisPARENT.transform.parent = _levelWalls.transform;
		Wall4UCount ++;
	}
	
	void Build_END_CAP( WallPiece i_piece)
	{
		GameObject thisPARENT = CreateParent ( "END_CAP_" , EndCapCount);
		
		// Create Side Handles
		GameObject SideA = CreateSide("SideA", 0.0f, thisPARENT);
		GameObject SideB = CreateSide("SideB", 180.0f, thisPARENT);
		
		// Create Base Pieces
		GameObject iEND_CAP = CreateBasePiece(END_CAP, 180.0f, SideA );
		
		if ( i_piece.long_U )
		{
			GameObject iEXT_CUL = CreateBasePiece(EXT_CUL, 180.0f, SideB );
			GameObject iEXT_CUR = CreateBasePiece(EXT_CUR, 0.0f, SideA );
		}
		
		// Translate and Rotate All Pieces
		thisPARENT.transform.Translate( i_piece.x, 0.0f, i_piece.z );
		thisPARENT.transform.Rotate(0.0f, i_piece.rot, 0.0f);
		
		thisPARENT.transform.parent = _levelWalls.transform;
		EndCapCount ++;
	}
	
	void Build_DOOR( DoorPiece i_door)
	{
		GameObject iPIECE;
		if ( i_door.type == DoorPcType.StandardM )
		{
			iPIECE = PrefabUtility.InstantiatePrefab( DOR_2M as GameObject ) as GameObject;
			iPIECE.transform.parent = _levelDoors.transform;
			iPIECE.tag = "Door";
			iPIECE.name = "DOR_2M_" + DoorPcCount;
		}
		else if ( i_door.type == DoorPcType.StandardU )
		{
			iPIECE = PrefabUtility.InstantiatePrefab( DOR_2U as GameObject ) as GameObject;
			iPIECE.transform.parent = _levelDoors.transform;
			iPIECE.tag = "Door";
			iPIECE.name = "DOR_2U_" + DoorPcCount;
		}
		else if ( i_door.type == DoorPcType.End )
		{
			iPIECE = PrefabUtility.InstantiatePrefab( DOR_END as GameObject ) as GameObject;
			iPIECE.transform.parent = _levelDoors.transform;
			iPIECE.tag = "Door";
			iPIECE.name = "DOR_END_" + DoorPcCount;	
			iPIECE.GetComponent("EndDoorCompnent");

			// Create and position the hackers end marker
			GameObject endMark = PrefabUtility.InstantiatePrefab( EndPrefab as GameObject ) as GameObject;
			endMark.transform.position = new Vector3( i_door.x, 54.1f, i_door.z );
			endMark.transform.Rotate(0.0f, (i_door.rot-180), 0.0f);
			GameObject endText = GameObject.Find ("EndText");
			endText.transform.localEulerAngles = new Vector3(90.0f, (180.0f-(i_door.rot-180)), 0.0f);
		}
		else // Start Door
		{
			iPIECE = PrefabUtility.InstantiatePrefab( DOR_END as GameObject ) as GameObject;
			iPIECE.transform.parent = _levelDoors.transform;
			iPIECE.tag = "Door";
			iPIECE.name = "DOR_START_" + DoorPcCount;	

			// Set Door Type
			EndDoorController doorScript = (EndDoorController) iPIECE.GetComponent ("EndDoorController");
			doorScript._type = DoorType.StartDoor;

			// Set the starting point for the level
			Debug.Log ("SETTING START POINT");
			GameObject StartPoint = GameObject.Find("StartPoint");
			float startX = i_door.x - 2.8f*(float)(Math.Sin ( i_door.rot * 0.017453f) );
			float startZ = i_door.z - 2.8f*(float)(Math.Cos ( i_door.rot * 0.017453f) );
			StartPoint.transform.position = new Vector3( startX, 1.2f, startZ);

			// Create and position the hackers start marker
			GameObject startMark = PrefabUtility.InstantiatePrefab( StartPrefab as GameObject ) as GameObject;
			startMark.transform.position = new Vector3( i_door.x, 54.1f, i_door.z );
			startMark.transform.Rotate(0.0f, (i_door.rot-180), 0.0f);
			GameObject startText = GameObject.Find ("StartText");
			startText.transform.localEulerAngles = new Vector3(90.0f, (180.0f-(i_door.rot-180)), 0.0f);
		}
		
		// Rotate and translate piece into place.
		iPIECE.transform.Translate( i_door.x, 0.0f, i_door.z );
		iPIECE.transform.Rotate(0.0f, i_door.rot, 0.0f);
		
		DoorPcCount ++;
	}
	
	void Build_COLLIDER( WallCollider i_piece)
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
	
	#endregion ------------------------------------------------
	
	// Helper Creation Methods
	private GameObject CreateParent( string i_name, int i_count )
	{
		GameObject thisPARENT = (GameObject)Instantiate( CONTAINER, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
		thisPARENT.tag = "WallContainer";
		thisPARENT.name = i_name + i_count;
		
		return thisPARENT;
	}
	
	private GameObject CreateSide( string i_name, float i_rotation, GameObject i_parent )
	{
		GameObject Side = (GameObject)Instantiate( SIDE, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
		Side.transform.Rotate(0, i_rotation, 0);
		Side.name = i_name;
		Side.transform.parent = i_parent.transform;
		
		return Side;
	}
	
	private GameObject CreateBasePiece( GameObject i_prefab, float i_rotation, GameObject i_parent )
	{
		GameObject iPIECE = PrefabUtility.InstantiatePrefab( i_prefab as GameObject ) as GameObject;
		//GameObject iPIECE = (GameObject)Instantiate( i_prefab, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
		iPIECE.transform.Rotate(0, i_rotation, 0);
		iPIECE.transform.parent = i_parent.transform;
		iPIECE.tag = "Wall";
		
		return iPIECE;
	}
	
}
