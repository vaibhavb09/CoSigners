using UnityEngine;
using System.Collections;
using System;

public class TracerAnimationNew : MonoBehaviour {

	private float calibrationTime;
	private float activeTime;
	private float radarTime;

	bool start;
	bool calibrating;
	bool active;

	private Transform ring;
	private Transform TracerBase;
	private Transform TracerTimer;
	private Transform TracerCover;

	public Material ActiveBase;
	public Material ActiveCover;

	private GenericTimer startTimer;
	private GenericTimer calibrateTimer;
	private GenericTimer activeTimer;

	private Tracer myTracer;

	// Use this for initialization
	void Start () {
	}

	// Calibration time must be at leat 0.7f seconds
	public void Set(float i_calTime, float i_activeTime, Tracer i_tracer)
	{
		//LoadMaterials();
		
		foreach ( Transform child in transform )
		{
			//Debug.Log ("Child is: " + child.name);
			if ( child.name.Equals("Tracer_Base"))
				TracerBase = child;
			else if ( child.name.Equals("Tracer_Cover"))
				TracerCover = child;
			else if ( child.name.Equals("Tracer_Ring"))
				ring = child;
			else if ( child.name.Equals("Tracer_Timer"))
				TracerTimer = child;
		}
		
		start = false;
		calibrating = false;
		active = false;
		TracerTimer.transform.renderer.enabled = false;
		ring.transform.renderer.enabled = false;
		
		calibrationTime = i_calTime - 0.6f;
		activeTime = i_activeTime;
		radarTime = 2.0f;

		myTracer = i_tracer;
	}

	public void RemoveTracer()
	{
		Destroy (this);
	}

	public void Run()
	{
		ring.transform.renderer.enabled = true;
		start = true;
	}

	private void LoadMaterials()
	{
		ActiveBase = Resources.Load("Materials/Hacker/Hexes/Tracer_Active_Base", typeof(Material)) as Material;
		ActiveCover = Resources.Load("Materials/Hacker/Hexes/Tracer_Active_Cover", typeof(Material)) as Material;
	}

	// Update is called once per frame
	void Update () {
		if ( start )
			PlayStartAnimation();

		if ( calibrating )
		{
			PlayCalibration();
		}
		else if ( active )
		{
			PlayActiveTimer();
		}
	}

	private void PlayStartAnimation()
	{
		float percent = myTracer.calibrationTicker/0.6f;
		//Debug.Log ("PERCENT = " + percent + "Calibration Ticker = " + myTracer.calibrationTicker);
		float r_scale = (1-percent)*3;

		if ( percent > 0.8f )
		{
			ring.transform.renderer.enabled = false;
			if ( !calibrating )
			{
				calibrating = true;
				TracerTimer.transform.renderer.enabled = true;
				EndStartTimer();
			}
		}
	

		{
			float b_alpha = 1- (( 0.8f - percent) * 5);
			TracerBase.renderer.material.SetColor ("_TintColor", new Color(.5f, .5f, .5f, b_alpha ));
			TracerCover.renderer.material.SetColor ("_TintColor", new Color(.5f, .5f, .5f, b_alpha ));
		}

		ring.localScale = new Vector3(r_scale, r_scale, r_scale);
	}

	private void EndStartTimer()
	{
		Destroy( startTimer );
		//Debug.Log("End Start Animation");
		start = false;
	}

	private void PlayCalibration()
	{
		float percent = myTracer.calibrationTicker/myTracer.calibratingTime;
		float calTime = (1 - ((percent-0.02f)));
		TracerTimer.renderer.material.SetFloat("_Cutoff", calTime );

		if ( percent > 0.85f)
		{
			float t_scale = ((percent-0.85f)*7);
			TracerTimer.localScale = new Vector3(1+t_scale, 1+t_scale, 1+t_scale);

			TracerCover.transform.renderer.material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, 1-((percent-0.85f)*7)) );
		}

		TracerBase.transform.renderer.material.SetColor("_TintColor", new Color(percent/2 + 0.5f, 0.5f-percent/4, 0.5f-percent/4, 1 ));
	}

	public void EndCalibrationTimer()
	{
		//Debug.Log("END CALIBRATION TIMER");
		//Destroy( calibrateTimer );
		TracerTimer.renderer.enabled = false;
		TracerCover.renderer.material = ActiveCover;
		TracerBase.renderer.material = ActiveBase;

		calibrating = false;
		active = true;
	}

	private void PlayActiveTimer()
	{
		float endAngle = myTracer.activeTicker%2.0f/2.0f * 360.0f;
		//Debug.Log ("PERCENT = " + endAngle + "Active Ticker = " + myTracer.activeTicker);

		TracerCover.localRotation = Quaternion.Euler( new Vector3( 90.0f, endAngle, 0.0f ) );
	}

	public void EndActiveTimer()
	{
		//Debug.Log ("END ACTIVE TIMER");
	}
}
