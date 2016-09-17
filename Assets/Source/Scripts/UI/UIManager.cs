using UnityEngine;
using System.Collections;

public class UIManager : MonoBehaviour {
	
	#region Singleton Declaration
	
	private static UIManager m_instance;
	
	public static UIManager Manager
	{
		get
		{
			if(m_instance == null)
			{
				m_instance = new UIManager();			
			}
			return m_instance;
		}
	}
	
	public UIManager () 
    { 
        m_instance = this;
    }
	#endregion
	
	#region ShockWave Prefabs
	
	public GameObject HackerPingCirclePrefab;
	public GameObject EMPShockWavePrefab;
	public GameObject IRShockWavePrefab;
	
	#endregion
	
	#region Button Prefabs
	
	public GameObject CaptureButtonPrefab;
	public GameObject ReleaseButtonPrefab;
	public GameObject EncryptButtonPrefab;
	public GameObject LockButtonPrefab;
	public GameObject UnlockButtonPrefab;
	public GameObject ViewButtonPrefab;
	public GameObject StopViewButtonPrefab;
	public GameObject CancleButtonPrefab;
	public GameObject ViewTopButtonPrefab;
	public GameObject ViewMidButtonPrefab;
	public GameObject ViewBotButtonPrefab;
	
	#endregion
	
	#region Materials
	
	public Material ControlNodeUnclickableMaterial;
	public Material ControlNodeNeutralMaterial;
	public Material ControlNodeCapturedMaterial;
	public Material ControlNodeEncryptedMaterial;
	public Material ControlNodeAboveClearanceMaterial;
	
	public Material CameraUnclickableMaterial;
	public Material CameraNeutralMaterial;
	public Material CameraCapturedMaterial;
	public Material CameraEncryptedMaterial;
	public Material CameraAboveClearanceMaterial;
	
	public Material DoorUnclickableMaterial;
	public Material DoorNeutralMaterial;
	public Material DoorCapturedLockedMaterial;
	public Material DoorCapturedUnlockedMaterial;
	public Material DoorEncryptedLockedMaterial;
	public Material DoorEncryptedUnlockedMaterial;
	public Material DoorAboveClearanceMaterial;
	
	public Material IRUnclickableMaterial;
	public Material IRNeutralMaterial;
	public Material IRCapturedMaterial;
	public Material IREncryptedMaterial;
	public Material IRAboveClearanceMaterial;
	
	public Material PasswordUnclickableMaterial;
	public Material PasswordNeutralMaterial;
	
	public Material PoweredUnclickableOffMaterial;
	public Material PoweredUnclickableOnMaterial;
	public Material PoweredNeutralOffMaterial;
	public Material PoweredNeutralOnMaterial;
	public Material PoweredCapturedMaterial;
	public Material PoweredEncryptedMaterial;
	public Material PoweredAboveClearanceMaterial;
	
	#endregion
		
	#region Private Field
	
	private GameObject _captureButton;
	private GameObject _releaseButton;
	private GameObject _lockButton;
	private GameObject _encryptButton;
	private GameObject _unlockButton;
	private GameObject _viewButton;
	private GameObject _stopViewButton;
	private GameObject _cancleButton;
	private GameObject _viewTopButton;
	private GameObject _viewMidButton;
	private GameObject _viewBotButton;
	
	#endregion
	
	#region Constants	
	//private Vector3 UP_LEFT = new Vector3(10, 1, -6);
	//private Vector3 UP_RIGHT = new Vector3(-2.5f, 1, -2);
	//private Vector3 MID_LEFT = new Vector3(2.5f, 1, 0);
	//private Vector3 MID_RIGHT = new Vector3(-2.5f, 1, 0);
	//private Vector3 UP_LEFT = new Vector3(2.5f, 1, -2);
	//private Vector3 DOWN_LEFT = new Vector3(2.5f, 1, 2);
	//private Vector3 DOWN_RIGHT = new Vector3(-2.5f, 1, 2);
	
	//private Vector3 VIEW_TOP = new Vector3(16, 1, -6);
	//private Vector3 VIEW_MID = new Vector3(16, 1, 0);
	//private Vector3 VIEW_BOT = new Vector3(16, 1, 6);
	#endregion
	
	private Node passwordNode;
	
	#region Generic Node State Material Changer, more expensive
	public void UpdateNodeMat(Node i_node)
	{
		/*
		switch(i_node._type)
		{
		case GameManager.NodeType.Door:
			UpdateDoorNodeMat(i_node);
			break;
		case GameManager.NodeType.Control:
			UpdateControlNodeMat(i_node);
			break;
		case GameManager.NodeType.SecurityCamera:
			UpdateSecCameraNodeMat(i_node);
			break;
		case GameManager.NodeType.InfraRed:
			UpdateIRNodeMat(i_node);
			break;
		case GameManager.NodeType.Password:
			UpdatePasswordNodeMat(i_node);
			break;
		case GameManager.NodeType.Powered:
			UpdatePoweredNodeMat(i_node);
			break;
		}
		*/
	}
	
