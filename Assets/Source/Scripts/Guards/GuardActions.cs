//#define _DEBUG_SPEW
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Behave.Runtime;

/* GuardPath: A series of 'n' points that indicate a guard movement path.
     Type - Determines how the path points are connected. 
                    "Loop" = Guard will proceed to point 1 after reaching point n. (Last Point)
                    "Reverse" = Guard will follow points in revers order after reaching point n. (Last Point)
                    "Stop" = Guard will Stop moving and hold his position after reaching point n. (Last Point)
     [DEPRECATED] Speed - The normal state walking speed of the guard in Meters/Second.
     [DEPRECATED] Duplicate - Determines weather multiple guards will be instantiated along this path
                    "True" = create multiple guards on this path at regular intervals. 
                    "False" = 1 guard only.
     [DEPRECATED] Frequency - The rate in seconds at which new guards are created on this path.

WayPoint:  An individual way point on the guard path
     Index - The index number of the point, Start at 0 and increment by 1.
    Pause1- The seconds that the guard should pause at this point after turning, but before looking.
     Look - The Direction that the guard looks when he reaches this point.
                    "Left" = The guard will pause to look Left before proceeding to the next point.
                    "Right" = The guard will pause to look to the Right before proceeding to the next point.
     Pause2 - The seconds that the guard should pause at this point after turning, but before proceeding.
                      
* Look and Turn Directions will be reversed for for mid points ( not endpoints ) when the guard is 
     moving in the reverse direction. So only enter Look and Turn parameters as if traversing points in order.

Turn:
	This option is somewhat deceptively named as it has been overloaded with multiple meanings.

	In a "Loop" or "Reverse" type of Guard, this indicates the direction a Guard will turn when moving away from this point and going to the next waypoint.
	Valid options for "Loop" or "Reverse" paths are :
	        1. "Left" Makes the Guard turn LEFT while leaving (Shocking right ? )
	        2. "Right" Makes the Guard turn RIGHT while leaving.

	In a "Stop" type of a Guard, this tag takes on some more meanings.
	IMPORTANT, THESE SPECIFIERS CAN ONLY BE USED FOR THE LAST POINT IN THE STOP PATH, USING THEM AT THE FIRST OR MIDDLE POINT MAY CAUSE UNEXPECTED BEHAVIOR.

	Valid options for "Stop” paths include (i.e. "Left" and "Right" can still be used aside from these, just not at the end point) :
	SCAN360 : Instructs the guard to do continuous 360 degree scans upon reaching the end of the "Stop" path.     
	SCAN180 : Instructs the guard to do continuous 180 degree scans upon reaching the end of the "Stop" path     
	 NOSCAN : Instructs the guard to just look in the direction that it was looking when it reached the end of the "Stop" path.

m_SearchBehaviorDuration : in the AI blackboard determines the amount of time guards spend searching for the player.

s_CapnewSearchPoints : determines the cap on the number of search points that the guard loops between

searchReferenceBias : Biases the search point selection in the favor of the players current position or the guards current position

m_AngularVelocity : The base angular velocity for a guard

m_AnglularCheckTreshold : The threshold after which the angle between two vectors is considered to be 0.

m_linearCheckThreshold : The treshold distance after which two positions are believed to coincide

*/			
public class GuardActions : MonoBehaviour 
{
	private int NumberOfPathNodes;
	private Vector3 []pathNodes;	
	
	#region AI 2.0 Data

	GameObject Player;
	
	NavMeshAgent guard;

	/****************AI 2.0*****************/

	/// <summary>
	/// Unity's Animation interface
	/// </summary>
	//private Animation mGuardAnimations = null;

	/// <summary>
	/// The path of this Guard
	/// </summary>
	public GuardPath Path;

	//private AlertSystem m_GuardAlertSystem;

	// guard AI BlackBoard
	private guardAIBlackBoard m_blackBoard;


	#region Design hooks

	// The threshold after which the angle between two vectors is considered to be 0. ( 0.99 is about 8 degrees )
	public float m_AnglularCheckTreshold = 0.996f;

	// The treshold distance after which two positions are believed to coincide
	public float m_linearCheckThreshold = 0.5f;

	// Angular Rotation Speed
	public float m_AngularVelocity = 50.0f;

	/// <summary>
	/// Indicates how biased the search behavior search point prediction is towards the players current position (i.e. how much we want to cheat)
	/// Higher values make the guard go to points closer to current position of player , may seem unfair, lower values make guard take search points
	/// closer to himself, may look unrealistic
	/// </summary>
	public float m_SearchReferenceBias = 0.8f;

	/// <summary>
	/// The duration for which the guard should carry on with its search behavior
	/// </summary>
	public float m_SearchBehaviorDuration = 60.0f;

	#endregion

	/// <summary>
	/// Indicates the currently relevant search point
	/// </summary>
	private SearchPoint m_CurrentSearchPoint;

	
	/// <summary>
	/// Holds the search points that the guard has gone to while in this search operation
	/// </summary>
	private List<int> mPreviousSearchPoints;

	/// <summary>
	/// The Animation controller for this guard object
	/// </summary>
	private GuardAnimationController mGuardAnimController;

	/***************************************/

	private bool  Player_Caught = false;
	
	public bool PlayerCaught
	{
		get {
			return Player_Caught;
		}
	}


	#endregion


	#region Visual Feedback

	private Material mPatrollingMaterial_Blue;
	private Material mSeenMaterial_Orange;
	private Material mAlertMaterial_Red;

	MeshRenderer[] mGuardVisor;


	#endregion


