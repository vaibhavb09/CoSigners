using UnityEngine;
using System.Collections;

public class SecurityNode : Node {
	
	private TopDown_Camera myTopDownCam;
	private string _playerInput;
	private bool _showGUI, _inputOver, _activated, _wrongPswd, _showWrongPswdGUI;
	private HackerActions myActions;
	public int _levelGranted;
	public bool usedUp;
	public int correctPasswordEntered = -1;
	private float timerStart, timerTime;
	
	private Texture2D _passwordBG;
	private Texture2D _submitButtonActive, _submitButtonNormal;
	private Texture2D _cancelButtonActive, _cancelButtonNormal;
	private Texture2D _buttonAccentLeft, _buttonAccentRight;
	private bool vKeyBoardEnterPressed;
	
	public SecurityNode()
	{
	}
	
	public void Set( SecurityNodeData i_data )
	{
		Index = i_data.Index;
		Connected = false;
		usedUp = false;
		_levelGranted = i_data.Level;
		SecurityLevel = i_data.SecurityLevel;
		PowerConsumption = 7;
		Type = GameManager.NodeType.Password;
		float myScale = GraphManager.Manager.DefaultScale;
		this.gameObject.transform.localScale = new Vector3(myScale, myScale, myScale);
	}
	
	
	// Use this for initialization
	void Start () {
		vKeyBoardEnterPressed = false;
		_playerInput = "";
		_showGUI = _inputOver = false;
		myTopDownCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<TopDown_Camera>();
		myActions = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<HackerActions>();
		
		_passwordBG = Resources.Load("Textures/UI/PasswordUI/password_bkg", typeof(Texture2D)) as Texture2D;
		_submitButtonActive = Resources.Load("Textures/UI/PasswordUI/btn_Submit_active", typeof( Texture2D ) ) as Texture2D;
		_submitButtonNormal = Resources.Load("Textures/UI/PasswordUI/btn_Submit_normal", typeof( Texture2D ) ) as Texture2D;		
		_cancelButtonActive = Resources.Load("Textures/UI/PasswordUI/btn_cancel_active", typeof( Texture2D ) ) as Texture2D;
		_cancelButtonNormal = Resources.Load("Textures/UI/PasswordUI/btn_cancel_normal", typeof( Texture2D ) ) as Texture2D;
		_buttonAccentLeft   = Resources.Load("Textures/UI/PasswordUI/btnAccent_right", typeof(Texture2D)) as Texture2D;
		_buttonAccentRight  = Resources.Load("Textures/UI/PasswordUI/btnAccent_left", typeof(Texture2D)) as Texture2D;
	}
			
	// Update is called once per frame
	void Update ()
	{		
		if( _wrongPswd )
		{
			timerStart = Time.time;
			_wrongPswd = false;	
		}
		
		if( _showWrongPswdGUI )
			timerTime = Time.time - timerStart;
			
	}
	
	
	public override void SetConnected( bool i_connected )
	{
		
		if((Connected == true ) && (i_connected == false))
		{
			// [ SOUND TAG ] [Node_Disconnect]
			if(GameManager.Manager.PlayerType == 2)
				soundMan.soundMgr.playOneShotOnSource(null,"Node_Disconnect",GameManager.Manager.PlayerType,GameManager.Manager.PlayerType);
			
			if( myActions._disabled == true )
			{
				myActions._disabled = false;
			}
		}

		Connected = i_connected;
		if ( usedUp )
		{
			this.gameObject.transform.renderer.material = GraphManager.Manager.SecurityAccepted;
		}
		else
		{
			if ( i_connected && HackerManager.Manager.CheckHackerClearance( this.SecurityLevel ) )
			{
				_showGUI = true;
				myActions._disabled = true;
				_activated = true;
				HackerManager.Manager.CurrentFocus = TextFocus.PasswordText;
				// Release all previous hacker actions
				PivotManager.Manager.CancelNewLink();
				
				this.gameObject.transform.renderer.material = GraphManager.Manager.SecurityConnected;
				
				// [ SOUND TAG ] Security Node powered [Node_Connect]
				if(GameManager.Manager.PlayerType == 2)
					soundMan.soundMgr.playOneShotOnSource(null,"Node_Connect",GameManager.Manager.PlayerType,GameManager.Manager.PlayerType);
			}
			else
			{
				if( HackerManager.Manager.CheckHackerClearance( this.SecurityLevel ) == false )
				{
					this.gameObject.transform.renderer.material = GraphManager.Manager.SecurityUnavailable;
				}
				else
					this.gameObject.transform.renderer.material = GraphManager.Manager.SecurityAvailable;
			}
		}
	}
	
	
	public override void SetClearance ( )
	{
		if ( usedUp )
		{
			this.gameObject.transform.renderer.material = GraphManager.Manager.SecurityAccepted;
		}
		else
		{
			if( !HackerManager.Manager.CheckHackerClearance( SecurityLevel ) )
			{
				this.gameObject.transform.renderer.material = GraphManager.Manager.SecurityUnavailable;
			}
			else
			{
				this.gameObject.transform.renderer.material = GraphManager.Manager.SecurityAvailable;
			}
		}
	}
	
