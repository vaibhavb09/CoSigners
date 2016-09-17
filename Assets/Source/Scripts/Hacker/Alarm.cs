using UnityEngine;
using System.Collections;

public class Alarm : MonoBehaviour {
	
	public GameObject GlowMesh;
	private bool _triggered = false;
	
	//private float _alarmRotationSpeed = 200.0f;
	
	public void Trigger()
	{
		animation.Play("Alarm_Dropdown");
		animation.PlayQueued("Alarm_Spinning");
		_triggered = true;
	}
	
	public void Disable()
	{
		_triggered = false;
		GlowMesh.renderer.enabled = false;
		animation.Play("Alarm_Close");
	}
	
	// Use this for initialization
	void Start () {
		//Trigger();
	}
	
	// Update is called once per frame
	void Update () {
		
		if(_triggered)
		{
			if(!animation.IsPlaying("Alarm_Dropdown"))
			{
				GlowMesh.renderer.enabled = true;
			}
		}
	}
}
