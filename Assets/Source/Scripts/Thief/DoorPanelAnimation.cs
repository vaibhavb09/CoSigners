using UnityEngine;
using System.Collections;


public class DoorPanelAnimation : MonoBehaviour
{
	public bool             animating;
	public Texture2D        blankTexture;
	public Texture2D        firstFlash;
	public Texture2D        secondFlash;
	public Texture2D        thirdFlash;
	public float            time;

	public void StartDoorPanelAnimation()
	{
		time=0.0f;
		animating = true;
	}

	void Start()
	{
		animating=false;
		time=0.0f;
		blankTexture = Resources.Load ("Textures/IT_Images/IT_TTS_01", typeof(Texture2D)) as Texture2D;
		firstFlash = Resources.Load ("Textures/IT_Images/IT_TTS_02", typeof(Texture2D)) as Texture2D;
		secondFlash = Resources.Load ("Textures/IT_Images/IT_TTS_03", typeof(Texture2D)) as Texture2D;
		thirdFlash = Resources.Load ("Textures/IT_Images/IT_TTS_04", typeof(Texture2D)) as Texture2D;
	}
	
	void Update()
	{
		if(animating)
		{
			if( time <= 0.3f)
			{
				renderer.material.mainTexture= blankTexture;
				time += Time.deltaTime;
			}
			else if( time> 0.3f && time<= 0.6f)
			{
				renderer.material.mainTexture= firstFlash;
				time += Time.deltaTime;
			}
			else if( time> 0.6f && time<= 0.9f)
			{
				renderer.material.mainTexture= secondFlash;
				time += Time.deltaTime;
			}
			else if( time> 0.9f && time<= 1.2f)
			{
				renderer.material.mainTexture= thirdFlash;
				time += Time.deltaTime;
			}
			else if( time> 1.2f && time<= 1.5f)
			{
				renderer.material.mainTexture= blankTexture;
				time += Time.deltaTime;
				animating=false;
			}
		}
	 }
}

	



