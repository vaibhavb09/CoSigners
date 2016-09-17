using UnityEngine;
//using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class Node : MonoBehaviour
{
	#region Members
	private int 					_index;				// The index number of the node.  Always corresponds to the intersections number that the node is on.
	private GameManager.NodeType 	_type;				// The type of node.
	private bool 					_connected;			// If the node is currently connected to a source node.
	private int						_securityLevel;		// The level of security clearance needed to access this node.
	private int						_powerConsumption;
	#endregion
	
	public Node()
	{
		//Debug.Log ("Created New Node 0 args");
	}
	
	#region Properties
	public bool Connected
	{
		get{
			return _connected;
		}
		set{
			this._connected = value;
		}
	}
	
	public int Index
	{
		get{
			return _index;
		}
		set{
			this._index = value;
		}
	}
	
	public int SecurityLevel
	{
		get{
			return _securityLevel;
		}
		set{
			this._securityLevel = value;
		}
	}
	
	public int PowerConsumption
	{
		get{
			return _powerConsumption;
		}
		set{
			this._powerConsumption = value;
		}
	}
	
	public GameManager.NodeType Type
	{
		get{
			return _type;
		}
		set{
			this._type = value;
		}
	}
	#endregion
	
	// Use this for initialization
	void Start () {
	}
			
	// Update is called once per frame
	void Update ()
	{
	}
	
	// Sets the connected state of the node
	public virtual void SetConnected( bool i_connected)
	{
	}
	
	// Sets the connected state of the node
	public virtual void SetClearance( )
	{
	}
	
	// Handles when a connected node is clicked on
	public virtual void HandleClickEvent()
	{
	}
	
	
}

