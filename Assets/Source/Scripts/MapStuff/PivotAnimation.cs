using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class PivotAnimation : MonoBehaviour {
	
	List<Pivot> myPivots;
	List<float> startAngles;
	List<float> destAngles;
	Vector3 center;
	float rotation, controlRotation, contStartAngle, contDestAngle, durration, myTimer;
	Action endAction;
	bool run;
	
	
	// Use this for initialization
	void Start () {
	}
	
	
	/// -----------------------------------------------------------------------------
	/// SET
	/// <summary>Sets all the animation parameters for the pivot</summary>
	/// Params : 	(Pivot) 	The Pivot object to be animated
	/// 			(Vector3) 	The center (or pivot point) of the rotation in world coordinates
	/// 			(float)		The number of euler degrees to rotate (this also indicates rotation direction)
	/// 			(float)		The number of seconds that the animation shoudl last
	/// 			(Action)	A delagate function to be called once the animation has completed.
	/// 			(float)		The number of euler degrees to rotate the Pivot control point.
	/// Return : none
	/// -----------------------------------------------------------------------------
	public void Set ( List<Pivot> i_pivots, Vector3 i_center, float i_rotation, float i_durration, Action i_action, float i_controlRotation )
	{
		myPivots = i_pivots;
		startAngles = new List<float>();
		destAngles = new List<float>();
		center = i_center;
		rotation = i_rotation;
		controlRotation = i_controlRotation;
		durration = i_durration;
		myTimer = 0;
		endAction = i_action;
		run = false;
	}
	
	
	/// -----------------------------------------------------------------------------
	/// UPDATE
	/// <summary>Runs the Timer once the animation is active</summary>
	/// 		This function also completes the animation once timer fills, and calls the ending Action.
	/// Params : none
	/// Return : none
	/// -----------------------------------------------------------------------------
	void Update () {
		if ( run )
		{
			myTimer += Time.deltaTime;
			if ( myTimer > durration )
			{
				for ( int i=0 ; i<myPivots.Count ; i++ )
				{
					rotation = destAngles[i] - myPivots[i].myConnections.vectorObject.transform.rotation.eulerAngles.y;
					myPivots[i].myConnections.vectorObject.transform.RotateAround( center, Vector3.down, -rotation);
					endAction();
				}
				//rotation = destAngle - myPivot.myConnections.vectorObject.transform.rotation.eulerAngles.y;			
				//myPivot.myConnections.vectorObject.transform.Translate( (center) );
				//myPivot.myConnections.vectorObject.transform.Rotate( Vector3.down , -rotation, Space.World);
				//myPivot.myConnections.vectorObject.transform.Translate( -center );
				
				//controlRotation = contDestAngle - myPivot.pivotControl.transform.rotation.eulerAngles.y;
				//myPivot.pivotControl.transform.Rotate(new Vector3(0.0f, controlRotation, 0.0f) );
				
				//Debug.Log ("**********TIMER FINISHED********** Pivot: " + myPivot.index );
				//Destroy(this);
			}
			else
			{
				Animate();
			}
		}
	}
	
	
	/// -----------------------------------------------------------------------------
	/// ANIMATE
	/// <summary>Performs the actual pivot and control point animations</summary>
	/// Params : none
	/// Return : none
	/// -----------------------------------------------------------------------------
	void Animate()
	{
		//float thisRotation = (startAngle + ((destAngle - startAngle)*PercentComplete()));		
		
		for ( int i=0 ; i<myPivots.Count ; i++ )
		{
			float thisRotation = (startAngles[i] + ((destAngles[i] - startAngles[i])*PercentComplete()));
			thisRotation = thisRotation - myPivots[i].myConnections.vectorObject.transform.rotation.eulerAngles.y;
			myPivots[i].myConnections.vectorObject.transform.RotateAround( center, Vector3.down, -thisRotation);
		}
		//myPivot.myConnections.vectorObject.transform.Translate( (center) );
		//myPivot.myConnections.vectorObject.transform.Rotate( Vector3.down , -thisRotation, Space.World);
		//myPivot.myConnections.vectorObject.transform.Translate( -center );
		
		//float thisContRotation = (contStartAngle + ((contDestAngle - contStartAngle)*PercentComplete()));
		//thisContRotation = thisContRotation - myPivot.pivotControl.transform.rotation.eulerAngles.y;
		//myPivot.pivotControl.transform.Rotate(new Vector3(0.0f, thisContRotation, 0.0f) );
	}
	
	
	/// -----------------------------------------------------------------------------
	/// RUN
	/// <summary>Begins running the animation timer and sets some initial variables</summary>
	/// Params : none
	/// Return : none
	/// -----------------------------------------------------------------------------
	public void Run()
	{
		run = true;
		for ( int i=0 ; i<myPivots.Count ; i++ )
		{
			startAngles.Add (0);
			destAngles.Add (0);
		}
		
		for ( int i=0 ; i<myPivots.Count ; i++ )
		{
			startAngles[i] = myPivots[i].myConnections.vectorObject.transform.rotation.eulerAngles.y;
			destAngles[i] = startAngles[i] + rotation;
		}
		
		//contStartAngle = myPivot.pivotControl.transform.rotation.eulerAngles.y;
		//contDestAngle = contStartAngle + controlRotation;
	}
	
	
	public void Stop()
	{
		run = false;
	}
	
	/// -----------------------------------------------------------------------------
	/// UPDATE
	/// <summary>Calculates the percent complete of the animation timer</summary>
	/// 			This may in the future modify the percentage based on some smoothing calculation. 
	/// Params : none
	/// Return : none
	/// -----------------------------------------------------------------------------
	public float PercentComplete()
	{
		return myTimer/durration;
	}
	
	/*
	public Pivot GetPivot()
	{
		return myPivot;
	}*/
}