	void UpdateDoorNodeMat(Node i_node)
	{
		/*
		// First check if node if node is above your security clearance
		if ( i_node.GetSecurityLevel() > GameManager.Manager.PlayerInventory.SecurityClearance ) 
		{
			i_node.SetMaterial(DoorAboveClearanceMaterial);
		}
		else
		{
			if(i_node.GetState() == GameManager.NodeState.Controlled)
			{
				if((i_node.ControllingObject.GetComponent<DoorController>().isLocked)||
				(i_node.ControllingObject.GetComponent<DoorController>().isDeadlocked))// door is locked
				{
					i_node.SetMaterial(DoorCapturedLockedMaterial);
				}
				else
				{
					i_node.SetMaterial(DoorCapturedUnlockedMaterial);
				}
			}
			else if(i_node.GetState() == GameManager.NodeState.Neutral)
			{
				i_node.SetMaterial(DoorNeutralMaterial);
			}
			else if(i_node.GetState() == GameManager.NodeState.Unreachable)
			{
				i_node.SetMaterial(DoorUnclickableMaterial);
			}
			/*
			else if(i_node.GetState() == GameManager.NodeState.Deactivated)
			{
				//make it unclickable mat for now
				i_node.SetMaterial(DoorUnclickableMaterial);
			}
			
		}*/
	}
	
	void UpdateControlNodeMat(Node i_node)
	{
		/*
		// First check if node if node is above your security clearance
		if ( i_node.GetSecurityLevel() > GameManager.Manager.PlayerInventory.SecurityClearance ) 
		{
			i_node.SetMaterial(ControlNodeAboveClearanceMaterial);
		}
		else
		{
			if(i_node.GetState() == GameManager.NodeState.Controlled)
			{
				i_node.SetMaterial(ControlNodeCapturedMaterial);
			}
			else if(i_node.GetState() == GameManager.NodeState.Neutral)
			{
				i_node.SetMaterial(ControlNodeNeutralMaterial);
			}
			else if(i_node.GetState() == GameManager.NodeState.Unreachable)
			{
				i_node.SetMaterial(ControlNodeUnclickableMaterial);
			}
		}*/
	}
	
	void UpdateSecCameraNodeMat(Node i_node)
	{
		/*
		// First check if node if node is above your security clearance
		if ( i_node.GetSecurityLevel() > GameManager.Manager.PlayerInventory.SecurityClearance ) 
		{
			i_node.SetMaterial(CameraAboveClearanceMaterial);
		}
		else
		{
			if(i_node.GetState() == GameManager.NodeState.Controlled)
			{
				i_node.SetMaterial(CameraCapturedMaterial);
			}
			else if(i_node.GetState() == GameManager.NodeState.Neutral)
			{
				i_node.SetMaterial(CameraNeutralMaterial);
			}
			else if(i_node.GetState() == GameManager.NodeState.Unreachable)
			{
				i_node.SetMaterial(CameraUnclickableMaterial);
			}
		}*/
	}
	
	void UpdateIRNodeMat(Node i_node)
	{
		/*
		// First check if node if node is above your security clearance
		if ( i_node.GetSecurityLevel() > GameManager.Manager.PlayerInventory.SecurityClearance ) 
		{
			i_node.SetMaterial(IRAboveClearanceMaterial);
		}
		else
		{
			if(i_node.GetState() == GameManager.NodeState.Controlled)
			{
				i_node.SetMaterial(IRCapturedMaterial);
			}
			else if(i_node.GetState() == GameManager.NodeState.Neutral)
			{
				i_node.SetMaterial(IRNeutralMaterial);
			}
			else if(i_node.GetState() == GameManager.NodeState.Unreachable)
			{
				i_node.SetMaterial(IRUnclickableMaterial);
			}
		}	*/	
	}
	
	void UpdatePasswordNodeMat(Node i_node)
	{
		/*
		// First check if node if node is above your security clearance
		if ( i_node.GetSecurityLevel() > GameManager.Manager.PlayerInventory.SecurityClearance ) 
		{
			i_node.SetMaterial(ControlNodeAboveClearanceMaterial);
		}
		else
		{
			if(i_node.GetState() == GameManager.NodeState.Controlled)
			{
				i_node.SetMaterial(ControlNodeCapturedMaterial);
			}
			else if(i_node.GetState() == GameManager.NodeState.Neutral)
			{
				i_node.SetMaterial(PasswordNeutralMaterial);
			}
			else if(i_node.GetState() == GameManager.NodeState.Unreachable)
			{
				i_node.SetMaterial(PasswordUnclickableMaterial);
			}
		}*/		
	}
	
	// Change it to placeholder for now, since we don't use this one anymore.
	void UpdatePoweredNodeMat(Node i_node)
	{

	}
	#endregion
	
	#region Door Related	
	//change the node mat to unlock based on door's state
	public void UpdateDoorMatToUnLock(Node i_nodeForDoor)
	{
		/*
		if(i_nodeForDoor.GetState() == GameManager.NodeState.Controlled)
		{
			i_nodeForDoor.SetMaterial(DoorCapturedUnlockedMaterial);
		}*/
	}
	
	//change the node mat to lock based on door's state
	public void UpdateDoorMatToLock(Node i_nodeForDoor)
	{
		/*
		if(i_nodeForDoor.GetState() == GameManager.NodeState.Controlled)
		{
			i_nodeForDoor.SetMaterial(DoorCapturedLockedMaterial);
		}*/
	}
	
	public void UpdateDoorMatToUnreachable(Node i_nodeForDoor)
	{
		//i_nodeForDoor.SetMaterial(DoorUnclickableMaterial);
	}
	
	public void UpdateDoorMatToNeutral(Node i_nodeForDoor)
	{
		//i_nodeForDoor.SetMaterial(DoorNeutralMaterial);
	}
	
