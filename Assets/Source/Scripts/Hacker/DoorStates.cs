using UnityEngine;
using System.Collections;

public enum DoorState { UNPOWERED, UNLOCKED, LOCKED, PROTECTED }

public class DoorStates {
	
	// -------------------------------------------------------
	// The hacker has clicked on the door node to interact with it
	// -------------------------------------------------------
	static public void OnInteract ( DoorNode i_door )
	{
		//Debug.Log ("Door is " + ((i_door.isOpen)?"OPEN" : "CLOSED"));
		//Debug.Log ("Hacker Interact");
		if ( i_door._doorType == DoorType.EndDoor ) // Hacer cannot interact with EndDoor
		{
			// Play Protected Beep Sound
		}
		else if ( i_door._doorState == DoorState.PROTECTED )
		{
			// Play Protected Beep Sound
		}
		else if ( i_door._doorState == DoorState.LOCKED )
		{
			i_door._doorState = DoorState.UNLOCKED;
			if( GameManager.Manager.PlayerType == 2)
				soundMan.soundMgr.playOneShotOnSource(null,"Door_Change_State_Hacker",GameManager.Manager.PlayerType,2);
			
			// Queue Unsecurelock disable Timer
			NetworkManager.Manager.DisableDoorTimer(i_door.Index, 2.0f/*1.6f*/);
			NetworkManager.Manager.UnSecureLockDoor( i_door.Index );
		}
		else if ( i_door._doorState == DoorState.UNLOCKED )
		{
			i_door._doorState = DoorState.LOCKED;
			if( GameManager.Manager.PlayerType == 2)
				soundMan.soundMgr.playOneShotOnSource(null,"Door_Change_State_Hacker",GameManager.Manager.PlayerType,2);
			
			// Queue Secure Lock disable Timer
			if ( i_door.isOpen )
			{
				NetworkManager.Manager.DisableDoorTimer(i_door.Index, 3.0f/*2.5f*/);
				NetworkManager.Manager.CloseDoor( i_door.Index );
			}
			else
				NetworkManager.Manager.DisableDoorTimer(i_door.Index, 2.0f/*1.6f*/);
			
			NetworkManager.Manager.SecureLockDoor( i_door.Index );
		}
	}
	
	// -------------------------------------------------------
	// The hacker has connected powered links to a previously 
	// unpowered door node.
	// -------------------------------------------------------
	static public void OnConnect ( DoorNode i_door )
	{
		// Most of the time on connect, a door will be unlocked
		DoorState myState = i_door._doorState;
		
		// First check conditions where door cannot be unlocked
		if ( i_door._doorType == DoorType.EndDoor && OverrideManager.Manager.IsActive )
		{
			i_door._doorState = DoorState.PROTECTED;
			//i_door.lockedInLockdown = true;
		}
		else
		{
			// If none of the edge cases, unlock the door.
			i_door._doorState = DoorState.UNLOCKED;
			NetworkManager.Manager.UnlockDoor( i_door.Index );
		}
	}
	
	// -------------------------------------------------------
	// The hacker has disconnected power from the door
	// -------------------------------------------------------
	static public void OnDisconnect ( DoorNode i_door )
	{
		//Debug.Log ("On Disconnect Door");
		//i_door._doorState = DoorState.UNPOWERED;
		i_door.Disconnect();
		
		if ( i_door._doorState == DoorState.LOCKED )
		{
			// Queue Unsecurelock disable Timer
			NetworkManager.Manager.DisableDoorTimer(i_door.Index, 1.6f);
			NetworkManager.Manager.UnSecureLockDoor( i_door.Index );
		}
		else if ( i_door.isOpen )
		{
			// Queue Close Door disable Timer
			NetworkManager.Manager.DisableDoorTimer(i_door.Index, 1.2f);
			NetworkManager.Manager.CloseDoor( i_door.Index );
		}
		
		// Lock the Door
		NetworkManager.Manager.LockDoor( i_door.Index );
		i_door._doorState = DoorState.UNPOWERED;
	}

	// -------------------------------------------------------
	//
	// -------------------------------------------------------
	static public void OnActorEnter ( DoorNode i_door )
	{
		//Debug.Log ("On Actor Enter");
		if ( i_door._doorType == DoorType.NormalDoor && i_door.Connected )
		{
			if ( i_door._disabled )
			{
				i_door.EndDisableTimer();
			}
			i_door.Block();
		}

	}

	// -------------------------------------------------------
	//
	// -------------------------------------------------------
	static public void OnActorExit ( DoorNode i_door )
	{
		//Debug.Log ("On Actor Exit");
		if ( i_door._doorType == DoorType.NormalDoor && i_door._disabled)
		{
			i_door.Unblock();
		}
	}

	// -------------------------------------------------------
	// The player has opened the door
	// -------------------------------------------------------
	static public void OnPlayerOpen ( DoorNode i_door )
	{
		//Debug.Log ("Player Open Door");
		if ( i_door.Connected )
		{
			//Debug.Log ("THIS DOOR IS CONNECTED");
			// Queue Door Open disable Timer
			if ( i_door._doorType == DoorType.NormalDoor)
				NetworkManager.Manager.DisableDoorTimer(i_door.Index, 1.2f);

			if ( i_door._doorType == DoorType.EndDoor || i_door._doorType == DoorType.StartDoor )
			{
				//Debug.Log ("THIS IS AN END DOOR");
				// Animate Open Ring
				i_door.StartRingTimer();
			}
			
			i_door.OpenDoor();
		}
	}
	
	// -------------------------------------------------------
	//	The player has closed the door
	// -------------------------------------------------------
	static public void OnPlayerClose ( DoorNode i_door )
	{
		i_door.isOpen = false;
		if ( i_door.Connected )
		{
			// Queue Door Close disable Timer
			if ( i_door._doorType == DoorType.NormalDoor)
				NetworkManager.Manager.DisableDoorTimer(i_door.Index, 1.2f);

			if ( i_door._doorType == DoorType.EndDoor || i_door._doorType == DoorType.StartDoor )
			{
				// Animate Close Ring
				i_door.StartRingTimer();
			}
			
			i_door.CloseDoor();
		}
	}
	
	
}
