using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ConeCheck : IPerceptionElement
{
	/// <summary>
	/// Indicates if the perception element is to be skipped on the success of the previous Element
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

	/// <summary>
	/// Indicates the dinstance after which this check is invalid
	/// </summary>
	private float mDistance;
	public float MaxDistance
	{
		get
		{
			return mDistance; 
		}
	}

	/// <summary>
	/// Indicates the total angle (in RADIANS) that this cone spans
	/// </summary>
	private float mHalfConeAngleRadians;
	public float HalfConeAngleRadians
	{
		get
		{
			return mHalfConeAngleRadians;
		}
	}
	
	/// <summary>
	/// Initializes a new instance of the <see cref="RadialCheck"/> class.
	/// </summary>
	/// <param name="iSkipOnHigherSuccess">If set to <c>true</c> this element will skip on higher elements success.</param>
	/// <param name="iDetectionBias">The detection bias that this element applies</param>
	/// <param name="iDistance">Indicates the dinstance after which this check is invalid</param>
	/// <param name="iConeAngleDegrees">Indicates the total angle (in DEGREES) that this cone spans</param>
	public ConeCheck(bool iSkipOnHigherSuccess,
	                 float iDetectionBias,
	                 float iDistance,
	                 float iConeAngleDegrees)
	{
		SkiponHigherSuccess = iSkipOnHigherSuccess;
		DetectionBias = iDetectionBias;
		mDistance = iDistance;
		mHalfConeAngleRadians = (iConeAngleDegrees * Mathf.PI / 180) / 2;
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
			float biasFactor = PerceptionHelpers.Self.doConeCheck(iCheckingObject.transform.forward,
			                                                      iObjectToBeChecked.transform.position,iCheckingObject.transform.position,mHalfConeAngleRadians,MaxDistance);

			result = new KeyValuePair<bool, float>(biasFactor == 0 ? false : true,DetectionBias * biasFactor);
		}

		if(result.Key)
		{
			//Debug.Log("CONE CHECK ******************************* SUCCESSFUL, ANGLE" + ( ( mHalfConeAngleRadians * 180 / Mathf.PI ) * 2 ));
		}
		return result;
	}

}

