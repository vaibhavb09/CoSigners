using UnityEngine;
using System.Collections;

public class Layers : MonoBehaviour {
	
	public GameManager.Layer layer;
	
	public void ChangeDisplay()
	{
		if((GameManager.Manager.CurrentDisplay & layer) == layer)
		{
			gameObject.renderer.enabled = true;
		}
		else
		{
			gameObject.renderer.enabled = false;
		}
	}
}
