using UnityEngine;
using System.Collections;

public class GuardPerception : MonoBehaviour { 


	// Essential

	// Public interface
	public bool seenPlayer;

	
	#region AI 2.0 members
	
	public Vector3 thieflastposition;
	
	#endregion

	// Imp

	// Indicates the total perception strength obtained by adding visual and audiotory senses
	public int perceptionStrength;

	// Indicates the perception strength of the visual sense
	public int visualPerceptionStrength;
	
	// Indicates the perception strength of the auditory sense
	public int auditoryPerceptionStrength;

	// Extraeneous

	#region Properties
	public bool Seen
	{
		get{
			return seenPlayer;
		}
	}



	GameObject OverLord;
	GameObject Player;
	GameObject FPSCamera;
	
	public float maxDistance;
	public float maxCosineVision;
	public GUIStyle MyStyle;
	public string guardName;
	
	private Vector3 rayDirection;
	private Vector3 enemyDirection;

	private float angleDot;
	private bool playerInFrontOfEnemy;
	private bool playerInStraightView;
	private bool playerInSideView;
	private bool playerCloseToEnemy;
	
	private LayerMask mask;
	
	// is set to true if the player was heard
	private bool heardPlayer;
	
	private int lostplayer;
	
	private float semiMajorAxis;
	private float semiMinorAxis;
	private float MaxAngleVision;
	private Vector3 ellipseCenter;
	private float focalDistance;
	private Vector3 f1,f2;
	
	private Vector3 raycastPosition;

	int m_delta_caught;
	
// DEBUG CODE
	//private LineRenderer lineRenderer;

	
	/// <summary>
	/// Gets a value indicating whether this <see cref="GuardPerception"/> was player heard.
	/// </summary>
	/// <value>
	/// <c>true</c> if was player heard; otherwise, <c>false</c>.
	/// </value>
	public bool wasPlayerHeard
	{
		get{
			return heardPlayer;
		}
	}
	#endregion
	
