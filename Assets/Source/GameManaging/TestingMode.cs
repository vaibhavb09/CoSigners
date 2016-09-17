using UnityEngine;
using System.Collections;

public class TestingMode : MonoBehaviour {
	
	public bool pausing = false;
	public bool testingMode = false;
	// Use this for initialization
	void Start () {

			//Debug.Log("game pause state at startup -"+pausing);
			if( pausing==true)
			{
				NetworkManager.Manager.PausingGame(pausing);
				NetworkManager.Manager.PausingStateChange(false);
				//pausing =false;
			}
	
	
	}
	
	public void ChangeTestMode()
	{
		testingMode=true;
	}
	public void ChangeToPause(bool i_pause)
	{
		pausing = i_pause;
	}
	// Update is called once per frame
	void Update () {
		/*
		if (Input.GetKeyDown (KeyCode.F11))
		{
			Debug.Log("press the F11 key");
			if( pausing==false)
			{	
				NetworkManager.Manager.PausingGame(pausing);
				NetworkManager.Manager.PausingStateChange(true);
			}
			else
			{
				NetworkManager.Manager.PausingGame(pausing);
				NetworkManager.Manager.PausingStateChange(false);
				//pausing =false;
			}	
		}
		
		if (Input.GetKey (KeyCode.F6))
		{
			Debug.Log("Disable the Lockdown Sequence");
			NetworkManager.Manager.ChangeTestMode();
			NetworkManager.Manager.OverrideSuccess();
			
			
		}
		if ( Input.GetKey(KeyCode.F2) )
			NetworkManager.Manager.RestartLevel();
		
		if( Input.GetKey(KeyCode.F4) )
		{
			
			NetworkManager.Manager.UnlockAllDoors();
			Debug.Log ("Open All the doors");
		}		
		if (Input.GetKey(KeyCode.F1))
		{
			
			NetworkManager.Manager.KillAllGuards();
			Debug.Log ("Kill All guards");
			//GameObject[] AllGuardD;
			// DGetComponent
			//var  AllGuardD;
			//AllGuardD = GameObject[];
			//AllGuardD =GameObject.FindGameObjectsWithTag("Guard");
			//var AllGuard = GameObject.FindWithTag("Guard");
			//Destroy(AllGuard);	
		}
		if (Input.GetKey(KeyCode.F12))
		{
			//var AllGuard2= GameObject.FindWithTag("Guard");
		    //AllGuard2.GetComponent<NavMeshAgent>().enabled = false;
			//AllGuard2.GetComponent<GuardActions>().enabled = false;
			//GetComponent<GuardPerception>().enabled = false;
			//AllGuard2.GetComponent<AlertMeter>().enabled = false;
			//AllGuard2.GetComponent<AlertSystem>().enabled = false;
			//AllGuard2.GetComponent<GuardBehaviourScript>().enabled = false;
			//AllGuard2.GetComponent<GuardSync>.enabled =false;
			NetworkManager.Manager.DisableDrones();
		
		}
		if (Input.GetKey (KeyCode.F9))
		{
			NetworkManager.Manager.AllPointsAccessable();
			Debug.Log("All Points Accessable");
			//Transmitter_Prefab(Clone)
			//GameObject.Find("HexGrid").GetComponent<Transmitter>().TransmitterAddRadius();
			//GameObject.Find("HexGrid").GetComponent<HexGrid>().AddRange();
		}
		if (Input.GetKey (KeyCode.F7))
		{
			NetworkManager.Manager.ResetHackerThreatMeter();
			Debug.Log("change the HackThreat to 0");
		}
		if (Input.GetKey (KeyCode.Keypad1))
		{
			NetworkManager.Manager.SetSecurityClearance(1);
			Debug.Log("Set SecurityClearance to 1");
		}
		if (Input.GetKey (KeyCode.Keypad2))
		{
			NetworkManager.Manager.SetSecurityClearance(3);
			Debug.Log("Set SecurityClearance minus 3");
		}
		if (Input.GetKey (KeyCode.Keypad3))
		{
			NetworkManager.Manager.SetSecurityClearance(5);
			Debug.Log("Set SecurityClearance pluse 5");
		}
		if (Input.GetKey (KeyCode.F3))
		{
			Debug.Log("super jump hahaha!!!");
			GameObject.Find ("Playertheif(Clone)").GetComponent<CharacterMotor>().jumping.baseHeight=3;
		}
		if (Input.GetKey (KeyCode.F5))
		{
			
			NetworkManager.Manager.DisableAllTracers();
		    Debug.Log ("press the F5 key");
			
		}
		if (Input.GetKey(KeyCode.F8))
		{
			
			NetworkManager.Manager.DisableAllJammers();
			Debug.Log("press the F8 key");
			
			
		}
		if( Input.GetKey( KeyCode.F10 ) )
		{
			BasicScoreSystem.Manager.PrintAllTheData();	
		}
	
	*/
	
	}


}


