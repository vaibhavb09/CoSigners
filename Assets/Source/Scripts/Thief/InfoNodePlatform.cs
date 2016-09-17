using UnityEngine;
using System.Collections;

public enum InfoPlatformStates
{
	LOCKED,
	UNLOCKED,
	INFO_SCREEN_UP_ANIMATION,
	READY_TO_PLAY_MOVIE,
	PLAYING_MOVIE,
}

public class InfoNodePlatform : MonoBehaviour 
{
	
	private bool 			_isOpen, _isPaused, _isActivated, _screenAnimationStarted = false, _platformAnimating = false;
	private Animation 	    []animations;
	private float 			_animationStartTime;
	public int 				InfoID;
	public InfoPlatformStates m_state = InfoPlatformStates.LOCKED;
	
	private bool			m_isInfoUpPlaying = false;
	
	public bool Paused
	{
		get
		{
			return _isPaused;
		}
		set
		{
			_isPaused = value;
		}
	}
	
	public bool Activated
	{
		get
		{
			return _isActivated;
		}
		set
		{
			_isActivated = value;
		}
	}
	
	// Use this for initialization
	void Start () 
	{
		animations = gameObject.GetComponentsInChildren<Animation>();
		_isOpen = false;
		_animationStartTime = 0.0f;
		Paused = false;
		Activated = false;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if( Activated )
		{
			switch( m_state )
			{
				case InfoPlatformStates.UNLOCKED: 
				{
					PlayScreenAnimation(); 
					break;
				}

				case InfoPlatformStates.INFO_SCREEN_UP_ANIMATION: 
				{
					PlayInfoUpAnimation(); 
					break;
				}
			}
		}
		else
		{
			PlayLockAnimation();
		}
	}

	public void LockInfoNodePlatform( bool i_activated )
	{
		Transform infoPanelScreen1 = gameObject.transform.FindChild("IT_Stand_V1").FindChild("ScreenStand").FindChild("IT_GlowMesh");
		Transform infoPanelScreen2 = gameObject.transform.FindChild("IT_Stand_V2").FindChild("ScreenStand").FindChild("IT_GlowMesh");
		Transform infoPanelScreen3 = gameObject.transform.FindChild("IT_Stand_V3").FindChild("ScreenStand").FindChild("IT_GlowMesh");
		
		Material glowOrange = Resources.Load("Materials/Thief/GlowMesh_Orange") as Material;
		
		infoPanelScreen1.renderer.material = infoPanelScreen2.renderer.material = infoPanelScreen3.renderer.material = glowOrange;

		infoPanelScreen1 = gameObject.transform.FindChild("IT_Stand_V1").FindChild("ScreenStand").FindChild("ScreenVisor").FindChild("Screen");
		infoPanelScreen2 = gameObject.transform.FindChild("IT_Stand_V2").FindChild("ScreenStand").FindChild("ScreenVisor").FindChild("Screen");
		infoPanelScreen3 = gameObject.transform.FindChild("IT_Stand_V3").FindChild("ScreenStand").FindChild("ScreenVisor").FindChild("Screen");
		
		Texture it_initScreen = Resources.Load("Textures/IT_Images/IT_TTS_01") as Texture;
		
		infoPanelScreen1.renderer.material.mainTexture = infoPanelScreen2.renderer.material.mainTexture = infoPanelScreen3.renderer.material.mainTexture = it_initScreen;
		
		Activated = i_activated;
		
		if( !_isOpen )
		{
			PlayLockAnimation();
		}
		
		_isOpen = true;
	}
	
	public void PlayInformation()
	{
		m_state = InfoPlatformStates.PLAYING_MOVIE;
		{
			NetworkManager.Manager.PlayInformation( InfoID );
		}
	}
	
	public void UnlockInfoNodePlatform( bool i_activated )
	{
		Transform infoPanelScreen1 = gameObject.transform.FindChild("IT_Stand_V1").FindChild("ScreenStand").FindChild("IT_GlowMesh");
		Transform infoPanelScreen2 = gameObject.transform.FindChild("IT_Stand_V2").FindChild("ScreenStand").FindChild("IT_GlowMesh");
		Transform infoPanelScreen3 = gameObject.transform.FindChild("IT_Stand_V3").FindChild("ScreenStand").FindChild("IT_GlowMesh");
		
		Material glowGreen = Resources.Load("Materials/Thief/Unlocked") as Material;
		
		infoPanelScreen1.renderer.material = infoPanelScreen2.renderer.material = infoPanelScreen3.renderer.material = glowGreen;
		
		Activated = i_activated;
		if( _isOpen )
		{
			PlayUnlockAnimation();
		}
		
		m_state = InfoPlatformStates.UNLOCKED;
		
		_isOpen = false;
	}
	
