//#define SHOW_DEBUG_DATA

using UnityEngine;
using System.Collections;
using System;

using System.Collections.Generic;
using System.IO;

public class soundMan
{
	// Stores all the sounds in the game
	// all sounds loaded at start time
	// Right now , internal descriptor name and clip, eventually it should be a hash
	//  [ EDIT ] doesnt need to be changed to a string hash, dictionary lookups are already optimized to O(1)
	// Source :  http://msdn.microsoft.com/en-us/library/xfhwa508.aspx check the Remarks section
	private Dictionary<string,soundFile> m_AllGameSounds;
	
	/// <summary>
	/// The satatic instance for the Sound Manager
	/// </summary>
	private static soundMan _instance;
	
	/// <summary>
	/// Global enable or disable for sounds
	/// </summary>
	public bool enableSounds = true;
	
	/// <summary>
	/// Gets the sound mgr.
	/// </summary>
	/// <value>
	/// The sound mgr.
	/// </value>
	public static soundMan soundMgr
	{
		get
		{
			if(_instance == null)
			{
				_instance = new soundMan();
			}
			return _instance;
		}
	}
	
	/// <summary>
	/// The audio source for the hacker
	/// </summary>
	private AudioSource m_HackerBGMSource;
	
	public AudioSource HackerBGMSource
	{
		get
		{
			return m_HackerBGMSource;
		}
		
		set
		{
			m_HackerBGMSource = value;
		}		
	}

	/// <summary>
	/// The audio source for the hacker
	/// </summary>
	private AudioSource m_TheifBGMSource;

	public AudioSource TheifBGMSource
	{
		get
		{
			return m_TheifBGMSource;
		}
		
		set
		{
			m_TheifBGMSource = value;
		}		
	}

	/// <summary>
	/// The audio source for the point man alarm sounds
	/// </summary>
	private AudioSource m_TheifAlarmSource;
	//public AudioSource pausedSource;

	
	// Use this for initialization
	public void Initialize ()
	{
		if(enableSounds)
		{
		
		m_AllGameSounds = new Dictionary<string, soundFile>();
		
		// reading the level descriptor as a text file
		TextAsset soundFileDescriptor = (TextAsset) Resources.Load("SFX/sound_descriptor");
		
		string[] _tempArray = new string[3];
		
#if SHOW_DEBUG_DATA
		Debug.Log("************************** Reading Sound Files ***************************");
#endif
		
		using (TextReader soundFileReader = new StringReader((string)soundFileDescriptor.text))
		{
			while(soundFileReader.Peek() >= 0)
			{
				// Read a line from the CSV descriptor file 
				string _soundFileDescription = soundFileReader.ReadLine();
				
				// split it by commas
				_tempArray = _soundFileDescription.Split(',');
			
				
#if SHOW_DEBUG_DATA
				//Debug.Log("Sound File Description : " + _soundFileDescription);
				//Debug.Log("Sound file internal descriptor read : " + _tempArray[0]);
				//Debug.Log("Sound file name read : " + _tempArray[1]);					
				//Debug.Log("Sound file client type : " + _tempArray[2]);
#endif
				
				int _soundClientType = Convert.ToInt32(_tempArray[2]);
				
				// If this clients type matches the sound clip being loaded
				if( (_soundClientType == -1 ) || (GameManager.Manager.PlayerType == _soundClientType))
				{
					// load the clip in
					AudioClip _tempClip = (AudioClip)Resources.Load (_tempArray[1]);
						
						if(_tempClip == null)
						{
							Debug.LogError("The sound clip for : " + _tempArray[1] + " was not loaded");
						}
					
					// create a new sound file
					soundFile _tempSoundFile = new soundFile(_tempClip,_soundClientType);
					
					// add the Sound file to the sounds dictionary
					m_AllGameSounds.Add(_tempArray[0],_tempSoundFile);
				}
			}
			
			soundFileReader.Close();			
		}
		
		//_instance = this;
#if SHOW_DEBUG_DATA
		Debug.Log("************************** END Reading Sound Files ***************************");
#endif
			
			// Initialize extra audio sources on prefabs
			// Point man init
			if(GameManager.Manager.PlayerType == 1)
			{
				GameObject Player = GameObject.Find("Playertheif(Clone)");
				MovementScript pointMan = (MovementScript)Player.GetComponent("MovementScript");
				pointMan.InitializeSounds();
				
				TheifBGMSource = Player.audio;
				//m_TheifAlarmSource = pointMan.PointManAlarmSource;
				
#if SHOW_DEBUG_DATA
				if(m_TheifAlarmSource == null)
					Debug.Log("Theif alarm source is null");
#endif
				
			}
			else
			{	
				// Hacker init
				GameObject Hacker = GameObject.Find("TopDownCamera");
				HackerBGMSource = Hacker.audio;
				TopDown_Camera.Manager.InitializeSounds();
			}
			
		}
	}
	
