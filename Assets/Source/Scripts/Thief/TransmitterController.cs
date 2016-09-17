using UnityEngine;
using System.Collections;

public class TransmitterController : MonoBehaviour 
{
	public bool initialTransmitter;
	
	void Start () 
	{
		ActivateTransmitter();
	}
	
	public void ActivateTransmitter()
	{
		transform.animation.Play("Opening");
		transform.animation.PlayQueued("Running");
	}
	
	public void ResetTransmitter()
	{
	}
	
	public void DeactivateTransmitter()
	{
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
}
