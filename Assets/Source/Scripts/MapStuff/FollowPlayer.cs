using UnityEngine;
using System.Collections;

public class FollowPlayer : MonoBehaviour {
	
	private GameObject Target;	
	
	// Use this for initialization
	void Start () {
		Target = GameObject.Find("Playertheif(Clone)");
	}
	
	// Update is called once per frame
	void Update () {
		if(Target == null)
		{
			Target = GameObject.Find("Playertheif(Clone)");
		}
		else
		{
			//tracking it on a 2D plane, only position for now. Need add rotation I suppose
			transform.position = new Vector3(Target.transform.position.x, 60.0f, Target.transform.position.z);		
			transform.rotation = new Quaternion(0 ,Target.transform.rotation.y, 0, Target.transform.rotation.w);
		}
	}
}
