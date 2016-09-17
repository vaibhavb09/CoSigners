using UnityEngine;
using System.Collections;

public enum DoorType
{
	NormalDoor,
	OfflineNormalDoor,
	EndDoor,
	StartDoor
};

public abstract class IDoorController : MonoBehaviour
{
	public int 			 		DoorNumber; //Hex index of door
	public DoorNode				_doorNode;
	public DoorType				_type;

	//<public interface >
	public abstract void        Load( int i_number, bool i_locked, bool i_open );
	public abstract void		SetClearance( bool i_clear );
	public abstract DoorType 	GetDoorType();
	public abstract int 		GetDoorIndex();
	public abstract void        InteractWithDoor();
	public abstract void 		OpenDoor();
	public abstract void 		CloseDoor();
	public abstract void 		LockDoor();
	public abstract void		UnlockDoor();
	public abstract void 		DeadlockDoor();
	public abstract void 		UnDeadlockDoor();
	public abstract bool 		CanDeadlockDoor();
	public abstract bool 		CanUndeadlockDoor();
	//</public interface >
}
