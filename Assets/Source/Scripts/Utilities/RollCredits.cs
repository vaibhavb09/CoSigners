using UnityEngine;
using System.Collections;

public class RollCredits : MonoBehaviour 
{
	public int HeightMultiplier;
	public int TimeSeconds;
	public int FadeInTime;
	public float ScrollSpeed;

	float LevelStartTime;
	GameObject creditsScreen;
	GameObject camera;
	bool isScrolling;
	Texture2D texture;

	void Start () 
	{
		creditsScreen = GameObject.Find("CreditsScreen");
		camera = GameObject.FindGameObjectWithTag("MainCamera");
		camera.transform.position = new Vector3(0, (HeightMultiplier-1) * 4.5f, 1);

		isScrolling = false;
		LevelStartTime = Time.time;
	}

	void Update () 
	{
		if( Time.time - LevelStartTime > TimeSeconds )
		{
			isScrolling = true;
		}
		else
		{
			Color forAlpha = creditsScreen.renderer.material.color;
			creditsScreen.renderer.material.color = new Color( forAlpha.r, forAlpha.g, forAlpha.b, (Time.time - FadeInTime) / (float)FadeInTime );
		}

		if( isScrolling )
		{
			//Debug.Log( isScrolling );
			Vector3 pos = camera.transform.position;
			camera.transform.position = new Vector3( pos.x, pos.y - ScrollSpeed, pos.z );
		}

		if( camera.transform.position.y <= (HeightMultiplier-1) * -4.5f )
		{
			//Debug.Log( camera.transform.position.y+ " "+ (HeightMultiplier - 1) * 9.0f);
			LevelStartTime = Time.time;
			isScrolling = false;
		}
	}

	void OnGUI()
	{
		if( GUI.Button( new Rect( Screen.width - Screen.width * 0.22f, Screen.height - Screen.height * 0.08f, Screen.width * 0.2f, Screen.height * 0.05f), "Return to Main Menu") )
		{
			NetworkManager.Manager.LoadLevel(0);
		}
	}
}