	void Start () 
	{
		//Debug.Log("Actions of Guards Instantiated");
		//if(networkView.isMine)
		if ( GameManager.Manager.PlayerType == 1 )
		{
			Player = GameObject.FindGameObjectWithTag("Player");
			guard = GetComponent<NavMeshAgent>();

			mPreviousSearchPoints = new List<int>();

			mGuardAnimController = new GuardAnimationController(transform.FindChild("soldier")
			                                                    .FindChild("Drone (1)").FindChild("Drone_Animated").GetComponent<Animation>());

			m_blackBoard = new guardAIBlackBoard(Path,mGuardAnimController);

			// GUARD VISOR COLORS [Probably should move this]
			mPatrollingMaterial_Blue = UnityEngine.Resources.Load ("Materials/Thief/GlowMesh_Blue", typeof(Material)) as Material;
			mSeenMaterial_Orange = UnityEngine.Resources.Load ("Materials/Thief/GlowMesh_Orange", typeof(Material)) as Material;
			mAlertMaterial_Red = UnityEngine.Resources.Load ("Materials/Thief/GlowingTexture", typeof(Material)) as Material;

			mGuardVisor = transform.FindChild("soldier")
				.FindChild("Drone (1)").FindChild("Drone_Animated").GetComponentsInChildren<MeshRenderer>();


			foreach (MeshRenderer r in mGuardVisor)
			{// VISUAL [TURN VISOR BLUE]
				// HACK
				if((r.name != "L_Wing") && (r.name != "R_Wing")
				   && (r.name != "Taser_Arm") 
				   && (r.name != "Taser_Point"))
				{
					Material [] cachedMats = r.materials; 
					if((r.name != "SpikeR_01") && (r.name != "SpikeR_02")
					   && (r.name != "SpikeR_03") && (r.name != "SpikeR_04"))
						cachedMats[0] = mPatrollingMaterial_Blue;
					else
						cachedMats[1] = mPatrollingMaterial_Blue;
					r.materials = cachedMats;
				}
			}
		}

		
		// Audio source to play the guard thrusters loop
		AudioSource guardThrusterSource = (AudioSource)gameObject.AddComponent("AudioSource"); 
		guardThrusterSource.playOnAwake = false;
		guardThrusterSource.maxDistance = 1.0f;
		guardThrusterSource.minDistance = 6.0f;
		guardThrusterSource.spread = 0.0f;
		guardThrusterSource.dopplerLevel = 1;
		guardThrusterSource.rolloffMode = AudioRolloffMode.Linear;
		guardThrusterSource.name = "Guard_Thruster_Source";
		guardThrusterSource.volume = 0.4F;
		
		// Thruster on guard 
		if( GameManager.Manager.PlayerType == 1)
		{
			soundMan.soundMgr.playOnSource(this.audio,"Guard_Idle_loop",true,GameManager.Manager.PlayerType);
			
			// Looping thruster on guard [SOUNDS TAG] [Guard_Thrusters_loop]
			//soundMan.soundMgr.playOnSource(guardThrusterSource,"Guard_Thrusters_loop",true,GameManager.Manager.PlayerType);
		}

		//mGuardAnimController.guardIdling();

	}

	/// <summary>
	/// Sets a new destination for the Guard to go to
	/// </summary>
	/// <param name="i_posToGo"> Vector 3 indicating the position that the guard needs to go to </param>
	public void goToPoint( Vector3 i_posToGo )
	{
		#if _DEBUG_SPEW

			Debug.Log("Going to point : " + i_posToGo);

			Debug.Log("Current position : " + guard.transform.position );
						
			Debug.Log(" PREV DESTINATION : " + gameObject.GetComponent<NavMeshAgent>().destination);

		#endif

		guard.GetComponent<NavMeshAgent>().destination = i_posToGo;
		#if _DEBUG_SPEW
			Debug.Log(" NEW DESTINATION : " + gameObject.GetComponent<NavMeshAgent>().destination);
		#endif
	}
	
	void BobAnimation()
	{
		transform.position += new Vector3(0.0f, 0.05f * Mathf.Sin(4 * Time.time), 0.0f);
	}

	#region Guard Helper functions

	public void pauseGuard()
	{
		m_blackBoard.pauseGuard();
		GetComponent<Perception>().enabled = false;
	}

	public void resumeGuard()
	{
		m_blackBoard.resumeGuard();
		GetComponent<Perception>().enabled = true;
	}

	#endregion

	void Update () 
	{
		if ( GameManager.Manager.PlayerType == 1 )
		{
			if(GameManager.Manager.PlayerType == 1) // is thief
			{
				if(guard == null)
				{
					guard = GetComponent<NavMeshAgent>();
				}
			}

			if(Vector3.Distance(this.transform.position,Player.transform.position) < m_linearCheckThreshold)
			{
				Player.GetComponent<ThiefActions>().playerCaught();
			}

			// AI 2.0 
			if(!GuardOverlord.Manager.GuardsPaused)
				//Bobbing animation
				BobAnimation();
			
			/************* AI 2.0 Rotations ****************/
			//Debug.Log("Guard Type : " + GetComponent<GuardBehaviourScript>().GuardType);
			if(GetComponent<GuardBehaviourScript>().GuardType == 1)
				if(m_blackBoard.seekDirection != 0)
				{
					guard.transform.Rotate(m_blackBoard.seekDirection * 
				                       m_AngularVelocity * Vector3.up * Time.deltaTime * m_blackBoard.AngularSpeedModifier);
				}
			/***********************************************/

			mGuardAnimController.updateAnimation();

			// AI 2.0 End
		}
	}

	// [NIK_QUERY]
	void OnTriggerStay(Collider hit)
	{
		if(hit.tag == "Door")
		{
			if(hit.gameObject.GetComponent<DoorController>().animating)
			{
				//gameObject.GetComponent<NavMeshAgent>().Stop();
			}
			else
			{
				//gameObject.GetComponent<NavMeshAgent>().Resume();
			}
		}
	}	

	#region Patrol Behavior Subtree AI 2.0

	/// <summary>
	/// Action taken when the guard needs to go to the (currently) most relevant waypoint
	/// </summary>
	/// <returns>The BehaveResult that indicates this actions state 
	/// [NIK] Do I ever NEED, to return Failure here ?</returns>
	public BehaveResult goToWayPoint()
	{

#if _DEBUG_SPEW
		Debug.Log("GOTOWAYPOINTACTION");
#endif

		// Tell the guard to go to the (currently) most relevant waypoint
		goToPoint(Path.WayPoints[m_blackBoard.m_MostRelevantWaypoint].Position);

		// Indicate Success
		return BehaveResult.Success;
	}
	/// <summary>
	/// Generic wrapper action that waits for the condition method inside it to be return Success
	/// </summary>
	/// <returns>Depending on the condition , if the condition is not successful yet, returns "Running"
	/// If the condition is Successful, returns Success
	/// [NIK] Do I ever NEED, to return Failure here ?</returns>
	public BehaveResult waitForCondition()
	{
		return isGuardAtWaypoint();
	}

