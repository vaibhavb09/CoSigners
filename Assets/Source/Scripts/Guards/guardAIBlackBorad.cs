using UnityEngine;
using System.Collections.Generic;


class guardAIBlackBoard
{

#region Patrol Data

	/// <summary>
	/// Indicates the patrol path of this guard.
	/// </summary>
	private GuardPath m_Path;

	/// <summary>
	/// Generally Indicates the waypoint that the Guard must navigate to next
	/// *In the case of the first waypoint it sometimes indicates the one where the Guard is at currently, this is Ok, the AI is meant to account for that 
	/// **In case of stop waypoints it will end up indicating the last waypoint that the guard has stopped at
	/// </summary>
	public int m_MostRelevantWaypoint;

	private GuardAnimationController mAnimationController;

	public int CurrentWaypoint
	{
		get
		{
			return m_MostRelevantWaypoint;
		}
		set
		{
			// If this is a valid value for the next Waypoint
			if(value < m_Path.WayPoints.Count)
				m_MostRelevantWaypoint = value;
		}
	}

	/// <summary>
	/// The decaying signal used for the waypoint waits
	/// STATE INFO : THIS IS CLEARED ON INTERRUPT
	/// </summary>
	public decayingSignal m_wayPointWaitDecayingSignal = null;
	public decayingSignal wayPointDecayingSignal
	{
		get 
		{
			return m_wayPointWaitDecayingSignal;
		}
		// Was causing stack overflows so had to remove and make the signal public
		//		set
		//		{
		//			wayPointDecayingSignal = value;
		//		}
	}

	// Indicates the direction of traversal on a given path
	// 1 means traversing in order
	// -1 means traversing in reverse order
	// is generally just 1 for Looping and Stop paths
	// oscillates between -1 and 1 for reversible paths
	// STATE INFO : THIS IS RETAINED ON INTERRUPT
	private int m_traverseDirection = 1;
	public int TraversalDirection
	{
		get
		{
			if(Mathf.Abs(m_traverseDirection) > 1)
				Debug.LogError("NIK : INVALID TRAVERSAL DIRECTION REQUESTED , if you see this tell Nik");
			
			return m_traverseDirection;
		}
	}

#endregion

#region Common Data

	/// <summary>
	/// The Direction that the guard is currently seeking alignment to
	/// </summary>
	private Vector3 m_seekingAlignment;
	public Vector3 seekingAlignmentVector
	{
		get
		{
			return m_seekingAlignment;
		}
		set
		{
			m_seekingAlignment = value;
		}
	}

	/// <summary>
	/// The direction in which the ROTATION to m_seekingAlignment will happen
	/// 0 : No seek in progress , thus no rotation
	/// 1 & -1 : Clockwise and anti-clockwise (	WHICH IS WHICH ? )
	/// STATE INFO : THIS IS CLEARED ON INTERRUPT
	/// </summary>
	private int m_seekDirection = 0;
	public int seekDirection
	{
		get
		{
			return m_seekDirection;
		}
		set
		{
			if(Mathf.Abs(m_seekDirection) > 1)
				Debug.LogError("Alignment being sought in invalid direction");

			if(value == 0)
			{
				if(m_seekDirection == 1)
					mAnimationController.turnRightEnd();
				else if(m_seekDirection == -1)
					mAnimationController.turnLeftEnd();
			}

			m_seekDirection = value;
		}
	}

	/// <summary>
	/// A modifier for the angular velocity of the Guards for all alignment operations
	/// needs to be 1 for the velocity to be normal
	/// 0 when there is a forced pause on rotation
	/// Greater than 1 for a speedup and less than 1 for a slowdown
	/// </summary>
	private int m_angularSpeedModifier = 1;
	public int AngularSpeedModifier
	{
		get
		{
			return m_angularSpeedModifier;
		}
	}

#endregion

#region Search Behavior Data

