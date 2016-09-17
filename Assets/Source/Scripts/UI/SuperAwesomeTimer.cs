using UnityEngine;
using System.Collections;

public class SuperAwesomeTimer : MonoBehaviour {
	
	/* Ready to be deleted, don't use this anymore
	private float _countDownTime = 3f;	
	private float _startTime = 0f;
	private Node _linkedNode;
	private GameManager.TimerType _type;
	
	#region Public Interface
	//these three are must-do
	public void SetType(GameManager.TimerType i_type)
	{
		_type = i_type;
	}
	
	public void SetCountDownTime(float i_time)
	{
		_countDownTime = i_time;
	}
	
	public void SetLinkedNode(Node i_node)
	{
		_linkedNode = i_node;
	}
	
	public void KillTheTimerBeforeItEnds()
	{
		SelfDestruction();
	}
	#endregion
	
	void Start()
	{
		_startTime = Time.time;	
	}
	
	void Update()
	{
		SetBar( (Time.time - _startTime)/_countDownTime);
		if(Mathf.Abs(Time.time - _startTime - _countDownTime) < 0.1f)
		{
			Finished();
		}
	}
	
	#region when countdown finished
	
	void Finished()
	{
		switch(_type)
		{
		case GameManager.TimerType.Capture:
			CaptureMyNode();
			break;
		case GameManager.TimerType.Release:
			ReleaseMyNode();
			break;
		case GameManager.TimerType.Encrypt:
			EncryptMyNode();
			break;
		case GameManager.TimerType.Tracking:
			TrackedMyNode();
			break;
		}
		SelfDestruction();
	}
	
	void CaptureMyNode()
	{
		
		//change its own state and UI
		_linkedNode.State_ChangeToControlled();
		
		//release the click lock
		GameManager.Manager.clickLock = false;
	
	}
	
	void ReleaseMyNode()
	{
		//change its own state
		_linkedNode.State_ChangeToReleased();
		
		//update the entire node system from source
		//GraphManager.Manager.UpdateNodeSystem();
		
		//release the click lock
		GameManager.Manager.clickLock = false;
		
	}
	
	void EncryptMyNode()
	{
		//change its own state and UI
		_linkedNode.state_Encrypt();

		//release the click lock
		GameManager.Manager.clickLock = false;
	}
	
	void TrackedMyNode()
	{
		
	}
	
	void SelfDestruction()
	{
		GameManager.Manager.clickLock = false;
		Destroy(gameObject);
	}
	
	#endregion
	
	#region visual related
	// use SetBar anywhere between 0 and 1
	void SetBar( float v )
	{
		float offset = 0f;
		if(_type == GameManager.TimerType.Release)
		{			
			offset = Mathf.Clamp01( v/2f );
			renderer.material.mainTextureOffset = new Vector2(offset, 0);
		}
		else if(_type != 0)
		{
			offset = 1f - Mathf.Clamp01( v/2f );
			renderer.material.mainTextureOffset = new Vector2(offset, 0);
		}
		else
		{
			//type is null, do nothing
		}
	}
	#endregion
	*/
}
