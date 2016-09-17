using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ThiefManager : MonoBehaviour {
	
	private static ThiefManager m_instance;
	
	public GameObject playerThief;
	public int maxHealth;
	private int currentHealth;
	private int transmitterCount;
	public int maxTransmitterCount;
	public bool gameIsPaused;
	//Amount of threat you want to bump up when a guard sees you.
	public float AlertDamage { get; set; }
	public TextFocus CurrentFocus { get; set; }

	// Use this for initialization
	void Start () 
	{
		gameIsPaused=false;
		CurrentFocus = TextFocus.TextChat;
	}
	
	public void Load( int i_tCount )
	{ 
		transmitterCount = i_tCount;
		maxTransmitterCount = i_tCount;
		playerThief =GameObject.Find("Playertheif(Clone)");
	}
		
	public static ThiefManager Manager
	{
		get
		{
			if(m_instance == null)
			{
				m_instance = new ThiefManager();			
			}
			return m_instance;
		}
	}
	
	public ThiefManager () 
    { 
        m_instance = this;
		currentHealth = maxHealth;
    }
	
	public int GetTransmitterCount()
	{
		return transmitterCount;
	}
	
	public void IncrementTransmitterCount()
	{
		transmitterCount++;
	}
	
	public void DecrementTransmitterCount()
	{
		transmitterCount--;
	}
	
	public bool IsTransmitterCountZero()
	{
		return (transmitterCount == 0);
	}
	
	public int GetCurrentHealth()
	{
		return currentHealth;
	}
	
	public void IncrementHealthBy( int value )
	{
		currentHealth += value;
		if( currentHealth > maxHealth )
		currentHealth = maxHealth;
	}
	
	public void DecrementHealthBy( int value )
	{
		currentHealth -= value;
		if( currentHealth <= 0 )
			KillPlayer();
	}
	
	public bool IsHealthFull()
	{
		return (currentHealth == maxHealth);
	}
	
	public void KillPlayer()
	{
		//Debug.Log( "Player dead" ); 
		//Kill animation or death message...
	}
	
	public void DisableThiefActions()
	{
		Screen.lockCursor = false;
		playerThief.GetComponent<MouseLookAround>().enabled = false;
		playerThief.GetComponent<FPSInputController>().enabled = false;
		playerThief.GetComponent<ThiefActions>().DisableInput();	
		//playerThief.GetComponent<MovementScript>().moveEnabled = false;
		playerThief.GetComponent<CharacterMotor>().enabled = false;
	}
	public void EnableThiefActions()
	{
		Screen.lockCursor = true;
		playerThief.GetComponent<MouseLookAround>().enabled = true;
		playerThief.GetComponent<FPSInputController>().enabled = true;
		playerThief.GetComponent<ThiefActions>().EnableInput();	
		//playerThief.GetComponent<MovementScript>().moveEnabled = true;
		playerThief.GetComponent<CharacterMotor>().enabled = true;
	}

	public void PauseGame()
	{
		if(GameManager.Manager.PlayerType == 1 ) //only thief can do this
		{
			//Debug.Log("Pausing Thieeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeef");
			gameIsPaused=true;
			soundMan.soundMgr.PauseGame(GameManager.Manager.PlayerType);
			playerThief.GetComponent<ThiefActions>().PauseGame();
			GuardOverlord.Manager.pauseAllGuards();

			//pause traces in all ways: new and old
			SecurityManager.Manager.DisableTracerCreators();

			DisableThiefActions();
			//NetworkManager.Manager.PauseGame(gameIsPaused);
		}
	}

	public void PlayInformation( int i_ID )
	{
		InfoNodeManager.Manager._currentInfoNodeID = i_ID;
		InfoNodeManager.Manager.PlayInformation(i_ID);
	}
	
	public void UnPauseGame()
	{
		if(GameManager.Manager.PlayerType == 1 ) //only thief can do this
		{
			gameIsPaused=false;
			soundMan.soundMgr.UnPauseGame(GameManager.Manager.PlayerType);
			playerThief.GetComponent<ThiefActions>().unPauseGame();
			GuardOverlord.Manager.resumeAllGuards();

			//unpause traces in all ways: new and old
			SecurityManager.Manager.EnableTracerCreators();

			EnableThiefActions();
			//NetworkManager.Manager.PauseGame(gameIsPaused);
		}
	}

	// Update is called once per frame
	void Update () 
	{
		//for debugging pause; to be removed
		if( Input.GetKeyDown( KeyCode.P ) )
		{
			NetworkManager.Manager.PauseGame(true);
			//PauseGame();
		}
		else if (Input.GetKeyDown(KeyCode.U))
		{
			NetworkManager.Manager.PauseGame(false);//UnPauseGame();
		}
		//till here
	}
}
