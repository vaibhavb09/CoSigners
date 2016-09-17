using UnityEngine;
using System.Collections;

public class PerceptionHelpers 
{
	/// <summary>
	/// The mask used for raycast collision checks
	/// </summary>
	private LayerMask mRayCastMask;

	/// <summary>
	/// The Singleton obkect self.
	/// </summary>
	static PerceptionHelpers mSelf;

	public static PerceptionHelpers Self
	{
		get
		{
			if (mSelf == null)
			{
				mSelf = new PerceptionHelpers();
			}
			return mSelf;
		}
	}

	public PerceptionHelpers()
	{
		// Clear the Raycast Mask
		mRayCastMask = 0;
		
		// Add the Ignore raycast layer to the mask (Layer to name doesnt work for some reason !)
		mRayCastMask = 1 << 2;
		
		// Add the ignore guards layer to the mask (Layer to name doesnt work for some reason !)
		mRayCastMask = 1 << 10;
		
		// Ad dthe see through ping layer to the ignore for guards
		mRayCastMask = 1 << 11;
		
		// invert the mask (the mask now collides with everything that is not on the "ignoreGuards" and "Ignore Raycast" layer
		mRayCastMask = ~mRayCastMask;

	}

	/// <summary>
	/// Does a Proximity check on two positions depending on the radial check threshold that was passed in
	/// </summary>
	/// <returns><c>true</c>, If the vectors are in the threshold distance of each other, <c>false</c> otherwise.</returns>
	/// <param name="iPositionToBeChecked">The position to be checked.</param>
	/// <param name="iReferencePosition">The Position from which distance is to be checked.</param>
	/// <param name="iRadialCheckDistance"> The distance that this radial check needs to look </param>
	public bool doProximitycheck(Vector3 iPositionToBeChecked,Vector3 iReferencePosition,float iRadialCheckDistance)
	{
		// Dist Squared check ?
		float _distance = Vector3.Distance(iPositionToBeChecked,iReferencePosition);

		// If the Point is in requisite distance
		if(_distance < iRadialCheckDistance)
			// Indicate success
			return true;
		else
			// Indicate failure
			return false;
	}

	/// <summary>
	/// Does a cone check to check if the player position is in the vision cone
	/// </summary>
	/// <returns><c>true</c>, if iPlayerPosition is in the view cone <c>false</c> otherwise.</returns>
	/// <param name="iConeForwardVector">The vision cone forward vector.</param>
	/// <param name="iReferencePosition">The Position from which the vision is to be checked.</param>
	/// <param name="iPositionToBeChecked">The position to be checked.</param>
	/// <param name="iHalfConeAngle">Half the angle of the vision cone </param>
	public float doConeCheck(Vector3 iConeForwardVector, Vector3 iReferencePosition,
	                        Vector3 iPositionToBeChecked, float iHalfConeAngle,
	                        float iConeDistance)
	{
		// Convert vectors to 2D [Memory allocations in a tight ass loop , BAD ]
		Vector2 _fwd2D = new Vector2(iConeForwardVector.x,iConeForwardVector.z);
		Vector2 _refPos2D = new Vector2(iReferencePosition.x,iReferencePosition.z);
		Vector2 _checkPosition2D = new Vector2(iPositionToBeChecked.x,iPositionToBeChecked.z);

		float dot = Vector2.Dot(_fwd2D.normalized, (_refPos2D - _checkPosition2D).normalized);
		// check if the "iPlayerPosition" is in the vision cone described by "iGuardPosition" , "iGuardForwardVector" and "iHalfConeAngle"
		if(dot > Mathf.Cos(iHalfConeAngle ))
		{
			// if successful , caclulate bias factor
			float _distance = Vector2.Distance(_refPos2D,_checkPosition2D);
			float biasFactor = _distance/iConeDistance;
			if(biasFactor < 1)
				return biasFactor;
			else
				return 0;
		}
		else
			// If the test failed then return a bias of 0
			return 0;
	}

	public bool doEllipseCheck()
	{
		throw new UnityException("Method not implemened yet");
	}

	/// <summary>
	/// Does the ray cast check and finds if the Players Position can be seen from the Guards position without any obstruction
	/// </summary>
	/// <returns><c>true</c> If the player is visible <c>false</c> otherwise.</returns>
	/// <param name="iReferencePosition">The origin of the raycast check.</param>
	/// <param name="iPositionToBeChecked">The end of the raycast check.</param>
	/// <param name="Ytolerance"> Indicates bias in the Y position of the position that is to be checked </param>
	/// <param name="maxDistance"> Indicates the maximum distance that needs to be checked on the raycast </param>
	/// <param name="iObjToBeSeen"> Indicates the object that is being sought in this raycast check </param>
	public bool doRayCastCheck(Vector3 iReferencePosition, 
								Vector3 iPositionToBeChecked
	                           , float Ytolerance
	                           , float maxDistance
	                           , GameObject iObjToBeSeen)
	{
		
		GameObject FPSCamera = GameObject.Find("FPSCamera");

		//Debug.Log("Y POSITION ******************** " + FPSCamera!=null? iPositionToBeChecked.y + FPSCamera.transform.localPosition.y : -1000);



		iReferencePosition= new Vector3(iReferencePosition.x, 1.5f , iReferencePosition.z);
		Vector3 posToBeChacked = new Vector3(iPositionToBeChecked.x,FPSCamera!=null? iPositionToBeChecked.y + FPSCamera.transform.localPosition.y : iPositionToBeChecked.y , iPositionToBeChecked.z);

		if(iObjToBeSeen.GetComponent<MovementScript>().CrouchMode)
			posToBeChacked.y = 0.65f;
		else
			posToBeChacked.y = 1.75f;


		// Calculate the direction for the raycast
		Vector3 _raycastDirection = posToBeChacked - iReferencePosition;
		_raycastDirection.Normalize();

		// Temporarily stores the raycast hit point information
		RaycastHit _hitInfo;

		if(Physics.Raycast(iReferencePosition, _raycastDirection,out _hitInfo,maxDistance,mRayCastMask))
		{
			if(_hitInfo.collider.gameObject == iObjToBeSeen)
				return true;
			else
				return false;
		}
		else
			return false;

	}
}

