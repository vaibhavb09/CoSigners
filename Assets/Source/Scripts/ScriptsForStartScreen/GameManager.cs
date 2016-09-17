using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum TextFocus
{
	TextChat = 1,
	PasswordText
}

public struct AccessPoint
{
	public int ID;
	public string path;
};

public struct InfoNodeData
{
	public string 	m_currentMovie;
	public bool	   	m_hackerReady;
	public bool 	m_thiefReady;
}

public class GameManager
{
	private static GameManager m_instance;
	private int m_playerType = 0;
	public static int ID = 0;
	public bool isStartUpFirstTime = true;
	public bool EnableLineShown = false;
	public bool EnableLineShownWhenSelected = false;
	public bool AnotherMenuOpened = false;
	public bool clickLock = false;
	public bool HackerCaught = false;
	public List<string> LevelNames = new List<string>();
	public List<AccessPoint> MasterAccessPoints = new List<AccessPoint>();
	public int m_roleReady = 0;
	public InfoNodeData _currentInfoData;
	public bool	m_isInfoNodeSourceMuted = false;

	public bool HackerReady = false;
	public bool ThiefReady = false;
	public bool LevelStarted = false;
	public bool InStartMenu = true;

	/** For Level Transition **/
	public bool playClicked = false, levelClicked = false, exitClicked = false;

	public Texture2D CurrentLevelTexture = null;
	public string CurrentLevelName = "";
	public string NextLevelName = "";
	public enum ButtonType
	{
		Capture,
		Release,  
		Encrypt,
		Jam,
		Unlock,
		View,
		Stopview,
		Cancle,
		TopView,
		MidView,
		BotView,
	}
	
	public enum NodeState
	{
		// Indicates a node that is controlled by the hacker
		Controlled = 1,
		// Indicates a node that is encrypted by the hacker
		//Encrypted = 2,
		// [NOT IN USE ? 4 / 16 / 2013]
		//Secured = 3,
		// Indicates a node that is accessible but not controlled by the hacker
		Neutral = 4,
		// Indicates a node that is not accessible by the hacker
		// 1. Above security clearance
		Unreachable = 5,
		// Indicates a node that is being captured by the hacker
		//Capturing = 6,
		// Indicates a node that is being released by the hacker
		//Releasing = 7,
		// Indicates a node that is being encrypted by the hacker
		//Encrypting = 8,
		// Indicates a node that was released but can be connected by connecting to one end of the subnetwork
		//	Nodes go in this state if
		// Subnetwork was terminated by security system or released by hacker
		//Deactivated=9,
		// Indicates a node that is inaccessible to the hacker and security system
		//PoweredOff=10,
		// Indicates a node that is inaccessible to the hacker because of security clearance
		aboveSecurityClearance=11,
		// Indicates a camera thats being used
		Viewing = 16,
	}
	
	public enum SecurityState
	{
		NotHere = 1,
		Tracking = 2,
		HackerDetected = 3,
		EMPDeactivation=4,
	}
	
	public enum Layer
	{
		Map = 1,
		Node = 2,
		ALL = 3,
	}
	
	public enum NodeType
	{
		Door = 1,
		SecurityCamera = 2,
		InfraRed = 3,
		Password = 4,
		Energy = 5,
	}
	
	public enum TimerType
	{
		Capture = 1,
		Encrypt = 2,
		Release = 3,
		Tracking = 4,
	}
	
	public enum ViewPosition
	{
		Top,
		Mid,
		Bot,
		None,
	}
	
	public Layer CurrentDisplay = Layer.ALL;
	
	//public string path = Application.dataPath.Substring (0, Application.dataPath.Length - 20 )+"Documents";
	public string path = Application.persistentDataPath.Substring (0, Application.persistentDataPath.Length );

	public int PlayerType
	{
		get
		{
			return m_playerType;
		}
		set
		{
			m_playerType = value;
		}
	}
	
	public GameManager () 
    {
        if (m_instance != null)
        {
            return;
        }

		_currentInfoData.m_currentMovie = "";
		_currentInfoData.m_hackerReady = false;
		_currentInfoData.m_thiefReady = false;
 
        m_instance = this;
    }
	
	public static GameManager Manager
	{
		get
		{
			if(m_instance == null)
			{
				m_instance = new GameManager();			
			}
			return m_instance;
		}
	}
	
	
	public Node NodeToView = null;
	
	/* doesn't need this
	private string m_playerName  = "";
	
	//public variables
	public string PlayerName
	{
		get
		{
			return m_playerName;
		}
		set
		{
			m_playerName = value;
		}
	}
	*/
	
}