	/// <summary>
	/// Gets the next way point most relevant to this Guard
	/// CODE REPLICATION FTW
	/// </summary>
	/// <returns>The next way point.</returns>
	public BehaveResult getNextWayPoint()
	{

		m_blackBoard.m_MostRelevantWaypoint+= m_blackBoard.TraversalDirection;
		
		if(Path.Type == "Stop")
		{
			if(m_blackBoard.m_MostRelevantWaypoint >= Path.WayPoints.Count)
				m_blackBoard.m_MostRelevantWaypoint = Path.WayPoints.Count-1;
		}
		// If the path is to be looped
		else if(Path.Type == "Loop")
		{
			m_blackBoard.m_MostRelevantWaypoint%=Path.WayPoints.Count;
		}
		// If the path is to be reversed
		else if(Path.Type == "Reverse")
		{
			// and the current waypoint has crossed bounds on the positive side
			if(m_blackBoard.m_MostRelevantWaypoint >= Path.WayPoints.Count)
			{
				// Set the next waypoint to the second last one (in order)
				m_blackBoard.m_MostRelevantWaypoint = Path.WayPoints.Count - 2;
				
				// reverse the traversal direction
				m_blackBoard.reverseTraversalDirection();
			}
			// if the current waypoint has crossed bounds on the negative side
			else if(m_blackBoard.m_MostRelevantWaypoint < 0)
			{
				// Set the next waypoint to the second one (in order)
				m_blackBoard.m_MostRelevantWaypoint = 1;
				
				// reverse the traversal direction
				m_blackBoard.reverseTraversalDirection();
			}
		}

		return BehaveResult.Success;
	}

	/// <summary>
	/// Helper function allows you to peek at the next waypoint.
	/// </summary>
	/// <returns>The next waypoint.</returns>
	public int peekNextWaypoint()
	{
		int _currentWaypt = m_blackBoard.m_MostRelevantWaypoint;

		_currentWaypt+= m_blackBoard.TraversalDirection;

		if(Path.Type == "Stop")
		{
			if(_currentWaypt >= Path.WayPoints.Count)
				_currentWaypt = Path.WayPoints.Count-1;
		}
		// If the path is to be looped
		else if(Path.Type == "Loop")
		{
			_currentWaypt%=Path.WayPoints.Count;
		}
		// If the path is to be reversed
		else if(Path.Type == "Reverse")
		{
			// and the current waypoint has crossed bounds on the positive side
			if(_currentWaypt >= Path.WayPoints.Count)
			{
				// Set the next waypoint to the second last one (in order)
				_currentWaypt = Path.WayPoints.Count - 2;

				// CODE REPLICATION FTW
				// reverse the traversal direction
				// m_blackBoard.reverseTraversalDirection();
			}
			// if the current waypoint has crossed bounds on the negative side
			else if(_currentWaypt < 0)
			{
				// Set the next waypoint to the second one (in order)
				_currentWaypt = 1;

				// CODE REPLICATION FTW
				// reverse the traversal direction
				// m_blackBoard.reverseTraversalDirection();
			}
		}

		return _currentWaypt;
	}

	/// <summary>
	/// Indicates whether the guard needs to do a 360 Degree scan at this waypoint
	/// </summary>
	/// <returns>Success if a 360 Degree scan is required, Failure if not </returns>
	public BehaveResult checkScan360()
	{
		// If the turn is defined as 360 degree scans
		if(Path.WayPoints[m_blackBoard.m_MostRelevantWaypoint].Turn == "SCAN360")
		{
			#if _DEBUG_SPEW
				Debug.Log("SCAN 360 Detected at Waypoint " + m_blackBoard.m_MostRelevantWaypoint);
			#endif

			m_blackBoard.seekDirection = 1;
			m_blackBoard.seekingAlignmentVector = new Vector3(0,0,0); // [NIK : IMPROVE]

			return BehaveResult.Success;
		}
		else
		{
			#if _DEBUG_SPEW
				Debug.Log("SCAN 360 Not detected at Waypoint " + m_blackBoard.m_MostRelevantWaypoint);
			#endif
			return BehaveResult.Failure;
		}
	}

	/// <summary>
	/// Actually Rotates the guard around itself
	/// </summary>
	/// <returns>The scan360.</returns>
	public BehaveResult doScan360()
	{
//		guard.transform.Rotate(50.0f*Vector3.up*Time.deltaTime);
		//Debug.Log("Rotation : " + guard.transform.rotation);

		return BehaveResult.Running;
	}


	/// <summary>
	/// Indicates whether the guard needs to do a 180 Degree scan at this waypoint
	/// </summary>
	/// <returns>Success if a 180 Degree scan is required, Failure if not </returns>
	public BehaveResult checkScan180()
	{

		// If the turn is defined as 360 degree scans
		if(Path.WayPoints[m_blackBoard.m_MostRelevantWaypoint].Turn == "SCAN180")
		{
			//Debug.Log("SCAN 180 Detected at Waypoint " + m_MostRelevantWaypoint);

			// Set the guard to seek alignment to its 'Right'
			m_blackBoard.seekAlignmentToVector(this.transform.right);

			//Debug.Log("SEEKING 180 Alignment to Vector :  " + m_blackBoard.seekingAlignmentVector);

			return BehaveResult.Success;
		}
		else
			return BehaveResult.Failure;
	}
	
	/// <summary>
	/// Actually Rotates the guard around itself
	/// </summary>
	/// <returns>The scan180.</returns>
	public BehaveResult doScan180()
	{
		if( isAligned())
		{
//			Debug.Log("Aligned to Vector :  " + this.transform.forward);

			// Realign to the other side in the 180 degree sweep & send '10' to revert the sweep direction
			m_blackBoard.seekAlignmentToVector(-this.transform.forward,10);

//			Debug.Log("SEEKING 180 Alignment to Vector :  " + m_blackBoard.seekingAlignmentVector);
		}
			return BehaveResult.Running;

	}

