using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LaserManager : MonoBehaviour {
	
	public List<GameObject> lasers;
	public GameObject laserBeamPrefab;
	
	#region Singleton Declaration
	private static LaserManager m_instance;

	public static LaserManager Manager
	{
		get
		{
			if(m_instance == null)
			{
				m_instance = new LaserManager();			
			}
			return m_instance;
		}
	}
	
	public LaserManager() 
    { 
        m_instance = this;
		lasers = new List<GameObject>();
    }
	#endregion
	
	// Use this for initialization
	void Start () 
	{

	}
	
	public void LoadLasers( GraphData i_gData )
	{
		laserBeamPrefab = (GameObject) Resources.Load("Prefabs/Theif/Laser");
		LaserData[] laserData = i_gData.Lasers;
		foreach(LaserData laser in laserData)
		{
			Vector3 pointA = GetWorldPosition( new Vector3( laser.pointAX, laser.pointAY, laser.pointAZ ) );
			Vector3 pointB = GetWorldPosition( new Vector3( laser.pointBX, laser.pointBY, laser.pointBZ ) );
			CreateLaserBetweenPoints( pointA, pointB, laser.groupID );
		}
	}
	
	#region public network API
		
	public void ActivateGroup( int i_groupID )
	{
		GameObject[] _lasers = GameObject.FindGameObjectsWithTag("Laser");
		foreach( GameObject laser in _lasers )
		{
			if( laser.GetComponent<LaserController>().groupID == i_groupID )
				laser.GetComponent<LaserController>().ActivateLaser();
		}
	}
	
	public void DeactivateGroup( int i_groupID )
	{
		//Debug.Log( "Deactivate group in LM: " +i_groupID );
		GameObject[] _lasers = GameObject.FindGameObjectsWithTag("Laser");
		foreach( GameObject laser in _lasers )
		{
			if( laser.GetComponent<LaserController>().groupID == i_groupID )
				laser.GetComponent<LaserController>().DeactivateLaser();
		}
	}
	
	public bool IsGroupActive( int i_groupID )
	{
		foreach( GameObject laser in lasers )
		{
			if( laser.GetComponent<LaserController>().groupID == i_groupID )
			{
				if(laser.GetComponent<LaserController>().isActive) // Only one laser of the group needs to be checked
					return true; 
				else 
					return false;
			}
		}
		return false;
	}
	
	#endregion

	//drawbeam

	public Transform DrawLaser( Vector3 pointA, Vector3 pointB )
	{
		GameObject thisObject = (GameObject)Instantiate( laserBeamPrefab, pointA, Quaternion.identity);
		thisObject.transform.position = pointA;
		thisObject.transform.LookAt(pointB);
		//Disabling this temporarily
		thisObject.transform.renderer.enabled = false;
		return thisObject.transform;
	}

	public void CreateLaserBetweenPoints( Vector3 pointA, Vector3 pointB, int i_groupID )
	{

		const float navMeshOffset = 2.0f;
		float distance = Vector3.Magnitude( pointA - pointB );
		Vector3 direction = pointA - pointB;
		direction.Normalize();
		Vector3 center = (pointA + pointB)/2;
		
		GameObject laser = GameObject.CreatePrimitive( PrimitiveType.Cylinder );
		laser.transform.localScale += new Vector3( -0.85f, distance/2 - 1, -0.85f);
		laser.transform.LookAt( direction );
		laser.transform.rotation *= Quaternion.Euler( 90.0f, 0.0f, 0.0f ); 
		laser.transform.position = center;
		laser.tag = "Laser";
		laser.name = "Laser";
		laser.layer =  LayerMask.NameToLayer("ignoreGuards");
		laser.renderer.enabled = false;

		/*
		GameObject navMeshEmptyObject = GameObject.CreatePrimitive( PrimitiveType.Cylinder );
		navMeshEmptyObject.transform.rotation *= Quaternion.Euler( 90.0f, 0.0f, 0.0f ); 
		navMeshEmptyObject.transform.position = new Vector3( center.x, center.y - navMeshOffset , center.z );
		navMeshEmptyObject.transform.localScale += new Vector3( distance - 1 , navMeshOffset , distance -1 );
		navMeshEmptyObject.AddComponent<NavMeshObstacle>();
		
		navMeshEmptyObject.renderer.enabled = false;
		navMeshEmptyObject.collider.enabled = false;
		navMeshEmptyObject.name = "LaserNavMeshObject";
		navMeshEmptyObject.layer = LayerMask.NameToLayer("ignoreGuards");*/
		
		laser.AddComponent("LaserController");
		//laser.GetComponent<LaserController>().Load(i_groupID, navMeshEmptyObject.transform, true);
		laser.GetComponent<LaserController>().Load(i_groupID, null, true, DrawLaser( pointA, pointB ));
		laser.GetComponent<CapsuleCollider>().radius = 2;
		laser.GetComponent<CapsuleCollider>().isTrigger = true;
		
		lasers.Add( laser );
	}
	
	public Vector3 GetWorldPosition( Vector3 pos )
	{
		RaycastHit hit;
		float cHeight = 0.0f;
		float yFactor = pos.y;
		pos.y = 0.0f;
		if( Physics.Raycast( pos, Vector3.up, out hit, 100.0f ) )
		{
			cHeight = hit.distance;
			//Debug.Log( "ceiling ht:"  +cHeight );
		}
		return new Vector3( pos.x, yFactor * cHeight, pos.z );
	}
	

	
	// Update is called once per frame
	void Update () 
	{
	
	}
}
