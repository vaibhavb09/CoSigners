using UnityEngine;
using System.Collections;

public class SecurityAccessPanel : MonoBehaviour {

	public Material activatedMat;
	public Material deactivatedMat;
	public Material lockdownMat;
	public Transform SAP_Antenna;
	public float cooldownTimeinSeconds;
	public float throwOffset;
	
	private bool activated;
	private bool lockdown;
	private float startTime;
	private float elapsedTime;
	private Transform screen1;
	private Transform screen2;
	
#if UNITY_IPHONE
	
	void Start(){}
	void Update(){}
	public void ActivatePanel(){}
	
#else
	
	// Use this for initialization
	void Start () 
	{
		startTime = 0.0f;
		elapsedTime = 0.0f;
		activated = false;
		lockdown = false;
		screen1 = transform.FindChild("SAP_Screen_Front");
		screen2 = transform.FindChild("SAP_Screen_Front1");
		screen2.renderer.enabled = false;
	}
	
	void ThrowAntenna()
	{
		Vector3 startPoint = transform.position + (transform.forward * throwOffset);
		Quaternion angle = Quaternion.LookRotation( transform.forward );
		Instantiate( SAP_Antenna, startPoint, angle );
	}
	
	void ChangeScreenMaterial( Material mat )
	{
		screen1.renderer.material = mat;
		//screen2.renderer.material = mat;
	}
	
	#region public interface
	
	public void ActivatePanel()
	{
		if( !activated && !lockdown )
		{
			startTime = Time.time;
			activated = true;
			ChangeScreenMaterial( activatedMat );
			ThrowAntenna();
			NetworkManager.Manager.ActivateSecurityPanel();
			
			// [ SOUND TAG ] [SAP_Interaction] [SAP_Screen prefab]
			soundMan.soundMgr.playOneShotOnSource(this.audio,"SAP_Interaction",GameManager.Manager.PlayerType);
		}
	}
	
	public void ActivateLockdownState()
	{
		lockdown = true;
		ChangeScreenMaterial( lockdownMat );
	}
	
	public void ReturnToOriginalState()
	{
		lockdown = false;
		if( activated )
			ChangeScreenMaterial( activatedMat );
		else
			ChangeScreenMaterial( deactivatedMat );
	}
	
	#endregion
	
	// Update is called once per frame
	void Update () 
	{
		if( activated )
		{
			elapsedTime = Time.time - startTime;
			if( elapsedTime >= cooldownTimeinSeconds )
			{
				//DeactivatePanel();
			}
		}
		
	}
	
#endif

}


