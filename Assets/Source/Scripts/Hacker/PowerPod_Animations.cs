using UnityEngine;
using System.Collections;
using System;

public class PowerPod_Animations : MonoBehaviour {

	private bool loaded;
	private bool place;
	private bool grow;
	private bool visible;
	private Transform wave;
	private Transform icon;

	private GenericTimer growTimer;
	private GenericTimer placeTimer;



	// Use this for initialization
	void Start () {
	}

	public void Set()
	{
		foreach ( Transform child in transform )
		{
			//Debug.Log ("Child is: " + child.name);
			if ( child.name.Equals("PowerPodWave"))
				wave = child;
			else if ( child.name.Equals("Transmitter_Prefab"))
				icon = child;
		}
		
		Action timerEndAction = delegate(){EndTimer();};
		growTimer = gameObject.AddComponent<GenericTimer>();
		growTimer.Set( 3.0f, true, timerEndAction );
		grow = true;
		visible = true;
		
		if ( wave == null || icon == null || growTimer == null)
		{
			//Debug.LogError("SOMETHING BROKE");
			loaded = false;
		}
		else
		{
			loaded = true;
			place = true;
			icon.renderer.material.SetColor("_TintColor", new Color( 0.2f, 0.2f, 0.2f, 0.5f));
			Place();
		}
	}

	// Update is called once per frame
	void Update () {
		if ( loaded && visible )
		{
			if ( place )
			{
				AnimatePlace();
			}
			else
			{
				if ( grow )
					AnimateGrow();
				else
					AnimateBurst();
			}
		}
	}

	public void SetVisible(bool i_vis)
	{
		visible = i_vis;
		if ( !visible )
		{
			wave.renderer.enabled = false;
			icon.renderer.enabled = false;
			growTimer.Pause();
			placeTimer.Pause();
		}

	}

	private void Place()
	{
		Action timerEndAction = delegate(){EndPlace();};
		placeTimer = gameObject.AddComponent<GenericTimer>();
		placeTimer.Set( 2.0f, false, timerEndAction );
		placeTimer.Run ();
	}

	private void EndTimer()
	{
		//Debug.Log ("END TIMER: " + ((grow)?"GROW" : "BURST"));
		if ( grow )
		{
			grow = false;
			wave.renderer.enabled = true;
		}
		else
		{
			grow = true;
			wave.renderer.enabled = false;
		}
	}

	private void AnimatePlace()
	{
		float placeScale = 10.2f;
		wave.localScale = new Vector3(placeScale, placeScale, placeScale);
		float percent = placeTimer.PercentCompleteEaseIn();
		float tint = 0.2f - (percent * 0.2f);
		wave.renderer.material.SetColor ("_TintColor", new Color(tint, tint, tint, 1-percent*2));
	}
	
	private void EndPlace()
	{
		place = false;
		Destroy(placeTimer);
		growTimer.Run ();
	}

	private void AnimateGrow()
	{
		float percent = growTimer.PercentCompleteEaseOut();
		float tint = 0.2f + (percent * 0.3f);
		icon.renderer.material.SetColor("_TintColor", new Color( tint, tint, tint, 0.5f));
	}

	private void AnimateBurst()
	{
		float percent = growTimer.PercentCompleteEaseOut();
		float w_scale;
		if ( percent < .5f)
			w_scale = (percent*10 + 0.5f);
		else
			w_scale = 5.5f;

		wave.localScale = new Vector3(w_scale, w_scale, w_scale);
		wave.renderer.material.SetColor ("_TintColor", new Color(.1f, .1f, .1f, 1-percent*2));
	
		float tint = 0;
		if ( percent < 0.3)
			tint = 0.5f - percent;
		else
			tint = 0.2f;

		icon.renderer.material.SetColor("_TintColor", new Color( tint, tint, tint, 0.5f));
	}
}
