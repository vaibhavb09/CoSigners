using UnityEngine;
using System.Collections;

public class PlayerSync : MonoBehaviour {

	// Use this for initialization
	void Start () {
		NetworkManager.Manager.SetPlayer(gameObject);
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		if(GameManager.Manager.PlayerType == 1) // is pointman
		{
			NetworkManager.Manager.SyncPlayerPosition(transform.position);
			NetworkManager.Manager.SyncPlayerRotation(transform.rotation);
		}
	}

	public void SetPosition(Vector3 newPosition)
	{
		transform.position = newPosition;
	}

	public void SetRotation(Quaternion newRotation)
	{
		transform.rotation = newRotation;
	}
}