	public void UpdateDoorMatToDeactivated(Node i_nodeForDoor)
	{
		//i_nodeForDoor.SetMaterial(DoorUnclickableMaterial);
	}
	
	//change the node mat to captured based on door's lock status
	public void UpdateDoorMatToCaptured(Node i_nodeForDoor)
	{
		/*
		if(i_nodeForDoor.ControllingObject.GetComponent<DoorController>().isLocked == false)
		{
			i_nodeForDoor.SetMaterial(DoorCapturedUnlockedMaterial);
		}
		else
		{
			i_nodeForDoor.SetMaterial(DoorCapturedLockedMaterial);
		}*/
	}
	
	//change the node mat to encrypted based on door's lock status
	public void UpdateDoorMatToEncrypted(Node i_nodeForDoor)
	{
		/*
		if(i_nodeForDoor.ControllingObject.GetComponent<DoorController>().isLocked == false)
		{
			i_nodeForDoor.SetMaterial(DoorEncryptedUnlockedMaterial);
		}
		else
		{
			i_nodeForDoor.SetMaterial(DoorEncryptedLockedMaterial);
		}	*/	
	}
	
	#endregion
	
	#region Camera Related

	public void UpdateCameraMatToUnreachable(Node i_nodeForCamera)
	{
		//i_nodeForCamera.SetMaterial(CameraUnclickableMaterial);
	}
	
	public void UpdateCameraMatToNeutral(Node i_nodeForCamera)
	{
		//i_nodeForCamera.SetMaterial(CameraNeutralMaterial);
	}
	
	public void UpdateCameraMatToCaptured(Node i_nodeForCamera)
	{
		//i_nodeForCamera.SetMaterial(CameraCapturedMaterial);
	}
	
	public void UpdateCameraMatToEncrytped(Node i_nodeForCamera)
	{
		//i_nodeForCamera.SetMaterial(CameraEncryptedMaterial);
	}
	
	public void UpdateCameraMatToDeactivated(Node i_nodeForCamera)
	{
		//i_nodeForCamera.SetMaterial(CameraUnclickableMaterial);
	}
	#endregion

	#region Control Node Related
	/*
	public void UpdateControlNodeToUnreachable(Node i_controlNode)
	{
		i_controlNode.SetMaterial(ControlNodeUnclickableMaterial);
	}
	
	public void UpdateControlNodeToDeactivated(Node i_controlNode)
	{
		i_controlNode.SetMaterial(ControlNodeUnclickableMaterial);
	}
	
	public void UpdateControlNodeToNeutral(Node i_controlNode)
	{
		i_controlNode.SetMaterial(ControlNodeNeutralMaterial);
	}
	
	public void UpdateControlNodeToCaptured(Node i_controlNode)
	{
		i_controlNode.SetMaterial(ControlNodeCapturedMaterial);
	}
	
	public void UpdateControlNodeToEncrypted(Node i_controlNode)
	{
		i_controlNode.SetMaterial(ControlNodeEncryptedMaterial);
	}
	*/
	#endregion
	
	#region IR Node Related
	
	public void UpdateIRNodeToUnreachable(Node i_IRNode)
	{
		//i_IRNode.SetMaterial(IRUnclickableMaterial);
	}
	
	public void UpdateIRNodeToDeactivated(Node i_IRNode)
	{
		//i_IRNode.SetMaterial(IRUnclickableMaterial);
	}
	
	public void UpdateIRNodeToNeutral(Node i_IRNode)
	{
		//i_IRNode.SetMaterial(IRNeutralMaterial);
	}
	
	public void UpdateIRNodeToCaptured(Node i_IRNode)
	{
		//i_IRNode.SetMaterial(IRCapturedMaterial);
	}
	
	public void UpdateIRNodeToEncrypted(Node i_IRNode)
	{
		//i_IRNode.SetMaterial(IREncryptedMaterial);
	}
	
	#endregion
	
