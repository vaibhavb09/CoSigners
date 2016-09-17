using UnityEngine;
using Steamworks;
using System.Collections;

public class SteamScript : MonoBehaviour 
{
	protected Callback<GameOverlayActivated_t> m_GameOverlayActivated;

	void Start() 
	{
		if(SteamManager.Initialized) 
		{
			string name = SteamFriends.GetPersonaName();
			Debug.Log(name);
		}
	}

	private void OnEnable() 
	{
		if (SteamManager.Initialized) 
		{
			m_GameOverlayActivated = Callback<GameOverlayActivated_t>.Create(OnGameOverlayActivated);
		}
	}

	private void OnGameOverlayActivated(GameOverlayActivated_t pCallback) 
	{
		if(pCallback.m_bActive != 0) 
		{
			Debug.Log("Steam Overlay has been activated");
		}
		else 
		{
			Debug.Log("Steam Overlay has been closed");
		}
	}
}
