using UnityEngine;
using System.Collections;

public class PowerOnNode : MonoBehaviour 
{
	public int PowerOnNodeID;
	
	Transform Indicator;
	
	public bool poweredOn;

	void Start () 
	{
		poweredOn = false;
		Indicator = transform.FindChild("Indicator");
		Indicator.renderer.material.color = Color.red;
		//NetworkManager.Manager.PowerNode(PowerOnNodeID, poweredOn);
	}
	
	void Update () 
	{
		
	}
	
	void OnTriggerStay(Collider hit)
	{
		if(Input.GetKeyDown(KeyCode.E))
		{
			if(!poweredOn)
			poweredOn = true;
			else
			poweredOn = false;
			
			//NetworkManager.Manager.PowerNode(PowerOnNodeID, poweredOn);
		}
	}
	
	public void ChangePowerNodeState(bool poweredOn)
	{
		if(poweredOn)
		{
			/**********************Indicator turns green***************************/
			
			Indicator.renderer.material.color = Color.green;
			
			/**********************************************************************/
		}
		else
		{
			/**********************Indicator turns red*****************************/
			
			Indicator.renderer.material.color = Color.red;
			
			/**********************************************************************/
		}
	}
}
