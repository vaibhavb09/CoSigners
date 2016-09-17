using UnityEngine;
using System.Collections;

public class ObjectPingAnimation : MonoBehaviour 
{
	public float PingDuration = 3.0f;
	public float FadeTime = 1.0f;
	private float FadeStartTime;
	private float PingTime;
	private Color color;
	
	void Start () 
	{
		PingTime = Time.time;
		color = new Color( 1.0f, 1.0f, 1.0f, 1.0f );
		transform.renderer.material.color = color;
	}
	
	void Update () 
	{
		//FadeIn();
		if( Time.time - PingTime >= PingDuration )
		{
			//if( FadeOut() )
			//color = new Color(1,1,1,Mathf.Lerp(1.0f, 0.0f, FadeTime));
			Finish();
		}
	}
	/*
	public bool FadeIn()
	{
		
		if( gameObject.renderer.material.color.a == 0 )
			FadeStartTime = Time.time;
		
		color.a += Time.deltaTime / FadeTime;
		gameObject.renderer.material.color = color;
		
		if( gameObject.renderer.material.color.a >= 0.95f )
			return true;
		else 
			return false;
	}
	*/
	/*
	public bool FadeOut()
	{
		if( gameObject.renderer.material.color.a >= 0.95f )
			FadeStartTime = Time.time;
		
		color.a -= Time.deltaTime / FadeTime;
		gameObject.renderer.material.color = color;
		
		
		
		Debug.Log ("Alpha:" + gameObject.renderer.material.color.a);
		
		if( gameObject.renderer.material.color.a <= 0.5f )
			return true;
		else 
			return false;
	}
	*/

	public void Finish()
	{
		Destroy ( gameObject );	
	}
}
