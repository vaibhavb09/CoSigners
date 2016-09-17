using UnityEngine;
using System.Collections;

public class playerInputAuthor : MonoBehaviour {
	
	public NetworkPlayer Owner;
	float lastClientHInput = 0f;
	float lastClientVInput = 0f;
	float serverCurrentHInput = 0f;
	float serverCurrentVInput = 0f;
	
	void Awake()
	{
		if(Network.isClient)
			enabled = false;
	}
	
	[RPC]
	void SetPlayer(NetworkPlayer player)
	{
		Owner = player;
		if(player == Network.player)
			enabled = true;
	}
	
	void Update()
	{
		if((Owner != null) && (Network.player == Owner))
		{
			float HInput = Input.GetAxis("Horizontal");
			float VInput = Input.GetAxis("Vertical");
			
			if(lastClientHInput!= HInput || lastClientVInput != VInput)
			{
				lastClientHInput = HInput;
				lastClientVInput = VInput;
				if(Network.isServer)
				{
					SendMovementInput(HInput, VInput);
				}
				else if(Network.isClient)
				{
					networkView.RPC("SendMovementInput", RPCMode.Server, HInput, VInput);
				}
			}
		}
		
		if(Network.isServer)
		{
			Vector3 moveDirection = new Vector3(serverCurrentHInput, 0, serverCurrentVInput);
			float speed =5;
			transform.Translate(speed * moveDirection * Time.deltaTime);
		}
	}
	
	[RPC]
	void SendMovementInput(float HInput, float VInput)
	{
		serverCurrentHInput = HInput;
		serverCurrentVInput = VInput;
	}
	
	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
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
        if (Network.isServer)
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
		
		Network.RemoveRPCs(Network.player);
        Network.DestroyPlayerObjects(Network.player);
		Destroy(gameObject);
    }
}
