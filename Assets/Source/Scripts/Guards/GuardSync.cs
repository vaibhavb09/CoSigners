using UnityEngine;
using System.Collections;

public class GuardSync : Photon.MonoBehaviour {
	
	public GameObject guard2DPrefab;
	private GameObject _guard2DPrefab;
	Vector3 _lastPos;
	Quaternion _lastRot;
	public int GuardId;
	float MiniMovement = 0.05f;
	
	// Use this for initialization
	void Start () {
		_guard2DPrefab = (GameObject)Instantiate(guard2DPrefab, 
		new Vector3(transform.position.x, 50.95f, transform.position.z) , transform.rotation);
		_guard2DPrefab.GetComponent<FollowGuard>().SetTarget(this.transform);
		_guard2DPrefab.renderer.material.color = Color.red;
		_guard2DPrefab.renderer.enabled = false;
	
	}

	public void Initialize()
	{
		_guard2DPrefab = (GameObject)Instantiate(guard2DPrefab, 
		                                         new Vector3(transform.position.x, 50.95f, transform.position.z) , transform.rotation);
		_guard2DPrefab.GetComponent<FollowGuard>().SetTarget(this.transform);
		_guard2DPrefab.renderer.material.color = Color.red;
		_guard2DPrefab.renderer.enabled = false;
	}
	
	public void EnableGuard2DRenderer()
	{
		_guard2DPrefab.renderer.enabled = true;
	}
	
	public void DisableGuard2DRenderer()
	{
		_guard2DPrefab.renderer.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		
		if(GameManager.Manager.PlayerType == 1) // is pointman
		{
			//transform.RotateAround(Vector3.up, 0.01f);
			if (Vector3.Distance(transform.position, _lastPos) > MiniMovement)
			{
			    _lastPos = transform.position;
				//Have to pass a extra data, doesn't worth it.
				NetworkManager.Manager.UpdateGuardPosition(transform.position, GuardId);
			    //photonView.RPC("SetPosition", PhotonTargets.Others, transform.position);
			}
			if(Quaternion.Angle(transform.rotation, _lastRot) > MiniMovement)
			{
				_lastRot = transform.rotation;
				NetworkManager.Manager.UpdateGuardRotation(transform.rotation, GuardId);
				//photonView.RPC("SetRotation", PhotonTargets.Others, transform.rotation);
			}
		}
		
		// For IR Display Check
		if(GameManager.Manager.PlayerType == 2) // is hacker
		{
			/*
			if (IRSystem.ISystem.IsIRActive)
			{
				if(IRSystem.ISystem.HasActiveIRNearBy(this.transform.position))
				{
					Debug.Log("Happened?");
					EnableGuard2DRenderer();
				}
				else
				{
					DisableGuard2DRenderer();
				}
			}
			else
			{
				if(_guard2DPrefab.renderer.enabled == true)
					DisableGuard2DRenderer();
			}
			*/
		}
	}
	
	//[PunRPC]
	public void SetPosition(Vector3 newPosition)
	{
	    transform.position = newPosition;
	}
	
	//[PunRPC]
	public void SetRotation(Quaternion newRotation)
	{
	    transform.rotation = newRotation;
	}
}