	#region Buttons Related, no use anymore, to be deleted - 8/5/2013
	/*
	public void CreateButtonsForNode(Node i_node)
	{
		Debug.Log("CREATE BUTTONS REQUESTED" + i_node._type);
			switch(i_node._type)
			{
			case GameManager.NodeType.Control:
				CreateControlNodeMenu(i_node);
				break;
			case GameManager.NodeType.Door:
				CreateDoorMenu(i_node);
				break;
			case GameManager.NodeType.SecurityCamera:
				CreateSecurityCameraMenu(i_node);
				break;
			case GameManager.NodeType.InfraRed:
				CreateIRMenu(i_node);
				break;
			case GameManager.NodeType.Bug:
				CreateCameraBugMenu(i_node);
				break;
			case GameManager.NodeType.Powered:
				CreatePowerNodeMenu(i_node);
				break;
			case GameManager.NodeType.Password:
				CreatePasswordNodeMenu(i_node);
				break;
			}
	}	
	

	// This is not neccesary as long as we only have one camera view at a time.
	public void CreateViewButtons(Node i_node)
	{
		_viewTopButton= (GameObject)Instantiate(ViewTopButtonPrefab, i_node.gameObject.transform.position + VIEW_TOP, Quaternion.identity);
		_viewTopButton.GetComponent<ButtonController>().SetLinkedNode(i_node);
		_viewTopButton.GetComponent<ButtonController>().SetButtonType(GameManager.ButtonType.TopView);
		_viewMidButton= (GameObject)Instantiate(ViewMidButtonPrefab, i_node.gameObject.transform.position + VIEW_MID, Quaternion.identity);
		_viewMidButton.GetComponent<ButtonController>().SetLinkedNode(i_node);
		_viewMidButton.GetComponent<ButtonController>().SetButtonType(GameManager.ButtonType.MidView);
		_viewBotButton= (GameObject)Instantiate(ViewBotButtonPrefab, i_node.gameObject.transform.position + VIEW_BOT, Quaternion.identity);
		_viewBotButton.GetComponent<ButtonController>().SetLinkedNode(i_node);
		_viewBotButton.GetComponent<ButtonController>().SetButtonType(GameManager.ButtonType.BotView);
	}
	
	public void ClearButtons()
	{ 
		GameObject[] buttons = GameObject.FindGameObjectsWithTag("Button");
		foreach(GameObject obj in buttons)
		{
			Destroy(obj);
		}
		
		// Clear Password Menu
		GameObject g_password = GameObject.Find("Password");
		Password this_password = g_password.GetComponent<Password>();
		this_password.clearPassword();
	}
	
	#region private section. This is for button stuff, no use anymore

	void CreateDoorMenu(Node i_node)
	{
		if(i_node.ControllingObject.GetComponent<DoorController>().isLocked && 
			(i_node.ControllingObject.GetComponent<DoorController>().isJammed == false))
			//||(i_node.ControllingObject.GetComponent<DoorController>().isJammed))// if door is locked
		{
			switch(i_node._state)
			{
			case GameManager.NodeState.Controlled:
				_unlockButton= (GameObject)Instantiate(UnlockButtonPrefab, i_node.gameObject.transform.position + UP_LEFT, Quaternion.identity);
				_unlockButton.GetComponent<ButtonController>().SetLinkedNode(i_node);
				_unlockButton.GetComponent<ButtonController>().SetButtonType(GameManager.ButtonType.Unlock);
				if(i_node.ControllingObject.GetComponent<DoorController>().isOpen)
					_unlockButton.renderer.material.color = Color.gray;
				_lockButton= (GameObject)Instantiate(LockButtonPrefab, i_node.gameObject.transform.position + DOWN_LEFT, Quaternion.identity);
				_lockButton.GetComponent<ButtonController>().SetLinkedNode(i_node);
				_lockButton.GetComponent<ButtonController>().SetButtonType(GameManager.ButtonType.Jam);
				if(i_node.ControllingObject.GetComponent<DoorController>().isOpen)
					_lockButton.renderer.material.color = Color.gray;
				_encryptButton= (GameObject)Instantiate(EncryptButtonPrefab, i_node.gameObject.transform.position + UP_RIGHT, Quaternion.identity);
				_encryptButton.GetComponent<ButtonController>().SetLinkedNode(i_node);
				_encryptButton.GetComponent<ButtonController>().SetButtonType(GameManager.ButtonType.Encrypt);
				_releaseButton = (GameObject)Instantiate(ReleaseButtonPrefab, i_node.gameObject.transform.position + DOWN_RIGHT, Quaternion.identity);
				_releaseButton.GetComponent<ButtonController>().SetLinkedNode(i_node);
				_releaseButton.GetComponent<ButtonController>().SetButtonType(GameManager.ButtonType.Release);
				break;
			case GameManager.NodeState.Encrypted:
				_unlockButton= (GameObject)Instantiate(UnlockButtonPrefab, i_node.gameObject.transform.position + UP_LEFT, Quaternion.identity);
				_unlockButton.GetComponent<ButtonController>().SetLinkedNode(i_node);
				_unlockButton.GetComponent<ButtonController>().SetButtonType(GameManager.ButtonType.Unlock);
				if(i_node.ControllingObject.GetComponent<DoorController>().isOpen)
					_unlockButton.renderer.material.color = Color.gray;				
				_lockButton= (GameObject)Instantiate(LockButtonPrefab, i_node.gameObject.transform.position + DOWN_LEFT, Quaternion.identity);
				_lockButton.GetComponent<ButtonController>().SetLinkedNode(i_node);
				_lockButton.GetComponent<ButtonController>().SetButtonType(GameManager.ButtonType.Jam);
				if(i_node.ControllingObject.GetComponent<DoorController>().isOpen)
					_lockButton.renderer.material.color = Color.gray;
				_releaseButton = (GameObject)Instantiate(ReleaseButtonPrefab, i_node.gameObject.transform.position + MID_RIGHT, Quaternion.identity);
				_releaseButton.GetComponent<ButtonController>().SetLinkedNode(i_node);
				_releaseButton.GetComponent<ButtonController>().SetButtonType(GameManager.ButtonType.Release);
				break;
			case GameManager.NodeState.Capturing:			
			case GameManager.NodeState.Releasing:
			case GameManager.NodeState.Encrypting:
				_cancleButton = (GameObject)Instantiate(ReleaseButtonPrefab, i_node.gameObject.transform.position + MID_RIGHT, Quaternion.identity);
				_cancleButton.GetComponent<ButtonController>().SetLinkedNode(i_node);
				_cancleButton.GetComponent<ButtonController>().SetButtonType(GameManager.ButtonType.Cancle);
				break;
			case GameManager.NodeState.Neutral:
				_captureButton = (GameObject)Instantiate(CaptureButtonPrefab, i_node.gameObject.transform.position + MID_LEFT, Quaternion.identity);
				_captureButton.GetComponent<ButtonController>().SetLinkedNode(i_node);
				_captureButton.GetComponent<ButtonController>().SetButtonType(GameManager.ButtonType.Capture);
				break;					
			}
		}
		else
		{
			if(i_node.ControllingObject.GetComponent<DoorController>().isJammed)
			{
				switch(i_node._state)
				{
				case GameManager.NodeState.Controlled:
					_unlockButton= (GameObject)Instantiate(UnlockButtonPrefab, i_node.gameObject.transform.position + MID_LEFT, Quaternion.identity);
					_unlockButton.GetComponent<ButtonController>().SetLinkedNode(i_node);
					_unlockButton.GetComponent<ButtonController>().SetButtonType(GameManager.ButtonType.Unlock);
					if(i_node.ControllingObject.GetComponent<DoorController>().isOpen)
						_unlockButton.renderer.material.color = Color.gray;
					_encryptButton= (GameObject)Instantiate(EncryptButtonPrefab, i_node.gameObject.transform.position + UP_RIGHT, Quaternion.identity);
					_encryptButton.GetComponent<ButtonController>().SetLinkedNode(i_node);
					_encryptButton.GetComponent<ButtonController>().SetButtonType(GameManager.ButtonType.Encrypt);
					_releaseButton = (GameObject)Instantiate(ReleaseButtonPrefab, i_node.gameObject.transform.position + DOWN_RIGHT, Quaternion.identity);
					_releaseButton.GetComponent<ButtonController>().SetLinkedNode(i_node);
					_releaseButton.GetComponent<ButtonController>().SetButtonType(GameManager.ButtonType.Release);
					break;
				case GameManager.NodeState.Encrypted:
					_unlockButton= (GameObject)Instantiate(UnlockButtonPrefab, i_node.gameObject.transform.position + MID_LEFT, Quaternion.identity);
					_unlockButton.GetComponent<ButtonController>().SetLinkedNode(i_node);
					_unlockButton.GetComponent<ButtonController>().SetButtonType(GameManager.ButtonType.Unlock);
					if(i_node.ControllingObject.GetComponent<DoorController>().isOpen)
						_unlockButton.renderer.material.color = Color.gray;
					_releaseButton = (GameObject)Instantiate(ReleaseButtonPrefab, i_node.gameObject.transform.position + MID_RIGHT, Quaternion.identity);
					_releaseButton.GetComponent<ButtonController>().SetLinkedNode(i_node);
					_releaseButton.GetComponent<ButtonController>().SetButtonType(GameManager.ButtonType.Release);
					break;
				case GameManager.NodeState.Capturing:			
				case GameManager.NodeState.Releasing:
				case GameManager.NodeState.Encrypting:
					_cancleButton = (GameObject)Instantiate(ReleaseButtonPrefab, i_node.gameObject.transform.position + MID_RIGHT, Quaternion.identity);
					_cancleButton.GetComponent<ButtonController>().SetLinkedNode(i_node);
					_cancleButton.GetComponent<ButtonController>().SetButtonType(GameManager.ButtonType.Cancle);
					break;
				case GameManager.NodeState.Neutral:
					_captureButton = (GameObject)Instantiate(CaptureButtonPrefab, i_node.gameObject.transform.position + MID_LEFT, Quaternion.identity);
					_captureButton.GetComponent<ButtonController>().SetLinkedNode(i_node);
					_captureButton.GetComponent<ButtonController>().SetButtonType(GameManager.ButtonType.Capture);
					break;					
				}			
			}
			else
			{
				switch(i_node._state)
				{
				case GameManager.NodeState.Controlled:
					_lockButton= (GameObject)Instantiate(LockButtonPrefab, i_node.gameObject.transform.position + MID_LEFT, Quaternion.identity);
					_lockButton.GetComponent<ButtonController>().SetLinkedNode(i_node);
					_lockButton.GetComponent<ButtonController>().SetButtonType(GameManager.ButtonType.Jam);
					if(i_node.ControllingObject.GetComponent<DoorController>().isOpen)
						_lockButton.renderer.material.color = Color.gray;
					_encryptButton= (GameObject)Instantiate(EncryptButtonPrefab, i_node.gameObject.transform.position + UP_RIGHT, Quaternion.identity);
					_encryptButton.GetComponent<ButtonController>().SetLinkedNode(i_node);
					_encryptButton.GetComponent<ButtonController>().SetButtonType(GameManager.ButtonType.Encrypt);
					_releaseButton = (GameObject)Instantiate(ReleaseButtonPrefab, i_node.gameObject.transform.position + DOWN_RIGHT, Quaternion.identity);
					_releaseButton.GetComponent<ButtonController>().SetLinkedNode(i_node);
					_releaseButton.GetComponent<ButtonController>().SetButtonType(GameManager.ButtonType.Release);
					break;
				case GameManager.NodeState.Encrypted:
					_lockButton= (GameObject)Instantiate(LockButtonPrefab, i_node.gameObject.transform.position + MID_LEFT, Quaternion.identity);
					_lockButton.GetComponent<ButtonController>().SetLinkedNode(i_node);
					_lockButton.GetComponent<ButtonController>().SetButtonType(GameManager.ButtonType.Jam);		
					if(i_node.ControllingObject.GetComponent<DoorController>().isOpen)
						_lockButton.renderer.material.color = Color.gray;
					_releaseButton = (GameObject)Instantiate(ReleaseButtonPrefab, i_node.gameObject.transform.position + MID_RIGHT, Quaternion.identity);
					_releaseButton.GetComponent<ButtonController>().SetLinkedNode(i_node);
					_releaseButton.GetComponent<ButtonController>().SetButtonType(GameManager.ButtonType.Release);
					break;
				case GameManager.NodeState.Capturing:			
				case GameManager.NodeState.Releasing:
				case GameManager.NodeState.Encrypting:
					_cancleButton = (GameObject)Instantiate(ReleaseButtonPrefab, i_node.gameObject.transform.position + MID_RIGHT, Quaternion.identity);
					_cancleButton.GetComponent<ButtonController>().SetLinkedNode(i_node);
					_cancleButton.GetComponent<ButtonController>().SetButtonType(GameManager.ButtonType.Cancle);
					break;
				case GameManager.NodeState.Neutral:
					_captureButton = (GameObject)Instantiate(CaptureButtonPrefab, i_node.gameObject.transform.position + MID_LEFT, Quaternion.identity);
					_captureButton.GetComponent<ButtonController>().SetLinkedNode(i_node);
					_captureButton.GetComponent<ButtonController>().SetButtonType(GameManager.ButtonType.Capture);
					break;					
				}				
			}
		}
	}
	
	void CreateControlNodeMenu(Node i_node)
	{
		switch(i_node._state)
		{
		case GameManager.NodeState.Controlled:
			_encryptButton= (GameObject)Instantiate(EncryptButtonPrefab, i_node.gameObject.transform.position + MID_LEFT, Quaternion.identity);
			_encryptButton.GetComponent<ButtonController>().SetLinkedNode(i_node);
			_encryptButton.GetComponent<ButtonController>().SetButtonType(GameManager.ButtonType.Encrypt);
			_releaseButton = (GameObject)Instantiate(ReleaseButtonPrefab, i_node.gameObject.transform.position + MID_RIGHT, Quaternion.identity);
			_releaseButton.GetComponent<ButtonController>().SetLinkedNode(i_node);
			_releaseButton.GetComponent<ButtonController>().SetButtonType(GameManager.ButtonType.Release);
			break;
		case GameManager.NodeState.Secured:
		case GameManager.NodeState.Encrypted:
			_releaseButton = (GameObject)Instantiate(ReleaseButtonPrefab, i_node.gameObject.transform.position + MID_LEFT, Quaternion.identity);
			_releaseButton.GetComponent<ButtonController>().SetLinkedNode(i_node);
			_releaseButton.GetComponent<ButtonController>().SetButtonType(GameManager.ButtonType.Release);
			break;
		case GameManager.NodeState.Capturing:			
		case GameManager.NodeState.Releasing:
		case GameManager.NodeState.Encrypting:
			_cancleButton = (GameObject)Instantiate(ReleaseButtonPrefab, i_node.gameObject.transform.position + MID_RIGHT, Quaternion.identity);
			_cancleButton.GetComponent<ButtonController>().SetLinkedNode(i_node);
			_cancleButton.GetComponent<ButtonController>().SetButtonType(GameManager.ButtonType.Cancle);
			break;
		case GameManager.NodeState.Neutral:
			_captureButton = (GameObject)Instantiate(CaptureButtonPrefab, i_node.gameObject.transform.position + MID_LEFT, Quaternion.identity);
			_captureButton.GetComponent<ButtonController>().SetLinkedNode(i_node);
			_captureButton.GetComponent<ButtonController>().SetButtonType(GameManager.ButtonType.Capture);
			break;
		}		
	}
	
	void CreateIRMenu(Node i_node)
	{
		switch(i_node._state)
		{
		case GameManager.NodeState.Controlled:
			_encryptButton= (GameObject)Instantiate(EncryptButtonPrefab, i_node.gameObject.transform.position + MID_LEFT, Quaternion.identity);
			_encryptButton.GetComponent<ButtonController>().SetLinkedNode(i_node);
			_encryptButton.GetComponent<ButtonController>().SetButtonType(GameManager.ButtonType.Encrypt);
			_releaseButton = (GameObject)Instantiate(ReleaseButtonPrefab, i_node.gameObject.transform.position + MID_RIGHT, Quaternion.identity);
			_releaseButton.GetComponent<ButtonController>().SetLinkedNode(i_node);
			_releaseButton.GetComponent<ButtonController>().SetButtonType(GameManager.ButtonType.Release);
			break;
		case GameManager.NodeState.Secured:
		case GameManager.NodeState.Encrypted:
			_releaseButton = (GameObject)Instantiate(ReleaseButtonPrefab, i_node.gameObject.transform.position + MID_LEFT, Quaternion.identity);
			_releaseButton.GetComponent<ButtonController>().SetLinkedNode(i_node);
			_releaseButton.GetComponent<ButtonController>().SetButtonType(GameManager.ButtonType.Release);
			break;
		case GameManager.NodeState.Capturing:			
		case GameManager.NodeState.Releasing:
		case GameManager.NodeState.Encrypting:
			_cancleButton = (GameObject)Instantiate(ReleaseButtonPrefab, i_node.gameObject.transform.position + MID_RIGHT, Quaternion.identity);
			_cancleButton.GetComponent<ButtonController>().SetLinkedNode(i_node);
			_cancleButton.GetComponent<ButtonController>().SetButtonType(GameManager.ButtonType.Cancle);
			break;
		case GameManager.NodeState.Neutral:
			_captureButton = (GameObject)Instantiate(CaptureButtonPrefab, i_node.gameObject.transform.position + MID_LEFT, Quaternion.identity);
			_captureButton.GetComponent<ButtonController>().SetLinkedNode(i_node);
			_captureButton.GetComponent<ButtonController>().SetButtonType(GameManager.ButtonType.Capture);
			break;
		}		
	}
	
	void CreateSecurityCameraMenu(Node i_node)
	{
		if(i_node._state >= GameManager.NodeState.Viewing) // this means viewing, YOU CAN ONLY STOP VIEW IF YOU ARE VIEWING!
		{
			_stopViewButton= (GameObject)Instantiate(StopViewButtonPrefab, i_node.gameObject.transform.position + MID_LEFT, Quaternion.identity);
			_stopViewButton.GetComponent<ButtonController>().SetLinkedNode(i_node);
			_stopViewButton.GetComponent<ButtonController>().SetButtonType(GameManager.ButtonType.Stopview);
		}
		else // this means not viewing
		{	
			switch(i_node._state)
			{				
			case GameManager.NodeState.Controlled:
				_viewButton= (GameObject)Instantiate(ViewButtonPrefab, i_node.gameObject.transform.position + MID_LEFT, Quaternion.identity);
				_viewButton.GetComponent<ButtonController>().SetLinkedNode(i_node);
				_viewButton.GetComponent<ButtonController>().SetButtonType(GameManager.ButtonType.View);
				_encryptButton= (GameObject)Instantiate(EncryptButtonPrefab, i_node.gameObject.transform.position + UP_RIGHT, Quaternion.identity);
				_encryptButton.GetComponent<ButtonController>().SetLinkedNode(i_node);
				_encryptButton.GetComponent<ButtonController>().SetButtonType(GameManager.ButtonType.Encrypt);
				_releaseButton = (GameObject)Instantiate(ReleaseButtonPrefab, i_node.gameObject.transform.position + DOWN_RIGHT, Quaternion.identity);
				_releaseButton.GetComponent<ButtonController>().SetLinkedNode(i_node);
				_releaseButton.GetComponent<ButtonController>().SetButtonType(GameManager.ButtonType.Release);
				break;
			case GameManager.NodeState.Encrypted:
				_viewButton= (GameObject)Instantiate(ViewButtonPrefab, i_node.gameObject.transform.position + MID_LEFT, Quaternion.identity);
				_viewButton.GetComponent<ButtonController>().SetLinkedNode(i_node);
				_viewButton.GetComponent<ButtonController>().SetButtonType(GameManager.ButtonType.View);
				_releaseButton = (GameObject)Instantiate(ReleaseButtonPrefab, i_node.gameObject.transform.position + MID_RIGHT, Quaternion.identity);
				_releaseButton.GetComponent<ButtonController>().SetLinkedNode(i_node);
				_releaseButton.GetComponent<ButtonController>().SetButtonType(GameManager.ButtonType.Release);
				break;
			case GameManager.NodeState.Capturing:			
			case GameManager.NodeState.Releasing:
			case GameManager.NodeState.Encrypting:
				_cancleButton = (GameObject)Instantiate(ReleaseButtonPrefab, i_node.gameObject.transform.position + MID_RIGHT, Quaternion.identity);
				_cancleButton.GetComponent<ButtonController>().SetLinkedNode(i_node);
				_cancleButton.GetComponent<ButtonController>().SetButtonType(GameManager.ButtonType.Cancle);
				break;
			case GameManager.NodeState.Neutral:
				_captureButton = (GameObject)Instantiate(CaptureButtonPrefab, i_node.gameObject.transform.position + MID_LEFT, Quaternion.identity);
				_captureButton.GetComponent<ButtonController>().SetLinkedNode(i_node);
				_captureButton.GetComponent<ButtonController>().SetButtonType(GameManager.ButtonType.Capture);
				break;
			}
		}		
	}
	
	void CreateCameraBugMenu(Node i_node)
	{
		if(i_node._state >= GameManager.NodeState.Viewing) // this means viewing, YOU CAN ONLY STOP VIEW IF YOU ARE VIEWING!
		{
			_stopViewButton= (GameObject)Instantiate(StopViewButtonPrefab, i_node.gameObject.transform.position + MID_LEFT, Quaternion.identity);
			_stopViewButton.GetComponent<ButtonController>().SetLinkedNode(i_node);
			_stopViewButton.GetComponent<ButtonController>().SetButtonType(GameManager.ButtonType.Stopview);
		}
		else // this means not viewing
		{	
			switch(i_node._state)
			{				
			case GameManager.NodeState.Controlled:
				_viewButton= (GameObject)Instantiate(ViewButtonPrefab, i_node.gameObject.transform.position + MID_LEFT, Quaternion.identity);
				_viewButton.GetComponent<ButtonController>().SetLinkedNode(i_node);	
				_viewButton.GetComponent<ButtonController>().SetButtonType(GameManager.ButtonType.View);
				break;
			}
		}		
	}
	
	void CreatePowerNodeMenu(Node i_node)
	{
		switch(i_node._state)
		{
		case GameManager.NodeState.Controlled:
			_encryptButton= (GameObject)Instantiate(EncryptButtonPrefab, i_node.gameObject.transform.position + MID_LEFT, Quaternion.identity);
			_encryptButton.GetComponent<ButtonController>().SetLinkedNode(i_node);
			_encryptButton.GetComponent<ButtonController>().SetButtonType(GameManager.ButtonType.Encrypt);
			_releaseButton = (GameObject)Instantiate(ReleaseButtonPrefab, i_node.gameObject.transform.position + MID_RIGHT, Quaternion.identity);
			_releaseButton.GetComponent<ButtonController>().SetLinkedNode(i_node);
			_releaseButton.GetComponent<ButtonController>().SetButtonType(GameManager.ButtonType.Release);
			break;
		case GameManager.NodeState.Secured:
		case GameManager.NodeState.Encrypted:
			_releaseButton = (GameObject)Instantiate(ReleaseButtonPrefab, i_node.gameObject.transform.position + MID_LEFT, Quaternion.identity);
			_releaseButton.GetComponent<ButtonController>().SetLinkedNode(i_node);
			_releaseButton.GetComponent<ButtonController>().SetButtonType(GameManager.ButtonType.Release);
			break;
		case GameManager.NodeState.Capturing:			
		case GameManager.NodeState.Releasing:
		case GameManager.NodeState.Encrypting:
			_cancleButton = (GameObject)Instantiate(ReleaseButtonPrefab, i_node.gameObject.transform.position + MID_RIGHT, Quaternion.identity);
			_cancleButton.GetComponent<ButtonController>().SetLinkedNode(i_node);
			_cancleButton.GetComponent<ButtonController>().SetButtonType(GameManager.ButtonType.Cancle);
			break;
		case GameManager.NodeState.Neutral:
			_captureButton = (GameObject)Instantiate(CaptureButtonPrefab, i_node.gameObject.transform.position + MID_LEFT, Quaternion.identity);
			_captureButton.GetComponent<ButtonController>().SetLinkedNode(i_node);
			_captureButton.GetComponent<ButtonController>().SetButtonType(GameManager.ButtonType.Capture);
			break;
		}				
	}
	
	void CreatePasswordNodeMenu(Node i_node)
	{
		//Debug.Log("Called Password Menu");
		GameObject g_password = GameObject.Find("Password");
		Password this_password = g_password.GetComponent<Password>();
		this_password.GetPassword(i_node);
		
	}
	#endregion
	*/
	