	/// <summary>
	/// Plays indicated sound on the indicated source , to stop, just call stop on source wherever required
	/// </summary>
	/// <param name='_currentSource'>
	/// The source on which sounds are to be played
	/// </param>
	/// <param name='_toBePlayed'>
	/// The sound that is to be played
	/// </param>
	/// <param name="_playerType">
	/// The player type for which it is acceptable to play this sound
	/// </param>
	/// <param name="i_SourceOverride">
	/// Indicates an overriding source on which this sound should be played, used for routing sounds directly to the point man or the hacker BGM source
	/// -1 by default , means that there must be a valid _currentSource on which this sound is to be played
	/// 1 means that the Point man BGM source should play this sound
	/// 2 means that the Hacker BGM source should play this sound (actually any number other than -1 and 1, but please be nice and use 2 )
	/// </param>
	public void playOnSource(AudioSource _currentSource , string _toBePlayed , bool _stopPrevious,int _clientType,int i_SourceOverride = -1 , float i_volume = 1.0f)
	{
		if(enableSounds)
		{
			if(i_SourceOverride != -1)
			{
				if(_clientType == i_SourceOverride)
					_currentSource = (i_SourceOverride == 1)? m_TheifBGMSource : m_HackerBGMSource;
				else if(i_SourceOverride == 3)
				{
					_currentSource = m_TheifAlarmSource;
					
				#if SHOW_DEBUG_DATA
					Debug.Log(" Source override was correct " + _currentSource.ToString());
				#endif
				}
				else 
				{
					Debug.LogError("This sound" + _toBePlayed + "should not be playing on this type of client [FIX THIS, Talk to NIK]" + _clientType);
					return;
				}
			}
	
#if SHOW_DEBUG_DATA
		Debug.Log("SOUND MAN WAS ASKED FOR " + _toBePlayed);
		Debug.Log("Source : " + _currentSource.name);
#endif
			
		// Stop any sound that is playing on this source 
		if(_stopPrevious)
			_currentSource.Stop();
		
		// if the current source is not playing any sound
		if(!_currentSource.isPlaying)
		{
			// And there are some sounds in the all Sounds list
			if(m_AllGameSounds == null)
				Debug.LogError("Sounds array is NULL");

			soundFile _tempClip = null;
			
			// Get the indicated sound File
			// [CHANGE] Change to an int check
			// Edit , doesnt need to be changed to a string hash, dictionary lookups are already optimized to O(1)
			// Source :  http://msdn.microsoft.com/en-us/library/xfhwa508.aspx check the Remarks section
			m_AllGameSounds.TryGetValue(_toBePlayed,out _tempClip);
						
			if(_tempClip != null)	
			{					
				// if this kind of a clip can be played on this client 
				if((_tempClip.clientType == -1) || ( _tempClip.clientType == _clientType) || (_clientType == -1))
				{			
					// Play it 
					_currentSource.clip = _tempClip.audioClip;
					_currentSource.volume = i_volume;
					_currentSource.Play();
					_currentSource.loop = true;
				}
				else
				{
					Debug.LogError("This sound" + _toBePlayed + "should not be playing on this type of client [FIX THIS, Talk to NIK]" + _clientType);
				}
					
			}
			else
				{
					Debug.LogError("Sound file for" + _toBePlayed + " not found");			
					if( _tempClip.clientType != _clientType)
					{
						Debug.LogError("This sound" + _toBePlayed + "should not be requested on this type of client [FIX THIS, Talk to NIK]" + _clientType);
					}

				}
		}
		}
	}
	
	/// <summary>
	/// Silences the source, specifically the looping sound on any given source is silenced
	/// </summary>
	/// <param name='_currentSource'>
	/// Source which is to be silenced
	/// </param>
	/// <param name="i_SourceOverride">
	/// Indicates an overriding source to be silenced
	/// -1 by default , means that there must be a valid _currentSource on which this sound is to be played
	/// 1 means that the Point man BGM source should play this sound
	/// 2 means that the Hacker BGM source should play this sound (actually any number other than -1 and 1, but please be nice and use 2 )
	/// </param>
	public void silenceSource(AudioSource _currentSource, int i_SourceOverride = -1)
	{
		if(enableSounds)
		{
			if(i_SourceOverride != -1)
				_currentSource = (i_SourceOverride == 1)? m_TheifBGMSource : m_HackerBGMSource;
			else if(i_SourceOverride == 3)
			{
				_currentSource = m_TheifAlarmSource;
				#if SHOW_DEBUG_DATA
					Debug.Log(" Source override was correct " + _currentSource.ToString());
				#endif
			}
			
		if(_currentSource != null)
			{
				//Debug.Log("SOURCE WAS NOT NULL *************************************** ");

				_currentSource.Stop();
			
			}

		}
	}
	
