using UnityEngine;
using System.Collections;

public class LaserController : MonoBehaviour 
{

	//Prototype script for lasers. This needs to be designed properly first.

	//private Transform navMeshObject;
	private Transform laserRenderer;
	
	public bool isActive;
	public int groupID;
	public float increasedAlertLevelCooldown;
	public float alertLevelIncreaseAmount;

	private Transform guard;
	private float startTime;
	private float ogSpeed;
	private float navMeshDelay;
	private bool updateNavMesh;
	
	// Use this for initialization
	void Start () 
	{
		
	}
	
	#region public API
	
	public void Load( int i_groupID, Transform i_navMeshObject, bool i_isActive, Transform i_laserRenderer )
	{
		groupID = i_groupID;
		isActive = i_isActive;
		//navMeshObject = i_navMeshObject;
		laserRenderer = i_laserRenderer;

		if( isActive )
			ActivateLaser();
		else
			DeactivateLaser();
	}
	
	public void ActivateLaser()
	{
		isActive = true;
		//laserRenderer.GetComponent<LaserBeam>().maxLength = 15;
		//laserRenderer.renderer.enabled = true;
		transform.renderer.enabled = true;
		collider.enabled = true;
		startTime = Time.time;
		updateNavMesh = true;
	}
	
	public void DeactivateLaser()
	{
		isActive = false;
		//laserRenderer.GetComponent<LaserBeam>().maxLength = 1;
		//laserRenderer.renderer.enabled = false;
		transform.renderer.enabled = false;
		collider.enabled = false;
		startTime = Time.time;
		updateNavMesh = true;
	}
	
	#endregion
	
	void SetNavMeshObstacle( bool value )
	{
		//Debug.Log( "Is nav mesh enabled : "+value );
		//navMeshObject.GetComponent<NavMeshObstacle>().enabled = value;
	}
	
	void KnockBack( GameObject i_player )
	{
		Transform _camera = (Transform)i_player.transform.FindChild("FPSCamera");
		Vector3 movementDir = _camera.parent.GetComponent<CharacterController>().velocity;
		Vector3 randomJitter = new Vector3( Random.Range(-0.1f,0.1f), Random.Range(-0.4f,0.4f), Random.Range(-0.1f,0.1f) );
		Vector3 cameraJitter = new Vector3( Random.Range(-0.2f,0.2f), Random.Range(-0.2f,0.2f), Random.Range(-0.2f,0.2f) );
		Vector3 impactDir = -1 * movementDir + randomJitter;
		impactDir.Normalize();
		_camera.Rotate( cameraJitter );
		i_player.GetComponent<ImpactScript>().AddImpact( impactDir , 5.0f );
	}
	
	void MessUpGuard( GameObject i_guard )
	{      
		//Animation stuff..
		guard = i_guard.transform;
		ogSpeed = i_guard.GetComponent<NavMeshAgent>().speed;
		i_guard.GetComponent<NavMeshAgent>().speed = 1.0f;
	}
	
	void MessUpPlayer( GameObject i_player )
	{
		//Debug.Log("Mess up player");
		KnockBack(i_player);
		NetworkManager.Manager.BoostAlertLevelForTime( increasedAlertLevelCooldown, alertLevelIncreaseAmount );
	}
	
	void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.tag == "Player")
		{
			MessUpPlayer( other.gameObject );		
		}
		else if( other.gameObject.tag == "Guard" )
		{
			MessUpGuard( other.gameObject );
		}
		
	}
	
	void OnTriggerStay(Collider other)
	{
		if(other.gameObject.tag == "Player")
		{
			MessUpPlayer( other.gameObject );		
		}
		else if( other.gameObject.tag == "Guard" )
		{
			MessUpGuard( other.gameObject );
		}
		
	}
	
	void OnTriggerExit(Collider other)
	{
		if( other.gameObject.tag == "Guard" )
		{
			guard = null;
			other.gameObject.GetComponent<NavMeshAgent>().speed = ogSpeed;
			//Debug.Log( "Guard left" );
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		if( guard != null && !isActive ) //Guard got slowed but laser was turned off, so he was never set back to og speed.
		{
			guard.GetComponent<NavMeshAgent>().speed = 3.0f;
			guard = null;
		}
		
		if( updateNavMesh )
		{
			
			if( (Time.time - startTime) > navMeshDelay )
			{
				updateNavMesh = false;
				
				if( isActive )
					SetNavMeshObstacle(true);
				else
					SetNavMeshObstacle(false);
			}
			
		}
		
	}
}
