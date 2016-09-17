using UnityEngine;
using System.Collections;

public class Triggers : MonoBehaviour {
	
	
	public Texture2D TextureForBackground = null;
	public Texture2D TextureForButton = null;
	public Texture2D TextureForMessage = null;
	public GUISkin MessagingGUISkin;
	public string  mystring = null;
	public Vector2 BoxPosition;
	public int M_BoxHeight;
	public int M_BoxWidth;
	public int m_type;
	bool pause;
	bool willpause;
	bool showGUI;
	bool TutorialToggle;
	public GUIStyle myStyle;
	int m_delta_timeout;
	public int guiHeight = 200;
	public int guiWidth = 200;
	
	// Use this for initialization
	void Start () {
		showGUI = false;
		m_delta_timeout = 10;
		if( GameManager.Manager.PlayerType == 1 )
			Screen.lockCursor = true;
	}
	
	void pausethegame()		
	{
		if(pause)
		{
			Time.timeScale = 0;
			GameObject.Find("Playertheif(Clone)").GetComponent<MouseLookAround>().enabled = false;
		}
	}
//	}
	void OnGUI()
	{
		//if(GameManager.Manager.PlayerType == 1) // is thief
		{
			if( showGUI )
			{
			 	if( m_type == 1 )
				{
		
					pausethegame();
					
					TextureForBackground =Resources.Load("Textures/MessageBox") as Texture2D;
					TextureForButton = Resources.Load("Textures/OK_Button") as Texture2D;
					TextureForMessage = Resources.Load("Textures/type1") as Texture2D;
					GUI.BeginGroup(new Rect((Screen.width/2-TextureForBackground.width/2),(Screen.height/2-TextureForBackground.height/2),TextureForBackground.width,TextureForBackground.height),TextureForBackground);
					GUI.skin = MessagingGUISkin;
					GUI.Box(new Rect((Screen.width/2-TextureForMessage.width/2),(Screen.height/2-TextureForMessage.height/2),TextureForMessage.width,TextureForMessage.height),TextureForMessage);
					if(GUI.Button(new Rect((Screen.width/2-TextureForButton.width/2-140),(Screen.height/2-TextureForButton.height/2+60),TextureForButton.width,TextureForButton.height),TextureForButton))
						{
							showGUI = false;
							Time.timeScale = 1;
							Screen.lockCursor = false;
							GameObject.Find("Playertheif").GetComponent<MouseLook>().enabled = true;
							//Debug.Log ("click the button");
						}
					GUI.EndGroup();
					if( Time.fixedTime % m_delta_timeout == 0 )
					{
						showGUI = false;	
						Time.timeScale = 1;
						Screen.lockCursor = false;
					}
				
				}
				if( m_type == 2 )
			
				{
		
					pausethegame();
					
					TextureForBackground =Resources.Load("Textures/MessageBox") as Texture2D;
					TextureForButton = Resources.Load("Textures/OK_Button") as Texture2D;
					TextureForMessage = Resources.Load("Textures/type2") as Texture2D;
					GUI.BeginGroup(new Rect((Screen.width/2-TextureForBackground.width/2),(Screen.height/2-TextureForBackground.height/2),TextureForBackground.width,TextureForBackground.height),TextureForBackground);
					GUI.skin = MessagingGUISkin;
					GUI.Box(new Rect((Screen.width/2-TextureForMessage.width/2),(Screen.height/2-TextureForMessage.height/2),TextureForMessage.width,TextureForMessage.height),TextureForMessage);
					if(GUI.Button(new Rect((Screen.width/2-TextureForButton.width/2-140),(Screen.height/2-TextureForButton.height/2+60),TextureForButton.width,TextureForButton.height),TextureForButton))
						{
							showGUI = false;
							Time.timeScale = 1;
							Screen.lockCursor = false;
							GameObject.Find("Playertheif").GetComponent<MouseLook>().enabled = true;
							//Debug.Log ("click the button");
						}
					GUI.EndGroup();
					if( Time.fixedTime % m_delta_timeout == 0 )
					{
						showGUI = false;	
						Time.timeScale = 1;
						Screen.lockCursor = false;
					}
				
				}
				if( m_type == 3 )
				{
		
					pausethegame();
					
					TextureForBackground =Resources.Load("Textures/MessageBox") as Texture2D;
					TextureForButton = Resources.Load("Textures/OK_Button") as Texture2D;
					TextureForMessage = Resources.Load("Textures/type3") as Texture2D;
					GUI.BeginGroup(new Rect((Screen.width/2-TextureForBackground.width/2),(Screen.height/2-TextureForBackground.height/2),TextureForBackground.width,TextureForBackground.height),TextureForBackground);
					GUI.skin = MessagingGUISkin;
					GUI.Box(new Rect((Screen.width/2-TextureForMessage.width/2),(Screen.height/2-TextureForMessage.height/2),TextureForMessage.width,TextureForMessage.height),TextureForMessage);
					if(GUI.Button(new Rect((Screen.width/2-TextureForButton.width/2-140),(Screen.height/2-TextureForButton.height/2+60),TextureForButton.width,TextureForButton.height),TextureForButton))
						{
							showGUI = false;
							Time.timeScale = 1;
							Screen.lockCursor = false;
							GameObject.Find("Playertheif").GetComponent<MouseLook>().enabled = true;
							//Debug.Log ("click the button");
						}
					GUI.EndGroup();
					if( Time.fixedTime % m_delta_timeout == 0 )
					{
						showGUI = false;	
						Time.timeScale = 1;
						Screen.lockCursor = false;
					}
				
				}
				if( m_type == 4 )
				{
		
					pausethegame();
					
					TextureForBackground =Resources.Load("Textures/MessageBox") as Texture2D;
					TextureForButton = Resources.Load("Textures/OK_Button") as Texture2D;
					TextureForMessage = Resources.Load("Textures/type4") as Texture2D;
					GUI.BeginGroup(new Rect((Screen.width/2-TextureForBackground.width/2),(Screen.height/2-TextureForBackground.height/2),TextureForBackground.width,TextureForBackground.height),TextureForBackground);
					GUI.skin = MessagingGUISkin;
					GUI.Box(new Rect((Screen.width/2-TextureForMessage.width/2),(Screen.height/2-TextureForMessage.height/2),TextureForMessage.width,TextureForMessage.height),TextureForMessage);
					if(GUI.Button(new Rect((Screen.width/2-TextureForButton.width/2-140),(Screen.height/2-TextureForButton.height/2+60),TextureForButton.width,TextureForButton.height),TextureForButton))
						{
							showGUI = false;
							Time.timeScale = 1;
							Screen.lockCursor = false;
							GameObject.Find("Playertheif").GetComponent<MouseLook>().enabled = true;
							//Debug.Log ("click the button");
						}
					GUI.EndGroup();
					if( Time.fixedTime % m_delta_timeout == 0 )
					{
						showGUI = false;	
						Time.timeScale = 1;
						Screen.lockCursor = false;
					}
				
				}
				if( m_type == 5 )
				{
		
					pausethegame();
					
					TextureForBackground =Resources.Load("Textures/MessageBox") as Texture2D;
					TextureForButton = Resources.Load("Textures/OK_Button") as Texture2D;
					TextureForMessage = Resources.Load("Textures/type5") as Texture2D;
					GUI.BeginGroup(new Rect((Screen.width/2-TextureForBackground.width/2),(Screen.height/2-TextureForBackground.height/2),TextureForBackground.width,TextureForBackground.height),TextureForBackground);
					GUI.skin = MessagingGUISkin;
					GUI.Box(new Rect((Screen.width/2-TextureForMessage.width/2),(Screen.height/2-TextureForMessage.height/2),TextureForMessage.width,TextureForMessage.height),TextureForMessage);
					if(GUI.Button(new Rect((Screen.width/2-TextureForButton.width/2-140),(Screen.height/2-TextureForButton.height/2+60),TextureForButton.width,TextureForButton.height),TextureForButton))
						{
							showGUI = false;
							Time.timeScale = 1;
							Screen.lockCursor = false;
							GameObject.Find("Playertheif").GetComponent<MouseLook>().enabled = true;
							//Debug.Log ("click the button");
						}
					GUI.EndGroup();
					if( Time.fixedTime % m_delta_timeout == 0 )
					{
						showGUI = false;	
						Time.timeScale = 1;
						Screen.lockCursor = false;
					}
				
				}
				
				if( m_type == 6 )
				{
					pausethegame();
					
					TextureForBackground =Resources.Load("Textures/MessageBox") as Texture2D;
					TextureForButton = Resources.Load("Textures/OK_Button") as Texture2D;
					TextureForMessage = Resources.Load("Textures/type6") as Texture2D;
					GUI.BeginGroup(new Rect((Screen.width/2-TextureForBackground.width/2),(Screen.height/2-TextureForBackground.height/2),TextureForBackground.width,TextureForBackground.height),TextureForBackground);
					GUI.skin = MessagingGUISkin;
					GUI.Box(new Rect((Screen.width/2-TextureForMessage.width/2),(Screen.height/2-TextureForMessage.height/2),TextureForMessage.width,TextureForMessage.height),TextureForMessage);
					if(GUI.Button(new Rect((Screen.width/2-TextureForButton.width/2-140),(Screen.height/2-TextureForButton.height/2+60),TextureForButton.width,TextureForButton.height),TextureForButton))
						{
							showGUI = false;
							Time.timeScale = 1;
							Screen.lockCursor = false;
							GameObject.Find("Playertheif").GetComponent<MouseLook>().enabled = true;
							//Debug.Log ("click the button");
						}
					GUI.EndGroup();
					if( Time.fixedTime % m_delta_timeout == 0 )
					{
						showGUI = false;	
						Time.timeScale = 1;
						Screen.lockCursor = false;
					}
				}
				if( m_type == 7 )
				{
					pausethegame();
					
					TextureForBackground =Resources.Load("Textures/MessageBox") as Texture2D;
					TextureForButton = Resources.Load("Textures/OK_Button") as Texture2D;
					TextureForMessage = Resources.Load("Textures/type7") as Texture2D;
					GUI.BeginGroup(new Rect((Screen.width/2-TextureForBackground.width/2),(Screen.height/2-TextureForBackground.height/2),TextureForBackground.width,TextureForBackground.height),TextureForBackground);
					GUI.skin = MessagingGUISkin;
					GUI.Box(new Rect((Screen.width/2-TextureForMessage.width/2),(Screen.height/2-TextureForMessage.height/2),TextureForMessage.width,TextureForMessage.height),TextureForMessage);
					if(GUI.Button(new Rect((Screen.width/2-TextureForButton.width/2-140),(Screen.height/2-TextureForButton.height/2+60),TextureForButton.width,TextureForButton.height),TextureForButton))
						{
							showGUI = false;
							Time.timeScale = 1;
							Screen.lockCursor = false;
							GameObject.Find("Playertheif").GetComponent<MouseLook>().enabled = true;
							//Debug.Log ("click the button");
						}
					GUI.EndGroup();
					if( Time.fixedTime % m_delta_timeout == 0 )
					{
						showGUI = false;	
						Time.timeScale = 1;
						Screen.lockCursor = false;
					}
				}
				if( m_type == 8 )
				{
					pausethegame();
					
					TextureForBackground =Resources.Load("Textures/MessageBox") as Texture2D;
					TextureForButton = Resources.Load("Textures/OK_Button") as Texture2D;
					TextureForMessage = Resources.Load("Textures/type8") as Texture2D;
					GUI.BeginGroup(new Rect((Screen.width/2-TextureForBackground.width/2),(Screen.height/2-TextureForBackground.height/2),TextureForBackground.width,TextureForBackground.height),TextureForBackground);
					GUI.skin = MessagingGUISkin;
					GUI.Box(new Rect((Screen.width/2-TextureForMessage.width/2),(Screen.height/2-TextureForMessage.height/2),TextureForMessage.width,TextureForMessage.height),TextureForMessage);
					if(GUI.Button(new Rect((Screen.width/2-TextureForButton.width/2-140),(Screen.height/2-TextureForButton.height/2+60),TextureForButton.width,TextureForButton.height),TextureForButton))
						{
							showGUI = false;
							Time.timeScale = 1;
							Screen.lockCursor = false;
							GameObject.Find("Playertheif").GetComponent<MouseLook>().enabled = true;
							//Debug.Log ("click the button");
						}
					GUI.EndGroup();
					if( Time.fixedTime % m_delta_timeout == 0 )
					{
						showGUI = false;	
						Time.timeScale = 1;
						Screen.lockCursor = false;
					}
				}
				if( m_type == 9 )
				{
					pausethegame();
					
					TextureForBackground =Resources.Load("Textures/MessageBox") as Texture2D;
					TextureForButton = Resources.Load("Textures/OK_Button") as Texture2D;
					TextureForMessage = Resources.Load("Textures/Message1") as Texture2D;
					GUI.BeginGroup(new Rect((Screen.width/2-TextureForBackground.width/2),(Screen.height/2-TextureForBackground.height/2),TextureForBackground.width,TextureForBackground.height),TextureForBackground);
					GUI.skin = MessagingGUISkin;
					GUI.Box(new Rect((Screen.width/2-TextureForMessage.width/2),(Screen.height/2-TextureForMessage.height/2),TextureForMessage.width,TextureForMessage.height),TextureForMessage);
					/*if(GUI.Toggle(Rect(0,0,20,20),false,"Do not Need the Tutorial"))
					{
						TutorialToggle =false;
						
					}*/
					if(GUI.Button(new Rect((Screen.width/2-TextureForButton.width/2-140),(Screen.height/2-TextureForButton.height/2+60),TextureForButton.width,TextureForButton.height),TextureForButton))
						{
							showGUI = false;
							Time.timeScale = 1;
							Screen.lockCursor = false;
							GameObject.Find("Playertheif").GetComponent<MouseLook>().enabled = true;
							//Debug.Log ("click the button");
						}
					GUI.EndGroup();
					if( Time.fixedTime % m_delta_timeout == 0 )
					{
						showGUI = false;	
						Time.timeScale = 1;
						Screen.lockCursor = false;
					}
				}
				if( m_type == 10 )
				{
					pausethegame();
					
					TextureForBackground =Resources.Load("Textures/MessageBox") as Texture2D;
					TextureForButton = Resources.Load("Textures/OK_Button") as Texture2D;
					TextureForMessage = Resources.Load("Textures/type10") as Texture2D;
					GUI.BeginGroup(new Rect((Screen.width/2-TextureForBackground.width/2),(Screen.height/2-TextureForBackground.height/2),TextureForBackground.width,TextureForBackground.height),TextureForBackground);
					GUI.skin = MessagingGUISkin;
					GUI.Box(new Rect((Screen.width/2-TextureForMessage.width/2),(Screen.height/2-TextureForMessage.height/2),TextureForMessage.width,TextureForMessage.height),TextureForMessage);
				
					if(GUI.Button(new Rect((Screen.width/2-TextureForButton.width/2-140),(Screen.height/2-TextureForButton.height/2+60),TextureForButton.width,TextureForButton.height),TextureForButton))
						{
							showGUI = false;
							Time.timeScale = 1;
							Screen.lockCursor = false;
							GameObject.Find("Playertheif").GetComponent<MouseLook>().enabled = true;
							//Debug.Log ("click the button");
							GameObject.Find("TriggersAtPositions9").GetComponent<Triggers>().enabled =false;
						}
					GUI.EndGroup();
					if( Time.fixedTime % m_delta_timeout == 0 )
					{
						showGUI = false;	
						Time.timeScale = 1;
						Screen.lockCursor = false;
					}
				}
				if( m_type == 11 )
				{
					BoxPosition = new Vector2 (200,200);
					M_BoxHeight =200;
					M_BoxWidth = 200;
					GUI.Box(new Rect(BoxPosition.x,BoxPosition.y,M_BoxHeight,M_BoxWidth),TextureForMessage);
				
					if( Time.fixedTime % m_delta_timeout == 0 )
					{
						showGUI = false;	
					}
				}
				if( m_type == 12 )
				{
		
					pausethegame();
					TextureForBackground =Resources.Load("Textures/background_finished") as Texture2D;
					TextureForButton = Resources.Load("Textures/restart_button") as Texture2D;
					//TextureForMessage = Resources.Load("Textures/message_caught") as Texture2D;
					GUI.BeginGroup(new Rect((Screen.width/2-TextureForBackground.width/2),(Screen.height/2-TextureForBackground.height/2),TextureForBackground.width,TextureForBackground.height),TextureForBackground);
					GUI.skin = MessagingGUISkin;
					//GUI.Box(new Rect((Screen.width/2-TextureForMessage.width/2),(Screen.height/2-TextureForMessage.height/2),TextureForMessage.width,TextureForMessage.height),TextureForMessage);
					if(GUI.Button(new Rect((Screen.width/2-TextureForButton.width/2-100),(Screen.height/2-TextureForButton.height/2),TextureForButton.width,TextureForButton.height),TextureForButton))
						{
							showGUI = false;
							Time.timeScale = 1;
							Screen.lockCursor = false;
							GameObject.Find("Playertheif(Clone)").GetComponent<MouseLook>().enabled = true;
							NetworkManager.Manager.RestartLevel();
							//Debug.Log ("click the button");
						}
					GUI.EndGroup();
					if( Time.fixedTime % m_delta_timeout == 0 )
					{
						showGUI = false;	
						Time.timeScale = 1;
						Screen.lockCursor = false;
					}
				
				}
			}
		}
	}
	
	void OnTriggerEnter( Collider other )
	{
		//if(GameManager.Manager.PlayerType == 1) // is thief
		{
			if( other.gameObject.tag == "Player" )
			{//Debug.Log("m_type = " + m_type );
				if( m_type >=1 && m_type <13 )
				{
					showGUI = true;	
					pause = true;
					NetworkManager.Manager.PauseGame(true);

				//	GameObject.Find("ddd");
				}
				GameObject.Find("MessageBox").GetComponent<MessageBox>().MessageBoxShow(2);	
			}
		}
	}	
	
	void OnTriggerExit( Collider other )
	{
		//if(GameManager.Manager.PlayerType == 1) // is thief
		{
			if( other.gameObject.tag == "Player" )
			{
				if( m_type >=1 && m_type < 13 )
				{
					showGUI = false;	
					pause = false;
				}
			}
		}
	}	
	
	// Update is called once per frame
	void Update () {
	}
}
 