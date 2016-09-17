using UnityEngine;
using System.Collections;

public class FinalLevelMusicTransition : MonoBehaviour 
{
	private float fadeVolume = 0.0f;
	private float fadeDuration = 2.0f;

	private bool musicPlayed = false;
	private bool transitionDone = false;

	private AudioSource mAudioSource = null;

	void Start () 
	{
		if( GameManager.Manager.PlayerType == 1 )
			mAudioSource = soundMan.soundMgr.TheifBGMSource;
		else if ( GameManager.Manager.PlayerType == 2 )
			mAudioSource =soundMan.soundMgr.HackerBGMSource;

		fadeVolume = mAudioSource.audio.volume;
	}

	void Update () 
	{
		if( mAudioSource == null && !transitionDone)
		{
			if( GameManager.Manager.PlayerType == 1 )
				mAudioSource = soundMan.soundMgr.TheifBGMSource;
			else if ( GameManager.Manager.PlayerType == 2 )
				mAudioSource =soundMan.soundMgr.HackerBGMSource;

			fadeVolume = mAudioSource.audio.volume;
		}

		if( gameObject.GetComponent<DoorController>().isOpen && !musicPlayed )
		{
			musicPlayed = true;
		}

		if( musicPlayed && fadeVolume >= 0.0f && mAudioSource != null )
		{
			mAudioSource.volume = fadeVolume; 
			fadeVolume -= Time.deltaTime/fadeDuration;
		}

		if( fadeVolume <= 0.0f )
		{
			soundMan.soundMgr.playOnSource(mAudioSource, "FinalLevelPart2", true, GameManager.Manager.PlayerType, -1, 0.7f);
			transitionDone = true;
			mAudioSource = null;

			gameObject.GetComponent<FinalLevelMusicTransition>().enabled = false;
		}
	}
}