#region Old Node Code
/*
public class Node : MonoBehaviour {
	
	
	#region members -7/12/2013 max
	private int _index;
	#endregion 
	
	#region Properties -7/12/2013 max
	public int Index
	{
		get
		{
			return _index;
		}
	}
	#endregion
	
	// Node TYPE Specific Variables
	private bool DOOR_defaultLocked;
	private bool DOOR_locked;
	private bool DOOR_closed;
	private float CAMERA_varianceAngle;
	private bool CAMERA_rotate;
	private float CAMERA_panSpeed;
	private float IR_radius;
	private float SECURITY_level;
	private string SECURITY_password;
	public bool IsSource = false;
			
	public GameManager.NodeState _state;
	public GameManager.NodeType _type;
	public GameManager.SecurityState _securityState;
	public int _SecurityLevel;
	public string MyPassword;
	public float IRRadius;
	
	#region prefabs and display
	
	public bool EnableLineShown = false;
	public GUIStyle Style;
	public GameObject ControllingObject;

	#endregion
	
	#region private and logic stuff
	
	private GameObject _IRWave;
		
	#endregion
		
	#region NODE SET UP METHODS
	public void SetUpDoorNode(DoorNodeData doorNode)
	{
		_index = doorNode.Index;
		_type = GameManager.NodeType.Door;
		this.transform.localScale = new Vector3(0.1f, 1f, 0.1f);
		State_ChangeToReleased();
		DOOR_defaultLocked = doorNode.Locked;
		DOOR_locked = doorNode.Locked;
		DOOR_closed = doorNode.Closed;
		
		
		// This is the example of setting up the scene objects to the node - Max
		
		//string whatDoIControll = doorNode.Object;
		//ControllingObject = GameObject.Find(whatDoIControll);
		//ControllingObject.GetComponent<DoorController>().isOpen = !doorNode.Closed;
		//ControllingObject.GetComponent<DoorController>().isLocked = !doorNode.Locked;
	}
	
	public void SetUpCameraNode(CameraNodeData cameraNode)
	{
		_index = cameraNode.Index;
		_type = GameManager.NodeType.SecurityCamera;
		this.transform.localScale = new Vector3(0.1f, 1f, 0.1f);
		State_ChangeToReleased();
		
		CAMERA_varianceAngle = cameraNode.Angle;
		CAMERA_rotate = cameraNode.Rotate;
		CAMERA_panSpeed = cameraNode.Speed;		
	}
	
	public void SetUpIRNode(IRNodeData IRNode)
	{
		_index = IRNode.Index;
		_type = GameManager.NodeType.InfraRed;
		this.transform.localScale = new Vector3(0.1f, 1f, 0.1f);
		State_ChangeToReleased();
		
		IR_radius = IRNode.Radius;
	}
	
	public void SetUpSecurityNode(SecurityNodeData securityNode)
	{
		_index = securityNode.Index;
		_type = GameManager.NodeType.Password;
		this.transform.localScale = new Vector3(0.1f, 1f, 0.1f);
		State_ChangeToReleased();
		
		SECURITY_level = securityNode.Level;
		SECURITY_password = "password";
	}
	#endregion
	
	#region Public Interface
	public void SetIndex(int i_index)
	{
		_index = i_index;
	}
	
	public void SetType(GameManager.NodeType i_type)
	{
		_type = i_type;
	}
	
	public void SetState(GameManager.NodeState i_state)
	{
		_state = i_state;
	}
	
	public void SetType(string i_type)
	{
		switch(i_type)
		{
		case "Door":
			_type = GameManager.NodeType.Door;
			break;
		case "IR":
			_type = GameManager.NodeType.InfraRed;
			break;
		case "Camera":
			_type = GameManager.NodeType.SecurityCamera;
			break;
		case "Password":
			_type = GameManager.NodeType.Password;
			break;
		//add more here...
		}
	}
	
	public bool IsControlled()
	{
		if(_state == GameManager.NodeState.Controlled)
			return true;
		else
			return false;
	}
	
	public void SetMaterial(Material i_mat)
	{
		renderer.material = i_mat;
	}
	
	public GameManager.NodeState GetState()
	{
		return _state;
	}
	
	public GameManager.NodeType GetNodeType()
	{
		return _type;
	}
	
	public int GetSecurityLevel()
	{
		return _SecurityLevel;
	}
		
	public void State_ChangeToControlled()
	{
		ChangeStateTo(GameManager.NodeState.Controlled);
		
		//sync the network
		if(_type == GameManager.NodeType.InfraRed)
		{
			Debug.Log ("Controlling IR Node: " + _index);
			NetworkManager.Manager.EnableIR(_index);
		}
	}
		
	public void State_ChangeToReleased()
	{
		ChangeStateTo(GameManager.NodeState.Neutral);
		
		//sync the network
		if(_type == GameManager.NodeType.Door)
		{
			//unlock the door on release
			//NetworkManager.Manager.LockDoor(_index);
		}
		else if(_type == GameManager.NodeType.InfraRed)
		{
			Debug.Log("Pass in Index for IR: " + _index);
			NetworkManager.Manager.DisableIR(_index);
		}
	}
	
	public void State_ChangeToEMPReleased()
	{
		ChangeStateTo(GameManager.NodeState.Unreachable);
		
		//sync the network
		if(_type == GameManager.NodeType.Door)
		{
			//unlock the door on release
			NetworkManager.Manager.LockDoor(_index);
		}
		else if(_type == GameManager.NodeType.InfraRed)
		{
			NetworkManager.Manager.DisableIR(_index);
		}		
	}
	
	private void ChangeStateTo(GameManager.NodeState i_state)
	{
		_state = i_state;
		UIManager.Manager.UpdateNodeMat(this);
	}
	#endregion
	
	#region IR related
	public void SetIRWave(GameObject i_wave)
	{
		_IRWave = i_wave;
	}
	
	public void DeleteIRWave()
	{
		Destroy(_IRWave);
	}
	#endregion

		
	void OnMouseDown()
	{
		if(_state == GameManager.NodeState.Controlled)
		{
			HandleClickEvent();
		}
		
	}
	
	void HandleClickEvent()
	{
		switch(_type)
		{
		case GameManager.NodeType.Door:
			HandleClickEventForDoor();
			break;
		case GameManager.NodeType.InfraRed:
			HandleClickEventForIR();
			break;
		case GameManager.NodeType.Password:
			HandleClickEventForPassword();
			break;
		case GameManager.NodeType.SecurityCamera:
			HandleClickEventForCamera();
			break;
		case GameManager.NodeType.Bug:
			HandleClickEventForBug();
			break;
		}
	}
	
	void HandleClickEventForDoor()
	{
		//only response if the door is not opened
		if(!ControllingObject.GetComponent<DoorController>().isOpen)
		{
			//if it's unlocked, lock it; else unlock it
			if(ControllingObject.GetComponent<DoorController>().isLocked == false)
			{
				NetworkManager.Manager.LockDoor(_index);
			}
			else
			{
				NetworkManager.Manager.UnlockDoor(_index);
			}
		}
	}
	
	void HandleClickEventForIR()
	{
		//placeholder functionality, enable and disable IR when you click
		if(IRSystem.ISystem.IsThisIRActive(this))
		{
			NetworkManager.Manager.DisableIR(_index);
		}
		else
		{
			NetworkManager.Manager.EnableIR(_index);
		}
		
	}
	
	void HandleClickEventForPassword()
	{
		GameObject g_password = GameObject.Find("Password");
		Password this_password = g_password.GetComponent<Password>();
		this_password.GetPassword(this);
	}
	
	void HandleClickEventForCamera()
	{
		//if it is viewing, stop view
		if(_state >= GameManager.NodeState.Viewing)
		{
			ControllingObject.camera.enabled = false;
			_state -= (int)GameManager.NodeState.Viewing;
			
			// Adjust Camera Back.
			GameObject TopDownCamera = GameObject.Find("TopDownCamera");
			Vector3 newPosition = new Vector3(TopDownCamera.transform.position.x, TopDownCamera.transform.position.y, TopDownCamera.transform.position.z-8);
			TopDownCamera.transform.position = newPosition;
			TopDownCamera.camera.orthographicSize -= 7;
		}
		else // if it is not viewing, start view
		{
			ControllingObject.camera.enabled = true;
			ControllingObject.camera.rect = new Rect(0.3f, 0.0f, 0.4f, 0.3f);
			_state += (int)GameManager.NodeState.Viewing;
		
			// Adjust Map View
			GameObject TopDownCamera = GameObject.Find("TopDownCamera");
			Vector3 newPosition = new Vector3(TopDownCamera.transform.position.x, TopDownCamera.transform.position.y, TopDownCamera.transform.position.z+8);
			TopDownCamera.transform.position = newPosition;
			TopDownCamera.camera.orthographicSize += 7;
		}
	}
	
	void HandleClickEventForBug()
	{
		//should be the same as camera
		//if it is viewing, stop view
		if(_state >= GameManager.NodeState.Viewing)
		{
			ControllingObject.camera.enabled = false;
			_state -= (int)GameManager.NodeState.Viewing;
			
			// Adjust Camera Back.
			GameObject TopDownCamera = GameObject.Find("TopDownCamera");
			Vector3 newPosition = new Vector3(TopDownCamera.transform.position.x, TopDownCamera.transform.position.y, TopDownCamera.transform.position.z-8);
			TopDownCamera.transform.position = newPosition;
			TopDownCamera.camera.orthographicSize -= 7;
		}
		else // if it is not viewing, start view
		{
			ControllingObject.camera.enabled = true;
			ControllingObject.camera.rect = new Rect(0.3f, 0.0f, 0.4f, 0.3f);
			_state += (int)GameManager.NodeState.Viewing;
		
			// Adjust Map View
			GameObject TopDownCamera = GameObject.Find("TopDownCamera");
			Vector3 newPosition = new Vector3(TopDownCamera.transform.position.x, TopDownCamera.transform.position.y, TopDownCamera.transform.position.z+8);
			TopDownCamera.transform.position = newPosition;
			TopDownCamera.camera.orthographicSize += 7;
		}
	}
	
	// Use this for initialization
	void Start () {
		
		// If Security Clearance has not been set, set it to 1
		if ( _SecurityLevel == null || _SecurityLevel == 0)
			_SecurityLevel = 1;

		
		// Set all of the initial material textures.
		//DestroyImmediate(renderer.material);

		//UIManager.Manager.UpdateNodeMat(this);
	}
			
	// Update is called once per frame
	void Update ()
	{
	}
	

}*/
#endregion