using UnityEngine;
using System.Collections;

public class ConnectionTesterNAT : Photon.MonoBehaviour {
	string testStatus = "Testing network connection capabilities.";
	string testMessage = "Test in progress";
	string shouldEnableNatMessage = "";
	public static bool doneTesting = false;
	bool probingPublicIP = false;
	int serverPort = 25566;
	float timer = 0.0f;
	ConnectionTesterStatus connectionTestResult = ConnectionTesterStatus.Undetermined;
	float _currentTimer= 0.0f;
	// Indicates if the useNat parameter be enabled when starting a server
	bool useNat = false;
	
	
	// Use this for initialization
	void Start () {
		
	}
	
	
	void OnGUI()
	{
		GUILayout.Label("Current Status: " + testStatus);
		GUILayout.Label("Test result : " + testMessage);
		GUILayout.Label(shouldEnableNatMessage);
		if (!doneTesting)
			TestConnection();
		if(doneTesting == true)
		{
			_currentTimer += Time.deltaTime;
			if(_currentTimer > 2.0f)
			{
				doneTesting=false;
				_currentTimer = 0.0f;
			}
		}
	}
	
	void TestConnection()
	{/*
		//Debug.Log(connectionTestResult);
		connectionTestResult = PhotonNetwork.TestConnectionNAT();
		switch (connectionTestResult)
		{
		case ConnectionTesterStatus.Error: 
			testMessage = "Problem determining NAT capabilities";
			doneTesting = true;
			break;
			
		case ConnectionTesterStatus.Undetermined: 
			testMessage = "Undetermined NAT capabilities";
			doneTesting = false;
			break;
			
		case ConnectionTesterStatus.PublicIPIsConnectable:
			testMessage = "Directly connectable public IP address.";
			useNat = false;
			doneTesting = true;
			break;
			
			// This case is a bit special as we now need to check if we can 
			// circumvent the blocking by using NAT punchthrough
		case ConnectionTesterStatus.PublicIPPortBlocked:
			testMessage = "Non-connectable public IP address (port " +
				serverPort +" blocked), running a server is impossible.";
			useNat = false;
			// If no NAT punchthrough test has been performed on this public 
			// IP, force a test
			if (!probingPublicIP) {
				connectionTestResult = PhotonNetwork.TestConnectionNAT();
				probingPublicIP = true;
				testStatus = "Testing if blocked public IP can be circumvented";
				timer = Time.time + 10;
			}
			// NAT punchthrough test was performed but we still get blocked
			else if (Time.time > timer) {
				probingPublicIP = false; 		// reset
				useNat = true;
				doneTesting = true;
			}
			break;
		case ConnectionTesterStatus.PublicIPNoServerStarted:
			testMessage = "Public IP address but server not initialized, "+
				"it must be started to check server accessibility. Restart "+
					"connection test when ready.";
			break;
			
		case ConnectionTesterStatus.LimitedNATPunchthroughPortRestricted:
			testMessage = "Limited NAT punchthrough capabilities. Cannot "+
				"connect to all types of NAT servers. Running a server "+
					"is ill advised as not everyone can connect.";
			useNat = true;
			doneTesting = true;
			break;
			
		case ConnectionTesterStatus.LimitedNATPunchthroughSymmetric:
			testMessage = "Limited NAT punchthrough capabilities. Cannot "+
				"connect to all types of NAT servers. Running a server "+
					"is ill advised as not everyone can connect.";
			useNat = true;
			doneTesting = true;
			break;
			
		case ConnectionTesterStatus.NATpunchthroughAddressRestrictedCone:
			testMessage = "NAT punchthrough capable. Can connect to all "+
				"servers and receive connections from all clients. Enabling "+
					"NAT punchthrough functionality.";
			useNat = true;
			doneTesting = true;
			break;
		case ConnectionTesterStatus.NATpunchthroughFullCone:
			testMessage = "NAT punchthrough capable. Can connect to all "+
				"servers and receive connections from all clients. Enabling "+
					"NAT punchthrough functionality.";
			useNat = true;
			doneTesting = true;
			break;
			
		default: 
			testMessage = "Error in test routine, got " + connectionTestResult;
			break;
		}
		if (doneTesting) {
			if (useNat)
				shouldEnableNatMessage = "When starting a server the NAT "+
					"punchthrough feature should be enabled (useNat parameter)";
			else
				shouldEnableNatMessage = "NAT punchthrough not needed";
			testStatus = "Done testing";
		}*/
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
