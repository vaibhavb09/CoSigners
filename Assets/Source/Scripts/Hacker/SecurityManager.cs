using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SecurityManager : MonoBehaviour {
	
	
	private static SecurityManager _instance; 
	//private List<TracerCreator> myTracerCreators;
	private List<GameObject> myTracerCreators;
	private List<GameObject> liveTracers;
	private Object TracerPrefab;
	private Object TCPrefab;
	private float _damageAmount;
	
	// Use this for initialization
	void Start () 
	{
		_instance = this;
		_damageAmount = 33.0f;
	}
	
	public static SecurityManager Manager
	{
		get
		{
			if(_instance == null)
			{
				_instance = new SecurityManager();			
			}
			return _instance;
		}
	}

	
	public SecurityManager () 
    { 
        _instance = this;
    }
	
	public float Damage
	{
		get{
			return _damageAmount;
		}
	}
	
	public List<GameObject> GetLiveTracers()
	{
		
		return liveTracers;
	}
	
	public void EnableTracerCreators()
	{
		if( myTracerCreators != null )
		foreach(GameObject creator in myTracerCreators)
		{
			TracerCreator TCScript = creator.GetComponent<TracerCreator>();
			TCScript.EnableTracerCreator();
		}
		//Debug.Log("#Max:Tracer creator enabled");
	}
	
	public void DisableTracerCreators()
	{
		if( myTracerCreators != null )
		foreach(GameObject creator in myTracerCreators)
		{
			TracerCreator TCScript = creator.GetComponent<TracerCreator>();
			TCScript.DisableTracerCreator();
		}
		//Debug.Log("Tracer creator disabled");
	}
	
	public void LoadTracers(GraphData i_gData)
	{
		// initialize list
		TracerPrefab = Resources.Load("Prefabs/Hacker/Graph/Tracer_Prefab", typeof(Object)) as Object;
		TCPrefab = Resources.Load("Prefabs/Hacker/TracerCreator", typeof(Object)) as Object;
		myTracerCreators = new List<GameObject>();
		liveTracers = new List<GameObject>();
		TracerCreatorData[] tracerCreatorData = i_gData.TracerCreators;
		foreach(TracerCreatorData tracerCreators in tracerCreatorData)
		{
			//Debug.Log ("About to create a tracer creator");
			// Create new Tracer Creator
			GameObject thisTC = (GameObject) Instantiate(TCPrefab);
			TracerCreator TCScript = thisTC.GetComponent<TracerCreator>();
			TCScript.Set(tracerCreators.Type, tracerCreators.Frequency);
			myTracerCreators.Add( thisTC );

			TracerData[] tracers = tracerCreators.Tracers;
			foreach(TracerData tracer in tracers)
			{
				TCScript.AddTracer (tracer);
			}
			TCScript.Run();
		}
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	
	
	public void ScrambleTracerAtHexIndex(int i_hexIndex)
	{
	
		for ( int i = 0 ; i < liveTracers.Count ; i++ )
		{
			if( liveTracers[i].GetComponent<Tracer>().getHexIndex()==i_hexIndex)
			{
				//Debug.Log("Calling Scramble");
				liveTracers[i].GetComponent<Tracer>().Scramble();
				break;
			}
		}
		
	}
	

	
	
	public void DeScrambleTracers(int i_hexIndex)
	{
		//Looking for tracers to deactivate
		List<int> surroundingHexes=HexGrid.Manager.GetSurroundingHexes(i_hexIndex);
		
		for(int i=0;i<surroundingHexes.Count;i++)
		{
			for(int j=0;j< liveTracers.Count;j++)
			{
				//Debug.Log("surround:"+surroundingHexes[i]);
				
				if(liveTracers[j].GetComponent<Tracer>().getHexIndex()==surroundingHexes[i])
				{
					liveTracers[j].GetComponent<Tracer>().DeScramble();
				}
			}
		}
	}
	
	public void AddTracer( GameObject i_tracer)
	{
		liveTracers.Add( i_tracer );
	}
	
	
	public void DestroyTracer( int i_tracer )
	{
		if( liveTracers != null )
		for ( int i=0 ; i<liveTracers.Count ; i++ )
		{
			bool found = false;
			if ( liveTracers[i].GetComponent<Tracer>().getHexIndex() == i_tracer )
			{
				found = true;
				liveTracers.Remove(liveTracers[i]);	
			}
			if ( found)
				break;
		}
	}

	
	
	public void DisableTracer()
	{
		for(int i=0;i<liveTracers.Count;i++)
		{
			DestroyTracer(liveTracers[i].GetComponent<Tracer>().getHexIndex() );
			
		}
		
	}
	
	public void CreateTracer( int i_index, float i_delay, float i_calibration, float i_active)
	{
		//Debug.Log ("!!!!!!!!!!!!!!!!!Create Tracer without Tracer Creator.");
		Vector3 newTracerPos = HexGrid.Manager.GetCoordHex(i_index, 60f);
		GameObject tempTracer = (GameObject) Instantiate(TracerPrefab);
		Tracer tracerScript = tempTracer.GetComponent<Tracer>();
		tracerScript.Set (i_index, newTracerPos, i_delay, i_calibration, i_active);
		AddTracer(tempTracer);

		Material ActiveBase = Resources.Load("Materials/Hacker/Hexes/Tracer_Active_Base", typeof(Material)) as Material;
		Material ActiveCover = Resources.Load("Materials/Hacker/Hexes/Tracer_Active_Cover", typeof(Material)) as Material;
		tracerScript._animation.ActiveBase = ActiveBase;
		tracerScript._animation.ActiveCover = ActiveCover;
	}
	
	
	public bool IsTracerOnHex( int i_hexIndex )
	{
		for ( int i=0 ; i<liveTracers.Count ; i++ )
		{
			if ( liveTracers[i].GetComponent<Tracer>().getHexIndex() == i_hexIndex )
			{
				return true;
			}
		}
		return false;
	}
	
	
	public TracerCreator.creatorType TypeOfTracerOnHex( int i_hexIndex )
	{
		for ( int i=0 ; i<myTracerCreators.Count ; i++ )
		{
			TracerCreator script = (TracerCreator) myTracerCreators[i].GetComponent<TracerCreator>();
			if ( script.ContainsTracerOnHex( i_hexIndex ) )
			{
				return script.Type;
			}
		}
		return TracerCreator.creatorType.Static;
	}
	
}
