using UnityEngine;

public class soundFile
{
	/// <summary>
	/// The Audio clip itself
	/// </summary>
	private AudioClip m_SoundClip;
	
	public AudioClip audioClip
	{
		get 
		{
			return m_SoundClip;	
		}
	}
	
	/// <summary>
	/// The type of the Client this sound can play on
	/// </summary>
	private int m_clientType;
	
	public int clientType
	{
		get
		{
			return m_clientType;			
		}
	}
	
	public soundFile ( AudioClip i_soundClip, int i_clientType)
	{
		m_SoundClip = i_soundClip;
		m_clientType = i_clientType;
	}
}

