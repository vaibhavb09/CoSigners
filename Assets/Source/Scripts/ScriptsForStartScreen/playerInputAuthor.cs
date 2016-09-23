using UnityEngine;
using System.Collections;

public class playerInputAuthor : Photon.MonoBehaviour {
	
	public PhotonPlayer Owner;
	float lastClientHInput = 0f;
	float lastClientVInput = 0f;
	float serverCurrentHInput = 0f;
	float serverCurrentVInput = 0f;
	
	void Awake()
	{
		if(PhotonNetwork.isNonMasterClientInRoom)
			enabled = false;
	}
	
	[PunRPC]
	void SetPlayer(PhotonPlayer player)
	{
		Owner = player;
		if(player == PhotonNetwork.player)
			enabled = true;
	}
	
	void Update()
	{
		if((Owner != null) && (PhotonNetwork.player == Owner))
		{
			float HInput = Input.GetAxis("Horizontal");
			float VInput = Input.GetAxis("Vertical");
			
			if(lastClientHInput!= HInput || lastClientVInput != VInput)
			{
				lastClientHInput = HInput;
				lastClientVInput = VInput;
				if(PhotonNetwork.isMasterClient)
				{
					SendMovementInput(HInput, VInput);
				}
				else if(PhotonNetwork.isNonMasterClientInRoom)
				{
					photonView.RPC("SendMovementInput", PhotonTargets.MasterClient, HInput, VInput);
				}
			}
		}
		
		if(PhotonNetwork.isMasterClient)
		{
			Vector3 moveDirection = new Vector3(serverCurrentHInput, 0, serverCurrentVInput);
			float speed =5;
			transform.Translate(speed * moveDirection * Time.deltaTime);
		}
	}
	
	[PunRPC]
	void SendMovementInput(float HInput, float VInput)
	{
		serverCurrentHInput = HInput;
		serverCurrentVInput = VInput;
	}
	
	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{	
		if(stream.isWriting)
		{
			Vector3 pos = transform.position;
			stream.Serialize(ref pos);
		}
		else
		{
			Vector3 posReceive = Vector3.zero;
			stream.Serialize(ref posReceive);
			transform.position = posReceive;
		}
	}
	
	void OnDisconnectedFromServer(NetworkDisconnection info) {
        if (PhotonNetwork.isMasterClient)
		{
            //Debug.Log("Local server connection disconnected");
		}
        else
		{
            if (info == NetworkDisconnection.LostConnection)
			{
                //Debug.Log("Lost connection to the server");
			}
            else
			{
                //Debug.Log("Successfully diconnected from the server");
			}
		}
		
		PhotonNetwork.RemoveRPCs(PhotonNetwork.player);
        PhotonNetwork.DestroyPlayerObjects(PhotonNetwork.player);
		Destroy(gameObject);
    }
}
