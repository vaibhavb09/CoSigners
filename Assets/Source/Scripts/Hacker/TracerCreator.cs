using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TracerCreator : MonoBehaviour {
	
	public enum creatorType { Static=0, Interval=1, Randomize=2, } 
	
	private float frequency;
	private creatorType myType;
	//private List<Tracer> myTracers;
	private List<TracerData> myTracers;
	private float myTicker;
	private bool run;

	public Material Material_Tracer_Calibrating;
	public Material Material_Tracer_Active;
	public Material Material_Tracer_Scrambled;
	public Material ActiveBase;
	public Material ActiveCover;
	public GameObject TracerPrefab;
	
	public creatorType Type
	{
		get{
			return myType;
		}
	}
	
	
	// Use this for initialization
	void Start () {
	}
	
	
	public void Set(string i_type, float i_frequency)
	{
		Material_Tracer_Calibrating = Resources.Load("Materials/Hacker/Hexes/Tracer_Calibrating", typeof(Material)) as Material; 
		Material_Tracer_Active = Resources.Load("Materials/Hacker/Hexes/Tracer_Active", typeof(Material))as Material;
		Material_Tracer_Scrambled=Resources.Load("Materials/Hacker/Hexes/Tracer_Scrambled", typeof(Material))as Material;
		TracerPrefab = Resources.Load ("Prefabs/Hacker/Graph/Tracer_Prefab", typeof(GameObject))as GameObject;
		
		ActiveBase = Resources.Load("Materials/Hacker/Hexes/Tracer_Active_Base", typeof(Material)) as Material;
		ActiveCover = Resources.Load("Materials/Hacker/Hexes/Tracer_Active_Cover", typeof(Material)) as Material;


		myTracers = new List<TracerData>();
		
		if ( i_type.Equals("Random") )
			myType = creatorType.Randomize;
		else if ( i_type.Equals("Interval") )
			myType = creatorType.Interval;
		else if ( i_type.Equals("Static") )
			myType = creatorType.Static;
		
		frequency = i_frequency;
		run = true;

		//Debug.Log ("TRACER CREATOR CREATED: " + i_type);
	}
	
	
	public void AddTracer( TracerData i_tracer)
	{
		myTracers.Add( i_tracer );
		//Debug.Log ("Added Tracer Data...");
	}
	
	public void DestroyTracer(Tracer i_tracer)
	{
		SecurityManager.Manager.DestroyTracer( i_tracer.getHexIndex() );
	}
	
	public void Run()
	{
		// Create my Tracers initially.
		for ( int i = 0 ; i < myTracers.Count ; i++ )
		{
			Vector3 newTracerPos = HexGrid.Manager.GetCoordHex(myTracers[i].HexIndex, 60f);
			GameObject tempTracer = (GameObject) Instantiate(TracerPrefab);
			Tracer tracerScript = tempTracer.GetComponent<Tracer>();
			tracerScript.Set (myTracers[i].HexIndex, newTracerPos, myTracers[i].Delay, myTracers[i].Calibration, myTracers[i].Active);
			SecurityManager.Manager.AddTracer(tempTracer);
			tracerScript._animation.ActiveBase = ActiveBase;
			tracerScript._animation.ActiveCover = ActiveCover;
		}
		
		if ( myType == creatorType.Static && GameManager.Manager.PlayerType == 2)
		{
			for ( int i = 0 ; i < myTracers.Count ; i++ )
			{
				NetworkManager.Manager.CreateTracer(myTracers[i].HexIndex, myTracers[i].Delay, myTracers[i].Calibration, 5000.0f);
			}
		}
		
		run = true;
	}
	
	public void EnableTracerCreator()
	{
		run = true;
	}
	
	public void DisableTracerCreator()
	{
		run = false;
	}
	
	// Update is called once per frame
	void Update () {
		if ( GameManager.Manager.PlayerType == 2 && run && (myType != creatorType.Static) ) // Never duplicate Static Tracers
		{
			myTicker += Time.deltaTime;
			if ( myTicker >= frequency )
			{
				myTicker = 0;
				//Debug.Log("TRACER CREATOR TRIGGER " + myType);
				if ( myType == creatorType.Interval )
				{
					for ( int i = 0 ; i < myTracers.Count ; i++ )
					{
						NetworkManager.Manager.CreateTracer(myTracers[i].HexIndex, myTracers[i].Delay, myTracers[i].Calibration, myTracers[i].Active);
					}
				}
				else if ( myType == creatorType.Randomize )
				{
					for ( int i = 0 ; i < myTracers.Count ; i++ )
					{
						int randomIndex = GetRandomIndex();
						NetworkManager.Manager.CreateTracer(randomIndex, myTracers[i].Delay, myTracers[i].Calibration, myTracers[i].Active);
					}
				}
			}
		}
	}
	
	public void CreateTracer( int i_index, float i_delay, float i_calibration, float i_active)
	{
		Vector3 newTracerPos = HexGrid.Manager.GetCoordHex(i_index, 60f);
		GameObject tempTracer = (GameObject) Instantiate(TracerPrefab);
		Tracer tracerScript = gameObject.AddComponent<Tracer>();
		tracerScript.Set (i_index, newTracerPos, i_delay, i_calibration, i_active);
		SecurityManager.Manager.AddTracer(tempTracer);
		tracerScript._animation.ActiveBase = ActiveBase;
		tracerScript._animation.ActiveCover = ActiveCover;
	}
	
	private int GetRandomIndex ()
	{
		// return index will verify the following qualities:
		// hexIndex must be a valid hex on the current grid.
		// hexIndex must be not be imediately touching any source node
		// hexIndex must not be on a hex where a tracer already exists.
		
		bool valid = false;
		int tryCount = 0;
		int randomIndex = 0;
		
		do 
		{
			randomIndex = (UnityEngine.Random.Range( HexGrid.Manager.rowSize/2 , HexGrid.Manager.gridSize/2))*2;
			valid = CheckHexIndex(randomIndex);
			tryCount ++;
		} while ( !valid && tryCount<100 );
		
		if ( valid )
			return randomIndex;
		else
			return HexGrid.Manager.rowSize+1;
	}
	
	
	private bool CheckHexIndex(int i_hexIndex)
	{
		if ( !HexGrid.Manager.IsHexInBounds( i_hexIndex ))
			return false;
		
		if ( ConnectionManager.Manager.IsHexTouchingSource( i_hexIndex ) )
			return false;
		
		if ( SecurityManager.Manager.IsTracerOnHex( i_hexIndex ) )
			return false;
			
		return true;
	}
	
	
	public bool ContainsTracerOnHex ( int i_hexIndex )
	{
		for ( int i=0 ; i<myTracers.Count ; i++ )
		{
			if ( myTracers[i].HexIndex == i_hexIndex )
				return true;
		}
		return false;
	}
}