	/// <summary>
	/// Indicates the currently relevant search point
	/// </summary>
	//public SearchPoint mCurrentSearchPoint;

	/// <summary>
	/// Holds the search points that the guard has gone to while in this search operation
	/// </summary>
	//public List<int> m_PreviousSearchPoints;

	/// <summary>
	/// The duration for which the guard should carry on with its search behavior
	/// </summary>
	public float m_SearchBehaviorDuration = 60.0f;

	/// <summary>
	/// The decaying signal used by the guard to time its search operations
	/// </summary>
	public decayingSignal isGuardSearchActive;

	/// <summary>
	/// Indicates to this guard if it needs to do its Search behavioru
	/// </summary>
	private bool m_SearchRequired;
	public bool SearchRequired
	{
		get
		{
			return m_SearchRequired;
		}
		set
		{
			m_SearchRequired = value;
		}
	}

#endregion


	/// <summary>
	/// Initializes a new instance of the <see cref="guardAIBlackBoard"/> class.
	/// </summary>
	/// <param name="iGuardPath">The patrol path for this guard.</param>
	public guardAIBlackBoard(GuardPath iGuardPath,GuardAnimationController iAnimationController)
	{
		m_MostRelevantWaypoint = 0;

		m_seekDirection = 0;

		m_wayPointWaitDecayingSignal = null;

		m_traverseDirection = 1;

		m_Path = iGuardPath;

		m_SearchRequired = false;

		mAnimationController = iAnimationController;

//		m_PreviousSearchPoints = new List<int>();
//		m_PreviousSearchPoints.Clear();	}
	}

	/// <summary>
	/// Makes the guard seek alignment to the indicated vector.
	/// </summary>
	/// <param name="i_seekingAlignmentVector"> The Direction that the guard should seek alignment to </param>
	/// <param name="i_seekDirection">The direction in which the ROTATION to m_seekingAlignment will happen / Default value : 1 
	/// any value with absolute value greater than 1 will result in the sweep direction being reverted </param>
	public void seekAlignmentToVector(Vector3 i_seekingAlignmentVector,int i_seekDirection = 1,bool animate = true)
	{
		int prevSeekDirection = seekDirection;

		// set the vector being sought
		seekingAlignmentVector = i_seekingAlignmentVector;

		if(Mathf.Abs(i_seekDirection) <= 1)
			// set the seek direction
			m_seekDirection = i_seekDirection;
		else
			m_seekDirection *= -1;

		if(animate)
			if(prevSeekDirection == 0)
			if(seekDirection == 1)
				mAnimationController.rightTurnStart();
			else if(seekDirection == -1)
				mAnimationController.turnLeftStart();
	}

	/// <summary>
	/// Reverses direction in which the guard turns from Clockwise to Anticlockwise and vise versa
	/// </summary>
	public void reverseSweepDirection()
	{
		m_seekDirection *= -1;
	}

	/// <summary>
	/// Indicates that the guard is now traversing the reverse of the original path.
	/// </summary>
	public void reverseTraversalDirection()
	{
		m_traverseDirection *= -1;
	}

	public void pauseGuard()
	{
		// Pause the Waypoint Timer
		if(wayPointDecayingSignal != null)
			wayPointDecayingSignal.Pause();
		// Pause the Search timer
		if(isGuardSearchActive != null)
			isGuardSearchActive.Pause();

		// Indcate that the Angular speed modifier is 0 (so as to stop all Alignment ops)
		m_angularSpeedModifier = 0;
	}

	public void resumeGuard()
	{
		// Resume the Waypoint Timer
		if(wayPointDecayingSignal != null)
			wayPointDecayingSignal.Resume();

		// Resume the search Timer
		if(isGuardSearchActive != null)
			isGuardSearchActive.Resume();
		
		// Indcate that the Angular speed modifier is 1 (so as to resume all Alignment ops)
		m_angularSpeedModifier = 1;
	}
}