using UnityEngine;
using System.Collections;

public class JammerController : MonoBehaviour {
	
	public Transform screen;
	public Transform glowMesh;
	public Material offMat;
	
	bool beginAnimation;
	
	// Use this for initialization
	void Start () 
	{
		beginAnimation = false;
	}
	
	public void DisableJammer()
	{
		beginAnimation = true;
		transform.animation.Play("Turn off");
	}
	
	// Update is called once per frame
	void Update () 
	{
		if( beginAnimation && !transform.animation.isPlaying )
		{
			screen.renderer.enabled = false;
			glowMesh.renderer.material = offMat;
			beginAnimation = false;
		}
	}
}
