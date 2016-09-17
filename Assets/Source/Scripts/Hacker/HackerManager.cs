using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class HackerManager : MonoBehaviour {
	
	private GameObject hackerTopCamera;
	public float pingCooldown;
	
	private static 		HackerManager _instance;
	public TextFocus	CurrentFocus { get; set; }
	private int 		hackerClearance;
	private bool 		pingReady;
	private bool 		pingActive;
	private GenericTimer pingTimer;
	private Transform 	pingAnimation;
	private Transform 	pingAnimation_Hex;
	public Transform 	objectPingAnimation;
	private bool 		pingOn;				// represents if the ping state is turned on in which case all clicks will create pings.
	public int			powerUsage;
	public int			powerCapacity;
	
	public Transform pingSentIndicator;
	public Transform pingSentIndicator_Hex;
	public Transform pingRecievedIndicator;
	
	private bool isThiefObjectPingOn = false;
	private float timeThiefObjectPing = 0.0f;
	public bool gameEnded;
	public bool gameIsPaused;
	public bool legendOpen;

	public static HackerManager Manager
	{
		get
		{
			if(_instance == null)
			{
				_instance = new HackerManager();			
			}
			return _instance;
		}
	}
	
	public int HackerClearance
	{
		get{
			return hackerClearance;	
		}
		set{
			this.hackerClearance = value;
		}
	}
	
	public bool PingOn
	{
		get{
			return pingOn;
		}
	}
	
	#region Constructor
	public HackerManager () 
    { 
        _instance = this;
    }
	#endregion
	
	// Use this for initialization
	void Start () 
	{
		hackerTopCamera = GameObject.Find("TopDownCamera");
		pingReady = true;
		pingActive = false;
		pingOn = false;
		powerUsage = 0;
		gameEnded = false;
		gameIsPaused = false;
		legendOpen = SetLegendStartState();
		CurrentFocus = TextFocus.TextChat;
		//objectPingAnimation = Resources.Load("Prefabs/Hacker/HackerObjectPing") as Transform;
	}
	
	// Update is called once per frame
	void Update () 
	{
	}
	
	private bool SetLegendStartState()
	{
		if ( Application.loadedLevelName.Equals ("TL_01") ||
		    Application.loadedLevelName.Equals ("TL_03") ||
		    Application.loadedLevelName.Equals ("TL_05") ||
		    Application.loadedLevelName.Equals ("TL_06"))
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	public bool PingReady()
	{
		return pingReady;
	}
	
	public void CreatePing( Vector3 i_worldPos, int i_hexIndex )
	{
		//Debug.Log ("I made a Ping at:" + i_hexIndex);
		pingActive = true;
		
		// Play my ping sound
		// AudioManager.Manager.HackerSentPing( true );
		
		// [Sound Tag] [Ping] [Hacker pinged point man]
		soundMan.soundMgr.playOneShotOnSource(null,"Ping",GameManager.Manager.PlayerType,2);
		
		BasicScoreSystem.Manager.PingsSentByHacker += 1;
		
		i_worldPos.y = 60.0f;
		pingAnimation = (Transform) Instantiate(pingSentIndicator, HexGrid.Manager.GetCoordHex(i_hexIndex, 60), Quaternion.identity);
		pingAnimation_Hex = (Transform) Instantiate(pingSentIndicator_Hex, HexGrid.Manager.GetCoordHex(i_hexIndex, 60), Quaternion.identity);
		
	}
		
	
	public void RecievePing( int i_hexIndex )
	{
		// AudioManager.Manager.HackerRecievedPing( true );
		
		Vector3 pingHexPos = HexGrid.Manager.GetCoordHex( i_hexIndex, 60.0f );
		Transform pingRecieved = (Transform) Instantiate(pingRecievedIndicator, pingHexPos, Quaternion.identity);
		
		// Hacker pinged by point man [SOUND TAG] [Ping]
		soundMan.soundMgr.playOneShotOnSource(null,"Ping",GameManager.Manager.PlayerType,2);
	}
	
	public void CreateThiefObjectPing( float rotation, float xScale, float zScale, float xPosition, float zPosition )
	{
		Transform pingObj = (Transform)Instantiate( objectPingAnimation, Vector3.zero, Quaternion.identity );
		//AudioManager.Manager.HackerRecievedPing( true );
		
		if( pingObj != null )
		{
			pingObj.localScale = new Vector3( xScale, 0.1f, zScale );
			
			pingObj.position = new Vector3( xPosition, 60.0f, zPosition );
			
			pingObj.transform.Rotate(Vector3.up, rotation);
			
			// Hacker Object ping [SOUND TAG] [Ping]
			// play only for hacker
			if(GameManager.Manager.PlayerType == 2)
				soundMan.soundMgr.playOneShotOnSource(null,"Ping",GameManager.Manager.PlayerType,2);
		}
	}
	
	public bool CheckHackerClearance( int i_nodeClearance )
	{
		if( HackerClearance >= i_nodeClearance )
		{
			return true;	
		}
		return false;
	}
	
	public void HackerDetected()
	{
		//Debug.Log ("GAME OVER");
	}
	
	public void SetMaxPowerCapacity( int i_pCap )
	{
		powerCapacity = i_pCap*10;
	}
	
	public void RefreshPower()
	{
		if ( GameManager.Manager.PlayerType == 2 )
		{
			powerUsage = ConnectionManager.Manager.ConectedCount;
			powerUsage += GraphManager.Manager.PowerUsage;
			
			//SetThreatRate();
		}
	}
	public void DisableHackerActions()
	{
		if (hackerTopCamera == null)
			hackerTopCamera = GameObject.Find("TopDownCamera");
		
		hackerTopCamera.GetComponent<HackerActions>().DisableHackerActions();
	}
	
	public void EnableHackerActions()
	{
		if (hackerTopCamera == null)
			hackerTopCamera = GameObject.Find("TopDownCamera");
		
		hackerTopCamera.GetComponent<HackerActions>().EnableHackerActions();
	}
	public bool isHackerActionsDisabled()
	{
		return hackerTopCamera.GetComponent<HackerActions>()._disabled;
	}
	public void DisableESCMenu()
	{
		if(hackerTopCamera == null)
		{
			hackerTopCamera = GameObject.Find("TopDownCamera");
		}
		if(hackerTopCamera.GetComponent<HackerGUI>()._showESC)
			hackerTopCamera.GetComponent<HackerGUI>()._showESC = false;			
	}

	public void PauseGame(bool pause)
	{
		if(GameManager.Manager.PlayerType == 2 ) //only hacker can do this
		{
			if(pause)
			{
				//Debug.Log("Pausing Haccccccccccccccccccccccccccckker");
				gameIsPaused = true;
				//Time.timeScale=0;
				//hackerTopCamera.GetComponent<HackerActions>().PauseGame();
				soundMan.soundMgr.PauseGame(GameManager.Manager.PlayerType);
				GuardOverlord.Manager.pauseAllGuards();
			}
			else 
			{
				gameIsPaused = false;
				//Time.timeScale=1;
				//hackerTopCamera.GetComponent<HackerActions>().unPauseGame();
				soundMan.soundMgr.UnPauseGame(GameManager.Manager.PlayerType);
				GuardOverlord.Manager.resumeAllGuards();
			}
		}
	}

	public void OpenLegend()
	{
		legendOpen = true;
	}

	public void CloseLegend()
	{
		legendOpen = false;
	}

	/*
	private void SetThreatRate()
	{
		
		float x = 0;
		NetworkManager.Manager.ModifyThreatRate(x);
	}*/
	
	/*
	private void FadeOutObjectPing()
	{
		if( 
		PingObjects[0].PingObject.renderer.material.color.a
	}
	*/
}
