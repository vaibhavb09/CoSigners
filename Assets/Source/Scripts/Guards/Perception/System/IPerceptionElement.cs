using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface IPerceptionElement 
{
	/// <summary>
	/// Indicates if the perception element is to be skipped
	/// </summary>
	bool SkiponHigherSuccess
	{
		get;
		set;
	}

	/// <summary>
	/// The detection bias that this element applies
	/// </summary>
	float DetectionBias
	{
		get;
		set;
	}

	/// <summary>
	/// Does an internal check to determine if any bias is to be applied and if required, returns the bias
	/// </summary>
	/// <returns>Returns a pair with the first value indicating the checks success or failure and hte second value indicating the bias</returns>
	KeyValuePair<bool,float> checkAndReturnBias(GameObject iObjectToBeChecked,GameObject iCheckingObject);
}

