using UnityEngine;
using System.Collections;

public class HackerPingEffectController : MonoBehaviour {
	
	public float LastTime = 6.0f;
	private float _startTime;
	private Vector3 _scale;
	private bool _set;
	private float _startScale;
	
	
	void SetSize(float i_float)
	{
		if(!_set)
		{
			_set = true;
			gameObject.renderer.enabled = true;
		}
		transform.localScale = (_scale * i_float);
		//Debug.Log("Updating PingCircle scale :" + transform.localScale);
		if (transform.localScale.x > 0.3f)
		{
			_startTime = Time.time;
			
		}
	}
	
	// Use this for initialization
	void Start () {
		_startTime = Time.time;
		_scale = transform.localScale ;
		//_startScale = _scale;
		_set = false;
		
	}
	
	// Update is called once per frame
	void Update () {
		//Debug.Log("Updating PingCircle");
		SetSize( (Time.time - _startTime) * 2.5f);
	}
	
	public void Finished()
	{
		Destroy(gameObject);
	}
}