	/// <summary>
	/// Indicates whether the guard needs to do no scan at this waypoint
	/// </summary>
	/// <returns>Success if no scan is required, Failure if not </returns>
	public BehaveResult checkNoScan()
	{
		// If the turn is defined as NO scans
		if(Path.WayPoints[m_blackBoard.m_MostRelevantWaypoint].Turn == "NOSCAN")
		{
			// Debug.Log("NOSCAN Detected at Waypoint " + m_MostRelevantWaypoint);
			return BehaveResult.Success;
		}
		else
			return BehaveResult.Failure;
	}
	
	/// <summary>
	/// Actually Rotates the guard around itself
	/// </summary>
	/// <returns>The scan180.</returns>
	public BehaveResult dontScan()
	{
		return BehaveResult.Running;
	}

	/******************* WAYPOINT ACTIONS ***********************/

	/// <summary>
	/// Makes the guard wait for the first pause duration
	/// </summary>
	/// <returns>The pause one.</returns>
	public BehaveResult doPauseOne()
	{
		// If there is an actual wait required
		if(Path.WayPoints[m_blackBoard.m_MostRelevantWaypoint].FirstPauseDuration> 0)
		{
			#if _DEBUG_SPEW
				Debug.Log(" First Pause duration is :  " + Path.WayPoints[m_blackBoard.m_MostRelevantWaypoint].FirstPauseDuration);
			#endif		

			// There is no waypointDecaySignal in existance
			if(m_blackBoard.m_wayPointWaitDecayingSignal == null)
			{
				// Create one 
				m_blackBoard.m_wayPointWaitDecayingSignal = gameObject.AddComponent<decayingSignal>();

				// Initialize it to decay in "Path.WayPoints[m_MostRelevantWaypoint].FirstPauseDuration"
				m_blackBoard.wayPointDecayingSignal.initialize(Path.WayPoints[m_blackBoard.m_MostRelevantWaypoint].FirstPauseDuration);
			}

			// if the signal has decayed
			if(!m_blackBoard.m_wayPointWaitDecayingSignal.IsActive)
			{
				#if _DEBUG_SPEW
					Debug.Log(" First Pause DONE ");
				#endif		

				// [IMPORTANT : ACCOUNT FOR BEING INTERRUPTED , implement wayPointTasksInit Action]
				// Destroy the signal
				GameObject.Destroy(m_blackBoard.wayPointDecayingSignal);

				// Indicate that no signal is in existance anymore 
				m_blackBoard.m_wayPointWaitDecayingSignal = null;
				return BehaveResult.Success;
			}
			// If the signal has not decayed yet, then wait
			else
			{
				#if _DEBUG_SPEW
					Debug.Log(" Waiting for the first pause to be over in " + m_blackBoard.wayPointDecayingSignal.timeToDecay );
				#endif
				return BehaveResult.Running;
			}
		}
		// If there is no firstPause defined move on immidiately
		else
		{
			#if _DEBUG_SPEW
				Debug.Log(" No First Pause Required ");
			#endif
			return BehaveResult.Success;
		}
	}
	
	public BehaveResult lookToDirection()
	{
		// if no look is required 
		if(Path.WayPoints[m_blackBoard.m_MostRelevantWaypoint].Look == "None")
			// return success and move on
			return BehaveResult.Success;

		// By default align to right [just here for the Use of unassigned local var issue]
		Vector3 _alignToVector = this.transform.right;

		// Turn direction
		int _turnDirection;

		// if no look is required 
		if(Path.WayPoints[m_blackBoard.m_MostRelevantWaypoint].Look == "Left")
		{
			_alignToVector = -this.transform.right * m_blackBoard.TraversalDirection;

			_turnDirection = -1 * m_blackBoard.TraversalDirection;
		}
		else if (Path.WayPoints[m_blackBoard.m_MostRelevantWaypoint].Look == "Right")
		{
			_alignToVector = this.transform.right * m_blackBoard.TraversalDirection;

			_turnDirection = 1 * m_blackBoard.TraversalDirection;
		}
		else 
		{
			#if _DEBUG_SPEW
				Debug.LogError("INVALID ALIGNMENT VECTOR REQUESTED");
			#endif
			return BehaveResult.Success;
		}

		// seek alignment to indicated vector
		m_blackBoard.seekAlignmentToVector(_alignToVector,_turnDirection);
		return BehaveResult.Success;
	}
	
	public BehaveResult doPauseTwo()
	{
		// If there is an actual wait required
		if(Path.WayPoints[m_blackBoard.m_MostRelevantWaypoint].SecondPauseDuration> 0)
		{
			// There is no waypointDecaySignal in existance
			if(m_blackBoard.wayPointDecayingSignal == null)
			{
				// Create one 
				m_blackBoard.m_wayPointWaitDecayingSignal = gameObject.AddComponent<decayingSignal>();
				
				// Initialize it to decay in "Path.WayPoints[m_MostRelevantWaypoint].SecondPauseDuration"
				m_blackBoard.wayPointDecayingSignal.initialize(Path.WayPoints[m_blackBoard.m_MostRelevantWaypoint].SecondPauseDuration);
			}
			
			// if the signal has decayed
			if(!m_blackBoard.wayPointDecayingSignal.IsActive)
			{
				// [IMPORTANT : ACCOUNT FOR BEING INTERRUPTED , implement wayPointTasksInit Action]
				// Destroy the signal
				GameObject.Destroy(m_blackBoard.m_wayPointWaitDecayingSignal);
				
				// Indicate that no signal is in existance anymore 
				m_blackBoard.m_wayPointWaitDecayingSignal = null;
				return BehaveResult.Success;
			}
			// If the signal has not decayed yet, then wait
			else
			{
				return BehaveResult.Running;
			}
		}
		// If there is no SecondPauseDuration defined move on immidiately
		else
			return BehaveResult.Success;
	}
	
