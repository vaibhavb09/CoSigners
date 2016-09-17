using UnityEngine;
using System.Collections;

public class TimerManager : MonoBehaviour {
		
	#region
	public GameObject CapturingTimerPrefab;
	public GameObject ReleasingTimerPrefab;
	public GameObject EncryptingTimerPrefab;
	public GameObject TrackingTimerPrefab;
	#endregion
	
	#region Singleton Declaration
	
	private static TimerManager m_instance;
	
	public static TimerManager Manager
	{
		get
		{
			if(m_instance == null)
			{
				m_instance = new TimerManager();			
			}
			return m_instance;
		}
	}
	
	public TimerManager () 
    { 
        m_instance = this;
    }
	
	#endregion
	
	#region public interface
	
	public void CreateTimerForNode(Node i_node, GameManager.TimerType i_type)
	{
		/* don't need this anymore
		switch(i_type)
		{
		case GameManager.TimerType.Capture:
			GameObject captureTimer = (GameObject)Instantiate(CapturingTimerPrefab, i_node.gameObject.transform.position + Vector3.down, Quaternion.identity);
			captureTimer.GetComponent<SuperAwesomeTimer>().SetCountDownTime(i_node.TimeToCapture);
			captureTimer.GetComponent<SuperAwesomeTimer>().SetLinkedNode(i_node);
			captureTimer.GetComponent<SuperAwesomeTimer>().SetType(GameManager.TimerType.Capture);
			i_node.SetCaptureTimer(captureTimer);		
			break;
		case GameManager.TimerType.Encrypt:
			GameObject encryptTimer = (GameObject)Instantiate(EncryptingTimerPrefab, i_node.gameObject.transform.position + Vector3.down, Quaternion.identity);
			encryptTimer.GetComponent<SuperAwesomeTimer>().SetCountDownTime(i_node.TimeToEncrypt);
			encryptTimer.GetComponent<SuperAwesomeTimer>().SetLinkedNode(i_node);
			encryptTimer.GetComponent<SuperAwesomeTimer>().SetType(GameManager.TimerType.Encrypt);
			i_node.SetEncryptTimer(encryptTimer);
			break;
		case GameManager.TimerType.Release:
			GameObject releasingTimer = (GameObject)Instantiate(ReleasingTimerPrefab, i_node.gameObject.transform.position + Vector3.down, Quaternion.identity);
			releasingTimer.GetComponent<SuperAwesomeTimer>().SetCountDownTime(i_node.TimeToRelease);
			releasingTimer.GetComponent<SuperAwesomeTimer>().SetLinkedNode(i_node);
			releasingTimer.GetComponent<SuperAwesomeTimer>().SetType(GameManager.TimerType.Release);
			i_node.SetReleaseTimer(releasingTimer);
			break;
		case GameManager.TimerType.Tracking:
			GameObject trackingTimer = (GameObject)Instantiate(TrackingTimerPrefab, i_node.gameObject.transform.position + Vector3.down, Quaternion.identity);
			if(GraphManager.Manager.JumpTime != 0)
				trackingTimer.GetComponent<SuperAwesomeTimer>().SetCountDownTime(GraphManager.Manager.JumpTime);
			else
				trackingTimer.GetComponent<SuperAwesomeTimer>().SetCountDownTime(5.0f);
				trackingTimer.GetComponent<SuperAwesomeTimer>().SetLinkedNode(i_node);
				trackingTimer.GetComponent<SuperAwesomeTimer>().SetType(GameManager.TimerType.Tracking);
				i_node.SetTrackingTimer(trackingTimer);
			break;
		}
		*/
	}
			
	#endregion
}
