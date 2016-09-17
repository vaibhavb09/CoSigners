using UnityEngine;
using System.Collections;

public class Password : MonoBehaviour {
	private Password m_instance;
	private bool showGUI;
	private bool passwordWrong;
	private string stringToEdit;
	private Node currentNode;
	
	
	public Password () 
    {
        m_instance = this;
    }
	
	// Use this for initialization
	void Start () 
	{
		showGUI = false;
		passwordWrong = false;
		stringToEdit = "";
		currentNode = null;
	}
	
	public void Update()
	{	
	}
	
	public void GetPassword(Node i_node)
	{
		showGUI = true;
		passwordWrong = false;
		currentNode = i_node;
	}
	
	public void VerifyPassword(Node i_node, string i_passwordAttempt)
	{
		/*
		if ( i_node != null )
		{
			if ( i_passwordAttempt.Equals(currentNode.MyPassword) )
			{
				// Correct Password
				Correct();
			}
			else
			{
				// Incorrect Password
				// Should give some kind of error message.
				Incorrect();
			}
		}*/

	}
	
	public void Correct ( )
	{
		// Change Node State
		//currentNode.State_ChangeToControlled();
		//currentNode._type = GameManager.NodeType.Control;
		
		// Set Players Security Clearance Level
		//GameManager.Manager.PlayerInventory.SecurityClearance = 2;
		//GraphManager.Manager.UpdateNodeSystem();
		
		// HARDCODED TO 32 for now [STATIC SECURITY SYSTEM]
		//GraphManager.overlord.addStaticSecurityAgentAt(32);		
	}
	
	
	public void Incorrect ( )
	{	
		stringToEdit = "";
		passwordWrong = true;
		showGUI = true;
	}
	
	
	public void clearPassword ( )
	{	
		stringToEdit = "";
		passwordWrong = false;
		showGUI = false;
	}
	
	
	void OnGUI() 
	{
		if ( showGUI )
		{
			if ( !passwordWrong )
			{
	        	stringToEdit = GUI.TextField (new Rect (Screen.width/2, Screen.height/2, 200, 20), stringToEdit, 25); 
				if (GUI.Button(new Rect(Screen.width/2, Screen.height/2+30, 200, 30), "Submit Password"))
				{
	            	print(stringToEdit);
					showGUI = false;
					VerifyPassword(currentNode, stringToEdit);
				}
			}
			else
			{
				GUI.Label ( new Rect(Screen.width/2, Screen.height/2-30, 200, 20), "Incorrect Passwrod. Try Again.");
				stringToEdit = GUI.TextField (new Rect (Screen.width/2, Screen.height/2, 200, 20), stringToEdit, 25); 
				if (GUI.Button(new Rect(Screen.width/2, Screen.height/2+30, 200, 30), "Submit Password"))
				{
	            	print(stringToEdit);
					showGUI = false;
					VerifyPassword(currentNode, stringToEdit);
				}
			}
		}
    }
	
}
