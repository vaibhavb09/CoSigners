using UnityEngine;
using System.Collections;

//Had to add this for the laser knockback effect
//because Character controllers don't have a rigid body

public class ImpactScript : MonoBehaviour {
	
	public float mass = 3.0f;
	public Vector3 impact = Vector3.zero;
	private CharacterController character; 
	
	// Use this for initialization
	void Start () 
	{
		character = (CharacterController)gameObject.GetComponent<CharacterController>();		
	}
	
	public void AddImpact( Vector3 dir, float force )
	{
		dir.Normalize();
  		if (dir.y < 0) 
			dir.y = -dir.y; // reflect down force on the ground
 		impact += dir.normalized * force / mass;
	}
	
	// Update is called once per frame
	void Update ()
	{
		// apply the impact force:
 		if (impact.magnitude > 0.2) 
		{
			character.Move(impact * Time.deltaTime);
		}
  		// consumes the impact energy each cycle:
  		impact = Vector3.Lerp(impact, Vector3.zero, 5*Time.deltaTime);
	}
}
