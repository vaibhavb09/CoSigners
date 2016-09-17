using UnityEngine;
using System.Collections;
using System;

public class PlayerInitialize : MonoBehaviour {
	
	
	public Transform playerPrefab;
	public Transform player2DPrefab;
	public Transform NetworkManagerPrefab;
	public Transform StartPoint;
	public int sensitivityX = 7;
	public int sensitivityY = 7;
	private GameObject _camera;
	
	private ArrayList playerScripts = new ArrayList();
	
	void Awake()
	{
		
	}
	// Use this for initialization
	void Start () 
	{	
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	public void InitPlayer()
	{
		//Debug.Log("#Max:Init Player Called");

		GameManager.Manager._currentInfoData.m_currentMovie = "";
		GameManager.Manager._currentInfoData.m_hackerReady = false;
		GameManager.Manager._currentInfoData.m_thiefReady = false;
		
		//if(GameManager.Manager.PlayerType == 1) //prisoner
		{
			SpawnPrisoner(Network.player);
		}
		//if(GameManager.Manager.PlayerType == 2) //hacker
		{
			SpawnHacker(Network.player);
		}
	}

	void SpawnPrisoner(NetworkPlayer i_player)
	{
		//Debug.Log("#Max:Spawned player" + i_player.ToString());
		SpawnPlayer(i_player);
		InitCamera();
	}

	void SpawnHacker(NetworkPlayer i_player)
	{
		string tempPlayerString = i_player.ToString();
		int playerNumber =  Convert.ToInt32(tempPlayerString);
		Transform newPlayer = (Transform)Instantiate(player2DPrefab,
			new Vector3(StartPoint.transform.position.x, 60, StartPoint.transform.position.z), transform.rotation);
		InitCamera();
	}
	
	void InitCamera()
	{
		if(GameManager.Manager.PlayerType == 1) // is thief
		{
			_camera = GameObject.Find("FPSCamera");
			_camera.camera.enabled = true;
			//Debug.Log("Loaded Level =" + Application.loadedLevelName);
			if( Application.loadedLevelName == "JM_53")
			{
				//Debug.Log("Setting VertexLit path jm_53");
				_camera.camera.renderingPath = RenderingPath.VertexLit;
			}
			GameObject _light = GameObject.Find("Hacker_Light");
			_light.light.enabled = false;
		}
		else if(GameManager.Manager.PlayerType == 2) // is a hacher
		{
			_camera = GameObject.Find("TopDownCamera");
			_camera.camera.enabled = true;
			GameObject _light = GameObject.Find("Thief_Light");
			_light.light.enabled = false;
			
		}
		else if(GameManager.Manager.PlayerType == 3) //is a observer
		{
			_camera = GameObject.Find("ObserveCamera");
			_camera.camera.enabled = true;
			
		}
	}
	
	void SpawnPlayer(NetworkPlayer player)
	{
		string tempPlayerString = player.ToString();
		int playerNumber =  Convert.ToInt32(tempPlayerString);
		Transform newPlayer;
		GameObject[] startDoor = PointmanNetManager.Manager.GetDoorsOfType(DoorType.StartDoor);

		if( startDoor != null && startDoor.Length != 0 )
		{ 
			Quaternion startDoorRotation = startDoor[0].transform.rotation;
			newPlayer = (Transform)Instantiate(playerPrefab, StartPoint.transform.position, startDoorRotation);
		}
		else
		{
			newPlayer = (Transform)Instantiate(playerPrefab, StartPoint.transform.position, StartPoint.transform.rotation);
		}
	}
	
	void OnPlayerDisconnected(NetworkPlayer player) {
        //Debug.Log("Clean up after player " + player);

        Network.RemoveRPCs(player);
        Network.DestroyPlayerObjects(player);
    }
}
