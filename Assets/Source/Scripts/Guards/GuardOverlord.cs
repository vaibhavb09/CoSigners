
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// Encapsulates the representation of a Waypoint
/// </summary>
public class WayPoint :IComparable<WayPoint>
{
	#region DataMembers

	/// <summary>
	/// The Position of the waypoint
	/// </summary>
	private Vector3 m_Position;
	public Vector3 Position
	{
		get
		{
			return m_Position;
		}
		set
		{
			m_Position = value;
		}
	}

	/// <summary>
	/// Indicates the order in which nodes are to be traversed
	/// </summary>
	private int m_OrderID;
	public int OrderID
	{
		get
		{
			return m_OrderID;
		}
		set
		{
			m_OrderID = value;
		}
	}

	/// <summary>
	/// Indicates the pause before the turn
	/// </summary>
	private float m_Pause1;
	public float FirstPauseDuration
	{
		get
		{
			return m_Pause1;
		}
		set
		{
			m_Pause1 = value;
		}
	}

	/// <summary>
	/// The look direction after the first pause
	/// Values : 
	/// None : Dont turn at all
	/// Left : Turn Left 90 degrees
	/// Right : Turn Right 90 degrees
	/// </summary>
	private string m_Look;
	public string Look
	{
		get
		{
			return m_Look;
		}
		set
		{
			m_Look = value;
		}
	}

	/// <summary>
	/// Indicates the pause after the turn
	/// </summary>
	private float m_Pause2;
	public float SecondPauseDuration
	{
		get
		{
			return m_Pause2;
		}
		set
		{
			m_Pause2 = value;
		}
	}

	/// <summary>
	/// The direction to turn after the second pause in order to orient to the next waypoint
	/// Values : 
	/// Closest : Choose the closest one and go to it
	/// Left : Turn Left 
	/// Right : Turn Right
	/// </summary>
	private string m_Turn;
	public string Turn
	{
		get
		{
			return m_Turn;
		}
		set
		{
			m_Turn = value;
		}
	}


	/// <summary>
	/// Compares the current object with another object of the same type.
	/// </summary>
	/// <returns>The other object to be compared to</returns>
	/// <param name="i_OtherWaypoint">I_ other waypoint.</param>
	public int CompareTo(WayPoint i_OtherWaypoint)
	{
		if(i_OtherWaypoint.OrderID < OrderID)
			return 1;
		else
			return 0;
	}

	#endregion
}

/// <summary>
/// Encapsulates the representation of a Guard Path
/// </summary>
public class GuardPath
{
	public int PathNum;
	public string Type;

	// Questionable
	public float lastCreated;

	// Contains a list of all Waypoints that make up a Guards Path
	public List<WayPoint> WayPoints;

	// [NIK : DEPRECATED]
	#region Deprecated
	public float Speed;
	public bool Duplicate;
	public float Frequency;
	#endregion

}

public class GuardOverlord : Photon.MonoBehaviour 
{
	/// <summary>
	/// The Static instance of the Guard overlord singleton
	/// </summary>
	private static GuardOverlord m_instance;
	public static GuardOverlord Manager
	{
		get
		{
			if(m_instance == null)
			{
				m_instance = new GuardOverlord();			
			}
			return m_instance;
		}
	}

	/// <summary>
	/// The guard prefab.
	/// </summary>
	public GameObject GuardPrefab;

	/// <summary>
	/// The list of all guards managed by the Guard Overlord
	/// </summary>
	public List<GameObject> m_Guards;

	/// <summary>
	/// The spawn timers.[Kiran]
	/// </summary>
	public float []SpawnTimers ;

	/// <summary>
	/// The guard paths, this is what basically defines a guard
	/// </summary>
	public GuardPath[] GuardPaths;
	
	/// <summary>
	/// The Distraction radius. [DEPRECATED]
	/// </summary>
	public float m_DistractionRadius = 10.0f;

	/// <summary>
	/// The Influence radius. [DEPRECATED]
	/// </summary>
	public float m_influenceRadius = 10.0f; 									//FOR GUARDS TO REACT TO EACH OTHER'S ACTIONS

	/// <summary>
	/// [DEPRECATED] indicates the radius over which the guard can hear players running 
	/// </summary>
	public float m_guardHearingRadius = 5.0f;

	/// <summary>
	/// Indicates whether the Guards are paused
	/// </summary>
	private bool mGuardsPaused;
	public bool GuardsPaused
	{
		get{ return mGuardsPaused;}
	}
	
	public GuardOverlord () 
    {        
        m_instance = this;
    }

