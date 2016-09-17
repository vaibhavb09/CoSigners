using UnityEngine;
using System.Collections;

public class PasswordPlatformAnimation : MonoBehaviour 
{
	//private static PasswordPlatformAnimation m_instance = null; 
	private bool m_isOpen = false;
	private TextMesh passwordTextMesh = null;
	private Animation gameObjectAnimation = null;
	private GameObject sparkles = null;
	private string currentPassword = "legendary";
	public float sparkleDelayTime = 4.0f;
	private float animStartTime = 0.0f;
	
	//public static PasswordPlatformAnimation Manager
	//{
	//	get
	//	{
	//		if(m_instance != null)
	//			return m_instance;
	//		else
	//		{
	//			m_instance= new PasswordPlatformAnimation();
	//			return m_instance;
	//		}
	//	}
	//}
	
	void Start()
	{
		//m_instance = this;
		gameObjectAnimation = gameObject.transform.GetChild(0).animation;
		passwordTextMesh = gameObject.transform.GetChild(2).GetComponent<TextMesh>();
		sparkles = gameObject.transform.GetChild(1).gameObject;
		gameObjectAnimation.Play("Default");
		sparkles.SetActive(false);
		// Bonus hex activated [SOUND TAG] Bonus_Hex_Idle
	}

	public void OpenPasswordPlatform( )
	{
		if( !m_isOpen )
		{
			if( !gameObjectAnimation.IsPlaying("Close") )
			{
				gameObjectAnimation.Play("Open");
				animStartTime = Time.time;
			}
			else
			{
				gameObjectAnimation.CrossFade("Open");
				animStartTime = Time.time;
			}
			
			// [ SOUND TAG ] [Password_reveal]
			// Play only on the theif side
			if(GameManager.Manager.PlayerType == 1)
				soundMan.soundMgr.playOneShotOnSource(this.audio,"Password_reveal",GameManager.Manager.PlayerType);
		}
		
		m_isOpen = true;
	}
	
	public void ClosePasswordPlatform( )
	{
		if( m_isOpen )
		{
			if( !gameObjectAnimation.IsPlaying("Open") ) //child 0 is password platform mesh
			{
				gameObjectAnimation.Play("Close");
			}
			else
			{
				gameObjectAnimation.CrossFade("Close");
			}
			
			// [ SOUND TAG ] [ Password_retract ]
			// Play only on the theif side
			if(GameManager.Manager.PlayerType == 1)
				soundMan.soundMgr.playOneShotOnSource(this.audio,"Password_retract",GameManager.Manager.PlayerType);
		}
		
		m_isOpen = false;
	}
	
	public void Update()
	{
		if( m_isOpen )
		{
			if( !gameObjectAnimation.IsPlaying("Open") && !passwordTextMesh.renderer.enabled ) 
			{
				DisplayPassword();
				NetworkManager.Manager.SetPasswordHacker( currentPassword );
				if (Time.time - animStartTime > sparkleDelayTime)
					sparkles.SetActive(true);
			}
		}
		else if( passwordTextMesh.renderer.enabled )
		{
			StopDisplayPassword();
			sparkles.SetActive(false);
		}
	}
	
	public bool isPasswordPlatformOpen()
	{
		return m_isOpen;	
	}
	
	public void DisplayPassword()
	{
		passwordTextMesh.renderer.enabled = true;
		currentPassword = PasswordGenerator.Manager.GeneratePassword( currentPassword );
		passwordTextMesh.text = currentPassword;
	}
	
	public void StopDisplayPassword()
	{
		passwordTextMesh.renderer.enabled = false;
	}
}