	public BehaveResult alignToLeave()
	{
		// if no look is required 
		if(Path.WayPoints[m_blackBoard.m_MostRelevantWaypoint].Turn == "None")
			// return success and move on
			return BehaveResult.Success;
		
		// Indicate the direction that the guard must align to in order to face the next waypoint

		#if _DEBUG_SPEW
			Debug.Log("Peeked waypoint : " + peekNextWaypoint() + " Current waypoint : " + m_blackBoard.m_MostRelevantWaypoint);
		#endif

		Vector3 _alignToVector = Path.WayPoints[peekNextWaypoint()].Position - Path.WayPoints[m_blackBoard.m_MostRelevantWaypoint].Position;
		
		// Align by turning to the left
		if(Path.WayPoints[m_blackBoard.m_MostRelevantWaypoint].Turn == "Left")
		{
			m_blackBoard.seekAlignmentToVector(_alignToVector,-1 * m_blackBoard.TraversalDirection);
		}
		// Align by turning to the right
		else if (Path.WayPoints[m_blackBoard.m_MostRelevantWaypoint].Turn == "Right")
		{
			m_blackBoard.seekAlignmentToVector(_alignToVector,1 * m_blackBoard.TraversalDirection);
		}
		else 
		{
			#if _DEBUG_SPEW
				Debug.LogError("INVALID ALIGNMENT VECTOR REQUESTED");
			#endif
		}

		#if _DEBUG_SPEW
			Debug.Log("Traversal Direction : " + m_blackBoard.TraversalDirection);
		#endif

		return BehaveResult.Success;
	}
	
	/******************* END WAYPOINT ACTIONS *******************/

	
	#endregion
	
	#region Common Behavior AI 2.0

	/// <summary>
	/// Checks if the guard is at the most relevant waypoint.
	/// </summary>
	/// <returns>If the guard is at the most relevant waypoint, returns Success
	/// else returns Running </returns>
	public BehaveResult isGuardAtWaypoint()
	{
	
		// If the guard is at the most relevant waypoint (close enough is m_linearCheckThreshold units)
		if(guard.remainingDistance < m_linearCheckThreshold )
		{

			#if _DEBUG_SPEW
				Debug.Log(" AT THE DESTINATION");
			#endif
			// return Success
			return BehaveResult.Success;
		}
		else
		{
			#if _DEBUG_SPEW
				Debug.Log(" WAITING FOR DESTINATION");
			#endif


			// return Running
			return BehaveResult.Running;
		}
	}

	/// <summary>
	/// Waits for guard to align to the current Alignment target Vector
	/// </summary>
	/// <returns>Success if the gurad has aligned, Running otherwise</returns>
	public BehaveResult waitForAlign()
	{
		// if vectors are aligned or no alignment is currently happening
		if(isAligned() || m_blackBoard.seekDirection == 0)
		{
			#if _DEBUG_SPEW
				Debug.Log("Alignment found");
			#endif

			// indicate that no alignment is being sought anymore 
			m_blackBoard.seekDirection = 0;

			return BehaveResult.Success;
		}
		else
		{
			return BehaveResult.Running;
		}
	}

	/// <summary>
	/// Checks if the Alert system has been triggered
	/// </summary>
	/// <returns>Success if the gurad is not alert (Thereby allowing th rest of the tree to evaluate)
	/// Failure if the guard is Alert prombting a bailout from the current action
	public BehaveResult highLevelInterruptDecorator(Behave.Runtime.Tree sender)
	{
		#if _DEBUG_SPEW
			Debug.Log("HIGH LEVEL INTERRUPT CHECK");
		#endif	

		if(GetComponent<Perception>().isGuardAlert)
		{
			#if _DEBUG_SPEW
				Debug.Log("INTERRUPTED");
			#endif

			sender.Reset();

			// VISUAL {TURN VISOR ORANGE]
			foreach (MeshRenderer r in mGuardVisor)
			{
				
				// HACK
				if((r.name != "L_Wing") && (r.name != "R_Wing")
				   && (r.name != "Taser_Arm") 
				   && (r.name != "Taser_Point"))
				{
					
					Material [] cachedMats = r.materials; 
					if((r.name != "SpikeR_01") && (r.name != "SpikeR_02")
					   && (r.name != "SpikeR_03") && (r.name != "SpikeR_04"))
						cachedMats[0] = mSeenMaterial_Orange;
					else
						cachedMats[1] = mSeenMaterial_Orange;
					r.materials = cachedMats;
				}
			}


			return BehaveResult.Failure;
		}
		else
		{
			#if _DEBUG_SPEW
				Debug.Log("NOT INTERRUPTED");
			#endif

			return BehaveResult.Success;
		}
	}

	/// <summary>
	/// Parallel companion to the high level interrupt Decorator.
	/// </summary>
	/// <returns>The result of the high level interrupt action.</returns>
	/// <param name="sender">Sender Tree.</param>
	public BehaveResult parallelHighLevelInterruptAction(Behave.Runtime.Tree sender)
	{
		#if _DEBUG_SPEW
			Debug.Log("HIGH LEVEL INTERRUPT CHECK");
		#endif	
		
		if(GetComponent<Perception>().isGuardAlert)
		{
			#if _DEBUG_SPEW
				Debug.Log("INTERRUPTED");
			#endif

//			if(m_blackBoard.isGuardSearchActive != null)
//			{
//				if(m_blackBoard.isGuardSearchActive.IsActive)
//					m_blackBoard.isGuardSearchActive.decayPrematurely();
//			}
			if(m_blackBoard.SearchRequired)
				m_blackBoard.SearchRequired = false;

			// VISUAL [TURN VISOR ORANGE]
			foreach (MeshRenderer r in mGuardVisor)
			{
				
				// HACK
				if((r.name != "L_Wing") && (r.name != "R_Wing")
				   && (r.name != "Taser_Arm") 
				   && (r.name != "Taser_Point"))
				{
					
					Material [] cachedMats = r.materials; 
					if((r.name != "SpikeR_01") && (r.name != "SpikeR_02")
					   && (r.name != "SpikeR_03") && (r.name != "SpikeR_04"))
						cachedMats[0] = mSeenMaterial_Orange;
					else
						cachedMats[1] = mSeenMaterial_Orange;
					r.materials = cachedMats;
				}
			}
			
			return BehaveResult.Failure;
		}
		else
		{
			#if _DEBUG_SPEW
				Debug.Log("NOT INTERRUPTED");
			#endif
			
			return BehaveResult.Success;
		}
	}



	/// <summary>
	/// Checks if the Alert system has been triggered
	/// </summary>
	/// <returns>Success if the gurad is not alert (Thereby allowing th rest of the tree to evaluate)
	/// Failure if the guard is Alert prombting a bailout from the current action
	public BehaveResult guardAlertInterruptor()
	{
		#if _DEBUG_SPEW
			Debug.Log("GUARD ALERT INTERRUPT CHECK");
		#endif
		if(GetComponent<Perception>().isGuardAlert)
		{
			#if _DEBUG_SPEW
				Debug.Log("IS ALERT");
			#endif
			return BehaveResult.Success;
		}
		else
			return BehaveResult.Failure;
	}


