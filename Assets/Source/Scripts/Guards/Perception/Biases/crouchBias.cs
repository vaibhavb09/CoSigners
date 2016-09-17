using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class crouchBias : IPerceptionElement
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

	public crouchBias(bool iSkipOnHigherSuccess,
	                  float iDetectionBias)
	{
		SkiponHigherSuccess = iSkipOnHigherSuccess;
		DetectionBias = iDetectionBias;
	}


	/// <summary>
	/// Does an internal check to determine if any bias is to be applied and if required, returns the bias
	/// DEPENDS ON iObjectToBeChecked being a player that has a "MovementScript" and provides valid "_wasCrawling"
	/// </summary>
	/// <returns>Returns a pair with the first value indicating the checks success or failure and the second value indicating the bias</returns>
	public KeyValuePair<bool,float> checkAndReturnBias(GameObject iObjectToBeChecked,GameObject iCheckingObject)
	{
		KeyValuePair<bool,float> result = new KeyValuePair<bool, float>(false,0);
		
		if(iObjectToBeChecked != null)
		{
			// Always returning true so as to allow the raycast to execute
			result = new KeyValuePair<bool, float>(true,iObjectToBeChecked.GetComponent<MovementScript>().CrouchMode ? DetectionBias : 0);
		}

//		if(result.Value == DetectionBias)
//			Debug.Log("CROUCH BIAS APPLIED *************************** ");


		return result;
	}
}