	#endregion
	
	#region Indicator
	
	public void UpdateLockDoorIndicatorMat(Node i_nodeForDoor)
	{
		//i_nodeForDoor.ControllingObject.GetComponentsInChildren<DoorIndicator>()[0].SetDoorIndicatorToLocked();
		//i_nodeForDoor.ControllingObject.GetComponentsInChildren<DoorIndicator>()[1].SetDoorIndicatorToLocked();
	}
	
	public void UpdateUnlockDoorIndicatorMat(Node i_nodeForDoor)
	{
		//i_nodeForDoor.ControllingObject.GetComponentsInChildren<DoorIndicator>()[0].SetDoorIndicatorToUnlocked();
		//i_nodeForDoor.ControllingObject.GetComponentsInChildren<DoorIndicator>()[1].SetDoorIndicatorToUnlocked();
	}
	
	public void UpdateJamDoorIndicatorMat(Node i_nodeForDoor)
	{
		//i_nodeForDoor.ControllingObject.GetComponentsInChildren<DoorIndicator>()[0].SetDoorIndicatorToJammed();
		//i_nodeForDoor.ControllingObject.GetComponentsInChildren<DoorIndicator>()[1].SetDoorIndicatorToJammed();
	}
	
	
	/*
	void OnGUI() {
		if ( showGUI )
		{
        	stringToEdit = GUI.TextField (new Rect (Screen.width/2, Screen.height/2, 200, 20), stringToEdit, 25); 
			if (GUI.Button(new Rect(Screen.width/2, Screen.height/2+30, 200, 30), "Submit Password"))
			{
            	print(stringToEdit);
				showGUI = false;
				Password.VerifyPassword(passwordNode, stringToEdit);
			}
		}
    }*/
	#endregion
	
