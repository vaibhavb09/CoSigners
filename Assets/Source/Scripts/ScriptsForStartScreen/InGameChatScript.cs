using UnityEngine;
using System.Collections;
using System;

public class InGameChatScript : MonoBehaviour {
	
	private Rect _chatWindow = new Rect(200, 200, 200, 400);
	private string _messBox = "Press enter to text chat.\n", _messageToSend = "";
	private GameObject _playerUtil;
	private bool _showTextField = false;
	private bool _enterWithinTextField = false;
	private KeyCode _previousKeyCode;
	
	private GUISkin _customSkin;
	
	
	void Start()
	{	
		_playerUtil = GameObject.Find("PlayerUtil");
		_messBox = "Press enter to text chat.\n";
		_customSkin = (GUISkin)Resources.Load("Skins/ChatSkin");
		VirtualKeyboard.EnterPressed(enter);
	}
	
	private void enter()
	{
		if(_showTextField == false)
		{
			_showTextField = true;
			
		}
		else
		{
			if(_messageToSend != "")
			{
				NetworkManager.Manager.SendChatMessage(_playerUtil.GetComponent<AccountSystem>().GetName() + ": " +  _messageToSend + "\r\n");
				_messageToSend = "";
				if (VirtualKeyboard.enabled == true)
					VirtualKeyboard.text = _messageToSend ;
			}
			_showTextField = false;
		}
	}
	
	private void OnGUI()
	{
		GUI.skin = _customSkin;
		GUI.depth = 1;
		if(Network.peerType != NetworkPeerType.Disconnected)
		{
			chatFunc();
		}
	}
	
	
	
	private void chatFunc() 
	{
		Event e = Event.current;
		if( ( GameManager.Manager.PlayerType == 1 && ThiefManager.Manager.CurrentFocus == TextFocus.TextChat ) || ( GameManager.Manager.PlayerType == 2 && HackerManager.Manager.CurrentFocus == TextFocus.TextChat ) )
		{
			if(e.keyCode == KeyCode.Return && e.type == EventType.keyDown)
			{
				//Debug.Log("happening?");
				if(_showTextField == false)
				{
					_showTextField = true;
					
				}
				else
				{
					if(_messageToSend != "")
					{
						NetworkManager.Manager.SendChatMessage(_playerUtil.GetComponent<AccountSystem>().GetName() + ": " +  _messageToSend + "\r\n");
						_messageToSend = "";
						if (VirtualKeyboard.enabled == true)
							VirtualKeyboard.text = _messageToSend ;
					}
					_showTextField = false;
				}
			}
		
		
			if(Application.loadedLevel == 0)
			{
				ScreenHelper.DrawTextBoxForChat(5, 11, 24, 24.5f, _messBox, 24, _customSkin);
				
				if(_showTextField)
				{
					if (VirtualKeyboard.enabled == true)
					{
						_messageToSend = VirtualKeyboard.text;
					}
					if (VirtualKeyboard.enabled == false)
					{
						GUI.SetNextControlName("Text1");
					}
					_messageToSend = ScreenHelper.DrawTextFieldForChat(5, 34.5f, 24, 1.5f, _messageToSend, 24, _customSkin);
					if (VirtualKeyboard.enabled == false)
					{
						GUI.FocusControl("Text1");
					}
					VirtualKeyboard.text = "";
				}
			}
			else
			{
				ScreenHelper.DrawTextBoxForChat(20, 31, 24, 4.5f, _messBox, 24, _customSkin);
				
				if(_showTextField)
				{
					if (VirtualKeyboard.enabled == false)
					{
						GUI.SetNextControlName("Text1");
					}
					if (VirtualKeyboard.enabled == true)
					{
						_messageToSend = VirtualKeyboard.text;
					}
					_messageToSend = ScreenHelper.DrawTextFieldForChat(20, 34.5f, 24, 1.5f, _messageToSend, 24, _customSkin);
					if (VirtualKeyboard.enabled == false) 
					{
						GUI.FocusControl("Text1");
					}
					VirtualKeyboard.text = "";
				}
			}
		}
	}
	
	public void SendMessage(string i_mess)
	{
		_messBox += i_mess;
	}
}