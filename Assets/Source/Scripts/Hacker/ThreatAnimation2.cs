using UnityEngine;
using System.Collections;

public class ThreatAnimation2 : MonoBehaviour 
{
	float m_maxWidth;
	float m_height;
	Texture2D FrameBg;
	Texture2D BgBoxTexture;
	Texture2D FgBoxTexture;
	
	float MainThreatLevel;
	float FollowThreatLevel;
	float InitFgLevel;
	float goToLevel;
	float tempFloat;

	float prevThreat;

	bool threatDecreased;

	float time = 0.0f;
	float rate = 1.0f;
	float thresholdFlicker = 0.75f;
	bool increase = true;
	
	void Start () 
	{
		MainThreatLevel = 0.0f;
		FollowThreatLevel = 0.0f;
		InitFgLevel = 0.0f;
		tempFloat = 0.0f;
		goToLevel = 0.0f;

		m_maxWidth = Screen.width * 0.208f;
		m_height = Screen.height * 0.045f;
		
		BgBoxTexture = Resources.Load("Textures/HackerGUI/Threat_bkg") as Texture2D;
		FgBoxTexture = Resources.Load("Textures/HackerGUI/Threat_frg") as Texture2D;
	}
	
	void Update () 
	{
		if(!ThiefManager.Manager.gameIsPaused && !HackerManager.Manager.gameIsPaused)
		{
			FollowThreatLevel = ( HackerThreat.Manager.Threat / HackerThreat.Manager.MaxThreat ) * m_maxWidth;
			
			MainThreatLevel = Mathf.Lerp( InitFgLevel, goToLevel, tempFloat * 2 );

			tempFloat += Time.deltaTime;
			
			if( tempFloat >= 0.5f )
			{
				InitFgLevel = MainThreatLevel;
				goToLevel = FollowThreatLevel;
				tempFloat = 0.0f;
			}

			prevThreat = FollowThreatLevel;
		}

		if( (HackerThreat.Manager.Threat / HackerThreat.Manager.MaxThreat) >= thresholdFlicker )
		{
			if( increase )
			{
				if( time >= 1.0f )
				{
					increase = false;
					time = 1.0f;
				}
				else
					time += Time.deltaTime * rate;
			}
			else
			{
				if( time <= 0.4f )
				{
					increase = true;
					time = 0.4f;
				}
				else
					time -= Time.deltaTime * rate;
			}

			rate = 1 + (HackerThreat.Manager.Threat - HackerThreat.Manager.MaxThreat * thresholdFlicker) / (HackerThreat.Manager.MaxThreat * (1 - thresholdFlicker) ) * 2;
		}
	}

	void OnGUI()
	{
		//ScreenHelper.DrawTexture( 0, 1, 32, 2, FrameBg );
		
		//ScreenHelper.DrawTexture( 4, 1, BgBoxLevel / ScreenHelper.GetUnitLength(), 2, BgBoxTexture );

		GUI.depth = 2;

		if( (HackerThreat.Manager.Threat / HackerThreat.Manager.MaxThreat) >= thresholdFlicker )
		{
			GUI.color = new Color(time, time, time);
		}

		if(GameManager.Manager.PlayerType == 1 )
		{
			GUI.DrawTexture( new Rect(Screen.width * 0.5f, Screen.height * 0.0655f, MainThreatLevel, m_height), FgBoxTexture); 		// Main Threat
			GUI.DrawTexture( new Rect(Screen.width * 0.5f, Screen.height * 0.0655f, -MainThreatLevel, m_height), FgBoxTexture);		// Main Threat
			
			GUI.DrawTexture( new Rect( (Screen.width * 0.5f + MainThreatLevel), Screen.height * 0.0655f, FollowThreatLevel - MainThreatLevel, m_height), BgBoxTexture); 		// Follow Threat
			GUI.DrawTexture( new Rect( (Screen.width * 0.5f - MainThreatLevel), Screen.height * 0.0655f, -(FollowThreatLevel - MainThreatLevel), m_height), BgBoxTexture);		// Follow Threat
		}
		else if(GameManager.Manager.PlayerType == 2)
		{
			GUI.DrawTexture( new Rect(Screen.width * 0.5f, Screen.height * 0.045f, MainThreatLevel, m_height), FgBoxTexture); 		// Main Threat
			GUI.DrawTexture( new Rect(Screen.width * 0.5f, Screen.height * 0.045f, -MainThreatLevel, m_height), FgBoxTexture);		// Main Threat
			
			GUI.DrawTexture( new Rect( (Screen.width * 0.5f + MainThreatLevel), Screen.height * 0.045f, FollowThreatLevel - MainThreatLevel, m_height), BgBoxTexture); 		// Follow Threat
			GUI.DrawTexture( new Rect( (Screen.width * 0.5f - MainThreatLevel), Screen.height * 0.045f, -(FollowThreatLevel - MainThreatLevel), m_height), BgBoxTexture);		// Follow Threat
		}

		GUI.color = Color.white;
	}
}