	#region ShockWave 
	
	public void CreateEMPShockWaveAtPlayerPosition(float i_radius)
	{
		
		GameObject Player = GameObject.Find("Playertheif(Clone)");
		GameObject EMPShockWave = (GameObject)Instantiate(EMPShockWavePrefab, Player.transform.position + new Vector3(0, 63, 0), Quaternion.identity);
		EMPShockWave.renderer.enabled = false;
		EMPShockWave.transform.localScale = EMPShockWave.transform.localScale * i_radius;
	}
	
	public void CreateIRShockWaveForNode(Node i_node, float i_radius)
	{
		GameObject IRShockWave = (GameObject)Instantiate(IRShockWavePrefab, i_node.gameObject.transform.position + new Vector3(0.0f,-0.2f,0.0f), Quaternion.identity);
		IRShockWave.transform.localScale = IRShockWave.transform.localScale * i_radius;
		//i_node.SetIRWave(IRShockWave);
	}
	
	#endregion
	
	#region Ping

	public void CreatePingCircleatHackerPingPosition(Vector3 pingPos, float i_radius)
	{
		
		//GameObject Ping = GameObject.FindGameObjectWithTag("PingSystem_Hacker(Clone)");
		GameObject PingCircle = (GameObject)Instantiate(HackerPingCirclePrefab, pingPos, Quaternion.identity);
		//PingCircle.renderer.enabled = false;
		PingCircle.transform.localScale = PingCircle.transform.localScale * i_radius;
	}

	public void DeletePingCircleatHackerPingPosition()
	{
		
		GameObject PingCircle = GameObject.Find("PingCircle(Clone)");
		Destroy(PingCircle);
	}
	
	#endregion
	
}