	/// <summary>
	/// Initializes the Guard Overlord
	/// </summary>
	public void OverLordInit(GraphData i_graphData)
	{
		// Creates the guard paths
		CreatePaths(i_graphData);

		m_Guards = new List<GameObject>();
		//Debug.Log("Calling Instantiate Guards");
		// Instatntiates the guards
		InstantiateGuards();

		//Debug.Log("After calling instantiate guards");


		// Fetch all Guards from the scene
		//GameObject [] listOfGameObjects;
		//listOfGameObjects = GameObject.FindGameObjectsWithTag("Guard");

		/// Push them into the list of managed guards
		//for( int i = 0; i < listOfGameObjects.Length ; i++)
		//{
		//	m_Guards.Add(listOfGameObjects[i]);
		//}
		//GameObject[] guard2D = GameObject.FindGameObjectsWithTag("Guard2D");
		//Debug.Log ("FINISHED OVERLOAD INITIALIZATION: " + guard2D.Length);
	}

	/// <summary>
	/// Pauses all guards.
	/// </summary>
	/// <returns><c>true</c>, if all guards were successfully paused, <c>false</c> if they were already paused or if they cannot be paused at this time. </returns>
	public bool pauseAllGuards()
	{
		if ( GameManager.Manager.PlayerType == 1 )
		{
			if(!mGuardsPaused)
			{
				mGuardsPaused = true;

				foreach(GameObject guard in m_Guards)
				{

				// Pause AI Updates
					guard.GetComponent<GuardBehaviourScript>().pauseAIUpdates();

				// Pause All Guard Sounds
					soundMan.soundMgr.pauseSource(guard.audio);

				// Pause All Guard Movement 
					guard.GetComponent<NavMeshAgent>().Stop();

				// Pause All Guard Timers
				// Pause Any Alignment Operations
					guard.GetComponent<GuardActions>().pauseGuard();
				}

				return true;
			}else
			{
				return false;
			}
		}
		return true;
	}

	/// <summary>
	/// Resumes all guards.
	/// </summary>
	/// <returns><c>true</c>, if all guards were resumed, <c>false</c> if they were already running.</returns>
	public bool resumeAllGuards()
	{
		if(mGuardsPaused)
		{
			mGuardsPaused = false;
			
			foreach(GameObject guard in m_Guards)
			{
				// Resume AI Updates
				guard.GetComponent<GuardBehaviourScript>().resumeAIUpdates();
				
				// Resume All Guard Sounds
				soundMan.soundMgr.resumeSource(guard.audio);
				
				// Resume All Guard Movement 
				guard.GetComponent<NavMeshAgent>().Resume();
				
				// Resume All Guard Timers
				// Resume Any Alignment Operations
				guard.GetComponent<GuardActions>().resumeGuard();
			}
			return true;
		}
		else
		{
			return false;
		}
	}


	void Update() 
	{
		/*
		for ( int i=0 ; i<m_Guards.Count ; i++ )
		{
			AlertSystem alertScript = m_Guards[i].GetComponent<AlertSystem>();
			if ( alertScript.AlertLevel > 0 )
			{
				// Determine if Guard is on Screen
				GameObject playerCamera = (GameObject)GameObject.Find ("FPSCamera");
				Vector3 screenPos = playerCamera.camera.WorldToScreenPoint( m_Guards[i].transform.position );
				bool onScreen = (screenPos.x<Screen.width && screenPos.x>0 && screenPos.y<Screen.height && screenPos.y>0 );
				
				if ( onScreen )
				{
//					Debug.Log ("GUARD IS ON SCREEN!!!");
				}
				else // Guard is offscreen
				{
					// Get Angle between Guard and Player view.
					Vector3 playerForward = playerCamera.transform.TransformDirection(Vector3.forward);
					Vector3 playerRight = playerCamera.transform.TransformDirection(Vector3.right);
					Vector3 targetDir = m_Guards[i].transform.position - playerCamera.transform.position;
					
					float angleFwd = Vector3.Angle(targetDir, playerForward);
					float angleRt = Vector3.Angle(targetDir, playerRight);
					bool right = (angleRt<90.0f);
					
					AlertPositioning positioningScript = m_Guards[i].GetComponent<AlertPositioning>();
					//positioningScript.
					
//					Debug.Log ("Angle: " + angleFwd + " - Right: " + right + " - Onscreen: " + screenPos.x + "," + screenPos.y + "," + screenPos.z);
				}
				
				
				
			}
		}*/
	}

