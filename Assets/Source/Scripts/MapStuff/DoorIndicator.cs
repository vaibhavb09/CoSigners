using UnityEngine;
using System.Collections;

public class DoorIndicator : MonoBehaviour 
{
	void Start () 
	{
		if(transform.parent.GetComponent<DoorController>().isLocked)
		{
			SetDoorIndicatorToLocked();
		}
		else
		{
			SetDoorIndicatorToUnlocked();
		}
	}
	
	public void SetDoorIndicatorToLocked()
	{
		renderer.material.color = Color.red;
	}
	
	public void SetDoorIndicatorToUnlocked()
	{
		renderer.material.color = Color.green;
	}
	
	public void SetDoorIndicatorToJammed()
	{
		renderer.material.color = Color.black;
	}
	
}
