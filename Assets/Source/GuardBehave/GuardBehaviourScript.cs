//#define _BEHAVIOR_DEBUG_SPEW

using UnityEngine;
using System.Collections;
using Behave.Runtime;
using Tree = Behave.Runtime.Tree;

public class GuardBehaviourScript : MonoBehaviour , IAgent{
	
	Tree m_Tree;

	bool m_isActive;
	public bool IsActive
	{
		get{return m_isActive;}
	}

	/// <summary>
	/// Indicates the type of the guard in use
	/// 0 : Default , Old guards [To be Deprecated]
	/// 1 : New guards AI 2.0
	/// </summary>
	private int m_GuardType = 0;
	public int GuardType
	{
		set
		{
			m_GuardType = value;
		}
		get
		{
			return m_GuardType;
		}
	}

	// Use this for initialization
	IEnumerator Start () {
	
		m_isActive = true;

	 if(m_GuardType == 1 )
		{
			// New AI
			m_Tree = BLGuardBehaveLibrary.InstantiateTree(
				BLGuardBehaveLibrary.TreeType.GuardCollection_GuardAI,this
				);
		}

		// [QUERY_NIK]
		//if ( GameManager.Manager.PlayerType == 1 )
		{
		while (Application.isPlaying && m_Tree != null)
		{
			//Debug.Log("Waiting for " + 1.0f/m_Tree.Frequency + " seconds");
			yield return new WaitForSeconds (1.0f/m_Tree.Frequency);	
			AIUpdate();
		}
		}

		//Debug.Log("Out of tree");
		
	}

	#region AI 2.0 helpers

	/// <summary>
	/// Pauses the AI updates.
	/// </summary>
	/// <returns><c>true</c>, if AI updates were paused, <c>false</c> if they were paused when the method was called</returns>
	public bool pauseAIUpdates()
	{
		if(IsActive)
		{
			m_isActive = false;
			return true;
		}else
			return false;
	}

	/// <summary>
	/// Resumes the AI updates.
	/// </summary>
	/// <returns><c>true</c>, if AI updates were resumed, <c>false</c> if they were already running when the method was called.</returns>
	public bool resumeAIUpdates()
	{
		if(!IsActive)
		{
			m_isActive = true;
			return true;
		}else
			return false;
	}

	#endregion

	void AIUpdate () 
	{
		if(IsActive)
			m_Tree.Tick();
	}

	public void	 Reset (Tree sender)
		{
		//Debug.Log("In Reset");
		}

	// [DEPRECATED]
	public int SelectTopPriority (Tree sender, params int[] IDs)
		{
			//Debug.Log("In SelectTopPriority  ActiveComponent - " + sender.ActiveComponent);
		//Debug.Log("In SelectTopPriority ContextType - " + BLNewBehaveLibrary0.ContextType);
		//Debug.Log("In SelectTopPriority ActiveContext - " + m_Tree.ActiveContext);
		//if (m_Tree.ActiveContext == BLNewBehaveLibrary0.ContextType.P1)
		//{
		//	Debug.Log("In SelectTopPriority  Priority P1 ");
		//}
		//if (m_Tree.ActiveContext == BLNewBehaveLibrary0.ContextType.P2)
		//{
		//	Debug.Log("In SelectTopPriority  Priority P2 ");
		//}
			return IDs[1];
		}


	public BehaveResult	 Tick (Tree sender, bool init)
	{
	
#if _BEHAVIOR_DEBUG_SPEW
		Debug.Log("GENERAL TICK");
		
		Debug.Log("Got ticked by " + (BLGuardBehaveLibrary.IsAction(sender.ActiveID).ToString()));
#endif
		
		return BehaveResult.Success;
	}


	#region Patrol Behavior Subtree AI 2.0

	public BehaveResult TickgoToWayPointAction(Tree sender)
	{
		#if _BEHAVIOR_DEBUG_SPEW
			Debug.Log(" Go to Waypoint Action ");
		#endif

		return gameObject.GetComponent<GuardActions>().goToWayPoint();
	}

