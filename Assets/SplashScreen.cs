using UnityEngine;
using System.Collections;

public class SplashScreen : MonoBehaviour 
{
	private float time = 0.0f;
	private float time1 = 2.5f;
	private float alpha = 0.0f;
	private bool  gameStarted = false;
	
	void Start () 
	{
		guiTexture.pixelInset = new Rect(0,0,Screen.width,Screen.height);
	}
	
	void Update ()
	{
		if( GameManager.Manager.isStartUpFirstTime )
		{
			if( time < 2.5f)
			{
				guiTexture.color = new Color(alpha/2.5f,alpha/2.5f,alpha/2.5f, alpha );
				alpha = Mathf.Lerp ( 0, 1, time / 1.0f );
				time += Time.deltaTime;
			}
			else if( time1 > 0.0f )
			{
				guiTexture.color = new Color(alpha/2.5f,alpha/2.5f,alpha/2.5f, alpha );
				alpha = Mathf.Lerp ( 0, 1, time1 / 1.0f );
				time1 -= Time.deltaTime;
			}
			else
			{
				guiTexture.color = new Color(0.0f,0.0f,0.0f,0.0f);
					
				if( !gameStarted )
				{
					gameStarted = true;
					gameObject.GetComponent<StartGameInterface>().Show();
					GameManager.Manager.isStartUpFirstTime = false;
				}	
			}
		}
		else if( !gameStarted)
		{
			gameStarted = true;
			gameObject.GetComponent<StartGameInterface>().Show();
		}
	}
}
