using UnityEngine;
using System.Collections;
using System.IO;

public class ChatScript : MonoBehaviour 
{
	private Rect _chatWindow = new Rect(200, 200, 200, 400);
	private string _messBox = "Press enter to text chat.\n", _messageToSend = "";
	private GameObject _playerUtil;
	private bool _showTextField = false;
	private bool _enterWithinTextField = false;
	private KeyCode _previousKeyCode;
	
	public GUISkin CustomSkin;
	
	void Start()
	{	
		_playerUtil = GameObject.Find("PlayerUtil");
		_messBox = "Press enter to text chat.\n";
		VirtualKeyboard.EnterPressed(enter);
	}
	
	private void OnGUI()
	{
		GUI.skin = CustomSkin;
		GUI.depth = 1;
		if(Network.peerType != NetworkPeerType.Disconnected)
		{
			chatFunc();
			//_chatWindow = GUI.Window(3, _chatWindow, chatFunc, "Chat");
		}
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
	
	private void chatFunc()
	{
		Event e = Event.current;

		if(e.keyCode == KeyCode.Return && e.type == EventType.keyDown)
		{
			//Debug.Log("Changing?");
			if(_showTextField == false)
			{
				_showTextField = true;
			}
			else
			{
				if(_messageToSend != "")
				{
					networkView.RPC("SendMessage", RPCMode.All, _playerUtil.GetComponent<AccountSystem>().GetName() + ": " +  _messageToSend + "\r\n");
					_messageToSend = "";
					if (VirtualKeyboard.enabled == true)
						VirtualKeyboard.text = _messageToSend ;
				}
				_showTextField = false;
			}
		}

		if(Application.loadedLevel == 0)
		{
			ScreenHelper.DrawTextBoxForChat(3, 16, 9, 14, _messBox, 24, CustomSkin);
			
			if(_showTextField)
			{
				if (VirtualKeyboard.enabled == true)
				{
					_messageToSend = VirtualKeyboard.text;
					//Debug.Log ("VirtualKeyboard.text; = " +VirtualKeyboard.text);
				}
				if (VirtualKeyboard.enabled == false) GUI.SetNextControlName("Text1");
				_messageToSend = ScreenHelper.DrawTextFieldForChat(3, 30.0f,9, 1.5f, _messageToSend, 24, CustomSkin);
				VirtualKeyboard.text = "";
				if (VirtualKeyboard.enabled == false)GUI.FocusControl("Text1");
			}
		}
		else
		{
			ScreenHelper.DrawTextBoxForChat(20, 31, 24, 4.5f, _messBox, 24, CustomSkin);

			if(_showTextField)
			{

				if (VirtualKeyboard.enabled == true)
					_messageToSend = VirtualKeyboard.text;
				if (VirtualKeyboard.enabled == false) GUI.SetNextControlName("Text1");
				_messageToSend = ScreenHelper.DrawTextFieldForChat(3, 30.0f,9, 1.5f, _messageToSend, 24, CustomSkin);
				if (VirtualKeyboard.enabled == false) GUI.FocusControl("Text1");
			}
		}
	}
	
	void Update()
	{
		/*
		if(Input.GetKeyDown(KeyCode.Return))
		{
			if(_showTextField == false)
			{
				_showTextField = true;
				Debug.Log("Changing?");
			}
			else
			{
				if(_messageToSend != "")
				{
					networkView.RPC("SendMessage", RPCMode.All, _playerUtil.GetComponent<AccountSystem>().GetName() + ": " +  _messageToSend + "\r\n");
					_messageToSend = "";
				}
				_showTextField = false;
			}
		}
		*/
	}
	
	
	[RPC]
	private void SendMessage(string i_mess)
	{
		_messBox += i_mess;
	}

}
