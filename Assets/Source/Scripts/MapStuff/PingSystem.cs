using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PingSystem : MonoBehaviour {
	
	private static PingSystem m_instance;
	
	public int _maxPings;
	public float _pingDuration=2.0f;
	public float _pingCircleRadius;
	
	private float _pingStartTime;
	private float _pingEndTime;
	
	
	//public GameObject ThiefPingPrefab;
	public GameObject HackerPingPrefab;
	
	
	private GameObject _thiefPing;
	private GameObject _hackerPing;
	
	private bool _active;
	private int _currentIndex;
	private Transform _pingTransform;
	
	// Use this for initialization
	void Start () {
		_currentIndex = 0;
		_active = false;
		}
	

	public void CreatePing(Transform _inTransform,Vector3 _hitPosition)
	{	
		//if (_currentIndex < _maxPings)
		{
			if(_active == true)
			{
			  DestroyPing();
			}
			
		_active = true;
		_currentIndex++;

	//	_pingTransform = _inTransform;
			_pingStartTime = Time.time;
			_pingEndTime = _pingStartTime + _pingDuration;
			
		//Debug.Log("Creating hacker ping");
		//_hackerPing = (GameObject)Instantiate(HackerPingPrefab,new Vector3 (_hitPosition.x,_hitPosition.y + 4.0f,_hitPosition.z),Quaternion.identity);
		//Debug.Log("Hacker ping pos:" + _hackerPing.transform.position);	
		UIManager.Manager.CreatePingCircleatHackerPingPosition(new Vector3 (_hitPosition.x,_hitPosition.y + 3.5f,_hitPosition.z) , _pingCircleRadius);

			//NetworkManager.Manager.SendPing(_hitPosition);
		//_thiefPing = (GameObject)Instantiate(ThiefPingPrefab, PingList[_currentIndex++].position, Quaternion.identity);		
		}
	}
	
	void Update()
	{	
	if (_active )
		{
			if (Time.time >= _pingEndTime)
			{
				//Debug.Log("PING SHOULD DIE");
				DestroyPing();
			}
		}
	}

	public void DestroyPing()
	{	
		//Destroy(_hackerPing);
		UIManager.Manager.DeletePingCircleatHackerPingPosition();
		_active = false;
	}			
		
	
	
	public PingSystem () 
    {
        m_instance = this;
    }

	public bool getPingStatus()
	{
		return _active;
	}

	public Transform getPingTransform()
	{
		return _pingTransform;
	}

	public float getPingStartTime()
	{
		return _pingStartTime;
	}

	public float getNoOfPings()
	{
		return _currentIndex;
	}
	
	public static PingSystem PSystem
	{
		get
		{
			if(m_instance == null)
			{
				m_instance = new PingSystem();			
			}
			return m_instance;
		}
	}
	
}