	public void PlayScreenAnimation()
	{
		Transform infoPanelScreen1 = gameObject.transform.FindChild("IT_Stand_V1").FindChild("ScreenStand").FindChild("ScreenVisor").FindChild("Screen");
		Transform infoPanelScreen2 = gameObject.transform.FindChild("IT_Stand_V2").FindChild("ScreenStand").FindChild("ScreenVisor").FindChild("Screen");
		Transform infoPanelScreen3 = gameObject.transform.FindChild("IT_Stand_V3").FindChild("ScreenStand").FindChild("ScreenVisor").FindChild("Screen");

		if( !animations[0].IsPlaying("Unlock") && _platformAnimating )
		{
			infoPanelScreen1.GetComponent<DoorPanelAnimation>().StartDoorPanelAnimation();
			infoPanelScreen2.GetComponent<DoorPanelAnimation>().StartDoorPanelAnimation();
			infoPanelScreen3.GetComponent<DoorPanelAnimation>().StartDoorPanelAnimation();
			_platformAnimating = false;
			_screenAnimationStarted = true;
		}
		else if( _screenAnimationStarted && infoPanelScreen1.GetComponent<DoorPanelAnimation>().animating == false )
		{
			_screenAnimationStarted = false;
			m_state = InfoPlatformStates.INFO_SCREEN_UP_ANIMATION;
		}
	}

	public void PlayUnlockAnimation()
	{
		Transform infoPanelScreen1 = gameObject.transform.FindChild("IT_Stand_V1").FindChild("ScreenStand").FindChild("IT_MainScreen_Dropface");
		
		if( !_platformAnimating )
		{
			for( int i = 0; i < animations.GetLength(0); i++)
			{
				animations[i].Play("Unlock");
				_platformAnimating = true;
			}
		}
	}

	public void StartInfoNodeAnimation()
	{
		PlayInformation();
	}
	
	public void PlayInfoUpAnimation()
	{
		switch( m_state )
		{
			case InfoPlatformStates.INFO_SCREEN_UP_ANIMATION:
			{
				Transform infoPanelScreen1 = gameObject.transform.FindChild("IT_Stand_V1").FindChild("ScreenStand").FindChild("ScreenVisor").FindChild("Screen");
				Transform infoPanelScreen2 = gameObject.transform.FindChild("IT_Stand_V2").FindChild("ScreenStand").FindChild("ScreenVisor").FindChild("Screen");
				Transform infoPanelScreen3 = gameObject.transform.FindChild("IT_Stand_V3").FindChild("ScreenStand").FindChild("ScreenVisor").FindChild("Screen");

				Texture it_screen = Resources.Load("Textures/IT_Images/IT_Screen") as Texture;
				
				infoPanelScreen1.renderer.material.mainTexture = infoPanelScreen2.renderer.material.mainTexture = infoPanelScreen3.renderer.material.mainTexture = it_screen;

				m_isInfoUpPlaying = true;
				
				if( !_platformAnimating )
				{
					for( int i = 0; i < animations.GetLength(0); i++)
					{
						animations[i].Play("InfoUp");
						_platformAnimating = true;
					}
				}
				else if( !animations[0].IsPlaying("InfoUp") )
				{
					m_state = InfoPlatformStates.READY_TO_PLAY_MOVIE;
					_platformAnimating = false;
				}
			}
			break;
		}
	}
	
	void PlayLockAnimation()
	{
		switch (m_state) 
		{
			case InfoPlatformStates.READY_TO_PLAY_MOVIE:
			{
				if( !_platformAnimating )
				{
					for( int i = 0; i < animations.GetLength(0); i++)
					{
						animations[i].Play("InfoClose");
						_platformAnimating = true;
					}
				}
				else if( !animations[0].isPlaying )
				{
					_platformAnimating = false;
					m_state = InfoPlatformStates.UNLOCKED;
				}
				
			}
				break;
				
			case InfoPlatformStates.UNLOCKED:
			{
				if( !_platformAnimating )
				{
					for( int i = 0; i < animations.GetLength(0); i++)
					{
						animations[i].Play("Lock");
						_platformAnimating = true;
					}
				}
				else if( !animations[0].isPlaying )
				{
					_platformAnimating = false;
					m_state = InfoPlatformStates.LOCKED;
				}
			}
			break;
			
		}
	}
}
