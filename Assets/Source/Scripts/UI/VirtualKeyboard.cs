using UnityEngine;
using System.Collections;
using System;
public class VirtualKeyboard : MonoBehaviour {
	
	//	#region singleton declearation
	//	private static VirtualKeyboard m_instance;
	//	
	//	public VirtualKeyboard () 
	//	{
	//		if (m_instance != null)
	//		{
	//			return;
	//		}
	//		
	//		m_instance = this;
	//	}
	//	
	//	public static VirtualKeyboard Manager
	//	{
	//		get
	//		{
	//			if(m_instance == null)
	//			{
	//				m_instance = new VirtualKeyboard();			
	//			}
	//			return m_instance;
	//		}
	//	}
	//	#endregion
	
	// Use this for initialization
	//ArrayList<GUI.Butto>
	public static bool isWindowsTablet;
	public static string text;
	public static bool enabled;
	public static bool enterPressed;
	private Texture2D VKeypadTex;
	public GUIStyle Button_VPAD;
	private bool capsToggle;
	public static float keyWidth = PosW(2.5f);
	public static float keySpacing = PosW(0.5f);
	public static Rect windowRect = new Rect (PosW(12), Screen.height - 5.5f * keyWidth , 14 * keyWidth + 14 * keySpacing, 5.5f * keyWidth);
	public static Rect toggleRect = new Rect(PosW(59), PosH(7), PosW(4), PosH (4));
	//private string[] lower = new string[] {"`", "1", "2", "3", "4", "5", "6", "7", "8", "9", "0", "-", "=", "q", "w", "e", "r", "t", "y", "u", "i", "o", "p", "[", "]", "\\", "a", "s", "d", "f", "g", "h", "j", "k", "l", ";", "'", "z", "x", "c", "v", "b", "n", "m", ",", ".", "/","","Caps / Symbol","BackSpace","Clear","Enter"}; 
	private string[] lower = new string[] {	"q", "w", "e", "r", "t", "y", "u", "i", "o", "p", "-", "=",
		"!", "a", "s", "d", "f", "g", "h", "j", "k", "l", ":", "(", ")",
		"@","z", "x", "c", "v", "b", "n", "m", ",", ".", "?","'",
		"BackSpace", "Space", "Clear","Enter"}; 
	//private string[] upper = new string[] {"~", "!", "@", "#", "$", "%", "^", "&", "*", "(", ")", "_", "+", "Q", "W", "E", "R", "T", "Y", "U", "I", "O", "P", "{", "}", "|", "A", "S", "D", "F", "G", "H", "J", "K", "L", ":", "\"", "Z", "X", "C", "V", "B", "N", "M", "<", ">", "?","","Caps / Symbol","BackSpace","Clear","Enter"}; 
	private string[] upper = new string[] {	"q", "w", "e", "r", "t", "y", "u", "i", "o", "p", "-", "=",
		"!", "a", "s", "d", "f", "g", "h", "j", "k", "l", ":", "(", ")",
		"z", "x", "c", "v", "b", "n", "m", ",", ".", "?","'",
		"BackSpace","Space","Clear","Enter"}; 
	private string[] common = new string[] {}; 
	private string[] selection;
	private static Action _enterPressed = null;
	public static bool isEnterPressed = false;
	public float posMoveStartTime;
	public Rect tempWindowPos = new Rect(windowRect.x,Screen.height,windowRect.width,windowRect.height);
	
	public enum VKeyBoardState
	{
		None,
		EnabledThisFrame,
		MovingToPosition,
		ReachedPosition,
		DisabledThisFrame,
		RemoteEnabled
	};
	
	public static VKeyBoardState vKeyBoardState = VKeyBoardState.None;
	
	public static void RemoteEnable()
	{
		//Debug.Log ("RemoteEnable");
		enabled = true;
		vKeyBoardState = VKeyBoardState.EnabledThisFrame;
	}
	
	public static void RemoteDisable()
	{
		//Debug.Log ("RemoteDisable");
		enabled = false;
		vKeyBoardState = VKeyBoardState.DisabledThisFrame;
	}
	
	public static void EnterPressed(Action i_enterPressed )
	{
		_enterPressed = i_enterPressed;
	}
	
	private static float PosW(float i_increment)
	{
		return (Screen.width/64)*i_increment;
	}
	
	private static float PosH(float i_increment)
	{
		return (Screen.height/36)*i_increment;
	}
	
	void Start () {
		isWindowsTablet = false;
		VKeypadTex = Resources.Load ("Textures/HackerGUI/WASD", typeof(Texture2D)) as Texture2D;
		capsToggle = false;
		text = "";
		
		//if (VirtualKeyboard.isWindowsTablet == true == true)
		enabled = false;
		//else
		//enabled = false;	
	}
	
