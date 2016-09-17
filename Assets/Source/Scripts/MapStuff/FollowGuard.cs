using UnityEngine;
using System.Collections;

public class FollowGuard : MonoBehaviour {
	
	private Transform _target;
	
	public void SetTarget(Transform i_target)
	{
		_target = i_target;
	}
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(_target == null)
		{
			//Target = GameObject.Find("playerPrefab(Clone)");
		}
		else
		{
			//tracking it on a 2D plane, only position for now. Need add rotation I suppose
			transform.position = new Vector3(_target.transform.position.x, 60.0f, _target.transform.position.z);
			
			transform.rotation = new Quaternion(0 ,_target.transform.rotation.y, 0, _target.transform.rotation.w);
		}
	}
}