	#endregion

	#region Search Behavior Subtree AI 2.0 
	
	/// <summary>
	/// Checks if a search operation is required this frame , bails out of the subtree if it is not
	/// </summary>
	/// <returns>Success if a search is actually required this frame 
	/// Failure if no search is required this frame
	public BehaveResult isSearchRequired(Behave.Runtime.Tree sender)
	{
		#if _DEBUG_SPEW
			Debug.Log("IS SEARCH REQUIRED CHECK");
		#endif	

		// if a search is required 
		if(m_blackBoard.SearchRequired)
		{
			#if _DEBUG_SPEW
				Debug.Log("NEW SEARCH REQUIRED");
			#endif	

			// And no pervious search is active 
			if(m_blackBoard.isGuardSearchActive == null)
			{

				#if _DEBUG_SPEW
					Debug.Log("NEW SEARCH INITIALIZED");
				#endif	

				// Activate one
				m_blackBoard.isGuardSearchActive = gameObject.AddComponent<decayingSignal>();

				m_blackBoard.isGuardSearchActive.initialize(m_SearchBehaviorDuration);

				// This search request has been acknowledged
				m_blackBoard.SearchRequired = false;
			}
		}

		if(m_blackBoard.isGuardSearchActive != null)
		{
			// If a Guard search is currently active 
			if(m_blackBoard.isGuardSearchActive.IsActive)
			{
				#if _DEBUG_SPEW
				Debug.Log("GUARD SEARCH IS ACTIVE Remaining time : " + m_blackBoard.isGuardSearchActive.timeToDecay);
				#endif	

				return BehaveResult.Success;
			}
			// If no search is currently active 
			else
			{

	#if _DEBUG_SPEW
				Debug.Log("BAILED out of search behavior by interruption");
	#endif
				// Play the patrolling sound on the Guard
				// Thruster on guard 
				if( GameManager.Manager.PlayerType == 1)
				{
					soundMan.soundMgr.playOnSource(this.audio,"Guard_Idle_loop",true,GameManager.Manager.PlayerType);					
				}

				// VISUAL [TURN VISOR BLUE]
				foreach (MeshRenderer r in mGuardVisor)
				{
					
					// HACK
					if((r.name != "L_Wing") && (r.name != "R_Wing")
					   && (r.name != "Taser_Arm") 
					   && (r.name != "Taser_Point"))
					{
						
						Material [] cachedMats = r.materials; 
						if((r.name != "SpikeR_01") && (r.name != "SpikeR_02")
						   && (r.name != "SpikeR_03") && (r.name != "SpikeR_04"))
							cachedMats[0] = mPatrollingMaterial_Blue;
						else
							cachedMats[1] = mPatrollingMaterial_Blue;
						r.materials = cachedMats;
					}
				}

				GameObject.Destroy(m_blackBoard.isGuardSearchActive);

				m_blackBoard.isGuardSearchActive = null;

				// This search request has been acknowledged
				m_blackBoard.SearchRequired = false;

				mPreviousSearchPoints.Clear();

				sender.Reset();
				return BehaveResult.Failure;
			}
		}else
		{

			#if _DEBUG_SPEW
				Debug.Log("BAILED out of search behavior");
			#endif

			return BehaveResult.Failure;
		}
	}

	//findMostRelevantSearchPoint
	public BehaveResult findMostRelevantSearchPoint(Behave.Runtime.Tree sender)
	{
		if(parallelHighLevelInterruptAction(sender) == BehaveResult.Failure)
			return BehaveResult.Failure;

		if(isSearchRequired(sender) == BehaveResult.Failure)
			return BehaveResult.Failure;

		{
			// Find the vector that points from the Guards position to the players position
			Vector3 guardToPlayer = Player.transform.position - this.transform.position;
			Vector3 referencePosition = this.transform.position + m_SearchReferenceBias * guardToPlayer;

			// Get a new most relevant Search Point 
			m_CurrentSearchPoint = SearchPoint.getMostRelevantSearchPoint(referencePosition,m_CurrentSearchPoint,ref mPreviousSearchPoints);

			// If a relevant search point was found
			if(m_CurrentSearchPoint != null)
			{
				// Indicate unsuccessful search and Bail
				#if _DEBUG_SPEW
					Debug.Log("Going to Search point");
				#endif

				// Go to it
				goToPoint(m_CurrentSearchPoint.Position);

				// Advance the sequence
				return BehaveResult.Success;
			}else
			{
				// Bail
				// Indicate unsuccessful search and Bail
				#if _DEBUG_SPEW
					Debug.Log("BAILED searching");
				#endif

				return BehaveResult.Failure;
			}
		}
	}


	public BehaveResult lookAroundAtSearchPoint(Behave.Runtime.Tree sender)
	{
		if(parallelHighLevelInterruptAction(sender) == BehaveResult.Failure)
		{
			#if _DEBUG_SPEW
				Debug.Log("BAILED LOOKING AROUND at search point (HIGH LEVEL INTERRUPT)");
			#endif
			return BehaveResult.Failure;
		}
		
		if(isSearchRequired(sender) == BehaveResult.Failure)
		{
			#if _DEBUG_SPEW
				Debug.Log("BAILED LOOKING AROUND at search point (NO SEARCH REQUIRED)");
			#endif

			return BehaveResult.Failure;
		}

	#if _DEBUG_SPEW
			Debug.Log("LOOKING AROUND at search point");
	#endif


		// Find the vector that the guard needs to align to in order to track the player
		Vector3 _alignmentVector = this.transform.position - Player.transform.position;
		
		// Find the direction in which the guard must turn to align to the player
		// Shortest angle to be traversed
		Vector3 _tempCross = Vector3.Cross(this.transform.forward,_alignmentVector);
		int turnDirection = ( _tempCross.y > 0 )? 1 : -1;
		
		// seek alignment to indicated vector [Turn direction is 1 right now, need to change to take smallest angle]
		m_blackBoard.seekAlignmentToVector(-this.transform.forward,turnDirection);

		return BehaveResult.Success;
	}


