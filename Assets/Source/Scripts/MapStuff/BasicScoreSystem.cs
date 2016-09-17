using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class BasicScoreSystem : MonoBehaviour {
	
	private static BasicScoreSystem _instance;
	public bool _scoreRequested;
	
	#region VariablesToBeWrittenToFile
	//Thief
	private int m_nTimesSeen, m_nTimesChased, m_nTimesAvoided, m_GuardAwarenessMeter;													//Guard related
	private int m_nJammersInTheLevel, m_nJammersDisabled;																				//Jammer related
	private int m_nTransmitterRestarted, m_nTransmittersPlaced, m_nTransmitterPickedUp;													//Transmitter related
	private int m_nScramblersUsed; 																										//Scrambler related
	private int m_nTPingsSent, m_nTObjectsPinged;																						//Ping related
	private bool m_CaughtByGuard;	
	private int m_noOfSAPsActivated, m_noOfSAPsInLevel;//End-game condition
	
	//Hacker
	private int m_nLinksCount, m_nTotalLinksPossible;																					//Links related
	private int m_nDoorsUnlocked, m_nDoorsNormalLocked, m_nDoorsLocked, m_nDoorsInTheLevel;												//Door Node related
	private int m_nIRNodesCaptured, m_nIRNodesInTheLevel;																				//IR Node related
	private int m_nSecurityNodesCaptured, m_nSecurityNodesInTheLevel;																	//Security Node related
	private int m_nHPingsSent, m_nHObjectsPinged;																						//Ping related
	private int m_CaughtByTracer;																										//End-game condition
	
	//Common
	private int m_PasswordsInTheLevel, m_PasswordsObtained;																				//Password related
	private int m_LockdownsNeeded;
	private int m_LevelCompleteTime;
	//Lockown related
	#endregion
	
	public static BasicScoreSystem Manager
	{
		get
		{
			if(_instance == null)
			{
				_instance = new BasicScoreSystem();			
			}
			return _instance;
		}
	}
		
	public BasicScoreSystem() 
    { 
        _instance = this;
    }

	#region CommonProperties
	public int LevelCompleteTime
	{
		get{
			return m_LevelCompleteTime;
		}
		set{
			this.m_LevelCompleteTime = value;
		}
	}

	public int PasswordsInTheLevel
	{
		get{
			return m_PasswordsInTheLevel;
		}
		set{
			this.m_PasswordsInTheLevel = value;
		}
	}
	
	public int PasswordsObtained
	{
		get{
			return m_PasswordsObtained;
		}
		set{
			this.m_PasswordsObtained = value;
		}
	}
	
	public int LockdownsNeeded
	{
		get{
			return m_LockdownsNeeded;
		}
		set{
			this.m_LockdownsNeeded = value;
		}
	}
	#endregion
	
	
	#region ThiefProperties

	public int NoOfSAPsActivated
	{
		get{
			return m_noOfSAPsActivated;
		}
		set{
			this.m_noOfSAPsActivated = value;
		}
	}
	public int NoOfSAPsInLevel
	{
		get{
			return m_noOfSAPsInLevel;
		}
		set{
			this.m_noOfSAPsInLevel = value;
		}
	}

	public bool ThiefCaughtByGuard
	{
		get{
			return m_CaughtByGuard;
		}
		set{
			this.m_CaughtByGuard = value;
		}
	}
	
	//Guard related
	public int TimesSeen
	{
		get{
			return m_nTimesSeen;
		}
		set{
			this.m_nTimesSeen = value;
		}
	}
	
	public int TimesChased
	{
		get{
			return m_nTimesChased;
		}
		set{
			this.m_nTimesChased = value;
		}
	}
	
	public int TimesAvoided
	{
		get{
			return m_nTimesAvoided;
		}
		set{
			this.m_nTimesAvoided = value;
		}
	}
	
	public int GuardAwarenessMeter
	{
		get{
			return m_GuardAwarenessMeter;
		}
		set{
			this.m_GuardAwarenessMeter = value;
		}
	}
	
	//Jammer related
	public int TotalJammers
	{
		get{
			return m_nJammersInTheLevel;
		}
		set{
			this.m_nJammersInTheLevel = value;
		}
	}
	
	public int JammersDisabled
	{
		get{
			return m_nJammersDisabled;
		}
		set{
			this.m_nJammersDisabled = value;
		}
	}
	
	//Transmitter related
	public int TransmittersPlaced
	{
		get{
			return m_nTransmittersPlaced;
		}
		set{
			this.m_nTransmittersPlaced = value;
		}
	}
	
	public int TransmittersRestarted
	{
		get{
			return m_nTransmitterRestarted;
		}
		set{
			this.m_nTransmitterRestarted = value;
		}
	}
	
	public int TransmittersPickedUp
	{
		get{
			return m_nTransmitterPickedUp;
		}
		set{
			this.m_nTransmitterPickedUp = value;
		}
	}
	
	//Scrambler related
	public int ScramblerUsed
	{
		get{
			return m_nScramblersUsed;
		}
		set{
			this.m_nScramblersUsed = value;
		}
	}
	
	//Ping related
	public int PingsSentByTheif
	{
		get{
			return m_nTPingsSent;
		}
		set{
			this.m_nTPingsSent = value;
		}
	}
	
	public int ObjectPingsSentByThief
	{
		get{
			return m_nTObjectsPinged;
		}
		set{
			this.m_nTObjectsPinged = value;
		}
	}
	#endregion
	
	#region HackerRelated
	public int HackerCaughtByTracer
	{
		get{
			return m_CaughtByTracer;
		}
		set{
			this.m_CaughtByTracer = value;
		}
	}
	
	//Links related
	public int LinksPlaced
	{
		get{
			return m_nLinksCount;
		}
		set{
			this.m_nLinksCount = value;
		}
	}
	
	public int TotalLinksPossible
	{
		get{
			return m_nTotalLinksPossible;
		}
		set{
			this.m_nTotalLinksPossible = value;
		}
	}
	
	//Door node related
	public int DoorsUnlocked
	{
		get{
			return m_nDoorsUnlocked;
		}
		set{
			this.m_nDoorsUnlocked = value;
		}
	}
	
	public int DoorsLocked
	{
		get{
			return m_nDoorsLocked;
		}
		set{
			this.m_nDoorsLocked = value;
		}
	}
	
	public int DoorsNormalLocked
	{
		get{
			return m_nDoorsNormalLocked;
		}
		set{
			this.m_nDoorsNormalLocked = value;
		}
	}
	
	public int TotalDoors
	{
		get{
			return m_nDoorsInTheLevel;
		}
		set{
			this.m_nDoorsInTheLevel = value;
		}
	}
	
	//IR Nodes related
	public int IRNodesCaptured
	{
		get{
			return m_nIRNodesCaptured;
		}
		set{
			this.m_nIRNodesCaptured = value;
		}
	}
	
	public int TotalIRs
	{
		get{
			return m_nIRNodesInTheLevel;
		}
		set{
			this.m_nIRNodesInTheLevel = value;
		}
	}
	
	//Security Node related
	public int SecurityNodesCaptured
	{
		get{
			return m_nSecurityNodesCaptured;
		}
		set{
			this.m_nSecurityNodesCaptured = value;
		}
	}
	
	public int TotalSecurityNodes
	{
		get{
			return m_nSecurityNodesInTheLevel;
		}
		set{
			this.m_nSecurityNodesInTheLevel = value;
		}
	}
	
	//Pings related
	public int PingsSentByHacker
	{
		get{
			return m_nHPingsSent;
		}
		set{
			this.m_nHPingsSent = value;
		}
	}
	
	public int ObjectsPingedByHacker
	{
		get{
			return m_nHObjectsPinged;
		}
		set{
			this.m_nHObjectsPinged = value;
		}
	}
	#endregion
	
	// Use this for initialization
	void Start () {
		//Thief
		TimesSeen = TimesChased = TimesAvoided = GuardAwarenessMeter = JammersDisabled = TransmittersPlaced = TransmittersPickedUp = TransmittersRestarted = 0;
		m_noOfSAPsActivated = m_noOfSAPsInLevel = 0;
		ScramblerUsed = PingsSentByTheif = ObjectPingsSentByThief = 0;
		
		//Hacker
		LinksPlaced = DoorsLocked = DoorsUnlocked = DoorsNormalLocked = SecurityNodesCaptured = IRNodesCaptured = PingsSentByHacker = ObjectsPingedByHacker = m_CaughtByTracer = 0;

		
		UnityEngine.Object[] tempSAPs;
		tempSAPs = GameObject.FindGameObjectsWithTag("SAP");
		//Debug.Log (" tempSAPs.GetLength - "+ tempSAPs.Length);
		foreach (UnityEngine.Object tempSAP in tempSAPs) 
		{
			m_noOfSAPsInLevel++;
		}
		
		//_scoreRequested = true;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	public void PrintAllTheData()
	{
		//if( _scoreRequested )
		{
			//Debug.Log("Printing out data");
			System.IO.File.WriteAllText( "Assets/SaveFile.txt", ( "Number of Times Seen: "+ TimesSeen + "\nNumber of Times Chased: " + TimesChased + 
				"\nNumber of Times Avoided: " + TimesAvoided + "\nGuard Awareness Meter: " + GuardAwarenessMeter + 
				"\nJammers Disabled: "+ JammersDisabled + "\nTransmeters Placed: " + TransmittersPlaced + "\nTransmeters Restarted: " + TransmittersRestarted + 
				"\nTransmitters Picked Up: " + TransmittersPickedUp + "\nScramblers Used: " +ScramblerUsed + "\nPings Sent by Thief: " + PingsSentByTheif + 
				"\nObjects Pinged by Thief: " + ObjectPingsSentByThief + "\nLinks Placed: " + LinksPlaced 
				+ "\nDoors Unlocked: " + DoorsUnlocked + "\nDoors Locked: " + DoorsLocked + "\nDoors Normal Locked: " + DoorsNormalLocked + 
				"\nIR Nodes Caputred: " + IRNodesCaptured + "\nSecurity Nodes Captured: " +	SecurityNodesCaptured + 
				"\nPings Sent By Hacker: " + PingsSentByHacker + "\nObjects Pinged by Hacker: " + ObjectsPingedByHacker + "Lockdowns this level: " + LockdownsNeeded ) );
			//_scoreRequested = false;
		}
	}
	
	public void ResetData()
	{
		PasswordsObtained = 0;
		PasswordsInTheLevel = 0;
		ThiefCaughtByGuard = false;
		TimesSeen = 0;
		TimesChased = 0;
		TimesAvoided = 0;
		GuardAwarenessMeter = 0;
		TotalJammers = 0;
		JammersDisabled = 0;
		TransmittersPlaced = 0;
		TransmittersRestarted = 0;
		TransmittersPickedUp = 0;
		ScramblerUsed = 0;
		PingsSentByTheif = 0;
		ObjectPingsSentByThief = 0;
		PingsSentByHacker = 0;
		ObjectsPingedByHacker = 0;
		HackerCaughtByTracer = 0;
		LinksPlaced = 0;
		DoorsLocked = 0;
		DoorsNormalLocked = 0;
		DoorsUnlocked = 0;
		IRNodesCaptured = 0;
		SecurityNodesCaptured = 0;
		LockdownsNeeded = 0;
	}
	
	public int ReturnStars()
	{
		if( LockdownsNeeded == 0 )
			return 3;
		else if( LockdownsNeeded == 1 )
			return 2;
		else if( LockdownsNeeded == 2 )
			return 1;
		else
			return 0;
	}
}
