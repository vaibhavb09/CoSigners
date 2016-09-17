using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LineOfSightBias : IPerceptionElement
{
	/// <summary>
	/// Indicates if the perception element is to be skipped
	/// </summary>
	public bool SkiponHigherSuccess
	{
		get;
		set;
	}
	
	/// <summary>
	/// The detection bias that this element applies
	/// </summary>
	public float DetectionBias
	{
		get;
		set;
	}

	// Default to a 60 degree angle
	private float mToleranceAngle = Mathf.Cos( 60 * (Mathf.PI / 180));

	public LineOfSightBias(bool iSkiponHigherSuccess,float iDetectionBias,float iToleranceAngleInDegrees)
	{
		SkiponHigherSuccess = iSkiponHigherSuccess;
		DetectionBias = iDetectionBias;

		if(iToleranceAngleInDegrees > 90)
		{
			//Debug.LogError("Tolerance angle in Degrees cannot be more than 90 degrees");
		}
		mToleranceAngle = Mathf.Cos( iToleranceAngleInDegrees * (Mathf.PI / 180));
	}
	
	/// <summary>
	/// Does an internal check to determine if any bias is to be applied and if required, returns the bias
	/// </summary>
	/// <returns>Returns a pair with the first value indicating the checks success or failure and the second value indicating the bias</returns>
	public KeyValuePair<bool,float> checkAndReturnBias(GameObject iObjectToBeChecked,GameObject iCheckingObject)
	{
		KeyValuePair<bool,float> result = new KeyValuePair<bool, float>(false,0);
		
		if(iObjectToBeChecked != null && iCheckingObject != null)
		{
			// TODO : Change the bias from a flat value to a distribution
			float cosineOfAngle = Vector2.Dot(iObjectToBeChecked.transform.forward.normalized,iCheckingObject.transform.forward.normalized);

			// Always returning true so as to allow next elements to execute
			result = new KeyValuePair<bool, float>( true, (cosineOfAngle > mToleranceAngle) ? DetectionBias : 0);
		}

		if(result.Value == DetectionBias)
		{
			//Debug.Log("LINE OF SIGHT BIAS APPLIED *************************** ");
		}

		return result;
	}
}