	public BehaveResult waitTillSearchPointAction(Behave.Runtime.Tree sender)
	{
		if(parallelHighLevelInterruptAction(sender) == BehaveResult.Failure)
		{
			#if _DEBUG_SPEW
			Debug.Log("BAILED WAITING FOR search point (HIGH LEVEL INTERRUPT)");
			#endif
			return BehaveResult.Failure;
		}
		
		if(isSearchRequired(sender) == BehaveResult.Failure)
		{
			#if _DEBUG_SPEW
			Debug.Log("BAILED WAITING FOR search point (NO SEARCH REQUIRED)");
			#endif
			
			return BehaveResult.Failure;
		}

		
		#if _DEBUG_SPEW
			Debug.Log(" WAITING FOR Search Point ");
		#endif
				
		return waitForCondition();
	}

	public BehaveResult waitForLookAround(Behave.Runtime.Tree sender)
	{
		if(parallelHighLevelInterruptAction(sender) == BehaveResult.Failure)
			return BehaveResult.Failure;
		
		if(isSearchRequired(sender) == BehaveResult.Failure)
			return BehaveResult.Failure;

		#if _DEBUG_SPEW
			Debug.Log(" WAITING FOR LOOK AROUND ");
		#endif
		
		return waitForAlign();
	}

	#endregion
	
	#region Chase Behavior Subtree AI 2.0

	/// <summary>
	/// Action taken when the guard sees the player and needs to track him
	/// </summary>
	/// <returns> Running while the player is being tracked (i.e. is visible to the guard)
	/// Success when the Alert meter reaches full while the player is being tracked 
	/// Fail if the guard loses sight of the player before the alert meter hits full</returns>
	public BehaveResult trackPlayer()
	{
		guard.GetComponent<NavMeshAgent>().Stop();

		// If the guard is fully alert
		if( gameObject.GetComponent<Perception>().AlertLevel >= 100.0f)
		{	
			// indicate that no alignment is being sought anymore 
			m_blackBoard.seekDirection = 0;

			// The player was successfully tracked, so a search is required
			m_blackBoard.SearchRequired = true;

			// SMALL HACK to fix [BUG :  Stop path Realignment]
			// If this is a stop path then when the guard gives chase, I reset his current waypoint to 0
			if(Path.Type == "Stop")
			{
				m_blackBoard.CurrentWaypoint = 0;
			}


			//mGuardAnimations.CrossFade("normalToDisturbed");
		
			mGuardAnimController.playNormalToAlert();
			mGuardAnimController.playAlertToChasing();

			// Indicate that Player was seen to the Scoring system
			NetworkManager.Manager.SetTimesChasedByGuard(1);

			// VISUAL {TURN VISOR RED]
			foreach (MeshRenderer r in mGuardVisor)
			{
				
				// HACK
				if((r.name != "L_Wing") && (r.name != "R_Wing")
				   && (r.name != "Taser_Arm") 
				   && (r.name != "Taser_Point"))
				{
					
					Material [] cachedMats = r.materials; 
					if((r.name != "SpikeR_01") && (r.name != "SpikeR_02")
					   && (r.name != "SpikeR_03") && (r.name != "SpikeR_04"))
						cachedMats[0] = mAlertMaterial_Red;
					else
						cachedMats[1] = mAlertMaterial_Red;
					r.materials = cachedMats;
				}
			}

			// Indicate successful tracking to the guard so that he can chase the player
			return BehaveResult.Success;
		}
		else if( gameObject.GetComponent<Perception>().AlertLevel <= 0.0f)
		{
			// indicate that no alignment is being sought anymore 
			m_blackBoard.seekDirection = 0;

			// Indicate unsuccessful tracking to the guard so that he can bail out and find a better action to take
			#if _DEBUG_SPEW
				Debug.Log("BAILED player tracking");
			#endif

			// The player was NOT successfully tracked, so a search is NOT required
			m_blackBoard.SearchRequired = false;

			// VISUAL {TURN VISOR BLUE]
			foreach (MeshRenderer r in mGuardVisor)
			{
				
				// HACK
				if((r.name != "L_Wing") && (r.name != "R_Wing")
				   && (r.name != "Taser_Arm") 
				   && (r.name != "Taser_Point"))
				{
					
					Material [] cachedMats = r.materials; 
					if((r.name != "SpikeR_01") && (r.name != "SpikeR_02")
					   && (r.name != "SpikeR_03") && (r.name != "SpikeR_04"))
						cachedMats[0] = mPatrollingMaterial_Blue;
					else
						cachedMats[1] = mPatrollingMaterial_Blue;
					r.materials = cachedMats;
				}
			}

			return BehaveResult.Failure;
		}
		else
		{

			// the vector that the guard needs to align to in order to track the player
			Vector3 _alignmentVector = Player.transform.position - this.transform.position;
			_alignmentVector.Normalize();
			
			// Find the direction in which the guard must turn to align to the player
			// Shortest angle to be traversed
			Vector3 _tempCross = Vector3.Cross(this.transform.forward,_alignmentVector);
			int turnDirection = ( _tempCross.y > 0 )? 1 : -1;


			// seek alignment to indicated vector 
			m_blackBoard.seekAlignmentToVector(_alignmentVector,turnDirection,false);

			// If the guard is already Aligned
			if(isAligned())
			{
				// indicate that no alignment is being sought anymore 
				m_blackBoard.seekDirection = 0;
			}

			// The player is being tracked, make sure that no search is required till Success [PRECAUTION]
			m_blackBoard.SearchRequired = false;

			return BehaveResult.Running;
		}
	}