	public BehaveResult TickwaitForWayPointAction(Tree sender)
	{
		#if _BEHAVIOR_DEBUG_SPEW
			Debug.Log(" Wait For Waypoint Action");
		#endif
		return gameObject.GetComponent<GuardActions>().waitForCondition();
	}

//	public BehaveResult TickdoWayPointAction(Tree sender)
//	{
//
//		Debug.Log(" Do Waypoint Action ");
//
//		return gameObject.GetComponent<GuardActions>().doWayPointActions();
//	}

	/******************* Scan 360 ***********************/
	public BehaveResult TickcheckFullAction(Tree sender)
	{
		#if _BEHAVIOR_DEBUG_SPEW
			Debug.Log(" Check 360 Action taken ");
		#endif
		return gameObject.GetComponent<GuardActions>().checkScan360();
	}

	public BehaveResult TickdoScanFullAction(Tree sender)
	{
		#if _BEHAVIOR_DEBUG_SPEW
			Debug.Log(" Do Scan 360 Action taken ");
		#endif

		return gameObject.GetComponent<GuardActions>().doScan360();
	}
	/******************* END Scan 360 ***********************/

	/******************* Scan 180 ***********************/
	public BehaveResult TickcheckHalfAction(Tree sender)
	{
		#if _BEHAVIOR_DEBUG_SPEW
			Debug.Log(" Check 180 Action taken");
		#endif
		return gameObject.GetComponent<GuardActions>().checkScan180();
	}
	
	public BehaveResult TickdoScanHalfAction(Tree sender)
	{
		#if _BEHAVIOR_DEBUG_SPEW
			Debug.Log(" Do Scan 180 Action taken ");
		#endif

		return gameObject.GetComponent<GuardActions>().doScan180();
	}
	/******************* END Scan 180 ***********************/


	/******************* NO Scan ***********************/
	public BehaveResult TickcheckNoScanAction(Tree sender)
	{
		#if _BEHAVIOR_DEBUG_SPEW
		Debug.Log(" Check No Action taken");
		#endif

		return gameObject.GetComponent<GuardActions>().checkNoScan();
	}
	
	public BehaveResult TickdontScanAction(Tree sender)
	{
		#if _BEHAVIOR_DEBUG_SPEW
			Debug.Log(" Do No Scan Action taken");
		#endif
		return gameObject.GetComponent<GuardActions>().dontScan();
	}
	/******************* END NO Scan***********************/

	/******************* WAYPOINT ACTIONS ***********************/

	public BehaveResult TickdoPauseOneAction(Tree sender)
	{
		#if _BEHAVIOR_DEBUG_SPEW
		Debug.Log(" Doing the first pause ");
		#endif
		return gameObject.GetComponent<GuardActions>().doPauseOne();
	}

	public BehaveResult TicklookToDirectionAction(Tree sender)
	{
		#if _BEHAVIOR_DEBUG_SPEW
		Debug.Log(" Looking in indicated direction ");
		#endif		
		return gameObject.GetComponent<GuardActions>().lookToDirection();
	}

	public BehaveResult TickdoPauseTwoAction(Tree sender)
	{
		#if _BEHAVIOR_DEBUG_SPEW
		Debug.Log(" Doing the second pause ");
		#endif	
		return gameObject.GetComponent<GuardActions>().doPauseTwo();
	}

	public BehaveResult TickalignToLeaveAction(Tree sender)
	{
		#if _BEHAVIOR_DEBUG_SPEW
		Debug.Log(" Aligning to leave ");
		#endif		
		return gameObject.GetComponent<GuardActions>().alignToLeave();
	}


	/******************* END WAYPOINT ACTIONS *******************/


	public BehaveResult TickgetNextWayPointAction(Tree sender)
	{
		#if _BEHAVIOR_DEBUG_SPEW
		Debug.Log("Get Next Waypoint Action");
		#endif		
		return gameObject.GetComponent<GuardActions>().getNextWayPoint();
	}


	#endregion

	#region Chase Behavior subtree AI 2.0