	// Use this for initialization
	void Start () 
	{
		OverLord = GameObject.FindGameObjectWithTag("GuardOverlord");
		//Player = GameObject.FindGameObjectWithTag("Player");
		
		Player = GameObject.Find("Playertheif(Clone)");
		
		FPSCamera = GameObject.Find("FPSCamera");
		
		m_delta_caught = 10;
		
		mask =0;
		
		// Add the Ignore raycast layer to the mask (Layer to name doesnt work for some reason !)
		mask = 1 << 2;
		
		// Add the ignore guards layer to the mask (Layer to name doesnt work for some reason !)
		mask = 1 << 10;
		
		// Ad dthe see through ping layer to the ignore for guards
		mask = 1 << 11;
		
		// invert the mask (the mask now collides with everything that is not on the "ignoreGuards" and "Ignore Raycast" layer
		mask = ~mask;
		
		
		perceptionStrength = 0;
		visualPerceptionStrength = 0;
		auditoryPerceptionStrength = 0;
		
		// DEBUG
		//lineRenderer = gameObject.AddComponent<LineRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
		
		if(Player == null || FPSCamera == null)
		{
			//Player = GameObject.FindGameObjectWithTag("Player");
			Player = GameObject.Find("Playertheif(Clone)");
			
			FPSCamera = GameObject.Find("FPSCamera");
		}
		else
		{
			
			raycastPosition = new Vector3(transform.localPosition.x,FPSCamera.transform.position.y+0.5f,transform.localPosition.z);
			
//			lineRenderer.SetPosition(0,raycastPosition);
//			lineRenderer.SetPosition(1,FPSCamera.transform.position);
				
			rayDirection = (FPSCamera.transform.position - raycastPosition).normalized;
			enemyDirection = transform.TransformDirection(Vector3.forward);
			angleDot = Vector3.Dot(rayDirection, enemyDirection);
			//Debug.Log("Player.transform.localPosition -" + Player.transform.localPosition +  "transform.localPosition - " + transform.localPosition + " angleDot - " + angleDot);
			semiMajorAxis = maxDistance/2.0f;
			MaxAngleVision = 1.04f;
			semiMinorAxis = semiMajorAxis * Mathf.Tan(MaxAngleVision/2.0f);
			focalDistance = Mathf.Sqrt( (semiMajorAxis * semiMajorAxis) - (semiMinorAxis * semiMinorAxis));
			f1 = transform.localPosition + transform.TransformDirection(Vector3.forward) * ( semiMajorAxis - focalDistance) ;
			f2 = transform.localPosition + transform.TransformDirection(Vector3.forward) * ( semiMajorAxis + focalDistance) ;		
			playerInStraightView = ( ( Vector3.Distance(Player.transform.localPosition,f1 ) ) + ( Vector3.Distance(Player.transform.localPosition,f2 )) <= 2*semiMajorAxis);
			
			semiMinorAxis = maxDistance/2.0f;
			semiMajorAxis = semiMinorAxis/Mathf.Tan(MaxAngleVision/2.0f);
			
			playerInSideView = (
				                    (((Player.transform.localPosition.x - transform.localPosition.x) * (Player.transform.localPosition.x - transform.localPosition.x))/(semiMajorAxis * semiMajorAxis)) +
				                    (((Player.transform.localPosition.z - transform.localPosition.z) * (Player.transform.localPosition.z - transform.localPosition.z))/(semiMinorAxis * semiMinorAxis))
					 				) <= 1;
						
			playerInFrontOfEnemy = angleDot > maxCosineVision;
			//Debug.Log("playerInStraightView - " + playerInStraightView + " playerInSideView - " + playerInSideView + " playerInFrontOfEnemy - " + playerInFrontOfEnemy + " maxDistance " + maxDistance);
			if ( playerInStraightView || (playerInSideView && playerInFrontOfEnemy))
			{ 	
		    	RaycastHit hit;
		    	if (Physics.Raycast (raycastPosition,rayDirection, out hit, maxDistance,mask) 
		    		&& hit.collider.gameObject==Player) 
		    	{
					//Debug.Log("Seen PLayer!!!!!!!!!!!  ");
		    	//enemy sees player
					seenPlayer = true;
					thieflastposition = Player.transform.localPosition;

					if (  playerInStraightView )
					{
						visualPerceptionStrength = 100;
					}
					else if ( angleDot > 0.5f)   // 60 degress
					{
						visualPerceptionStrength = 50;
					}
					else if ( angleDot > 0.259f) // 75 degrees
					{
						visualPerceptionStrength = 34;
					}
		    	} 
				else
				{
					visualPerceptionStrength = 0;
					seenPlayer = false;
				}
		    		
			}
			else
			{
				visualPerceptionStrength = 0;
				seenPlayer = false;
			}
			
			// HACK FOR GUARDS NOT HEARING PLAYER
			auditoryPerceptionStrength = 0;
			
			// Calculating the total perception strength for this guard
			perceptionStrength = (visualPerceptionStrength + auditoryPerceptionStrength);
			//perceptionStrength = (auditoryPerceptionStrength);
			
			// indicate if the player was heard
			heardPlayer = auditoryPerceptionStrength > 0;
			
			// if the player was heard, indicate that position
			if(heardPlayer)
			{
				thieflastposition = Player.transform.localPosition;
			}
			
			// cap the value at 100
			perceptionStrength = perceptionStrength > 100 ? 100 : perceptionStrength;
			
			// reset the auditory perception strength
			auditoryPerceptionStrength = 0;
		}
	}
	
	/**
	 * @Description : Method to be called by the overlord when the theif was heard 
	 * @Param : Indicates the intensity with which the theif was heard, ranges from 0 to 1 
	 * 			where 0 is outside the guards hearing radius and 1 is right on the theif.
	 * */
	public bool heardTheif(float iIntensity)
	{
		// Actually sound intensity should follow a parabolic curve like this one
		// auditoryPerceptionStrength = -(iIntensity*iIntensity)*scalingFactor + maxPerception
		// or auditoryPerceptionStrength = -(distFromPlayer*distFromPlayer)*scalingFactor + maxPerception
		auditoryPerceptionStrength = (int) (iIntensity * 100);
		
		return true;
	}
}

