using UnityEngine;
using System.Collections;

public class ScoreTest : Photon.MonoBehaviour {
	
	private GameObject _playerUtil;
	// Use this for initialization
	void Start () {
		_playerUtil = GameObject.Find("PlayerUtil");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	private void OnGUI()
	{
		if(GUI.Button(new Rect(40, 0, 200, 20), "AddHackerOverallScore"))
		{
			//PlayerProfile.HackerOverallScore++;
			//if(PlayerProfile.HackerOverallScore > 9)
			{
			//	PlayerProfile.HackerOverallScore = 9;
			}
		}
		
		if(GUI.Button(new Rect(40, 40, 200, 20), "AddPointmanOverallScore"))
		{
			//PlayerProfile.PointmanOverallScore++;
			//if(PlayerProfile.PointmanOverallScore > 9)
			{
				//PlayerProfile.PointmanOverallScore = 9;
			}			
		}
		
		if(GUI.Button(new Rect(40, 80, 200, 20), "AddPointmanScoreForThisLevel"))
		{
			//PlayerProfile.PointmanScorePerLevel[1]++;
			//if(PlayerProfile.PointmanScorePerLevel[1] > 9)
			{
				//PlayerProfile.PointmanScorePerLevel[1] = 9;
			}			
		}
		
		if(GUI.Button(new Rect(40, 120, 200, 20), "AddHackerScoreForThisLevel"))
		{
			//PlayerProfile.HackerScorePerLevel[1]++;
			//if(PlayerProfile.HackerScorePerLevel[1] > 9)
			{
				//PlayerProfile.HackerScorePerLevel[1] = 9;
			}
		}
		
		if(GUI.Button(new Rect(40, 160, 200, 20), "ReturnToMainMenu"))
		{
			//PlayerProfile.SavePlayerProfile();
			//PlayerProfile.SavePlayerProfile();
			Application.LoadLevel(0);
			//PhotonNetwork.Disconnect();
		}
	}
}
