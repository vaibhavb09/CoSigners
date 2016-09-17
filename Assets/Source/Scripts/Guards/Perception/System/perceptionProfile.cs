using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class perceptionProfile
{
	/// <summary>
	/// Stores all perception elements that make up this perception profile
	/// </summary>
	public List<IPerceptionElement> mAllPerceptionElements;

	/// <summary>
	/// Converts the charge to alert [CONVERT TO DELEGATE]
	/// </summary>
	/// <returns>The alert value depending on current charge </returns>
	/// <param name="iCharge">I charge.</param>
	public float convertChargeToAlert(float iCharge)
	{
		return iCharge / 10;
	}

	/// <summary>
	/// Adds the perception element.
	/// </summary>
	/// <param name="iNewPerceptionElement">The New perception element to be added </param>
	public void addPerceptionElement(IPerceptionElement iNewPerceptionElement)
	{
		mAllPerceptionElements.Add(iNewPerceptionElement);
	}



	public perceptionProfile()
	{
		mAllPerceptionElements = new List<IPerceptionElement>();
	}
}