	/// <summary>
	/// Plays the one shot sound on a source.
	/// </summary>
	/// <param name='_currentSource'>
	/// Source on which the sound is to be played
	/// </param>
	/// <param name='_toBePlayed'>
	/// The sound that is to be played
	/// </param>
	/// <param name='_clientType'>
	/// The type of player requesting this sound
	/// </param>
	/// <param name='_volume'>
	/// The volume at which this sound is to be played
	/// </param>
	/// /// <param name="i_SourceOverride">
	/// Indicates an overriding source on which this sound should be played, used for routing sounds directly to the point man or the hacker BGM source
	/// -1 by default , means that there must be a valid _currentSource on which this sound is to be played
	/// 1 means that the Point man BGM source should play this sound
	/// 2 means that the Hacker BGM source should play this sound (actually any number other than -1 and 1, but please be nice and use 2 )
	/// </param>
	public void playOneShotOnSource(AudioSource _currentSource , string _toBePlayed,int _clientType,int i_SourceOverride = -1,float _volume = 1.0f)
	{
		
		if(m_AllGameSounds != null)
		{
		#if SHOW_DEBUG_DATA
			//Debug.Log("SOUND MAN WAS ASKED FOR " + _toBePlayed);
		#endif
			
			if(enableSounds)
			{
				if(i_SourceOverride != -1)
				{
					if(_clientType == i_SourceOverride)
						_currentSource = (i_SourceOverride == 1)? m_TheifBGMSource : m_HackerBGMSource;
					else if(i_SourceOverride == 3)
					{
						_currentSource = m_TheifAlarmSource;
					}
					else
					{
						//Debug.LogError("This sound" + _toBePlayed + "should not be playing on this type of client [FIX THIS, Talk to NIK]" + _clientType);
						return;
					}
				}
				
			soundFile _tempClip;
			
			// set the indicated sound to be on this source
			m_AllGameSounds.TryGetValue(_toBePlayed,out _tempClip);
			
			if(_tempClip != null)	
			{
				// if this kind of a clip can be played on this client 
				if((_tempClip.clientType == -1) || ( _tempClip.clientType == _clientType) || (_clientType == -1) )
				{			
					if(_currentSource == null)
					{
							//Debug.LogError("Current source is null");
					}
					else
					{
						_currentSource.PlayOneShot(_tempClip.audioClip,_volume);
							
						#if SHOW_DEBUG_DATA
							//Debug.Log("SOUND MAN PLAYED " + _toBePlayed);
						#endif
					}
				}else
				{
					#if SHOW_DEBUG_DATA
					// Remove from release
					//Debug.LogError("This sound" + _toBePlayed + "should not be playing on this type of client [FIX THIS, Talk to NIK]" + _clientType);
					#endif
				}
			}
			else
				{
					//Debug.LogError("Sound " + _toBePlayed + " not found , this is probably Ok, a sound of a different client type is being requested on the other side");
				}
			}
		}
	}

	public void pauseSource(AudioSource i_CurrentSource, int i_SourceOverride = -1)
	{
		if(i_SourceOverride == -1)
		{
			if(i_CurrentSource != null)
			{
				i_CurrentSource.Pause();
			}
		}else
		{
			if(enableSounds)
			{
				AudioSource _currentSource=null;
				
//				if(i_SourceOverride != -1)
//					_currentSource = (i_SourceOverride == 1)? m_TheifBGMSource : m_HackerBGMSource;
//				else if(i_SourceOverride == 3)
//				{
//					_currentSource = m_TheifAlarmSource;
//				}
//				else 
//				{
//					return;
//				}

				switch(i_SourceOverride)
				{
				case 1: 
				{
					if(m_TheifBGMSource.isPlaying)
						_currentSource = m_TheifBGMSource;
					break;
				}
				case 2:
				{
					if(m_HackerBGMSource.isPlaying)
						_currentSource = m_HackerBGMSource;
					break;
				}
				case 3:
				{
					if(m_TheifAlarmSource)
					if(m_TheifAlarmSource.isPlaying)
						_currentSource = m_TheifAlarmSource;
					break;
				}
				}

				if(_currentSource != null)
					if(_currentSource.isPlaying)
				{		
					//pausedSource= _currentSource;
					_currentSource.Pause();
				}
			}
		}
	}

	public void resumeSource(AudioSource i_CurrentSource, int i_SourceOverride = -1)
	{
		if(i_SourceOverride == -1)
		{
			if(i_CurrentSource != null)
			{
				i_CurrentSource.Play();
			}
		}else
		{
			switch(i_SourceOverride)
			{
				case 1: 
					{
						if(!m_TheifBGMSource.isPlaying)
							m_TheifBGMSource.Play();
						break;
					}
				case 2:
					{
						if(!m_HackerBGMSource.isPlaying)
							m_HackerBGMSource.Play();
						break;
					}
				case 3:
					{
				        if(m_TheifAlarmSource)
						if(!m_TheifAlarmSource.isPlaying)
							m_TheifAlarmSource.Play();
						break;
					}
			}
		}
	}

	public void PauseGame(int i_SourceOverride)
	{
		// Pausing the BGM souce for this client
		pauseSource(null,i_SourceOverride);

		// Pausing the Alarm source
		pauseSource(null,3);
	}

	public void UnPauseGame(int i_SourceOverride = -1)
	{
		if(enableSounds)
		{
			// Resuming the BGM souce for this client
			resumeSource(null,i_SourceOverride);
			
			// Resuming the Alarm source
			resumeSource(null,3);
		}
	}
}

