using System.IO;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
 
public class CERTACues : MonoBehaviour
{
	List<string> triggeredSounds;
	AudioSource certaSource;

	public CERTACues()
	{

	}

	void Start () 
	{
		triggeredSounds=new List<string>();
		certaSource=GameObject.Find("InfoNodeSource").GetComponent<AudioSource>();
	}

	void OnTriggerEnter( Collider hit )
	{
		//Debug.Log("Certa will you say somethang?");
		if( GameManager.Manager.PlayerType == 1 && !ThiefManager.Manager.gameIsPaused )
		{
			if(hit.gameObject.CompareTag("CERTAAudio") )
			{
				for ( int i=0 ; i<triggeredSounds.Count ; i++ )
				{
					if(hit.gameObject.name.Equals(triggeredSounds[i]))
					{
						//already triggered
						return;
					}
				}
				//Debug.Log("PLayyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyying ooooooooooooooooonnnnnnnnne shottttttttttttttttttt");
				if(certaSource!=null)
				{
					//play one shot sound
					soundMan.soundMgr.playOneShotOnSource(certaSource, hit.gameObject.name, GameManager.Manager.PlayerType);
					//add to the triggered sounds
					triggeredSounds.Add(hit.gameObject.name);
				}

			}
		}
	}
}


