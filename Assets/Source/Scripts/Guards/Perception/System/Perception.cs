//#define _PERCEPTION_DEBUG_SPEW

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Perception : MonoBehaviour
{
	// Mantains a list of all available guard perception profiles
	private List<perceptionProfile> mAllPerceptionProfiles;

	// Indicates the perception profile in use currently
	private int currentPerceptionProfile = 0;

	// Indicates the voltage applied in charge calculations
	private const float mVoltage = 1000;

	// Indicates the bias applied to voltage
	// Used to Increase (1) / Decrease (-1) or Keep Charge constant (0)
	private float mVoltageBias = 1.0f;

	// Indicates the resistance applied in charge calculations
	private float mResistance = 1;

	public float YOffsetInDefaultPrf = 0.3f;
	private bool enabled = true;

	// strores the fps camera for the player
	GameObject FPSCamera;

	private bool clearToChargeThreatMeter = true;

	private Vector3 thieflastposition;
	public Vector3 ThiefLastPosition
	{
		get{
			return thieflastposition;
		}
	}

	// The total charge value
	private float mCharge = 0;
	private float Charge
	{
		get
		{
			return mCharge;
		}
	}

	/// <summary>
	/// Gets the alert level of the guard as a number between 1 and 100
	/// </summary>
	/// <value>The alert level.</value>
	public float AlertLevel
	{
		get
		{
			return mAllPerceptionProfiles[currentPerceptionProfile].convertChargeToAlert(Charge);
		}
	}

	/// <summary>
	/// Gets a value indicating whether this <see cref="Perception"/> guard is alert.
	/// </summary>
	/// <value><c>true</c> if guard is alert; otherwise, <c>false</c>.</value>
	public bool isGuardAlert
	{
		get
		{
			return (Charge > 0);
		}
	}

	private float mPrevCharge = 0;
	

	void OnGUI(){
		//GUI.Label(new Rect(0,0,200,200)," Guard profile in use : " + currentPerceptionProfile);
	}

	// Use this for initialization
	void Start ()
	{

		mAllPerceptionProfiles = new List<perceptionProfile>();

		FPSCamera = GameObject.Find("FPSCamera");

		#region profile 1
			perceptionProfile profile1 = new perceptionProfile();
			profile1.addPerceptionElement(new RadialCheck(true,-2,1.5f));
			profile1.addPerceptionElement(new ConeCheck(true,1,30,60));	
			profile1.addPerceptionElement(new ConeCheck(true,1,20,120));
			profile1.addPerceptionElement(new crouchBias(false,0.5f));
			profile1.addPerceptionElement(new LineOfSightBias(false,0.5f,60));	
			profile1.addPerceptionElement(new RayCastCheck(false,2,0.4f));
			mAllPerceptionProfiles.Add(profile1);
		#endregion

		#region profile 2
			perceptionProfile profile2 = new perceptionProfile();
			profile2.addPerceptionElement(new RadialCheck(true,-2,3));
			profile2.addPerceptionElement(new ConeCheck(true,1,30,60));	
			profile2.addPerceptionElement(new ConeCheck(true,1,20,120));
			profile2.addPerceptionElement(new crouchBias(false,1f));
			profile2.addPerceptionElement(new LineOfSightBias(false,1f,60));	
			profile2.addPerceptionElement(new RayCastCheck(false,3,YOffsetInDefaultPrf));
			mAllPerceptionProfiles.Add(profile2);
		#endregion

		#region profile 3
			perceptionProfile profile3 = new perceptionProfile();
			profile3.addPerceptionElement(new RadialCheck(true,-2,3));
			profile3.addPerceptionElement(new ConeCheck(true,1,10,60));
			profile3.addPerceptionElement(new ConeCheck(true,1,20,120));
			profile3.addPerceptionElement(new crouchBias(false,1));
			profile3.addPerceptionElement(new LineOfSightBias(false,1,60));
			profile3.addPerceptionElement(new RayCastCheck(false,2,0.0f));
			mAllPerceptionProfiles.Add(profile3);
		#endregion

//		lineRenderer = gameObject.AddComponent<LineRenderer>();
//		lineRenderer.useWorldSpace = true;

	}

	// Update is called once per frame
	void Update ()
	{
		if(FPSCamera == null)
		{
			FPSCamera = GameObject.Find("FPSCamera");
		}


		if(enabled)
		{
		// Reset the last check success value
		bool lastCheckSuccessValue = false;

		// Resetting the resistance
		mResistance = 0;

		for(int i = 0; i < mAllPerceptionProfiles[currentPerceptionProfile].mAllPerceptionElements.Count ; i++)
		{

			// If this perception check is to be run
			if(lastCheckSuccessValue != mAllPerceptionProfiles[currentPerceptionProfile].mAllPerceptionElements[i].SkiponHigherSuccess)
			{
				// Do the check
				KeyValuePair<bool,float> checkResultAndBias = mAllPerceptionProfiles[currentPerceptionProfile]
					.mAllPerceptionElements[i].checkAndReturnBias( (i == mAllPerceptionProfiles[currentPerceptionProfile].mAllPerceptionElements.Count ? FPSCamera : PlayerAccess.Self.Player ),this.gameObject);
				
				// If the check was successful
				if(checkResultAndBias.Key == true)
				{
					// Adjust resistance
					mResistance += checkResultAndBias.Value;				
				}
				
				// Indicate the last checks success value
				lastCheckSuccessValue = checkResultAndBias.Key;
			}

			// If this is the LAST , i.e. Deciding element in the list then Check success and adjust Voltage 
			if(i == (mAllPerceptionProfiles[currentPerceptionProfile].mAllPerceptionElements.Count - 1) )
			{

//					lineRenderer.SetPosition(0,this.gameObject.transform.position);
//					lineRenderer.SetPosition(1,FPSCamera.transform.position);

				// If the last check was successful
				if(lastCheckSuccessValue)
					{// Indicate that the Charge needs to rise
						mVoltageBias = 1;
						thieflastposition = PlayerAccess.Self.Player.transform.localPosition;
					}
				// If the last check was UNsuccessful
				else
				{
					// And the charge is more than 0
					if(Charge > 0)
					{
						// Reduce it
						mVoltageBias = -1;

						// set resistance to a flat reduce amount 
						mResistance = 8;
					}
					// Otherwise
					else
						{		
							// Make sure that the charge doesnt go below zero 
							mCharge = 0;
							
							// Make sure that the charge isnt altered
							mVoltageBias = 0;
						}
				}
			}
		}

		
		// Calculate the charge
		mCharge += Time.deltaTime * (( mVoltageBias * mVoltage ) / (mResistance > 0 ? mResistance : 1));

			if(clearToChargeThreatMeter)
			{
				if(mCharge >=1000)
				{
					NetworkManager.Manager.BoostAlertLevelForTime( 0.25f, ThiefManager.Manager.AlertDamage );
					clearToChargeThreatMeter = false;
				}
			}else
			{
				if(mCharge <= 0)
					clearToChargeThreatMeter = true;
			}

		
//		if(mPrevCharge < 1000 && mCharge >= 1000)
//			{
//				Debug.Log(" TIME **************************** ADDED ");
//				//This should stop the trolling. Threat increases when the Alert meter is full.
//				NetworkManager.Manager.BoostAlertLevelForTime( 0.25f, 10 );
//			}

		// Bounds check
		if(mCharge >= 1000)
		{
			mCharge = 1000;
		}
		else if(mCharge < 0)
			mCharge = 0;


		mPrevCharge = Charge;

#if _PERCEPTION_DEBUG_SPEW
		Debug.Log("************************************");
		Debug.Log("CHARGE : " + mCharge);
		Debug.Log("Voltage Bias : " + mVoltageBias);
		Debug.Log("Voltage : " + mVoltage);
		Debug.Log("Resistance : " + mResistance);
		Debug.Log("************************************");
#endif

		if (Input.GetKeyDown(KeyCode.F10))
		{
			currentPerceptionProfile++;
			currentPerceptionProfile = currentPerceptionProfile%mAllPerceptionProfiles.Count;

			#if _PERCEPTION_DEBUG_SPEW
				Debug.Log("PERCEPTION PROFILE CHANGED TO " +  currentPerceptionProfile);
			#endif
		}

		}

		if (Input.GetKeyDown(KeyCode.F11))
		{
			enabled = !enabled;
		}

	}
}

