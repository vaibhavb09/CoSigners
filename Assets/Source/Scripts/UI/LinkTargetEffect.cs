using UnityEngine;
using System.Collections;

public class LinkTargetEffect : MonoBehaviour {
	
	private float ticker = 0;
	private float timer = 0.05f;
	private float AnimationRate = 0.005f;
	private float newOffset = 0;
	public bool clockwise = false;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		//Debug.Log ("Rotating");
		/*
		if ( clockwise )
		{
			this.transform.Rotate( new Vector3(0.0f, 0.5f, 0.0f));
		}
		else
		{
			this.transform.Rotate( new Vector3(0.0f, -0.5f, 0.0f));
		}
		*/
		//transform.Rotate( transform.position, 0.5f);
		/*
		ticker += Time.deltaTime;
		if ( ticker > timer )
		{
			ticker = 0;
			newOffset += AnimationRate;
	        if( renderer.enabled )
	        {
				transform.Rotate(0.0f, AnimationRate, 0.0f);
	        }
		}*/
	}
	
}