	void PasswordSubmitted()
	{
		_inputOver = true;	
		_showGUI = false;
		RespondToAction();
		if (VirtualKeyboard.isWindowsTablet == true)
			VirtualKeyboard.RemoteDisable();
	}

	public void vKeyBoardEnter()
	{
		//Debug.Log("vKeyBoardEnter");
		vKeyBoardEnterPressed = true;
	}
	void OnGUI()
	{
		if( GameManager.Manager.PlayerType == 2 )
		{
			if( this.Connected && HackerManager.Manager.CheckHackerClearance( this.SecurityLevel ) && _showGUI == true )
			{
				//Zoom out and Remove the current GUI so this one can work.\
//				if ( !myTopDownCam.Zooming )
//				{
//					myTopDownCam.ZoomOut();
//				}
					
				if (VirtualKeyboard.isWindowsTablet == true && VirtualKeyboard.enabled == false)
				{
					VirtualKeyboard.RemoteEnable();
					VirtualKeyboard.EnterPressed(vKeyBoardEnter);

				}

				ScreenHelper.DrawTexture( 24, 8, 16, 14, _passwordBG );


				if( vKeyBoardEnterPressed || ScreenHelper.DrawButton( 20, 17, 12, 2, _submitButtonActive, _submitButtonNormal ) )
				{
					vKeyBoardEnterPressed =false;
					PasswordSubmitted();
				}
				
				if( Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return )
				{
					PasswordSubmitted();
				}
				
				if( ScreenHelper.DrawButton( 32, 18, 12, 2, _cancelButtonActive, _cancelButtonNormal ) )
				{
					GraphManager.Manager.HackerClickedCancel();
				}
				ScreenHelper.DrawTexture(29, 18, 4, 2, _buttonAccentLeft);
				ScreenHelper.DrawTexture(31, 17, 4, 2, _buttonAccentRight);
				GUI.SetNextControlName("PlayerInput");
				_playerInput = ScreenHelper.DrawTextField( 28, 13, 8, 3, _playerInput, 25 );
				GUI.FocusControl("PlayerInput");
				if (VirtualKeyboard.isWindowsTablet == true)
					_playerInput = VirtualKeyboard.text;
				if( _showWrongPswdGUI )
				{
					if( timerTime < 2 )
					{
						ScreenHelper.DrawText( 28, 16, 8, 3, "Wrong Password", 23, new Color( 1.0f, 1.0f, 0.0f, 1.0f ) );
					}
				}
				
			}	
		}
	}
	
	public void RespondToAction()
	{
		if( _inputOver && Connected && _activated )
		{
			if( ProcessInput() )
			{
				//Right password
				_showGUI = false;
				myActions._disabled = false;

				/*
				// Raise Security Clearance
				BasicScoreSystem.Manager.SecurityNodesCaptured += 1;
				HackerManager.Manager.HackerClearance += 1;
				BasicScoreSystem.Manager.PasswordsObtained += 1;
				GraphManager.Manager.RefreshSecurityClearance();
				GraphManager.Manager.RefreshGraph();
				*/

				NetworkManager.Manager.RaiseSecurityClearance();


				_activated = false;
				usedUp = true;
				_showWrongPswdGUI = false;
//				GraphManager.Manager.HandleClearanceIncrease( true );
				
				this.gameObject.transform.renderer.material = GraphManager.Manager.SecurityAccepted;
				correctPasswordEntered = 1;
				// Password was entered successfully [SOUND TAG] [Password_enter_Succeed]
				soundMan.soundMgr.playOneShotOnSource(null,"Password_enter_Succeed",GameManager.Manager.PlayerType,2);
				this.SecurityLevel += 2;
				HackerManager.Manager.CurrentFocus = TextFocus.TextChat;
			}
			else
			{
				//Wrong password
				_showGUI = true;
				_inputOver = false;
				correctPasswordEntered = 0;
				_wrongPswd = true;
				_showWrongPswdGUI = true;
				
				soundMan.soundMgr.playOneShotOnSource(null,"Password_enter_fail",GameManager.Manager.PlayerType,2);
				// Password was entered incorrectly [SOUND TAG] [Password_enter_fail]
			}
		}
	}
		
	public void HackerClickedCancel()
	{
		_showGUI = false;
		myActions._disabled = false;
		_playerInput ="";
		HackerManager.Manager.CurrentFocus = TextFocus.TextChat;
		RespondToAction();
		if (VirtualKeyboard.isWindowsTablet == true)
			VirtualKeyboard.RemoteDisable();
	}

	public bool ProcessInput()
	{
		foreach( string s in PasswordGenerator.Manager.AcceptablePasswords )
		{
			if( string.Equals( _playerInput.ToLower(), s.ToLower() ) )
			{
				PasswordGenerator.Manager.AcceptablePasswords.Remove( s );
				return true;
			}
		}
		return false;
	}
	
	public override void HandleClickEvent()
	{
		if( Connected )
		{
			_showGUI = true;
			myActions._disabled = true;
			_activated = true;
			HackerManager.Manager.CurrentFocus = TextFocus.PasswordText;
		}
	}
	
}
