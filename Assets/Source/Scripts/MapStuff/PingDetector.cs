using UnityEngine;
using System.Collections;

public class PingDetector : MonoBehaviour {
	
	private GameObject clickedObject;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	//void OnMouseDown () {
		void Update () {
			if(Input.GetMouseButtonDown(1)) // Only Hacker can ping
			{		
				if(GameManager.Manager.PlayerType == 2 || GameManager.Manager.PlayerType == 0)
				{	
					//Debug.Log("Hacker Ping Right Click Detected");
					Ray ray;
					RaycastHit hit;
					ray = Camera.main.ScreenPointToRay(Input.mousePosition);
					if(Physics.Raycast(ray, out hit, 1000))
					{
						//get the position of the POI
						Vector3 location = hit.point;
						clickedObject = hit.transform.gameObject;
				
						//Debug.Log("Hacker Ping Right Click on " + clickedObject.name + "with tag" +clickedObject.tag+" at hit position : " +hit.point + " at object position : " + clickedObject.transform.position );
				
						if ((clickedObject.tag == "HackerCanopy"))
						{
							//Debug.Log("Hacker Ping Right Click on Canopy Detected");
							PingSystem.PSystem.CreatePing(clickedObject.transform,hit.point);
						}
					}
				}	
		    }
	}
}