	void OnGUI () 
	{
		
		
		if ( isWindowsTablet && GUI.Button (toggleRect,GUIContent.none,Button_VPAD))
		{
			enabled = !enabled;
			if (enabled == true)
				vKeyBoardState = VKeyBoardState.EnabledThisFrame;
			else
				vKeyBoardState = VKeyBoardState.DisabledThisFrame;
		}
		
		if (enabled)
		{
			//GUI.Window (0, windowRect, WindowFunction, "Virtual Keyboard");
			
			
			
			//Debug.Log ("Enabled = " + enabled);
			if (tempWindowPos.y < windowRect.y)
			{
				vKeyBoardState = VKeyBoardState.ReachedPosition;
				GUI.Window (0, windowRect, WindowFunction, "Virtual Keyboard");
				//Debug.Log ("tempWindowPos.y < windowRect.y");
			}
			
			if (vKeyBoardState == VKeyBoardState.MovingToPosition)
			{
				//Debug.Log ("vKeyBoardState == VKeyBoardState.MovingToPosition");
				float distCovered = (Time.time - posMoveStartTime) * 1000;
				//Debug.Log ("distCovered = " + distCovered);
				//float fracJourney = distCovered / (Screen.height - windowRect.y);
				tempWindowPos.y = Screen.height - distCovered;
				//tempWindowPos.y = Vector3.Lerp(Screen.height, windowRect.y, fracJourney);
				tempWindowPos = new Rect(windowRect.x,tempWindowPos.y,windowRect.width,windowRect.height);
				GUI.Window (0, tempWindowPos, WindowFunction, "Virtual Keyboard");
			}
			
			if (vKeyBoardState == VKeyBoardState.EnabledThisFrame)
			{
				//Debug.Log ("vKeyBoardState == VKeyBoardState.EnabledThisFrame");
				//tempWindowPos = new Rect(windowRect.x,Screen.height,windowRect.width,windowRect.height);
				GUI.Window (0, tempWindowPos, WindowFunction, "Virtual Keyboard");
				vKeyBoardState = VKeyBoardState.MovingToPosition;
				posMoveStartTime = Time.time;
			}
			
			
		}
		
		else
		{
			//Debug.Log ("Enabled = " + enabled);
			if (tempWindowPos.y > Screen.height)
			{
				vKeyBoardState = VKeyBoardState.ReachedPosition;
				//GUI.Window (0, windowRect, WindowFunction, "Virtual Keyboard");
				//Debug.Log ("tempWindowPos.y < windowRect.y");
			}
			
			if (vKeyBoardState == VKeyBoardState.MovingToPosition)
			{
				//Debug.Log ("vKeyBoardState == VKeyBoardState.MovingToPosition");
				float distCovered = (Time.time - posMoveStartTime) * 100;
				//Debug.Log ("distCovered = " + distCovered);
				//float fracJourney = distCovered / (Screen.height - windowRect.y);
				tempWindowPos.y = tempWindowPos.y + distCovered;
				//tempWindowPos.y = Vector3.Lerp(Screen.height, windowRect.y, fracJourney);
				tempWindowPos = new Rect(windowRect.x,tempWindowPos.y,windowRect.width,windowRect.height);
				GUI.Window (0, tempWindowPos, WindowFunction, "Virtual Keyboard");
			}
			
			if (vKeyBoardState == VKeyBoardState.DisabledThisFrame)
			{
				//Debug.Log ("vKeyBoardState == VKeyBoardState.DisabledThisFrame");
				//tempWindowPos = new Rect(windowRect.x,tempWindowPos.y,windowRect.width,windowRect.height);
				GUI.Window (0, tempWindowPos, WindowFunction, "Virtual Keyboard");
				vKeyBoardState = VKeyBoardState.MovingToPosition;
				posMoveStartTime = Time.time;
			}
			
		}
		
		
	}
	
	void WindowFunction (int windowID) {
		// Draw any Controls inside the window here
		//GUI.Label (new Rect (5 * keyWidth, 0.5f * keyWidth,6 * keyWidth ,keyWidth), "Input : " + text);
		
		int i = 1;
		int j = 1;
		if (capsToggle)
			selection = upper;
		else
			selection = lower;
		foreach (string Key in selection)
		{
			float finalKeyWidth = 0;
			float finalKeyPosX = 0;
			if (Key == "Caps / Symbol" || Key == "BackSpace" || Key == "Clear" || Key == "Enter" || Key == "Space")
			{
				finalKeyWidth = keyWidth * 3; finalKeyPosX = ((i - 1) * finalKeyWidth ) + keyWidth + (i - 1) * 5 * keySpacing;
			}
			else
			{
				if (j !=2)
				{
					finalKeyWidth = keyWidth; finalKeyPosX = i * keyWidth + i * keySpacing;
				}
				else
				{
					finalKeyWidth = keyWidth; finalKeyPosX = i * keyWidth + i * keySpacing - keyWidth/2;
				}
			}
			if (GUI.Button (new Rect ( finalKeyPosX, j * keyWidth + j * keySpacing - keyWidth/2, finalKeyWidth, keyWidth), Key)) 
			{
				//Debug.Log ("Cliked "+Key);
				if ( Key == "Caps / Symbol")
					capsToggle = !capsToggle;
				else if ( Key == "BackSpace" )
					text = text.Remove(text.Length - 1);
				else if ( Key == "Clear" )
					text = "";
				else if ( Key == "Enter" )
				{
					RemoteDisable();
					isEnterPressed = true;
					//if(_enterPressed != null)
					//{
					//_enterPressed();
					//break;
					//}
					//text = "";
				}
				else if ( Key == "Space" )
					text = text +" ";
				else
				{
					text = text + Key;
				}
				
			}
			i++;
			int noOfKeysInRow = 0;
			if (j == 1)
				noOfKeysInRow = 12;
			if (j == 2)
				noOfKeysInRow = 13;
			if (j == 3)
				noOfKeysInRow = 12;
			if (j == 4)
				noOfKeysInRow = 4;
			
			if (i % (noOfKeysInRow+1) == 0)
			{
				i = 1;
				j++;
			}
		}
		
		
		//GUI.DragWindow();
	}
	
	
	// Update is called once per frame
	void Update () {
		isEnterPressed =false;
	}
}