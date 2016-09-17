using UnityEngine;
using System.Collections;

public class ThiefRecievePingController : MonoBehaviour {
	
	public float AnimateTime;
	
	private float _startTime;
	private Vector3 _scale;
	private bool _set;
	
	
	// Use this for initialization
	void Start () {
		// Make object invisible first
		gameObject.renderer.enabled = false;
		_set = false;
		_startTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () 
	{
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
		
		float deg = (i_percent*180)*3;
		float rad = deg * Mathf.Deg2Rad; 
		float opacity = Mathf.Abs(Mathf.Sin(rad));
		
		//Debug.Log ("Percent: " + i_percent + "Degrees: " + deg + " - opacity: " + opacity);
		
		Color newColor = new Color(1, 1, 1, ((opacity*0.75f)+0.25f) );
        transform.renderer.material.color = newColor;
		
		
	}
	
	void Finish()
	{
		Destroy(gameObject);
	}
}
