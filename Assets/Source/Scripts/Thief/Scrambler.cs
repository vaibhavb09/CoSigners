using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Scrambler : MonoBehaviour {
	
	private enum ScramblerStates{ opening=0, looping=1, closing=2, Dead=3 }
	private int hexIndex;
	public float scrambleTime;
	private ScramblerStates myState;
	
#if UNITY_IPHONE
	
	void Start(){}
	void Update(){}
	public void Set(int i_hexIndex, List<int> i_checkHexes){}
	public void SelfDestroy(){}
	
#else
	
	public void Set(int i_hexIndex, List<int> i_checkHexes)
	{
		scrambleTime=30.0f;
	    hexIndex=i_hexIndex;
		myState=ScramblerStates.opening;
		gameObject.GetComponentInChildren<Animation>().animation.Play("opening");
		
		
		// [SOUND TAG] [Scrambler_Sound_loop]
		// If this is the theif then play this sound
		if(GameManager.Manager.PlayerType == 1)
			soundMan.soundMgr.playOnSource(this.audio,"Scrambler_Sound_loop",true,GameManager.Manager.PlayerType);
		
		//Find tracer around the hex grid in one hex radius
		for(int i=0;i<i_checkHexes.Count;i++)
		{
		  SecurityManager.Manager.ScrambleTracerAtHexIndex(i_checkHexes[i]);	
			
		}
	}
	
 
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
		scrambleTime-=Time.deltaTime;
	
		
		if(!gameObject.GetComponentInChildren<Animation>().animation.IsPlaying("opening") && myState==ScramblerStates.opening)
		{
			//Debug.Log("open");
			myState=ScramblerStates.looping;
			gameObject.GetComponentInChildren<Animation>().animation.Play("loop");
			gameObject.GetComponentInChildren<Animation>().animation.wrapMode=WrapMode.Loop;
		}
		else if(!gameObject.GetComponentInChildren<Animation>().animation.IsPlaying("closing") && myState==ScramblerStates.closing)
		{
			//Debug.Log("close now");
			myState=ScramblerStates.Dead;
			SecurityManager.Manager.DeScrambleTracers(hexIndex);
			gameObject.GetComponentInChildren<Animation>().animation.Stop();
			
			//notify Thief
			ThiefGrid.Manager.RemoveScrambler(hexIndex);
			
			//notify hacker
			NetworkManager.Manager.RemoveScrambler(hexIndex);
			
		}
		
		else if(scrambleTime<=0 && myState==ScramblerStates.looping)
		{
			// [SOUND TAG] [Scrambler_Disarmed]
			// If this is the theif then play this sound
			if(GameManager.Manager.PlayerType == 1)
				soundMan.soundMgr.playOneShotOnSource(this.audio,
							"Scrambler_Disarmed",GameManager.Manager.PlayerType);
			
			//Debug.Log("started closing");
			myState=ScramblerStates.closing;
			gameObject.GetComponentInChildren<Animation>().animation.wrapMode=WrapMode.Once;
			gameObject.GetComponentInChildren<Animation>().animation.Play("closing");
		}
		
	}
	
	
	public void SelfDestroy()
	{
		
	}
	
#endif
	
}
