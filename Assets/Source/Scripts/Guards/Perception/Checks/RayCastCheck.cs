using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RayCastCheck : IPerceptionElement
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

	private float mRayCastYOffset;
	public float RayCastYOffset
	{
		get
		{
			return mRayCastYOffset;
		}
	}

	public RayCastCheck(bool iSkipOnHigherSuccess, float iDetectionBias,float iRayCastYOffset)
	{
		SkiponHigherSuccess = iSkipOnHigherSuccess;
		DetectionBias = iDetectionBias;
		mRayCastYOffset = iRayCastYOffset;
	}

	/// <summary>
	/// Does an internal check to determine if any bias is to be applied and if required, returns the bias
	/// DEPENDS ON iObjectToBeChecked being a player that has a "MovementScript" and provides valid "_wasCrawling"
	/// </summary>
	/// <returns>Returns a pair with the first value indicating the checks success or failure and the second value indicating the bias</returns>
	public KeyValuePair<bool,float> checkAndReturnBias(GameObject iObjectToBeChecked,GameObject iCheckingObject)
	{
		KeyValuePair<bool,float> result = new KeyValuePair<bool, float>(false,0);

		if(iObjectToBeChecked != null && iCheckingObject != null)
		{

			result = new KeyValuePair<bool, float>(PerceptionHelpers.Self.doRayCastCheck(iCheckingObject.transform.position,
			                                  iObjectToBeChecked.transform.position , mRayCastYOffset, Mathf.Infinity , PlayerAccess.Self.Player)
			                                                                ,DetectionBias);
		}

//		if(result.Key)
//			Debug.Log("RAYCAST ******************************* SUCCESSFUL");
		return result;
	}
}

