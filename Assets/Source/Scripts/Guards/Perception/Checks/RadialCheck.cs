using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RadialCheck : IPerceptionElement
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

	private float mRadius;
	public float Radius
	{
		get
		{
			return mRadius; 
		}
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="RadialCheck"/> class.
	/// </summary>
	/// <param name="iSkipOnHigherSuccess">If set to <c>true</c> this element will skip on higher elements success.</param>
	/// <param name="iDetectionBias">The detection bias that this element applies</param>
	/// <param name="iRadius">The radius of the check</param>
	public RadialCheck(bool iSkipOnHigherSuccess,
	                   float iDetectionBias,
	                   float iRadius)
	{
		SkiponHigherSuccess = iSkipOnHigherSuccess;
		DetectionBias = iDetectionBias;
		mRadius = iRadius;
	}

	/// <summary>
	/// Does an internal check to determine if any bias is to be applied and if required, returns the bias
	/// </summary>
	/// <returns>Returns a pair with the first value indicating the checks success or failure and hte second value indicating the bias</returns>
	public KeyValuePair<bool,float> checkAndReturnBias(GameObject iObjectToBeChecked,GameObject iCheckingObject)
	{

		
		KeyValuePair<bool,float> result = new KeyValuePair<bool, float>(false,0);
		
		if(iObjectToBeChecked != null && iCheckingObject != null)
		{
			result = new KeyValuePair<bool, float>(PerceptionHelpers.Self.doProximitycheck(
				iObjectToBeChecked.transform.position,iCheckingObject.transform.position,Radius),DetectionBias);
		}


		if(result.Key)
		{
			//Debug.Log("CONE CHECK ******************************* SUCCESSFUL");
		}

		return result;
	}
}

