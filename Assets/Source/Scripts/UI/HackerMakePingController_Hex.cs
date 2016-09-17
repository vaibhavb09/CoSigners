using UnityEngine;
using System.Collections;

public class HackerMakePingController_Hex : MonoBehaviour {
	
	public float AnimateTime;
	public Vector3 StartScale;
	public Vector3 EndScale;
	
	private float _startTime;
	private Vector3 _scale;
	private bool _set;
	
	
	// Use this for initialization
	void Start () {
		// Make object invisible first
		AnimateTime = 2.5f;
		gameObject.renderer.enabled = false;
		_set = false;
		transform.localScale = StartScale;
		_startTime = Time.time;
		
	}
	
	// Update is called once per frame
	void Update () {
		float delta = Time.time - _startTime;
		float percent = delta/AnimateTime;
		
		Animate( percent );
		
		if( delta > AnimateTime )
		{
			Finish();
		}	
	}
	
	void Animate(float i_percent)
	{
		//Debug.Log ("Animating");
		if(!_set)
		{
			_set = true;
			gameObject.renderer.enabled = true;
		}
		
		float myValue = (1.0f-i_percent)*0.3f;
		Color newColor = new Color(myValue, myValue, myValue, 1.0f);

		renderer.material.SetColor("_TintColor", newColor);
	}
	
	void Finish()
	{
		Destroy(gameObject);
	}
}