	/// <summary>
	/// Instantiates guards depengin on what is read from the XML file
	/// </summary>
	public void InstantiateGuards()
	{
		//if ( GameManager.Manager.PlayerType == 1)
		{
			int i=0;
			//Debug.Log("No of guardPaths= "+GuardPaths.Length);
			foreach(GuardPath guardPath_temp in GuardPaths)
			{
				//Debug.Log("in guard instantiate");
				string tempPlayerString = PhotonNetwork.player.ToString();
				int playerNumber =  Convert.ToInt32(tempPlayerString);

				Vector3 _guardLookDirection = GuardPaths[i].WayPoints[1].Position - GuardPaths[i].WayPoints[0].Position;
				Quaternion initialLook = Quaternion.identity;
				initialLook.SetLookRotation(_guardLookDirection,Vector3.up);

				//Transform newPlayer = (Transform)PhotonNetwork.Instantiate(player2DPrefab,
				//new Vector3(StartPoint.transform.position.x, 60, StartPoint.transform.position.z), transform.rotation, playerNumber);
				GameObject tempGuard = (GameObject)Instantiate(GuardPrefab, GuardPaths[i].WayPoints[0].Position, initialLook);
				tempGuard.GetComponent<GuardSync>().GuardId = i;
				m_Guards.Add(tempGuard);
				//GameObject tempGuard = (GameObject)Instantiate(GuardPrefab, GuardPaths[i].WayPoints[0].Pos, Quaternion.identity);
				tempGuard.GetComponent<GuardActions>().Path = GuardPaths[i];
				//tempGuard.GetComponent<GuardActions>().GuardName = "XMLGuard";

				// Guard AI 2.0
				//tempGuard.GetComponent<GuardActions>().CurrentWaypoint = 0;
				tempGuard.GetComponent<GuardBehaviourScript>().GuardType = 1;

				GuardPaths[i].lastCreated = Time.time;

				//Debug.Log ("FINISHED INITILIZING GURAD PREFAB");
				//GuardSync syncScript = tempGuard.GetComponent<GuardSync>();
				//syncScript.Initialize();

				i++;
				//break;
			}
		}
		
	}
	
	
	public void CreatePaths(GraphData i_graphData)
	{//Debug.Log("In Create Paths Function");
		GuardPathData[] guardPathDatalist = i_graphData.GuardPaths;
		int i = 0;
		//Debug.Log("IguardPathDatalist length= "+guardPathDatalist.Length);
		GuardPaths =  new GuardPath[guardPathDatalist.Length];
		SpawnTimers = new float[guardPathDatalist.Length];
		foreach(GuardPathData guardPathdata_temp in guardPathDatalist)
		{
			GuardPaths[i] = new GuardPath();
			GuardPaths[i].PathNum = i;
			
			GuardPaths[i].Type 		= guardPathdata_temp.Type;
			GuardPaths[i].Speed 	= guardPathdata_temp.Speed;
			GuardPaths[i].Duplicate = guardPathdata_temp.Duplicate;
			GuardPaths[i].Frequency = guardPathdata_temp.Frequency;
			
			SpawnTimers[i] = guardPathdata_temp.Frequency;
			
			GuardPaths[i].lastCreated = 0.0f;
			
			WayPointData[] wayPointDataList = guardPathdata_temp.WayPoints;
			
			GuardPaths[i].WayPoints = new List<WayPoint>(); //new WayPoint[wayPointDataList.Length];
			
			int j=0;
			foreach(WayPointData wayPointdata_temp in wayPointDataList)
			{
				GuardPaths[i].WayPoints.Add(new WayPoint());

				//Debug.Log("Creating new waypoint for guard "+wayPointdata_temp.XPos+ " " +wayPointdata_temp.ZPos);
				GuardPaths[i].WayPoints[j].Position =  new Vector3(wayPointdata_temp.XPos, 2.0f , wayPointdata_temp.ZPos);
				GuardPaths[i].WayPoints[j].OrderID = wayPointdata_temp.OrderID;
				GuardPaths[i].WayPoints[j].FirstPauseDuration 	= wayPointdata_temp.Pause1;
				GuardPaths[i].WayPoints[j].Look 	= wayPointdata_temp.Look;
				GuardPaths[i].WayPoints[j].SecondPauseDuration 	= wayPointdata_temp.Pause2;
				GuardPaths[i].WayPoints[j].Turn 	= wayPointdata_temp.Turn;
				j++;
			}

			// Sort the Waypoints for this guard according to their order id
			GuardPaths[i].WayPoints.Sort();

			i++;
		}
		
	}
	
	/**
	 * @Description : Indicates to the guard overlord that the theif is running 
	 * 				  and takes up the task of informing the guards autitory perception systems of the fact
	 * @Param : 1. The transform of the theif
	 * */
	public bool mThiefIsRunning(Transform iThiefPosition)
    {
		// For each guard on the level
        foreach (GameObject guard in m_Guards)
        {
			// Calculate distance to theif
			float distToGuard = Vector3.Distance(guard.transform.position,
				iThiefPosition.transform.position);
			
			// Inform the guard of the amount of auditory perception
//			((GuardPerception)guard.GetComponent("GuardPerception"))
//					.heardTheif((distToGuard > m_guardHearingRadius) ? 0 : (distToGuard/m_guardHearingRadius));			
        }
        return true;
	}
    
}