	public BehaveResult TicktrackPlayerAction(Tree sender)
	{
		#if _BEHAVIOR_DEBUG_SPEW
			Debug.Log("TRACKED PLAYER");
		#endif
		return gameObject.GetComponent<GuardActions>().trackPlayer();
	}

	public BehaveResult TickchasePlayerAction(Tree sender)
	{
		#if _BEHAVIOR_DEBUG_SPEW
			Debug.Log("CHASED PLAYER");
		#endif
		return gameObject.GetComponent<GuardActions>().chasePlayer();
	}

	#endregion

	#region Search2 behavior AI 2.0
	// findMostRelevantSearchPoint
	public BehaveResult TickfindMostRelevantSearchPointAction(Tree sender)
	{
		#if _BEHAVIOR_DEBUG_SPEW
			Debug.Log("FIND ********************** MOST RELEVANT SEARCH POINT TICK");
		#endif

		return gameObject.GetComponent<GuardActions>().findMostRelevantSearchPoint(sender);
	}

	public BehaveResult TickwaitForSearchPointAction(Tree sender)
	{
		#if _BEHAVIOR_DEBUG_SPEW
			Debug.Log("WAITING for SEARCH POINT TICK");
		#endif
		
		return gameObject.GetComponent<GuardActions>().waitTillSearchPointAction(sender);

	}

	public BehaveResult TicklookAroundAtSearchPointAction(Tree sender)
	{
		#if _BEHAVIOR_DEBUG_SPEW
			Debug.Log("LOOKING AROUND AT THE SEARCH POINT");
		#endif
		return gameObject.GetComponent<GuardActions>().lookAroundAtSearchPoint(sender);
	}

	public BehaveResult TickwaitForLookAroundAction(Tree sender)
	{
		#if _BEHAVIOR_DEBUG_SPEW
			Debug.Log("WAITING FOR GUARD LOOK AROUND TO FINISH");
		#endif
		return gameObject.GetComponent<GuardActions>().waitForLookAround(sender);
	}

	#endregion

	#region Common AI 2.0

	public BehaveResult TickhighLevelInterruptDecorator(Tree sender)
	{
		//Debug.Log("HIGH LEVEL INTERRUPT HERE");

		return gameObject.GetComponent<GuardActions>().highLevelInterruptDecorator(sender);
		//return BehaveResult.Success;
	}

	public BehaveResult TickparallelHighLevelInterruptAction(Tree sender)
	{
		#if _BEHAVIOR_DEBUG_SPEW
			Debug.Log("PARALLEL TO HIGH LEVEL INTERRUPT");
		#endif

		return gameObject.GetComponent<GuardActions>().highLevelInterruptDecorator(sender);

	}

	public BehaveResult TickisGuardAlertAction(Tree sender)
	{
		//Debug.Log("HIGH LEVEL INTERRUPT HERE");
		
		return gameObject.GetComponent<GuardActions>().guardAlertInterruptor();
		//return BehaveResult.Failure;
	}

	public BehaveResult TickwaitForAlignAction(Tree sender)
	{
#if _BEHAVIOR_DEBUG_SPEW
		Debug.Log("Waiting for alignment");
#endif
		return gameObject.GetComponent<GuardActions>().waitForAlign();
	}

	/// <summary>
	/// Checks if a search is required this AI Frame, interrupts if it isnt
	/// </summary>
	/// <returns>The result of the Tick</returns>
	/// <param name="sender">Behavior Tree</param>
	public BehaveResult TickisSearchRequiredDecorator(Tree sender)
	{
		//Debug.Log("HIGH LEVEL INTERRUPT HERE");
		
		return gameObject.GetComponent<GuardActions>().isSearchRequired(sender);
		//return BehaveResult.Success;
	}

	//parallelIsSearchRequiredInterrupt
	public BehaveResult TickparallelIsSearchRequiredInterruptAction(Tree sender)
	{
		#if _BEHAVIOR_DEBUG_SPEW
			Debug.Log("PARALLEL TO SEARCH REQUIRED INTERRUPT");
		#endif

		return gameObject.GetComponent<GuardActions>().isSearchRequired(sender);
	}

	#endregion


}