	/// <summary>
	/// Action taken when the guard has reached full alert and is chasing the player
	/// </summary>
	/// <returns> Running while the player is being chased
	/// Success (If the player is caught)
	/// Fail if the guard loses sight of the player and the alert meter hits 0</returns>
	public BehaveResult chasePlayer()
	{

		// if the Guard is no longer alert
		if( gameObject.GetComponent<Perception>().AlertLevel <= 0.0f)
		{
			// Indicate unsuccessful chase to the guard so that he can bail out and find a better action to take
			#if _DEBUG_SPEW
				Debug.Log("BAILED");
			#endif

			mGuardAnimController.playAlertToNormal();

			// Thruster on guard 
			if( GameManager.Manager.PlayerType == 1)
			{
				soundMan.soundMgr.playOnSource(this.audio,"Guard_Searching_loop",true,GameManager.Manager.PlayerType);			
			}

			// VISUAL {TURN VISOR ORANGE]
			foreach (MeshRenderer r in mGuardVisor)
			{
				
				// HACK
				if((r.name != "L_Wing") && (r.name != "R_Wing")
				   && (r.name != "Taser_Arm") 
				   && (r.name != "Taser_Point"))
				{
					
					Material [] cachedMats = r.materials; 
					if((r.name != "SpikeR_01") && (r.name != "SpikeR_02")
					   && (r.name != "SpikeR_03") && (r.name != "SpikeR_04"))
						cachedMats[0] = mSeenMaterial_Orange;
					else
						cachedMats[1] = mSeenMaterial_Orange;
					r.materials = cachedMats;
				}
			}

			return BehaveResult.Failure;
		}
		// Otherwise , chase the player
		else
		{
		
			// If the player is not currently visible
			if(!GetComponent<Perception>().isGuardAlert)
			{
				// If the guard is at the players last known position
				if(guard.remainingDistance < m_linearCheckThreshold)
				{
					guard.Stop();

					// Find the vector that the guard needs to align to in order to track the player
					Vector3 _alignmentVector = Player.transform.position - this.transform.position;
					_alignmentVector.Normalize();
					
					// Find the direction in which the guard must turn to align to the player
					// Shortest angle to be traversed
					Vector3 _tempCross = Vector3.Cross(this.transform.forward,_alignmentVector);
					int turnDirection = ( _tempCross.y > 0 )? 1 : -1;

					// If the guard is already Aligned
					if(isAligned())
					{
						#if _DEBUG_SPEW
							Debug.Log("ALIGNED");
						#endif
						// indicate that no alignment is being sought anymore 
						m_blackBoard.seekDirection = 0;
					}
					else
					{

						bool animate = false;

						// Check if turning bank animation is required
						float Angle = ((Vector3.Dot(_alignmentVector,this.transform.forward)) /
						               (_alignmentVector.magnitude * this.transform.forward.magnitude));
						
						float angleInDegrees = Mathf.Acos(Angle) * 180 / Mathf.PI;
						
						// If the turn is less than 70 degrees , donmt bank
						if(angleInDegrees > 70)
							animate = true;

						// seek alignment to indicated vector 
						m_blackBoard.seekAlignmentToVector(_alignmentVector,turnDirection,animate);

						#if _DEBUG_SPEW
							Debug.Log("NOT ALIGNED YET");
						#endif
					}
				}
			}
			// If the player is in view make sure that the Navmesh is taking care of rotations
			else
			{
				// CHASE IS HERE
				guard.SetDestination(GetComponent<Perception>().ThiefLastPosition);

				// indicate that no alignment is being sought anymore 
				m_blackBoard.seekDirection = 0;
			}

			return BehaveResult.Running;
		}

	}


	#endregion

	#region AI 2.0 helpers

	/// <summary>
	/// Checks if the forward vector for the guard is aligned to the current seekingAlignmentVector
	/// as indicated by the AI Blackboard (m_blackBoard)
	/// </summary>
	/// <returns><c>true</c>, if the guard is looking in the direction indicated by seekingAlignmentVector, <c>false</c> otherwise.</returns>
	bool isAligned()
	{
		float Angle = ((Vector3.Dot(m_blackBoard.seekingAlignmentVector,this.transform.forward)) /
		               (m_blackBoard.seekingAlignmentVector.magnitude * this.transform.forward.magnitude));



#if _DEBUG_SPEW
		Debug.Log("ANGLE : " + Angle + " DOT " + Vector3.Dot(m_blackBoard.seekingAlignmentVector,this.transform.forward));

		float angleInDegrees = Mathf.Acos(Angle) * 180 / Mathf.PI;

		Debug.Log("ANGLE DEGREES : " + angleInDegrees);
#endif

		if(Angle > m_AnglularCheckTreshold) // || (Angle < -m_AnglularCheckTreshold)
		{
			#if _DEBUG_SPEW
				Debug.Log(" ALIGNED ");
			#endif
			return true;
		}
		else
		{
			#if _DEBUG_SPEW
				Debug.Log(" NOT ALIGNED ");
			#endif
			return false;
		}
	}

	// [DEPRECATED]
	/// <summary>
	/// Does a check for all preconditions on every action of the patrol subtree and indicates whether or not we are clear to execute that action
	/// </summary>
	/// <returns><c>true</c>, if tree precondition check passed i.e. The guard is not in the alert or the searching state, <c>false</c> otherwise.</returns>
	//	bool patrolTreePreconditionCheck()
	//	{
	//		// PRECONDITION 1 : If the guard is alert, BAIL
	//		if( gameObject.GetComponent<AlertSystem>().AlertLevel >= 100.0f)
	//		{
	//			#if _DEBUG_SPEW
	//				Debug.Log("PRECONDITION 1 failed in precheck for patrol subtree GAURD IS ALERT, BAILING");
	//			#endif
	//			return false;
	//		}
	//		else
	//			return true;
	//	}

	#endregion
	
		
	void OnGUI()
	{
		if(Player != null)
		{
			if( Player_Caught )
			{
				GUI.Label( new Rect( Screen.width /4, Screen.height/4, 400, 400 ), "BUSTED!!!" );	
				GUI.Label( new Rect( Screen.width /2, Screen.height/2, Screen.width/2, Screen.height/2 ), "Press F2 tp restart the Game" );	
			}
		}
	}
	
	[RPC]
	void SetPosition(Vector3 newPosition)
	{
	    transform.position = newPosition;
	}
	
	[RPC]
	void SetRotation(Quaternion newRotation)
	{
	    transform.rotation = newRotation;
	}
	
	//returns -1 when to the left, 1 to the right, and 0 for forward/backward
    public float AngleDir(Vector3 fwd, Vector3 targetDir, Vector3 up)

    {

        Vector3 perp = Vector3.Cross(fwd, targetDir);

        float dir = Vector3.Dot(perp, up);

 

        if (dir > 0.0f) {

            return 1.0f;

        } else if (dir < 0.0f) {

            return -1.0f;

        } else {

            return 0.0f;

        }

    }
}
