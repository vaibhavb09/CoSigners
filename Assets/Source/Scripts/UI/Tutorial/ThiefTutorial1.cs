using UnityEngine;
using System.Collections;

public class ThiefTutorial1 : MonoBehaviour 
{
	int currentEvent = -1;
	bool LockdownOccured = false;
	GameObject t_openDoor;
	
	void Start () 
	{
		t_openDoor = GameObject.Find ("Tutorial2OpenDoor");
	}
	
	void Update () 
	{	
		if( t_openDoor != null )
		if ( !LockdownOccured && t_openDoor.GetComponent<DoorController>().isOpen )
		{
			LockdownOccured = true;
			NetworkManager.Manager.IncreaseThreatAmount((int)(HackerThreat.Manager.MaxThreat - HackerThreat.Manager.Threat));
		}
	}

	void OnTriggerExit( Collider hit )
	{	
		
	}
}
