using UnityEngine;
using System.Collections;

public class InitFlag : MonoBehaviour {
	
	void Awake()
	{

	}

	void OnLevelWasLoaded(int level) 
	{
		if (level != 0 && level != 1)
		{
			InitControl.StartInitialization();
		}
			//print("Woohoo");
		
	}
}
