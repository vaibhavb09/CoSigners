using UnityEngine;
using System.Collections;
using System;

/// MouseLook rotates the transform based on the mouse delta.
/// Minimum and Maximum values can be used to constrain the possible rotation

/// To make an FPS style character:
/// - Create a capsule.
/// - Add the MouseLook script to the capsule.
///   -> Set the mouse look to use LookX. (You want to only turn character but not tilt it)
/// - Add FPSInputController script to the capsule
///   -> A CharacterMotor and a CharacterController component will be automatically added.

/// - Create a camera. Make the camera a child of the capsule. Reset it's transform.
/// - Add a MouseLook script to the camera.
///   -> Set the mouse look to use LookY. (You want the camera to tilt up and down like a head. The character already turns.)
[AddComponentMenu("Camera-Control/Mouse Look")]
public class MouseLookAround : MonoBehaviour {

	public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
	public RotationAxes axes = RotationAxes.MouseXAndY;
	public float sensitivityX = 15F;
	public float sensitivityY = 15F;

	public float minimumX = -360F;
	public float maximumX = 360F;

	public float minimumY = -60F;
	public float maximumY = 60F;
	
	private float startAngle = 0f;
	private float endAngle = 0F;
	private float startOffset = 0f;
	private float endOffset = 0f;	
	

	float rotationY = 0F;
	private float peekDirection;
	private bool processPeeking = false;
	GameObject _camera;
	GenericTimer peekTimer;


	void Update ()
	{
		if(GameManager.Manager.PlayerType == 1) //if the player is the point man
		{
			var _joystick = false;
			if( Input.GetJoystickNames().Length != 0 )
			{
				_joystick = true;
			}

//			if( _joystick )
//			{
//				if (axes == RotationAxes.MouseXAndY)
//				{
//					float rotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse XXBox360") * sensitivityX;
//					
//					rotationY += Input.GetAxis("Mouse YXBox360") * sensitivityY;
//					rotationY = Mathf.Clamp (rotationY, minimumY, maximumY);
//					
//					transform.localEulerAngles = new Vector3(-rotationY, rotationX, transform.localEulerAngles.z);
//				}
//				else if (axes == RotationAxes.MouseX)
//				{
//					Debug.Log("Joystick Working X");
//					transform.Rotate(transform.localEulerAngles.x, Input.GetAxis("Mouse XXBox360") * sensitivityX, transform.localEulerAngles.z);
//				}
//				else if( axes == RotationAxes.MouseY )
//				{
//					Debug.Log("Joystick Working Y");
//					rotationY += Input.GetAxis("Mouse YXBox360") * sensitivityY;
//					rotationY = Mathf.Clamp (rotationY, minimumY, maximumY);
//					
//					transform.localEulerAngles = new Vector3(-rotationY, transform.localEulerAngles.y, transform.localEulerAngles.z);
//				}
//			}
//			else
			{
				if (axes == RotationAxes.MouseXAndY)
				{
					float rotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivityX;
					
					rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
					rotationY = Mathf.Clamp (rotationY, minimumY, maximumY);
					
					transform.localEulerAngles = new Vector3(-rotationY, rotationX, transform.localEulerAngles.z);
				}
				else if (axes == RotationAxes.MouseX)
				{
					transform.Rotate(transform.localEulerAngles.x, Input.GetAxis("Mouse X") * sensitivityX, transform.localEulerAngles.z);
				}
				else if( axes == RotationAxes.MouseY )
				{
					rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
					rotationY = Mathf.Clamp (rotationY, minimumY, maximumY);
					
					transform.localEulerAngles = new Vector3(-rotationY, transform.localEulerAngles.y, transform.localEulerAngles.z);
				}
			}
			#region peeking
			if ( processPeeking )
				ProcessPeekAmount();

			#endregion
		}
		
	}
	
	
	public void StartPeek(bool peekRight)
	{		
		//Debug.Log ("Starting Peek");
		processPeeking = true;
		startAngle = 0;
		
		// If peeking left
		if ( !peekRight )
		{
			startAngle = 0f;
			endAngle = 30f;
			startOffset = 0f;
			endOffset = -0.5f;
		}
		// If peeking left
		else
		{
			startAngle = 359.9f;
			endAngle = 330f;
			startOffset = 0f;
			endOffset = 0.5f;
		}
		
		Action timerEndAction = delegate(){PausePeek();};
		if ( peekTimer == null )
			peekTimer = gameObject.AddComponent<GenericTimer>();
		peekTimer.Set( 0.5f, false, timerEndAction );
		peekTimer.Run();
	}
	
	
	public void EndPeek(bool peekRight)
	{
		//Debug.Log ("Ending Peek");
		processPeeking = true;
		startAngle = transform.localEulerAngles.z;
		endAngle = (peekRight)?359.9f : 0.0f;
		startOffset = _camera.transform.localPosition.x;
		endOffset = 0;
		
		Action timerEndAction = delegate(){PausePeek();};
		if ( peekTimer == null )
			peekTimer = gameObject.AddComponent<GenericTimer>();
		peekTimer.Set( 0.5f, false, timerEndAction );
		peekTimer.Run();
	}
	
	
	public void PausePeek()
	{
		processPeeking = false;
	}
	
	
	private void ProcessPeekAmount()
	{
		Vector3 tmpAngle = transform.localEulerAngles;
		//tmpAngle.z = Mathf.Lerp (startAngle, endAngle, peekTimer.PercentComplete() );
		tmpAngle.z = Mathf.Lerp (startAngle, endAngle, peekTimer.PercentCompleteEaseOut() );
		transform.localEulerAngles = tmpAngle;
		
		//float xTranslate = Mathf.Lerp ( startOffset, endOffset, peekTimer.PercentComplete() );
		float xTranslate = Mathf.Lerp ( startOffset, endOffset, peekTimer.PercentCompleteEaseOut() );
		_camera.transform.localPosition = new Vector3 ( xTranslate, 0.7f, 0.0f);
	
	}
	
	
	void Start ()
	{
		// Make the rigid body not change rotation
		if (rigidbody)
			rigidbody.freezeRotation = true;
		
		_camera = GameObject.Find("FPSCamera");
		
		if(GameManager.Manager.PlayerType == 1)
		{
			gameObject.AddComponent("AudioListener");
		}
	}
}